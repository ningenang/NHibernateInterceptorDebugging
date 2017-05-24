using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace NHibernateCleanupModule
{
	/// <summary>
	/// When an ASP.NET State Server other than InProc, the Session_End event
	/// does not get fired.  This is an HttpModule which uses some workarounds
	/// and fires a static event when a session ends, with the value of a single
	/// configurable session variable in the event arguments.
	/// </summary>
	public class SessionEndModule : IHttpModule
	{
		#region Private Variables

		private HttpApplication httpApplication;
		private static string sessionObjectKey;

		#endregion

		#region Accessors

		/// <summary>
		/// This is the key of the item in the session which should be returned
		/// in the SessionEnd event (as the SessionObject).
		/// </summary>
		/// <example>
		///	If you're storing the user ID in the session, under a key called 'UserId'
		/// and need to do something with it in the SessionEnd event, you would set
		/// this to 'UserId', which would cause the value of the session key called
		/// 'UserId' to be returned.
		/// </example>
		public static string SessionObjectKey
		{
			get
			{
				return sessionObjectKey;
			}
			set
			{
				sessionObjectKey = value;
			}
		}

		#endregion

		#region IHttpModule Implementation

		public virtual void Init(HttpApplication context)
		{
			httpApplication = context;
			httpApplication.PreRequestHandlerExecute += (sender, e) =>
			{
				// We only want to update the session when an ASPX page is being viewed
				// We're also doing this in the PreRequestHandler, as doing it elsewhere
				// (like the PostRequestHandler) can cause some strange behaviour.

				if (Path.GetExtension(httpApplication.Context.Request.Path).ToLower() == ".aspx"
					&& HttpContext.Current != null && HttpContext.Current.Session != null)
				{
					var currentSession = HttpContext.Current.Session;

					// Get the session timeout
					TimeSpan sessionTimeout = new TimeSpan(0, 0, currentSession.Timeout, 0, 0);

					// Get the object in the session we want to retrieve when the session times out
					object sessionObject = currentSession[SessionObjectKey];

					if (sessionObject != null)
					{

						// Add the object to the cache with the current session id, and set a cache removal callback method
						HttpContext.Current.Cache.Insert(currentSession.SessionID, sessionObject, null,
							DateTime.MaxValue, sessionTimeout, CacheItemPriority.NotRemovable,
							new CacheItemRemovedCallback((string key, object value, CacheItemRemovedReason reason) =>
							{
								if (reason == CacheItemRemovedReason.Expired)
								{
									if (SessionEnd != null)
									{
										SessionEndedEventArgs args = new SessionEndedEventArgs(key, value);
										SessionEnd(this, args);
									}
								}

							}));
					}
				}

			};
		}

		public void Dispose()
		{
			// Do Nothing
		}

		#endregion

		#region Public events

		/// <summary>
		/// Event raised when the session ends
		/// </summary>
		public static event EventHandler<SessionEndedEventArgs> SessionEnd;

		#endregion
	}

	/// <summary>
	/// SessionEndedEventArgs for use in the SessionEnd event
	/// </summary>
	public class SessionEndedEventArgs : EventArgs
	{
		public readonly string SessionId;
		public readonly object SessionObject;

		public SessionEndedEventArgs(string sessionId, object sessionObject)
		{
			SessionId = sessionId;
			SessionObject = sessionObject;
		}
	}
}

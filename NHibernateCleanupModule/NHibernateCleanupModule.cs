using DAL.SessionManagement;
using log4net;
using System;
using System.Web;
using System.Web.SessionState;

namespace NHibernateCleanupModule
{
	public class NHibernateCleanupModule : SessionEndModule, IRequiresSessionState
	{
		private static ILog log = LogManager.GetLogger(nameof(NHibernateCleanupModule));

		public override void Init(HttpApplication context)
		{
			base.Init(context);

			// The session key is stored in the HTTP Session
			if (SessionManager.Instance.SessionKeyStorageType == SessionKeyStorageTypeEnum.HTTPSESSION)
			{
				CleanupHttpSession(context);
			}

			// The session key is stored in the HTTP context
			else if (SessionManager.Instance.SessionKeyStorageType == SessionKeyStorageTypeEnum.HTTPCONTEXT)
			{
				CleanupHttpContext(context);
			}
		}

		private static void CleanupHttpContext(System.Web.HttpApplication context)
		{
			context.EndRequest += (sender, args) =>
			{
				if (sender as HttpApplication != null)
				{
					// Only dispose if it exists
					if (SessionManager.Instance.SessionStore.ContainsKey(SessionManager.Instance.DefaultSessionKey))
					{
						SessionManager.Instance.DisposeSession(SessionManager.Instance.DefaultSessionKey);
					}
				}
			};
		}

		private static void CleanupHttpSession(System.Web.HttpApplication context)
		{
			// Get a reference to the session module
			IHttpModule module = context.Modules["Session"];

			if (module != null && module.GetType() == typeof(SessionStateModule))
			{
				SessionStateModule stateModule = (SessionStateModule)module;
				stateModule.Start += (sender, args) =>
				{
					// Ensure that expired NHibernate sessions are disposed
					SessionManager.Instance.GetSession(SessionManager.Instance.DefaultSessionKey);
					SessionEndModule.SessionObjectKey = SessionManager.SessionKeyIdentifier;
					EventHandler<SessionEndedEventArgs> sessionEndHandler = null;

					sessionEndHandler = (s, a) =>
					{
						SessionManager.Instance.DisposeSession((Guid)(a.SessionObject));
						SessionEndModule.SessionEnd -= sessionEndHandler;
					};

					SessionEndModule.SessionEnd += sessionEndHandler;
				};
			}
		}





	}

}

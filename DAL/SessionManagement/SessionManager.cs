using NHibernate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace DAL.SessionManagement
{
	/// <summary>
	/// Singleton class for managing NHibernate Sessions. The core functionality is the SessionStore Dictionary 
	/// that stores ISessions with a <see cref="Guid"/> key. Methods for managing the sessions are also included.
	/// </summary>
	public class SessionManager
	{
		public const string SessionKeyIdentifier = "GUID_SESSION_KEY";

		private readonly object syncRoot = new object();

		/// <summary>Default session key for current thread.</summary>
		private ThreadLocal<Guid> threadLocalSessionKey;

		/// <summary>
		/// Static session key for singleton sessions
		/// </summary>
		private static readonly Guid singletonSessionKey = Guid.NewGuid();

		/// <summary>
		/// Session key storage type
		/// </summary>
		private SessionKeyStorageTypeEnum _sessionKeyStorageType;

		/// <summary>
		/// Prevents a default instance of the <see cref="SessionManager"/> class from being created.
		/// </summary>
		private SessionManager()
		{

			// Determine and assign session key storage type to this session manager
			this.SessionKeyStorageType = SessionConfiguration.SessionKeyStorageType;

			// Create session store
			SessionStore = new Dictionary<Guid, ISession>();

			if (this.SessionKeyStorageType == SessionKeyStorageTypeEnum.THREADLOCAL)
			{
				// Create thread local session key
				threadLocalSessionKey = new ThreadLocal<Guid>();
			}
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <value>The singleton instance.</value>
		/// <remarks>
		/// This is a thread-safe, lazy singleton. See http://www.yoda.arachsys.com/csharp/singleton.html
		/// for more details about its implementation.
		/// </remarks>
		public static SessionManager Instance
		{
			get
			{
				return Nested.SessionManager;
			}
		}

		/// <summary>
		/// Gets the default session key.
		/// </summary>
		/// <value>The default session key.</value>
		public Guid DefaultSessionKey
		{
			get
			{
				if (!HasDefaultSessionKey())
				{
					SetDefaultSessionKey(Guid.NewGuid());
				}
				return GetDefaultSessionKey();
			}
		}

		/// <summary>
		/// Gets or sets the session store.
		/// </summary>
		/// <value>The session store.</value>
		public IDictionary<Guid, ISession> SessionStore
		{
			get;
			private set;
		}

		/// <summary>
		/// The type of session storage for this session
		/// </summary>
		public SessionKeyStorageTypeEnum SessionKeyStorageType
		{
			get { return _sessionKeyStorageType; }
			set { _sessionKeyStorageType = value; }
		}

		/// <summary>
		/// Gets the default <see cref="ISession"/>.
		/// </summary>
		/// <returns>An <see cref="ISession"/> for quering the data store.</returns>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public ISession GetSession()
		{
			return GetSession(DefaultSessionKey);
		}

		/// <summary>
		/// Gets an <see cref="ISession"/> associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key</param>
		/// <returns>An <see cref="ISession"/> for quering the data store.</returns>
		public ISession GetSession(Guid sessionKey)
		{
			lock (syncRoot)
			{

				if (SessionStore.ContainsKey(sessionKey))
				{
					// Re-initiate session if it is in an unusable state
					if (SessionStore[sessionKey] == null)
					{
						SessionStore[sessionKey] = SessionFactory.Instance.OpenSession();
					}
					else if (SessionStore[sessionKey].IsOpen == false)
					{
						SessionStore[sessionKey].Dispose();
						SessionStore[sessionKey] = SessionFactory.Instance.OpenSession();
					}

					return SessionStore[sessionKey];
				}

				// Open new session, add it to SessionStore and return the session
				ISession session = SessionFactory.Instance.OpenSession();
				SessionStore.Add(sessionKey, session);

				return session;
			}
		}

		/// <summary>
		/// Clears the contents of the default session.
		/// </summary>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void ClearSession()
		{
			ClearSession(DefaultSessionKey);
		}

		/// <summary>
		/// Clears the contents of the session associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		public void ClearSession(Guid sessionKey)
		{
			if (SessionStore.ContainsKey(sessionKey))
			{
				if (SessionStore[sessionKey] != null)
				{
					SessionStore[sessionKey].Clear();
				}
			}
		}

		/// <summary>
		/// Disposes the default session.
		/// </summary>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void DisposeSession()
		{
			DisposeSession(DefaultSessionKey);
		}

		/// <summary>
		/// Disposes the session associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		public void DisposeSession(Guid sessionKey)
		{
			lock (syncRoot)
			{
				if (SessionStore.ContainsKey(sessionKey))
				{
					// Dispose session if it exists
					if (SessionStore[sessionKey] != null)
					{
						SessionStore[sessionKey].Dispose();
					}

					SessionStore.Remove(sessionKey);
				}
			}
		}

		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void FlushSession()
		{
			FlushSession(DefaultSessionKey);
		}

		/// <summary>
		/// Flush the session associated with the given session key.
		/// </summary>
		public void FlushSession(Guid sessionKey)
		{
			if (SessionStore.ContainsKey(sessionKey))
			{
				// Dispose session if it exists
				if (SessionStore[sessionKey] != null)
				{
					SessionStore[sessionKey].Flush();
				}
			}
		}

		/// <summary>
		/// Begins a transaction on the default session.
		/// </summary>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void BeginTransaction()
		{
			BeginTransaction(DefaultSessionKey);
		}

		/// <summary>
		/// Begins a transaction on the session associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		public void BeginTransaction(Guid sessionKey)
		{
			GetSession(sessionKey).Transaction.Begin();
		}

		/// <summary>
		/// Commits the transaction on the default session.
		/// </summary>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void CommitTransaction()
		{
			CommitTransaction(DefaultSessionKey);
		}

		/// <summary>
		/// Commits the transaction on the session associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		public void CommitTransaction(Guid sessionKey)
		{
			GetSession(sessionKey).Transaction.Commit();
		}

		/// <summary>
		/// Rolls back the transaction on the default session.
		/// </summary>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void RollbackTransaction()
		{
			RollbackTransaction(DefaultSessionKey);
		}

		/// <summary>
		/// Rolls back the transaction on the session associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		public void RollbackTransaction(Guid sessionKey)
		{
			GetSession(sessionKey).Transaction.Rollback();
		}

		/// <summary>
		/// Determines whether the default session has an open transaction.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the default session has an open transaction; otherwise, <c>false</c>.
		/// </returns>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public bool HasOpenTransaction()
		{
			return HasOpenTransaction(DefaultSessionKey);
		}

		/// <summary>
		/// Determines whether the session associated with the specified session key has an open transaction.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		/// <returns>
		/// 	<c>true</c> if the session associated with the specified session key has an open transaction;
		/// 	otherwise, <c>false</c>.
		/// </returns>
		public bool HasOpenTransaction(Guid sessionKey)
		{
			ISession session = GetSession(sessionKey);
			if (session.Transaction != null && session.Transaction.IsActive)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Removes <paramref name="entity"/> from the cache of the default session.
		/// </summary>
		/// <param name="entity">The entity to evict.</param>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void Evict(object entity)
		{
			Evict(DefaultSessionKey, entity);
		}

		/// <summary>
		/// Removes <paramref name="entity"/> from the cache of the session associated with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		/// <param name="entity">The entity to evict.</param>
		public void Evict(Guid sessionKey, object entity)
		{
			ISession session = GetSession(sessionKey);
			if (session.Contains(entity))
			{
				session.Evict(entity);
			}
		}

		/// <summary>
		/// Re-reads the state of <paramref name="entity"/> from the underlying database using the default session.
		/// </summary>
		/// <param name="entity">The entity to refresh.</param>
		[Obsolete("Method is provided for compatability reasons only. The SessionKey overload should be used instead.")]
		public void Refresh(object entity)
		{
			Refresh(DefaultSessionKey, entity);
		}

		/// <summary>
		/// Re-reads the state of <paramref name="entity"/> from the underlying database using the session associated
		/// with the specified session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		/// <param name="entity">The entity to refresh.</param>
		public void Refresh(Guid sessionKey, object entity)
		{
			ISession session = GetSession(sessionKey);
			if (session.Contains(entity))
			{
				session.Refresh(entity);
			}
		}

		/// <summary>
		/// Determines whether a default session key has been set.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if a default session key has been set; otherwise, <c>false</c>.
		/// </returns>
		private bool HasDefaultSessionKey()
		{
			switch (this.SessionKeyStorageType)
			{
				case SessionKeyStorageTypeEnum.HTTPSESSION:
					return HttpContext.Current.Session == null || HttpContext.Current.Session[SessionKeyIdentifier] != null;

				case SessionKeyStorageTypeEnum.HTTPCONTEXT:
					return HttpContext.Current == null || HttpContext.Current.Items[SessionKeyIdentifier] != null;

				case SessionKeyStorageTypeEnum.THREADLOCAL:
					return threadLocalSessionKey.IsValueCreated;

				case SessionKeyStorageTypeEnum.STATIC:
					return singletonSessionKey != null && singletonSessionKey != default(Guid);

				case SessionKeyStorageTypeEnum.NONE:
					return true;

				default:
					throw new Exception("SessionManager incorrectly configured");
			}
		}

		/// <summary>
		/// Gets the default session key.
		/// </summary>
		/// <returns>The default session key.</returns>
		private Guid GetDefaultSessionKey()
		{
			switch (this.SessionKeyStorageType)
			{
				case SessionKeyStorageTypeEnum.HTTPSESSION:
					if (HttpContext.Current.Session == null)
					{
						return Guid.NewGuid();
					}
					else
					{
						return (Guid)HttpContext.Current.Session[SessionKeyIdentifier];
					}

				case SessionKeyStorageTypeEnum.HTTPCONTEXT:
						return (Guid)HttpContext.Current.Items[SessionKeyIdentifier];

				case SessionKeyStorageTypeEnum.THREADLOCAL:
					return threadLocalSessionKey.Value;

				case SessionKeyStorageTypeEnum.STATIC:
					return singletonSessionKey;

				case SessionKeyStorageTypeEnum.NONE:
					return Guid.NewGuid();

				default:
					throw new Exception("SessionManager incorrectly configured");
			}
		}

		/// <summary>
		/// Sets the default session key.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		private void SetDefaultSessionKey(Guid sessionKey)
		{
			switch (this.SessionKeyStorageType)
			{
				case SessionKeyStorageTypeEnum.HTTPSESSION:
					
					if (HttpContext.Current.Session != null)
					{
						HttpContext.Current.Session[SessionKeyIdentifier] = sessionKey;
					}

					break;

				case SessionKeyStorageTypeEnum.HTTPCONTEXT:

					HttpContext.Current.Items[SessionKeyIdentifier] = sessionKey;

					break;

				case SessionKeyStorageTypeEnum.THREADLOCAL:

					threadLocalSessionKey.Value = sessionKey;
					break;

				case SessionKeyStorageTypeEnum.STATIC:

					// Already assigned to in the variable initializer
					break;

				case SessionKeyStorageTypeEnum.NONE:

					// Never assigned to since we always use a new Guid
					break;

				default:
					throw new Exception("SessionManager incorrectly configured");
			}
		}

		/// <summary>
		/// Assists with ensuring thread-safe, lazy singleton
		/// </summary>
		private class Nested
		{
			internal static readonly SessionManager SessionManager = new SessionManager();
		}
	}
}

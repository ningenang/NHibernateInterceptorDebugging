using NHibernate;
using NHibernate.Cfg;
using System;

namespace DAL.SessionManagement
{
	/// <summary>
	/// Singleton class containing the NHibernate SessionFactory. Keeps a single instance of the expensive 
	/// <see cref="ISessionFactory"/> object. Provides methods relevant to SessionFactory usage, such as OpenSession.
	/// </summary>
	public class SessionFactory
	{
		private ISessionFactory sessionFactory;

		public static IInterceptor SessionFactoryInterceptor;

		public static event EventHandler<SessionFactoryConfiguringEventArgs> SessionFactoryConfiguring;

		/// <summary>
		/// Prevents a default instance of the <see cref="SessionFactory"/> class from being created.
		/// </summary>
		private SessionFactory()
		{
			Init();
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <value>The singleton instance.</value>
		/// <remarks>
		/// This is a thread-safe, lazy singleton. See http://www.yoda.arachsys.com/csharp/singleton.html
		/// for more details about its implementation.
		/// </remarks>
		public static SessionFactory Instance
		{
			get
			{
				return Nested.SessionFactory;
			}
		}

		private static readonly object _lock = new object();

		/// <summary>
		/// Opens a new session from the NHibernate SessionFactory.
		/// </summary>
		/// <returns>The newly opened session.</returns>
		public ISession OpenSession()
		{
			lock (_lock)
			{
				var session = sessionFactory.OpenSession();

				ApplyDeadlockPriority(ApplyFlushMode(session));

				return session;
			}
		}

		/// <summary>
		/// Apply configured application deadlock priority if any
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		private ISession ApplyDeadlockPriority(ISession session)
		{
			string priority;

			if (!string.IsNullOrWhiteSpace(priority = SessionConfiguration.DeadlockPriority))
			{
				session.CreateSQLQuery(string.Format("SET DEADLOCK_PRIORITY {0}", priority)).ExecuteUpdate();
			}

			return session;
		}

		/// <summary>
		/// Apply configured application flush mode if any
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		private ISession ApplyFlushMode(ISession session)
		{
			if (session != null)
			{
				session.FlushMode = SessionConfiguration.FlushMode;
			}

			return session;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private void Init()
		{
			try
			{
				var configuration = new Configuration();

				if (SessionFactoryInterceptor != null)
					configuration.SetInterceptor(SessionFactoryInterceptor);

				configuration.Configure();
				OnSessionFactoryConfiguring(configuration);
				sessionFactory = configuration.BuildSessionFactory();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				while (ex.InnerException != null)
				{
					Console.Error.WriteLine(ex.Message);
					ex = ex.InnerException;
				}
				throw;
			}
		}

		private void OnSessionFactoryConfiguring(Configuration configuration)
		{
			if (SessionFactoryConfiguring != null)
			{
				SessionFactoryConfiguring(this, new SessionFactoryConfiguringEventArgs(configuration));
			}
		}


		public class SessionFactoryConfiguringEventArgs : EventArgs
		{
			public Configuration Configuration { get; private set; }

			public SessionFactoryConfiguringEventArgs(Configuration configuration)
			{
				Configuration = configuration;
			}
		}

		public class SessionFactoryOpenSessionEventArgs : EventArgs
		{

			private NHibernate.ISession session;

			public SessionFactoryOpenSessionEventArgs(NHibernate.ISession session)
			{
				this.session = session;
			}

			public NHibernate.ISession Session
			{
				get
				{
					return this.session;
				}
			}

			public NHibernate.IInterceptor Interceptor
			{
				get;
				set;
			}
		}

		/// <summary>
		/// Assists with ensuring thread-safe, lazy singleton
		/// </summary>
		private class Nested
		{
			internal static readonly SessionFactory SessionFactory;

			static Nested()
			{
				try
				{
					SessionFactory = new SessionFactory();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine(ex);
					throw;
				}
			}
		}
	}
}

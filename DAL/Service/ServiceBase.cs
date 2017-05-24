using DAL.DAO;
using DAL.SessionManagement;
using log4net;
using System;
using System.Collections.Generic;

namespace DAL.Service
{
	/// <summary>
	/// Base class for all services.
	/// </summary>
	/// <typeparam name="T">The DTO type represented by the service.</typeparam>
	public abstract class ServiceBase<T> : IService<T> where T : class
	{

		private static readonly ILog log = LogManager.GetLogger(nameof(ServiceBase<T>));
		/// <summary>
		/// Gets the unique session key for this instance.
		/// </summary>
		/// <value>The unique session key for this instance.</value>
		protected Guid SessionKey { get; private set; }

		/// <summary>
		/// Gets the context DAO.
		/// </summary>
		/// <value>The context DAO.</value>
		protected abstract IDAO<T, int> ContextDAO { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceBase&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="sessionKey">The session key.</param>
		public ServiceBase(Guid sessionKey)
		{
			SessionKey = sessionKey;
			InitializeResources();
		}

		public virtual List<T> GetAll()
		{
			return ContextDAO.GetAll();
		}

		public virtual List<T> GetAllCacheable()
		{
			return ContextDAO.GetAllCacheable();
		}

		public virtual T GetById(int id)
		{
			return ContextDAO.GetById(id);
		}

		public virtual T LoadById(int id)
		{
			return ContextDAO.LoadById(id);
		}

		public virtual T Save(T entity)
		{
			return RunFunctionInTransaction(ContextDAO.Save, entity);
		}

		public virtual T SaveOrUpdate(T entity)
		{
			return RunFunctionInTransaction(ContextDAO.SaveOrUpdate, entity);
		}

		public virtual T Merge(T entity)
		{
			return RunFunctionInTransaction(ContextDAO.Merge, entity);
		}

		public virtual void Delete(T entity)
		{
			RunActionInTransaction(ContextDAO.Delete, entity);
		}

		public virtual void Refresh(T entity)
		{
			SessionManager.Instance.Refresh(SessionKey, entity);
		}

		protected virtual void InitializeResources()
		{
		}

		protected T RunFunctionInTransaction(Func<T, T> action, T entity)
		{
			T result;

			bool transactionExists = SessionManager.Instance.HasOpenTransaction(SessionKey);

			if (!transactionExists)
			{
				SessionManager.Instance.BeginTransaction(SessionKey);
			}

			try
			{
				result = action(entity);

				if (!transactionExists)
				{
					SessionManager.Instance.CommitTransaction(SessionKey);
				}
			}
			catch (Exception ex)
			{
				log.Fatal(ex);

				try
				{
					if (!transactionExists && SessionManager.Instance.HasOpenTransaction(SessionKey))
					{
						SessionManager.Instance.RollbackTransaction(SessionKey);
					}
				}
				finally
				{
					SessionManager.Instance.GetSession(SessionKey).Close();
				}

				throw;
			}

			return result;
		}

		protected void RunActionInTransaction(Action<T> action, T entity)
		{
			bool transactionExists = SessionManager.Instance.HasOpenTransaction(SessionKey);

			if (!transactionExists)
			{
				SessionManager.Instance.BeginTransaction(SessionKey);
			}

			try
			{
				action(entity);

				if (!transactionExists)
				{
					SessionManager.Instance.CommitTransaction(SessionKey);
				}
			}
			catch (Exception ex)
			{
				log.Fatal(ex);
				try
				{
					if (!transactionExists && SessionManager.Instance.HasOpenTransaction(SessionKey))
					{
						SessionManager.Instance.RollbackTransaction(SessionKey);
					}
				}
				finally
				{
					SessionManager.Instance.GetSession(SessionKey).Close();
				}

				throw;
			}
		}
	}
}

using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace DAL.DAO
{
	public abstract class DAOBase<TDTO, TObjectId> : IDAO<TDTO, TObjectId> where TDTO : class
	{
		/// <summary>
		/// Gets or sets the NHibernate session used by the DAO.
		/// </summary>
		/// <value>The NHibernate session used by the DAO.</value>
		protected ISession Session { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the session has an open transaction.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the session has an open transaction; otherwise, <c>false</c>.
		/// </value>
		protected bool SessionHasOpenTransaction
		{
			get
			{
				return Session.Transaction != null && Session.Transaction.IsActive;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DAOBase"/> class.
		/// </summary>
		public DAOBase(ISession session)
		{
			Session = session;
		}

		/// <summary>
		/// Returns all of the objects of the Type
		/// </summary>
		public List<TDTO> GetAll()
		{
			return GetByCriteria();
		}

		/// <summary>
		/// Returns all of the objects of the Type, potentially returning them from the 2nd-level cache.
		/// </summary>
		public List<TDTO> GetAllCacheable()
		{
			return GetByCacheableCriteria();
		}

		/// <summary>
		/// Get an object by its ID
		/// </summary>
		public TDTO GetById(TObjectId id)
		{
			return Session.Get<TDTO>(id);
		}

		/// <summary>
		/// Get an object by its ID
		/// </summary>
		public TDTO LoadById(TObjectId id)
		{
			return Session.Load<TDTO>(id);
		}

		/// <summary>
		/// Get values with criteria
		/// </summary>
		/// <param name="criterion">criteria to get values by</param>
		public List<TDTO> GetByCriteria(params ICriterion[] criterion)
		{
			ICriteria criteria = Session.CreateCriteria<TDTO>();

			foreach (ICriterion criterium in criterion)
			{
				criteria.Add(criterium);
			}

			return new List<TDTO>(criteria.List<TDTO>());
		}

		/// <summary>
		/// Get values with criteria. Also sets the cirteria.SetCacheable to true.
		/// </summary>
		/// <param name="criterion">criteria to get values by</param>
		public List<TDTO> GetByCacheableCriteria(params ICriterion[] criterion)
		{
			ICriteria criteria = Session.CreateCriteria<TDTO>();

			foreach (ICriterion criterium in criterion)
			{
				criteria.Add(criterium);
			}

			criteria.SetCacheable(true);
			criteria.SetCacheRegion("ReadOnlyDay");

			return new List<TDTO>(criteria.List<TDTO>());
		}

		/// <summary>
		/// Save the object
		/// </summary>
		public TDTO Save(TDTO obj)
		{
			Session.Save(obj);
			return obj;
		}

		/// <summary>
		/// Can be called with objects that have generated ids for updating
		/// or creating new entries.
		/// </summary>
		public TDTO SaveOrUpdate(TDTO obj)
		{
			Session.SaveOrUpdate(obj);
			return obj;
		}

		/// <summary>
		/// Save, update, or merge entries that have been detached from a session
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public TDTO Merge(TDTO obj)
		{
			Session.Merge(obj);
			return obj;
		}

		/// <summary>
		/// Delete the object
		/// </summary>
		public void Delete(TDTO obj)
		{
			Session.Delete(obj);
		}

		/// <summary>
		/// Clear the session cache
		/// </summary>
		public void Flush()
		{
			Session.Flush();
		}
	}
}

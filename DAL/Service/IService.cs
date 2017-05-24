using System.Collections.Generic;

namespace DAL.Service
{
	/// <summary>
	/// Common interface for all services.
	/// </summary>
	/// <typeparam name="T">The DTO type represented by the service.</typeparam>
	public interface IService<T> where T : class
	{
		/// <summary>
		/// Get a list of all objects.
		/// </summary>
		/// <returns>A list of all objects.</returns>
		List<T> GetAll();


		/// <summary>
		/// Get a list of all objects, potentially returning them from the 2nd-level cache.
		/// </summary>
		/// <returns>A list of all objects.</returns>
		List<T> GetAllCacheable();

		/// <summary>
		/// Gets one row from the entity table matching the <param name="id" />.
		/// </summary>
		/// <remarks>Uses NHibernate's Get method.</remarks>
		T GetById(int id);

		/// <summary>
		/// Loads one row from the entity table matching the <param name="id" />.
		/// </summary>
		/// <remarks>Uses NHibernate's Load method.</remarks>
		T LoadById(int id);

		/// <summary>
		/// Saves the <param name="entity" /> object.
		/// </summary>
		T Save(T entity);

		/// <summary>
		/// Saves the <param name="entity" /> object.
		/// </summary>
		T SaveOrUpdate(T entity);

		/// <summary>
		/// Save or merge the <param name="entity" /> object.
		/// </summary>
		T Merge(T entity);

		/// <summary>
		/// Deletes the <param name="entity" /> object.
		/// </summary>
		void Delete(T entity);

		/// <summary>
		/// Refreshes the <param name="entity" /> object.
		/// </summary>
		void Refresh(T entity);
	}
}

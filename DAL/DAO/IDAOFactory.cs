using DAL.DAO.Classes;
using NHibernate;

namespace DAL.DAO
{
	/// <summary>
	/// The factory defines the method which the DAL implementors must contain
	/// </summary>
	public interface IDAOFactory
	{
		IPersonDAO GetPersonDAO(ISession session);
		IVoyageDAO GetVoyageDAO(ISession session);
	}
}

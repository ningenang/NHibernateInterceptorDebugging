using DAL.DAO.Classes;
using NHibernate;

namespace DAL.DAO
{
	/// <summary>
	/// The factory defines the method which the DAL implementors must contain
	/// </summary>
	public class DAOFactory : IDAOFactory
	{
		public IPersonDAO GetPersonDAO(ISession session) => new PersonDAO(session);

		public IVoyageDAO GetVoyageDAO(ISession session) => new VoyageDAO(session);
	}
}

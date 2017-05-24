using DAL.DTO.Classes;
using NHibernate.Linq;
using System.Linq;

namespace DAL.DAO.Classes
{
	public sealed partial class PersonDAO
	{
		public Person GetByUserIdentification(string userIdentification)
		{
			return Session.Query<Person>().SingleOrDefault(p => p.UserIdentification == userIdentification);
		}
	}
}

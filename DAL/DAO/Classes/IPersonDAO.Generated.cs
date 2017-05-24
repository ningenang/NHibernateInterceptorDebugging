using DAL.DTO.Classes;

namespace DAL.DAO.Classes
{
	public interface IPersonDAO : IDAO<Person, int>
	{
		Person GetByUserIdentification(string userIdentification);
	}
}

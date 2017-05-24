using DAL.DTO.Classes;

namespace DAL.Service.Classes
{
	/// <summary>
	/// Interface for <see cref="PersonService" />.
	/// </summary>
	/// <remarks>This interface must be edited manually with members from 
	/// <see cref="PersonService" />.</remarks>
	public interface IPersonService : IService<Person>
	{

		Person GetByUserIdentification(string userIdentification);
	}
}

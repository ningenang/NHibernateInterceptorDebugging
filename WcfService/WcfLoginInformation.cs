using DAL.Service.Classes;

namespace WcfService
{
	public class WcfLoginInformation : ILoginInformation
	{
		public string UserIdentification { get; set; }
		public int ID { get; set; }


		public static WcfLoginInformation Authenticate(string userName)
		{
			var dto = new PersonService().GetByUserIdentification(userName);
			if (dto == null)
				return null;

			return new WcfLoginInformation {
				ID = dto.PersonID,
				UserIdentification = dto.UserIdentification
			};
		}
	}
}
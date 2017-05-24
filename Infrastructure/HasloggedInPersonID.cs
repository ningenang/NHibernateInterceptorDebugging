using System;

namespace Infrastructure
{
	[Serializable]
	public class HasLoggedInPersonID
	{
		public int LoggedInPersonID { get; set; }
		public string UserIdentification { get; set; }
	}
}

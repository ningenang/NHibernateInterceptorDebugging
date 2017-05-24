﻿using DAL.DTO.Classes;

namespace DAL.Service.Classes
{
	/// <summary>
	/// Service for accessing <see cref="Person"/> objects.
	/// </summary>
	/// <remarks>
	/// All custom implementations should be done here. Override the method 
	/// <see cref="InitializeResources"/> to initialize other DAOs and resources.
	/// </remarks>	
	public sealed partial class PersonService
	{
		public Person GetByUserIdentification(string userIdentification)
		{
			return contextDAO.GetByUserIdentification(userIdentification);
		}
	}
}

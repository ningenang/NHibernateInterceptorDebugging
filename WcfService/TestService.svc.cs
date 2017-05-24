using DAL.Service.Classes;
using System;
using System.ServiceModel.Activation;
using WcfService.Schema;

namespace WcfService
{

	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class TestService : ITestService
	{
		private static ResponseHeaderType CreateResponse(StatusCodeEnumType status, string message = null) => new ResponseHeaderType
		{
			StatusCode = status,
			StatusMessage = message
		};


		public ResponseHeaderType SaveOrUpdateVoyage(VoyageRequest request)
		{
			try
			{
				// "Authenticate" user
				var user = new PersonService().GetByUserIdentification(request.Header.UserName);

				if (user == null)
				{
					return CreateResponse(StatusCodeEnumType.AccessDenied, $"Login failed for user {request.Header.UserName}");
				}

			}
			catch (Exception ex)
			{
				return CreateResponse(StatusCodeEnumType.ServerError, ex.Message);
			}
			
			return CreateResponse(StatusCodeEnumType.OK);
		}
	}
}

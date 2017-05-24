using DAL.Service.Classes;
using Infrastructure;
using log4net;
using Microsoft.Practices.Unity;
using SSNInfrastructure.LifetimeManagers;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Util;
using WcfService.Schema;

namespace WcfService
{

	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class TestService : ITestService
	{

		private static ILog log = LogManager.GetLogger(nameof(TestService));

		private static ResponseHeaderType CreateResponse(StatusCodeEnumType status, string message = null) => new ResponseHeaderType
		{
			StatusCode = status,
			StatusMessage = message
		};


		public ResponseHeaderType SaveOrUpdateVoyage(VoyageRequest request)
		{
			try
			{
				UnityWrapper.ConfiguredContainer.RegisterType<ILoginInformation, WcfLoginInformation>(
				new UnityOperationContextLifetimeManager(),
				new InjectionFactory(
					container =>
					{
						log.Debug($"Operation context hash: {OperationContext.Current.GetHashCode()} Creating WCFLoginInformation {request.Header.UserName}");
						return WcfLoginInformation.Authenticate(request.Header.UserName);
					})
				);

				LoggedInPersonIDInterceptorUtil.RegisterPersonIdProviderWcfContext(() => new HasLoggedInPersonID
				{
					LoggedInPersonID = UnityWrapper.Resolve<ILoginInformation>().ID,
					UserIdentification = UnityWrapper.Resolve<ILoginInformation>().UserIdentification,
				});

				
				var user = UnityWrapper.Resolve<ILoginInformation>();
				if (user == null)
				{
					return CreateResponse(StatusCodeEnumType.AccessDenied, $"Login failed for user {request.Header.UserName}");
				}



				var dto = new VoyageService().GetById(request.Body.VoyageID) ?? new DAL.DTO.Classes.Voyage();

				if (!string.IsNullOrWhiteSpace(request.Body.ShipName))
					dto.ShipName = request.Body.ShipName;

				if (!string.IsNullOrWhiteSpace(request.Body.ToLocation))
					dto.ToLocation = request.Body.ToLocation;

				if (!string.IsNullOrWhiteSpace(request.Body.FromLocation))
					dto.FromLocation = request.Body.FromLocation;

				if (request.Body.ETD != default(DateTime))
					dto.ETD = request.Body.ETD;

				if (request.Body.ETA != default(DateTime))
					dto.ETA = request.Body.ETA;

				dto = new VoyageService().SaveOrUpdate(dto);

				if (dto.ModifiedByPersonID != user.ID)
				{
					var modifiedBy = new PersonService().GetById(dto.ModifiedByPersonID.GetValueOrDefault());
					return CreateResponse(StatusCodeEnumType.ServerError, $"The voyage with ID {dto.VoyageID} was modified by {modifiedBy.UserIdentification} - not {user.UserIdentification}");
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

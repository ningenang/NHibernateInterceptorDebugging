using System.ServiceModel;
using WcfService.Schema;

namespace WcfService
{
	[ServiceContract]
	public interface ITestService
	{
		[OperationContract]
		ResponseHeaderType SaveOrUpdateVoyage(VoyageRequest request);

	}

}

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace WcfExtensions.Inspectors
{
	public class ContextDispatchMessageInspector : IDispatchMessageInspector
	{
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{

			using (var buffer = request.CreateBufferedCopy(int.MaxValue))
			{
				OperationContext.Current.Extensions.Add(new WcfOperationContext());

				request = buffer.CreateMessage();
			}

			return null; //Correlation state passed to BeforeSendReply
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			OperationContext.Current.Extensions.Remove(WcfOperationContext.Current);
		}
	}
}

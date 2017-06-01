using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using WcfExtensions.Inspectors;

namespace WcfExtensions.Behavior
{
	public class ContextServiceBehavior : BehaviorExtensionElement, IServiceBehavior
	{
		public override Type BehaviorType => typeof(ContextServiceBehavior);

		protected override object CreateBehavior()
		{
			return new ContextServiceBehavior();
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
			{
				if (channelDispatcher == null)
					continue;

				foreach (var endpoint in channelDispatcher.Endpoints)
				{
					endpoint.DispatchRuntime.MessageInspectors.Add(new ContextDispatchMessageInspector());
				}
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		
	}
}

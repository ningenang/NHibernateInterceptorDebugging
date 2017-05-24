using log4net;
using Microsoft.Practices.Unity;
using SSNInfrastructure;
using SSNInfrastructure.LifetimeManagers;
using System;
using System.ServiceModel;
using System.Web;

namespace Infrastructure
{
	public delegate void RegisterTypes(IUnityContainer container);

	public static class UnityWrapper
	{
		private static readonly ILog log = LogManager.GetLogger(nameof(UnityWrapper));

		private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(ValueFactory);

		/// <summary>
		/// Called when registration occurs.
		/// </summary>
		public static event RegisterTypes RegisterTypes = delegate { };

		public static IUnityContainer ConfiguredContainer
		{
			get
			{
				//check if WCF context
				var context = OperationContext.Current;
				if (context != null)
				{
					var provider = context.EndpointDispatcher.DispatchRuntime.InstanceProvider as UnityInstanceProvider;
					if (provider != null)
						return provider.GetContainer();

				}

				return (HttpContext.Current?.Application.GetContainer() ?? Container).Value;
			}
		}


		internal static IUnityContainer ValueFactory()
		{
			var container = new UnityContainer();

			RegisterTypes(container);


			return container;
		}

		public static T Resolve<T>()
		{
			var resolved = ConfiguredContainer.Resolve<T>();

			var context = OperationContext.Current;
			if (context != null)
			{
				if (typeof(T).Name == nameof(HasLoggedInPersonID))
				{
					var value = resolved as HasLoggedInPersonID;
					log.Debug($"OperationContext hash: {context.GetHashCode()}. Resolving {nameof(HasLoggedInPersonID)} ({value.LoggedInPersonID}, {value.UserIdentification})");
				}

			}

			return resolved;
		}

		/// <summary>
		/// Used when manually creating new objects 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T Inject<T>(this T obj)
		{
			return ConfiguredContainer.BuildUp<T>(obj);
		}

	}
}

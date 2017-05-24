using DAL.SessionManagement;
using Infrastructure;
using log4net;
using Microsoft.Practices.Unity;
using SSNInfrastructure.LifetimeManagers;
using System;
using System.ServiceModel;

namespace Util
{
	public static class LoggedInPersonIDInterceptorUtil
	{
		private static readonly ILog log = LogManager.GetLogger(nameof(LoggedInPersonIDInterceptorUtil));
	
		public static void SetupSessionFactoryInterceptor()
		{
			SessionFactory.SessionFactoryInterceptor = new LoggedInPersonIDInterceptor();
		}

		private static void RegisterPersonIdProvider(Func<int?> loggedInPersonIDProvider, LifetimeManager lifeTimeManager)
		{
			UnityWrapper.ConfiguredContainer.RegisterType<HasLoggedInPersonID, HasLoggedInPersonID>(
				lifeTimeManager,
				new InjectionFactory(c =>
				{
					var value = loggedInPersonIDProvider();
					log.Debug($"OperationContext hash: {OperationContext.Current?.GetHashCode()} Creating HasLoggedInPersonID ({value})");

					return new HasLoggedInPersonID { LoggedInPersonID = value ?? 0 };
				})
			);
		}

		private static void RegisterPersonIdProvider(Func<HasLoggedInPersonID> loggedInPersonIDProvider, LifetimeManager lifeTimeManager)
		{
			UnityWrapper.ConfiguredContainer.RegisterType<HasLoggedInPersonID, HasLoggedInPersonID>(
				lifeTimeManager,
				new InjectionFactory(c =>
				{
					var value = loggedInPersonIDProvider();
					log.Debug($"OperationContext hash: {OperationContext.Current?.GetHashCode()} Creating HasLoggedInPersonID ({value.LoggedInPersonID}, {value.UserIdentification})");

					return value;
				})
			);
		}


		public static void RegisterPersonIdProviderWcfContext(Func<int?> loggedInPersonIDProvider)
		{
			RegisterPersonIdProvider(loggedInPersonIDProvider, new UnityOperationContextLifetimeManager());
		}

		public static void RegisterPersonIdProviderWcfContext(Func<HasLoggedInPersonID> loggedInPersonIDProvider)
		{
			RegisterPersonIdProvider(loggedInPersonIDProvider, new UnityOperationContextLifetimeManager());
		}

		public static void RegisterPersonIdProviderHttpSessionContext(Func<int?> loggedInPersonIDProvider)
		{
			RegisterPersonIdProvider(loggedInPersonIDProvider, new HttpSessionLifetimeManager());
		}

		public static void RegisterPersonIdProviderThreadLocalContext(Func<int?> loggedInPersonIDProvider)
		{
			RegisterPersonIdProvider(loggedInPersonIDProvider, new PerThreadLifetimeManager());
		}

	}
}

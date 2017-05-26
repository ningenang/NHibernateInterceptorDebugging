using DAL.SessionManagement;
using log4net;
using System;
using System.ServiceModel;
using System.Web;

namespace Util
{
	public static class LoggedInPersonIDInterceptorUtil
	{
		private static readonly ILog log = LogManager.GetLogger(nameof(LoggedInPersonIDInterceptorUtil));


		public static readonly string HttpContextItemsKey = "LoggedInPersonIDInterceptor";
		public static void SetupSessionFactoryInterceptor()
		{
			SessionFactory.SessionFactoryInterceptor = new LoggedInPersonIDInterceptor();
		}

		public static void RegisterPersonIdProvider(Func<int?> loggedInPersonIdProvider) {

			var loggedInPersonId = loggedInPersonIdProvider();

			log.Debug($"Operation context hash: {OperationContext.Current.GetHashCode()} LIPID: {loggedInPersonId}");

			HttpContext.Current.Items[HttpContextItemsKey] = loggedInPersonId;
		}



	}
}

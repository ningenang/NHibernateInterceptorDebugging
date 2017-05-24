using log4net;
using System;
using Util;

namespace WcfService
{
	public class Global : System.Web.HttpApplication
	{
		private static readonly ILog log = LogManager.GetLogger(nameof(Global));
		protected void Application_Start(object sender, EventArgs e)
		{
			log4net.Config.XmlConfigurator.Configure();

			LoggedInPersonIDInterceptorUtil.SetupSessionFactoryInterceptor();
		}

	}
}
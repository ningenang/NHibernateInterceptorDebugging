using DAL.SessionManagement;
using System;

namespace Util
{
	/// <summary>
	/// Provides a simple abstraction without creating a dependency to NHibernate
	/// </summary>
	public static class LoggedInPersonIDInterceptorUtil
	{

		public static void SetSessionFactoryInterceptor(AbstractLoggedInPersonIDInterceptor interceptor)
		{
			SessionFactory.SessionFactoryInterceptor = interceptor;
		}

		public static void RegisterPersonIDProvider(Func<int?> loggedInPersonIDProvider)
		{
			var interceptor = SessionFactory.SessionFactoryInterceptor as AbstractLoggedInPersonIDInterceptor;

			if (interceptor == null)
				throw new NotSupportedException($"The SessionFactory must be configured with a SessionFactoryInterceptor of type AbstractLoggedInPersonIDInterceptor");

			interceptor.SetValue(loggedInPersonIDProvider);
		}

	}
}

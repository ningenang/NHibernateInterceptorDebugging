using System;
using WcfExtensions;

namespace Util
{
	public class WcfLoggedInPersonIDInterceptor : AbstractLoggedInPersonIDInterceptor
	{
		private const string LoggedInPersonIDInterceptorOperationContextKey = "LoggedInPersonIDInterceptorOperationContextKey";

		public override int? GetValue()
		{
			if (int.TryParse(WcfOperationContext.Current.Items[LoggedInPersonIDInterceptorOperationContextKey]?.ToString(), out var returnValue))
				return returnValue;

			return null;
		}

		public override void SetValue(Func<int?> valueProvider)
		{
			WcfOperationContext.Current.Items[LoggedInPersonIDInterceptorOperationContextKey] = valueProvider();
		}
	}
}

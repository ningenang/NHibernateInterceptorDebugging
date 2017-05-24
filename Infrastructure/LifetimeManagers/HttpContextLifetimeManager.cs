using Microsoft.Practices.Unity;
using System;
using System.Web;

namespace SSNInfrastructure.LifetimeManagers
{
	/// <summary>
	/// Provides lifetime management services based on the HttpContext Items collection. Using this lifetime manager, 
	/// dependencies are constructed and remain available for the entirety of a single HTTP call
	/// </summary>
	public class HttpContextLifetimeManager : LifetimeManager, IDisposable
	{
		/// <summary>
		/// The key used to reference the HttpContext.Current.Items collection
		/// </summary>
		private readonly string _key = Guid.NewGuid().ToString();

		/// <summary>
		/// Retrieves the dependency injection from the HttpContext
		/// </summary>
		/// <returns></returns>
		public override object GetValue()
		{
			return HttpContext.Current.Items[_key];
		}

		/// <summary>
		/// Removes the dependency injection from the HttpContext
		/// </summary>
		public override void RemoveValue()
		{
			HttpContext.Current.Items.Remove(_key);
		}

		/// <summary>
		/// Sets the dependency injection value in the HttpContext
		/// </summary>
		/// <param name="newValue"></param>
		public override void SetValue(object newValue)
		{
			HttpContext.Current.Items[_key] = newValue;
		}

		/// <summary>
		/// Disposes and removes the dependency injection from the HttpContext
		/// </summary>
		public void Dispose()
		{
			RemoveValue();
		}
	}
}

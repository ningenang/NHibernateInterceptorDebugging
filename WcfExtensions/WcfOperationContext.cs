using System.Collections;
using System.ServiceModel;

namespace WcfExtensions
{
	public class WcfOperationContext : IExtension<OperationContext>
	{

		private readonly IDictionary _items;
		public static WcfOperationContext Current => OperationContext.Current.Extensions.Find<WcfOperationContext>();

		internal WcfOperationContext()
		{
			_items = new Hashtable();
		}

		public IDictionary Items => _items;


		public void Attach(OperationContext owner)
		{
		}

		public void Detach(OperationContext owner)
		{
		}
	}
}

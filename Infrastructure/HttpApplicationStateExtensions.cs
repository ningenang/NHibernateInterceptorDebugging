using Infrastructure;
using Microsoft.Practices.Unity;
using System;
using System.Web;

namespace SSNInfrastructure
{
	public static class HttpApplicationStateExtensions
    {
        private const string GlobalContainerKey = "DiContainer";
        private const string RequestContainerKey = "DiChildContainer";

        public static Lazy<IUnityContainer> GetContainer(this HttpApplicationState appState)
        {
            appState.Lock();
            try
            {
                var myContainer = appState[GlobalContainerKey] as Lazy<IUnityContainer>;

                if (myContainer != null)
                    return myContainer;

                myContainer = new Lazy<IUnityContainer>(UnityWrapper.ValueFactory);
                //new UnityContainer();
                appState[GlobalContainerKey] = myContainer;

                return myContainer;
            }
            finally
            {
                appState.UnLock();
            }
        }

        public static void SetContainer(this HttpApplicationState appState, IUnityContainer container)
        {
            appState.Lock();

            try
            {
                appState[GlobalContainerKey] = container;
            }
            finally
            {
                appState.UnLock();
            }
        }

        private static readonly object ContainerLock = new object();
        public static IUnityContainer GetChildContainer(this HttpContext context)
        {
            var childContainer = context.Items[RequestContainerKey] as IUnityContainer;

            if (childContainer != null) return childContainer;

            lock (ContainerLock)
            {
                childContainer = GetContainer(context.Application).Value.CreateChildContainer();
                context.SetChildContainer(childContainer);
            }

            return childContainer;
        }

        public static void SetChildContainer(this HttpContext context, IUnityContainer container)
        {
            lock (ContainerLock)
            {
                context.Items[RequestContainerKey] = container;
            }
        }
    }
}

using Autofac;
using System;
using System.Linq;
using System.Reflection;

namespace EasyHttpClient.Extensions.Autofac
{
    public static class AutofacExtensions
    {
        private static MethodInfo _createMethod = typeof(EasyHttpClientFactory).GetMethod("Create", new Type[] { });

        public static void RegisterEasyClient(this ContainerBuilder builder, Type[] clientTypes, Action<EasyClientConfig> configure = null)
        {
            var config = new EasyClientConfig();
            configure?.Invoke(config);
            var factory = new EasyHttpClientFactory();
            factory.Config = config;

            clientTypes = clientTypes.Where(x => x.IsInterface).ToArray();

            foreach (var t in clientTypes)
            {
                builder.Register(c =>
                {
                    return _createMethod.MakeGenericMethod(t).Invoke(factory, null); ;
                }).As(t);
            }
        }
    }
}

using EasyHttpClient.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyHttpClient
{
    public class EasyHttpClientFactory
    {
        public EasyClientConfig Config { get; set; } = new EasyClientConfig();

        public T Create<T>(string host)
        {
            return Create<T>(new Uri(host));
        }

        public T Create<T>(Uri host)
        {
            Config.Host = host;
            return Create<T>();
        }

        public T Create<T>()
        {
            ValidateEasyClientConfig();
            var executer = new ProxyMethodExecuter(Config);
            IProxyFactory proxyFactory;
#if NET472
            proxyFactory = new RealProxyFactory<T>();
#else
            proxyFactory = new ReflectionProxyFactory();
#endif
            return proxyFactory.Create<T>(executer);
        }

        private void ValidateEasyClientConfig()
        {
            if (Config == null)
            {
                throw new ArgumentNullException(nameof(Config));
            }
            if (Config.Host == null)
            {
                throw new ArgumentNullException(nameof(Config.Host));
            }
        }
    }
}

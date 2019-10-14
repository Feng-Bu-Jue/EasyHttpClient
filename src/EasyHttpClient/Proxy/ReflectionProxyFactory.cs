using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EasyHttpClient.Proxy
{
    public class ReflectionProxyFactory : DispatchProxy , IProxyFactory
    {
        private ProxyMethodExecutor _executor;

        public T Create<T>(ProxyMethodExecutor executor)
        {
            object instance = Create<T, ReflectionProxyFactory>();
            ((ReflectionProxyFactory)instance).SetParameters(executor);
            return (T)instance;
        }

        private void SetParameters(ProxyMethodExecutor Executor)
        {
            _executor = Executor;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _executor.Execute(targetMethod, args);
        }
    }
}

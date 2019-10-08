using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EasyHttpClient.Proxy
{
    public class ReflectionProxyFactory : DispatchProxy , IProxyFactory
    {
        private ProxyMethodExecuter _executer;

        public T Create<T>(ProxyMethodExecuter executer)
        {
            object instance = Create<T, ReflectionProxyFactory>();
            ((ReflectionProxyFactory)instance).SetParameters(executer);
            return (T)instance;
        }

        private void SetParameters(ProxyMethodExecuter executer)
        {
            _executer = executer;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _executer.Excute(targetMethod, args);
        }
    }
}

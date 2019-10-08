#if NET472
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace EasyHttpClient.Proxy
{
    public class RealProxyFactory : RealProxy, IProxyFactory
    {
        public RealProxyFactory(Type type):base(type)
        {

        }

        private ProxyMethodExecuter _executer;

        public T Create<T>(ProxyMethodExecuter executer)
        {
            _executer = executer;

            return (T)this.GetTransparentProxy(); 
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;
            try
            {
                var result = _executer.Excute(methodInfo, methodCall.Args);
                return new ReturnMessage(result, new object[0], 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, methodCall);
            }
        }
    }

    public class RealProxyFactory<T> : RealProxyFactory
    {
        public RealProxyFactory() : base(typeof(T))
        {

        }
    }
}
#endif
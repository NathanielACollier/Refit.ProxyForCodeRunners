using System;
using Refit;
using Castle.DynamicProxy;
using System.Net.Http;

using System.Collections.Generic;
using System.Linq;

namespace Refit.ProxyForCodeRunners
{


    // to get it to work in linqpad and roslynpad you need to use proxy that uses Castle.Core here: https://gist.github.com/bennor/c73870e810f8245b2b1d

    // This script should be improved somehow to put this code in a common place.  Make sure it's the new vs2017 csproj format.  For now we are checking it in as is...

    public class ProxyRestService
    {
        static readonly ProxyGenerator Generator = new ProxyGenerator();

        public static T For<T>(HttpClient client)
            where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException("T must be an interface.");
            }

            var interceptor = new RestMethodInterceptor<T>(client);
            return Generator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
        }

        public static T For<T>(string hostUrl)
            where T : class
        {
            var client = new HttpClient() { BaseAddress = new Uri(hostUrl) };
            return For<T>(client);
        }

        class RestMethodInterceptor<T> : IInterceptor
        {
            static readonly Dictionary<string, Func<HttpClient, object[], object>> methodImpls
                = RequestBuilder.ForType<T>().InterfaceHttpMethods
                    .ToDictionary(k => k, v => RequestBuilder.ForType<T>().BuildRestResultFuncForMethod(v));

            readonly HttpClient client;

            public RestMethodInterceptor(HttpClient client)
            {
                this.client = client;
            }

            public void Intercept(IInvocation invocation)
            {
                if (!methodImpls.ContainsKey(invocation.Method.Name))
                {
                    throw new NotImplementedException();
                }
                invocation.ReturnValue = methodImpls[invocation.Method.Name](client, invocation.Arguments);
            }
        }
    }
}

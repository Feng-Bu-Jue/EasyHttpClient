using Autofac;
using EasyHttpClient;
using System;
using Xunit;
using EasyHttpClient.Extensions.Autofac;

namespace XUnitTest
{
    public class Test
    {
        [Fact]
        public async void SendTest()
        {
            var factory = new EasyHttpClientFactory();
            var baiduApiClient = factory.Create<IBaiduApiClient>("https://m.baidu.com/");
            var response = await baiduApiClient.SearchSugguestion("Are You OK");
            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async void AutofacTest()
        {
            var builder = new ContainerBuilder();
            var types = new Type[] { typeof(IBaiduApiClient) };
            builder.RegisterEasyClient(types, config =>
            {
                config.Host = new Uri("https://m.baidu.com/");
            });
            var container = builder.Build();
            var baiduApiClient = container.Resolve<IBaiduApiClient>();
            Assert.NotNull(baiduApiClient);
        }
    }
}

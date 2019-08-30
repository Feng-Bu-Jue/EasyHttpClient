using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace EasyHttpClient
{
    public class EasyClientConfig
    {
        public Uri Host { get; set; }
        public IHttpClientProvider HttpClientProvider { get; set; }
        public HttpClientSettings HttpClientSettings { get; set; }
        public bool AuthorizeRequired { get; set; }

        public EasyClientConfig()
        {
            HttpClientSettings = new HttpClientSettings();
            HttpClientProvider = new DefaultHttpClientProvider();
        }
    }
}

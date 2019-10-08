using EasyHttpClient.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace EasyHttpClient.Utilities
{
    public class HttpRequestMessageBuilder
    {
        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);

        internal StringFormatAttribute DefaultStringFormatter { get; private set; }

        public MultiPartAttribute MultiPartAttribute { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public UriBuilder UriBuilder { get; set; }
        public JsonSerializer JsonSerializer { get; private set; }

        private JsonSerializerSettings _jsonSetting;
        public JsonSerializerSettings JsonSetting
        {
            get
            {
                return _jsonSetting;
            }
            set
            {
                _jsonSetting = value;
                this.JsonSerializer = JsonSerializer.Create(value);
            }
        }

        public HttpNameValueCollection Headers { get; private set; }
        public List<CookieHeaderValue> Cookies { get; private set; }
        public Dictionary<string, string> PathParams { get; private set; }
        public HttpNameValueCollection QueryStrings { get; private set; }
        public HttpNameValueCollection FormBodys { get; private set; }
        public JToken JsonBody { get; set; }
        public List<HttpContent> RawContents { get; set; }

        public Version HttpVersion { get; set; }

        public HttpRequestMessageBuilder(HttpMethod httpMethod, UriBuilder uriBuilder, HttpClientSettings httpSettings, bool keepOrginalQueryString)
            :this(httpMethod, uriBuilder, httpSettings)
        {
            if (!string.IsNullOrWhiteSpace(uriBuilder.Query))
            {
                var parsedQueryString = HttpUtility.ParseQueryString(uriBuilder.Query, Utf8Encoding);
                foreach (var k in parsedQueryString.Keys)
                {
                    this.QueryStrings.Add(k.ToString(), parsedQueryString.GetValues(k.ToString()));
                }
            }
        }
        public HttpRequestMessageBuilder(HttpMethod httpMethod, UriBuilder uriBuilder, HttpClientSettings httpSettings)
        {
            this.HttpMethod = httpMethod;
            this.UriBuilder = uriBuilder;
            this.JsonSetting = httpSettings.JsonSerializerSettings;
            this.DefaultStringFormatter = new StringFormatAttribute(httpSettings.DateTimeFormat);
            this.Headers = new HttpNameValueCollection();
            this.Cookies = new List<CookieHeaderValue>();
            this.PathParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.QueryStrings = new HttpNameValueCollection();
            this.FormBodys = new HttpNameValueCollection();
            this.RawContents = new List<HttpContent>();
            this.HttpVersion = httpSettings.HttpVersion;
        }

        public HttpRequestMessage Build()
        {
            var httpMessage = new HttpRequestMessage(HttpMethod, UriBuilder.Uri);
            httpMessage.Version = HttpVersion;

            if (this.PathParams.Any())
            {
                var path = HttpUtility.UrlDecode(UriBuilder.Path);
                foreach (var p in this.PathParams)
                {
                    path = path.Replace("{" + p.Key + "}", p.Value, StringComparison.OrdinalIgnoreCase);
                }
                UriBuilder.Path = HttpUtility.UrlPathEncode(path);
            }

            if (this.QueryStrings.Any())
                UriBuilder.Query = string.Join("&", this.QueryStrings.Select(q => q.Key + "=" + HttpUtility.UrlEncode(q.Value)));

            if (this.HttpMethod == HttpMethod.Post || this.HttpMethod.Method == "PATCH"
                || this.HttpMethod == HttpMethod.Put)
            {
                if (this.MultiPartAttribute != null)
                {
                    var multipleContent = new MultipartContent(this.MultiPartAttribute.MultiPartType, Guid.NewGuid().ToString());

                    if (FormBodys.Any())
                    {
                        var content = new FormUrlEncodedContent(this.FormBodys);
                        multipleContent.Add(content);
                    }
                    if (JsonBody != null)
                    {
                        var content = new StringContent(JsonBody.ToString(), Utf8Encoding, "application/json");
                        multipleContent.Add(content);
                    }

                    if (RawContents.Any())
                    {
                        foreach (var c in RawContents)
                            multipleContent.Add(c);
                    }

                    httpMessage.Content = multipleContent;
                }
                else
                {
                    if (new[]{
                            FormBodys.Any(),
                            JsonBody!=null,
                            RawContents.Any()
                        }.Count(i => i == true) > 1)
                    {
                        throw new NotSupportedException("Not support multiple kinds of http content in a message!");
                    }

                    if (FormBodys.Any())
                    {
                        httpMessage.Content = new FormUrlEncodedContent(this.FormBodys);
                        if (httpMessage.Content.Headers != null && httpMessage.Content.Headers.ContentType!=null)
                        {
                            httpMessage.Content.Headers.ContentType.CharSet = Utf8Encoding.HeaderName;
                        }
                    }
                    else if (JsonBody != null)
                    {
                        httpMessage.Content = new StringContent(JsonBody.ToString(), Utf8Encoding, "application/json");
                        if (httpMessage.Content.Headers != null && httpMessage.Content.Headers.ContentType != null)
                        {
                            httpMessage.Content.Headers.ContentType.CharSet = Utf8Encoding.HeaderName;
                        }
                    }
                    if (RawContents.Any())
                    {
                        httpMessage.Content = RawContents.FirstOrDefault();
                    }
                }

            }
            httpMessage.RequestUri = this.UriBuilder.Uri;


            foreach (var h in this.Headers.GroupBy(h => h.Key))
            {
                httpMessage.Headers.TryAddWithoutValidation(h.Key, h.Select(i => i.Value));
            }

            foreach (var c in this.Cookies)
                httpMessage.Headers.GetCookies().Add(c);


            return httpMessage;
        }
    }
}

using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Core.Common;

namespace Makaba.Utils {
    public static class HttpClientUtils {
        public static void SetupClient(IUrlService urlService, HttpClient client, IHttpFilter filter) {
            var baseFilter = filter as HttpBaseProtocolFilter;
            if (baseFilter == null)
                return;

            HttpCookieCollection cookies = baseFilter.CookieManager.GetCookies(urlService.GetFullUrl("/"));
            foreach (HttpCookie cookie in cookies) {
                client.DefaultRequestHeaders.Cookie.Add(new HttpCookiePairHeaderValue(cookie.Name, cookie.Value));
            }
        }
    }
}
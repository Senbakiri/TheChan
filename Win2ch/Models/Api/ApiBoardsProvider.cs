using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Newtonsoft.Json;

namespace Win2ch.Models.Api
{
    class ApiBoardsProvider
    {
        public async Task<List<Category>> GetCategories()
        {
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            var client = new HttpClient(httpFilter);
            var json = await client.GetStringAsync(new Uri(Urls.BoardsList));

            return await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<Dictionary<string, List<Board>>>(json)
                    .Select(x => new Category(x.Value) { Name = x.Key }).ToList());
        }
    }
}

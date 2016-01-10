using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Newtonsoft.Json;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models.Api
{
    class ApiBoardsProvider
    {
        public async Task<List<Category>> GetCategories()
        {
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            var client = new HttpClient(httpFilter);
            var response = await client.GetAsync(new Uri(Urls.BoardsList));

            if (!response.IsSuccessStatusCode)
                throw new HttpException(response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            return await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<Dictionary<string, List<Board>>>(json)
                    .Select(x => new Category(x.Value) { Name = x.Key }).ToList());
        }
    }
}

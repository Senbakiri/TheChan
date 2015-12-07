using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace Win2ch.Models
{
    class BoardsProvider
    {
        public async Task<List<Category>> GetCategories()
        {
            var client = new HttpClient();
            var json = await client.GetStringAsync(new Uri(Urls.BoardsList));

            return await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<Dictionary<string, List<Board>>>(json)
                    .Select(x => new Category(x.Value) { Name = x.Key }).ToList());
        }
    }
}

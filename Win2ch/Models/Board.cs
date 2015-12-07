using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Win2ch.Models
{
    public class Board
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public async Task<List<Thread>> GetThreads(int pageN)
        {
            if (pageN < 0)
                throw new ArgumentException("PageN must be greater than zero", nameof(pageN));

            var page = pageN == 0 ? "index" : pageN.ToString();
            var url = new Uri(string.Format(Urls.ThreadsList, Id.ToLower(), page));
            var client = new HttpClient();

            var response = await client.GetAsync(url);
            string data = await response.Content.ReadAsStringAsync();

            JToken results = (JToken) await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject(data));

            // TODO: I don't think that this is a good solution
            return await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<List<Thread>>(results["threads"].ToString()).Select(t =>
                {
                    t.Board = this;
                    return t;
                }).ToList());
        }
    }
}

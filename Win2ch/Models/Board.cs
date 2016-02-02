using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models {
    public class Board {
        public string Id { get; set; }
        public string Name { get; set; }

        public Board(string id, string name = "") {
            Id = id;
            Name = name;
        }

        public async Task<List<Thread>> GetThreads(int pageN) {
            if (pageN < 0)
                throw new ArgumentException("PageN must be greater than zero", nameof(pageN));

            var page = pageN == 0 ? "index" : pageN.ToString();
            var url = new Uri(string.Format(Urls.ThreadsList, Id.ToLower(), page));
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            var client = new HttpClient(httpFilter);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpException(response.StatusCode);
            string data = await response.Content.ReadAsStringAsync();

            JToken results = (JToken)await Task.Factory.StartNew(() =>
               JsonConvert.DeserializeObject(data));

            Name = results["BoardName"].ToString();

            var threads = await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<List<Thread>>(results["threads"].ToString()));
            SetupThreads(threads);
            return threads;
        }

        private void SetupThreads(List<Thread> threads) {
            foreach (var thread in threads) {
                thread.Board = this;
                thread.Posts = Thread.FillPosts(thread.Posts, this).ToList();
                for (int i = 1; i < thread.Posts.Count; ++i) {
                    var post = thread.Posts[i];
                    post.Position = thread.TotalPosts + i + 1;
                }
            }
        }

        public async Task<Post> GetPost(int num) {
            var url = new Uri(string.Format(Urls.SinglePost, Id, num));
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            var client = new HttpClient(httpFilter);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpException(response.StatusCode);
            var postJson = await response.Content.ReadAsStringAsync();
            postJson.CheckForApiError();
            return JsonConvert.DeserializeObject<List<Post>>(postJson).First();
        }

        protected bool Equals(Board other) {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Board)obj);
        }

        public override int GetHashCode() {
            return Id?.GetHashCode() ?? 0;
        }
    }
}

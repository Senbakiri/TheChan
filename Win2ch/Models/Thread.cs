using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Newtonsoft.Json;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models
{
    public class Thread
    {
        public Board Board { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public string Name => Posts?.FirstOrDefault()?.Subject;
        public int? Num => Convert.ToInt32(Posts?.FirstOrDefault()?.Num);
        
        public async Task<List<Post>> GetPostsFrom(int n)
        {
            var url = new Uri(string.Format(Urls.ThreadPosts, Board.Id, Posts.First().Num, n));
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            var client = new HttpClient(httpFilter);
            var response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            return await Task.Factory.StartNew(() =>
                FillDeps(JsonConvert.DeserializeObject<List<Post>>(json), Board).ToList());
        }

        public static IEnumerable<Post> FillDeps(IEnumerable<Post> posts, Board b)
        {
            return posts.Select(p =>
            {
                p.Board = b;
                foreach (var info in p.Images)
                    info.Board = b;
                return p;
            });
        }

        public async Task Reply(NewPostInfo info)
        {
            if (!Num.HasValue || Board == null)
                // TODO: Replace with separate exception
                throw new Exception("Invalid thread");

            using (var client = new HttpClient())
            {


                var content = new HttpMultipartFormDataContent
                {
                    {new HttpStringContent("1"), "json"},
                    {new HttpStringContent("post"), "task"},
                    {new HttpStringContent(Board.Id), "board"},
                    {new HttpStringContent(Num.ToString()), "thread"},
                    {new HttpStringContent(info.Comment ?? ""), "comment"}
                };
                var response = await client.PostAsync(new Uri(Urls.Posting), content);
                var responseString = await response.Content.ReadAsStringAsync();
                responseString.CheckForApiError();
            }
        }
    }
}

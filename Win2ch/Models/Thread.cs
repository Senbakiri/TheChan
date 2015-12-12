using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Web;
using Windows.Web.Http;
using Newtonsoft.Json;

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
            var client = new HttpClient();
            string json = await client.GetStringAsync(url);

            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Post>>(json));
        }

        public async void Reply(ReplyInfo info)
        {
            if (!Num.HasValue || Board == null)
                // TODO: Replace with separate exception
                throw new Exception("Invalid thread");

            using (var client = new HttpClient())
            {
                var data = new Dictionary<string, string>
                {
                    ["json"] = "1",
                    ["task"] = "post",
                    ["captcha_type"] = "2chcaptcha",
                    ["board"] = Board.Id,
                    ["thread"] = Num.ToString(),
                    ["comment"] = info.Comment,
                };


                var content = new HttpMultipartFormDataContent
                {
                    {new HttpStringContent("1"), "json"},
                    {new HttpStringContent("post"), "task"},
                    {new HttpStringContent(Board.Id), "board"},
                    {new HttpStringContent(Num.ToString()), "thread"},
                    {new HttpStringContent(info.Comment), "comment"}
                };
                var response = await client.PostAsync(new Uri(Urls.Posting), content);
                var responseString = await response.Content.ReadAsStringAsync();
            }
        }
    }
}

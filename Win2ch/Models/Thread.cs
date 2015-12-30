using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using Win2ch.Models.Exceptions;
using Win2ch.Services;
using Buffer = Windows.Storage.Streams.Buffer;

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
            var json = await response.Content.ReadAsStringAsync();

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
                SetupHeaders(client);
                var content = await SetupContent(info);

                var uri = new Uri(Urls.Posting);
                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                responseString.CheckForApiError();
            }
        }

        private async Task<IHttpContent> SetupContent(NewPostInfo postInfo)
        {
            var content = new HttpMultipartFormDataContent
            {
                {new HttpStringContent("1"), "json"},
                {new HttpStringContent("post"), "task"},
                {new HttpStringContent(Board.Id), "board"},
                {new HttpStringContent(Num?.ToString() ?? "0"), "thread"},
                {new HttpStringContent(postInfo.EMail ?? ""), "email"},
                {new HttpStringContent(postInfo.Name ?? ""), "name"},
                {new HttpStringContent(postInfo.Subject ?? ""), "subject"},
                {new HttpStringContent(postInfo.Comment ?? ""), "comment"}
            };

            if (postInfo.Files == null)
                return content;

            var imageIndex = 1;
            foreach (var file in postInfo.Files.Take(4))
            {

                var stream = await file.OpenAsync(FileAccessMode.Read);
                var fileContent = CreateFileContent(stream,
                    HttpMediaTypeHeaderValue.Parse(file.ContentType));
                content.Add(fileContent, $"image{imageIndex}", file.Name);
                imageIndex++;
            }

            return content;
        }

        private HttpStreamContent CreateFileContent(IInputStream stream,
            HttpMediaTypeHeaderValue contentType)
        {
            var fileContent = new HttpStreamContent(stream);
            fileContent.Headers.ContentType = contentType;
            return fileContent;
        }

        private void SetupHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept-Charset", "ISO-8859-1");
        }
    }
}

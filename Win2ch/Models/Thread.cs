using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using Win2ch.Models.Exceptions;

namespace Win2ch.Models {
    public class Thread {
        public Board Board { get; set; }

        public virtual List<Post> Posts { get; set; } = new List<Post>();

        public virtual string Name => Posts?.FirstOrDefault()?.Subject;

        [JsonProperty("posts_count")]
        public int TotalPosts { get; set; }

        [JsonProperty("thread_num")]
        public long Num { get; private set; }

        [JsonProperty("files_count")]
        public int FilesCount { get; set; }

        public Thread(long num, string boardId) {
            Num = num;
            Board = new Board(boardId);
        }

        public async Task<List<Post>> GetPostsFrom(int n) {
            var url = new Uri(string.Format(Urls.ThreadPosts, Board.Id, Num, n));
            var httpFilter = new HttpBaseProtocolFilter();
            httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            var client = new HttpClient(httpFilter);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpException(response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            json.CheckForApiError();

            return await Task.Factory.StartNew(() =>
                FillPosts(JsonConvert.DeserializeObject<List<Post>>(json), Board, n).ToList());
        }

        public static IEnumerable<Post> FillPosts(List<Post> posts, Board b, int firstPostPos = 1) {
            var result = new List<Post>();
            var answers = new Dictionary<long, List<Post>>();

            for (var i = 0; i < posts.Count; i++) {
                var post = posts[i];
                FillAnswers(post, answers);

                post.Board = b;

                post.Position = firstPostPos + i;
                foreach (var info in post.Images)
                    info.Board = b;
                result.Add(post);
            }

            CompleteAnswersProcessing(posts, answers);

            return result;
        }

        private static void CompleteAnswersProcessing(IReadOnlyCollection<Post> posts,
            Dictionary<long, List<Post>> answers) {
            foreach (var postN in answers.Keys) {
                var post = posts.FirstOrDefault(p => p.Num == postN);
                if (post == null)
                    continue;
                post.Replies = answers[postN];
            }
        }

        /// <summary>
        /// Finds all links to post in given post and marks post as answer to each of answer
        /// </summary>
        /// <param name="post">Post where to find answers</param>
        /// <param name="answers">Where to store</param>
        private static void FillAnswers(Post post, Dictionary<long, List<Post>> answers) {
            var answerRegex = new Regex(@">>(\d+)");
            var matches = answerRegex.Matches(post.Comment);
            foreach (var match in matches.Cast<Match>()) {
                if (match.Groups.Count < 2)
                    continue;
                var postN = Convert.ToInt64(match.Groups[1].Captures[0].Value);
                if (!answers.ContainsKey(postN))
                    answers.Add(postN, new List<Post>());
                answers[postN].Add(post);
            }
        }

        public async Task Reply(NewPostInfo info) {
            if (Board == null)
                // TODO: Replace with separate exception
                throw new Exception("Invalid thread");

            using (var client = new HttpClient()) {
                SetupHeaders(client);
                var content = await SetupContent(info);

                var uri = new Uri(Urls.Posting);
                var response = await client.PostAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                    throw new HttpException(response.StatusCode);
                var responseString = await response.Content.ReadAsStringAsync();
                responseString.CheckForApiError();
            }
        }

        private async Task<IHttpContent> SetupContent(NewPostInfo postInfo) {
            var content = new HttpMultipartFormDataContent
            {
                {new HttpStringContent("1"), "json"},
                {new HttpStringContent("post"), "task"},
                {new HttpStringContent(Board.Id), "board"},
                {new HttpStringContent(Num.ToString()), "thread"},
                {new HttpStringContent(postInfo.EMail ?? ""), "email"},
                {new HttpStringContent(postInfo.Name ?? ""), "name"},
                {new HttpStringContent(postInfo.Subject ?? ""), "subject"},
                {new HttpStringContent(postInfo.Comment ?? ""), "comment"}
            };

            if (postInfo.Files == null)
                return content;

            var imageIndex = 1;
            foreach (var file in postInfo.Files.Take(4)) {

                var stream = await file.OpenReadAsync();
                var fileContent = CreateFileContent(stream,
                    HttpMediaTypeHeaderValue.Parse(stream.ContentType));
                content.Add(fileContent, $"image{imageIndex}", $"attachedImage{imageIndex}");
                imageIndex++;
            }

            return content;
        }

        private HttpStreamContent CreateFileContent(IInputStream stream,
            HttpMediaTypeHeaderValue contentType) {
            var fileContent = new HttpStreamContent(stream);
            fileContent.Headers.ContentType = contentType;
            return fileContent;
        }

        private void SetupHeaders(HttpClient client) {
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAppendWithoutValidation(
                "Accept-Charset", "ISO-8859-1");
        }

        public override bool Equals(object obj) {
            return obj is Thread && Equals((Thread) obj);
        }

        protected bool Equals(Thread other) {
            return Equals(Board, other.Board) && Num == other.Num;
        }

        public override int GetHashCode() {
            unchecked {
                return ((Board?.GetHashCode() ?? 0)*397) ^ Num.GetHashCode();
            }
        }
    }
}

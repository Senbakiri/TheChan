using System;
using Core.Common;
using Core.Common.Links;
using Makaba.Services.Url;
using Xunit;

namespace MakabaTests {
    
    public class UrlServiceTest {

        private UrlService Service { get; } = new UrlService();

        [Fact]
        public void GeneratesCorrectUrlForBoardLink() {
            var link = new BoardLink("b");
            var expectedUri = new Uri("https://2ch.hk/b");
            Assert.Equal(expectedUri, Service.GetUrlForLink(link));
        }

        [Fact]
        public void GeneratesCorrectUrlForThreadLink() {
            var link = new ThreadLink("b", 1234567);
            var expectedUri = new Uri("https://2ch.hk/b/res/1234567.html");
            Assert.Equal(expectedUri, Service.GetUrlForLink(link));
        }

        [Fact]
        public void GeneratesCorrectUrlForPostLink() {
            var link = new PostLink("b", 1234567, 12345678);
            var expectedUri = new Uri("https://2ch.hk/b/res/1234567.html#12345678");
            Assert.Equal(expectedUri, Service.GetUrlForLink(link));
        }

        [Fact]
        public void CorrectlyIdentifiesBoardUrl() {
            CheckUrl<BoardLink>("https://2ch.hk/b", LinkType.Board, "b");
        }

        [Fact]
        public void CorrectlyIdentifiesThreadUrl() {
            CheckUrl<ThreadLink>("https://2ch.hk/b/res/1234567.html", LinkType.Thread, "b", 1234567);
        }

        [Fact]
        public void CorrectlyIdentifiesPostUrl() {
            CheckUrl<PostLink>("https://2ch.hk/b/res/1234567.html#12345678", LinkType.Post, "b", 1234567, 12345678);
        }

        [Fact]
        public void CorrectlyIdentifiesWrongUrl() {
            CheckUrl<object>("htps//2ch.hk/b/res/1234567.html", LinkType.None);
        }

        [Fact]
        public void CorrectlyIdentifiesUnknownUrl() {
            CheckUrl<object>("https://google.com/", LinkType.Unknown);
        }

        private void CheckUrl<TLink>(string url,
                                           LinkType expectedLinkType,
                                           string expectedBoardId = null,
                                           long expectedThreadNum = -1,
                                           long expectedPostNum = -1) {
            LinkType type = Service.DetermineLinkType(url);
            LinkBase link = Service.GetLink(url);
            Assert.Equal(expectedLinkType, type);
            if (link != null && expectedLinkType != LinkType.None && expectedLinkType != LinkType.Unknown) {
                Assert.IsType<TLink>(link);
            }

            switch (expectedLinkType) {
                case LinkType.Board:
                    var boardLink = (BoardLink) link;
                    Assert.Equal(expectedBoardId, boardLink.BoardId);
                    break;
                case LinkType.Thread:
                    var threadLink = (ThreadLink) link;
                    Assert.Equal(expectedBoardId, threadLink.BoardId);
                    Assert.Equal(expectedThreadNum, threadLink.ThreadNumber);
                    break;
                case LinkType.Post:
                    var postLink = (PostLink) link;
                    Assert.Equal(expectedBoardId, postLink.BoardId);
                    Assert.Equal(expectedThreadNum, postLink.ThreadNumber);
                    Assert.Equal(expectedPostNum, postLink.PostNumber);
                    break;
            }
        }
    }
}
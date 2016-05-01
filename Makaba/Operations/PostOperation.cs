using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Core.Common;
using Core.Converters;
using Core.Models;
using Core.Operations;
using Makaba.Entities;

namespace Makaba.Operations {
    public class PostOperation : HttpMultipartPostOperationWithJsonResponseBase<PostingResultEntity, PostingResult>, IPostOperation {
        public PostOperation(IUrlService urlService, IConverter<PostingResultEntity, PostingResult> resultConverter) {
            UrlService = urlService;
            ResultConverter = resultConverter;
        }

        private IUrlService UrlService { get; }
        protected override IConverter<PostingResultEntity, PostingResult> ResultConverter { get; }
        public override Uri Uri { get; protected set; }
        public PostInfo PostInfo { get; set; }
        public string BoardId { get; set; }
        public long Parent { get; set; }

        public override async Task<PostingResult> ExecuteAsync() {
            Uri = UrlService.GetPostingUrl();
            await SetupHeaders();
            return await base.ExecuteAsync();
        }

        private async Task SetupHeaders() {
            AddString("json", 1);
            AddString("task", "post");
            AddString("board", BoardId);
            AddString("thread", Parent);
            AddString("email", PostInfo.EMail);
            AddString("name", PostInfo.Name);
            AddString("subject", PostInfo.Subject);
            AddString("op_mark", PostInfo.IsOp ? "1" : "0");
            AddString("comment", PostInfo.Text);

            var imageIndex = 0;
            foreach (IRandomAccessStreamReference file in PostInfo.Files) {
                await AddFile(file, $"image{imageIndex}", $"attachedImage{imageIndex}");
                ++imageIndex;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.Converters;
using Core.Models;
using Makaba.Entities;

namespace Makaba.Converters {
    public class PostConverter : IConverter<PostEntity, Post> {
        public PostConverter(IUrlService urlService) {
            UrlService = urlService;
        }

        private IUrlService UrlService { get; }
        public string BoardId { get; set; }

        public Post Convert(PostEntity source) {
            IList<Attachment> files = GetFiles(source);
            return new Post(
                source.Num,
                source.Parent,
                source.Subject,
                source.Name,
                source.Trip,
                source.Email,
                source.Comment,
                source.IsOp,
                source.IsBanned,
                source.IsClosed,
                source.Sticky != 0,
                files,
                DateUtils.TimestampToDateTime(source.Timestamp));
        }

        private IList<Attachment> GetFiles(PostEntity source) {
            if (source.Files == null)
                return new List<Attachment>();

            return source.Files.Select(f =>
                new Attachment(
                    f.Name,
                    f.Path,
                    UrlService.GetFileUrl(BoardId, f.Path),
                    f.Size,
                    f.Width,
                    f.Height,
                    f.Thumbnail,
                    UrlService.GetFileUrl(BoardId, f.Thumbnail),
                    f.ThumbnailWidth,
                    f.ThumbnailHeight,
                    (AttachmentType) f.Type)).ToList();
        }
    }
}
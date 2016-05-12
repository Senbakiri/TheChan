using System;

namespace Core.Models {
    public class Attachment {
        public Attachment(string name,
                          string path,
                          Uri uri,
                          int size,
                          int width,
                          int height,
                          string thumbnailPath,
                          Uri thumbnailUri,
                          int thumbnailWidth,
                          int thumbnailHeight,
                          AttachmentType type) {
            Name = name;
            Path = path;
            Uri = uri;
            Size = size;
            Width = width;
            Height = height;
            ThumbnailPath = thumbnailPath;
            ThumbnailUri = thumbnailUri;
            ThumbnailWidth = thumbnailWidth;
            ThumbnailHeight = thumbnailHeight;
            Type = type;
        }

        public string Name { get; }
        public string Path { get; }
        public Uri Uri { get; }
        public int Size { get; }
        public int Width { get; }
        public int Height { get; }
        public string ThumbnailPath { get; }
        public Uri ThumbnailUri { get; }
        public int ThumbnailWidth { get; }
        public int ThumbnailHeight { get; }
        public AttachmentType Type { get; }
    }
}
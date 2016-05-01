namespace Core.Common {
    public sealed class PostInfo {
        public string Text { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public bool IsOp { get; set; }

        public PostInfo Clone() => new PostInfo {
            Text = Text,
            EMail = EMail,
            Name = Name,
            Subject = Subject,
            IsOp = IsOp
        };

        public void Clear() {
            Text = Subject = string.Empty;
        }
    }
}
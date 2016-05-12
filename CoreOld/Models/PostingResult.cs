namespace Core.Models {
    public class PostingResult {
        public PostingResult(bool isSuccessful, long? postNumber = null, string error = null) {
            IsSuccessful = isSuccessful;
            Error = error;
            PostNumber = postNumber;
        }

        public bool IsSuccessful { get; }
        public long? PostNumber { get; }
        public string Error { get; }
    }
}
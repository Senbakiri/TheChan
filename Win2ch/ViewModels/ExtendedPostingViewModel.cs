using System;
using Caliburn.Micro;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using Win2ch.Common.UI;
using Win2ch.Extensions;

namespace Win2ch.ViewModels {
    public class ExtendedPostingViewModel : PropertyChangedBase {
        private string postText;
        private int selectionStart;
        private int selectionLength;
        private string eMail;
        private string name;
        private string subject;
        private bool isOp;
        private bool isSage;
        private bool isWorking;

        public ExtendedPostingViewModel(IShell shell, IBoard board, PostInfo postInfo, ThreadLink threadLink) {
            PostInfo = postInfo;
            SetupProperties();
            Shell = shell;
            Board = board;
            BoardId = threadLink.BoardId;
            Parent = threadLink.ThreadNumber;
        }

        public ExtendedPostingViewModel(IShell shell, IBoard board, PostInfo postInfo, string boardId) {
            PostInfo = postInfo;
            SetupProperties();
            Shell = shell;
            Board = board;
            BoardId = boardId;
            Parent = 0;
        }

        private PostInfo PostInfo { get; }
        private IShell Shell { get; }
        private IBoard Board { get; }
        private string BoardId { get; }
        private long Parent { get; }
        public event EventHandler<PostInfoChangedEventArgs> PostInfoChanged;

        public bool IsWorking {
            get { return this.isWorking; }
            private set {
                if (value == this.isWorking)
                    return;
                this.isWorking = value;
                NotifyOfPropertyChange();
            }
        }

        public string PostText {
            get { return this.postText; }
            set {
                if (value == this.postText)
                    return;
                this.postText = value;
                PostInfo.Text = value;
                NotifyOfPropertyChange();
                NotifyOfPostInfoChange();
            }
        }

        public string EMail {
            get { return this.eMail; }
            set {
                if (value == this.eMail)
                    return;
                this.eMail = value;
                PostInfo.EMail = value;
                IsSage = value.EqualsNc("sage");
                NotifyOfPropertyChange();
                NotifyOfPostInfoChange();
            }
        }

        public string Name {
            get { return this.name; }
            set {
                if (value == this.name)
                    return;
                this.name = value;
                PostInfo.Name = value;
                NotifyOfPropertyChange();
                NotifyOfPostInfoChange();
            }
        }

        public string Subject {
            get { return this.subject; }
            set {
                if (value == this.subject)
                    return;
                this.subject = value;
                PostInfo.Subject = value;
                NotifyOfPropertyChange();
                NotifyOfPostInfoChange();
            }
        }

        public bool? IsOp {
            get { return this.isOp; }
            set {
                if (value == this.isOp)
                    return;
                this.isOp = value.GetValueOrDefault();
                PostInfo.IsOp = value.GetValueOrDefault();
                NotifyOfPropertyChange();
                NotifyOfPostInfoChange();
            }
        }

        public bool? IsSage {
            get { return this.isSage; }
            set {
                if (value == this.isSage)
                    return;
                var val = value.GetValueOrDefault();
                this.isSage = val;
                if (val)
                    EMail = "sage";
                else if (EMail.EqualsNc("sage"))
                    EMail = "";
                NotifyOfPropertyChange();
            }
        }

        public int SelectionStart {
            get { return this.selectionStart; }
            set {
                if (value == this.selectionStart)
                    return;
                this.selectionStart = value;
                NotifyOfPropertyChange();
            }
        }

        public int SelectionLength {
            get { return this.selectionLength; }
            set {
                if (value == this.selectionLength)
                    return;
                this.selectionLength = value;
                NotifyOfPropertyChange();
            }
        }

        private void NotifyOfPostInfoChange() {
            PostInfoChanged?.Invoke(this, new PostInfoChangedEventArgs(PostInfo));
        }

        private void SetupProperties() {
            PostText = PostInfo.Text;
            Name = PostInfo.Name;
            EMail = PostInfo.EMail;
            Subject = PostInfo.Subject;
            IsOp = PostInfo.IsOp;
        }

        public void Insert(string text) {
            var selStart = SelectionStart;
            var selLen = SelectionLength;
            PostText = (PostText ?? "").Replace("\r\n", "\n").Insert(selStart, text);
            SelectionStart = selStart + selLen + text.Length;
        }

        public void Tag(string tag) {
            var selStart = SelectionStart;
            var selLen = SelectionLength;
            var first = $"[{tag}]";
            var second = $"[/{tag}]";
            PostText = (PostText ?? "")
                .Replace("\r\n", "\n")
                .Insert(selStart + selLen, second)
                .Insert(selStart, first);
            SelectionStart = selStart + selLen + first.Length + second.Length;
        }

        public async void Send() {
            IsWorking = true;
            Shell.LoadingInfo.InProgress("[Posting]");
            PostInfo postInfo = PostInfo.Clone();
            try {
                PostingResult result = await Board.PostAsync(postInfo, BoardId, Parent);
                if (!result.IsSuccessful)
                    throw new Exception(result.Error);
                Shell.LoadingInfo.Success("[Posted]");
                PostInfo.Clear();
                Close();
            } catch (Exception e) {
                Shell.LoadingInfo.Error(e.Message);
            }

            IsWorking = false;
        }

        public void Close() {
            if (!IsWorking)
                Shell.HidePopup();
        }
    }

    public class PostInfoChangedEventArgs : EventArgs {
        public PostInfoChangedEventArgs(PostInfo postInfo) {
            PostInfo = postInfo;
        }

        public PostInfo PostInfo { get; }
    }
}
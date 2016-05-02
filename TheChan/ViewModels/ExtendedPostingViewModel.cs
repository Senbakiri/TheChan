using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Caliburn.Micro;
using Core.Common;
using Core.Common.Links;
using Core.Models;
using TheChan.Common.Core;
using TheChan.Common.UI;
using TheChan.Extensions;

namespace TheChan.ViewModels {
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
        private bool canAttach;

        public ExtendedPostingViewModel(IShell shell, IBoard board, PostInfo postInfo, ThreadLink threadLink) {
            PostInfo = postInfo;
            Shell = shell;
            Board = board;
            BoardId = threadLink.BoardId;
            Parent = threadLink.ThreadNumber;
            SetupProperties();
        }

        public ExtendedPostingViewModel(IShell shell, IBoard board, PostInfo postInfo, string boardId) {
            PostInfo = postInfo;
            Shell = shell;
            Board = board;
            BoardId = boardId;
            Parent = 0;
            SetupProperties();
        }

        private PostInfo PostInfo { get; }
        private IShell Shell { get; }
        private IBoard Board { get; }
        private string BoardId { get; }
        private long Parent { get; }
        public event EventHandler<PostInfoChangedEventArgs> PostInfoChanged;
        public event EventHandler PostSent;

        public ObservableCollection<AttachmentViewModel> Attachments { get; } =
            new ObservableCollection<AttachmentViewModel>();

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

        public bool CanAttach {
            get { return this.canAttach; }
            private set {
                if (value == this.canAttach)
                    return;
                this.canAttach = value;
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
            Attachments.CollectionChanged += AttachmentsOnCollectionChanged;
            CanAttach = Board.MaxAttachments > 0;
        }

        private void AttachmentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) {
            CanAttach = Attachments.Count < Board.MaxAttachments;
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
            Shell.LoadingInfo.InProgress(Tab.GetLocalizationStringForView("Posting", "SendingPost"));
            PostInfo postInfo = PostInfo.Clone();
            try {
                PostingResult result = await Board.PostAsync(postInfo, BoardId, Parent);
                if (!result.IsSuccessful)
                    throw new Exception(result.Error);  
                Shell.LoadingInfo.Success(Tab.GetLocalizationStringForView("Posting", "PostSent"));
                PostSent?.Invoke(this, new EventArgs());
                PostInfo.Clear();
                NotifyOfPostInfoChange();
                IsWorking = false;
                Close();
            } catch (Exception e) {
                Shell.LoadingInfo.Error(e.Message);
                IsWorking = false;
            }

        }

        public async void PickAndAttach() {
            var picker = new FileOpenPicker();
            foreach (string format in Board.AllowedAttachmentFormats) {
                picker.FileTypeFilter.Add(format);
            }

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            List<StorageFile> files = (await picker.PickMultipleFilesAsync()).ToList();
            int howManyFilesCanAttach = Board.MaxAttachments - Attachments.Count;
            if (files.Count > howManyFilesCanAttach)
                files.RemoveRange(howManyFilesCanAttach, files.Count - howManyFilesCanAttach);

            foreach (StorageFile file in files) {
                await Attach(file);
            }
        }

        private async Task Attach(IRandomAccessStreamReference reference, string name) {
            if (Attachments.Count >= Board.MaxAttachments)
                return;

            var bitmap = new BitmapImage();
            IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync();
            if (PostInfo.Files == null)
                PostInfo.Files = new List<IRandomAccessStreamReference>();
            PostInfo.Files.Add(reference);
            bitmap.SetSource(stream);
            Attachments.Add(new AttachmentViewModel {
                Name = name,
                Image = bitmap,
                Type = stream.ContentType,
                Reference = reference
            });
        }

        public Task Attach(IStorageFile file) {
            return Attach(file, file.Name);
        }

        public Task AttachPastedFile(IRandomAccessStreamReference reference) {
            return Attach(reference, Tab.GetLocalizationStringForView("Posting", "Untitled"));
        }

        public void Detach(AttachmentViewModel attachment) {
            Attachments.Remove(attachment);
            PostInfo.Files.Remove(attachment.Reference);
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
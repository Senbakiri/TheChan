using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;
using Win2ch.Models;
using Win2ch.Models.Exceptions;
using Win2ch.Views;
using ViewModelBase = Win2ch.Mvvm.ViewModelBase;

namespace Win2ch.ViewModels
{
    public class PostingViewModel : ViewModelBase
    {
        public const int MaxAttachedFiles = 4;

        private string _Text = "";
        private Thread _Thread;
        private bool _IsWorking;
        private string _СurrentJob;
        private int _SelectionStart;
        private int _SelectionLength;
        private string _Subject;
        private string _Name;
        private string _EMail;
        private NewPostInfo _postInfo;
        private bool _CanAttachImages = true;
        private readonly Dictionary<BitmapImage, StorageFile>
            _attachedStorageFiles = new Dictionary<BitmapImage, StorageFile>(); 

        private NewPostInfo PostInfo
        {
            get { return _postInfo; }
            set
            {
                _postInfo = value;
                Text = value.Comment;
                EMail = value.EMail;
                Name = value.Name;
            }
        }

        public ICommand SendCommand { get; }
        public ICommand TagCommand { get; }
        public ICommand InsertCommand { get; }
        public ICommand AttachImageCommand { get; }

        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value ?? "";
                PostInfo.Comment = _Text;
               RaisePropertyChanged();
            }
        }

        public string Subject
        {
            get { return _Subject; }
            set
            {
                _Subject = value;
                PostInfo.Subject = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                PostInfo.Name = value;
                RaisePropertyChanged();
            }
        }

        public string EMail
        {
            get { return _EMail; }
            set
            {
                _EMail = value;
                PostInfo.EMail = value;
                RaisePropertyChanged();
            }
        }

        public Thread Thread
        {
            get { return _Thread; }
            private set
            {
                _Thread = value;
                RaisePropertyChanged();
            }
        }

        public bool IsWorking
        {
            get { return _IsWorking; }
            set
            {
                _IsWorking = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentJob
        {
            get { return _СurrentJob; }
            set
            {
                _СurrentJob = value;
                RaisePropertyChanged();
            }
        }

        public int SelectionStart
        {
            get { return _SelectionStart; }
            set
            {
                _SelectionStart = value;
                RaisePropertyChanged();
            }
        }

        public int SelectionLength
        {
            get { return _SelectionLength; }
            set
            {
                _SelectionLength = value;
                RaisePropertyChanged();
            }
        }

        public bool CanAttachImages
        {
            get { return _CanAttachImages; }
            private set
            {
                _CanAttachImages = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BitmapImage> AttachedImages { get; } 

        public PostingViewModel()
        {
            AttachedImages = new ObservableCollection<BitmapImage>();
            AttachedImages.CollectionChanged += AttachedImagesOnCollectionChanged;
            SendCommand = new DelegateCommand(Send);
            TagCommand = new DelegateCommand<string>(Tag);
            InsertCommand = new DelegateCommand<string>(Insert);
            AttachImageCommand = new DelegateCommand(AttachImage, () => CanAttachImages);
        }

        private void AttachedImagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CanAttachImages = AttachedImages.Count < MaxAttachedFiles;

            if (e.Action == NotifyCollectionChangedAction.Remove)
                _attachedStorageFiles.Remove(e.OldItems[0] as BitmapImage);
            PostInfo.Files = _attachedStorageFiles.Values.ToList();
        }

        private async void AttachImage()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            var files = (await picker.PickMultipleFilesAsync()).ToList();
            var howManyFilesCanAttach = MaxAttachedFiles - AttachedImages.Count;
            if (files.Count > howManyFilesCanAttach)
                files.RemoveRange(howManyFilesCanAttach, files.Count - howManyFilesCanAttach);

            AttachImages(files);
        }

        private void Insert(string text)
        {
            var selStart = SelectionStart;
            var selLen = SelectionLength;
            Text = Text.Insert(selStart, text);
            SelectionStart = selStart + selLen + text.Length;
        }

        private void Tag(string tag)
        {
            var selStart = SelectionStart;
            var selLen = SelectionLength;
            var first = $"[{tag}]";
            var second = $"[/{tag}]";
            Text = Text
                .Replace("\r\n", "\n")
                .Insert(selStart + selLen, second)
                .Insert(selStart, first);
            SelectionStart = selStart + selLen + first.Length + second.Length;
        }

        private async void Send()
        {
            try
            {
                IsWorking = true;
                CurrentJob = "Отправка";
                await Thread.Reply(PostInfo);
                PostInfo.Files?.Clear();
                Text = "";
                NavigationService.GoBack();
            }
            catch (ApiException e)
            {
                await new MessageDialog(e.Message, "Ошибка").ShowAsync();
            }
            finally
            {
                IsWorking = false;
            }
        }

        private async void AttachImages(IEnumerable<StorageFile> images)
        {
            if (images == null)
                return;

            foreach (var file in images)
            {
                var image = new BitmapImage();
                var stream = await file.OpenAsync(FileAccessMode.Read);
                _attachedStorageFiles.Add(image, file);
                image.SetSource(stream);
                AttachedImages.Add(image);
            }
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var navigationInfo = parameter as PostingPageNavigationInfo;
            if (mode != NavigationMode.New || navigationInfo == null)
                return;

            PostInfo = navigationInfo.PostInfo;
            Thread = navigationInfo.Thread;
            AttachImages(PostInfo?.Files);
        }
    }
}

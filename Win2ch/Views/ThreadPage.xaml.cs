using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Win2ch.Controls;
using Win2ch.Models;
using Win2ch.ViewModels;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win2ch.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ThreadPage
    {
        public ThreadViewModel ViewModel => DataContext as ThreadViewModel;

        public ThreadPage()
        {
            InitializeComponent();
        }

        private void Header_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (Posts.Items != null && Posts.Items.Count > 0)
                Posts.ScrollIntoView(Posts.Items[0]);
        }

        private void ScrollDown_OnClicked(object sender, RoutedEventArgs e)
        {
            if (Posts.Items != null && Posts.Items.Count > 0)
                Posts.ScrollIntoView(Posts.Items[Posts.Items.Count - 1]);
        }
        

        private void PostControl_OnReply(object sender, PostReplyEventArgs e)
        {
            string textToAdd;

            if (e.SelectedText.Length > 0)
            {
                var pre = FastReplyTextBox.SelectionStart > 0 ? "\n" : "";
                textToAdd = $"{pre}>>{e.Post.Num}\n> {e.SelectedText}\n";
            }
            else
                textToAdd = $">>{e.Post.Num}\n";

            var selectionIndex = FastReplyTextBox.SelectionStart;
            FastReplyTextBox.Text = FastReplyTextBox.Text.Insert(selectionIndex, textToAdd);
            FastReplyTextBox.SelectionStart = selectionIndex + textToAdd.Length;
        }

        private void FastReply_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.FastReplyCommand.Execute(null);
            FastReplyTextBox.Text = "";
        }

        private void PostControl_OnImageClick(object sender, ImageClickEventArgs e)
        {
            ViewModel.ShowImageCommand.Execute(e.ImageInfo);
        }
    }
}

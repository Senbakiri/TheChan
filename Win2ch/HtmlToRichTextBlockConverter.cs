using Windows.Data.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Win2ch
{
    class HtmlToRichTextBlockConverter : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(HtmlToRichTextBlockConverter), new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, object value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        public static object GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rich = d as RichTextBlock;
            if (rich == null || e.NewValue == null) return;

            var html = e.NewValue.ToString();

            rich.Blocks.Clear();

            var p = new Paragraph();
            p.Inlines.Add(new Run { Text = HtmlUtilities.ConvertToText(html) });
            rich.Blocks.Add(p);
        }
    }
}

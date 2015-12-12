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

            Paragraph p;

            if (rich.Blocks.Count == 0 || !(rich.Blocks[0] is Paragraph))
            {
                p = new Paragraph();
                rich.Blocks.Clear();
                rich.Blocks.Add(p);
            }
            else
            {
                p = (Paragraph) rich.Blocks[0];;
                p.Inlines.Clear();
            }

            p.Inlines.Add(new Run { Text = HtmlUtilities.ConvertToText(html) });
        }
    }
}

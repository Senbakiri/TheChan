using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Documents;
using HtmlAgilityPack;

namespace Win2ch.Common.Html {
    public class InlineWrapper {
        public ICollection<Inline> Children { get; set; }
        public string Text { get; set; }
        private Action<Inline> _styleAction;
        private Func<InlineWrapper, Inline> _converter = StandartConverter; 

        public InlineWrapper(IEnumerable<Inline> children) {
            Children = new List<Inline>(children);
        }

        public InlineWrapper(string text) {
            Text = text;
        }

        public InlineWrapper(IEnumerable<Inline> children, string text)
            : this(children) {
            Text = text;
        }

        public static Inline StandartConverter(InlineWrapper wrapper) {
            Inline inline;
            if (wrapper.Children?.Count > 0)
                inline = wrapper.CreateSpanWithChildren();
            else
                inline = new Run { Text = HtmlEntity.DeEntitize(wrapper.Text) ?? string.Empty };
            return inline;
        }

        public Inline Unwrap() {
            var inline = this._converter(this);
            this._styleAction?.Invoke(inline);
            return inline;
        }

        public InlineWrapper SetConverter(Func<InlineWrapper, Inline> handler) {
            if (handler != null)
                this._converter = handler;
            return this;
        }

        public InlineWrapper Style(Action<Inline> style) {
            this._styleAction += style;
            return this;
        }

        private Span CreateSpanWithChildren() {
            var span = new Span();
            foreach (var inline in Children) {
                this._styleAction?.Invoke(inline);
                span.Inlines.Add(inline);
            }
            return span;
        }
    }
}

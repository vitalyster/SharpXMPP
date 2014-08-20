using System.Collections;
using System.Windows;
using System.Windows.Documents;

namespace SharpXMPP.WPF.Helpers
{
    public class ParagraphInlineBehavior : DependencyObject
    {
        public static readonly DependencyProperty TemplateResourceNameProperty =
            DependencyProperty.RegisterAttached("TemplateResourceName",
                                                typeof(string),
                                                typeof(ParagraphInlineBehavior),
                                                new UIPropertyMetadata(null, OnParagraphInlineChanged));
        public static string GetTemplateResourceName(DependencyObject obj)
        {
            return (string)obj.GetValue(TemplateResourceNameProperty);
        }
        public static void SetTemplateResourceName(DependencyObject obj, string value)
        {
            obj.SetValue(TemplateResourceNameProperty, value);
        }

        public static readonly DependencyProperty ParagraphInlineSourceProperty =
            DependencyProperty.RegisterAttached("ParagraphInlineSource",
                                                typeof(IEnumerable),
                                                typeof(ParagraphInlineBehavior),
                                                new UIPropertyMetadata(null, OnParagraphInlineChanged));
        public static IEnumerable GetParagraphInlineSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(ParagraphInlineSourceProperty);
        }
        public static void SetParagraphInlineSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(ParagraphInlineSourceProperty, value);
        }

        private static void OnParagraphInlineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var paragraph = d as Paragraph;
            var inlines = GetParagraphInlineSource(paragraph);
            var templateName = GetTemplateResourceName(paragraph);
            if (inlines != null && templateName != null)
            {
                paragraph.Inlines.Clear();
                foreach (var inline in inlines)
                {
                    var templateInline = paragraph.FindResource(templateName) as ArrayList;
                    var span = new Span {DataContext = inline};
                    span.Inlines.Add(templateInline[0] as Inline);
                    
                    paragraph.Inlines.Add(span);
                }
            }
        }
    }
}

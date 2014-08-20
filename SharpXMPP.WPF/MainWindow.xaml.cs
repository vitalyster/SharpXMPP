using System.Windows;
using SharpXMPP.WPF.ViewModels;

namespace SharpXMPP.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void MessageDelegate(string block);
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        /*
        void LogXml(XElement args)
        {
            Dispatcher.Invoke(new MessageDelegate(AddStanza), args.ToString());
        }

        private void Button1Click(object sender, RoutedEventArgs e)
        {
            client = new XmppClientConnection(new JID(textBox1.Text), passwordBox1.Password) { InitialPresence = true};
            client.Element += (o, args) => Dispatcher.Invoke((Action)(() => LogXml(args.Stanza)));
            ThreadPool.QueueUserWorkItem((o) => client.Connect());
        }

        private static Color ColorForToken(XmlToken token, string tokenText)
        {
            var color = Colors.White;
            switch (token.Kind)
            {
                case XmlTokenKind.Open:
                case XmlTokenKind.OpenClose:
                case XmlTokenKind.Close:
                case XmlTokenKind.SelfClose:
                case XmlTokenKind.CommentBegin:
                case XmlTokenKind.CommentEnd:
                case XmlTokenKind.CDataBegin:
                case XmlTokenKind.CDataEnd:
                case XmlTokenKind.Equals:
                case XmlTokenKind.OpenProcessingInstruction:
                case XmlTokenKind.CloseProcessingInstruction:
                case XmlTokenKind.AttributeValue:
                    color = Colors.Blue;
                    break;
                case XmlTokenKind.ElementName:
                    color = Colors.Brown;
                    break;
                case XmlTokenKind.TextContent:
                    color = Colors.White;
                    break;
                case XmlTokenKind.AttributeName:
                case XmlTokenKind.Entity:
                    color = Colors.Red;
                    break;
                case XmlTokenKind.CommentText:
                    color = Colors.Green;
                    break;
            }

            return color;
        }

        private void AddStanza(string text)
        {
            var tokenizer = new XmlTokenizer();
            var mode = XmlTokenizerMode.OutsideElement;
            var xml = text;

            var tokens = tokenizer.Tokenize(xml, ref mode);
            var tokenTexts = new List<string>(tokens.Count);
            var colors = new List<Color>(tokens.Count);
            var position = 0;
            foreach (var token in tokens)
            {
                var tokenText = xml.Substring(position, token.Length);
                tokenTexts.Add(tokenText);
                var color = ColorForToken(token, tokenText);
                colors.Add(color);
                position += token.Length;
            }
            var para = new Paragraph();
            for (var i = 0; i < tokens.Count; i++)
            {
                var run = new Run { Foreground = new SolidColorBrush(colors[i]), Text = tokenTexts[i] };
                para.Inlines.Add(run);
            }
            doc.Blocks.Add(para);
            DependencyObject DO = scroll;

            bool val = false;

            ScrollViewer sv = null;

            while (val == false)
            {

                DO = VisualTreeHelper.GetChild(DO, 0);

                if (!(DO is ScrollViewer)) continue;
                sv = DO as ScrollViewer;

                val = true;
            }

            sv.ScrollToBottom();
        }
         */
    }
}

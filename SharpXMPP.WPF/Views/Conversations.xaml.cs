using SharpXMPP.WPF.ViewModels;

namespace SharpXMPP.WPF.Views
{
    /// <summary>
    /// Interaction logic for Conversations.xaml
    /// </summary>
    public partial class Conversations
    {
        public Conversations()
        {
            InitializeComponent();
            DataContext = new ConversationsViewModel();
        }
    }
}

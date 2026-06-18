using MemoirDraft.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationView.xaml
    /// </summary>
    public partial class AuthorizationView : Window
    {
        public AuthorizationView(AuthorizationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }


        private void HeaderBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}

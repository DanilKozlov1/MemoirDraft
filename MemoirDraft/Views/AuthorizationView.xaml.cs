using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationView.xaml
    /// </summary>
    public partial class AuthorizationView : Window
    {
        private readonly LoginPage _loginPage;
        private readonly RegisterPage _registerPage;

        public AuthorizationView()
        {
            InitializeComponent();

            _loginPage = new LoginPage();
            _registerPage = new RegisterPage();

            ContentFrame.Navigate(_loginPage);
        }

        private void LoginTab_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(_loginPage);
        }

        private void RegisterTab_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(_registerPage);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HeaderBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}

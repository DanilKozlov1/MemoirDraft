using MemoirDraft.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowModel viewModel)
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
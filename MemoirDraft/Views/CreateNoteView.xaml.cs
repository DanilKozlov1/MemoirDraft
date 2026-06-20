using MemoirDraft.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.Views
{
    public partial class CreateNoteView : Window
    {
        public CreateNoteView(CreateNoteViewModel viewModel)
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
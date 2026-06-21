using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.Views
{
    public partial class NoteView : Window
    {
        public NoteView()
        {
            InitializeComponent();
        }


        private void HeaderBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
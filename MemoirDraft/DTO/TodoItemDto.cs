using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoirDraft.DTO
{
    public class TodoItemDto : INotifyPropertyChanged
    {
        private int _id;
        private string _text = string.Empty;
        private bool _isDone;

        public int Id
        {
            get => _id;
            set 
            {
                if (_id != value) 
                { 
                    _id = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public string Text
        {
            get => _text;
            set 
            { 
                if (_text != value) 
                { 
                    _text = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public bool IsDone
        {
            get => _isDone;
            set 
            { 
                if (_isDone != value) 
                { 
                    _isDone = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

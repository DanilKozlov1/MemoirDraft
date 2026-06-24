using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoirDraft.DTO
{
    public class NoteDto : INotifyPropertyChanged
    {
        private int _id;
        public string _title = string.Empty;
        public string _noteTypeName = string.Empty;
        public bool _isFavorite;

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

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NoteTypeName
        {
            get => _noteTypeName;
            set
            {
                if (_noteTypeName != value)
                {
                    _noteTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
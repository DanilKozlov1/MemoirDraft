using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoirDraft.DTO
{
    public class FilterDto : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private bool _isActive;
        private bool _isSpecial;

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

        public string Name
        {
            get => _name;
            set 
            { 
                if (_name != value) 
                { 
                    _name = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set 
            { 
                if (_isActive != value) 
                { 
                    _isActive = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public bool IsSpecial
        {
            get => _isSpecial;
            set 
            { 
                if (_isSpecial != value) 
                { 
                    _isSpecial = value; 
                    OnPropertyChanged(); 
                } 
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
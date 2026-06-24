using MemoirDraft.DTO.Abstractions;

namespace MemoirDraft.DTO
{
    public class FilterDto : ObservableDto
    {
        private int _id;
        private string _name = string.Empty;
        private bool _isActive;
        private bool _isSpecial;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public bool IsSpecial
        {
            get => _isSpecial;
            set => SetProperty(ref _isSpecial, value);
        }
    }
}
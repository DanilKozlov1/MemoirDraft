using MemoirDraft.DTO.Abstractions;

namespace MemoirDraft.DTO
{
    public class CreateNotePageDto : ObservableDto
    {
        private string _name = string.Empty;
        private bool _isActive;
        private object? _content;

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

        public object? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
    }
}
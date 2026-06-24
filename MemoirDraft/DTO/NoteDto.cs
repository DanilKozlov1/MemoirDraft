using MemoirDraft.DTO.Abstractions;

namespace MemoirDraft.DTO
{
    public class NoteDto : ObservableDto
    {
        private int _id;
        public string _title = string.Empty;
        public string _noteTypeName = string.Empty;
        public bool _isFavorite;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string NoteTypeName
        {
            get => _noteTypeName;
            set => SetProperty(ref _noteTypeName, value);
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }
    }
}
using MemoirDraft.DTO.Abstractions;

namespace MemoirDraft.DTO
{
    public class TodoItemDto : ObservableDto
    {
        private int _id;
        private string _text = string.Empty;
        private bool _isDone;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public bool IsDone
        {
            get => _isDone;
            set => SetProperty(ref _isDone, value);
        }
    }
}

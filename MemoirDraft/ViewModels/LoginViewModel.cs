using MemoirDraft.Services;
using MemoirDraft.ViewModels.Abstractions;

namespace MemoirDraft.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private SessionService _sessionService;


        public LoginViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;
        }
    }
}
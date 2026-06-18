using MemoirDraft.Commands;
using MemoirDraft.Services;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private SessionService _sessionService;

        public ICommand TestCommand { get; }


        public RegisterViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            TestCommand = new RelayCommand(() =>
            {
                MessageBox.Show("Test");
            });
        }
    }
}
using System.Windows.Input;

namespace MemoirDraft.Commands
{
    /// <summary>
    /// Реализация команд, синхронная
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Выполняемая функция
        /// </summary>
        private readonly Action<object> _execute;
        /// <summary>
        /// Функция, что проверяет возможность выполнения
        /// </summary>
        private readonly Func<object, bool>? _canExecute;

        /// <summary>
        /// Событие, возникающее при изменении возможности выполнения команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(), _ => canExecute?.Invoke() ?? true) { }


        /// <summary>
        /// Может ли выполнится команда
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        /// <returns>Выполнилась ли команда</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            _execute(parameter);
            CommandManager.InvalidateRequerySuggested();
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
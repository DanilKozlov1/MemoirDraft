using MemoirDraft.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace MemoirDraft.Services
{
    public class WindowsService
    {
        private readonly ILogger<WindowsService> _logger;

        private readonly IServiceProvider _services;

        /// <summary>
        /// Словарь открытых, не модальных, окон
        /// </summary>
        private readonly Dictionary<Type, Window> _openedWindows;


        public WindowsService(ILogger<WindowsService> logger, IServiceProvider services)
        {
            _logger = logger;

            _services = services;

            _openedWindows = new Dictionary<Type, Window>();
        }


        /// <summary>
        /// Открытие окна
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        private void OpenWindow<TView>() where TView : Window
        {
            var type = typeof(TView);

            if (_openedWindows.TryGetValue(type, out var window) && window.IsVisible)
            {
                _logger.LogInformation("Окно {WindowName} уже открыто. Перевод фокуса на него (Активация).", type.Name);
                window.Activate();
                return;
            }

            _logger.LogInformation("Инициализация и открытие немодального окна: {WindowName}.", type.Name);

            var win = _services.GetRequiredService<TView>();
            win.Closed += (s, e) =>
            {
                _logger.LogInformation("Немодальное окно {WindowName} было закрыто пользователем.", type.Name);
                _openedWindows.Remove(type);
            };

            win.Show();
            _openedWindows[type] = win;

            if (typeof(TView) != typeof(AuthorizationView))
                Application.Current.MainWindow = win;
        }

        /// <summary>
        /// Открытие окна как модальное
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        /// <returns>Результат работы окна - DialogResult</returns>
        private bool? OpenModalWindow<TView>() where TView : Window
        {
            var type = typeof(TView);
            _logger.LogInformation(
                "Открытие модального диалогового окна: {WindowName}.",
                type.Name
            );

            var win = _services.GetRequiredService<TView>();

            var result = win.ShowDialog();
            _logger.LogInformation(
                "Модальное окно {WindowName} закрыто. DialogResult: {Result}",
                type.Name, result
            );

            return result;
        }

        public void OpenAuthorization() => OpenWindow<AuthorizationView>();

        public void OpenMainWindow() => OpenWindow<MainWindow>();

        public bool? OpenCreateNote() => OpenModalWindow<CreateNoteView>();

        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="dataContext">Контекст нужного окна</param>
        public void CloseWindow(object dataContext)
        {
            var target = dataContext ?? this;

            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == target);

            if (window != null)
            {
                _logger.LogInformation(
                    "Запрос на закрытие окна для ViewModel: {ViewModelName}",
                    target.GetType().Name
                );

                window.Close();
            }
        }
        /// <summary>
        /// Закрытие модального окна
        /// </summary>
        /// <param name="dataContext">Контекст нужного окна</param>
        /// <param name="dialogResult">Результат диалога</param>
        public void CloseWindow(object dataContext, bool dialogResult)
        {
            var target = dataContext ?? this;

            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == target);

            if (window != null)
            {
                _logger.LogInformation(
                    "Запрос на закрытие окна для ViewModel: {ViewModelName}",
                    target.GetType().Name
                );

                window.DialogResult = dialogResult;
                window.Close();
            }
        }
    }
}
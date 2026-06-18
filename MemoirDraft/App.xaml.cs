using MemoirDraft.Database;
using MemoirDraft.Repositories;
using MemoirDraft.Repositories.Interfaces;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.Utils;
using MemoirDraft.ViewModels;
using MemoirDraft.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace MemoirDraft
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Контейнер
        /// </summary>
        public static IServiceProvider? Services { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Конфигурация
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            // Регистарция Serilog
            SerilogConfigurator.ConfigureLogging(services, config);

            // База данных
            services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(config.GetConnectionString("Default")));

            // Репозитории
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INoteTypeRepository, NoteTypeRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();

            // Сервисы
            services.AddSingleton<SessionService>();
            services.AddSingleton<WindowsService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INoteTypeService, NoteTypeService>();
            services.AddScoped<INoteService, NoteService>();

            // ViewModels
            services.AddTransient<AuthorizationViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();

            // Views
            services.AddTransient<AuthorizationView>();
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();

            Services = services.BuildServiceProvider();

            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                if (db.Database.CanConnect())
                    Log.Information("Успешное подключение к СУБД PostgreSQL.");
                else
                {
                    Log.Fatal("КРИТИЧЕСКАЯ ОШИБКА: База данных PostgreSQL недоступна или строка подключения неверна.");
                    MessageBox.Show("Ошибка запуска. Проверьте логи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                    Shutdown();
                    return;
                }

                db.Database.Migrate();

                var startupScope = Services.CreateScope();
                var win = startupScope.ServiceProvider.GetRequiredService<AuthorizationView>();

                Log.Information("Приложение запущено.");
                win.Show();
            }
            catch (Exception ex)
            {
                Log.Fatal("Критическая ошибка приложения: {exMessage}", ex);
                MessageBox.Show("Ошибка запуска. Проверьте логи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Приложение завершает свою работу.");
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}
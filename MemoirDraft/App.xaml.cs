using MemoirDraft.Database;
using MemoirDraft.Repositories;
using MemoirDraft.Repositories.Interfaces;
using MemoirDraft.Services;
using MemoirDraft.Services.DatabaseNoteMode;
using MemoirDraft.Services.FileOnlyNoteMode;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.Utils;
using MemoirDraft.ViewModels;
using MemoirDraft.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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


        /// <summary>
        /// Добавление репозиториев в контейнер
        /// </summary>
        /// <param name="services">Контейнер</param>
        private void AddRepositories(ServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INoteTypeRepository, NoteTypeRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
        }

        /// <summary>
        /// Добавление базовых сервисов в контейнер
        /// </summary>
        /// <param name="services">Контейнер</param>
        private void AddBaseServices(ServiceCollection services, string storageMode)
        {
            services.AddSingleton<SessionService>();
            services.AddSingleton<WindowsService>();
            services.AddSingleton<IFileStorageService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<FileStorageService>>();
                var config = provider.GetRequiredService<IConfiguration>();
                return new FileStorageService(logger, config, storageMode);
            });

            services.AddSingleton<SyncService>();
            
            services.AddScoped<IDataPortService, DataPortService>();

            services.AddScoped<IUserService, UserService>();
        }

        /// <summary>
        /// Добавление сервиса сохранения заметок с учётом StorageMode в контейнер
        /// </summary>
        /// <param name="services">Контейнер</param>
        private void SetStorageMode(ServiceCollection services, string storageMode)
        {
            switch (storageMode)
            {
                case "FileOnly":
                    services.AddScoped<INoteTypeService, FileOnlyNoteTypeService>();
                    services.AddScoped<INoteService, FileOnlyNoteService>();

                    break;
                case "DatabaseOnly":
                    services.AddScoped<INoteTypeService, NoteTypeService>();
                    services.AddScoped<INoteService, NoteService>();

                    break;
                case "DatabaseAndFile":
                default:
                    services.AddScoped<INoteTypeService, NoteTypeService>();
                    services.AddScoped<NoteService>();
                    services.AddScoped<FileOnlyNoteService>();

                    services.AddScoped<INoteService>(provider =>
                    {
                        var db = provider.GetRequiredService<NoteService>();
                        var file = provider.GetRequiredService<FileOnlyNoteService>();
                        var logger = provider.GetRequiredService<ILogger<FullNoteService>>();
                        return new FullNoteService(logger, db, file);
                    });

                    break;
            }
        }

        /// <summary>
        /// Добавление моделей View в контейнер
        /// </summary>
        /// <param name="services">Контейнер</param>
        private void AddViewModels(ServiceCollection services)
        {
            services.AddTransient<AuthorizationViewModel>();
            services.AddTransient<LoginPageModel>();
            services.AddTransient<RegisterPageModel>();
            services.AddTransient<MainWindowModel>();
            services.AddTransient<CreateNoteViewModel>();
            services.AddTransient<SimpleNotePageModel>();
            services.AddTransient<TodoNotePageModel>();
            services.AddTransient<NoteViewModel>();
        }

        /// <summary>
        /// Добавление View в контейнер
        /// </summary>
        /// <param name="services">Контейнер</param>
        private void AddViews(ServiceCollection services)
        {
            services.AddTransient<AuthorizationView>();
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<MainWindow>();
            services.AddTransient<CreateNoteView>();
            services.AddTransient<SimpleNotePage>();
            services.AddTransient<TodoNotePage>();
            services.AddTransient<NoteView>();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var storageMode = config.GetValue<string>("Storage:Mode") ?? "DatabaseAndFile";

            if (storageMode != "FileOnly")
            {
                try
                {
                    var connectionString = config.GetConnectionString("Default");
                    var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
                    optionsBuilder.UseNpgsql(connectionString);

                    using var initContext = new AppDBContext(optionsBuilder.Options);
                    initContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Log.Error($"Ошибка БД: {ex.Message}");
                    storageMode = "FileOnly";
                }
            }

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            SerilogConfigurator.ConfigureLogging(services, config);

            services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(config.GetConnectionString("Default")));

            AddRepositories(services);
            AddBaseServices(services, storageMode);

            SetStorageMode(services, storageMode);
            
            AddViewModels(services);
            AddViews(services);

            Services = services.BuildServiceProvider();

            if (storageMode == "FileOnly")
                Log.ForContext("SourceContext", "App").Warning("Запуск в режиме FileOnly (БД недоступна или отключена).");
            else
                Log.ForContext("SourceContext", "App").Information("Успешный запуск. База данных PostgreSQL готова к работе.");

            var startupScope = Services.CreateScope();
            Window win = startupScope.ServiceProvider.GetRequiredService<MainWindow>();

            if (config.GetValue<string>("Settings:AppMode") == "Auth")
                win = startupScope.ServiceProvider.GetRequiredService<AuthorizationView>();

            win.Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            Log.ForContext("SourceContext", "App")
                .Information("Приложение завершает свою работу.");
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using CommunityToolkit.Maui.Storage;
using Diary.Clients;
using Diary.Clients.Interfaces;
using Diary.Commands;
using Diary.Commands.Interfaces;
using Diary.Platforms;
using Diary.Repositories;
using Diary.Repositories.Interfaces;
using Diary.Resources.Fonts;
using Diary.Services;
using Diary.Services.Interfaces;
using Diary.ViewModels.Entry;
using Diary.ViewModels.Interfaces;
using Diary.ViewModels.Label;
using Diary.ViewModels.Map;
using Diary.ViewModels.Media;
using Diary.Views;
using Diary.Views.Entry;
using Diary.Views.ImportExport;
using Diary.Views.Label;
using Diary.Views.Map;
using Diary.Views.Media;
using Diary.Views.Template;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace Diary
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
#if ANDROID
                .UseLocalNotification()
                .UseMauiMaps()
#elif WINDOWS
                .UseMauiCommunityToolkitMaps(Constants.BingMapsApiKey)
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("FontAwesome-Solid.ttf", Fonts.FontAwesome);
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Regular.ttf", Fonts.PoppinsRegular);
                    fonts.AddFont("Poppins-Bold.ttf", Fonts.PoppinsBold);
                });

            ConfigureServices(builder.Services);
            ConfigureRepositories(builder.Services);
            ConfigureClients(builder.Services);
            ConfigureViewModels(builder.Services);
            ConfigureViews(builder.Services);
            ConfigurePopups(builder.Services);


#if DEBUG
            builder.Logging.AddDebug();
#endif

            RegisterRoutes();

            var app = builder.Build();

            SetupDirectories();

            Task.Run(async () => await SetupDatabaseAsync(app)).GetAwaiter().GetResult();

            return app;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGlobalExceptionService, GlobalExceptionService>();
            services.AddSingleton<IGlobalExceptionServiceInitializer, GlobalExceptionServiceInitializer>();
            services.AddSingleton<ICommandFactory, CommandFactory>();
            services.AddSingleton<IFilePicker>(FilePicker.Default);
            services.AddSingleton<IFileSaver>(FileSaver.Default);
            services.AddSingleton<IMediaPicker>(MediaPicker.Default);
            services.AddSingleton<IImportExportService, ImportExportService>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddSingleton<IMediaRepository, MediaRepository>();
            services.AddSingleton<IEntryRepository, EntryRepository>();
            services.AddSingleton<ILabelRepository, LabelRepository>();
            services.AddSingleton<ITemplateRepository, TemplateRepository>();
            services.AddSingleton<ILabelEntryRepository, LabelEntryRepository>();
            services.AddSingleton<ILabelTemplateRepository, LabelTemplateRepository>();
            services.AddSingleton<IEntryMediaRepository, EntryMediaRepository>();
        }

        private static void ConfigureClients(IServiceCollection services)
        {
            services.AddSingleton<IMediaClient, MediaClient>();
            services.AddSingleton<IEntryClient, EntryClient>();
            services.AddSingleton<ILabelClient, LabelClient>();
            services.AddSingleton<ITemplateClient, TemplateClient>();
        }

        private static void ConfigureViews(IServiceCollection services)
        {
            services.Scan(selector => selector
                .FromAssemblyOf<App>()
                .AddClasses(filter => filter.AssignableTo<ContentPageBase>())
                .AsSelf()
                .WithTransientLifetime());
        }

        private static void ConfigureViewModels(IServiceCollection services)
        {
            services.Scan(selector => selector
                .FromAssemblyOf<App>()
                .AddClasses(filter => filter.AssignableTo<IViewModel>())
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
        }

        private static void ConfigurePopups(IServiceCollection services)
        {
            services.AddTransientPopup<MapPopupView, MapPopupViewModel>();
            services.AddTransientPopup<MediaPopupView, MediaPopupViewModel>();
            services.AddTransientPopup<ColorPickerPopupView, ColorPickerPopupViewModel>();
            services.AddTransientPopup<FilterSortPopupView, FilterSortPopupViewModel>();
        }

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute("//labels/edit", typeof(LabelEditView));
            Routing.RegisterRoute("//labels/create", typeof(LabelCreateView));

            Routing.RegisterRoute("//entries/detail", typeof(EntryDetailView));
            Routing.RegisterRoute("//entries/edit", typeof(EntryEditView));
            Routing.RegisterRoute("//entries/create", typeof(EntryCreateView));
            Routing.RegisterRoute("//entries/timeMachine", typeof(TimeMachineView));
            Routing.RegisterRoute("//entries/timeMachine/detail", typeof(EntryDetailView));

            Routing.RegisterRoute("//templates/detail", typeof(TemplateDetailView));
            Routing.RegisterRoute("//templates/edit", typeof(TemplateEditView));
            Routing.RegisterRoute("//templates/create", typeof(TemplateCreateView));

            Routing.RegisterRoute("//importexport", typeof(ImportExportView));
        }

        private static void SetupDirectories()
        {
            SetupDirectory(Constants.AppFolder);
            SetupDirectory(Constants.MediaPath);
            SetupDirectory(Constants.TempPath);
        }

        private static void SetupDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static async Task SetupDatabaseAsync(MauiApp app)
        {
            if (!File.Exists(Constants.DatabasePath))
            {
                var mediaRepository = app.Services.GetRequiredService<IMediaRepository>();
                var entryRepository = app.Services.GetRequiredService<IEntryRepository>();
                var labelRepository = app.Services.GetRequiredService<ILabelRepository>();
                var templateRepository = app.Services.GetRequiredService<ITemplateRepository>();
                var labelEntryRepository = app.Services.GetRequiredService<ILabelEntryRepository>();
                var labelTemplateRepository = app.Services.GetRequiredService<ILabelTemplateRepository>();
                var entryMediaRepository = app.Services.GetRequiredService<IEntryMediaRepository>();

                await mediaRepository.CreateTableAsync();
                await entryRepository.CreateTableAsync();
                await labelRepository.CreateTableAsync();
                await templateRepository.CreateTableAsync();
                await labelEntryRepository.CreateTableAsync();
                await labelTemplateRepository.CreateTableAsync();
                await entryMediaRepository.CreateTableAsync();

                await DataSeedService.SeedAsync(entryRepository, labelRepository, templateRepository);
            }
        }
    }
}

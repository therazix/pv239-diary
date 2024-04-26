using CommunityToolkit.Maui;
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
using Diary.Views;
using Diary.Views.Entry;
using Diary.Views.Label;
using Diary.Views.Template;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;

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


#if DEBUG
            builder.Logging.AddDebug();
#endif

            RegisterRoutes();

            var app = builder.Build();

            Task.Run(async () => await SetupDatabaseAsync(app)).GetAwaiter().GetResult();

            return app;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => SecureStorage.Default);
            services.AddSingleton<IGlobalExceptionService, GlobalExceptionService>();
            services.AddSingleton<IGlobalExceptionServiceInitializer, GlobalExceptionServiceInitializer>();
            services.AddSingleton<ICommandFactory, CommandFactory>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddSingleton<IEntryRepository, EntryRepository>();
            services.AddSingleton<ILabelRepository, LabelRepository>();
            services.AddSingleton<ITemplateRepository, TemplateRepository>();
            services.AddSingleton<ILabelEntryRepository, LabelEntryRepository>();
            services.AddSingleton<ILabelTemplateRepository, LabelTemplateRepository>();
        }

        private static void ConfigureClients(IServiceCollection services)
        {
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

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute("//labels/edit", typeof(LabelEditView));
            Routing.RegisterRoute("//labels/create", typeof(LabelCreateView));

            Routing.RegisterRoute("//entries/detail", typeof(EntryDetailView));
            Routing.RegisterRoute("//entries/edit", typeof(EntryEditView));
            Routing.RegisterRoute("//entries/create", typeof(EntryCreateView));

            Routing.RegisterRoute("//templates/detail", typeof(TemplateDetailView));
            Routing.RegisterRoute("//templates/edit", typeof(TemplateEditView));
            Routing.RegisterRoute("//templates/create", typeof(TemplateCreateView));
        }

        private static async Task SetupDatabaseAsync(MauiApp app)
        {
            var secureStorage = app.Services.GetRequiredService<ISecureStorage>();
            //secureStorage.Remove(Constants.FirstRunKey);
            var isFirstRun = await secureStorage.GetAsync(Constants.FirstRunKey);

            if (string.IsNullOrEmpty(isFirstRun))
            {
                var directory = Path.GetDirectoryName(Constants.DatabasePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var entryRepository = app.Services.GetRequiredService<IEntryRepository>();
                var labelRepository = app.Services.GetRequiredService<ILabelRepository>();
                var templateRepository = app.Services.GetRequiredService<ITemplateRepository>();
                var labelEntryRepository = app.Services.GetRequiredService<ILabelEntryRepository>();
                var labelTemplateRepository = app.Services.GetRequiredService<ILabelTemplateRepository>();

                await entryRepository.CreateTableAsync();
                await labelRepository.CreateTableAsync();
                await templateRepository.CreateTableAsync();
                await labelEntryRepository.CreateTableAsync();
                await labelTemplateRepository.CreateTableAsync();

                await DataSeedService.SeedAsync(entryRepository, labelRepository, templateRepository);

                await secureStorage.SetAsync(Constants.FirstRunKey, "false");
            }
        }
    }
}

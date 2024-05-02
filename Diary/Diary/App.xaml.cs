using Diary.Clients.Interfaces;
using Diary.Helpers;
using Diary.Services.Interfaces;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace Diary;
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        InitializeComponent();

        MainPage = new AppShell();

        LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
    }

    private async void Current_NotificationActionTapped(NotificationActionEventArgs e)
    {
        var deliveredNotifications = await LocalNotificationCenter.Current.GetDeliveredNotificationList();
        var notificationIds = deliveredNotifications.Select(n => n.NotificationId);

        if (notificationIds.Contains(NotificationHelper.GetNotificationIdFromCreationDate(DateTime.Now)))
        {
            await Shell.Current.GoToAsync("//entries/timeMachine");
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        var globalExceptionServiceInitializer = _serviceProvider.GetRequiredService<IGlobalExceptionServiceInitializer>();
        globalExceptionServiceInitializer.Initialize();
    }
}

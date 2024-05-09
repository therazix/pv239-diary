namespace Diary.Helpers;

public static class LocationHelper
{
    public static async Task<bool> HasLocationPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status == PermissionStatus.Granted;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<Location?> GetAnyLocationAsync()
    {
        var location = await GetCachedLocationAsync();
        if (location == null)
        {
            location = await GetCurrentLocationAsync();
        }
        return location;
    }


    public static async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
            return await Geolocation.Default.GetLocationAsync(request);
        }

        catch
        {
            return null;
        }
    }

    public static async Task<Location?> GetCachedLocationAsync()
    {
        try
        {
            return await Geolocation.Default.GetLastKnownLocationAsync();
        }
        catch
        {
            return null;
        }
    }
}

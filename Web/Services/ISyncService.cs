using Web.Models;

namespace Web.Services;

public interface ISyncService
{
    BusyTask? GetCurrentSyncTask();
    Task SyncAll();
}
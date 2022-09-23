using System.Runtime.CompilerServices;
using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ISettingsService
{
    Task<SettingsResource> GetSettings();
    Task UpdateSettings(SettingsResource settings);
}
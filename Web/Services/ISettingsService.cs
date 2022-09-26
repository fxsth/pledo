using System.Runtime.CompilerServices;
using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ISettingsService
{
    Task<IEnumerable<SettingsResource>> GetSettings();
    Task UpdateSettings(IEnumerable<SettingsResource> settings);
}
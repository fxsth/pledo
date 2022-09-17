using System.Runtime.CompilerServices;
using Web.Models;

namespace Web.Services;

public interface ISettingsService
{
    Task<Directories> GetDirectories();
    Task UpdateDirectories(Directories directories);
}
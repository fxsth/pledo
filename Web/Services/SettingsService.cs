using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using Web.Models.DTO;
using DbContext = Web.Data.DbContext;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private const string MovieDirectoryKey = "MovieDirectoryPath";
    private const string EpisodeDirectoryKey = "EpisodeDirectoryPath";
    private readonly UnitOfWork _unitOfWork;
    private readonly DbContext _dbContext;

    public SettingsService(UnitOfWork unitOfWork, DbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<string> GetMovieDirectory()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(MovieDirectoryKey);
        if (setting == null)
            throw new InvalidOperationException("The movie directory setting is missing in db.");
        return setting.Value;
    }
    
    public async Task<string> GetEpisodeDirectory()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(EpisodeDirectoryKey);
        if (setting == null)
            throw new InvalidOperationException("The episode directory setting is missing in db.");
        return setting.Value;
    }

    public async Task<IEnumerable<SettingsResource>> GetSettings()
    {
        var settings = _unitOfWork.SettingRepository.GetAll();
        return settings.Select(x => new SettingsResource()
        {
            Key = x.Key,
            Description = x.Description,
            Value = x.Value,
            Name = x.Name ?? ""
        });
    }

    public async Task UpdateSettings(IEnumerable<SettingsResource> settings)
    {
        foreach (var setting in settings)
        {    
            var settingFromDb = await _unitOfWork.SettingRepository.GetById(setting.Key);
            if (settingFromDb != null)
            {
                settingFromDb.Value = setting.Value;
            }
            else
            {
                await _unitOfWork.SettingRepository.Insert(new KeyValueSetting()
                {
                    Key = setting.Key,
                    Value = setting.Value
                });
            }
        }await _unitOfWork.Save();
    }

    public async Task<bool> ResetDatabase()
    {
        await _dbContext.Database.CloseConnectionAsync();
        bool reset = await _dbContext.Database.EnsureDeletedAsync() && await _dbContext.Database.EnsureCreatedAsync();
        DbInitializer.Initialize(_dbContext);
        return reset;
    }
}
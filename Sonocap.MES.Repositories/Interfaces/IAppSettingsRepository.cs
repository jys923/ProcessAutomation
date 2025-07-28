using SonoCap.MES.Models;
using SonoCap.MES.Repositories.Base;

namespace SonoCap.MES.Repositories.Interfaces
{
    public interface IAppSettingsRepository : IRepositoryBase<AppSettings>
    {
        Task<int> GetNextSequenceAsync(string settingKey);
        Task<int> GetNextSequenceSqlAsync(string settingKey);
        Task<SettingItemResult?> GetSettingValueAndTypeAsync(string settingKey);
        Task SetSettingValueAsync(string settingKey, string value, SettingValueType valueType = SettingValueType.STRING);
    }
}
using CGM.Communication.Log;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data.Repository
{
    public class SettingRepository : BaseRepository<Setting>
    {

        public SettingRepository(CgmUnitOfWork uow):base(uow)
        {

        }

        public Setting GetSettings()
        {
            var query = _uow.Connection.Table<Setting>().Where(v => v.SettingId == 1);
            Setting setting = query.FirstOrDefault();
            
            if (setting==null)
            {
                setting = new Setting();
            };
            if (!string.IsNullOrEmpty( setting.OtherSettingsJson))
            {
                setting.OtherSettings = JsonConvert.DeserializeObject<OtherSettings>(setting.OtherSettingsJson);
            }
            return setting;
        }

        public override void Update(Setting entity)
        {
            entity.OtherSettingsJson = JsonConvert.SerializeObject(entity.OtherSettings);
            base.Update(entity);
        }

        public void CheckSettings()
        {
            var settings = GetSettings();
            if (string.IsNullOrEmpty(settings.NightscoutUrl) || string.IsNullOrEmpty(settings.NightscoutSecretkey))
            {
                Logger.LogInformation("No Nightscout settings entered. Please enter Nightscout settings in the settings page.");
            }
        }
    }
}

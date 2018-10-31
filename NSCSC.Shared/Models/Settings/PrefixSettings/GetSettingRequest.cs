using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCSC.Shared.Models.Settings.PrefixSettings
{
    public class GetSettingRequest : RequestBaseModels
    {
        public List<PrefixSettingsModels> ListSetting { get; set; }
        public GetSettingRequest()
        {
            ListSetting = new List<PrefixSettingsModels>();
        }
    }
}

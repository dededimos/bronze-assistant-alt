using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteApplicationSettings.DTOs
{
    public class GeneralApplicationSettingDTO : DTO
    {
        public string SettingName { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
    }

    public enum GeneralApplicationSettings
    {
        SelectedTheme,
        SelectedLanguage,
        ApplicationVersion
    }

}

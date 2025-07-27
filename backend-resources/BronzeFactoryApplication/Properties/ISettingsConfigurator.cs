using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Properties
{
    public interface ISettingsConfigurator
    {
        Task ConfigureUserSettings();
        Task SetLanguage();
        Task SetStartupTheme();
    }
}

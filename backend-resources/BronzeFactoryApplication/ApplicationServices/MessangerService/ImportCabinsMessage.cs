using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A message when a code import is requested
    /// </summary>
    public class ImportCabinsMessage
    {
        public string CodePrimary { get; set; }
        public CabinSettings? SettingsPrimary { get; set; }
        public string CodeSecondary { get; set; } 
        public CabinSettings? SettingsSecondary { get; set; }
        public string CodeTertiary { get; set; } 
        public CabinSettings? SettingsTertiary { get; set; }
        public string RefPA0 { get; }

        public ImportCabinsMessage(
            (CabinSettings? settings ,string code) primary , 
            (CabinSettings? settings, string code) secondary, 
            (CabinSettings? settings, string code) tertiary,
            string refPA0)
        {
            SettingsPrimary = primary.settings;
            CodePrimary = primary.code;
            SettingsSecondary = secondary.settings;
            CodeSecondary = secondary.code;
            SettingsTertiary = tertiary.settings;
            CodeTertiary = tertiary.code;
            RefPA0 = refPA0;
        }

        public (CabinSettings? settings , string code) GetPrimaryMessage()
        {
            return (SettingsPrimary, CodePrimary);
        }
        public (CabinSettings? settings, string code) GetSecondaryMessage()
        {
            return (SettingsSecondary, CodeSecondary);
        }
        public (CabinSettings? settings, string code) GetTertiaryMessage()
        {
            return (SettingsTertiary, CodeTertiary);
        }
    }
}

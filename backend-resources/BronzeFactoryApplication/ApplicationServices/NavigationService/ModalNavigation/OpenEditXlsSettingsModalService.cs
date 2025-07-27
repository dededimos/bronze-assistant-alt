using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditXlsSettingsModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<XlsSettingsEditModalViewModel> modalVmFactory;
        private readonly Func<XlsSettingsGlassesViewModel> settingsVmFactory;

        public OpenEditXlsSettingsModalService(ModalsContainerViewModel modalsContainer , 
            Func<XlsSettingsEditModalViewModel> modalVmFactory,
            Func<XlsSettingsGlassesViewModel> settingsVmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.modalVmFactory = modalVmFactory;
            this.settingsVmFactory = settingsVmFactory;
        }

        /// <summary>
        /// Opens a Modal to Edit or Create new Settings
        /// </summary>
        /// <param name="settings">The Settings to be Edited</param>
        /// <param name="isNewSetting">Weather those are new Settings</param>
        public void OpenModal(XlsSettingsGlasses settings, bool isNewSetting = false)
        {
            // Create the Modal
            var modal = modalVmFactory.Invoke();
            // Create the Viewmodel of the Settings to be Edited
            var settingsVm = settingsVmFactory.Invoke();
            // Set the Settings of the ViewModel
            settingsVm.SetSettings(settings,isNewSetting);
            //If new Set them as not Selected;
            if (isNewSetting) { settingsVm.IsSelected = false; }
            // Set the ViewModel to the Edit Modal
            modal.SetSettings(settingsVm);
            // Open the Modal
            modalsContainer.OpenModal(modal);
        }
    }
}

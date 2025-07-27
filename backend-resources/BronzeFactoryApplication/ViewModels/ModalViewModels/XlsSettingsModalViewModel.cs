using BronzeFactoryApplication.ApplicationServices.SettingsService;
using BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class XlsSettingsModalViewModel : ModalViewModel
    {
        private readonly IXlsSettingsProvider settingsProvider;
        private readonly CloseModalService closeModalService;
        private readonly OpenEditXlsSettingsModalService openEditSettingsModalService;
        [ObservableProperty]
        private IEnumerable<string> availableSettingsNames = Enumerable.Empty<string>();

        /// <summary>
        /// The Setting Name that is currently selected by the User (ex. in the ComboBox)
        /// </summary>
        [ObservableProperty]
        private string selectedSettingName = string.Empty;

        /// <summary>
        /// The Setting that is currently set as the SelectedSetting for the Xls , (saved as IsSelected in the Database)
        /// </summary>
        [ObservableProperty]
        private string savedSelectedSettingName = string.Empty;
        

        [ObservableProperty]
        private bool isBusy;

        public XlsSettingsModalViewModel(IXlsSettingsProvider settingsProvider,
                                         CloseModalService closeModalService,
                                         OpenEditXlsSettingsModalService openEditSettingsModalService)
        {
            Title = "lngXlsSettings".TryTranslateKey();
            this.settingsProvider = settingsProvider;
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosed += OnModalClosed;
            this.openEditSettingsModalService = openEditSettingsModalService;
        }

        /// <summary>
        /// Informs when the Edit Modal Has Closed so that it can re-Retrieve the Names of a newly saved item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void OnModalClosed(object? sender, ModalClosedEventArgs e)
        {
            if (e.TypeOfClosedModal == typeof(XlsSettingsEditModalViewModel))
            {
                try
                {
                    AvailableSettingsNames = await settingsProvider.GetAvailableSettingsNamesAsync();
                }
                catch (Exception ex)
                {
                    MessageService.LogAndDisplayException(ex);
                }
            }
        }

        public async Task InitilizeViewModel()
        {
            IsBusy = true;
            try
            {
                AvailableSettingsNames = await settingsProvider.GetAvailableSettingsNamesAsync();

                //Set the SettingsName That is Selected
                var selectedSettings = await settingsProvider.GetSelectedSettingsAsync();
                SelectedSettingName = selectedSettings.SettingsName;
                SavedSelectedSettingName = selectedSettings.SettingsName;
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Selects a Setting for Edition but also for the used Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveSettingAsSelected()
        {
            try
            {
                //Set the Currently SelectedSettingName as the Selected Setting
                await settingsProvider.SelectSettingAsync(SelectedSettingName);
                SavedSelectedSettingName = SelectedSettingName;
                MessageService.Information.SaveSuccess();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
        }

        [RelayCommand]
        private async Task EditSetting()
        {
            //Retrieve the currently selected Settings
            var settingsToEdit = await settingsProvider.GetXlsSettingAsync(SelectedSettingName);

            //Pass the Settings to the open Modal Service for Editing
            openEditSettingsModalService.OpenModal(settingsToEdit,false);
        }

        [RelayCommand]
        private async Task CreateNewSetting()
        {
            //Retrieve the Default settings
            var defaultSetting = await settingsProvider.GetDefaultSettings();

            //Pass the Default Setting to the Modal
            openEditSettingsModalService.OpenModal(defaultSetting,true);
        }

        [RelayCommand]
        private void Close()
        {
            closeModalService.CloseModal(this);
        }


        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {

            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }
}

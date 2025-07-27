using BronzeFactoryApplication.ApplicationServices.SettingsService;
using BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class XlsSettingsEditModalViewModel : ModalViewModel
    {
        private readonly IXlsSettingsProvider settingsProvider;
        private readonly CloseModalService closeModalService;
        
        [ObservableProperty]
        private XlsSettingsGlassesViewModel? settings;

        /// <summary>
        /// Weather the Current settings trying to be Edited is the Default Setting
        /// </summary>
        [ObservableProperty]
        private bool isDefaultSetting;

        private bool hasSaved;
        private bool hasEdits;

        public XlsSettingsEditModalViewModel(IXlsSettingsProvider settingsProvider,
            CloseModalService closeModalService)
        {
            this.settingsProvider = settingsProvider;
            this.closeModalService = closeModalService;
            closeModalService.ModalClosing += OnModalClosing;
        }

        /// <summary>
        /// Fires Wheenver there are Edits and Trys to Close without a save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (hasSaved is false && hasEdits && e.ClosingModal == this)
            {
                if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                {
                    e.ShouldCancelClose = true;
                }
            }
        }

        /// <summary>
        /// Set the Settings to be Edited
        /// </summary>
        /// <param name="settings"></param>
        public void SetSettings(XlsSettingsGlassesViewModel settings)
        {
            this.Settings = settings;
            if (settings.IsNewSetting)
            {
                Title = "lngXlsSettingsNew".TryTranslateKey();
                settings.SettingsName = "????";
            }
            else
            {
                if (settings.Id == 1)
                {
                    Title = "lngDefaultSettings".TryTranslateKey();
                    IsDefaultSetting = true;
                }
                else
                {
                    Title = "lngXlsSettingsEdit".TryTranslateKey() + $" : {settings.SettingsName}";
                }
            }
            settings.PropertyChanged += OnSettingChanged;
        }

        /// <summary>
        /// Whenever a Setting changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Note that there are edits and unsubscribe
            hasEdits = true;
            if (Settings != null)
            {
                Settings.PropertyChanged -= OnSettingChanged;
            }
        }

        /// <summary>
        /// Saves the Current Edits if there are any
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SaveEdits()
        {
            if (this.Settings == null) { ThrowHelper.ThrowArgumentNullException(nameof(this.Settings)); }
            if (!hasEdits) { closeModalService.CloseModal(this); }
            try
            {
                Guard.IsNotNullOrEmpty(Settings.SettingsName, nameof(Settings.SettingsName));
                Guard.IsNotEqualTo("????",Settings.SettingsName, nameof(Settings.SettingsName));
                Guard.IsNotNullOrEmpty(Settings.WorksheetName, nameof(Settings.WorksheetName));

                var editedSettings = Settings.ToXlsSettingsGlasses();
                if (Settings.IsNewSetting)
                {
                    editedSettings.Id = 0; //New Setting Autogenerate Id
                    var insertedId = await settingsProvider.AddNewXlsSettingsAsync(editedSettings);
                }
                else
                {
                    await settingsProvider.UpdateXlsSettingsAsync(editedSettings);
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
                return;
            }
            hasSaved = true;
            hasEdits = false;
            MessageService.Information.SaveSuccess();
            closeModalService.CloseModal(this);
        }

        [RelayCommand]
        private void CancelEdit()
        {
            hasEdits = false; //Mark as not having any edits so it can close without prompting that there were changes
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
                closeModalService.ModalClosing -= OnModalClosing;
                if (Settings != null)
                {
                    Settings.PropertyChanged -= OnSettingChanged;
                    Settings.Dispose();
                }
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

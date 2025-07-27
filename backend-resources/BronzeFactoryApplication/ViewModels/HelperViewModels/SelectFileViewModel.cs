using AzureBlobStorageLibrary;
using CommonHelpers.HttpHelpers.FileModels;
using ImageSharpUtilities;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.HelperViewModels
{
    public partial class SelectFileViewModel : BaseViewModel
    {
        /// <summary>
        /// The CurrentFiles Url
        /// </summary>
        [ObservableProperty]
        private string currentFileUrl = string.Empty;

        /// <summary>
        /// The Selected File to Disk that will replace the Current FileUrl
        /// </summary>
        [ObservableProperty]
        private string selectedFileDiskPath = string.Empty;

        public SelectFileViewModel()
        {

        }

        /// <summary>
        /// Selects an Image from the Disk
        /// </summary>
        [RelayCommand]
        public void SelectImage()
        {
            // Create a Dialog for the User to Select an Image
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the Path of the Image
                string filePath = openFileDialog.FileName;
                if (filePath.EndsWith(".png") || filePath.EndsWith(".jpeg") || filePath.EndsWith(".jpg"))
                {
                    SelectedFileDiskPath = filePath;
                }
                else
                {
                    MessageService.Warning("Selected File is not a Supported Image , please Select 'png' or 'jpg' or 'jpeg' files", "Not Supported File");
                } 
            };
        }

        [RelayCommand]
        public void SelectPdfFile()
        {
            // Create a Dialog for the User to Select an Image
            OpenFileDialog openFileDialog = new()
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the Path of the Image
                string filePath = openFileDialog.FileName;
                if (filePath.EndsWith(".pdf"))
                {
                    SelectedFileDiskPath = filePath;
                }
                else
                {
                    MessageService.Warning("Selected File is not a Pdf , please Select 'pdf' files", "Not Supported File");
                }
                SelectedFileDiskPath = filePath;
            };
        }

    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace BronzeFactoryApplication.Helpers
{
    public static class GeneralHelpers
    {
        /// <summary>
        /// Checks wheather a window is Open (Valid Only for Transient Windows)
        /// </summary>
        /// <param name="window">Checks wheather a Particular Window Type is Open</param>
        /// <returns>true if it is, False if it is not</returns>
        public static bool IsWindowOpen(string windowName)
        {
            return string.IsNullOrWhiteSpace(windowName)
                ? Application.Current.Windows.OfType<Window>().Any()
                : Application.Current.Windows.OfType<Window>().Any(w => w.Name == windowName);
        }

        /// <summary>
        /// Checks wheather a Certain Window is Open , and brings it to the Front
        /// </summary>
        /// <param name="typeOfWindow">The Window's Type</param>
        /// <returns>True if the Window was Open and brought to Front, False if it was not Open</returns>
        public static bool BringWindowToFrontIfOpen(Type typeOfWindow)
        {
            Window? window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.GetType() == typeOfWindow);

            if (window is null)
            {
                return false;
            }
            else
            {
                if (window.WindowState is WindowState.Minimized)
                {
                    window.WindowState = WindowState.Normal;
                    window.Activate();
                }
                else if (window.WindowState is WindowState.Maximized or WindowState.Normal)
                {
                    window.WindowState = WindowState.Minimized;
                }
                return true;
            }
        }

        /// <summary>
        /// Opens a Dialog to Select an Image File from the Local Disk
        /// </summary>
        /// <returns>The Selected File's Path or an Empty String when nothing is selected</returns>
        public static string SelectImageFile()
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
                    return filePath;
                }
                else
                {
                    MessageService.Warning("Selected File is not a Supported Image , please Select 'png' or 'jpg' or 'jpeg' files", "Not Supported File");
                }
            };

            return string.Empty;
        }

        /// <summary>
        /// Opens a Dialog to Select a PDF File from the Local Disk
        /// </summary>
        /// <returns>The Selected File's Path</returns>
        public static string SelectPdfFile()
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
                    return filePath;
                }
                else
                {
                    MessageService.Warning("Selected File is not a Pdf , please Select 'pdf' files", "Not Supported File");
                }
            };
            return string.Empty;
        }

        /// <summary>
        /// Writes undeleted Urls to a JsonFile
        /// </summary>
        /// <param name="urls"></param>
        public static void WriteUndeletedUrlsToJsonFile(params string[] urls)
        {

            // Combine the folder path with the file name
            var filePath = Path.Combine(App.ApplicationDataFolderPath, "UndeletedPhotosUrls.json");

            // Initialize a list to hold the strings
            List<string> strings;

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // If the file exists, read and deserialize the JSON content
                string existingJson = File.ReadAllText(filePath);
                strings = JsonSerializer.Deserialize<List<string>>(existingJson) ?? new List<string>();
            }
            else
            {
                // If the file does not exist, create a new list
                strings = new List<string>();
            }

            // Append the new text
            strings.AddRange(urls);

            // Serialize the updated list back to JSON
            string newJson = JsonSerializer.Serialize(strings);

            // Write the new JSON to the file
            File.WriteAllText(filePath, newJson);
        }
    }
}

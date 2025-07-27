using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Helpers
{
    public static class GlobalCommands
    {
        /// <summary>
        /// A Command that Copies Text to the Clipboard
        /// </summary>
        public static IRelayCommand<string> CopyToClipboardCommand { get => new RelayCommand<string>(CopyToClipboard); }
        /// <summary>
        /// A command that Opens a Pdf File , Either by Downloading it or By Selecting it from the Local File System
        /// </summary>
        public static IAsyncRelayCommand<string> OpenPdfFileAsyncCommand { get => new AsyncRelayCommand<string>(OpenPdfFileAsync); }

        /// <summary>
        /// Copies the provided text to the Clipboard
        /// </summary>
        /// <param name="textToCopy"></param>
        public static void CopyToClipboard(string? textToCopy)
        {
            if (string.IsNullOrEmpty(textToCopy))
            {
                MessageService.Warning("The text you are trying to copy is Empty or Null", "String to Copy is Undefined");
                return;
            }
            Clipboard.SetText(textToCopy);
        }
        /// <summary>
        /// Opens a pdf or Downloads it and opens it if the url is from the Web
        /// </summary>
        /// <param name="filePathOrUrl"></param>
        /// <returns></returns>
        public static async Task OpenPdfFileAsync(string? filePathOrUrl)
        {
            if (string.IsNullOrEmpty(filePathOrUrl))
            {
                MessageService.Warning($"The Provided File Path or Url was Empty or null.", "Pdf Open Error");
                return;
            }

            string? filePathToRun = null;

            if (Uri.TryCreate(filePathOrUrl, UriKind.Absolute, out Uri? uri) &&
                (uri?.Scheme == Uri.UriSchemeHttp || uri?.Scheme == Uri.UriSchemeHttps))
            {
                filePathToRun = await DownloadPdfAsync(filePathOrUrl);
                if (string.IsNullOrEmpty(filePathToRun) || !IsPdf(filePathToRun))
                {
                    MessageService.Warning($"The provided URL does not point to a valid PDF file.", "Pdf Open Error");
                }
            }
            else
            {
                if (File.Exists(filePathOrUrl) && IsPdf(filePathOrUrl))
                {
                    filePathToRun = filePathOrUrl;
                }
                else
                {
                    MessageService.Warning($"The provided File Path does not point to a valid PDF file.", "Pdf Open Error");
                }
            }

            if (!string.IsNullOrEmpty(filePathToRun))
            {
                Process.Start(new ProcessStartInfo(filePathToRun) { UseShellExecute = true });
            }
        }

        /// <summary>
        /// Downloads a Pdf and Saves it to the Temp Folder for later Opening
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The File Path to the Temporary folder</returns>
        private static async Task<string?> DownloadPdfAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentType?.MediaType == "application/pdf")
                    {
                        var tempFilePath = Path.Combine(Path.GetTempPath(), "downloaded.pdf");
                        using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
                        return tempFilePath;
                    }
                }
            }

            return null;
        }
        private static bool IsPdf(string filePath)
        {
            return Path.GetExtension(filePath)?.Equals(".pdf", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}

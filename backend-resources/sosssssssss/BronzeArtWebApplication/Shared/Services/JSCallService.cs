using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Services
{
    /// <summary>
    /// A Service that allows Writing staff to the Clipboard
    /// </summary>
    public class JSCallService
    {
        private readonly IJSRuntime js;

        public JSCallService(IJSRuntime js)
        {
            this.js = js;
        }

        /// <summary>
        /// Calls JSRuntim and Writes the Given Text to the Clipboard
        /// </summary>
        /// <param name="text">The Text</param>
        /// <returns>a Value Task</returns>
        public ValueTask WriteTextToClipboardAsync(string text)
        {
            return js.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
        /// <summary>
        /// Tries to Copy a Table to Clipboard , and can remove rows and Columns by their indices
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="columnsToRemove">The Indices of the Columns to Remove</param>
        /// <param name="rowsToRemove">The Indices of the Rows to Remove</param>
        /// <returns></returns>
        public async Task WriteTableToClipboard(string tableId, int[] columnsToRemove , int[] rowsToRemove)
        {
            if (columnsToRemove is null)
            {
                columnsToRemove = new int[] { };
            }
            if (rowsToRemove is null)
            {
                rowsToRemove = new int[] { };
            }
            await js.InvokeVoidAsync("copyTableToClipboard", tableId,columnsToRemove,rowsToRemove);
        }
        /// <summary>
        /// Copies the first table residing inside another element
        /// </summary>
        /// <param name="elementId">The element's id</param>
        /// <param name="columnsToRemove">The indices of the Columns of the table to remove</param>
        /// <param name="rowsToRemove">The indices of the Rows of the table to remove</param>
        /// <returns></returns>
        public async Task CopyTableOfElementToClipboard(string elementId , int[] columnsToRemove , int[] rowsToRemove)
        {
            await js.InvokeVoidAsync("copyTableInsideElement", elementId,columnsToRemove, rowsToRemove);
        }


        /// <summary>
        /// Gets the Base URI for the Current application Instance
        /// </summary>
        /// <returns></returns>
        public ValueTask<string> GetBaseUri()
        {
            return js.InvokeAsync<string>("getBaseUri");
        }

        /// <summary>
        /// Opens the requested url in a new Tab,
        /// (KNOWN BLAZOR FRAMEWORK BUG : While in Debug mode this will throw an exception and hang the original Tab , Just press F12 twice and the application is responsive again)
        /// </summary>
        /// <param name="url">The url to Open</param>
        /// <returns></returns>
        public async Task NavigateToNewTab(string url)
        {
            await js.InvokeAsync<object>("open", url, "_blank");
        }

        /// <summary>
        /// Scrolls back to top
        /// </summary>
        /// <returns></returns>
        public async Task ScrollToTop(string elementId = "")
        {
            if (elementId == "")
            {
                // Scroll to top the Main Element
                await js.InvokeVoidAsync("scrollToTop");
            }
            else
            {
                await js.InvokeVoidAsync("scrollToTopElement", elementId);
            }
        }

        public async Task ScrollElementIntoViewAsync(string elementId)
        {
            if (string.IsNullOrEmpty(elementId))
            {
                return;
            }
            else
            {
                await js.InvokeVoidAsync("scrollToElementWithAnimation", elementId, "opaqueToVisiblePulseAnimation", 2);
            }
        }

        /// <summary>
        /// Gets the Window Dimensions for the current browser window
        /// </summary>
        /// <returns></returns>
        public async Task<WindowDimensions> GetWindowDimensions()
        {
            return await js.InvokeAsync<WindowDimensions>("GetWindowDimensions");
        }

        /// <summary>
        /// Triggers a file download from the specific url and the provided filename
        /// </summary>
        /// <param name="fileName">The FileName of the File that will be downloaded</param>
        /// <param name="url">The url of the File to download</param>
        /// <returns></returns>
        public async Task TriggerFileDownload(string fileName, string url)
        {
            await js.InvokeVoidAsync("triggerFileDownload", fileName, url);
        }
        /// <summary>
        /// Triggers an Excel File Download from a Base64String
        /// </summary>
        /// <param name="fileName">The Name of the File</param>
        /// <param name="bytesBase64">The Base64String that is the content of the File</param>
        /// <returns></returns>
        public async Task TriggerExcelFileDownload(string fileName, string bytesBase64)
        {
            await js.InvokeVoidAsync("downloadExcelFile", bytesBase64, fileName);
        }

        /// <summary>
        /// Saves byte data as a file , converting it into base64String
        /// </summary>
        /// <param name="filename">The name of the File along with its extension</param>
        /// <param name="data">The Byte Data</param>
        /// <returns></returns>
        public async Task SaveGeneratedFileAs(string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }

        /// <summary>
        /// Prints the Current Displayed Page . Without Going Back afterwards
        /// </summary>
        /// <returns></returns>
        public async Task PrintCurrentPage()
        {
            await js.InvokeVoidAsync("printCurrentPage");
        }

        /// <summary>
        /// Focuses an element by its id
        /// </summary>
        /// <param name="id">The Id of the element to Focus</param>
        /// <returns></returns>
        public async Task FocusElement(string id)
        {
            await js.InvokeVoidAsync("focusElement", id);
        }
        public async Task FocusInputInsideParent(string parentId)
        {
            await js.InvokeVoidAsync("focusInputInsideParent", parentId);
        }
    }

    public class WindowDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

}

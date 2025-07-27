using BronzeArtWebApplication.Shared.Services;
using CommonHelpers.HttpHelpers.FileModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BronzeArtWebApplication.Shared.Helpers;
using System.Linq;
using BlazorDownloadFile;
using ShowerEnclosuresModelsLibrary.Factory;
using ExcelSupportLib;

namespace BronzeArtWebApplication.Pages.ManageOptionsPage
{
    public partial class OnlyToCheckAuthorizeView : ComponentBase
    {
        /// <summary>
        /// Downloads a Backup in Json Format of the Accessories Properties and the Accessories List
        /// </summary>
        /// <returns></returns>
        //private async Task DownloadBackUp()
        //{
        //    //Inform Component there is a Call in Progress
        //    isBusy = true;
        //    await InvokeAsync(StateHasChanged);

        //    // Backup Properties
        //    var response = await api.GetAllPropertiesJson();
        //    VariousHelpers.LogToConsole(response.summary);
        //    if (response.summary.Status is OperationStatus.Successful)
        //    {
        //        snackbar.Add(response.summary.Message, Severity.Success);
        //        await downloader.DownloadFileFromText($"PropertiesBackup-{DateTime.Now:dd-MM-yyyy}.json", response.jsonStringProperties, System.Text.Encoding.UTF8, "application/octet-stream", false);
        //    }
        //    else
        //    {
        //        snackbar.Add(response.summary.Message, Severity.Error);
        //    }
        //    await Task.Run(() => Task.Delay(2000));
        //    // Backup Accessories
        //    var response2 = await api.GetAllAccessoriesJson();
        //    VariousHelpers.LogToConsole(response2.summary);
        //    if (response2.summary.Status is OperationStatus.Successful)
        //    {
        //        snackbar.Add(response2.summary.Message, Severity.Success);
        //        await downloader.DownloadFileFromText($"AccessoriesBackup-{DateTime.Now:dd-MM-yyyy}.json", response2.jsonStringAccessories, System.Text.Encoding.UTF8, "application/octet-stream", false);
        //    }
        //    else
        //    {
        //        snackbar.Add(response2.summary.Message, Severity.Error);
        //    }

        //    isBusy = false;
        //    await InvokeAsync(StateHasChanged);
        //}

        //private async Task GetExcelTestReport()
        //{
        //    await Task.Delay(5);
        //    //TO IMPLEMENT WHEN CLOSEDXML 097.1 Is Released
        //    //isBusy = true;
        //    //await InvokeAsync(StateHasChanged);
        //    //var cabin = cabinFactory.CreateCabin(
        //    //    ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums.CabinDrawNumber.Draw9S, 
        //    //    ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums.CabinSynthesisModel.Primary);
        //    //byte[] reportBytes = ProductReports.GetCabinReport(cabin);
        //    //await js.SaveGeneratedFileAs("test.xlsx", reportBytes);
        //    //isBusy = false;
        //    //await InvokeAsync(StateHasChanged);
        //}

        //private async Task DownloadImageTest()
        //{
        //    isBusy = true;
        //    await InvokeAsync(StateHasChanged);

        //    var response = await api.GetImage();
        //    VariousHelpers.LogToConsole(response.summary);
        //    if (response.summary.Status is OperationStatus.Successful)
        //    {
        //        snackbar.Add(response.summary.Message, Severity.Success);
        //        Console.WriteLine(response.jsonStringImage);
        //    }
        //    else
        //    {
        //        snackbar.Add(response.summary.Message, Severity.Error);
        //    }
        //    isBusy = false;
        //    await InvokeAsync(StateHasChanged);
        //}


        #region NOTES
        //
        // ResizePicture to the Needed Max Dimensions (If the Image is smaller it will remain small)
        //
        // var format = "image/png";
        // var resizedImageFile = await e.File.RequestImageFileAsync(format, 700, 500); 

        // Another Method to Post is , Serilize first to JSON String , then Transform string into byte Array , then create HttpContnt and Post it
        //var buffer2 = System.Text.Encoding.UTF8.GetBytes(json);
        //HttpContent content = new ByteArrayContent(buffer2);
        //await http.PostAsync("/api/UploadImageToBlobContainer", content);

        #endregion
    }
}

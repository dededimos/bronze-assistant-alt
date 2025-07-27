using AKSoftware.Localization.MultiLanguages;
using BathAccessoriesModelsLibrary;
using BronzeArtWebApplication.Components.Various.VariousDialogs;
using CommonHelpers;
using MudBlazor;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Services
{
    public interface IMessageService
    {
        public const string DialogOk = "DialogOk";
        public const string DialogCancel = "DialogCancel";
        public const string DialogYes = "DialogYes";
        public const string DialogNo = "DialogNo";
        public const string DialogConfirm = "DialogConfirm";
        public const string DialogSubmit = "DialogSubmit";
        public const string DialogClose = "DialogActionButtonClose";


        /// <summary>
        /// Displays a message Box with the Provided Message and Title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        Task InfoAsync(string title,string message);
        /// <summary>
        /// Displays a question Message and Returns a value
        /// </summary>
        /// <param name="title">The Title of the Dialog</param>
        /// <param name="question">The Question of the Dialog</param>
        /// <returns></returns>
        Task<MessageResult> QuestionAsync(string title, string question, string okButtonText = IMessageService.DialogOk, string cancelButtonText = IMessageService.DialogCancel);
        /// <summary>
        /// Opens the Add to Basket Dialog
        /// </summary>
        /// <param name="accessory">The Accessory to Add</param>
        /// <param name="selectedFinish">The Finish Preselected on the Dialog</param>
        Task OpenAddToBasketDialogAsync(BathroomAccessory accessory, AccessoryFinish selectedFinish);
        /// <summary>
        /// Opens the Dialogs for the Saved Baskets
        /// </summary>
        Task OpenSavedBasketsDialogAsync();
        /// <summary>
        /// Opens the Dialog to insert a name for a Basket 
        /// </summary>
        /// <param name="nameOnTextField">The Starting Name that the input will display before any user interaction</param>
        /// <returns></returns>
        Task<string> OpenInputBasketNameAsync(string nameOnTextField);
        Task<string> OpenInputStringDialogAsync(string dialogTitle, string textfieldPlaceholder, string textfieldStartingValue, int maxInputLength = 1000);
        Task ErrorAsync(string title, string message);
        Task ErrorAsync(Exception ex);
    }

    public class MessageService : IMessageService
    {
        private readonly IDialogService dialogService;
        private readonly ILanguageContainerService lc;

        public MessageService(IDialogService dialogService ,ILanguageContainerService lc)
        {
            this.dialogService = dialogService;
            this.lc = lc;
        }

        /// <summary>
        /// Displays a message Box with the Provided Message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public async Task InfoAsync(string title , string message)
        {
            var parameters = new DialogParameters<InfoDialog>();
            parameters.Add(d => d.Message, message);
            await dialogService.ShowAsync<InfoDialog>(title.AddSpacesToEnd(3), parameters);
        }

        public async Task ErrorAsync(string title, string message)
        {
            await InfoAsync(title, message);
        }
        public async Task ErrorAsync(Exception ex)
        {
            await InfoAsync($"{ex.GetType().Name}", $"There was an Unexpected Error :{Environment.NewLine}{Environment.NewLine}{ex.Message}");
        }

        /// <summary>
        /// Displays a question Message and Returns a value
        /// </summary>
        /// <param name="title">The Title of the Dialog</param>
        /// <param name="question">The Question of the Dialog</param>
        /// <returns></returns>
        public async Task<MessageResult> QuestionAsync(string title , string question , string okButtonText = IMessageService.DialogOk , string cancelButtonText = IMessageService.DialogCancel)
        {
            // Pass parameters into the Dialog Component
            var parameters = new DialogParameters<QuestionDialog>();
            parameters.Add(d => d.Question, question);
            parameters.Add(d => d.OkButtonText, okButtonText);
            parameters.Add(d => d.CancelButtonText, cancelButtonText);
            // Show the Dialog Component
            var dialog = await dialogService.ShowAsync<QuestionDialog>(title, parameters);
            var result = await dialog.Result;
            // Parse the Result , if parse fails return cancel
            return result.Data is MessageResult messageResult? messageResult : MessageResult.Cancel;
        }

        public async Task OpenAddToBasketDialogAsync(BathroomAccessory accessory,AccessoryFinish selectedFinish)
        {
            var parameters = new DialogParameters<AddProductsToBasketDialog>();
            parameters.Add(d=> d.AccessoryToAdd , accessory);
            parameters.Add(d => d.SelectedFinish, selectedFinish);
            var dialog = await dialogService.ShowAsync<AddProductsToBasketDialog>(lc.Keys["AddToQuoteBasket"], parameters, new() { CloseOnEscapeKey=true});
        }
        /// <summary>
        /// Opens the Dialogs for the Saved Baskets
        /// </summary>
        public async Task OpenSavedBasketsDialogAsync()
        {
            var dialog = await dialogService.ShowAsync<SavedBasketsDialog>();
        }
        /// <summary>
        /// Opens the Dialog to insert a name for a Basket 
        /// </summary>
        /// <param name="nameOnTextField">The Starting Name that the input will display before any user interaction</param>
        /// <returns></returns>
        public async Task<string> OpenInputBasketNameAsync(string nameOnTextField)
        {
            return await OpenInputStringDialogAsync(lc.Keys["InputName"], lc.Keys["BasketName"], nameOnTextField,45);
        }

        public async Task<string> OpenInputStringDialogAsync(string dialogTitle ,string textfieldPlaceHolder , string textfieldStartingValue,int maxInputLength = 1000)
        {
            var parameters = new DialogParameters<StringInputDialog>();
            parameters.Add(d => d.InputName, textfieldPlaceHolder);
            parameters.Add(d => d.StartingInput, textfieldStartingValue);
            parameters.Add(d => d.MaxInputLength, maxInputLength);
            var dialog = await dialogService.ShowAsync<StringInputDialog>(dialogTitle,parameters);
            var result = await dialog.Result;
            return result?.Data?.ToString() ?? string.Empty;
        }

    }

    public enum MessageResult
    {
        Ok,
        Cancel
    }
    
}

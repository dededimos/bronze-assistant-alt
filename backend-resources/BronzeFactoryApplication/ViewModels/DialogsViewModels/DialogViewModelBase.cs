using BronzeFactoryApplication.ApplicationServices.DialogService;

namespace BronzeFactoryApplication.ViewModels.DialogsViewModels
{
    public abstract class DialogViewModelBase<T>
    {
        /// <summary>
        /// The Title of the Dialog
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// The Message of the Dialog
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// The Result of the Dialog
        /// </summary>
        public T? DialogResult { get; set; }

        /// <summary>
        /// Initializes a Dialog with the defined message and Title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public DialogViewModelBase(string title, string message)
        {
            Title = title;
            Message = message;
        }
        /// <summary>
        /// Initilizes a Dialog with an Empty Title and Message
        /// </summary>
        public DialogViewModelBase() : this(string.Empty, string.Empty) { }
        /// <summary>
        /// Initilizes a Dialog with an Empty Message
        /// </summary>
        public DialogViewModelBase(string title) : this(title, string.Empty) { }

        /// <summary>
        /// Closes the selected Dialog with the Selected Result
        /// </summary>
        /// <param name="dialog">The Dialog</param>
        /// <param name="result">The Result of the Dialog</param>
        public void CloseDialogWithResult(IDialogWindow dialog, T result)
        {
            DialogResult = result;
            if (dialog != null) dialog.DialogResult = true; //This Actually Closes the Dialog when set !! only null keeps it open!
        }
    }
}

namespace BronzeFactoryApplication.ApplicationServices.DialogService
{
    public interface IDialogWindow
    {
        /// <summary>
        /// Weather this dialog Was Closed with producing a Result or Just Closed (Cancelled)
        /// </summary>
        bool? DialogResult { get; set; }
        /// <summary>
        /// The Data Context of the Dialog
        /// </summary>
        object DataContext { get; set; }
        /// <summary>
        /// Show the Dialog
        /// </summary>
        /// <returns>True if it returns a Result false if Cancelled (Closed without a Result)</returns>
        bool? ShowDialog();
    }
}

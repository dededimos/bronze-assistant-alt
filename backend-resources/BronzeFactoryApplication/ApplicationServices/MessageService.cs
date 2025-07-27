using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageBoxHC = HandyControl.Controls.MessageBox;
using HandyControl.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

namespace BronzeFactoryApplication.ApplicationServices
{
    public static class MessageService
    {
        /// <summary>
        /// Logs an Exception and Displays the Error
        /// </summary>
        /// <param name="ex">The Exception</param>
        public static void LogAndDisplayException(Exception ex)
        {
            Log.Error(ex, "{message}", ex.Message);
            DisplayException(ex);
        }

        public static void LogAndDisplayErrorMessage(string msg,string title)
        {
            Log.Error(msg);
            Error(msg, title);
        }

        /// <summary>
        /// Displays the Exception
        /// </summary>
        /// <param name="ex">The Exception</param>
        public static void DisplayException(Exception ex)
        {
            MessageBoxInfo error = new()
            {
                Button = MessageBoxButton.OK,
                Message = ex.Message,
                Caption = "lngFailure".TryTranslateKey(),
                IconBrushKey = ResourceToken.DangerBrush,
                IconKey = ResourceToken.ErrorGeometry,
                ConfirmContent = "lngOk".TryTranslateKey()
            };
            MessageBoxHC.Show(error);
        }

        /// <summary>
        /// Displays a Message in a MessageBox and Logs it as Information
        /// </summary>
        /// <param name="msg">The Message</param>
        /// <param name="title">The Title of the Message Box</param>
        public static void Info(string msg,string title)
        {
            MessageBoxInfo info = new()
            {
                Button = MessageBoxButton.OK,
                Message = msg,
                Caption = title,
                IconBrushKey = ResourceToken.InfoBrush,
                IconKey = ResourceToken.InfoGeometry,
                ConfirmContent = "lngOk".TryTranslateKey()
            };
            Log.Information("{msg}",msg);
            MessageBoxHC.Show(info);
        }

        /// <summary>
        /// Displays a Question in a MessageBox and Logs it along with the UserSelection
        /// </summary>
        /// <param name="msg">The Message</param>
        /// <param name="title">The Title of the Message Box</param>
        /// <param name="OkButtonText">The Text of the Button Confirmation</param>
        /// <param name="CancelButtonText">The Text of the Button Cancellation</param>
        /// <returns>A Windows Message Box Result of OK-CANCEL</returns>
        public static MessageBoxResult Question(string msg, string title,string OkButtonText,string CancelButtonText)
        {
            MessageBoxInfo question = new()
            {
                Button = MessageBoxButton.OKCancel,
                Message = msg,
                Caption = title,
                IconBrushKey = ResourceToken.DarkWarningBrush,
                IconKey = ResourceToken.AskGeometry,
                DefaultResult = MessageBoxResult.Cancel,
                CancelContent = CancelButtonText,
                ConfirmContent = OkButtonText
            };
            
            // Display Message
            MessageBoxResult result = MessageBoxHC.Show(question);
            
            // Log Result and User Selection
            string selectedResult = result == MessageBoxResult.OK ? OkButtonText : CancelButtonText;
            Log.Information("{msg}  [{selectedResult}]",msg,selectedResult);
            
            return result;
        }

        /// <summary>
        /// Displays a Warning Message in a MessageBox and Logs it as Warning
        /// </summary>
        /// <param name="msg">The Message</param>
        /// <param name="title">The Title of the Message Box</param>
        public static void Warning(string msg,string title)
        {
            MessageBoxInfo warning = new()
            {
                Button = MessageBoxButton.OK,
                Message = msg,
                Caption = title,
                IconBrushKey = ResourceToken.WarningBrush,
                IconKey = ResourceToken.WarningGeometry,
                ConfirmContent = "lngOk".TryTranslateKey()
            };
            Log.Warning("{msg}", msg);
            MessageBoxHC.Show(warning);
        }

        public static void Error(string msg , string title)
        {
            MessageBoxInfo error = new()
            {
                Button = MessageBoxButton.OK,
                Message = msg,
                Caption = title,
                IconBrushKey = ResourceToken.DangerBrush,
                IconKey = ResourceToken.ErrorGeometry,
                ConfirmContent = "lngOk".TryTranslateKey()
            };
            Log.Warning("{msg}", msg);
            MessageBoxHC.Show(error);
        }

        public static class Questions
        {
            /// <summary>
            /// Message Asking if the Application Should Close
            /// </summary>
            /// <returns>A MessageBox OK-CANCEL Result</returns>
            public static MessageBoxResult ApplicationClose() =>
                Question("lngApplicationCloseQuestion".TryTranslateKey(),
                         "lngApplicationClose".TryTranslateKey(),
                         "lngAppCloseButton".TryTranslateKey(),
                         "lngCancel".TryTranslateKey());
            public static MessageBoxResult ApplicationRestartLanguageChange() =>
                Question($"{"lngLanguageChanged".TryTranslateKey()} {"lngNonSavedLostContinue".TryTranslateKey()}",
                         "lngInformation".TryTranslateKey(),
                         "lngRestartButton".TryTranslateKey(),
                         "lngCancel".TryTranslateKey());
            public static MessageBoxResult ApplicationRestartThemeChange() =>
                Question($"{"lngThemeChanged".TryTranslateKey()} {"lngNonSavedLostContinue".TryTranslateKey()}",
                         "lngInformation".TryTranslateKey(),
                         "lngRestartButton".TryTranslateKey(),
                         "lngCancel".TryTranslateKey());
            public static MessageBoxResult UnsavedChangesContinue() =>
                Question("lngUnsavedEditsMessage".TryTranslateKey(),
                    "lngUnsavedEdits".TryTranslateKey(),
                    "lngCloseAndLoseChanges".TryTranslateKey(),
                    "lngNo".TryTranslateKey());
            public static MessageBoxResult ReapplyLetteringNumbering() =>
                Question("lngReapplyNumberingLetteringQuestion".TryTranslateKey(),
                    "lngQuestion".TryTranslateKey(),
                    "lngYes".TryTranslateKey(),
                    "lngNo".TryTranslateKey());
            public static MessageBoxResult ExcelSavedAskOpenFile(string filename) =>
                Question($"{"lngXlsGenerationSuccess".TryTranslateKey()}{Environment.NewLine}{Environment.NewLine}{"lngFile".TryTranslateKey()}:{Environment.NewLine}{filename}{Environment.NewLine}{Environment.NewLine}{"lngOpenFileQuestion".TryTranslateKey()}"
                    , "lngSaveSuccessful".TryTranslateKey(), "lngYes".TryTranslateKey(), "lngNo".TryTranslateKey());
            public static MessageBoxResult FileSavedAskOpenFile(string fileName) =>
                Question($"{"lngFileSaveSuccess".TryTranslateKey()}{Environment.NewLine}{Environment.NewLine}{"lngFile".TryTranslateKey()}:{Environment.NewLine}{fileName}{Environment.NewLine}{Environment.NewLine}{"lngOpenFileQuestion".TryTranslateKey()}"
                    , "lngSaveSuccessful".TryTranslateKey(), "lngYes".TryTranslateKey(), "lngNo".TryTranslateKey());
            public static MessageBoxResult ThisWillRemoveEditedItemContinue() =>
                Question("lngThisWillRemoveEditedItemContinue".TryTranslateKeyWithoutError(), "lngRemoveItemTitle".TryTranslateKeyWithoutError(), "lngRemove".TryTranslateKeyWithoutError(), "lngCancel".TryTranslateKeyWithoutError());
        }
        public static class Information
        {
            /// <summary>
            /// Displays the NotSupported Language Message
            /// </summary>
            public static void NotSupportedLanguage() => Info("lngNotSupportedLanguageMessage".TryTranslateKey(), "lngNotSupportedLanguageMessageTitle".TryTranslateKey());
            /// <summary>
            /// Displays the NotSupported Language Message
            /// </summary>
            public static void NotSupportedTheme() => Info("lngNotSupportedThemeMessage".TryTranslateKey(), "lngNotSupportedThemeMessageTitle".TryTranslateKey());
            public static void SaveSuccess() => Info("lngSaveSuccessful".TryTranslateKey(), "lngSave".TryTranslateKey());
            public static void DeletionSuccess() => Info("lngDeleteSuccessful".TryTranslateKey(), "lngDelete".TryTranslateKey());
            public static void AlreadySelectedLanguage() => Info("lngLanguageAlreadySelected".TryTranslateKey(), "lngInformation".TryTranslateKey());
            public static void AlreadySelectedTheme() => Info("lngThemeAlreadySelected".TryTranslateKey(), "lngInformation".TryTranslateKey());
        }
        public static class Warnings
        {
            public static void SettingsThemeNotFoundDefaultSet() => Warning("lngThemeNotFoundDefaultSet".TryTranslateKey(), "lngThemeNotFoundTitle".TryTranslateKey());
            public static void AlreadySaved() => Warning("lngAlreadySaved".TryTranslateKey(), "lngSave".TryTranslateKey());
            public static void InvalidOrderId() => Warning("lngIncorrectGlassesOrderId".TryTranslateKey(), "lngSaveFailure".TryTranslateKey());
            public static void UnsavedChangesCannotProceedWithoutSave() => Warning("lngUnsavedChangesSaveToProceed".TryTranslateKeyWithoutError(), "lngUnsavedChangesCannotProceed".TryTranslateKeyWithoutError());
        }

        public static class Errors
        {
            public static void FailedToInitilizeMemoryRepo() => Error("lngFailedToReInitilizeMemoryRepo".TryTranslateKey(), "lngInitilizationFailed".TryTranslateKey());
        }
    }
}

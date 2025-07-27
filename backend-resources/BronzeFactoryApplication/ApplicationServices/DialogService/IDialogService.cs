using BronzeFactoryApplication.ViewModels.DialogsViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.DialogService
{
    public interface IDialogService
    {
        /// <summary>
        /// Opens the Dialog corresponding to the Passed ViewModel
        /// </summary>
        /// <typeparam name="T">The Result of the Dialog</typeparam>
        /// <param name="dialogVm">The Dialogs ViewModel</param>
        /// <returns>The Result of the Dialog</returns>
        T OpenDialog<T>(DialogViewModelBase<T> dialogVm);

        /// <summary>
        /// Opens a Message Dialog
        /// </summary>
        /// <param name="title">The Title of the Dialog</param>
        /// <param name="message">The Message of the Dialog</param>
        /// <returns>A Basic Dialog Result</returns>
        DialogBasicResult OpenMessageDialog(string title, string message);
        /// <summary>
        /// Opens a Modal At a Seperate Window without Obstructing interaction with the Main Window
        /// </summary>
        /// <param name="modal"></param>
        void OpenDialogAsWindow(ModalViewModel modal);
    }
}

using BronzeFactoryApplication.ViewModels.DialogsViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.Views.Dialogs;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BronzeFactoryApplication.ApplicationServices.DialogService.DefaultImplementation
{
    public class DialogService : IDialogService
    {
        public T OpenDialog<T>(DialogViewModelBase<T> dialogVm)
        {
            //Make the Window
            DialogWindow dialog = new()
            {
                DataContext = dialogVm,
                //Cannot set Owner property to a Window that has not been shown previously.(if not Active will throw this)
                //If not Window is Active this will default to Null
                Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive) 
            };

            //If Managed to Set Owner then Pop it in center of owner and play with Owners Opacity
            if (dialog.Owner is not null)
            {
                dialog.Owner.Opacity = 0.5;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                
                dialog.ShowDialog();
                dialog.Owner.Opacity = 1;
            }
            else //Pop it on center without modifying anyother window
            {
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.ShowDialog();
            }

            //Return the Result of the vm , if Null throw
            return dialogVm.DialogResult ?? throw new Exception("The Dialog Result Instance was Null");
        }

        public void OpenDialogAsWindow(ModalViewModel modal)
        {
            DialogWindow window = new()
            {
                DataContext = modal,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
        }

        /// <summary>
        /// Opens a Message Box with the Corresponding Message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DialogBasicResult OpenMessageDialog(string title, string message)
        {
            return OpenDialog(new MessageDialogViewModel(title, message));
        }
    }

}

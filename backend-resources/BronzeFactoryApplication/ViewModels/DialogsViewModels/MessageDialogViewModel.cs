using BronzeFactoryApplication.ApplicationServices.DialogService;
using BronzeFactoryApplication.ApplicationServices.DialogService.DefaultImplementation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.DialogsViewModels
{
    public partial class MessageDialogViewModel : DialogViewModelBase<DialogBasicResult>
    {
        public MessageDialogViewModel(string title, string message) : base(title, message)
        {
        }

        [RelayCommand]
        private void OK(IDialogWindow dialog)
        {
            CloseDialogWithResult(dialog, DialogBasicResult.Ok);
        }
    }
}

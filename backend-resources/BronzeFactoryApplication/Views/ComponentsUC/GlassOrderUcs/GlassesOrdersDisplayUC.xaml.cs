using BronzeFactoryApplication.ViewModels.ModalViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BronzeFactoryApplication.Views.ComponentsUC.GlassOrderUcs
{
    /// <summary>
    /// Interaction logic for GlassesOrdersDisplayUC.xaml
    /// </summary>
    public partial class GlassesOrdersDisplayUC : UserControl
    {
        public GlassesOrdersDisplayUC()
        {
            InitializeComponent();
        }

        //Deprecated => Now Initilized with a Command Trigger on Loaded (This was to Trace the Change of DataContext and Retrieve Accordingly)
        //private async void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    GlassesOrdersDisplayModalViewModel? vm = e.NewValue as GlassesOrdersDisplayModalViewModel;
        //    if (vm is not null)
        //    {
        //        try
        //        {
        //            await vm.TryFillFromCache();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageService.Warning(ex.Message, "Unexpected Failure");
        //            Log.Error(ex, "{message}", ex.Message);
        //        }
        //    }
        //}
    }
}

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

namespace BronzeFactoryApplication.Views.ComponentsUC.MirrorsViewsUcs.MirrorsOrdersRelevantViews
{
    /// <summary>
    /// Interaction logic for AddRowToMirrorsOrderView.xaml
    /// </summary>
    public partial class AddRowToMirrorsOrderView : UserControl
    {
        public AddRowToMirrorsOrderView()
        {
            InitializeComponent();
        }
        private void PA0Textbox_Loaded(object sender, RoutedEventArgs e)
        {
            this.PA0Textbox.Focus();
        }
    }
}

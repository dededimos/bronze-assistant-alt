using HandyControl.Controls;
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

namespace BronzeFactoryApplication.Views.Modals
{
    /// <summary>
    /// Interaction logic for AddSynthesisToOrderModalUC.xaml
    /// </summary>
    public partial class AddSynthesisToOrderModalUC : UserControl
    {
        

        public AddSynthesisToOrderModalUC()
        {
            InitializeComponent();
        }

        private void PA0Textbox_Loaded(object sender, RoutedEventArgs e)
        {
            this.PA0Textbox.Focus();
        }
    }
}

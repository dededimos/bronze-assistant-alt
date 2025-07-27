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
    /// Interaction logic for EditMirrorsOrdersDetailsView.xaml
    /// </summary>
    public partial class EditMirrorsOrdersDetailsView : UserControl
    {
        public EditMirrorsOrdersDetailsView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Focus the Order TextBox if Enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderIdTextbox_Loaded(object sender, RoutedEventArgs e)
        {
            if (OrderIdTextbox.IsEnabled)
            {
                this.OrderIdTextbox.Focus();
            }
        }
        /// <summary>
        /// Focus the Notes TextBox if the OrderId TextBox is not Enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotesTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!OrderIdTextbox.IsEnabled)
            {
                this.NotesTextBox.Focus();
            }
        }
    }
}

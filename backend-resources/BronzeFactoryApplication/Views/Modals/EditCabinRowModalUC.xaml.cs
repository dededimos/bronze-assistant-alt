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
    /// Interaction logic for EditCabinRowModalUC.xaml
    /// </summary>
    public partial class EditCabinRowModalUC : UserControl
    {
        public EditCabinRowModalUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the Focus to Notes as soon as Modal Opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotesTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            NotesTextBox.Focus();
        }
    }
}

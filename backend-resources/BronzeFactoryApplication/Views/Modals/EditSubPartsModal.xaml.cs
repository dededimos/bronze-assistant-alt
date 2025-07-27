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
    /// Interaction logic for EditSubPartsModal.xaml
    /// </summary>
    public partial class EditSubPartsModal : UserControl
    {
        public EditSubPartsModal()
        {
            InitializeComponent();
        }

        private void PartChooseComboBox2_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                Keyboard.Focus(PartChooseComboBox2);
            }
        }
    }
}

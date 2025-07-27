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

namespace BronzeFactoryApplication.Views.ComponentsUC.CabinPropertiesUserControls
{
    /// <summary>
    /// Interaction logic for AdvancedCabinPropsUC.xaml
    /// </summary>
    public partial class AdvancedCabinPropsUC : UserControl
    {


        public bool AreRestrictedFieldsOpen
        {
            get { return (bool)GetValue(AreRestrictedFieldsOpenProperty); }
            set { SetValue(AreRestrictedFieldsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AreRestrictedFieldsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AreRestrictedFieldsOpenProperty =
            DependencyProperty.Register("AreRestrictedFieldsOpen", typeof(bool), typeof(AdvancedCabinPropsUC), new PropertyMetadata(false));




        public AdvancedCabinPropsUC()
        {
            InitializeComponent();
        }
    }
}

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

namespace BronzeFactoryApplication.Views.SettingsUCs
{
    /// <summary>
    /// Interaction logic for GlassesStockSettingsUC.xaml
    /// </summary>
    public partial class GlassesStockSettingsUC : UserControl
    {
        public GlassesStockSettingsUC()
        {
            InitializeComponent();
        }



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GlassesStockSettingsUC), new PropertyMetadata(Orientation.Horizontal));


    }
}

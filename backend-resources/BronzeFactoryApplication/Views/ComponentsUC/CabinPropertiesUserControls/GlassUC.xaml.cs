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
    /// Interaction logic for EditGlassModalUC.xaml
    /// </summary>
    public partial class GlassUC : UserControl
    {

        /// <summary>
        /// Weather the Draw Edit is Allowed
        /// </summary>
        public bool AllowDrawEdit
        {
            get { return (bool)GetValue(AllowDrawEditProperty); }
            set { SetValue(AllowDrawEditProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowDrawEdit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDrawEditProperty =
            DependencyProperty.Register("AllowDrawEdit", typeof(bool), typeof(GlassUC), new PropertyMetadata(false));




        public GlassUC()
        {
            InitializeComponent();
        }
    }
}

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

namespace BronzeFactoryApplication.Views.ComponentsUC.DrawsRelevantUcs
{
    /// <summary>
    /// Interaction logic for DrawingPresentationOptionsGlobalView.xaml
    /// </summary>
    public partial class DrawingPresentationOptionsGlobalView : UserControl
    {
        public DrawingPresentationOptionsGlobalView()
        {
            InitializeComponent();
        }

        private void Expander3_Expanded(object sender, RoutedEventArgs e)
        {
            Expander1.IsExpanded = false;
            Expander2.IsExpanded = false;
        }

        private void Expander2_Expanded(object sender, RoutedEventArgs e)
        {
            Expander1.IsExpanded = false;
            Expander3.IsExpanded = false;
        }

        private void Expander1_Expanded(object sender, RoutedEventArgs e)
        {
            Expander2.IsExpanded = false;
            Expander3.IsExpanded = false;
        }
    }
}

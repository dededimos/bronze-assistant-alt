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
    /// Interaction logic for GlassesOrderUC.xaml
    /// </summary>
    public partial class GlassesOrderUC : UserControl
    {
        public GlassesOrderUC()
        {
            InitializeComponent();
        }

        private void CabinsGroupingToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Adds Grouping to the Glasses Rows
            CollectionViewSource cvs = (CollectionViewSource)OrderRowsGroupBoxGrid.Resources["OrderedCabinsView"];
            cvs.GroupDescriptions.Clear();
            cvs.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CabinOrderRow.SynthesisKey)));
            cvs.View?.Refresh();
        }

        private void CabinsGroupingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //Removes the Grouping from the Glasses Rows
            CollectionViewSource cvs = (CollectionViewSource)OrderRowsGroupBoxGrid.Resources["OrderedCabinsView"];
            cvs.GroupDescriptions.Clear();
            cvs.View?.Refresh();
        }
    }
}

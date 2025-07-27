using CommonHelpers;
using SVGGlassDrawsLibrary;
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
    /// Interaction logic for StockedGlassesModalUC.xaml
    /// </summary>
    public partial class StockedGlassesModalUC : UserControl
    {
        public StockedGlassesModalUC()
        {
            InitializeComponent();
        }

        private void StockListWindowGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Find the Collection View Source inside the Grids Resources
            CollectionViewSource cvs = (CollectionViewSource)StockListWindowGrid.Resources["GlassesStockViewSource"];
            //Add the Filtering for each of the Rows of the CollectionViewSource
            cvs.View.Filter = row =>
            {
                StockedGlassViewModel glass = (StockedGlassViewModel)row;
                if (glass is null ||
                    FilterGlassDrawComboBox is null ||
                    FilterGlassLengthTextBox is null ||
                    FilterGlassHeightTextBox is null ||
                    FilterGlassThicknessComboBox is null)
                    return false;

                //The Filter
                return glass.Glass.Height.ToString().Contains(FilterGlassHeightTextBox.Text)
                && glass.Glass.Length.ToString().Contains(FilterGlassLengthTextBox.Text)
                && (string.IsNullOrEmpty(FilterGlassDrawComboBox.SelectedItem?.ToString()) ||
                 glass.Glass.Draw.ToString() == FilterGlassDrawComboBox.SelectedItem?.ToString())
                && (string.IsNullOrEmpty(FilterGlassThicknessComboBox.SelectedItem?.ToString()) ||
                 glass.Glass.Thickness.ToString() == FilterGlassThicknessComboBox.SelectedItem?.ToString());
            };
        }

        private void FilterGlassDrawComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Refresh the View to reapply Filtering
            CollectionViewSource cvs = (CollectionViewSource)StockListWindowGrid.Resources["GlassesStockViewSource"];
            cvs.View.Refresh();
        }

        private void FilterGlassTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Refresh the View to reapply Filtering
            CollectionViewSource cvs = (CollectionViewSource)StockListWindowGrid.Resources["GlassesStockViewSource"];
            cvs.View.Refresh();
        }

        /// <summary>
        /// Passes the Generated Draw Visual to the Glass being added instead of the Grid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlassUC_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GlassesStockModalViewModel? vm = DataContext as GlassesStockModalViewModel;
            if (vm is not null && vm.IsDrawToList)
            {
                vm.IsDrawToList = false;
                vm.GlassDraw.SetGlassDraw(vm.GlassRowToAdd);
            }
        }
        private void StockListStackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GlassesStockModalViewModel? vm = DataContext as GlassesStockModalViewModel;
            if (vm is not null && vm.IsDrawToList is false)
            {
                //Give the Glass Draw To The Selected Glass
                if (vm.SelectedRow is not null)
                {
                    vm.GlassDraw.SetGlassDraw(vm.SelectedRow.Glass);
                    vm.IsDrawToList = true;
                }
            }
        }
    }
}

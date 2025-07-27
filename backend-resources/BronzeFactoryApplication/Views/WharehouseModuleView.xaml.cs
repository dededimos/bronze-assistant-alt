using GalaxyStockHelper;
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

namespace BronzeFactoryApplication.Views
{
    /// <summary>
    /// Interaction logic for WharehouseModuleView.xaml
    /// </summary>
    public partial class WharehouseModuleView : UserControl
    {
        public WharehouseModuleView()
        {
            InitializeComponent();
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StockDataGrid.ItemsSource is ICollectionView view)
            {
                view.Refresh();
            }
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            CodeFilterTextBox.Text = string.Empty;
            AisleFilterTextBox.Text = string.Empty;
            PositionFilterTextBox.Text = string.Empty;
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not WharehouseItem item)
                return;

            string codeFilter = CodeFilterTextBox?.Text ?? string.Empty;
            string aisleFilter = AisleFilterTextBox?.Text ?? string.Empty;
            string positionFilter = PositionFilterTextBox?.Text ?? string.Empty;

            bool matchesCode = string.IsNullOrEmpty(codeFilter) ||
                              (item.FullCode?.IndexOf(codeFilter, StringComparison.OrdinalIgnoreCase) >= 0);

            bool matchesAisle = string.IsNullOrEmpty(aisleFilter);
            bool matchesPosition = string.IsNullOrEmpty(positionFilter);

            // Check StockInfo collection for matching Aisle or Position
            if (item.StockInfo != null && (item.StockInfo.Count > 0))
            {
                foreach (var stock in item.StockInfo)
                {
                    if (!matchesAisle && !string.IsNullOrEmpty(stock.Aisle) &&
                        stock.Aisle.IndexOf(aisleFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matchesAisle = true;
                    }

                    if (!matchesPosition && !string.IsNullOrEmpty(stock.Position) &&
                        stock.Position.IndexOf(positionFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matchesPosition = true;
                    }

                    if (matchesAisle && matchesPosition)
                        break;
                }
            }

            e.Accepted = matchesCode && matchesAisle && matchesPosition;
        }
    }
}


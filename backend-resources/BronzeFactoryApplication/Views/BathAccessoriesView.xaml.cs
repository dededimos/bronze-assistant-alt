using AccessoriesRepoMongoDB.Entities;
using CommonHelpers;
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
using static CommonHelpers.CommonExtensions;

namespace BronzeFactoryApplication.Views
{
    /// <summary>
    /// Interaction logic for BathAccessoriesView.xaml
    /// </summary>
    public partial class BathAccessoriesView : UserControl
    {
        public BathAccessoriesView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Filters the Accessories ViewSource for the DataGrid Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string codeFilter = CodeFilterTextBox.Text.NormalizeGreekToLatin();

            if (string.IsNullOrWhiteSpace(codeFilter))
            {
                e.Accepted = true;
                return;
            }
            
            if (e.Item is BathAccessoryEntity entity)
            {
                e.Accepted = entity.Code.Contains(codeFilter, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Clears All the Filters for the Accessories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearFiltersClick(object sender, RoutedEventArgs e)
        {
            
            CodeFilterTextBox.Text = string.Empty;
            RefreshAccessoriesCollectionView();
        }

        /// <summary>
        /// Refreshes the CollectionView of Accessories (this will reapply any Filters)
        /// </summary>
        private void RefreshAccessoriesCollectionView()
        {
            // Retrieve the CollectionViewSource from the resources by key
            CollectionViewSource cvs = (CollectionViewSource)MainGrid.Resources["BathAccessoriesViewSource"];

            // Refresh the view to reapply the filter
            cvs.View.Refresh();
        }

        private void CodeFilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RefreshAccessoriesCollectionView();
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            RefreshAccessoriesCollectionView();
        }
    }
}

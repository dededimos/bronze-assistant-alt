using AccessoriesRepoMongoDB.Entities;
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
using UsersRepoMongoDb;

namespace BronzeFactoryApplication.Views.Modals
{
    /// <summary>
    /// Interaction logic for EditUserInfoModalUC.xaml
    /// </summary>
    public partial class EditUserInfoModalUC : UserControl
    {
        public EditUserInfoModalUC()
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
            string codeFilter = NameFilterTextBox.Text.NormalizeGreekToLatin();

            if (string.IsNullOrWhiteSpace(codeFilter))
            {
                e.Accepted = true;
                return;
            }

            if (e.Item is UserInfoEntity entity)
            {
                e.Accepted = entity.GraphUserDisplayName.Contains(codeFilter, StringComparison.InvariantCultureIgnoreCase)
                    || entity.UserName.Contains(codeFilter,StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Clears All the Filters for the Accessories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearFiltersClick(object sender, RoutedEventArgs e)
        {

            NameFilterTextBox.Text = string.Empty;
            RefreshCollectionView();
        }

        /// <summary>
        /// Refreshes the CollectionView of Accessories (this will reapply any Filters)
        /// </summary>
        private void RefreshCollectionView()
        {
            // Retrieve the CollectionViewSource from the resources by key
            CollectionViewSource cvs = (CollectionViewSource)MainGrid.Resources["UsersViewSource"];

            // Refresh the view to reapply the filter
            cvs.View.Refresh();
        }
        private void NameFilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RefreshCollectionView();
            }
        }
        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            RefreshCollectionView();
        }
    }
}

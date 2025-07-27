using AccessoriesRepoMongoDB.Entities;
using MirrorsRepositoryMongoDB.Entities;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.MirrorsViewsUcs.EntitiesManagment.EntitiesDataGridsUCs
{
    /// <summary>
    /// Interaction logic for CoinstraintsDataGrid.xaml
    /// </summary>
    public partial class MirrorConstraintsRepoDatagridView : UserControl
    {
        public MirrorConstraintsRepoDatagridView()
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
            string? codeFilter = ShapeFilterBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(codeFilter))
            {
                e.Accepted = true;
                return;
            }

            if (e.Item is not null && e.Item is MirrorConstraintsEntity entity)
            {
                e.Accepted = entity.Constraints.ConcerningMirrorShape.ToString() == codeFilter;
            }
            else { e.Accepted = false; }
        }
        /// <summary>
        /// Clears All the Filters for the Accessories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearFiltersClick(object sender, RoutedEventArgs e)
        {

            ShapeFilterBox.SelectedItem = null!;
            RefreshCollectionView();
        }
        /// <summary>
        /// Refreshes the CollectionView of Accessories (this will reapply any Filters)
        /// </summary>
        private void RefreshCollectionView()
        {
            // Retrieve the CollectionViewSource from the resources by key
            CollectionViewSource cvs = (CollectionViewSource)Resources["DatagridSource"];

            // Refresh the view to reapply the filter
            cvs.View.Refresh();
        }

        private void ShapeFilterBox_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshCollectionView();
        }
    }
}

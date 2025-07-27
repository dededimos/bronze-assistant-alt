using MirrorsLib.Enums;
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
    /// Interaction logic for MirrorSeriesRepoDatagridView.xaml
    /// </summary>
    public partial class MirrorSeriesRepoDatagridView : UserControl
    {
        public MirrorSeriesRepoDatagridView()
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
            string codeFilter = CodeFilterBox.Text;
            string descriptionFilter = SeriesFilterBox.Text;
            string? shapeFilter = ShapeFilterBox.SelectedItem?.ToString();

            if (e.Item is not null && e.Item is MirrorSeriesElementEntity entity)
            {
                bool containsCode = string.IsNullOrWhiteSpace(codeFilter) || entity.Code.Contains(codeFilter.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase);
                bool containsDescription = string.IsNullOrWhiteSpace(descriptionFilter) || entity.LocalizedDescriptionInfo.Name.LocalizedValues.Values.Any(v => v.RemoveDiacritics().Contains(descriptionFilter.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase));
                bool containsShape = string.IsNullOrEmpty(shapeFilter) || entity.Constraints.ConcerningMirrorShape.ToString().Equals(shapeFilter, StringComparison.OrdinalIgnoreCase);

                e.Accepted = containsCode && containsDescription && containsShape;
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
            CodeFilterBox.Text = string.Empty;
            SeriesFilterBox.Text = string.Empty;
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

        private void TextboxFilterBox_TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshCollectionView();
        }
    }
}

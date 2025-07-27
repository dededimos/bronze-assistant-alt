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
    /// Interaction logic for PositionInstructionsRepoDataGridView.xaml
    /// </summary>
    public partial class PositionInstructionsRepoDataGridView : UserControl
    {
        public PositionInstructionsRepoDataGridView()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string? typeFilter = TypeFilterBox.SelectedItem?.ToString();
            string codeFilter = CodeFilterBox.Text ?? string.Empty;
            codeFilter = codeFilter.NormalizeGreekToLatin().ToLower();

            if (e.Item is not null && e.Item is MirrorElementPositionEntity entity)
            {
                bool evalType = !string.IsNullOrEmpty(typeFilter);
                bool evalCode = !string.IsNullOrEmpty(codeFilter);

                e.Accepted =
                    (evalType == false || entity.Instructions.InstructionsType.ToString() == typeFilter)
                    &&
                    (evalCode == false || entity.Code.Contains(codeFilter, StringComparison.CurrentCultureIgnoreCase));
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
            CodeFilterBox.Text = string.Empty;
            TypeFilterBox.SelectedItem = null!;
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
        private void TypeFilter_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshCollectionView();
        }
        private void ConcerningShapeFilter_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshCollectionView();
        }
        private void TasTextBox_TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RefreshCollectionView();
            }
        }
        private void ApplyFiltersClick(object sender, RoutedEventArgs e)
        {
            RefreshCollectionView();
        }
    }
}

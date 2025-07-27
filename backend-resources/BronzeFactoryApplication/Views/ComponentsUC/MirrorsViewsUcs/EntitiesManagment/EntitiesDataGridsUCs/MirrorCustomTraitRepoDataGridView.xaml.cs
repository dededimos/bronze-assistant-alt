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
    /// Interaction logic for MirrorCustomTraitRepoDataGridView.xaml
    /// </summary>
    public partial class MirrorCustomTraitRepoDataGridView : UserControl
    {
        public MirrorCustomTraitRepoDataGridView()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string codeFilter = CodeFilterBox.Text ?? string.Empty;
            codeFilter = codeFilter.NormalizeGreekToLatin().ToLower();

            if (e.Item is not null && e.Item is MirrorElementEntity entity)
            {
                bool evalCode = !string.IsNullOrEmpty(codeFilter);
                e.Accepted = evalCode == false || entity.Code.ToLower().Contains(codeFilter);
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

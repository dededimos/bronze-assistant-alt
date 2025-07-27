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
    /// Interaction logic for MirrorApplicationOptionsDataGridView.xaml
    /// </summary>
    public partial class MirrorApplicationOptionsDataGridView : UserControl
    {
        public MirrorApplicationOptionsDataGridView()
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
            string optionsTypeFilter = OptionsTypeFilterBox.Text;

            if (string.IsNullOrWhiteSpace(optionsTypeFilter))
            {
                e.Accepted = true;
                return;
            }

            if (e.Item is not null && e.Item is MirrorApplicationOptionsEntity entity)
            {
                e.Accepted = entity.OptionsType.TryTranslateKeyWithoutError().ToLowerInvariant().RemoveDiacritics().Contains(optionsTypeFilter.ToLowerInvariant().RemoveDiacritics());
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
            OptionsTypeFilterBox.Text = string.Empty;
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

        private void OptionsTypeFilterBox_TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshCollectionView();
        }
    }
}

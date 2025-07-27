using BronzeFactoryApplication.ApplicationServices.DataService;
using BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders;
using CommonHelpers;
using Microsoft.AspNetCore.Components.Forms;
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
    /// Interaction logic for GalaxyOrdersDisplayUC.xaml
    /// </summary>
    public partial class GalaxyOrdersDisplayUC : UserControl
    {
        public GalaxyOrdersDisplayUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add the Filters to the CollectionView Source when the Grid Has Been Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource cvs = (CollectionViewSource)DisplayGalaxyOrdersGrid.Resources["GalaxyDocumentsViewSource"];
            cvs.View.Filter = row =>
            {
                BronzeDocumentViewModel doc = (BronzeDocumentViewModel)row;
                if (doc is null || FilterDocumentNumberTextBox is null || FilterClientNameTextBox is null) return false;
                
                //The Filter
                return doc.DocumentNumber.NormalizeGreekToLatin().Contains(FilterDocumentNumberTextBox.Text.NormalizeGreekToLatin())
                && doc.ClientName.NormalizeGreekToLatin().Contains(FilterClientNameTextBox.Text.NormalizeGreekToLatin());
            };
        }

        private void FilterDocumentNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource cvs = (CollectionViewSource)DisplayGalaxyOrdersGrid.Resources["GalaxyDocumentsViewSource"];
            cvs.View.Refresh();
        }
    }
}

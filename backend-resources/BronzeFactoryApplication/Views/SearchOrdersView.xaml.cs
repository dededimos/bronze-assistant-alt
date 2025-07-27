using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
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
    /// Interaction logic for SearchOrdersView.xaml
    /// </summary>
    public partial class SearchOrdersView : UserControl
    {
        /// <summary>
        /// When a match is UnderWay
        /// </summary>
        private bool isCurrentlyMatchingGlassOrCabin;

        public SearchOrdersView()
        {
            InitializeComponent();
        }

        private void GlassesGroupingToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Adds Grouping to the Glasses Rows
            CollectionViewSource cvs = (CollectionViewSource)DataGridsDockPanel.Resources["GlassRowsViewSource"];
            cvs.GroupDescriptions.Clear();
            cvs.SortDescriptions.Clear();
            cvs.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GlassOrderRowViewModel.CabinRowKey)));
            if (GlassRowsDataGrid is not null) GlassRowsDataGrid.CanUserSortColumns = false;
            cvs.View?.Refresh();
        }

        private void GlassesGroupingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //Removes the Grouping from the Glasses Rows
            CollectionViewSource cvs = (CollectionViewSource)DataGridsDockPanel.Resources["GlassRowsViewSource"];
            cvs.GroupDescriptions.Clear();
            cvs.SortDescriptions.Clear();
            if (GlassRowsDataGrid is not null) GlassRowsDataGrid.CanUserSortColumns = true;
            cvs.View?.Refresh();
        }

        private void CabinsGroupingToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Adds Grouping to the Glasses Rows
            CollectionViewSource cvs = (CollectionViewSource)DataGridsDockPanel.Resources["CabinRowsViewSource"];
            cvs.GroupDescriptions.Clear();
            cvs.SortDescriptions.Clear();
            cvs.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CabinOrderRow.SynthesisKey)));
            if(CabinRowsDataGrid is not null) CabinRowsDataGrid.CanUserSortColumns = false;
            cvs.View?.Refresh();
        }

        private void CabinsGroupingToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //Adds Grouping to the Glasses Rows
            CollectionViewSource cvs = (CollectionViewSource)DataGridsDockPanel.Resources["CabinRowsViewSource"];
            cvs.GroupDescriptions.Clear();
            if (CabinRowsDataGrid is not null) CabinRowsDataGrid.CanUserSortColumns = true;
            cvs.View?.Refresh();
        }

        /// <summary>
        /// When Selecting a CabinRow the GlassDatagrid Scrolls into View the First Glass of this Cabin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CabinRowsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CabinsLinkToGlassesToggleButton is null 
                || CabinsLinkToGlassesToggleButton.IsChecked is false or null
                || isCurrentlyMatchingGlassOrCabin) //supress if another match is underWay
            {
                return;
            }
            if (e.AddedItems?.Count > 0)
            {
                if (e.AddedItems[0] is CabinOrderRow cabinRowSelected && GlassRowsDataGrid.ItemsSource != null)
                {
                    //Set A Match is In Progress (will prevent another Match triggering from this one)
                    isCurrentlyMatchingGlassOrCabin = true;

                    //Select the First Glass that Matches the Key (and place also the selected index there)
                    var itemToSelect = GlassRowsDataGrid.ItemsSource.Cast<GlassOrderRowViewModel>().FirstOrDefault(r => r.CabinRowKey == cabinRowSelected.CabinKey);
                    if (itemToSelect is not null)
                    {
                        GlassRowsDataGrid.SelectedItem = itemToSelect;
                        GlassRowsDataGrid.SelectedIndex = GlassRowsDataGrid.Items.IndexOf(itemToSelect);
                        GlassRowsDataGrid.ScrollIntoView(itemToSelect);
                    }

                    // Release the Matching
                    isCurrentlyMatchingGlassOrCabin = false;
                }
            }
        }

        /// <summary>
        /// When Selecting a GlassRow the GlassDatagrid Scrolls into View the First Glass of this Cabin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlassRowsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlassesLinkToCabinsToggleButton is null 
                || GlassesLinkToCabinsToggleButton.IsChecked is false or null 
                || isCurrentlyMatchingGlassOrCabin)
            {
                return;
            }

            if (e.AddedItems?.Count > 0)
            {
                if (e.AddedItems[0] is GlassOrderRowViewModel glassRowSelected && CabinRowsDataGrid.ItemsSource != null)
                {
                    //Set A Match is In Progress (will prevent another Match triggering from this one)
                    isCurrentlyMatchingGlassOrCabin = true;

                    //Select the First Glass that Matches the Key (and place also the selected index there)
                    var itemToSelect = CabinRowsDataGrid.ItemsSource.Cast<CabinOrderRow>().FirstOrDefault(r => r.CabinKey == glassRowSelected.CabinRowKey);
                    if (itemToSelect is not null)
                    {
                        CabinRowsDataGrid.SelectedItem = itemToSelect;
                        CabinRowsDataGrid.SelectedIndex = CabinRowsDataGrid.Items.IndexOf(itemToSelect);
                        CabinRowsDataGrid.ScrollIntoView(itemToSelect);
                    }

                    isCurrentlyMatchingGlassOrCabin = false;
                }
            }
        }

        private void OpenSmallSettings_Click(object sender, RoutedEventArgs e)
        {
            SmallOrdersSettingsPopup.IsOpen = !SmallOrdersSettingsPopup.IsOpen;
        }

        //Change the stays open property on mouse enter leave , so that it doesnt collide with the ButtonClick
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            SmallOrdersSettingsPopup.StaysOpen = true;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            SmallOrdersSettingsPopup.StaysOpen = false;
        }
    }
}

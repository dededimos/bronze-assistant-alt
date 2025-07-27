using System;
using System.Collections;
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

namespace BronzeFactoryApplication.Views.HelperViews
{
    /// <summary>
    /// Interaction logic for TasListBox.xaml
    /// </summary>
    public partial class TasListBox : UserControl
    {
        public TasListBox()
        {
            InitializeComponent();
        }



        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TasListBox), new PropertyMetadata(defaultValue: null));


        public string IconTooltip
        {
            get { return (string)GetValue(IconTooltipProperty); }
            set { SetValue(IconTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTooltipProperty =
            DependencyProperty.Register("IconTooltip", typeof(string), typeof(TasListBox), new PropertyMetadata(string.Empty));



        public string ListBoxTitle
        {
            get { return (string)GetValue(ListBoxTitleProperty); }
            set { SetValue(ListBoxTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListBoxTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListBoxTitleProperty =
            DependencyProperty.Register("ListBoxTitle", typeof(string), typeof(TasListBox), new PropertyMetadata(string.Empty));



        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TasListBox), new PropertyMetadata(defaultValue: null));

        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(TasListBox), new PropertyMetadata(GetDefaultItemsPanel()));


        /// <summary>
        /// Returns a VirtualizingStackPanel for the ListBox (each time a new one this way  Property metadata can have different references between)
        /// </summary>
        /// <returns></returns>
        private static ItemsPanelTemplate GetDefaultItemsPanel()
        {
            // Create a FrameworkElementFactory for VirtualizingStackPanel
            var factory = new FrameworkElementFactory(typeof(VirtualizingStackPanel));
            // Create an ItemsPanelTemplate wrapping the VirtualizingStackPanel
            var template = new ItemsPanelTemplate(factory);

            return template;
        }

    }
}

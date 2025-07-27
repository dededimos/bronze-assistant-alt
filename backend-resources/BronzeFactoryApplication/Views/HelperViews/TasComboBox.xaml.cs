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
    /// Interaction logic for TasComboBox.xaml
    /// </summary>
    public partial class TasComboBox : UserControl
    {
        public TasComboBox()
        {
            InitializeComponent();
        }

        public event SelectionChangedEventHandler ComboBoxSelectionChanged
        {
            add { ComboBox.SelectionChanged += value; }
            remove { ComboBox.SelectionChanged -= value; }
        }

        public string IconToolTip
        {
            get { return (string)GetValue(IconToolTipProperty); }
            set { SetValue(IconToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconToolTipProperty =
            DependencyProperty.Register("IconToolTip", typeof(string), typeof(TasComboBox), new PropertyMetadata(string.Empty));



        public string ComboBoxTitle
        {
            get { return (string)GetValue(ComboBoxTitleProperty); }
            set { SetValue(ComboBoxTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComboBoxTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComboBoxTitleProperty =
            DependencyProperty.Register("ComboBoxTitle", typeof(string), typeof(TasComboBox), new PropertyMetadata(string.Empty));



        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TasComboBox), new PropertyMetadata(defaultValue:null));



        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(TasComboBox), new FrameworkPropertyMetadata(defaultValue: null, flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TasComboBox), new FrameworkPropertyMetadata(defaultValue: null, flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(TasComboBox), new PropertyMetadata(string.Empty));



        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(TasComboBox), new PropertyMetadata(200d));



        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TasComboBox), new PropertyMetadata(defaultValue:null));



        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(TasComboBox), new PropertyMetadata(GetDefaultItemsPanel()));



        /// <summary>
        /// Returns a VirtualizingStackPanel for the Combobox (each time a new one this way  Property metadata can have different references between)
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


        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(TasComboBox), new PropertyMetadata(defaultValue:null));

        public bool ShowClearButton
        {
            get { return (bool)GetValue(ShowClearButtonProperty); }
            set { SetValue(ShowClearButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowClearButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowClearButtonProperty =
            DependencyProperty.Register("ShowClearButton", typeof(bool), typeof(TasComboBox), new PropertyMetadata(false));



        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(TasComboBox), new PropertyMetadata(defaultValue:false));



        public string TextSearchPath
        {
            get { return (string)GetValue(TextSearchPathProperty); }
            set { SetValue(TextSearchPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextSearchPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextSearchPathProperty =
            DependencyProperty.Register("TextSearchPath", typeof(string), typeof(TasComboBox), new PropertyMetadata(defaultValue:null));

        public bool DropDownElementConsistentWidth
        {
            get { return (bool)GetValue(DropDownElementConsistentWidthProperty); }
            set { SetValue(DropDownElementConsistentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownElementConsistentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownElementConsistentWidthProperty =
            DependencyProperty.Register("DropDownElementConsistentWidth", typeof(bool), typeof(TasComboBox), new PropertyMetadata(defaultValue:true));

        public double ComboBoxHeight
        {
            get { return (double)GetValue(ComboBoxHeightProperty); }
            set { SetValue(ComboBoxHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComboBoxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComboBoxHeightProperty =
            DependencyProperty.Register("ComboBoxHeight", typeof(double), typeof(TasComboBox), new PropertyMetadata(double.NaN));




    }
}

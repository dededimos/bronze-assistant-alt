using CommonInterfacesBronze;
using Microsoft.Graph.Drives.Item.Items.Item.Workbook.Functions.Product;
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

namespace BronzeFactoryApplication.Views.HelperViews
{
    /// <summary>
    /// Interaction logic for TasLocalizedStringView.xaml
    /// </summary>
    public partial class TasLocalizedStringView : UserControl
    {
        public TasLocalizedStringView()
        {
            InitializeComponent();
        }




        public LocalizedString LocalizedString
        {
            get { return (LocalizedString)GetValue(LocalizedStringProperty); }
            set { SetValue(LocalizedStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LocalizedString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocalizedStringProperty =
            DependencyProperty.Register("LocalizedString", typeof(LocalizedString), typeof(TasLocalizedStringView), new PropertyMetadata(defaultValue: LocalizedString.Undefined()));



        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(TasLocalizedStringView), new PropertyMetadata(defaultValue:null));



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TasLocalizedStringView), new PropertyMetadata(defaultValue:null));



        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(TasLocalizedStringView), new PropertyMetadata("UndefinedName"));





        public double PropertyNameTextBlockWidth
        {
            get { return (double)GetValue(PropertyNameTextBlockWidthProperty); }
            set { SetValue(PropertyNameTextBlockWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyNameTextBlockWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameTextBlockWidthProperty =
            DependencyProperty.Register("PropertyNameTextBlockWidth", typeof(double), typeof(TasLocalizedStringView), new PropertyMetadata(double.NaN));



        public double TranslatedTextMaxWidth
        {
            get { return (double)GetValue(TranslatedTextMaxWidthProperty); }
            set { SetValue(TranslatedTextMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TranslatedTextMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TranslatedTextMaxWidthProperty =
            DependencyProperty.Register("TranslatedTextMaxWidth", typeof(double), typeof(TasLocalizedStringView), new PropertyMetadata(150d));



        public double PropertyNameMaxWidth
        {
            get { return (double)GetValue(PropertyNameMaxWidthProperty); }
            set { SetValue(PropertyNameMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyNameMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameMaxWidthProperty =
            DependencyProperty.Register("PropertyNameMaxWidth", typeof(double), typeof(TasLocalizedStringView), new PropertyMetadata(150d));





    }
}

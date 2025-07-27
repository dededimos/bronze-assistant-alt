using BronzeFactoryApplication.Helpers.Behaviours;
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
    /// Interaction logic for TasNumericTextBox.xaml
    /// </summary>
    public partial class TasNumericTextBox : UserControl
    {
        public TasNumericTextBox()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TasNumericTextBox), new FrameworkPropertyMetadata(defaultValue: string.Empty, flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            });

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TasNumericTextBox), new PropertyMetadata(Int32.MaxValue));

        public string IconTooltip
        {
            get { return (string)GetValue(IconTooltipProperty); }
            set { SetValue(IconTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTooltipProperty =
            DependencyProperty.Register("IconTooltip", typeof(string), typeof(TasNumericTextBox), new PropertyMetadata(defaultValue: string.Empty));

        // Using a DependencyProperty as the backing store for PlaceHolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(TasNumericTextBox), new PropertyMetadata(defaultValue:string.Empty));



        public string TextBoxTitle
        {
            get { return (string)GetValue(TextBoxTitleProperty); }
            set { SetValue(TextBoxTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxTitleProperty =
            DependencyProperty.Register("TextBoxTitle", typeof(string), typeof(TasNumericTextBox), new PropertyMetadata(string.Empty));

        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Precision.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(int), typeof(TasNumericTextBox), new PropertyMetadata(0));

        public TextBoxInputMode InputMode
        {
            get { return (TextBoxInputMode)GetValue(InputModeProperty); }
            set { SetValue(InputModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputModeProperty =
            DependencyProperty.Register("InputMode", typeof(TextBoxInputMode), typeof(TasNumericTextBox), new PropertyMetadata(TextBoxInputMode.None));

        public bool JustPositiveInput
        {
            get { return (bool)GetValue(JustPositiveInputProperty); }
            set { SetValue(JustPositiveInputProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JustPositiveInput.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JustPositiveInputProperty =
            DependencyProperty.Register("JustPositiveInput", typeof(bool), typeof(TasNumericTextBox), new PropertyMetadata(true));

        public System.Windows.Media.Brush TextBoxForeground
        {
            get { return (System.Windows.Media.Brush)GetValue(TextBoxForegroundProperty); }
            set { SetValue(TextBoxForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxForegroundProperty =
            DependencyProperty.Register("TextBoxForeground", typeof(System.Windows.Media.Brush), typeof(TasNumericTextBox), new PropertyMetadata(Application.Current.Resources["PrimaryTextBrush"] as System.Windows.Media.Brush));



        public Visibility TitleVisibility
        {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(TasNumericTextBox), new PropertyMetadata(Visibility.Visible));



        public bool ShowClearButton
        {
            get { return (bool)GetValue(ShowClearButtonProperty); }
            set { SetValue(ShowClearButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowClearButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowClearButtonProperty =
            DependencyProperty.Register("ShowClearButton", typeof(bool), typeof(TasNumericTextBox), new PropertyMetadata(false));


    }
}

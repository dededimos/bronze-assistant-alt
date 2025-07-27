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
    /// Interaction logic for TasTextBox.xaml
    /// </summary>
    public partial class TasTextBox : UserControl
    {
        public TasTextBox()
        {
            InitializeComponent();
        }
        public event TextChangedEventHandler TextBoxTextChanged
        {
            add { CustomTextBox.TextChanged += value; }
            remove { CustomTextBox.TextChanged -= value; }
        }

        public event KeyEventHandler TextBoxKeyDown
        {
            add { CustomTextBox.KeyDown += value; }
            remove { CustomTextBox.KeyDown -= value; }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TasTextBox),
                new FrameworkPropertyMetadata(defaultValue: null, flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TasTextBox), new PropertyMetadata(Int32.MaxValue));


        /// <summary>
        /// The Place Holder of the TextBox when there is no Value in it
        /// </summary>
        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceHolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(TasTextBox), new PropertyMetadata(defaultValue:string.Empty));



        public string IconTooltip
        {
            get { return (string)GetValue(IconTooltipProperty); }
            set { SetValue(IconTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTooltipProperty =
            DependencyProperty.Register("IconTooltip", typeof(string), typeof(TasTextBox), new PropertyMetadata(string.Empty));



        public string TextBoxTitle
        {
            get { return (string)GetValue(TextBoxTitleProperty); }
            set { SetValue(TextBoxTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxTitleProperty =
            DependencyProperty.Register("TextBoxTitle", typeof(string), typeof(TasTextBox), new PropertyMetadata(string.Empty));



        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TasTextBox), new PropertyMetadata(false));



        public bool ShowClearButton
        {
            get { return (bool)GetValue(ShowClearButtonProperty); }
            set { SetValue(ShowClearButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowClearButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowClearButtonProperty =
            DependencyProperty.Register("ShowClearButton", typeof(bool), typeof(TasTextBox), new PropertyMetadata(false));




        public System.Windows.Media.Brush TextBoxBackground
        {
            get { return (System.Windows.Media.Brush)GetValue(TextBoxBackgroundProperty); }
            set { SetValue(TextBoxBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxBackgroundProperty =
            DependencyProperty.Register("TextBoxBackground", typeof(System.Windows.Media.Brush), typeof(TasTextBox), new PropertyMetadata(Application.Current.Resources["RegionBrush"] as System.Windows.Media.Brush));



        public Thickness TextBoxBorderThickness
        {
            get { return (Thickness)GetValue(TextBoxBorderThicknessProperty); }
            set { SetValue(TextBoxBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxBorderThicknessProperty =
            DependencyProperty.Register("TextBoxBorderThickness", typeof(Thickness), typeof(TasTextBox), new PropertyMetadata(new Thickness(1)));

        public System.Windows.Media.Brush TextBoxForeground
        {
            get { return (System.Windows.Media.Brush)GetValue(TextBoxForegroundProperty); }
            set { SetValue(TextBoxForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBoxForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxForegroundProperty =
            DependencyProperty.Register("TextBoxForeground", typeof(System.Windows.Media.Brush), typeof(TasTextBox), new PropertyMetadata(Application.Current.Resources["PrimaryTextBrush"] as System.Windows.Media.Brush));





    }
}

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
    /// Interaction logic for TasColorPicker.xaml
    /// </summary>
    public partial class TasColorPicker : UserControl
    {
        public TasColorPicker()
        {
            OpenCloseColorPickerCommand = new RelayCommand(OpenCloseColorPicker);
            InitializeComponent();
        }

        public IRelayCommand OpenCloseColorPickerCommand { get; }
        private SolidColorBrush _selectedColorBeforeChange = Brushes.Red;
        private void OpenCloseColorPicker()
        {
            if (!ColorPickerPopUp.IsOpen)
            {
                //Save the Color Currently so to revert
                _selectedColorBeforeChange = SelectedColor.Clone();
            }
            
            ColorPickerPopUp.IsOpen = !ColorPickerPopUp.IsOpen;
        }

        //Change the stays open property on mouse enter leave , so that it doesnt collide with the ButtonClick
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ColorPickerPopUp.StaysOpen = true;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ColorPickerPopUp.StaysOpen = false;
        }
        private void ColorPickerControl_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {
            OpenCloseColorPicker();
        }
        private void ColorPickerControl_Canceled(object sender, EventArgs e)
        {
            SelectedColor = _selectedColorBeforeChange;
            OpenCloseColorPicker();
        }

        public string IconTooltip
        {
            get { return (string)GetValue(IconTooltipProperty); }
            set { SetValue(IconTooltipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTooltip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTooltipProperty =
            DependencyProperty.Register("IconTooltip", typeof(string), typeof(TasColorPicker), new PropertyMetadata(defaultValue:null));



        public string ColorPickerTitle
        {
            get { return (string)GetValue(ColorPickerTitleProperty); }
            set { SetValue(ColorPickerTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorPickerTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorPickerTitleProperty =
            DependencyProperty.Register("ColorPickerTitle", typeof(string), typeof(TasColorPicker), new PropertyMetadata(defaultValue:string.Empty));



        public SolidColorBrush SelectedColor
        {
            get { return (SolidColorBrush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(SolidColorBrush), typeof(TasColorPicker), new PropertyMetadata(Brushes.Red));

        
    }
}

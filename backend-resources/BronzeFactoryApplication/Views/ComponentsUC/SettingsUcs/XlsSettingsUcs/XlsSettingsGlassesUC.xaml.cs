using BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.SettingsUcs.XlsSettingsUcs
{
    /// <summary>
    /// Interaction logic for XlsSettingsGlassesUC.xaml
    /// </summary>
    public partial class XlsSettingsGlassesUC : UserControl
    {
        private Border? colorPickerOwner;

        [RelayCommand]
        private void OpenColorPicker(Border borderToChangeColor)
        {
            colorPickerOwner = borderToChangeColor;
            if (colorPickerOwner is not null)
            {
                ColorPickerControl.SelectedBrush = colorPickerOwner.Background.Clone() as SolidColorBrush;
                maskBorder.Visibility = Visibility.Visible;
                ColorPickerContainer.Visibility = Visibility.Visible;
            }
        }
        
        public XlsSettingsGlassesUC()
        {
            InitializeComponent();
        }


        private void ColorPicker_Confirmed(object sender, HandyControl.Data.FunctionEventArgs<Color> e)
        {

            if (colorPickerOwner is not null)
            {
                colorPickerOwner.Background = ColorPickerControl.SelectedBrush;
            }
            
            ColorPickerContainer.Visibility = Visibility.Collapsed;
            maskBorder.Visibility = Visibility.Collapsed;

            // Release the owner
            colorPickerOwner = null;
        }

        private void ColorPicker_Canceled(object sender, EventArgs e)
        {
            ColorPickerContainer.Visibility = Visibility.Collapsed;
            maskBorder.Visibility = Visibility.Collapsed;

            // Release the owner
            colorPickerOwner = null;
        }

    }
}

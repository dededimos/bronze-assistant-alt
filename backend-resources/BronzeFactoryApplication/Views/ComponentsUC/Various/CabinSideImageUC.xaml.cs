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

namespace BronzeFactoryApplication.Views.ComponentsUC.Various
{
    /// <summary>
    /// Interaction logic for CabinSideImageUC.xaml
    /// </summary>
    public partial class CabinSideImageUC : UserControl
    {
        public CabinSideImageUC()
        {
            InitializeComponent();
        }


        public CabinDrawNumber DrawNumber
        {
            get { return (CabinDrawNumber)GetValue(DrawNumberProperty); }
            set { SetValue(DrawNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawNumberProperty =
            DependencyProperty.Register("DrawNumber", typeof(CabinDrawNumber), typeof(CabinSideImageUC), new PropertyMetadata(CabinDrawNumber.None));



        public CabinSynthesisModel SynthesisModel
        {
            get { return (CabinSynthesisModel)GetValue(SynthesisModelProperty); }
            set { SetValue(SynthesisModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SynthesisModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SynthesisModelProperty =
            DependencyProperty.Register("SynthesisModel", typeof(CabinSynthesisModel), typeof(CabinSideImageUC), new PropertyMetadata(CabinSynthesisModel.Primary));



        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(CabinSideImageUC), new PropertyMetadata(85d));
    }
}

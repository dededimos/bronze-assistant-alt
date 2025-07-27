using BronzeFactoryApplication.Helpers.Other;
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

namespace BronzeFactoryApplication.Views.Modals
{
    /// <summary>
    /// Interaction logic for PrintCabinBomModal.xaml
    /// </summary>
    public partial class PrintCabinBomModal : UserControl
    {
        public PrintCabinBomModal()
        {
            InitializeComponent();
        }

        //How much to scale the ViewBox its time (stores its time how much is already scaled)
        private double scaleFactor = 1;
        
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            scaleFactor -= 0.1; // 10% less
            //Check if actual HEIGHT OF THE VIEWBOX by the factor will get smaller than 0.70 of Window Height
            if (PagesViewBox.ActualWidth * scaleFactor >= 300)
            {
                PagesViewBox.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);
            }
            else
            {
                //else return the scale to what it was and apply nothing
                scaleFactor += 0.1;
            }
        }
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            scaleFactor += 0.1; //10% more
            //Check if the ViewBox will transform to more than the Max it can if not then proceed
            if (PagesViewBox.ActualWidth * scaleFactor <= PagesViewBox.MaxWidth)
            {
                PagesViewBox.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);
            }
            else
            {
                //else return the scale to what it was and apply nothing
                scaleFactor -= 0.1;
            }
        }

        private void TestPrint_Click(object sender, RoutedEventArgs e)
        {
            List<FrameworkElement> elements = new();

            foreach (var item in BomsItemsControl.Items)
            {
                if (BomsItemsControl.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement element)
                {
                    elements.Add(element);
                }
            }
            
            WPFHelpers.PrintFrameWorkElements(elements);
        }
    }
}

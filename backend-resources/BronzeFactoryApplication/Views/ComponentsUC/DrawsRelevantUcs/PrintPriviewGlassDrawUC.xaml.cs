using BronzeFactoryApplication.Helpers.Other;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.DrawsRelevantUcs
{
    /// <summary>
    /// Interaction logic for PrintPriviewGlassDrawUC.xaml
    /// </summary>
    public partial class PrintPriviewGlassDrawUC : UserControl
    {
        public PrintPriviewGlassDrawUC()
        {
            InitializeComponent();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintScrollViewer.ScrollToTop();
            //Otherwise the ScrollViewer is Wider than the Grid and the Internal measurements get messy
            PrintScrollViewer.HorizontalAlignment = HorizontalAlignment.Center;
            

            WPFHelpers.PrintFrameWorkElement(PrintGrid);
            PrintScrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}

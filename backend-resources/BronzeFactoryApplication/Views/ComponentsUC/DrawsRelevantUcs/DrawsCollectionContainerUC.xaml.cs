using SVGDrawingLibrary.Models;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.DrawsRelevantUcs
{
    /// <summary>
    /// Interaction logic for DrawsCollectionContainerUC.xaml
    /// </summary>
    public partial class DrawsCollectionContainerUC : UserControl
    {
        /// <summary>
        /// The Collection of Draws
        /// </summary>
        public IEnumerable<DrawShape> DrawsCollection
        {
            get { return (IEnumerable<DrawShape>)GetValue(DrawsCollectionProperty); }
            set { SetValue(DrawsCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawsCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawsCollectionProperty =
            DependencyProperty.Register("DrawsCollection", typeof(IEnumerable<DrawShape>), typeof(DrawsCollectionContainerUC), new PropertyMetadata(Enumerable.Empty<DrawShape>()));


        public DrawsCollectionContainerUC()
        {
            InitializeComponent();
        }
    }
}

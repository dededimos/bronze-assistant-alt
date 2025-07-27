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
    /// Interaction logic for DrawContainerUC.xaml
    /// </summary>
    public partial class DrawContainerUC : UserControl
    {

        #region 1.VIEWBOX MaxWidth DP
        /// <summary>
        /// The Maximum Width of the Draw ViewBox
        /// </summary>
        public double ViewBoxMaxWidth
        {
            get { return (double)GetValue(ViewBoxMaxWidthProperty); }
            set { SetValue(ViewBoxMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewBoxMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewBoxMaxWidthProperty =
            DependencyProperty.Register("ViewBoxMaxWidth", typeof(double), typeof(DrawContainerUC), new PropertyMetadata(500d));
        #endregion

        #region 2.VIEWBOX MaxHeight DP
        /// <summary>
        /// The Maximum Height of the Draws ViewBox
        /// </summary>
        public double ViewBoxMaxHeight
        {
            get { return (double)GetValue(ViewBoxMaxHeightProperty); }
            set { SetValue(ViewBoxMaxHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewBoxMaxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewBoxMaxHeightProperty =
            DependencyProperty.Register("ViewBoxMaxHeight", typeof(double), typeof(DrawContainerUC), new PropertyMetadata(500d));
        #endregion

        #region 3.DIMENSIONS FillStroke DP
        /// <summary>
        /// The Brush of the Dimensions
        /// </summary>
        public System.Windows.Media.Brush DimensionsFillStroke
        {
            get { return (System.Windows.Media.Brush)GetValue(DimensionsFillStrokeProperty); }
            set { SetValue(DimensionsFillStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DimensionsFillStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DimensionsFillStrokeProperty =
            DependencyProperty.Register("DimensionsFillStroke", typeof(System.Windows.Media.Brush), typeof(DrawContainerUC), new PropertyMetadata(GetDefaultDimensionsBrush()));
        #endregion

        #region 4.Override Glass Stroke DP
        public System.Windows.Media.Brush OverrideGlassStroke
        {
            get { return (System.Windows.Media.Brush)GetValue(OverrideGlassStrokeProperty); }
            set { SetValue(OverrideGlassStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GlassStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverrideGlassStrokeProperty =
            DependencyProperty.Register("OverrideGlassStroke", typeof(System.Windows.Media.Brush), typeof(DrawContainerUC), new PropertyMetadata(Brushes.Transparent)); 
        #endregion


        /// <summary>
        /// Retrieves the Default Value for the Dimensions Brush
        /// </summary>
        /// <returns></returns>
        private static System.Windows.Media.Brush GetDefaultDimensionsBrush()
        {
            var brush = Application.Current.TryFindResource("PrimaryTextBrush") ?? Brushes.Red;
            return (System.Windows.Media.Brush)brush;
        }





        public DrawContainerUC()
        {
            InitializeComponent();
        }
    }
}

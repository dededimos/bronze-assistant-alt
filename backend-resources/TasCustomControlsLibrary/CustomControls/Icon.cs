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

namespace TasCustomControlsLibrary.CustomControls
{
    /// <summary>
    /// An Icon Control
    /// </summary>
    public class Icon : Control
    {
        /// <summary>
        /// The Icon Geometry Data / Path Data
        /// </summary>
        public Geometry GeometryData 
        {
            get
            {
                return (Geometry)GetValue(GeometryDataProperty);
            }
            set
            {
                SetValue(GeometryDataProperty, value);
            }
        }
        public static readonly DependencyProperty GeometryDataProperty =
            DependencyProperty.Register("GeometryData",
                                         typeof(Geometry),
                                         typeof(Icon),
                                         new PropertyMetadata(new PathGeometry()));
        /// <summary>
        /// The Fill of the Icon
        /// </summary>
        public Brush Fill 
        {
            get
            {
                return (Brush)GetValue(FillProperty);
            }
            set
            {
                SetValue(FillProperty, value);
            }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill",
                                         typeof(Brush),
                                         typeof(Icon),
                                         new PropertyMetadata(Brushes.Black));



        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Icon), new PropertyMetadata(0d));



        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(Icon), new PropertyMetadata(Brushes.Transparent));






        //static Icon()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        //}
    }
}

/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:TasCustomControlsLibrary.CustomControls"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:TasCustomControlsLibrary.CustomControls;assembly=TasCustomControlsLibrary.CustomControls"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:Icon/>
///
/// GEORGE:
/// We have to delcare each property that we need to expose to xaml as a dependency property as below
/// The Dependency Properties are as the private fileds of the exposed properties
/// </summary>
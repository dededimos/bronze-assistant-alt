using SVGCabinDraws;
using SVGCabinDraws.ConcreteDraws;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace BronzeFactoryApplication.Views.ComponentsUC.CabinPropertiesUserControls;

/// <summary>
/// Interaction logic for CabinPropertiesUC.xaml
/// </summary>
public partial class CabinPropertiesUC : UserControl
{
    public CabinPropertiesUC()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A Command Binding with which to Open the Advanced Properties of the Current Cabin Model represented by this User Control
    /// The Command Parameter is the Whole DataContext of the UC (a CabinPropertiesViewModel)
    /// </summary>
    public ICommand OpenAdvancedCommand
    {
        get { return (ICommand)GetValue(OpenAdvancedCommandProperty); }
        set { SetValue(OpenAdvancedCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OpenAdvancedCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OpenAdvancedCommandProperty =
        DependencyProperty.Register("OpenAdvancedCommand", typeof(ICommand), typeof(CabinPropertiesUC), new UIPropertyMetadata(null));

    private readonly CabinStepDraw stepDraw = new(300, 300);
    public Geometry StepGeometry { get => Geometry.Parse(stepDraw.Step.GetShapePathData()); }
    public Geometry DimensionStepLength { get => Geometry.Parse(stepDraw.StepLengthDimension.GetShapePathData()); }
    public Geometry DimensionStepHeight { get => Geometry.Parse(stepDraw.StepHeightDimension.GetShapePathData()); }
    public Geometry LeftWall { get => Geometry.Parse(stepDraw.LeftWall.GetShapePathData()); }
    public Geometry Floor { get => Geometry.Parse(stepDraw.Floor.GetShapePathData()); }
    public Geometry HelpLine1 { get => Geometry.Parse(stepDraw.HelpLine1.GetShapePathData()); }
    public Geometry HelpLine2 { get => Geometry.Parse(stepDraw.HelpLine2.GetShapePathData()); }

}

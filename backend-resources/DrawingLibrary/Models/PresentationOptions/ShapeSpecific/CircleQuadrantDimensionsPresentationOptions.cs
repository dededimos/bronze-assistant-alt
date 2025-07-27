using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class CircleQuadrantDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<CircleQuadrantDimensionsPresentationOptions>
    {
        /// <summary>
        /// Weather to show the Radius dimension of the Shape
        /// </summary>
        public bool ShowRadius { get; set; }
        public double RadiusMarginFromShape { get; set; }
        public DimensionLineOptions RadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions RadiusPresentationOptions { get; set; } = new();

        public override CircleQuadrantDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (CircleQuadrantDimensionsPresentationOptions)base.GetDeepClone();
            clone.RadiusLineOptions = this.RadiusLineOptions.GetDeepClone();
            clone.RadiusPresentationOptions = this.RadiusPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}

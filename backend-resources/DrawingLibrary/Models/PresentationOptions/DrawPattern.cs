using ShapesLibrary;
using ShapesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions
{
    /// <summary>
    /// (NOT FINISHED IF NEEDED CAN BE EASILY IMPLEMENTED BUT NEEDS TIME ... BETTER KEEP TWO - THREE PATTERN TYPES ONLY)A pattern to fill shapes , Essentially its a Rectangle Containing a Shape
    /// </summary>
    public abstract class DrawPattern
    {
        /// <summary>
        /// The Length of the Pattern Boundary
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// The Height of the Pattern Boundary
        /// </summary>
        public double Height { get; set; }
        public DrawPattern(double length, double height)
        {
            Length = length;
            Height = height;
        }
    }

    public class DotPattern : DrawPattern
    {
        /// <summary>
        /// Creates a dot Pattern
        /// </summary>
        /// <param name="dotSpacing">The space between the Dots</param>
        /// <param name="dotRadius">The Radius of each dot</param>
        public DotPattern(double dotSpacing, double dotRadius) : base(2*dotSpacing+2*dotRadius, 2 * dotSpacing + 2 * dotRadius)
        {
            DotRadius = dotRadius;
        }
        public double DotRadius { get; }
    }
    public class HatchLinePattern : DrawPattern
    {
        public HatchLinePattern(double lineSpacing,double lineAngleRadians,double lineWidth,double linespacing) : base(lineSpacing,linespacing)
        {
            double diagonalLength = linespacing * Math.Sqrt(2);

            //Calculate it as if it was straight
            //(0,0) is the topLeft of the rect holding the line so start and end are half the size away when the line is drawn passing through the center of the Rect
            var start = new PointXY(-diagonalLength / 2, linespacing / 2);
            var end = new PointXY(diagonalLength / 2, linespacing / 2);
        }
    }
    public class CustomShapePattern : DrawPattern
    {
        public CustomShapePattern(double length, double height) : base(length, height)
        {
        }
    }
}

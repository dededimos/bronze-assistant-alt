

namespace SVGDrawingLibrary.Models
{
    /// <summary>
    /// A coordinates Point X,Y
    /// </summary>
    public class DrawPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// Creates a 0,0 Point
        /// </summary>
        public DrawPoint()
        {

        }

        /// <summary>
        /// Creates a Draw Point with X,Y coordinates
        /// </summary>
        /// <param name="X">the X axis coordinate</param>
        /// <param name="Y">the Y axis coordinate</param>
        public DrawPoint(double X , double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}


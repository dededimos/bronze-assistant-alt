namespace ShapesLibrary.Services
{
    /// <summary>
    /// Represents a point in a XY Coordinate System
    /// </summary>
    /// <param name="X">The X coordinate</param>
    /// <param name="Y">The Y coordinate</param>
    public record struct PointXY(double X, double Y)
    {
        public readonly bool IsNaN => double.IsNaN(X) || double.IsNaN(Y);
        public override readonly string ToString()
        {
            return $"({X:0.###},{Y:0.###})";
        }
        /// <summary>
        /// Returns the Point after a specific scale has been applied to a specific Origin
        /// </summary>
        /// <param name="scaleFactor"></param>
        /// <param name="scaleOrigin"></param>
        /// <returns></returns>
        public readonly PointXY GetScaledPoint(double scaleFactor , PointXY scaleOrigin)
        {
            // Calculate the new X and Y by scaling the distance from the scale origin
            double newX = scaleOrigin.X + (this.X - scaleOrigin.X) * scaleFactor;
            double newY = scaleOrigin.Y + (this.Y - scaleOrigin.Y) * scaleFactor;

            return new PointXY(newX, newY);
        }
    }
}



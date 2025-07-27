using DrawingLibrary.Models.ConcreteGraphics;

namespace DrawingLibrary.Interfaces
{
    public interface IDimensionableDrawing : IDrawing
    {
        /// <summary>
        /// Returns the Drawings of the Dimensions of this Drawing
        /// </summary>
        /// <returns></returns>
        IEnumerable<DimensionLineDrawing> GetDimensionsDrawings();
        /// <summary>
        /// Returns the HelpLines of the Drawing
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDrawing> GetHelpLinesDrawings();
    }
}

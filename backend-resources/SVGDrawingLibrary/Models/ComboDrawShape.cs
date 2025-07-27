using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models
{
    public abstract class ComboDrawShape : DrawShape
    {
        /// <summary>
        /// Gets the List of the Draw Shapes of the Combo
        /// </summary>
        /// <returns>A list of DrawShape Objects</returns>
        public abstract List<DrawShape> GetComboShapes();

        /// <summary>
        /// Returns the Path Data of the Combo Shape
        /// </summary>
        /// <returns>The Path Data String</returns>
        public override string GetShapePathData()
        {
            StringBuilder builder = new();
            foreach (DrawShape shape in GetComboShapes())
            {
                builder.Append(shape.GetShapePathData());
            }
            return builder.ToString();
        }

    }
}
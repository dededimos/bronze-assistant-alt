using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder.Models
{
    /// <summary>
    /// A side of a Mirror Draw -- Contains All the Shapes of the Side being Drawn
    /// </summary>
    public class MirrorDrawSide
    {
        public DrawShape GlassDraw { get; set; }
        public DrawShape SupportDraw { get; set; }
        public DrawShape SandblastDraw { get; set; }
        public List<DrawShape> ExtrasDraws { get; set; } = new();
        public List<DrawShape> DimensionsDraws { get; set; } = new();

        public MirrorDrawSide()
        {

        }

        /// <summary>
        /// Returns the Shapes List of the Draw
        /// </summary>
        /// <returns>A List of DrawShapes</returns>
        public List<DrawShape> GetShapesToDraw()
        {
            List<DrawShape> shapes = new();
            
            //Add Glass Draw
            if (GlassDraw is not null) { shapes.Add(GlassDraw);}
            
            //Add Support Draw
            if (SupportDraw is not null) { shapes.Add(SupportDraw); }
            
            //Add Sandblast Draw
            if (SandblastDraw is not null) { shapes.Add(SandblastDraw); }
            
            //Add Extras Draws
            foreach (var shape in ExtrasDraws)
            {
                shapes.Add(shape);
            }

            return shapes; //All Draws in A List
        }


    }
}

using SVGDrawingLibrary.Enums;
using MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder
{
    public class MirrorDrawBuilderOptions
    {
        /// <summary>
        /// The Mirror to Draw
        /// </summary>
        public Mirror MirrorToDraw { get; set; }
        /// <summary>
        /// The Side that is Being Drawn
        /// </summary>
        public DrawnSide SideToDraw { get; set; }
        /// <summary>
        /// The Margins of the Draw
        /// </summary>
        public double DrawMargin { get; set; }
    }
}

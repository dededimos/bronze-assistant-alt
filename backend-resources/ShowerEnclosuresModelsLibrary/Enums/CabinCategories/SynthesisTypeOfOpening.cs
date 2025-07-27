using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums.CabinCategories
{
    /// <summary>
    /// The Type of Opening of the Synthesis (Single Piece Opening , Diagonal , Double Straight e.t.c.)
    /// </summary>
    public enum SynthesisTypeOfOpening
    {
        /// <summary>
        /// Without Opening
        /// </summary>
        None,
        /// <summary>
        /// One Single Piece that Opens
        /// </summary>
        SinglePiece,
        /// <summary>
        /// Two Diagonal Pieces that Open
        /// </summary>
        Diagonal,
        /// <summary>
        /// Two Straight Pieces that Open
        /// </summary>
        StraightTwoPiece,
        
        /// <summary>
        /// Flipper Panel Opening
        /// </summary>
        Flipper

    }
}

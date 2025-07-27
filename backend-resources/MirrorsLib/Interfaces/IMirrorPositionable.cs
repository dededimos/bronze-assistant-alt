using ShapesLibrary;
using ShapesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Interfaces
{
    public interface IMirrorPositionable
    {
        /// <summary>
        /// The Minimum Distance that the Positionable can be from the Support
        /// </summary>
        public double MinDistanceFromSupport { get; }
        /// <summary>
        /// The Minimum Distance that the Positionable can be from the Sandblast
        /// </summary>
        public double MinDistanceFromSandblast { get; }
        /// <summary>
        /// The Minimum Distance that the Positionable can be from other Modules
        /// </summary>
        public double MinDistanceFromOtherModules { get; }

        /// <summary>
        /// Sets the Position of this Positionable
        /// </summary>
        /// <param name="point">the position Point of the Item</param>
        /// <param name="parentCenter">The Center of the Parent</param>
        public void SetPosition(PointXY point, PointXY parentCenter);
        /// <summary>
        /// Returns the Current Position of the Positionable inside its parent
        /// If item is not placed yet, returns 0,0
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition();
    }
}

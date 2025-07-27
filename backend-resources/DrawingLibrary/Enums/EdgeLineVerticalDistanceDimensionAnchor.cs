using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Enums
{
    /// <summary>
    /// The Anchor Point of a Dimension Line measuring the Vertical Distance of and Edge from somewhere
    /// <para>Example : Left => The Distance Dimension of a rectangle from another line above it will be anchored to the Left Top Vertex of the Rectangle</para>
    /// </summary>
    public enum EdgeLineVerticalDistanceDimensionAnchor
    {
        None,
        Left,
        Middle,
        Right,
    }
}

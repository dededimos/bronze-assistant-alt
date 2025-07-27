using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Exceptions
{
    public class NotSupportedProjectionException(object shapeOrPoint,Vector2D direction) : Exception($"Projection of {shapeOrPoint.GetType().Name} onto a certain Direction Vector is Not Supported , Direction :{direction}"){}
}

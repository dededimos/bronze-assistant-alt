using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Interfaces
{
    public interface IRingableShape : IShapeInfo
    {
        IRingShape GetRingShape(double ringThickness);
    }
}

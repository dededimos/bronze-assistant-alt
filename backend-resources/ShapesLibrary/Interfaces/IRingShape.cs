using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Interfaces
{
    public interface IRingShape : IShapeInfo
    {
        public double Thickness { get; }
        IRingableShape GetInnerRingWholeShape();
        IRingableShape GetOuterRingWholeShape();
    }
}

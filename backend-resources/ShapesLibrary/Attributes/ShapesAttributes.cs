using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShapeOriginAttribute : Attribute
    {
        private readonly string origin;
        public ShapeOriginAttribute(string origin)
        {
            this.origin = origin;
        }
    }
}

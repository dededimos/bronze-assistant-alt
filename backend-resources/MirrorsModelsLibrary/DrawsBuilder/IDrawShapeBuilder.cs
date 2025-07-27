using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder
{
    public interface IDrawShapeBuilder
    {
        public DrawShape BuildShape();
        public void ResetBuilder();
    }
}

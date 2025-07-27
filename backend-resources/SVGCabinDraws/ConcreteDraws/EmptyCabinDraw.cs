using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGCabinDraws.ConcreteDraws
{
    public class EmptyCabinDraw : CabinDraw<Cabin>
    {
        public override CabinFinishEnum MetalFinish { get => CabinFinishEnum.NotSet; }
        public override double SingleDoorOpening { get => 0; }

        public EmptyCabinDraw(Cabin cabin):base(cabin)
        {
            
        }

        public override List<DrawShape> GetAllDraws()
        {
            return new();
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            return new();
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            return new();
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            return new();
        }

        protected override void InitilizeDraw()
        {
            return;
        }

        protected override void PlaceDrawNames()
        {
            return;
        }

        protected override void PlaceParts()
        {
            return;
        }
    }
}

using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.FreeDraws
{
    public class CabinWFlipperDraw : CabinDraw<CabinWFlipper>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;
        public DrawShape HingeTop { get; set; }
        public DrawShape HingeBottom { get; set; }
        public DrawShape DoorGlassDraw1 { get; set; }

        private Glass DoorGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }

        public CabinWFlipperDraw(CabinWFlipper cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            DoorGlassDraw1 = DrawsFactory.BuildGlassDraw(DoorGlass);
            DoorGlassDraw1.LayerNo = 1;

            HingeTop = DrawsFactory.HingeDraws.BuildHingeNew(cabin.Parts.Hinge);
            
            HingeBottom = DrawsFactory.HingeDraws.BuildHingeNew(cabin.Parts.Hinge);
        }

        protected override void PlaceParts()
        {
            //AT (0 + AER , 0)
            DoorGlassDraw1.SetCenterOrStartX(cabin.Parts.Hinge.GlassGapAER, CSCoordinate.Start);
            DoorGlassDraw1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection,CSCoordinate.Start);

            //At (StarX of Glass + LengthInGlass - Total Length , StartY of Glass + Distance from Top Of Glass)
            HingeTop.SetCenterOrStartX(
                DoorGlassDraw1.GetBoundingBoxRectangle().StartX
                + cabin.Parts.Hinge.InDoorLength
                - cabin.Parts.Hinge.LengthView
                , CSCoordinate.Start);
            HingeTop.SetCenterOrStartY(
                DoorGlassDraw1.GetBoundingBoxRectangle().StartY 
                + GlassProcessesConstants.ProcessesW.FlipperHingeHoleTopDistance
                , CSCoordinate.Center);

            //At (StarX of Glass + LengthInGlass - Total Length , EndY of Glass - Distance from Bottom Of Glass)
            HingeBottom.SetCenterOrStartX(
                DoorGlassDraw1.GetBoundingBoxRectangle().StartX
                + cabin.Parts.Hinge.InDoorLength
                - cabin.Parts.Hinge.LengthView
                , CSCoordinate.Start);
            HingeBottom.SetCenterOrStartY(
                DoorGlassDraw1.GetBoundingBoxRectangle().EndY 
                - GlassProcessesConstants.ProcessesW.FlipperHingeHoleBottomDistance
                , CSCoordinate.Center);
        }

        protected override void PlaceDrawNames()
        {
            DoorGlassDraw1.Name = nameof(DoorGlassDraw1);
            HingeTop.Name = nameof(HingeTop);
            HingeBottom.Name = nameof(HingeBottom);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(HingeTop);
            draws.Add(HingeBottom);
            draws.Add(DoorGlassDraw1);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(DoorGlassDraw1);
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(HingeTop);
            draws.Add(HingeBottom);
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            return new();
        }

    }
}

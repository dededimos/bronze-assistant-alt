using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using SVGDrawingLibrary.Models;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.DBDraws
{
    public class CabinDBDraw : CabinDraw<CabinDB>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;

        public DrawShape HingeTop { get; set; }
        public DrawShape HingeWallPlateTop { get; set; }
        public DrawShape HingeBottom { get; set; }
        public DrawShape HingeWallPlateBottom { get; set; }
        public DrawShape DoorHandle1 { get; set; }
        public DrawShape DoorGlassDraw1 { get; set; }
        public DrawShape MagnetStrip { get; set; }
        public DrawShape MagnetAluminium { get; set; }

        private Glass DoorGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }

        /// <summary>
        /// Creates a Draw of a DB Cabin
        /// </summary>
        /// <param name="cabin"></param>
        public CabinDBDraw(CabinDB cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            HingeTop = DrawsFactory.HingeDraws.BuildHingeDBMainBody(cabin.Parts.Hinge);
            HingeWallPlateTop = DrawsFactory.HingeDraws.BuildHingeDBWallPlate(cabin.Parts.Hinge);

            HingeBottom = DrawsFactory.HingeDraws.BuildHingeDBMainBody(cabin.Parts.Hinge);
            HingeWallPlateBottom = DrawsFactory.HingeDraws.BuildHingeDBWallPlate(cabin.Parts.Hinge);

            if (cabin.Parts.Handle is not null)
            {
                DoorHandle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            }

            DoorGlassDraw1 = DrawsFactory.BuildGlassDraw(DoorGlass);
            DoorGlassDraw1.LayerNo = 1;

            if (cabin.Parts.CloseStrip is not null)
            {
                MagnetStrip = DrawsFactory.BuildStripDraw(
                    cabin.Parts.CloseStrip.OutOfGlassLength, 
                    cabin.Parts.CloseStrip.CutLength);
            }

            if (cabin.Parts.CloseProfile is not null)
            {
                MagnetAluminium = DrawsFactory.BuildProfileDraw(
                    cabin.Parts.CloseProfile.ThicknessView, 
                    cabin.Parts.CloseProfile.CutLength);
            }

        }

        protected override void PlaceParts()
        {

            //At (Position after GlassGap , 0)
            DoorGlassDraw1.SetCenterOrStartX(cabin.Parts.Hinge.GlassGapAER, CSCoordinate.Start);
            //If there is correction move it a little bit further down.
            DoorGlassDraw1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

            //At (After Thickness of WallPlate , Position of Cut From Top of the Glass)
            HingeTop.SetCenterOrStartX(cabin.Parts.Hinge.WallPlateThicknessView, CSCoordinate.Start);
            HingeTop.SetCenterOrStartY(DoorGlassDraw1.GetBoundingBoxRectangle().StartY + GlassProcessesConstants.ProcessesHotel8000.CutTopDistanceDB, CSCoordinate.Center);
            HingeWallPlateTop.SetCenterOrStartX(0, CSCoordinate.Start);
            HingeWallPlateTop.SetCenterOrStartY(HingeTop.ShapeCenterY, CSCoordinate.Center);

            //At (After Thickness of Wall Plate, Position of Cut from Bottom of the Glass -- Door Glass Start Draw from Y=0 )
            HingeBottom.SetCenterOrStartX(cabin.Parts.Hinge.WallPlateThicknessView, CSCoordinate.Start);
            HingeBottom.SetCenterOrStartY(DoorGlassDraw1.GetBoundingBoxRectangle().EndY - GlassProcessesConstants.ProcessesHotel8000.CutBottomDistanceDB, CSCoordinate.Center);
            HingeWallPlateBottom.SetCenterOrStartX(0, CSCoordinate.Start);
            HingeWallPlateBottom.SetCenterOrStartY(HingeBottom.ShapeCenterY, CSCoordinate.Center);

            if (DoorHandle1 is not null)
            {
                //At (EndOf Glass - Distance from End , Middle of Glass)
                DoorHandle1.SetCenterOrStartX(
                    DoorGlassDraw1.GetBoundingBoxRectangle().EndX
                    - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge()
                    , CSCoordinate.Center);
                DoorHandle1.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY, CSCoordinate.Center);
            }

            if (MagnetStrip != null)
            {
                //At (EndOf Glass , 0 )
                MagnetStrip.SetCenterOrStartX(DoorGlassDraw1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                MagnetStrip.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY, CSCoordinate.Center);
                
                if (MagnetAluminium != null)
                {
                    //At (EndX of Strip , StartY of Strip)
                    MagnetAluminium.SetCenterOrStartX(MagnetStrip.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                    MagnetAluminium.SetCenterOrStartY(MagnetStrip.GetBoundingBoxRectangle().StartY, CSCoordinate.Start);
                }
            }
        }

        protected override void PlaceDrawNames()
        {
            HingeTop.Name = nameof(HingeTop);
            HingeWallPlateTop.Name = nameof(HingeWallPlateTop);
            HingeBottom.Name = nameof(HingeBottom);
            HingeWallPlateBottom.Name = nameof(HingeWallPlateBottom);

            if (DoorHandle1 is not null)
            {
                DoorHandle1.Name = nameof(DoorHandle1);
            }

            DoorGlassDraw1.Name = nameof(DoorGlassDraw1);

            if (MagnetStrip != null)
            {
                MagnetStrip.Name = nameof(MagnetStrip);
            }
            if (MagnetAluminium != null)
            {
                MagnetAluminium.Name = nameof(MagnetAluminium);
            }
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(HingeTop);
            draws.Add(HingeWallPlateTop);
            draws.Add(HingeBottom);
            draws.Add(HingeWallPlateBottom);
            draws.AddNotNull(DoorHandle1);
            draws.Add(DoorGlassDraw1);
            draws.AddNotNull(MagnetStrip);
            draws.AddNotNull(MagnetAluminium);

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
            draws.Add(HingeWallPlateTop);
            draws.Add(HingeBottom);
            draws.Add(HingeWallPlateBottom);
            draws.AddNotNull(DoorHandle1);
            draws.AddNotNull(MagnetAluminium);
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(MagnetStrip);
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }
    }
}

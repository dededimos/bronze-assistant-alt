using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.WSDraws
{
    public class CabinWSDraw : CabinDraw<CabinWS>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;

        public DrawShape WallAluminium1 { get; set; }
        public DrawShape MagnetStrip1 { get; set; }
        public DrawShape MagnetAluminium { get; set; }
        public DrawShape Handle1 { get; set; }
        public DrawShape SupportBarGuide { get; set; }
        public DrawShape DoorStopperMetalPart { get; set; }
        public DrawShape DoorStopperBumperPart { get; set; }
        public DrawShape GuiderBottom1 { get; set; }
        public DrawShape Door1WheelLeft { get; set; }
        public DrawShape Door1WheelRight { get; set; }

        public DrawShape SlidingGlassDraw1 { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }

        private Glass SlidingGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }
        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }

        public CabinWSDraw(CabinWS cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            WallAluminium1 = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallFixer.ThicknessView,
                cabin.Parts.WallFixer.CutLength);

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);
            SlidingGlassDraw1 = DrawsFactory.BuildGlassDraw(SlidingGlass);

            if (cabin.Parts.CloseStrip is not null)
            {
                MagnetStrip1 = DrawsFactory.BuildStripDraw(
                    cabin.Parts.CloseStrip.OutOfGlassLength, 
                    cabin.Parts.CloseStrip.CutLength);
            }

            if (cabin.Parts.CloseProfile is not null)
            {
                MagnetAluminium = DrawsFactory.BuildProfileDraw(
                    cabin.Parts.CloseProfile.ThicknessView, 
                    cabin.Parts.CloseProfile.CutLength);
            }

            if (cabin.Parts.Handle is not null)
            {
                Handle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            }
            
            SupportBarGuide = DrawsFactory.BuildSupportBarClamp(cabin.Parts.SupportBar);
            DoorStopperMetalPart = DrawsFactory.SmartInoxParts.BuildDoorStopper();
            DoorStopperBumperPart = DrawsFactory.SmartInoxParts.BuildStopperBumper();
            
            GuiderBottom1 = DrawsFactory.BuildProfileDraw(
                cabin.LengthMin 
                - cabin.Parts.WallFixer.ThicknessView, 
                CabinPartsDimensions.SmartInoxParts.BottomDriverHeight);
            
            Door1WheelLeft = DrawsFactory.SmartInoxParts.BuildWheel();
            Door1WheelRight = DrawsFactory.SmartInoxParts.BuildWheel();

            FixedGlassDraw1.LayerNo = 1;
            DoorStopperMetalPart.LayerNo = 2;
            DoorStopperBumperPart.LayerNo = 2;
            Door1WheelLeft.LayerNo = 2;
            Door1WheelRight.LayerNo = 2;
            GuiderBottom1.LayerNo = 2;
            if (Handle1 is not null)
            {
                Handle1.LayerNo = 2;
            }
            SlidingGlassDraw1.LayerNo = 3;
            if (MagnetStrip1 is not null)
            {
                MagnetStrip1.LayerNo = 3;
            }
        }

        protected override void PlaceParts()
        {
            //At (0 , 0 + Support Bar Out Of Glass Height if its considered in the Cabin Height
            WallAluminium1.SetCenterOrStartX(0, CSCoordinate.Start);
            WallAluminium1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

            //At (EndX of AL1 - ALST , StartY of AL1)
            FixedGlassDraw1.SetCenterOrStartX(
                WallAluminium1.GetBoundingBoxRectangle().EndX 
                - cabin.Parts.WallFixer.GlassInProfileDepth, 
                CSCoordinate.Start);
            FixedGlassDraw1.SetCenterOrStartY(
                WallAluminium1.GetBoundingBoxRectangle().StartY,
                CSCoordinate.Start);
            
            double fixedGlassStartX = FixedGlassDraw1.GetBoundingBoxRectangle().StartX;
            double fixedGlassStartY = FixedGlassDraw1.GetBoundingBoxRectangle().StartY;
            double fixedGlassEndX = FixedGlassDraw1.GetBoundingBoxRectangle().EndX;
            double fixedGlassEndY = FixedGlassDraw1.GetBoundingBoxRectangle().EndY;

            //The SupportBar StartX Is Placed on the Fixed Glass - Overlap + Length of Stopper , StartY of Glass - OutOfGlassLength
            SupportBarGuide.SetCenterOrStartX(
                fixedGlassEndX 
                - cabin.Constraints.Overlap
                + CabinPartsDimensions.SmartInoxParts.DoorStopperWidth, 
                CSCoordinate.Start);
            SupportBarGuide.SetCenterOrStartY(
                fixedGlassStartY 
                - cabin.Parts.SupportBar.OutOfGlassHeight, 
                CSCoordinate.Start);

            //At EndX of Fixed Glass - Overlap , StartY of FixedGlass
            SlidingGlassDraw1.SetCenterOrStartX(
                fixedGlassEndX 
                - cabin.Constraints.Overlap, 
                CSCoordinate.Start);
            SlidingGlassDraw1.SetCenterOrStartY(
                fixedGlassStartY, 
                CSCoordinate.Start);
            

            double slidingGlassStartX = SlidingGlassDraw1.GetBoundingBoxRectangle().StartX;
            double slidingGlassStartY = SlidingGlassDraw1.GetBoundingBoxRectangle().StartY;
            double slidingGlassEndX = SlidingGlassDraw1.GetBoundingBoxRectangle().EndX;
            double slidingGlassEndY = SlidingGlassDraw1.GetBoundingBoxRectangle().EndY;

            //At (StartX of Sliding Glass , StartY of Sliding Glass - Out Of Glass Height)
            DoorStopperMetalPart.SetCenterOrStartX(
                slidingGlassStartX, 
                CSCoordinate.Start);
            DoorStopperMetalPart.SetCenterOrStartY(
                slidingGlassStartY 
                - cabin.Parts.SupportBar.OutOfGlassHeight, 
                CSCoordinate.Start);
            

            //At (EndX of DoorStopperMeta , StartY of Metal + Distance from Top)
            DoorStopperBumperPart.SetCenterOrStartX(
                DoorStopperMetalPart.GetBoundingBoxRectangle().EndX, 
                CSCoordinate.Start);
            DoorStopperBumperPart.SetCenterOrStartY(
                DoorStopperMetalPart.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.SmartInoxParts.BumperDistanceFromTopOfStopper, 
                CSCoordinate.Start);

            //At (StartX of Sliding Glass + Distance from Left , EndY of Glass - Distance from Bottom)
            Door1WheelLeft.SetCenterOrStartX(
                slidingGlassStartX 
                + GlassProcessesConstants.ProcessesWS.WheelHoleLeftDistance, 
                CSCoordinate.Center);
            Door1WheelLeft.SetCenterOrStartY(
                slidingGlassEndY 
                - GlassProcessesConstants.ProcessesWS.WheelHoleBottomDistance, 
                CSCoordinate.Center);

            //At (EndX of Sliding Glass - Distance from Right , EndY of Glass - Distance from Bottom)
            Door1WheelRight.SetCenterOrStartX(
                slidingGlassEndX 
                - GlassProcessesConstants.ProcessesWS.WheelHoleRightDistance, 
                CSCoordinate.Center);
            Door1WheelRight.SetCenterOrStartY(
                slidingGlassEndY 
                - GlassProcessesConstants.ProcessesWS.WheelHoleBottomDistance, 
                CSCoordinate.Center);

            //At (EndX of AL1 , EndY of FixedGlass - Height of Driver)
            GuiderBottom1.SetCenterOrStartX(
                WallAluminium1.GetBoundingBoxRectangle().EndX, 
                CSCoordinate.Start);
            GuiderBottom1.SetCenterOrStartY(
                fixedGlassEndY 
                - CabinPartsDimensions.SmartInoxParts.BottomDriverHeight, 
                CSCoordinate.Start);

            if (Handle1 is not null)
            {
                //At (EndX of Sliding Glass - Distance from Right , Middle of Glass)
                Handle1.SetCenterOrStartX(
                    slidingGlassEndX
                    - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge(),
                    CSCoordinate.Center);
                Handle1.SetCenterOrStartY(
                    SlidingGlassDraw1.ShapeCenterY,
                    CSCoordinate.Center);
            }

            if (MagnetStrip1 != null)
            {
                //At EndX of Sliding Glass , StartY of Sliding Glass
                MagnetStrip1.SetCenterOrStartX(slidingGlassEndX, CSCoordinate.Start);
                MagnetStrip1.SetCenterOrStartY(slidingGlassStartY, CSCoordinate.Start);
                
                if (MagnetAluminium != null)
                {
                    MagnetAluminium.SetCenterOrStartX(MagnetStrip1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                    MagnetAluminium.SetCenterOrStartY(slidingGlassStartY, CSCoordinate.Start);
                }
            }
        }

        protected override void PlaceDrawNames()
        {
            WallAluminium1.Name = nameof(WallAluminium1);
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
            SlidingGlassDraw1.Name = nameof(SlidingGlassDraw1);
            
            if (MagnetStrip1 != null)
            {
                MagnetStrip1.Name = nameof(MagnetStrip1);
            }
            
            if (MagnetAluminium != null)
            {
                MagnetAluminium.Name = nameof(MagnetAluminium);
            }
            
            if (Handle1 is not null)
            {
                Handle1.Name = nameof(Handle1);
            }
            
            SupportBarGuide.Name = nameof(SupportBarGuide);
            DoorStopperMetalPart.Name = nameof(DoorStopperMetalPart);
            DoorStopperBumperPart.Name = nameof(DoorStopperBumperPart);
            GuiderBottom1.Name = nameof(GuiderBottom1);
            Door1WheelLeft.Name = nameof(Door1WheelLeft);
            Door1WheelRight.Name = nameof(Door1WheelRight);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(FixedGlassDraw1);
            draws.Add(SlidingGlassDraw1);
            draws.AddNotNull(MagnetStrip1);
            draws.AddNotNull(MagnetAluminium);
            draws.AddNotNull(Handle1);
            draws.Add(SupportBarGuide);
            draws.Add(DoorStopperMetalPart);
            draws.Add(DoorStopperBumperPart);
            draws.Add(GuiderBottom1);
            draws.Add(Door1WheelLeft);
            draws.Add(Door1WheelRight);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(SlidingGlassDraw1);
            draws.Add(FixedGlassDraw1);
            return draws;
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.AddNotNull(MagnetAluminium);
            draws.AddNotNull(Handle1);
            draws.Add(SupportBarGuide);
            draws.Add(DoorStopperMetalPart);
            draws.Add(GuiderBottom1);
            draws.Add(Door1WheelLeft);
            draws.Add(Door1WheelRight);
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(MagnetStrip1);
            return draws;
        }

    }
}

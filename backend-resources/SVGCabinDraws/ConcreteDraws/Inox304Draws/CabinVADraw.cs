using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.Inox304Draws
{
    //EXACT COPY OF VS WITHOUT MAGNET ALUMINIUM !!
    public class CabinVADraw : CabinDraw<CabinVA>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;
        public DrawShape WallSupportTop1 { get; set; }
        public DrawShape WallSupportBottom1 { get; set; }
        public DrawShape WallProfile1 { get; set; }
        public DrawShape StepWallProfile { get; set; }
        public DrawShape Bar { get; set; }
        public DrawShape BarBaseLeft { get; set; }
        public DrawShape AngleConnector { get; set; }
        public DrawShape MagnetStrip1 { get; set; }
        public DrawShape Handle1 { get; set; }
        public DrawShape DoorStopper1 { get; set; }
        public DrawShape StopperBumper1 { get; set; }
        public DrawShape DoorStopper2 { get; set; }
        public DrawShape StopperBumper2 { get; set; }
        public DrawShape GuiderBottom1 { get; set; }
        public DrawShape FloorProfile1 { get; set; }
        public DrawShape StepFloorProfile { get; set; }
        public DrawShape Door1WheelLeft { get; set; }
        public DrawShape Door1WheelRight { get; set; }
        public DrawShape Door1WheelLockLeft { get; set; }
        public DrawShape Door1WheelLockRight { get; set; }
        public DrawShape Fixed1LockLeft { get; set; }
        public DrawShape Fixed1LockRight { get; set; }
        public DrawShape SlidingGlassDraw1 { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }

        /// <summary>
        /// If there is Step
        /// </summary>
        public DrawShape StepWallArea { get; set; }

        private Glass SlidingGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }
        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }


        public CabinVADraw(CabinVA cabin) : base(cabin) { }
        
        protected override void InitilizeDraw()
        {
            BarBaseLeft = DrawsFactory.Inox304Parts.BuildBarBase();
            AngleConnector = DrawsFactory.BuildAngleConnector(cabin.Parts.Angle);

            DoorStopper1 = DrawsFactory.Inox304Parts.BuildDoorStopper();
            DoorStopper2 = DrawsFactory.Inox304Parts.BuildDoorStopper();
            StopperBumper1 = DrawsFactory.Inox304Parts.BuildStopperBumper();
            StopperBumper2 = DrawsFactory.Inox304Parts.BuildStopperBumper();

            if (cabin.Parts.WallSideFixer is CabinSupport wallsupport1)
            {
                WallSupportTop1 = DrawsFactory.HingeDraws.BuildGlassSupport(wallsupport1);
                WallSupportBottom1 = DrawsFactory.HingeDraws.BuildGlassSupport(wallsupport1);
            }
            else if (cabin.Parts.WallSideFixer is Profile wallProfile1)
            {
                WallProfile1 = DrawsFactory.BuildProfileDraw(wallProfile1.ThicknessView, wallProfile1.CutLength);
                if (cabin.HasStep)
                {
                    StepWallProfile = DrawsFactory.BuildProfileDraw(wallProfile1.ThicknessView, wallProfile1.CutLengthStepPart);
                }
            }

            if (cabin.Parts.Handle is not null)
            {
                Handle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            }

            MagnetStrip1 = DrawsFactory.BuildStripDraw(
                cabin.Parts.CloseStrip.OutOfGlassLength, 
                cabin.Parts.CloseStrip.CutLength);

            Door1WheelLeft = DrawsFactory.Inox304Parts.BuildWheel();
            Door1WheelRight = DrawsFactory.Inox304Parts.BuildWheel();

            Door1WheelLockLeft = DrawsFactory.Inox304Parts.BuildWheelLock();
            Door1WheelLockRight = DrawsFactory.Inox304Parts.BuildWheelLock();

            Fixed1LockLeft = DrawsFactory.Inox304Parts.BuildFixedGlassLock();
            Fixed1LockRight = DrawsFactory.Inox304Parts.BuildFixedGlassLock();

            if (cabin.Parts.BottomFixer is CabinSupport bottomGuider)
            {
                GuiderBottom1 = DrawsFactory.HingeDraws.BuildGlassSupport(bottomGuider);
            }
            else if (cabin.Parts.BottomFixer is Profile floorProfile1)
            {
                FloorProfile1 = DrawsFactory.BuildProfileDraw(floorProfile1.CutLength, floorProfile1.ThicknessView);
                if (cabin.HasStep)
                {
                    StepFloorProfile = DrawsFactory.BuildProfileDraw(floorProfile1.CutLengthStepPart, floorProfile1.ThicknessView);
                }
            }

            Bar = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalBar.CutLength,
                cabin.Parts.HorizontalBar.ThicknessView);

            BarBaseLeft = DrawsFactory.Inox304Parts.BuildBarBase();
            AngleConnector = DrawsFactory.Inox304Parts.BuildBarBase();

            SlidingGlassDraw1 = DrawsFactory.BuildGlassDraw(SlidingGlass);
            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);

            if (cabin.HasStep)
            {
                double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
                double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;
                StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
            }
           
            FixedGlassDraw1.LayerNo = 1;
            BarBaseLeft.LayerNo = 2;
            Door1WheelLeft.LayerNo = 2;
            Door1WheelRight.LayerNo = 2;
            Door1WheelLockLeft.LayerNo = 2;
            Door1WheelLockRight.LayerNo = 2;
            AngleConnector.LayerNo = 2;
            DoorStopper1.LayerNo = 2;
            StopperBumper1.LayerNo = 2;
            DoorStopper2.LayerNo = 2;
            StopperBumper2.LayerNo = 2;

            if (Handle1 is not null)
            {
                Handle1.LayerNo = 2;
            }
            
            Bar.LayerNo = 3;
            SlidingGlassDraw1.LayerNo = 4;

            if (MagnetStrip1 is not null)
            {
                MagnetStrip1.LayerNo = 4;
            }
        }

        protected override void PlaceParts()
        {
            double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
            double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;

            #region 1. WallFixing

            if (cabin.Parts.WallSideFixer is Profile profile)
            {
                WallProfile1.SetCenterOrStartX(0, CSCoordinate.Start);
                //If there is a correction the Profile will be further down , as the correction dictates
                WallProfile1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

                // At (EndX of Wall Al1 - ALST , StartY of Wall Al1 -- When the AL1 is 0Length do not calculate ALST
                FixedGlassDraw1.SetCenterOrStartX(
                    WallProfile1.GetBoundingBoxRectangle().EndX
                    - profile.GlassInProfileDepth, CSCoordinate.Start);
                FixedGlassDraw1.SetCenterOrStartY(
                    WallProfile1.GetBoundingBoxRectangle().StartY
                    , CSCoordinate.Start);

                if (StepWallProfile is not null)
                {
                    StepWallProfile.SetCenterOrStartX(stepLength, CSCoordinate.Start);
                    StepWallProfile.SetCenterOrStartY(
                        WallProfile1.GetBoundingBoxRectangle().EndY
                        , CSCoordinate.Start);
                }
            }
            else if (cabin.Parts.WallSideFixer is CabinSupport supportWall1)
            {
                FixedGlassDraw1.SetCenterOrStartX(supportWall1.GlassGapAER, CSCoordinate.Start);
                // If there is a correction the Fixed Glass will be a little further down as the correction dictates
                FixedGlassDraw1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

                WallSupportTop1.SetCenterOrStartX(
                    FixedGlassDraw1.GetBoundingBoxRectangle().StartX
                    - supportWall1.GlassGapAER
                    , CSCoordinate.Start);
                WallSupportBottom1.SetCenterOrStartX(
                    FixedGlassDraw1.GetBoundingBoxRectangle().StartX
                    - supportWall1.GlassGapAER
                    , CSCoordinate.Start);

                //Place the Supports on the Glass
                WallSupportTop1.SetCenterOrStartY(
                    FixedGlassDraw1.GetBoundingBoxRectangle().StartY
                    + GlassProcessesConstants.ProcessesInox304.SupportHoleTopDistanceVF
                    , CSCoordinate.Center);
                WallSupportBottom1.SetCenterOrStartY(
                    FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                    - GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVF
                    , CSCoordinate.Center);
            }

            #endregion

            #region 2. Fixed and Sliding Glass1
            double fixedGlassStartX = FixedGlassDraw1.GetBoundingBoxRectangle().StartX;
            double fixedGlassStartY = FixedGlassDraw1.GetBoundingBoxRectangle().StartY;
            double fixedGlassEndX = FixedGlassDraw1.GetBoundingBoxRectangle().EndX;
            double fixedGlassEndY = FixedGlassDraw1.GetBoundingBoxRectangle().EndY;

            //At (EndX of Fixed Glass - Overlap , StartY of FixedGlass)
            SlidingGlassDraw1.SetCenterOrStartX(
                fixedGlassEndX
                - cabin.Constraints.Overlap
                , CSCoordinate.Start);
            SlidingGlassDraw1.SetCenterOrStartY(
                fixedGlassStartY
                , CSCoordinate.Start);

            double slidingGlassStartX = SlidingGlassDraw1.GetBoundingBoxRectangle().StartX;
            double slidingGlassEndX = SlidingGlassDraw1.GetBoundingBoxRectangle().EndX;
            double slidingGlassStartY = SlidingGlassDraw1.GetBoundingBoxRectangle().StartY;

            #endregion

            #region 3.Bottom Fixing 1
            switch (cabin.Parts.BottomFixer)
            {
                case Profile floorProfile:
                    if (cabin.HasStep)
                    {
                        if (WallProfile1 is not null)
                        {
                            StepFloorProfile.SetCenterOrStartX(
                                WallProfile1.GetBoundingBoxRectangle().EndX
                                , CSCoordinate.Start);
                            StepFloorProfile.SetCenterOrStartY(
                                WallProfile1.GetBoundingBoxRectangle().EndY
                                - floorProfile.ThicknessView
                                , CSCoordinate.Start);

                            FloorProfile1.SetCenterOrStartX(
                                StepWallProfile.GetBoundingBoxRectangle().EndX
                                , CSCoordinate.Start);
                            FloorProfile1.SetCenterOrStartY(
                                StepWallProfile.GetBoundingBoxRectangle().EndY
                                - floorProfile.ThicknessView
                                , CSCoordinate.Start);
                        }
                        else //Even if it has Supports the Floor Profile is placed on all the Glass
                        {
                            StepFloorProfile.SetCenterOrStartX(
                                FixedGlassDraw1.GetBoundingBoxRectangle().StartX
                                , CSCoordinate.Start);
                            StepFloorProfile.SetCenterOrStartY(
                                FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                                - stepHeight
                                - floorProfile.ThicknessView
                                , CSCoordinate.Start);

                            FloorProfile1.SetCenterOrStartX(
                                FixedGlassDraw1.GetBoundingBoxRectangle().StartX
                                + stepLength
                                , CSCoordinate.Start);
                            FloorProfile1.SetCenterOrStartY(
                                FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                                - floorProfile.ThicknessView
                                , CSCoordinate.Start);
                        }
                    }
                    else
                    {
                        if (WallProfile1 is not null)
                        {
                            FloorProfile1.SetCenterOrStartX(
                                WallProfile1.GetBoundingBoxRectangle().EndX
                                , CSCoordinate.Start);
                            FloorProfile1.SetCenterOrStartY(
                                WallProfile1.GetBoundingBoxRectangle().EndY
                                - floorProfile.ThicknessView
                                , CSCoordinate.Start);
                        }
                        else
                        {
                            FloorProfile1.SetCenterOrStartX(
                                FixedGlassDraw1.GetBoundingBoxRectangle().StartX
                                , CSCoordinate.Start);
                            FloorProfile1.SetCenterOrStartY(
                                FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                                - floorProfile.ThicknessView
                                , CSCoordinate.Start);
                        }
                    }
                    break;
                case CabinSupport supportBottom1:
                    //At (EndX of Glass - (WidthOfStopper - OutOfGlassLength) , EndY of Glass - Height of Stopper)
                    GuiderBottom1.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndX
                        - supportBottom1.LengthView, CSCoordinate.Start);
                    GuiderBottom1.SetCenterOrStartY(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                        - supportBottom1.HeightView
                        , CSCoordinate.Start); ;
                    break;
                default:
                    //Do nothing
                    break;
            }
            #endregion

            #region 4.Step Wall Area

            if (StepWallArea != null)
            {
                StepWallArea.SetCenterOrStartX(0, CSCoordinate.Start);
                StepWallArea.SetCenterOrStartY(cabin.Height - stepHeight, CSCoordinate.Start);
            }

            #endregion

            #region 5.Various Small Parts

            //At (StartX of Glass , StartY of Glass - FixedLock Distance from Top)
            BarBaseLeft.SetCenterOrStartX(
                0, 
                CSCoordinate.Start);
            BarBaseLeft.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);
            
            //At (StartX of Glass , StartY of Glass - FixedLock Distance from Top)
            Bar.SetCenterOrStartX(
                cabin.LengthMin / 2d, 
                CSCoordinate.Center);
            Bar.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);

            //At (StartX of Glass + Distance from Left , StartY of Glass + Distance from Top)
            Fixed1LockLeft.SetCenterOrStartX(
                fixedGlassStartX 
                + GlassProcessesConstants.ProcessesInox304.BarHoleLeftDistanceVA, 
                CSCoordinate.Center);
            Fixed1LockLeft.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);

            //At (EndX of Glass - Distance from Right , StartY of Glass + Distance from Top)
            Fixed1LockRight.SetCenterOrStartX(
                fixedGlassEndX 
                - GlassProcessesConstants.ProcessesInox304.BarHoleRightDistanceVA, 
                CSCoordinate.Center);
            Fixed1LockRight.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);

            //At (StartX of Sliding Glass + Distance from Left , Start Y of Sliding Glass + Top Distance)
            Door1WheelLeft.SetCenterOrStartX(
                slidingGlassStartX 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleLeftDistanceVS, 
                CSCoordinate.Center);
            Door1WheelLeft.SetCenterOrStartY(
                slidingGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleTopDistanceVS, 
                CSCoordinate.Center);

            //At (EndX of Sliding Glass - Distance from Right , Start Y of Sliding Glass + Top Distance)
            Door1WheelRight.SetCenterOrStartX(
                slidingGlassEndX 
                - GlassProcessesConstants.ProcessesInox304.WheelHoleRightDistanceVS, 
                CSCoordinate.Center);
            Door1WheelRight.SetCenterOrStartY(
                slidingGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleTopDistanceVS, 
                CSCoordinate.Center);

            //At ( X of Left Wheel , Y of Left Wheel + In Between Distance)
            Door1WheelLockLeft.SetCenterOrStartX(
                Door1WheelLeft.ShapeCenterX, 
                CSCoordinate.Center);
            Door1WheelLockLeft.SetCenterOrStartY(
                Door1WheelLeft.ShapeCenterY 
                + GlassProcessesConstants.ProcessesInox304.WheelStopperBetweenDistanceVS, 
                CSCoordinate.Center);

            //At ( X of Right Wheel , Y of Right Wheel + In Between Distance)
            Door1WheelLockRight.SetCenterOrStartX(
                Door1WheelRight.ShapeCenterX, 
                CSCoordinate.Center);
            Door1WheelLockRight.SetCenterOrStartY(
                Door1WheelRight.ShapeCenterY 
                + GlassProcessesConstants.ProcessesInox304.WheelStopperBetweenDistanceVS, 
                CSCoordinate.Center);

            //At (EndX of Bar - Length of BarBase , CenterY of Bar)
            AngleConnector.SetCenterOrStartX(
                cabin.LengthMin 
                - AngleConnector.BoundingBoxLength, 
                CSCoordinate.Start);
            AngleConnector.SetCenterOrStartY(
                Bar.ShapeCenterY, 
                CSCoordinate.Center);

            //The Back Stopper must be Placed at the end position of the Door Opening
            //So at the StartX of WheelLeft - Opening of the Door (Opening/2 of Cabin)
            //At (StartX of Glass + Distance From Glass Start , EndY of Bar - Height of Stopper + Extra Small Length Below Bar)
            DoorStopper1.SetCenterOrStartX(
                Door1WheelLeft.GetBoundingBoxRectangle().StartX 
                - SingleDoorOpening 
                - DoorStopper1.BoundingBoxLength, 
                CSCoordinate.Start);
            DoorStopper1.SetCenterOrStartY(
                Bar.GetBoundingBoxRectangle().EndY
                - CabinPartsDimensions.Inox304Parts.DoorStopperHeight
                + CabinPartsDimensions.Inox304Parts.DoorStopperExtraLengthBelowBar, 
                CSCoordinate.Start);

            //THE BUMPER MUST BE OVERLAPPED BY THE DOOR ON OPENING!
            //At (EndX of Stopper , StartY of Stopper + Distance from top of it)
            StopperBumper1.SetCenterOrStartX(
                DoorStopper1.GetBoundingBoxRectangle().EndX, 
                CSCoordinate.Start);
            StopperBumper1.SetCenterOrStartY(
                DoorStopper1.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.Inox304Parts.BumperDistanceFromTopOfStopper, 
                CSCoordinate.Start);

            //At (EndY of WheelRight , EndY of Bar - Height of Stopper + Extra Small Length Below Bar)
            DoorStopper2.SetCenterOrStartX(
                Door1WheelRight.GetBoundingBoxRectangle().EndX, 
                CSCoordinate.Start);
            DoorStopper2.SetCenterOrStartY(
                Bar.GetBoundingBoxRectangle().EndY
                - CabinPartsDimensions.Inox304Parts.DoorStopperHeight
                + CabinPartsDimensions.Inox304Parts.DoorStopperExtraLengthBelowBar, CSCoordinate.Start);

            //At (EndX of Stopper , StartY of Stopper + Distance from top of it)
            StopperBumper2.SetCenterOrStartX(
                DoorStopper2.GetBoundingBoxRectangle().StartX 
                - StopperBumper2.BoundingBoxLength, 
                CSCoordinate.Start);
            StopperBumper2.SetCenterOrStartY(
                DoorStopper2.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.Inox304Parts.BumperDistanceFromTopOfStopper, 
                CSCoordinate.Start);

            if (Handle1 is not null)
            {
                //At (EndX of Sliding Glass - Distance from Edge , CenterY of Sliding Glass)
                Handle1.SetCenterOrStartX(
                    slidingGlassEndX
                    - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge(),
                    CSCoordinate.Center);
                Handle1.SetCenterOrStartY(
                    SlidingGlassDraw1.ShapeCenterY,
                    CSCoordinate.Center);
            }
            
            //At (EndX of Sliding Glass , StartY of Sliding Glass)
            MagnetStrip1.SetCenterOrStartX(
                slidingGlassEndX, 
                CSCoordinate.Start);
            MagnetStrip1.SetCenterOrStartY(
                slidingGlassStartY, 
                CSCoordinate.Start);

            #endregion

            //Rearrange Supports When Cabin Has Step
            if (cabin.HasStep && cabin.Parts.WallSideFixer is CabinSupport)
            {
                //if Step height is above threshold of not havin support ,
                //then move the support onto the step
                if (stepHeight > GlassProcessesConstants.ProcessesInox304.StepMaxHeightWithoutSupportHole)
                {
                    WallSupportBottom1.SetCenterOrStartX(
                        fixedGlassStartX + stepLength
                        , CSCoordinate.Start);
                }
                //If the step height is below the threshold then take the support and place it 50cm above it
                //only if the distance already is not above 50cm
                else if (stepHeight + GlassProcessesConstants.ProcessesInox304.SupportHoleMinDistanceFromStep > GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVA)
                {
                    WallSupportBottom1.SetCenterOrStartY(
                        fixedGlassEndY
                        - stepHeight
                        - GlassProcessesConstants.ProcessesInox304.SupportHoleMinDistanceFromStep,
                        CSCoordinate.Start);
                }
                //Else leave default positions
            }
        }

        protected override void PlaceDrawNames()
        {
            BarBaseLeft.Name = nameof(BarBaseLeft);
            AngleConnector.Name = nameof(AngleConnector);
            DoorStopper1.Name = nameof(DoorStopper1);
            DoorStopper2.Name = nameof(DoorStopper2);
            StopperBumper1.Name = nameof(StopperBumper1);
            StopperBumper2.Name = nameof(StopperBumper2);

            if (WallSupportTop1 is not null && WallSupportBottom1 is not null)
            {
                WallSupportTop1.Name = nameof(WallSupportTop1);
                WallSupportBottom1.Name = nameof(WallSupportBottom1);
            }
            if (WallProfile1 is not null)
            {
                WallProfile1.Name = nameof(WallProfile1);
                if (StepWallProfile is not null)
                {
                    StepWallProfile.Name = nameof(StepWallProfile);
                }
            }

            if (StepWallArea != null)
            {
                StepWallArea.Name = nameof(StepWallArea);
            }

            if (Handle1 is not null)
            {
                Handle1.Name = nameof(Handle1);
            }
            
            MagnetStrip1.Name = nameof(MagnetStrip1);
            Door1WheelLeft.Name = nameof(Door1WheelLeft);
            Door1WheelRight.Name = nameof(Door1WheelRight);
            Door1WheelLockLeft.Name = nameof(Door1WheelLockLeft);
            Door1WheelLockRight.Name = nameof(Door1WheelLockRight);
            Fixed1LockLeft.Name = nameof(Fixed1LockLeft);
            Fixed1LockRight.Name = nameof(Fixed1LockRight);
            if (GuiderBottom1 is not null)
            {
                GuiderBottom1.Name = nameof(GuiderBottom1);
            }
            if (FloorProfile1 is not null)
            {
                FloorProfile1.Name = nameof(FloorProfile1);
                if (StepFloorProfile is not null)
                {
                    StepFloorProfile.Name = nameof(StepFloorProfile);
                }
            }
            
            Bar.Name = nameof(Bar);
            SlidingGlassDraw1.Name = nameof(SlidingGlassDraw1);
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(BarBaseLeft);
            draws.Add(AngleConnector);
            draws.Add(DoorStopper1);
            draws.Add(DoorStopper2);
            draws.Add(StopperBumper1);
            draws.Add(StopperBumper2);
            draws.AddNotNull(WallSupportTop1);
            draws.AddNotNull(WallSupportBottom1);
            draws.AddNotNull(WallProfile1);
            draws.AddNotNull(StepWallProfile);
            draws.AddNotNull(StepWallArea);
            draws.AddNotNull(Handle1);
            draws.Add(MagnetStrip1);
            draws.Add(Door1WheelLeft);
            draws.Add(Door1WheelRight);
            draws.Add(Door1WheelLockLeft);
            draws.Add(Door1WheelLockRight);
            draws.Add(Fixed1LockLeft);
            draws.Add(Fixed1LockRight);
            draws.AddNotNull(GuiderBottom1);
            draws.AddNotNull(FloorProfile1);
            draws.AddNotNull(StepFloorProfile);
            draws.Add(Bar);
            draws.Add(SlidingGlassDraw1);
            draws.Add(FixedGlassDraw1);

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
            draws.Add(BarBaseLeft);
            draws.Add(AngleConnector);
            draws.Add(DoorStopper1);
            draws.Add(DoorStopper2);
            draws.AddNotNull(WallSupportTop1);
            draws.AddNotNull(WallSupportBottom1);
            draws.AddNotNull(WallProfile1);
            draws.AddNotNull(StepWallProfile);
            draws.AddNotNull(Handle1);
            draws.Add(Door1WheelLeft);
            draws.Add(Door1WheelRight);
            draws.Add(Door1WheelLockLeft);
            draws.Add(Door1WheelLockRight);
            draws.Add(Fixed1LockLeft);
            draws.Add(Fixed1LockRight);
            draws.AddNotNull(GuiderBottom1);
            draws.AddNotNull(FloorProfile1);
            draws.AddNotNull(StepFloorProfile);
            draws.Add(Bar);
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(MagnetStrip1);
            return draws;
        }

        public override List<DrawShape> GetHelperDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(StepWallArea);
            return draws;
        }
    }
}

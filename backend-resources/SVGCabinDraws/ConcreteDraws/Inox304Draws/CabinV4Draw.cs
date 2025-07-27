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
    public class CabinV4Draw : CabinDraw<CabinV4>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening / 2;
        public DrawShape WallSupportTop1 { get; set; }
        public DrawShape WallSupportBottom1 { get; set; }
        public DrawShape WallProfile1 { get; set; }
        public DrawShape StepWallProfile { get; set; }
        public DrawShape WallProfile2 { get; set; }
        public DrawShape WallSupportTop2 { get; set; }
        public DrawShape WallSupportBottom2 { get; set; }
        public DrawShape Bar { get; set; }
        public DrawShape BarBaseLeft { get; set; }
        public DrawShape BarBaseRight { get; set; }
        public DrawShape MagnetStrip1 { get; set; }
        public DrawShape MagnetStrip2 { get; set; }
        public DrawShape Handle1 { get; set; }
        public DrawShape Handle2 { get; set; }
        public DrawShape DoorStopper1 { get; set; }
        public DrawShape StopperBumper1 { get; set; }
        public DrawShape DoorStopper2 { get; set; }
        public DrawShape StopperBumper2 { get; set; }
        public DrawShape DoorStopper3 { get; set; }
        public DrawShape StopperBumper3 { get; set; }
        public DrawShape DoorStopper4 { get; set; }
        public DrawShape StopperBumper4 { get; set; }
        public DrawShape GuiderBottom1 { get; set; }
        public DrawShape GuiderBottom2 { get; set; }
        public DrawShape FloorProfile1 { get; set; }
        public DrawShape FloorProfile2 { get; set; }
        public DrawShape StepFloorProfile { get; set; }
        public DrawShape Door1WheelLeft { get; set; }
        public DrawShape Door1WheelRight { get; set; }
        public DrawShape Door1WheelLockLeft { get; set; }
        public DrawShape Door1WheelLockRight { get; set; }
        public DrawShape Door2WheelLeft { get; set; }
        public DrawShape Door2WheelRight { get; set; }
        public DrawShape Door2WheelLockLeft { get; set; }
        public DrawShape Door2WheelLockRight { get; set; }
        public DrawShape Fixed1LockLeft { get; set; }
        public DrawShape Fixed1LockRight { get; set; }
        public DrawShape Fixed2LockLeft { get; set; }
        public DrawShape Fixed2LockRight { get; set; }
        public DrawShape SlidingGlassDraw1 { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }
        public DrawShape SlidingGlassDraw2 { get; set; }
        public DrawShape FixedGlassDraw2 { get; set; }


        /// <summary>
        /// If there is Step
        /// </summary>
        public DrawShape StepWallArea { get; set; }

        private Glass SlidingGlass1 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.DoorGlass).FirstOrDefault(); }
        private Glass SlidingGlass2 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.DoorGlass).Skip(1).FirstOrDefault(); }
        private Glass FixedGlass1 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.FixedGlass).FirstOrDefault(); }
        private Glass FixedGlass2 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.FixedGlass).Skip(1).FirstOrDefault(); }

        public CabinV4Draw(CabinV4 cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            BarBaseLeft = DrawsFactory.Inox304Parts.BuildBarBase();
            BarBaseRight = DrawsFactory.Inox304Parts.BuildBarBase();

            DoorStopper1 = DrawsFactory.Inox304Parts.BuildDoorStopper();
            DoorStopper2 = DrawsFactory.Inox304Parts.BuildDoorStopper();
            DoorStopper3 = DrawsFactory.Inox304Parts.BuildDoorStopper();
            DoorStopper4 = DrawsFactory.Inox304Parts.BuildDoorStopper();
            StopperBumper1 = DrawsFactory.Inox304Parts.BuildStopperBumper();
            StopperBumper2 = DrawsFactory.Inox304Parts.BuildStopperBumper();
            StopperBumper3 = DrawsFactory.Inox304Parts.BuildStopperBumper();
            StopperBumper4 = DrawsFactory.Inox304Parts.BuildStopperBumper();

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

            if (cabin.Parts.WallFixer2 is CabinSupport wallSupport2)
            {
                WallSupportTop2 = DrawsFactory.HingeDraws.BuildGlassSupport(wallSupport2);
                WallSupportBottom2 = DrawsFactory.HingeDraws.BuildGlassSupport(wallSupport2);
            }
            else if (cabin.Parts.WallFixer2 is Profile wallProfile2)
            {
                WallProfile2 = DrawsFactory.BuildProfileDraw(wallProfile2.ThicknessView, wallProfile2.CutLength);
            }

            Handle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            Handle2 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            
            MagnetStrip1 = DrawsFactory.BuildStripDraw(
                cabin.Parts.CloseStrip.OutOfGlassLength, 
                cabin.Parts.CloseStrip.CutLength);
            MagnetStrip2 = DrawsFactory.BuildStripDraw(
                cabin.Parts.CloseStrip.OutOfGlassLength,
                cabin.Parts.CloseStrip.CutLength);

            Door1WheelLeft = DrawsFactory.Inox304Parts.BuildWheel();
            Door1WheelRight = DrawsFactory.Inox304Parts.BuildWheel();
            Door2WheelLeft = DrawsFactory.Inox304Parts.BuildWheel();
            Door2WheelRight = DrawsFactory.Inox304Parts.BuildWheel();

            Door1WheelLockLeft = DrawsFactory.Inox304Parts.BuildWheelLock();
            Door1WheelLockRight = DrawsFactory.Inox304Parts.BuildWheelLock();
            Door2WheelLockLeft = DrawsFactory.Inox304Parts.BuildWheelLock();
            Door2WheelLockRight = DrawsFactory.Inox304Parts.BuildWheelLock();

            Fixed1LockLeft = DrawsFactory.Inox304Parts.BuildFixedGlassLock();
            Fixed1LockRight = DrawsFactory.Inox304Parts.BuildFixedGlassLock();
            Fixed2LockLeft = DrawsFactory.Inox304Parts.BuildFixedGlassLock();
            Fixed2LockRight = DrawsFactory.Inox304Parts.BuildFixedGlassLock();

            if (cabin.Parts.BottomFixer1 is CabinSupport)
            {
                GuiderBottom1 = DrawsFactory.HingeDraws.BuildGlassSupport(cabin.Parts.BottomFixer1 as CabinSupport);
            }
            else if (cabin.Parts.BottomFixer1 is Profile floorProfile1)
            {
                FloorProfile1 = DrawsFactory.BuildProfileDraw(floorProfile1.CutLength, floorProfile1.ThicknessView);
                if (cabin.HasStep)
                {
                    StepFloorProfile = DrawsFactory.BuildProfileDraw(floorProfile1.CutLengthStepPart, floorProfile1.ThicknessView);
                }
            }

            if (cabin.Parts.BottomFixer2 is CabinSupport)
            {
                GuiderBottom2 = DrawsFactory.HingeDraws.BuildGlassSupport(cabin.Parts.BottomFixer2 as CabinSupport);
            }
            else if (cabin.Parts.BottomFixer2 is Profile floorProfile2)
            {
                FloorProfile2 = DrawsFactory.BuildProfileDraw(floorProfile2.CutLength, floorProfile2.ThicknessView);
            }

            Bar = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalBar.CutLength,
                cabin.Parts.HorizontalBar.ThicknessView);
            

            SlidingGlassDraw1 = DrawsFactory.BuildGlassDraw(SlidingGlass1);
            

            SlidingGlassDraw2 = DrawsFactory.BuildGlassDraw(SlidingGlass2);

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass1);
            

            FixedGlassDraw2 = DrawsFactory.BuildGlassDraw(FixedGlass2);

            if (cabin.HasStep)
            {
                double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
                double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;
                StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
            }
            FixedGlassDraw1.LayerNo = 1;
            FixedGlassDraw2.LayerNo = 1;
            Door1WheelLeft.LayerNo = 2;
            Door1WheelRight.LayerNo = 2;
            Door1WheelLockLeft.LayerNo = 2;
            Door1WheelLockRight.LayerNo = 2;
            Door2WheelLeft.LayerNo = 2;
            Door2WheelRight.LayerNo = 2;
            Door2WheelLockLeft.LayerNo = 2;
            Door2WheelLockRight.LayerNo = 2;
            DoorStopper1.LayerNo = 2;
            StopperBumper1.LayerNo = 2;
            DoorStopper2.LayerNo = 2;
            StopperBumper2.LayerNo = 2;
            DoorStopper3.LayerNo = 2;
            StopperBumper3.LayerNo = 2;
            DoorStopper4.LayerNo = 2;
            StopperBumper4.LayerNo = 2;
            Handle1.LayerNo = 2;
            Handle2.LayerNo = 2;
            BarBaseRight.LayerNo = 2;
            BarBaseLeft.LayerNo = 2;
            Bar.LayerNo = 3;
            SlidingGlassDraw1.LayerNo = 4;
            SlidingGlassDraw2.LayerNo = 4;
            MagnetStrip1.LayerNo = 4;
            MagnetStrip2.LayerNo = 4;
        }

        protected override void PlaceParts()
        {
            double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
            double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;

            #region 1.Wall Fixing 1
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
            switch (cabin.Parts.BottomFixer1)
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

            #region 5.Various Small Parts First Half

            //At (StartX of Glass , StartY of Glass - FixedLock Distance from Top)
            BarBaseLeft.SetCenterOrStartX(
                0
                , CSCoordinate.Start);
            BarBaseLeft.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA
                , CSCoordinate.Center);

            //At (StartX of Glass , StartY of Glass - FixedLock Distance from Top)
            Bar.SetCenterOrStartX(
                cabin.LengthMin/2d
                , CSCoordinate.Center);
            Bar.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA
                , CSCoordinate.Center);

            //At (StartX of Glass + Distance from Left , StartY of Glass + Distance from Top)
            Fixed1LockLeft.SetCenterOrStartX(
                fixedGlassStartX 
                + GlassProcessesConstants.ProcessesInox304.BarHoleLeftDistanceVA
                , CSCoordinate.Center);
            Fixed1LockLeft.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA
                , CSCoordinate.Center);

            //At (EndX of Glass - Distance from Right , StartY of Glass + Distance from Top)
            Fixed1LockRight.SetCenterOrStartX(
                fixedGlassEndX 
                - GlassProcessesConstants.ProcessesInox304.BarHoleRightDistanceVA
                , CSCoordinate.Center);
            Fixed1LockRight.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA
                , CSCoordinate.Center);

            //At (StartX of Sliding Glass + Distance from Left , Start Y of Sliding Glass + Top Distance)
            Door1WheelLeft.SetCenterOrStartX(
                slidingGlassStartX 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleLeftDistanceVS
                , CSCoordinate.Center);
            Door1WheelLeft.SetCenterOrStartY(
                slidingGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleTopDistanceVS
                , CSCoordinate.Center);

            //At (EndX of Sliding Glass - Distance from Right , Start Y of Sliding Glass + Top Distance)
            Door1WheelRight.SetCenterOrStartX(
                slidingGlassEndX 
                - GlassProcessesConstants.ProcessesInox304.WheelHoleRightDistanceVS
                , CSCoordinate.Center);
            Door1WheelRight.SetCenterOrStartY(
                slidingGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleTopDistanceVS
                , CSCoordinate.Center);

            //At ( X of Left Wheel , Y of Left Wheel + In Between Distance)
            Door1WheelLockLeft.SetCenterOrStartX(
                Door1WheelLeft.ShapeCenterX
                , CSCoordinate.Center);
            Door1WheelLockLeft.SetCenterOrStartY(
                Door1WheelLeft.ShapeCenterY 
                + GlassProcessesConstants.ProcessesInox304.WheelStopperBetweenDistanceVS
                , CSCoordinate.Center);

            //At ( X of Right Wheel , Y of Right Wheel + In Between Distance)
            Door1WheelLockRight.SetCenterOrStartX(
                Door1WheelRight.ShapeCenterX
                , CSCoordinate.Center);
            Door1WheelLockRight.SetCenterOrStartY(
                Door1WheelRight.ShapeCenterY 
                + GlassProcessesConstants.ProcessesInox304.WheelStopperBetweenDistanceVS
                , CSCoordinate.Center);

            //The Back Stopper must be Placed at the end position of the Door Opening
            //So at the StartX of WheelLeft - Opening of the Door (Opening/2 of Cabin)
            //At (StartX of Glass + Distance From Glass Start , EndY of Bar - Height of Stopper + Extra Small Length Below Bar)
            DoorStopper1.SetCenterOrStartX(
                Door1WheelLeft.GetBoundingBoxRectangle().StartX 
                - SingleDoorOpening 
                - DoorStopper1.BoundingBoxLength
                , CSCoordinate.Start);
            DoorStopper1.SetCenterOrStartY(
                Bar.GetBoundingBoxRectangle().EndY
                - CabinPartsDimensions.Inox304Parts.DoorStopperHeight
                + CabinPartsDimensions.Inox304Parts.DoorStopperExtraLengthBelowBar
                , CSCoordinate.Start);

            //THE BUMPER MUST BE OVERLAPPED BY THE DOOR ON OPENING!
            //At (EndX of Stopper , StartY of Stopper + Distance from top of it)
            StopperBumper1.SetCenterOrStartX(
                DoorStopper1.GetBoundingBoxRectangle().EndX
                , CSCoordinate.Start);
            StopperBumper1.SetCenterOrStartY(
                DoorStopper1.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.Inox304Parts.BumperDistanceFromTopOfStopper
                , CSCoordinate.Start);

            //At (EndY of WheelRight , EndY of Bar - Height of Stopper + Extra Small Length Below Bar)
            DoorStopper2.SetCenterOrStartX(
                Door1WheelRight.GetBoundingBoxRectangle().EndX
                , CSCoordinate.Start);
            DoorStopper2.SetCenterOrStartY(
                Bar.GetBoundingBoxRectangle().EndY
                - CabinPartsDimensions.Inox304Parts.DoorStopperHeight
                + CabinPartsDimensions.Inox304Parts.DoorStopperExtraLengthBelowBar
                , CSCoordinate.Start);

            //At (Start of Stopper2 , StartY of Stopper2 + Distance from top of it)
            StopperBumper2.SetCenterOrStartX(
                DoorStopper2.GetBoundingBoxRectangle().StartX 
                - StopperBumper2.BoundingBoxLength
                , CSCoordinate.Start);
            StopperBumper2.SetCenterOrStartY(
                DoorStopper2.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.Inox304Parts.BumperDistanceFromTopOfStopper
                , CSCoordinate.Start);

            //At (EndX of Sliding Glass - Distance from Edge , CenterY of Sliding Glass)
            Handle1.SetCenterOrStartX(
                slidingGlassEndX 
                - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge()
                , CSCoordinate.Center);
            Handle1.SetCenterOrStartY(
                SlidingGlassDraw1.ShapeCenterY
                , CSCoordinate.Center);

            //At (EndX of Sliding Glass , StartY of Sliding Glass)
            MagnetStrip1.SetCenterOrStartX(
                slidingGlassEndX, 
                CSCoordinate.Start);
            MagnetStrip1.SetCenterOrStartY(
                slidingGlassStartY, 
                CSCoordinate.Start);

            //At (EndX of Strip1 , StartY of Strip1)
            MagnetStrip2.SetCenterOrStartX(
                MagnetStrip1.GetBoundingBoxRectangle().EndX,
                CSCoordinate.Start);
            MagnetStrip2.SetCenterOrStartY(
                MagnetStrip1.GetBoundingBoxRectangle().StartY, 
                CSCoordinate.Start);

            #endregion

            #region 6.Fixed and Sliding Glass2
            //At (EndX of Strip2 , StartY of Strip2)
            SlidingGlassDraw2.SetCenterOrStartX(
                MagnetStrip2.GetBoundingBoxRectangle().EndX,
                CSCoordinate.Start);
            SlidingGlassDraw2.SetCenterOrStartY(
                MagnetStrip2.GetBoundingBoxRectangle().StartY,
                CSCoordinate.Start);

            double slidingGlass2StartX = SlidingGlassDraw2.GetBoundingBoxRectangle().StartX;
            double slidingGlass2EndX = SlidingGlassDraw2.GetBoundingBoxRectangle().EndX;
            double slidingGlass2StartY = SlidingGlassDraw2.GetBoundingBoxRectangle().StartY;
            double slidingGlass2EndY = SlidingGlassDraw2.GetBoundingBoxRectangle().EndY;

            //At (EndX of Bar - GlassLength , Y = 0)
            FixedGlassDraw2.SetCenterOrStartX(
                slidingGlass2EndX 
                - cabin.Constraints.Overlap,
                CSCoordinate.Start);
            FixedGlassDraw2.SetCenterOrStartY(0, CSCoordinate.Start);

            double fixedGlass2StartX = FixedGlassDraw2.GetBoundingBoxRectangle().StartX;
            double fixedGlass2StartY = FixedGlassDraw2.GetBoundingBoxRectangle().StartY;
            double fixedGlass2EndX = FixedGlassDraw2.GetBoundingBoxRectangle().EndX;
            double fixedGlass2EndY = FixedGlassDraw2.GetBoundingBoxRectangle().EndY;

            #endregion

            #region 7.Various Small Parts Second Half

            //At (StartX of Sliding Glass2 + Distance from Edge , CenterY of Sliding Glass2)
            Handle2.SetCenterOrStartX(
                slidingGlass2StartX 
                + cabin.Parts.Handle.GetHandleCenterDistanceFromEdge(),
                CSCoordinate.Center);
            Handle2.SetCenterOrStartY(
                SlidingGlassDraw2.ShapeCenterY, 
                CSCoordinate.Center);

            //At (StartX of Sliding Glass2 + Distance from Left , Start Y of Sliding Glass2 + Top Distance)
            Door2WheelLeft.SetCenterOrStartX(
                slidingGlass2StartX 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleLeftDistanceVS,
                CSCoordinate.Center);
            Door2WheelLeft.SetCenterOrStartY(
                slidingGlass2StartY 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleTopDistanceVS,
                CSCoordinate.Center);

            //At (EndX of Sliding Glass2 - Distance from Right , Start Y of Sliding Glass2 + Top Distance)
            Door2WheelRight.SetCenterOrStartX(
                slidingGlass2EndX 
                - GlassProcessesConstants.ProcessesInox304.WheelHoleRightDistanceVS,
                CSCoordinate.Center);
            Door2WheelRight.SetCenterOrStartY(
                slidingGlass2StartY 
                + GlassProcessesConstants.ProcessesInox304.WheelHoleTopDistanceVS,
                CSCoordinate.Center);

            //At ( X of Left Wheel 2 , Y of Left Wheel 2 + In Between Distance)
            Door2WheelLockLeft.SetCenterOrStartX(
                Door2WheelLeft.ShapeCenterX, 
                CSCoordinate.Center);
            Door2WheelLockLeft.SetCenterOrStartY(
                Door2WheelLeft.ShapeCenterY 
                + GlassProcessesConstants.ProcessesInox304.WheelStopperBetweenDistanceVS,
                CSCoordinate.Center);

            //At ( X of Right Wheel2 , Y of Right Wheel2 + In Between Distance)
            Door2WheelLockRight.SetCenterOrStartX(
                Door2WheelRight.ShapeCenterX,
                CSCoordinate.Center);
            Door2WheelLockRight.SetCenterOrStartY(
                Door2WheelRight.ShapeCenterY 
                + GlassProcessesConstants.ProcessesInox304.WheelStopperBetweenDistanceVS,
                CSCoordinate.Center);

            //The Front Stopper must be Placed at the Start position of the Door Opening
            //So at the StartX of WheelLeft - Length of Stopper
            //At (StartX of Wheel - Length of Stopper , EndY of Bar - Height of Stopper + Extra Small Length Below Bar)
            DoorStopper3.SetCenterOrStartX(
                Door2WheelLeft.GetBoundingBoxRectangle().StartX 
                - DoorStopper3.BoundingBoxLength, 
                CSCoordinate.Start);
            DoorStopper3.SetCenterOrStartY(
                Bar.GetBoundingBoxRectangle().EndY
                - CabinPartsDimensions.Inox304Parts.DoorStopperHeight
                + CabinPartsDimensions.Inox304Parts.DoorStopperExtraLengthBelowBar,
                CSCoordinate.Start);

            //THE BUMPER MUST BE OVERLAPPED BY THE DOOR ON OPENING!
            //At (EndX of Stopper3 , StartY of Stopper3 + Distance from top of it)
            StopperBumper3.SetCenterOrStartX(
                DoorStopper3.GetBoundingBoxRectangle().EndX,
                CSCoordinate.Start);
            StopperBumper3.SetCenterOrStartY(
                DoorStopper3.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.Inox304Parts.BumperDistanceFromTopOfStopper, 
                CSCoordinate.Start);

            //At (EndY of WheelRight + DoorOpening , EndY of Bar - Height of Stopper + Extra Small Length Below Bar)
            DoorStopper4.SetCenterOrStartX(
                Door2WheelRight.GetBoundingBoxRectangle().EndX 
                + SingleDoorOpening, 
                CSCoordinate.Start);
            DoorStopper4.SetCenterOrStartY(
                Bar.GetBoundingBoxRectangle().EndY
                - CabinPartsDimensions.Inox304Parts.DoorStopperHeight
                + CabinPartsDimensions.Inox304Parts.DoorStopperExtraLengthBelowBar,
                CSCoordinate.Start);

            //At (Start of Stopper4 , StartY of Stopper4 + Distance from top of it)
            StopperBumper4.SetCenterOrStartX(
                DoorStopper4.GetBoundingBoxRectangle().StartX 
                - StopperBumper4.BoundingBoxLength, 
                CSCoordinate.Start);
            StopperBumper4.SetCenterOrStartY(
                DoorStopper4.GetBoundingBoxRectangle().StartY 
                + CabinPartsDimensions.Inox304Parts.BumperDistanceFromTopOfStopper,
                CSCoordinate.Start);

            //At (StartX of Glass2 + Distance from Left , StartY of Glass2 + Distance from Top)
            Fixed2LockLeft.SetCenterOrStartX(
                fixedGlass2StartX 
                + GlassProcessesConstants.ProcessesInox304.BarHoleLeftDistanceVA, 
                CSCoordinate.Center);
            Fixed2LockLeft.SetCenterOrStartY(
                fixedGlass2StartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);

            //At (EndX of Glass2 - Distance from Right , StartY of Glass2 + Distance from Top)
            Fixed2LockRight.SetCenterOrStartX(
                fixedGlass2EndX 
                - GlassProcessesConstants.ProcessesInox304.BarHoleRightDistanceVA, 
                CSCoordinate.Center);
            Fixed2LockRight.SetCenterOrStartY(
                fixedGlass2StartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);

            //At (EndX of Bar - Length , StartY of Glass - FixedLock Distance from Top)
            BarBaseRight.SetCenterOrStartX(
                cabin.LengthMin 
                - BarBaseRight.BoundingBoxLength, 
                CSCoordinate.Start);
            BarBaseRight.SetCenterOrStartY(
                fixedGlass2StartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVA, 
                CSCoordinate.Center);

            #endregion

            #region 8.Bottom Fixing 2

            switch (cabin.Parts.BottomFixer2)
            {
                case Profile floorProfile2:
                    FloorProfile2.SetCenterOrStartX(
                                fixedGlass2StartX
                                , CSCoordinate.Start);
                    FloorProfile2.SetCenterOrStartY(
                        fixedGlass2EndY
                        - floorProfile2.ThicknessView
                        , CSCoordinate.Start);
                    break;
                case CabinSupport supportBottom2:
                    GuiderBottom2.SetCenterOrStartX(
                        fixedGlass2StartX
                        , CSCoordinate.Start);
                    GuiderBottom2.SetCenterOrStartY(
                        fixedGlass2EndY
                        - supportBottom2.HeightView
                        , CSCoordinate.Start);
                    break;
                default:
                    break;
            }

            #endregion

            #region 9. Wall Fixing 2

            switch (cabin.Parts.WallFixer2)
            {
                case Profile wallProfile2:
                    WallProfile2.SetCenterOrStartX(
                        fixedGlass2EndX
                        - wallProfile2.GlassInProfileDepth,
                        CSCoordinate.Start);
                    //If there is a correction the Profile will be further down , as the correction dictates
                    WallProfile2.SetCenterOrStartY(
                        fixedGlass2StartY,
                        CSCoordinate.Start);
                    break;
                case CabinSupport supportWall2:
                    
                    WallSupportTop2.SetCenterOrStartX(
                        fixedGlass2EndX
                        - (supportWall2.LengthView - supportWall2.GlassGapAER),
                        CSCoordinate.Start);
                    WallSupportBottom2.SetCenterOrStartX(
                        fixedGlass2EndX
                        - (supportWall2.LengthView - supportWall2.GlassGapAER),
                        CSCoordinate.Start);

                    //Place the Supports on the Glass
                    WallSupportTop2.SetCenterOrStartY(
                        fixedGlassStartY
                        + GlassProcessesConstants.ProcessesInox304.SupportHoleTopDistanceVF,
                        CSCoordinate.Center);
                    WallSupportBottom2.SetCenterOrStartY(
                        fixedGlass2EndY
                        - GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVF,
                        CSCoordinate.Center);
                    break;
                default:
                    break;
            }

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
                else if(stepHeight + GlassProcessesConstants.ProcessesInox304.SupportHoleMinDistanceFromStep > GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVA)
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
            BarBaseRight.Name = nameof(BarBaseRight);

            DoorStopper1.Name = nameof(DoorStopper1);
            DoorStopper2.Name = nameof(DoorStopper2);
            DoorStopper3.Name = nameof(DoorStopper3);
            DoorStopper4.Name = nameof(DoorStopper4);
            StopperBumper1.Name = nameof(StopperBumper1);
            StopperBumper2.Name = nameof(StopperBumper2);
            StopperBumper3.Name = nameof(StopperBumper3);
            StopperBumper4.Name = nameof(StopperBumper4);

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
            if (WallProfile2 is not null)
            {
                WallProfile2.Name = nameof(WallProfile2);
            }

            if (WallSupportTop2 is not null && WallSupportBottom2 is not null)
            {
                WallSupportTop2.Name = nameof(WallSupportTop2);
                WallSupportBottom2.Name = nameof(WallSupportBottom2);
            }
            if (StepWallArea != null)
            {
                StepWallArea.Name = nameof(StepWallArea);
            }

            Handle1.Name = nameof(Handle1);
            Handle2.Name = nameof(Handle2);
            MagnetStrip1.Name = nameof(MagnetStrip1);
            MagnetStrip2.Name = nameof(MagnetStrip2);

            Door1WheelLeft.Name = nameof(Door1WheelLeft);
            Door1WheelRight.Name = nameof(Door1WheelRight);
            Door2WheelLeft.Name = nameof(Door2WheelLeft);
            Door2WheelRight.Name = nameof(Door2WheelRight);

            Door1WheelLockLeft.Name = nameof(Door1WheelLockLeft);
            Door1WheelLockRight.Name = nameof(Door1WheelLockRight);
            Door2WheelLockLeft.Name = nameof(Door2WheelLockLeft);
            Door2WheelLockRight.Name = nameof(Door2WheelLockRight);

            Fixed1LockLeft.Name = nameof(Fixed1LockLeft);
            Fixed1LockRight.Name = nameof(Fixed1LockRight);
            Fixed2LockLeft.Name = nameof(Fixed2LockLeft);
            Fixed2LockRight.Name = nameof(Fixed2LockRight);

            if (GuiderBottom1 is not null)
            {
                GuiderBottom1.Name = nameof(GuiderBottom1);
            }
            if (GuiderBottom2 is not null)
            {
                GuiderBottom2.Name = nameof(GuiderBottom2);
            }
            if (FloorProfile1 is not null)
            {
                FloorProfile1.Name = nameof(FloorProfile1);
                if (StepFloorProfile is not null)
                {
                    StepFloorProfile.Name = nameof(StepFloorProfile);
                }
            }
            if (FloorProfile2 is not null)
            {
                FloorProfile2.Name = nameof(FloorProfile2);
            }

            Bar.Name = nameof(Bar);

            SlidingGlassDraw1.Name = nameof(SlidingGlassDraw1);
            SlidingGlassDraw2.Name = nameof(SlidingGlassDraw2);
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
            FixedGlassDraw2.Name = nameof(FixedGlassDraw2);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(BarBaseLeft);
            draws.Add(BarBaseRight);
            draws.Add(DoorStopper1);
            draws.Add(DoorStopper2);
            draws.Add(DoorStopper3);
            draws.Add(DoorStopper4);
            draws.Add(StopperBumper1);
            draws.Add(StopperBumper2);
            draws.Add(StopperBumper3);
            draws.Add(StopperBumper4);
            draws.AddNotNull(WallSupportTop1);
            draws.AddNotNull(WallSupportBottom1);
            draws.AddNotNull(WallProfile1);
            draws.AddNotNull(StepWallProfile);
            draws.AddNotNull(WallProfile2);
            draws.AddNotNull(StepWallArea);
            draws.AddNotNull(WallSupportTop2);
            draws.AddNotNull(WallSupportBottom2);
            draws.Add(Handle1);
            draws.Add(Handle2);
            draws.Add(MagnetStrip1);
            draws.Add(MagnetStrip2);
            draws.Add(Door1WheelLeft);
            draws.Add(Door1WheelRight);
            draws.Add(Door2WheelLeft);
            draws.Add(Door2WheelRight);
            draws.Add(Door1WheelLockLeft);
            draws.Add(Door1WheelLockRight);
            draws.Add(Door2WheelLockLeft);
            draws.Add(Door2WheelLockRight);
            draws.Add(Fixed1LockLeft);
            draws.Add(Fixed1LockRight);
            draws.Add(Fixed2LockLeft);
            draws.Add(Fixed2LockRight);
            draws.AddNotNull(GuiderBottom1);
            draws.AddNotNull(GuiderBottom2);
            draws.AddNotNull(FloorProfile1);
            draws.AddNotNull(StepFloorProfile);
            draws.AddNotNull(FloorProfile2);
            draws.Add(Bar);
            draws.Add(SlidingGlassDraw1);
            draws.Add(SlidingGlassDraw2);
            draws.Add(FixedGlassDraw1);
            draws.Add(FixedGlassDraw2);

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
            draws.Add(SlidingGlassDraw2);
            draws.Add(FixedGlassDraw2);
            return draws;
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(BarBaseLeft);
            draws.Add(BarBaseRight);
            draws.Add(DoorStopper1);
            draws.Add(DoorStopper2);
            draws.Add(DoorStopper3);
            draws.Add(DoorStopper4);
            draws.AddNotNull(WallSupportTop1);
            draws.AddNotNull(WallSupportBottom1);
            draws.AddNotNull(WallProfile1);
            draws.AddNotNull(StepWallProfile);
            draws.AddNotNull(WallProfile2);
            draws.AddNotNull(WallSupportTop2);
            draws.AddNotNull(WallSupportBottom2);
            draws.Add(Handle1);
            draws.Add(Handle2);
            draws.Add(Door1WheelLeft);
            draws.Add(Door1WheelRight);
            draws.Add(Door2WheelLeft);
            draws.Add(Door2WheelRight);
            draws.Add(Door1WheelLockLeft);
            draws.Add(Door1WheelLockRight);
            draws.Add(Door2WheelLockLeft);
            draws.Add(Door2WheelLockRight);
            draws.Add(Fixed1LockLeft);
            draws.Add(Fixed1LockRight);
            draws.Add(Fixed2LockLeft);
            draws.Add(Fixed2LockRight);
            draws.AddNotNull(GuiderBottom1);
            draws.AddNotNull(GuiderBottom2);
            draws.AddNotNull(FloorProfile1);
            draws.AddNotNull(StepFloorProfile);
            draws.AddNotNull(FloorProfile2);
            draws.Add(Bar);
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(MagnetStrip1);
            draws.Add(MagnetStrip2);
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

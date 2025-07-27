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
    public class CabinVFDraw : CabinDraw<CabinVF>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => 0;

        public DrawShape WallSupportTop1 { get; set; }
        public DrawShape WallSupportBottom1 { get; set; }
        public DrawShape WallProfile1 { get; set; }
        public DrawShape StepWallProfile { get; set; }
        public DrawShape SideProfile { get; set; }
        public DrawShape SideSupportTop { get; set; }
        public DrawShape SideSupportBottom { get; set; }

        public DrawShape BarSupport { get; set; }
        public DrawShape FloorStopper { get; set; }
        public DrawShape FloorProfile1 { get; set; }
        public DrawShape StepFloorProfile { get; set; }

        public DrawShape FixedGlassDraw1 { get; set; }

        /// <summary>
        /// If there is Step
        /// </summary>
        public DrawShape StepWallArea { get; set; }

        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }

        public CabinVFDraw(CabinVF cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);
            
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

            if (cabin.Parts.BottomFixer is CabinSupport bottomSupport)
            {
                FloorStopper = DrawsFactory.HingeDraws.BuildGlassSupport(bottomSupport);
            }
            else if (cabin.Parts.BottomFixer is Profile floorProfile1)
            {
                FloorProfile1 = DrawsFactory.BuildProfileDraw(floorProfile1.CutLength, floorProfile1.ThicknessView);
                if (cabin.HasStep)
                {
                    StepFloorProfile = DrawsFactory.BuildProfileDraw(floorProfile1.CutLengthStepPart, floorProfile1.ThicknessView);
                }
            }

            BarSupport = DrawsFactory.Inox304Parts.BuildBarBaseFixedGlassSupport();

            if (cabin.Parts.SideFixer is CabinSupport sideSupport)
            {
                SideSupportTop = DrawsFactory.HingeDraws.BuildGlassSupport(sideSupport);
                SideSupportBottom = DrawsFactory.HingeDraws.BuildGlassSupport(sideSupport);
            }
            else if (cabin.Parts.SideFixer is Profile sideProfile)
            {
                SideProfile = DrawsFactory.BuildProfileDraw(sideProfile.ThicknessView, sideProfile.CutLength);
            }

            if (cabin.HasStep)
            {
                double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
                double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;
                StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
            }
                        
            FixedGlassDraw1.LayerNo = 1;
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

            #region 2.Fixed Glass Coordinated

            double fixedGlassStartX = FixedGlassDraw1.GetBoundingBoxRectangle().StartX;
            double fixedGlassStartY = FixedGlassDraw1.GetBoundingBoxRectangle().StartY;
            double fixedGlassEndX = FixedGlassDraw1.GetBoundingBoxRectangle().EndX;
            double fixedGlassEndY = FixedGlassDraw1.GetBoundingBoxRectangle().EndY;

            #endregion

            #region 3.Bottom Fixing
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
                case FloorStopperW floorStopperW:
                    //At (EndX of Glass - (WidthOfStopper - OutOfGlassLength) , EndY of Glass - Height of Stopper)
                    FloorStopper.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndX
                        - floorStopperW.LengthView
                        + floorStopperW.OutOfGlassLength, 
                        CSCoordinate.Start);
                    FloorStopper.SetCenterOrStartY(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                        - floorStopperW.HeightView
                        , CSCoordinate.Start); ;
                    break;
                case CabinSupport supportBottom1:
                    //At (EndX of Glass - (WidthOfStopper - OutOfGlassLength) , EndY of Glass - Height of Stopper)
                    FloorStopper.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndX
                        - supportBottom1.LengthView, CSCoordinate.Start);
                    FloorStopper.SetCenterOrStartY(
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

            #region 5.Side Fixing

            if (cabin.Parts.SideFixer is Profile sideProfile)
            {
                SideProfile.SetCenterOrStartX(
                    fixedGlassEndX
                    -sideProfile.GlassInProfileDepth, 
                    CSCoordinate.Start);
                //If there is a correction the Profile will be further down , as the correction dictates
                SideProfile.SetCenterOrStartY(
                    cabin.Constraints.FinalHeightCorrection, 
                    CSCoordinate.Start);
            }
            else if (cabin.Parts.SideFixer is CabinSupport sideSupport)
            {
                SideSupportTop.SetCenterOrStartX(
                    fixedGlassEndX
                    - sideSupport.LengthView
                    + sideSupport.GlassGapAER
                    , CSCoordinate.Start);
                SideSupportBottom.SetCenterOrStartX(
                    fixedGlassEndX
                    - sideSupport.LengthView
                    + sideSupport.GlassGapAER
                    , CSCoordinate.Start);

                //Place the Supports on the Glass
                SideSupportTop.SetCenterOrStartY(
                    fixedGlassStartY
                    + GlassProcessesConstants.ProcessesInox304.SupportHoleTopDistanceVF
                    , CSCoordinate.Center);
                SideSupportBottom.SetCenterOrStartY(
                    fixedGlassEndY
                    - GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVF
                    , CSCoordinate.Center);
            }

            #endregion


            BarSupport.SetCenterOrStartX(
                fixedGlassEndX 
                - GlassProcessesConstants.ProcessesInox304.BarHoleRightDistanceVF, 
                CSCoordinate.Center);
            BarSupport.SetCenterOrStartY(
                fixedGlassStartY 
                + GlassProcessesConstants.ProcessesInox304.BarHoleTopDistanceVF, 
                CSCoordinate.Center);

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
            if (WallSupportBottom1 is not null && WallSupportTop1 is not null)
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

            if (SideProfile is not null)
            {
                SideProfile.Name = nameof(SideProfile);
            }
            if (SideSupportBottom is not null && SideSupportTop is not null)
            {
                SideSupportTop.Name = nameof(SideSupportTop);
                SideSupportBottom.Name = nameof(SideSupportBottom);
            }

            BarSupport.Name = nameof(BarSupport);

            if (FloorStopper is not null)
            {
                FloorStopper.Name = nameof(FloorStopper);
            }
            if (FloorProfile1 is not null)
            {
                FloorProfile1.Name = nameof(FloorProfile1);
                if (StepFloorProfile is not null)
                {
                    StepFloorProfile.Name = nameof(StepFloorProfile);
                }
            }
            
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);

            if (StepWallArea != null)
            {
                StepWallArea.Name = nameof(StepWallArea);
            }
        }


        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(WallSupportTop1);
            draws.AddNotNull(WallSupportBottom1);
            draws.AddNotNull(WallProfile1);
            draws.AddNotNull(StepWallProfile);
            draws.AddNotNull(StepWallArea);
            draws.Add(BarSupport);
            draws.AddNotNull(FloorStopper);
            draws.AddNotNull(FloorProfile1);
            draws.AddNotNull(StepFloorProfile);
            draws.AddNotNull(SideProfile);
            draws.AddNotNull(SideSupportBottom);
            draws.AddNotNull(SideSupportTop);
            draws.Add(FixedGlassDraw1);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(FixedGlassDraw1);
            return draws;
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(WallSupportTop1);
            draws.AddNotNull(WallSupportBottom1);
            draws.AddNotNull(WallProfile1);
            draws.AddNotNull(StepWallProfile);
            draws.Add(BarSupport);
            draws.AddNotNull(FloorStopper);
            draws.AddNotNull(FloorProfile1);
            draws.AddNotNull(StepFloorProfile);
            draws.AddNotNull(SideProfile);
            draws.AddNotNull(SideSupportBottom);
            draws.AddNotNull(SideSupportTop);
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            return new();
        }

        public override List<DrawShape> GetHelperDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(StepWallArea);
            return draws;
        }

    }
}

using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
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
    public class CabinWDraw : CabinDraw<CabinW>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => 0;

        public DrawShape WallAluminium1 { get; set; }
        public DrawShape GlassToGlassSupportTop { get; set; }
        public DrawShape GlassToGlassSupportBottom { get; set; }
        public DrawShape TopProfile { get; set; }
        public DrawShape SideProfile { get; set; }

        public DrawShape GlassToGlassSupportSideTop { get; set; }
        public DrawShape GlassToGlassSupportSideBottom { get; set; }


        public DrawShape FloorAluminium { get; set; }
        public DrawShape GlassAluminium1 { get; set; }
        public DrawShape SupportBar { get; set; }
        public DrawShape FloorStopper { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }
        public DrawShape MagnetStrip { get; set; }

        /// <summary>
        /// The Step-Part of the Wall Aluminium
        /// </summary>
        public DrawShape StepWallAluminium1 { get; set; }
        public DrawShape StepFloorAluminium1 { get; set; }
        public DrawShape StepWallArea { get; set; }

        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }

        public CabinWDraw(CabinW cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
            double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;

            switch (cabin.Parts.WallSideFixer)
            {
                case Profile profile:
                    WallAluminium1 = DrawsFactory.BuildProfileDraw(
                        profile.ThicknessView, 
                        profile.CutLength);
                    if (cabin.HasStep)
                    {
                        StepWallAluminium1 = DrawsFactory.BuildProfileDraw(
                            profile.ThicknessView, 
                            profile.CutLengthStepPart);
                        StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
                    }
                    break;
                case CabinSupport support:
                    if (cabin.HasStep)
                    {
                        StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
                    }
                    GlassToGlassSupportTop = DrawsFactory.HingeDraws.BuildGlassSupport(support);
                    GlassToGlassSupportBottom = DrawsFactory.HingeDraws.BuildGlassSupport(support);
                    break;
                default:
                    if (cabin.HasStep)
                    {
                        StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
                    }
                    break;
            }

            switch (cabin.Parts.BottomFixer)
            {
                case Profile profile:
                    FloorAluminium = DrawsFactory.BuildProfileDraw(
                            profile.CutLength,
                            profile.ThicknessView);
                    if (cabin.HasStep)
                    {
                        StepFloorAluminium1 = DrawsFactory.BuildProfileDraw(
                            profile.CutLengthStepPart, 
                            profile.ThicknessView);                        
                    }
                    break;
                case FloorStopperW support:
                    FloorStopper = DrawsFactory.BuildFloorStopper(support);
                    break;
                default:
                    break;
            }

            switch (cabin.Parts.TopFixer)
            {
                case Profile topProfile:
                    TopProfile = DrawsFactory.BuildProfileDraw(
                        topProfile.CutLength,
                        topProfile.ThicknessView);
                    break;
                default:
                    break;
            }

            switch (cabin.Parts.SideFixer)
            {
                case Profile sideProfile:
                    SideProfile = DrawsFactory.BuildProfileDraw(
                        sideProfile.ThicknessView,
                        sideProfile.CutLength);
                    break;
                case CabinSupport sideSupport:
                    GlassToGlassSupportSideBottom = DrawsFactory.HingeDraws.BuildGlassSupport(sideSupport);
                    GlassToGlassSupportSideTop = DrawsFactory.HingeDraws.BuildGlassSupport(sideSupport);
                    break;
                default:
                    break;
            }

            if (cabin.Parts.SupportBar is not null)
            {
                SupportBar = DrawsFactory.BuildSupportBarClamp(cabin.Parts.SupportBar);
            }
           
            //Irrelevant
            GlassAluminium1 = DrawsFactory.BuildProfileDraw(0, 0);

            if (cabin.Parts.CloseStrip is not null)
            {
                MagnetStrip = DrawsFactory.BuildStripDraw(
                    cabin.Parts.CloseStrip.OutOfGlassLength, 
                    cabin.Parts.CloseStrip.CutLength);
            }

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);
            FixedGlassDraw1.LayerNo = 1;
        }

        protected override void PlaceParts()
        {
            double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
            double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;

            if (WallAluminium1 is not null && cabin.Parts.WallSideFixer is Profile profile)
            {
                WallAluminium1.SetCenterOrStartX(0, CSCoordinate.Start);
                //If there is a correction the Profile will be further down , as the correction dictates
                WallAluminium1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

                // At (EndX of Wall Al1 - ALST , StartY of Wall Al1 -- When the AL1 is 0Length do not calculate ALST
                FixedGlassDraw1.SetCenterOrStartX(WallAluminium1.GetBoundingBoxRectangle().EndX - profile.GlassInProfileDepth, CSCoordinate.Start);
                FixedGlassDraw1.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().StartY, CSCoordinate.Start);

                if (cabin.HasStep && StepWallAluminium1 is not null)
                {
                    StepWallAluminium1.SetCenterOrStartX(stepLength, CSCoordinate.Start);
                    StepWallAluminium1.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY, CSCoordinate.Start);
                }
            }
            else if (GlassToGlassSupportTop is not null 
                && GlassToGlassSupportBottom is not null 
                && cabin.Parts.WallSideFixer is CabinSupport support)
            {
                // At (EndX of Wall Al1 - ALST , StartY of Wall Al1 -- When the AL1 is 0Length do not calculate ALST
                FixedGlassDraw1.SetCenterOrStartX(support.GlassGapAER, CSCoordinate.Start);
                // If there is a correction the Fixed Glass will be a little further down as the correction dictates
                FixedGlassDraw1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

                GlassToGlassSupportTop.SetCenterOrStartX(FixedGlassDraw1.GetBoundingBoxRectangle().StartX - support.GlassGapAER, CSCoordinate.Start);
                GlassToGlassSupportBottom.SetCenterOrStartX(FixedGlassDraw1.GetBoundingBoxRectangle().StartX - support.GlassGapAER, CSCoordinate.Start);


                //Place the Supports on the Glass
                GlassToGlassSupportTop.SetCenterOrStartY(FixedGlassDraw1.GetBoundingBoxRectangle().StartY 
                                                         + GlassProcessesConstants.ProcessesInox304.SupportHoleTopDistanceVF
                                                         , CSCoordinate.Center);
                GlassToGlassSupportBottom.SetCenterOrStartY(FixedGlassDraw1.GetBoundingBoxRectangle().EndY 
                                                            - GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVF
                                                            , CSCoordinate.Center);
            }
            else
            {
                //There are no Fixers only Glass
                FixedGlassDraw1.SetCenterOrStartX(0, CSCoordinate.Start);
                // If there is a correction the Fixed Glass will be a little further down as the correction dictates
                FixedGlassDraw1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);
            }

            if (cabin.HasStep)
            {
                StepWallArea.SetCenterOrStartX(0, CSCoordinate.Start);
                StepWallArea.SetCenterOrStartY(cabin.Height - stepHeight, CSCoordinate.Start);
            }

            switch (cabin.Parts.BottomFixer)
            {
                case Profile floorProfile:
                    if (cabin.HasStep)
                    {
                        if (WallAluminium1 is not null)
                        {
                            StepFloorAluminium1.SetCenterOrStartX(WallAluminium1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                            StepFloorAluminium1.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY - floorProfile.ThicknessView, CSCoordinate.Start);
                            FloorAluminium.SetCenterOrStartX(StepWallAluminium1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                            FloorAluminium.SetCenterOrStartY(StepWallAluminium1.GetBoundingBoxRectangle().EndY - floorProfile.ThicknessView, CSCoordinate.Start);
                        }
                        else //Even if it has Supports the Floor Profile is placed on all the Glass
                        {
                            StepFloorAluminium1.SetCenterOrStartX(StepWallArea.GetBoundingBoxRectangle().StartX, CSCoordinate.Start);
                            StepFloorAluminium1.SetCenterOrStartY(StepWallArea.GetBoundingBoxRectangle().StartY - floorProfile.ThicknessView, CSCoordinate.Start);

                            FloorAluminium.SetCenterOrStartX(StepWallArea.GetBoundingBoxRectangle().EndX , CSCoordinate.Start);
                            FloorAluminium.SetCenterOrStartY(StepWallArea.GetBoundingBoxRectangle().EndY - floorProfile.ThicknessView, CSCoordinate.Start);
                        }
                    }
                    else
                    {
                        if (WallAluminium1 is not null)
                        {
                            FloorAluminium.SetCenterOrStartX(WallAluminium1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                            FloorAluminium.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY - floorProfile.ThicknessView, CSCoordinate.Start);
                        }
                        else
                        {
                            FloorAluminium.SetCenterOrStartX(FixedGlassDraw1.GetBoundingBoxRectangle().StartX,CSCoordinate.Start);
                            FloorAluminium.SetCenterOrStartY(FixedGlassDraw1.GetBoundingBoxRectangle().EndY - floorProfile.ThicknessView, CSCoordinate.Start);
                        }
                    }
                    break;
                case FloorStopperW floorStopper:
                    //At (EndX of Glass - (WidthOfStopper - OutOfGlassLength) , EndY of Glass - Height of Stopper)
                    FloorStopper.SetCenterOrStartX(FixedGlassDraw1.GetBoundingBoxRectangle().EndX - (floorStopper.LengthView - floorStopper.OutOfGlassLength), CSCoordinate.Start);
                    FloorStopper.SetCenterOrStartY(FixedGlassDraw1.GetBoundingBoxRectangle().EndY - floorStopper.HeightView, CSCoordinate.Start); ;
                    break;
                default:
                    //Do nothing
                    break;
            }

            if (TopProfile is not null && cabin.Parts.TopFixer is Profile topProfile)
            {
                if (WallAluminium1 is not null)
                {
                    TopProfile.SetCenterOrStartX(
                        WallAluminium1.GetBoundingBoxRectangle().EndX,
                        CSCoordinate.Start);
                    TopProfile.SetCenterOrStartY(
                        WallAluminium1.GetBoundingBoxRectangle().StartY,
                        CSCoordinate.Start);
                }
                if (GlassToGlassSupportTop is not null)
                {
                    TopProfile.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().StartX,
                        CSCoordinate.Start);
                    TopProfile.SetCenterOrStartY(
                        cabin.Constraints.FinalHeightCorrection,
                        CSCoordinate.Start);
                }
                //Move the Glass A bit further Down to make up for the Added Profile on Top
                FixedGlassDraw1.SetCenterOrStartY(
                    TopProfile.GetBoundingBoxRectangle().EndY
                    - topProfile.GlassInProfileDepth,
                    CSCoordinate.Start);
            }

            switch (cabin.Parts.SideFixer)
            {
                case Profile sideProfile:
                    SideProfile.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndX
                        - sideProfile.GlassInProfileDepth,
                        CSCoordinate.Start);
                    //Either Start of Top Profile if there otherwise start of Glass
                    var sideProfileStartingY = TopProfile?.GetBoundingBoxRectangle().StartY 
                                             ?? FixedGlassDraw1.GetBoundingBoxRectangle().StartY ;
                    SideProfile.SetCenterOrStartY(
                        sideProfileStartingY,
                        CSCoordinate.Start);
                    break;
                case CabinSupport sideSupport:
                    //At the End of Glass Minus their Length (plus a little bit outside for the Gap they Have)
                    GlassToGlassSupportSideTop.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndX
                        - sideSupport.LengthView
                        + sideSupport.GlassGapAER
                        , CSCoordinate.Start);
                    GlassToGlassSupportSideBottom.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndX
                        - sideSupport.LengthView
                        + sideSupport.GlassGapAER
                        , CSCoordinate.Start);

                    //Place the Supports on the Glass
                    GlassToGlassSupportSideTop.SetCenterOrStartY(
                        FixedGlassDraw1.GetBoundingBoxRectangle().StartY
                        + GlassProcessesConstants.ProcessesInox304.SupportHoleTopDistanceVF
                        , CSCoordinate.Center);
                    GlassToGlassSupportSideBottom.SetCenterOrStartY(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                        - GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVF
                        , CSCoordinate.Center);
                    break;
                default:
                    break;
            }

            if (cabin.Parts.SupportBar is not null)
            {
                //At (EndX Of Glass - Distance of Support Center , StartY of Glass - OutOfGlassHeight)
                SupportBar.SetCenterOrStartX(
                    FixedGlassDraw1.GetBoundingBoxRectangle().EndX 
                    - cabin.Parts.SupportBar.ClampCenterDistanceFromGlass
                    , CSCoordinate.Center);

                //if there is profile on top move the support bar to hold the profile not the glass
                if (TopProfile is not null)
                {
                    SupportBar.SetCenterOrStartY(
                    TopProfile.GetBoundingBoxRectangle().StartY
                    - cabin.Parts.SupportBar.OutOfGlassHeight
                    , CSCoordinate.Start);
                }
                else
                {
                    SupportBar.SetCenterOrStartY(
                    FixedGlassDraw1.GetBoundingBoxRectangle().StartY
                    - cabin.Parts.SupportBar.OutOfGlassHeight
                    , CSCoordinate.Start);
                }
            }

            if (MagnetStrip is not null)
            {
                MagnetStrip.SetCenterOrStartX(FixedGlassDraw1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                //Place the Strip below the Corner Radius , if there is one
                MagnetStrip.SetCenterOrStartY(
                    FixedGlassDraw1.GetBoundingBoxRectangle().StartY
                    + cabin.Constraints.CornerRadiusTopEdge
                    ,CSCoordinate.Start);
                MagnetStrip.LayerNo = 1;
            }

            //Rearrange Supports When Cabin Has Step
            if (cabin.HasStep && cabin.Parts.WallSideFixer is CabinSupport)
            {
                //if Step height is above threshold of not havin support ,
                //then move the support onto the step
                if (stepHeight > GlassProcessesConstants.ProcessesInox304.StepMaxHeightWithoutSupportHole)
                {
                    GlassToGlassSupportBottom.SetCenterOrStartX(
                        FixedGlassDraw1.GetBoundingBoxRectangle().StartX + stepLength
                        , CSCoordinate.Start);
                }
                //If the step height is below the threshold then take the support and place it 50cm above it
                //only if the distance already is not above 50cm
                else if (stepHeight + GlassProcessesConstants.ProcessesInox304.SupportHoleMinDistanceFromStep > GlassProcessesConstants.ProcessesInox304.SupportHoleBottomDistanceVA)
                {
                    GlassToGlassSupportBottom.SetCenterOrStartY(
                        FixedGlassDraw1.GetBoundingBoxRectangle().EndY
                        - stepHeight
                        - GlassProcessesConstants.ProcessesInox304.SupportHoleMinDistanceFromStep,
                        CSCoordinate.Start);
                }
                //Else leave default positions
            }


        }

        protected override void PlaceDrawNames()
        {
            if (WallAluminium1 is not null)
            {
                WallAluminium1.Name = nameof(WallAluminium1);
            }
            if (StepWallAluminium1 is not null)
            {
                StepWallAluminium1.Name = nameof(StepWallAluminium1);
            }
            if (StepFloorAluminium1 is not null)
            {
                StepFloorAluminium1.Name = nameof(StepFloorAluminium1);
            }
            if (StepWallArea != null)
            {
                StepWallArea.Name = nameof(StepWallArea);
            }
            if (GlassToGlassSupportBottom != null && GlassToGlassSupportTop != null)
            {
                GlassToGlassSupportTop.Name = nameof(GlassToGlassSupportTop);
                GlassToGlassSupportBottom.Name = nameof(GlassToGlassSupportBottom);
            }
            if (GlassToGlassSupportSideTop is not null && GlassToGlassSupportSideBottom is not null)
            {
                GlassToGlassSupportSideTop.Name = nameof(GlassToGlassSupportSideTop);
                GlassToGlassSupportSideBottom.Name = nameof(GlassToGlassSupportSideBottom);
            }
            if (TopProfile is not null)
            {
                TopProfile.Name = nameof(TopProfile);
            }
            if (SideProfile is not null)
            {
                SideProfile.Name = nameof(SideProfile);
            }

            if (FloorAluminium != null)
            {
                FloorAluminium.Name = nameof(FloorAluminium);
            }

            GlassAluminium1.Name = nameof(GlassAluminium1);

            if (SupportBar is not null)
            {
                SupportBar.Name = nameof(SupportBar);
            }

            if (FloorStopper != null)
            {
                FloorStopper.Name = nameof(FloorStopper);
            }
            
            if (MagnetStrip != null)
            {
                MagnetStrip.Name = nameof(MagnetStrip);
            }

            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(WallAluminium1);
            draws.AddNotNull(StepFloorAluminium1);
            draws.AddNotNull(StepWallAluminium1);
            draws.AddNotNull(StepWallArea);
            draws.AddNotNull(GlassToGlassSupportTop);
            draws.AddNotNull(GlassToGlassSupportBottom);

            draws.AddNotNull(GlassToGlassSupportSideTop);
            draws.AddNotNull(GlassToGlassSupportSideBottom);
            draws.AddNotNull(TopProfile);
            draws.AddNotNull(SideProfile);

            draws.AddNotNull(FloorAluminium);
            draws.AddNotNull(GlassAluminium1);
            draws.AddNotNull(SupportBar);
            draws.AddNotNull(FloorStopper);
            draws.AddNotNull(MagnetStrip);
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
            draws.AddNotNull(WallAluminium1);
            draws.AddNotNull(StepFloorAluminium1);
            draws.AddNotNull(StepWallAluminium1);
            draws.AddNotNull(GlassToGlassSupportTop);
            draws.AddNotNull(GlassToGlassSupportBottom);
            draws.AddNotNull(FloorAluminium);
            draws.AddNotNull(GlassAluminium1);
            draws.AddNotNull(SupportBar);
            draws.AddNotNull(FloorStopper);
            draws.AddNotNull(GlassToGlassSupportSideTop);
            draws.AddNotNull(GlassToGlassSupportSideBottom);
            draws.AddNotNull(TopProfile);
            draws.AddNotNull(SideProfile);
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(MagnetStrip);
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

using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.HBDraws
{
    public class CabinHBDraw : CabinDraw<CabinHB>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;

        public DrawShape WallAluminium1 { get; set; }
        public DrawShape GlassAluminium1 { get; set; }
        public DrawShape GlassToGlassSupportTop { get; set; }
        public DrawShape GlassToGlassSupportBottom { get; set; }
        public DrawShape MagnetAluminium { get; set; }
        public DrawShape MagnetStrip { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }
        public DrawShape DoorGlassDraw1 { get; set; }
        public DrawShape DoorHandle1 { get; set; }
        public DrawShape HingeTop { get; set; }
        public DrawShape HingeBottom { get; set; }
        public DrawShape SupportBar { get; set; }
        public DrawShape FloorStopper { get; set; }
        public DrawShape FloorAluminium { get; set; }

        /// <summary>
        /// The Step-Part of the Wall Aluminium
        /// </summary>
        public DrawShape StepWallAluminium1 { get; set; }
        public DrawShape StepFloorAluminium1 { get; set; }
        public DrawShape StepWallArea { get; set; }

        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }
        private Glass DoorGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }

        public CabinHBDraw(CabinHB cabin) : base(cabin) { }

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
                case Profile floorProfile:
                    FloorAluminium = DrawsFactory.BuildProfileDraw(
                        floorProfile.CutLength,
                        floorProfile.ThicknessView);
                    if (cabin.HasStep)
                    {
                        StepFloorAluminium1 = DrawsFactory.BuildProfileDraw(
                            floorProfile.CutLengthStepPart,
                            floorProfile.ThicknessView);
                    }
                    break;
                case FloorStopperW stopper:
                    FloorStopper = DrawsFactory.BuildFloorStopper(stopper);
                    break;
                default:
                    //Do nothing no bottom fixer
                    break;
            }

            if (cabin.Parts.CloseProfile is not null)
            {
                MagnetAluminium = DrawsFactory.BuildProfileDraw(
                    cabin.Parts.CloseProfile.ThicknessView,
                    cabin.Parts.CloseProfile.CutLength);
            }

            if (cabin.Parts.CloseStrip is not null)
            {
                MagnetStrip = DrawsFactory.BuildStripDraw(
                    cabin.Parts.CloseStrip.OutOfGlassLength,
                    cabin.Parts.CloseStrip.CutLength);
            }

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);
            FixedGlassDraw1.LayerNo = 1;

            DoorGlassDraw1 = DrawsFactory.BuildGlassDraw(DoorGlass,cabin.SynthesisModel); //draw opposite any rounding
            DoorGlassDraw1.LayerNo = 1;

            if (cabin.Parts.Handle is not null)
            {
                DoorHandle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            }
            HingeTop = DrawsFactory.HingeDraws.BuildHingeNew(cabin.Parts.Hinge);
            HingeBottom = DrawsFactory.HingeDraws.BuildHingeNew(cabin.Parts.Hinge);
            SupportBar = DrawsFactory.BuildSupportBarClamp(cabin.Parts.SupportBar);
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

                if (StepWallAluminium1 is not null)
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
                            FloorAluminium.SetCenterOrStartX(FixedGlassDraw1.GetBoundingBoxRectangle().StartX, CSCoordinate.Start);
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

            var fixedGlassRectangle = FixedGlassDraw1.GetBoundingBoxRectangle();

            if (cabin.Parts.SupportBar is not null)
            {
                //At (EndX Of Glass - Distance of Support Center , StartY of Glass - OutOfGlassHeight)
                SupportBar.SetCenterOrStartX(
                    fixedGlassRectangle.EndX
                    - cabin.Parts.SupportBar.ClampCenterDistanceFromGlass
                    , CSCoordinate.Center);
                SupportBar.SetCenterOrStartY(
                    fixedGlassRectangle.StartY
                    - cabin.Parts.SupportBar.OutOfGlassHeight
                    , CSCoordinate.Start);
            }

            //At (EndX of Fixed + Gap , StartY of Fixed)
            DoorGlassDraw1.SetCenterOrStartX(
                fixedGlassRectangle.EndX 
                + cabin.Parts.Hinge.GlassGapAER
                , CSCoordinate.Start);
            DoorGlassDraw1.SetCenterOrStartY(
                fixedGlassRectangle.StartY
                , CSCoordinate.Start);
            
            var doorGlassRectangle = DoorGlassDraw1.GetBoundingBoxRectangle();

            if (MagnetStrip is not null)
            {
                MagnetStrip.SetCenterOrStartX(
                    doorGlassRectangle.EndX,
                    CSCoordinate.Start);
                //Place the Strip below the Corner Radius if there is one
                MagnetStrip.SetCenterOrStartY(
                    DoorGlassDraw1.GetBoundingBoxRectangle().StartY
                    +cabin.Constraints.CornerRadiusTopEdge
                    ,CSCoordinate.Start);
            }

            //At (EndX of FixedGlass - The Distance Inside the Glass , StartY of Door Glass + Distance from Top)
            HingeTop.SetCenterOrStartX(
                fixedGlassRectangle.EndX 
                - (cabin.Parts.Hinge.LengthView 
                    - cabin.Parts.Hinge.InDoorLength 
                    - cabin.Parts.Hinge.GlassGapAER)
                , CSCoordinate.Start);
            HingeTop.SetCenterOrStartY(
                doorGlassRectangle.StartY 
                + GlassProcessesConstants.ProcessesHotel8000.CutTopDistanceHB2
                , CSCoordinate.Center);

            //At (EndX of Fixed - The Distance Inside the Glass , EndY of Door Glass - Distance from Bottom)
            HingeBottom.SetCenterOrStartX(
                fixedGlassRectangle.EndX
                - (cabin.Parts.Hinge.LengthView
                    - cabin.Parts.Hinge.InDoorLength
                    - cabin.Parts.Hinge.GlassGapAER)
                , CSCoordinate.Start);
            HingeBottom.SetCenterOrStartY(
                doorGlassRectangle.EndY 
                - GlassProcessesConstants.ProcessesHotel8000.CutBottomDistanceHB2
                , CSCoordinate.Center);

            if (DoorHandle1 is not null)
            {
                //At (EndOf Glass - Distance from End , Middle of Glass)
                DoorHandle1.SetCenterOrStartX(
                    doorGlassRectangle.EndX
                    - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge()
                    , CSCoordinate.Center);
                DoorHandle1.SetCenterOrStartY(
                    DoorGlassDraw1.ShapeCenterY
                    , CSCoordinate.Center);
            }

            if (MagnetAluminium is not null)
            {
                MagnetAluminium.SetCenterOrStartX(
                    MagnetStrip.GetBoundingBoxRectangle().EndX
                    , CSCoordinate.Start);
                MagnetAluminium.SetCenterOrStartY(
                    cabin.Constraints.FinalHeightCorrection,
                    CSCoordinate.Start);
            }

        }

        protected override void PlaceDrawNames()
        {
            if (WallAluminium1 is not null)
            {
                WallAluminium1.Name = nameof(WallAluminium1);
            }
            if (GlassAluminium1 is not null)
            {
                GlassAluminium1.Name = nameof(GlassAluminium1);
            }
            if (GlassToGlassSupportTop is not null && GlassToGlassSupportBottom is not null)
            {
                GlassToGlassSupportTop.Name = nameof(GlassToGlassSupportTop);
                GlassToGlassSupportBottom.Name = nameof(GlassToGlassSupportBottom);
            }

            if (StepWallArea != null) StepWallArea.Name = nameof(StepWallArea);
            if (StepWallAluminium1 != null) StepWallAluminium1.Name = nameof(StepWallAluminium1);
            if (StepFloorAluminium1 != null) StepFloorAluminium1.Name = nameof(StepFloorAluminium1);
            if (MagnetAluminium != null) MagnetAluminium.Name = nameof(MagnetAluminium);
            if (MagnetStrip != null) MagnetStrip.Name = nameof(MagnetStrip);
            
            
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
            DoorGlassDraw1.Name = nameof(DoorGlassDraw1);
            if (DoorHandle1 is not null)
            {
                DoorHandle1.Name = nameof(DoorHandle1);
            }
            HingeTop.Name = nameof(HingeTop);
            HingeBottom.Name = nameof(HingeBottom);
            SupportBar.Name = nameof(SupportBar);

            if (FloorStopper != null)
            {
                FloorStopper.Name = nameof(FloorStopper);
            }

            if (FloorAluminium != null)
            {
                FloorAluminium.Name = nameof(FloorAluminium);
            }
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(WallAluminium1);
            draws.AddNotNull(GlassAluminium1);
            draws.AddNotNull(StepFloorAluminium1);
            draws.AddNotNull(StepWallAluminium1);
            draws.AddNotNull(StepWallArea);
            draws.AddNotNull(MagnetAluminium);
            draws.AddNotNull(MagnetStrip);
            draws.AddNotNull(GlassToGlassSupportBottom);
            draws.AddNotNull(GlassToGlassSupportTop);
            draws.Add(FixedGlassDraw1);
            draws.Add(DoorGlassDraw1);
            draws.AddNotNull(DoorHandle1);
            draws.Add(HingeTop);
            draws.Add(HingeBottom);
            draws.Add(SupportBar);
            draws.AddNotNull(FloorStopper);
            draws.AddNotNull(FloorAluminium);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(FixedGlassDraw1);
            draws.Add(DoorGlassDraw1);
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(WallAluminium1);
            draws.AddNotNull(GlassAluminium1);
            draws.AddNotNull(GlassToGlassSupportBottom);
            draws.AddNotNull(GlassToGlassSupportTop);
            draws.AddNotNull(StepFloorAluminium1);
            draws.AddNotNull(StepWallAluminium1);
            draws.AddNotNull(MagnetAluminium);
            draws.AddNotNull(DoorHandle1);
            draws.Add(HingeTop);
            draws.Add(HingeBottom);
            draws.Add(SupportBar);
            draws.AddNotNull(FloorStopper);
            draws.AddNotNull(FloorAluminium);
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

        public override List<DrawShape> GetHelperDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(StepWallArea);
            return draws;
        }
    }
}

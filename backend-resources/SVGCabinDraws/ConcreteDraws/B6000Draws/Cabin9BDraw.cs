using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using SVGCabinDraws.Enums;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.B6000Draws
{
    public class Cabin9BDraw : CabinDraw<Cabin9B>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;

        public DrawShape WallAluminium1 { get; set; }
        public DrawShape StepWallAluminium1 { get; set; }
        public DrawShape StepFloorAluminium1 { get; set; }
        public DrawShape StepWallArea { get; set; }
        public DrawShape GlassAluminium1 { get; set; }
        public DrawShape MagnetAluminium { get; set; }
        public DrawShape MagnetAluminiumStrip { get; set; }
        public DrawShape DoorHandle1 { get; set; }
        public DrawShape HingeTop { get; set; }
        public DrawShape HingeSupportTop { get; set; }
        public DrawShape HingeBottom { get; set; }
        public DrawShape HingeSupportBottom { get; set; }
        public DrawShape HorizontalAluminiumTop { get; set; }
        public DrawShape HorizontalAluminiumBottom { get; set; }

        public DrawShape DoorGlassDraw1 { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }

        private Glass DoorGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }
        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }

        /// <summary>
        /// Creates a Draw of a 9B Cabin
        /// </summary>
        /// <param name="cabin"></param>
        public Cabin9BDraw(Cabin9B cabin) : base(cabin) { }

        /// <summary>
        /// Builds all the Parts of the Draw
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override void InitilizeDraw()
        {
            //If the Wall Aluminiums are connectors then the used thickness is the inner thickness
            //When the Profile is a Connector the Calculation of Thickness should be that of the front Side and not the Side that goes to the 9F Profile
            //The Connector Profiles have a ThicknessView for the 9F Part and an Inner Thickness View for the Part that is in front
            int profile1Thickness;
            int profile2Thickness;
            if (cabin.Parts.WallProfile1?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile1Thickness = cabin.Parts.WallProfile1.InnerThicknessView;
            }
            else
            {
                profile1Thickness = cabin.Parts.WallProfile1?.ThicknessView ?? 0;
            }
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.MagnetProfile || cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
            {
                profile2Thickness = cabin.Parts.WallProfile2.InnerThicknessView;
            }
            else
            {
                profile2Thickness = cabin.Parts.WallProfile2?.ThicknessView ?? 0;
            }

            WallAluminium1 = DrawsFactory.BuildProfileDraw(
                profile1Thickness, 
                cabin.Parts.WallProfile1.CutLength);

            //If there is a step Divide the Wall Aluminium into two pieces
            if (cabin.HasStep)
            {
                double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
                double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;

                StepFloorAluminium1 = DrawsFactory.BuildProfileDraw(
                    cabin.Parts.StepBottomProfile.CutLength,
                    cabin.Parts.StepBottomProfile.ThicknessView);

                StepWallAluminium1 = DrawsFactory.BuildProfileDraw(
                    profile1Thickness,
                    cabin.Parts.WallProfile1.CutLengthStepPart);

                StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
            }

            GlassAluminium1 = DrawsFactory.BuildProfileDraw(0, 0);

            HorizontalAluminiumTop = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalProfileTop.CutLength, 
                cabin.Parts.HorizontalProfileTop.ThicknessView);
            HorizontalAluminiumTop.LayerNo = 3; //Drawn on the Back of Side Auminium and Door Glass

            HorizontalAluminiumBottom = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalProfileBottom.CutLength, 
                cabin.Parts.HorizontalProfileBottom.ThicknessView);
            HorizontalAluminiumBottom.LayerNo = 3; //Drawn on the Back of Side Auminium and Door Glass

            if (FixedGlass != null)
            {
                FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);
                FixedGlassDraw1.LayerNo = 4; //Drawn on the Back of Horizontal Aluminiums
            }

            DoorGlassDraw1 = DrawsFactory.BuildGlassDraw(DoorGlass);
            DoorGlassDraw1.LayerNo = 1; //Drawn to the Front! Must be above Aluminium Magnet

            HingeTop = DrawsFactory.HingeDraws.Build9BHinge(HingePosition.Top,cabin.Parts.Hinge);
            HingeSupportTop = DrawsFactory.HingeDraws.Build9BHingeSupport(cabin.Parts.Hinge);
            HingeSupportTop.LayerNo = 4; //Drawn on the Back of Horizontal Aluminiums

            HingeBottom = DrawsFactory.HingeDraws.Build9BHinge(HingePosition.Bottom,cabin.Parts.Hinge);
            HingeSupportBottom = DrawsFactory.HingeDraws.Build9BHingeSupport(cabin.Parts.Hinge);
            HingeSupportBottom.LayerNo = 4; //Drawn on the Back of Horizontal Aluminiums

            DoorHandle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);

            MagnetAluminium = DrawsFactory.BuildProfileDraw(
                profile2Thickness, 
                cabin.Parts.WallProfile2.CutLength);
            MagnetAluminium.LayerNo = 2;


            //Draw the Strip at the Door Glasses Height otherwise throw exception -- there must always be a door glass
            MagnetAluminiumStrip = DrawsFactory.BuildStripDraw(
                cabin.Parts.CloseStrip.OutOfGlassLength,
                cabin.Parts.CloseStrip.CutLength);
            MagnetAluminiumStrip.LayerNo = 0; // Drawn on the Front of everything else
        }

        /// <summary>
        /// Places The Parts Coordinates
        /// </summary>
        protected override void PlaceParts()
        {
            //At (0,0)
            WallAluminium1.SetCenterOrStartX(0, CSCoordinate.Start);
            WallAluminium1.SetCenterOrStartY(0, CSCoordinate.Start);

            //At (0,0) IRRELEVANT NOT IMPEMENTED DRAW
            GlassAluminium1.SetCenterOrStartX(0, CSCoordinate.Start);
            GlassAluminium1.SetCenterOrStartY(0, CSCoordinate.Start);


            double stepLength = cabin.GetStepCut()?.StepLength ?? 0;
            double stepHeight = cabin.GetStepCut()?.StepHeight ?? 0;

            if (StepWallAluminium1 != null && StepFloorAluminium1 != null && StepWallArea != null)
            {
                //At (EndX of AL1 , EndY of AL1 - Height of Aluminium)
                StepFloorAluminium1.SetCenterOrStartX(WallAluminium1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
                StepFloorAluminium1.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY - cabin.Parts.StepBottomProfile.ThicknessView, CSCoordinate.Start);
                //At (0,EndY of AL1)
                StepWallAluminium1.SetCenterOrStartX(stepLength, CSCoordinate.Start);
                StepWallAluminium1.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY, CSCoordinate.Start);

                //At (0,EndY of AL1)
                StepWallArea.SetCenterOrStartX(0, CSCoordinate.Start);
                StepWallArea.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY, CSCoordinate.Start);
            }


            //At (End of Wall Aluminium , 0)
            HorizontalAluminiumTop.SetCenterOrStartX(WallAluminium1.BoundingBoxLength, CSCoordinate.Start);
            HorizontalAluminiumTop.SetCenterOrStartY(0, CSCoordinate.Start);

            //At (End of Wall Aluminium , Bottom of Cabin)
            HorizontalAluminiumBottom.SetCenterOrStartX(WallAluminium1.BoundingBoxLength + stepLength, CSCoordinate.Start);
            HorizontalAluminiumBottom.SetCenterOrStartY(cabin.Height - cabin.Parts.HorizontalProfileBottom.ThicknessView, CSCoordinate.Start);

            if (FixedGlass != null)
            {
                //At (InsideWall Aluminium ALST , Distance of Glass From Top)
                FixedGlassDraw1.SetCenterOrStartX(WallAluminium1.BoundingBoxLength - cabin.Parts.WallProfile1.GlassInProfileDepth, CSCoordinate.Start);
                FixedGlassDraw1.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.ThicknessView - cabin.Parts.HorizontalProfileTop.GlassInProfileDepth, CSCoordinate.Start);

                //At (End of FixedGlass + AER , SAME centerY as CENTER of FixedGlass)
                DoorGlassDraw1.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX + FixedGlassDraw1.BoundingBoxLength / 2 + cabin.Constraints.GlassGapAERHorizontal, CSCoordinate.Start);
                DoorGlassDraw1.SetCenterOrStartY(FixedGlassDraw1.ShapeCenterY, CSCoordinate.Center);

                //At (EndX of FixedGlass + PivotDistance , StartY of DoorGlass - OverlapingHeight)
                HingeTop.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX + FixedGlassDraw1.BoundingBoxLength / 2d + cabin.Constraints.HingeDistanceFromDoorGlass , CSCoordinate.Start);
                HingeTop.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY - DoorGlassDraw1.BoundingBoxHeight / 2d - cabin.Parts.Hinge.HingeOverlappingHeight , CSCoordinate.Start);

                //At (EndX of FixedGlass + PivotDistance , EndY of DoorGlass + OverlapingHeight - Height of Hinge)
                HingeBottom.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX + FixedGlassDraw1.BoundingBoxLength / 2d + cabin.Constraints.HingeDistanceFromDoorGlass, CSCoordinate.Start);
                HingeBottom.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY + DoorGlassDraw1.BoundingBoxHeight / 2d + cabin.Parts.Hinge.HingeOverlappingHeight - cabin.Parts.Hinge.HeightView, CSCoordinate.Start);
            }
            else //When there is no Fixed Part
            {
                //At (End of WallAluminium + AER , SAME centerY as CENTER of Wall Aluminium)
                DoorGlassDraw1.SetCenterOrStartX(WallAluminium1.ShapeCenterX + WallAluminium1.BoundingBoxLength / 2d + cabin.Constraints.GlassGapAERHorizontal, CSCoordinate.Start);
                DoorGlassDraw1.SetCenterOrStartY(WallAluminium1.ShapeCenterY, CSCoordinate.Center);

                //At (EndX of AL1 + PivotDistance , Same Y as DoorGlass + OverlapingHeight
                HingeTop.SetCenterOrStartX(WallAluminium1.ShapeCenterX + WallAluminium1.BoundingBoxLength / 2d + cabin.Constraints.HingeDistanceFromDoorGlass, CSCoordinate.Start);
                HingeTop.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY - DoorGlassDraw1.BoundingBoxHeight / 2d - cabin.Parts.Hinge.HingeOverlappingHeight, CSCoordinate.Start);

                //At (EndX of AL1 + PivotDistance , EndY of DoorGlass + OverlapingHeight - Height of Hinge)
                HingeBottom.SetCenterOrStartX(WallAluminium1.ShapeCenterX + WallAluminium1.BoundingBoxLength / 2d + cabin.Constraints.HingeDistanceFromDoorGlass, CSCoordinate.Start);
                HingeBottom.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY + DoorGlassDraw1.BoundingBoxHeight / 2d + cabin.Parts.Hinge.HingeOverlappingHeight - cabin.Parts.Hinge.HeightView, CSCoordinate.Start);
            }

            //At (Center X Same as CenterX of Hinge , StartY of Hinge - Height of The Supprt
            HingeSupportTop.SetCenterOrStartX(HingeTop.ShapeCenterX, CSCoordinate.Center);
            HingeSupportTop.SetCenterOrStartY(HingeTop.GetBoundingBoxRectangle().StartY - cabin.Parts.Hinge.SupportTubeHeight , CSCoordinate.Start);

            //At (Center X Same as CenterX of Hinge , StartY of Hinge - Height of The Supprt
            HingeSupportBottom.SetCenterOrStartX(HingeBottom.ShapeCenterX, CSCoordinate.Center);
            HingeSupportBottom.SetCenterOrStartY(HingeBottom.GetBoundingBoxRectangle().EndY, CSCoordinate.Start);

            //CENTER AT (End of Door Glass - HandleDistance , Mid of Door Glass )
            DoorHandle1.SetCenterOrStartX(
                DoorGlassDraw1.GetBoundingBoxRectangle().EndX 
                - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge()
                , CSCoordinate.Center);
            DoorHandle1.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY, CSCoordinate.Center);

            //At (End of Door Glass , Same with DoorGlass Center)
            MagnetAluminiumStrip.SetCenterOrStartX(DoorGlassDraw1.GetBoundingBoxRectangle().EndX, CSCoordinate.Start);
            MagnetAluminiumStrip.SetCenterOrStartY(DoorGlassDraw1.ShapeCenterY, CSCoordinate.Center);

            //At (The Same Start as the MagnetStrip , The Same Y as the Magnet Strip (its behind the Magnet Strip)
            MagnetAluminium.SetCenterOrStartX(MagnetAluminiumStrip.ShapeCenterX + MagnetAluminiumStrip.BoundingBoxLength/2d, CSCoordinate.Start);
            MagnetAluminium.SetCenterOrStartY(MagnetAluminiumStrip.ShapeCenterY, CSCoordinate.Center);
        }

        /// <summary>
        /// Places The Fied name as the Name of the Draw
        /// </summary>
        protected override void PlaceDrawNames()
        {
            WallAluminium1.Name = nameof(WallAluminium1);
            if (StepFloorAluminium1 is not null)
            {
                StepFloorAluminium1.Name = nameof(StepFloorAluminium1);
            }
            if (StepWallAluminium1 is not null)
            {
                StepWallAluminium1.Name = nameof(StepWallAluminium1);
            }
            if (StepWallArea is not null)
            {
                StepWallArea.Name = nameof(StepWallArea);
            }
            GlassAluminium1.Name = nameof(GlassAluminium1);
            MagnetAluminium.Name = nameof(MagnetAluminium);
            MagnetAluminiumStrip.Name = nameof(MagnetAluminiumStrip);
            DoorHandle1.Name = nameof(DoorHandle1);
            HingeTop.Name = nameof(HingeTop);
            HingeSupportTop.Name = nameof(HingeSupportTop);
            HingeBottom.Name = nameof(HingeBottom);
            HingeSupportBottom.Name = nameof(HingeSupportBottom);
            HorizontalAluminiumBottom.Name = nameof(HorizontalAluminiumBottom);
            HorizontalAluminiumTop.Name = nameof(HorizontalAluminiumTop);
            if (FixedGlass != null)
            {
                FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
            }
            DoorGlassDraw1.Name = nameof(DoorGlassDraw1);
            DoorHandle1.Name = nameof(DoorHandle1);
        }

        /// <summary>
        /// Returns the List of Draws , If the Draw is Not Available it returns an Empty List
        /// </summary>
        /// <returns>Returns the List of Draws or Empty</returns>
        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.AddNotNull(StepFloorAluminium1);
            draws.AddNotNull(StepWallAluminium1);
            draws.AddNotNull(StepWallArea);
            draws.Add(GlassAluminium1);
            draws.Add(MagnetAluminium);
            draws.Add(MagnetAluminiumStrip);
            draws.Add(DoorHandle1);
            draws.Add(HingeTop);
            draws.Add(HingeSupportTop);
            draws.Add(HingeBottom);
            draws.Add(HingeSupportBottom);
            draws.Add(HorizontalAluminiumBottom);
            draws.Add(HorizontalAluminiumTop);
            draws.Add(DoorGlassDraw1);
            draws.AddNotNull(FixedGlassDraw1);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        /// <summary>
        /// Gets the List of Draws of the Various Parts that have a MetalFinish
        /// </summary>
        /// <returns></returns>
        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.AddNotNull(StepFloorAluminium1);
            draws.AddNotNull(StepWallAluminium1);
            draws.Add(GlassAluminium1);
            draws.Add(MagnetAluminium);
            draws.Add(MagnetAluminiumStrip);
            draws.Add(DoorHandle1);
            draws.Add(HingeTop);
            draws.Add(HingeSupportTop);
            draws.Add(HingeBottom);
            draws.Add(HingeSupportBottom);
            draws.Add(HorizontalAluminiumBottom);
            draws.Add(HorizontalAluminiumTop);
            return draws;
        }

        /// <summary>
        /// Gets the List of Draws of the Glasses
        /// </summary>
        /// <returns></returns>
        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(DoorGlassDraw1);
            draws.AddNotNull(FixedGlassDraw1);
            return draws;
        }

        /// <summary>
        /// Gets the List of Draws of the Polycarbonics
        /// </summary>
        /// <returns></returns>
        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
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


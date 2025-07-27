using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
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
    public class Cabin94Draw : CabinDraw<Cabin94>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening / 2d;
        public DrawShape WallAluminium1 { get; set; }
        public DrawShape WallAluminium2 { get; set; }
        public DrawShape GlassAluminium1 { get; set; }
        public DrawShape GlassAluminium2 { get; set; }
        public DrawShape MagnetStrip1 { get; set; }
        public DrawShape MagnetStrip2 { get; set; }
        public DrawShape Handle1 { get; set; }
        public DrawShape Handle2 { get; set; }
        public DrawShape HorizontalAluminiumTop { get; set; }
        public DrawShape HorizontalAluminiumBottom { get; set; }
        public DrawShape SlidingGlassDraw1 { get; set; }
        public DrawShape SlidingGlassDraw2 { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }
        public DrawShape FixedGlassDraw2 { get; set; }

        /// <summary>
        /// The Step-Part of the Wall Aluminium
        /// </summary>
        public DrawShape StepWallAluminium1 { get; set; }
        public DrawShape StepFloorAluminium1 { get; set; }
        public DrawShape StepWallArea { get; set; }

        private Glass SlidingGlass1 { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }
        private Glass SlidingGlass2 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.DoorGlass).Skip(1).FirstOrDefault(); }
        private Glass FixedGlass1 { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }
        private Glass FixedGlass2 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.FixedGlass).Skip(1).FirstOrDefault(); }

        public Cabin94Draw(Cabin94 cabin) : base(cabin) { }

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
            if (cabin.Parts.WallProfile2?.ProfileType == CabinProfileType.ConnectorProfile)
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

            WallAluminium2 = DrawsFactory.BuildProfileDraw(
                profile2Thickness, 
                cabin.Parts.WallProfile2.CutLength);

            //Irrelevant
            GlassAluminium1 = DrawsFactory.BuildProfileDraw(0, 0);
            GlassAluminium2 = DrawsFactory.BuildProfileDraw(0, 0);

            //Draw the Strip at the Sliding Gasses Height otherwise throw exception -- there must always be a sliding glass
            MagnetStrip1 = DrawsFactory.BuildStripDraw(
                cabin.Parts.CloseStrip.OutOfGlassLength, 
                cabin.Parts.CloseStrip.CutLength);
            MagnetStrip1.LayerNo = 3; // Drawn to the Back
 
            MagnetStrip2 = DrawsFactory.BuildStripDraw(
                cabin.Parts.CloseStrip.OutOfGlassLength, 
                cabin.Parts.CloseStrip.CutLength);
            MagnetStrip2.LayerNo = 3; // Drawn to the Back

            Handle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            Handle2 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);

            HorizontalAluminiumTop = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalProfileTop.CutLength, 
                cabin.Parts.HorizontalProfileTop.ThicknessView);
            HorizontalAluminiumTop.LayerNo = 1; //Drawn back of the rest Aluminiums  Because it enters inside AL3

            HorizontalAluminiumBottom = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalProfileBottom.CutLength, 
                cabin.Parts.HorizontalProfileBottom.ThicknessView);
            HorizontalAluminiumBottom.LayerNo = 1; //Drawn back of the rest Aluminiums  Because it enters inside AL3

            SlidingGlassDraw1 = DrawsFactory.BuildGlassDraw(SlidingGlass1);
            SlidingGlassDraw1.LayerNo = 3; //Drawn to the back
            SlidingGlassDraw2 = DrawsFactory.BuildGlassDraw(SlidingGlass2);
            SlidingGlassDraw2.LayerNo = 3; //Drawn to the back

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass1);
            FixedGlassDraw1.LayerNo = 2; //Drawn to the back before sliding
            FixedGlassDraw2 = DrawsFactory.BuildGlassDraw(FixedGlass2);
            FixedGlassDraw2.LayerNo = 2; //Drawn to the back before sliding
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

            //At (InsideWall Aluminium ALST , Distance of Glass From Top)
            FixedGlassDraw1.SetCenterOrStartX(WallAluminium1.BoundingBoxLength - cabin.Parts.WallProfile1.GlassInProfileDepth, CSCoordinate.Start);
            FixedGlassDraw1.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.ThicknessView - cabin.Parts.HorizontalProfileTop.GlassInProfileDepth, CSCoordinate.Start);

            //At (End of FixedGlass - Overlap , Distance of Glass From Top)
            SlidingGlassDraw1.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX + FixedGlassDraw1.BoundingBoxLength / 2 - cabin.Constraints.Overlap, CSCoordinate.Start);
            SlidingGlassDraw1.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.SliderDistance, CSCoordinate.Start);

            //CENTER AT (End of Sliding Glass - HandleDistance + MagnStrip , Mid of Sliding Glass )
            Handle1.SetCenterOrStartX(
                SlidingGlassDraw1.GetBoundingBoxRectangle().EndX 
                - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge()
                , CSCoordinate.Center);
            Handle1.SetCenterOrStartY(SlidingGlassDraw1.ShapeCenterY, CSCoordinate.Center);

            //At (End of Sliding Glass , Same Margin with SlidingGlass)
            MagnetStrip1.SetCenterOrStartX(SlidingGlassDraw1.ShapeCenterX + SlidingGlassDraw1.BoundingBoxLength / 2, CSCoordinate.Start);
            MagnetStrip1.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.SliderDistance, CSCoordinate.Start);

            //At (End of MagnetStrip1 , Same Margin with SlidingGlass)
            MagnetStrip2.SetCenterOrStartX(MagnetStrip1.ShapeCenterX + MagnetStrip1.BoundingBoxLength / 2, CSCoordinate.Start);
            MagnetStrip2.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.SliderDistance, CSCoordinate.Start);

            //At (End of MagnetStrip2 , Distance of Glass From Top)
            SlidingGlassDraw2.SetCenterOrStartX(MagnetStrip2.ShapeCenterX + MagnetStrip2.BoundingBoxLength / 2, CSCoordinate.Start);
            SlidingGlassDraw2.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.SliderDistance, CSCoordinate.Start);

            //CENTER AT (START of Sliding Glass2 + HandleDistance - MagnStrip , Mid of Sliding Glass )
            Handle2.SetCenterOrStartX(
                SlidingGlassDraw2.GetBoundingBoxRectangle().StartX 
                + cabin.Parts.Handle.GetHandleCenterDistanceFromEdge()
                , CSCoordinate.Center);
            Handle2.SetCenterOrStartY(SlidingGlassDraw2.ShapeCenterY, CSCoordinate.Center);

            //At (End of Sliding Glass2 - overlap , Distance of Glass From Top)
            FixedGlassDraw2.SetCenterOrStartX(SlidingGlassDraw2.ShapeCenterX + SlidingGlassDraw2.BoundingBoxLength / 2 - cabin.Constraints.Overlap, CSCoordinate.Start);
            FixedGlassDraw2.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.ThicknessView - cabin.Parts.HorizontalProfileTop.GlassInProfileDepth, CSCoordinate.Start);

            //At (0,0) IRRELEVANT NOT IMPEMENTED DRAW
            GlassAluminium2.SetCenterOrStartX(0, CSCoordinate.Start);
            GlassAluminium2.SetCenterOrStartY(0, CSCoordinate.Start);

            //At (End of Fixed Glass2 - ALST, 0)
            WallAluminium2.SetCenterOrStartX(FixedGlassDraw2.ShapeCenterX + FixedGlassDraw2.BoundingBoxLength / 2 - cabin.Parts.WallProfile2.GlassInProfileDepth, CSCoordinate.Start);
            WallAluminium2.SetCenterOrStartY(0, CSCoordinate.Start);
        }

        protected override void PlaceDrawNames()
        {
            WallAluminium1.Name = nameof(WallAluminium1);
            WallAluminium2.Name = nameof(WallAluminium2);
            GlassAluminium1.Name = nameof(GlassAluminium1);
            GlassAluminium2.Name = nameof(GlassAluminium2);
            if (StepWallAluminium1 != null && StepFloorAluminium1 != null && StepWallArea != null)
            {
                StepFloorAluminium1.Name = nameof(StepFloorAluminium1);
                StepWallAluminium1.Name = nameof(StepWallAluminium1);
                StepWallArea.Name = nameof(StepWallArea);
            }
            MagnetStrip1.Name = nameof(MagnetStrip1);
            MagnetStrip2.Name = nameof(MagnetStrip2);
            Handle1.Name = nameof(Handle1);
            Handle2.Name = nameof(Handle2);
            HorizontalAluminiumBottom.Name = nameof(HorizontalAluminiumBottom);
            HorizontalAluminiumTop.Name = nameof(HorizontalAluminiumTop);
            SlidingGlassDraw1.Name = nameof(SlidingGlassDraw1);
            SlidingGlassDraw2.Name = nameof(SlidingGlassDraw2);
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
            FixedGlassDraw2.Name = nameof(FixedGlassDraw2);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(WallAluminium2);
            draws.Add(GlassAluminium1);
            draws.Add(GlassAluminium2);
            if (StepWallAluminium1 != null && StepFloorAluminium1 != null && StepWallArea != null)
            {
                draws.Add(StepFloorAluminium1);
                draws.Add(StepWallAluminium1);
                draws.Add(StepWallArea);
            }
            draws.Add(MagnetStrip1);
            draws.Add(MagnetStrip2);
            draws.Add(Handle1);
            draws.Add(Handle2);
            draws.Add(HorizontalAluminiumBottom);
            draws.Add(HorizontalAluminiumTop);
            draws.Add(SlidingGlassDraw1);
            draws.Add(SlidingGlassDraw2);
            draws.Add(FixedGlassDraw1);
            draws.Add(FixedGlassDraw2);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }
        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(WallAluminium2);
            draws.Add(GlassAluminium1);
            draws.Add(GlassAluminium2);
            if (StepWallAluminium1 != null && StepFloorAluminium1 != null)
            {
                draws.Add(StepFloorAluminium1);
                draws.Add(StepWallAluminium1);
            }
            draws.Add(Handle1);
            draws.Add(Handle2);
            draws.Add(HorizontalAluminiumBottom);
            draws.Add(HorizontalAluminiumTop);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }
        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new()
            {
                SlidingGlassDraw1,
                SlidingGlassDraw2,
                FixedGlassDraw1,
                FixedGlassDraw2
            };

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }
        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(MagnetStrip1);
            draws.Add(MagnetStrip2);
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

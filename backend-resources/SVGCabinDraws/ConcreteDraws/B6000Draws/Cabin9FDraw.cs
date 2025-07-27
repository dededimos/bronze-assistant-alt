using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
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
    public class Cabin9FDraw : CabinDraw<Cabin9F>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;
        public DrawShape WallAluminium1 { get; set; }
        public DrawShape ConnectorlAluminium { get; set; }
        public DrawShape GlassAluminium1 { get; set; }
        public DrawShape HorizontalAluminiumTop { get; set; }
        public DrawShape HorizontalAluminiumBottom { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }

        /// <summary>
        /// The Step-Part of the Wall Aluminium
        /// </summary>
        public DrawShape StepWallAluminium1 { get; set; }
        public DrawShape StepFloorAluminium1 { get; set; }
        public DrawShape StepWallArea { get; set; }

        private Glass FixedGlass1 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.FixedGlass).FirstOrDefault(); }

        public Cabin9FDraw(Cabin9F cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            WallAluminium1 = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallProfile1.ThicknessView, 
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
                    cabin.Parts.WallProfile1.ThicknessView, 
                    cabin.Parts.WallProfile1.CutLengthStepPart);
                
                StepWallArea = DrawsFactory.HelperDraws.BuildWall(stepLength, stepHeight);
            }

            ConnectorlAluminium = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallProfile2.ThicknessView, 
                cabin.Parts.WallProfile2.CutLength);

            //Irrelevant
            GlassAluminium1 = DrawsFactory.BuildProfileDraw(0, 0);

            HorizontalAluminiumTop = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalProfileTop.CutLength, 
                cabin.Parts.HorizontalProfileTop.ThicknessView);
            HorizontalAluminiumTop.LayerNo = 1; //Drawn back of the rest Aluminiums  Because it enters inside AL3

            HorizontalAluminiumBottom = DrawsFactory.BuildProfileDraw(
                cabin.Parts.HorizontalProfileBottom.CutLength, 
                cabin.Parts.HorizontalProfileBottom.ThicknessView);
            HorizontalAluminiumBottom.LayerNo = 1; //Drawn back of the rest Aluminiums  Because it enters inside AL3

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass1);
            FixedGlassDraw1.LayerNo = 2; //Drawn to the back before sliding
        }

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
                //At (stepLength , EndY of AL1)
                StepWallAluminium1.SetCenterOrStartX(stepLength, CSCoordinate.Start);
                StepWallAluminium1.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY, CSCoordinate.Start);

                //At (0,EndY of AL1)
                StepWallArea.SetCenterOrStartX(0, CSCoordinate.Start);
                StepWallArea.SetCenterOrStartY(WallAluminium1.GetBoundingBoxRectangle().EndY, CSCoordinate.Start);
            }

            //At (End of Wall Aluminium , 0)
            HorizontalAluminiumTop.SetCenterOrStartX(cabin.Parts.WallProfile1.ThicknessView, CSCoordinate.Start);
            HorizontalAluminiumTop.SetCenterOrStartY(0, CSCoordinate.Start);

            //At (End of Wall Aluminium , Bottom of Cabin)
            HorizontalAluminiumBottom.SetCenterOrStartX(cabin.Parts.WallProfile1.ThicknessView + stepLength, CSCoordinate.Start);
            HorizontalAluminiumBottom.SetCenterOrStartY(cabin.Height - cabin.Parts.HorizontalProfileBottom.ThicknessView, CSCoordinate.Start);

            //At (InsideWall Aluminium ALST , Distance of Glass From Top)
            FixedGlassDraw1.SetCenterOrStartX(cabin.Parts.WallProfile1.ThicknessView - cabin.Parts.WallProfile1.GlassInProfileDepth, CSCoordinate.Start);
            FixedGlassDraw1.SetCenterOrStartY(cabin.Parts.HorizontalProfileTop.ThicknessView - cabin.Parts.HorizontalProfileTop.GlassInProfileDepth , CSCoordinate.Start);

            //At (End of Fixed Glass2 - ALST, 0)
            ConnectorlAluminium.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX + FixedGlassDraw1.BoundingBoxLength / 2 - cabin.Parts.WallProfile2.GlassInProfileDepth, CSCoordinate.Start);
            ConnectorlAluminium.SetCenterOrStartY(0, CSCoordinate.Start);

        }

        protected override void PlaceDrawNames()
        {
            WallAluminium1.Name = nameof(WallAluminium1);
            GlassAluminium1.Name = nameof(GlassAluminium1);
            if (StepWallAluminium1 != null && StepFloorAluminium1 != null && StepWallArea != null)
            {
                StepFloorAluminium1.Name = nameof(StepFloorAluminium1);
                StepWallAluminium1.Name = nameof(StepWallAluminium1);
                StepWallArea.Name = nameof(StepWallArea);
            }
            HorizontalAluminiumBottom.Name = nameof(HorizontalAluminiumBottom);
            HorizontalAluminiumTop.Name = nameof(HorizontalAluminiumTop);
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
            ConnectorlAluminium.Name = nameof(ConnectorlAluminium);
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(GlassAluminium1);
            if (StepWallAluminium1 != null && StepFloorAluminium1 != null && StepWallArea != null)
            {
                draws.Add(StepFloorAluminium1);
                draws.Add(StepWallAluminium1);
                draws.Add(StepWallArea);
            }
            draws.Add(HorizontalAluminiumBottom);
            draws.Add(HorizontalAluminiumTop);
            draws.Add(FixedGlassDraw1);
            draws.Add(ConnectorlAluminium);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetMetalFinishPartsDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(GlassAluminium1);
            if (StepWallAluminium1 != null && StepFloorAluminium1 != null)
            {
                draws.Add(StepFloorAluminium1);
                draws.Add(StepWallAluminium1);
            }
            draws.Add(HorizontalAluminiumBottom);
            draws.Add(HorizontalAluminiumTop);
            draws.Add(ConnectorlAluminium);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(FixedGlassDraw1);
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            return new List<DrawShape>();
        }

        public override List<DrawShape> GetHelperDraws()
        {
            List<DrawShape> draws = new();
            draws.AddNotNull(StepWallArea);
            return draws;
        }

    }
}

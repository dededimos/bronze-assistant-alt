using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.NBDraws
{
    public class CabinNBDraw : CabinDraw<CabinNB>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;

        public DrawShape WallAluminium1 { get; set; }
        public DrawShape RotatingProfile { get; set; }
        public DrawShape MagnetStrip { get; set; }
        public DrawShape MagnetAluminium { get; set; }
        public DrawShape DoorHandle1 { get; set; }
        public DrawShape DoorGlassDraw1 { get; set; }

        private Glass DoorGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.DoorGlass); }

        public CabinNBDraw(CabinNB cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            //Draw Two Profiles to Represent the HingeProfile
            WallAluminium1 = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallHinge.ThicknessView
                -cabin.Parts.WallHinge.InnerThicknessView,
                cabin.Parts.WallHinge.CutLength);
            RotatingProfile = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallHinge.InnerThicknessView,
                cabin.Parts.WallHinge.CutLength);

            DoorGlassDraw1 = DrawsFactory.BuildGlassDraw(DoorGlass,cabin.SynthesisModel);

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

            if (cabin.Parts.Handle is not null)
            {
                DoorHandle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            }

            DoorGlassDraw1.LayerNo = 1;
        }

        protected override void PlaceParts()
        {
            //Gets placed Before Y=0 , because of the extra Height of the Hinge
            WallAluminium1.SetCenterOrStartX(0,CSCoordinate.Start);
            WallAluminium1.SetCenterOrStartY(0,CSCoordinate.Start);

            RotatingProfile.SetCenterOrStartX(
                WallAluminium1.GetBoundingBoxRectangle().EndX,
                CSCoordinate.Start);
            RotatingProfile.SetCenterOrStartY(
                WallAluminium1.GetBoundingBoxRectangle().StartY, 
                CSCoordinate.Start);

            //At (EndX of AL1 - ALST , 0 )
            DoorGlassDraw1.SetCenterOrStartX(
                RotatingProfile.GetBoundingBoxRectangle().EndX 
                - cabin.Parts.WallHinge.GlassInProfileDepth, 
                CSCoordinate.Start);
            DoorGlassDraw1.SetCenterOrStartY(
                RotatingProfile.GetBoundingBoxRectangle().StartY
                + cabin.Parts.WallHinge.TopHeightAboveGlass
                , CSCoordinate.Start);

            if (MagnetStrip != null)
            {
                //At (EndX of Glass , StartY of Glass)
                MagnetStrip.SetCenterOrStartX(
                    DoorGlassDraw1.GetBoundingBoxRectangle().EndX, 
                    CSCoordinate.Start);
                MagnetStrip.SetCenterOrStartY(
                    DoorGlassDraw1.GetBoundingBoxRectangle().StartY + cabin.Constraints.CornerRadiusTopEdge,
                    CSCoordinate.Start);

                if (MagnetAluminium != null)
                {
                    //At (EndX of Strip , StartY of Strip)
                    MagnetAluminium.SetCenterOrStartX(
                        MagnetStrip.GetBoundingBoxRectangle().EndX, 
                        CSCoordinate.Start);
                    MagnetAluminium.SetCenterOrStartY(
                        MagnetStrip.GetBoundingBoxRectangle().StartY, 
                        CSCoordinate.Start);
                }
            }

            if (DoorHandle1 is not null)
            {
                //At (EndOf Glass - Distance from End , Middle of Glass)
                DoorHandle1.SetCenterOrStartX(
                    DoorGlassDraw1.GetBoundingBoxRectangle().EndX 
                    - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge(), 
                    CSCoordinate.Center);
                DoorHandle1.SetCenterOrStartY(
                    DoorGlassDraw1.ShapeCenterY, 
                    CSCoordinate.Center);
            }
        }

        protected override void PlaceDrawNames()
        {
            WallAluminium1.Name = nameof(WallAluminium1);
            RotatingProfile.Name = nameof(RotatingProfile);
            DoorGlassDraw1.Name = nameof(DoorGlassDraw1);
            
            if (MagnetStrip != null)
            {
                MagnetStrip.Name = nameof(MagnetStrip);
            }

            if (MagnetAluminium != null)
            {
                MagnetAluminium.Name = nameof(MagnetAluminium);
            }

            if (DoorHandle1 is not null)
            {
                DoorHandle1.Name = nameof(DoorHandle1);
            }
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(RotatingProfile);
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
            draws.Add(WallAluminium1);
            draws.Add(RotatingProfile);
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

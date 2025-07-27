using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
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
    public class CabinEDraw : CabinDraw<CabinE>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => 0;

        public DrawShape SupportBarLeft { get; set; }
        public DrawShape SupportBarRight { get; set; }
        public DrawShape FloorStopperLeft { get; set; }
        public DrawShape FloorStopperRight { get; set; }
        public DrawShape BottomProfile { get; set; }
        public DrawShape FixedGlassDraw1 { get; set; }
        private Glass FixedGlass { get => cabin.Glasses.FirstOrDefault(g => g.GlassType is GlassTypeEnum.FixedGlass); }

        public CabinEDraw(CabinE cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            SupportBarLeft = DrawsFactory.BuildSupportBarClamp(cabin.Parts.SupportBar);
            
            SupportBarRight = DrawsFactory.BuildSupportBarClamp(cabin.Parts.SupportBar);

            //According to which bottom fixer we have :
            switch (cabin.Parts.BottomFixer)
            {
                case FloorStopperW stopper:
                    FloorStopperLeft = DrawsFactory.BuildFloorStopper(stopper);
                    FloorStopperRight = DrawsFactory.BuildFloorStopper(stopper);
                    break;
                case Profile profile:
                    BottomProfile = DrawsFactory.BuildProfileDraw(
                        profile.CutLength, 
                        profile.ThicknessView);
                    break;
                default:
                    break;
            }

            FixedGlassDraw1 = DrawsFactory.BuildGlassDraw(FixedGlass);
            FixedGlassDraw1.LayerNo = 1;
        }

        protected override void PlaceParts()
        {
            FloorStopperW stopper = cabin.Parts.BottomFixer as FloorStopperW;
            bool hasStopper = stopper is not null;

            Profile floorProfile = cabin.Parts.BottomFixer as Profile;
            bool hasFloorProfile = floorProfile is not null;

            //At ( After the Thickness of Bottom Stopper, after the SupportBar or 0 if height is without the Support Bar)
            FixedGlassDraw1.SetCenterOrStartX(hasStopper ? stopper.OutOfGlassLength : 0 , CSCoordinate.Start);

            //If there is a correction the Fixed Glass will be  moved further down.
            FixedGlassDraw1.SetCenterOrStartY(cabin.Constraints.FinalHeightCorrection, CSCoordinate.Start);

            if (hasStopper)
            {
                // At (0,Bottom of the Glass - height of the floor stopper)
                FloorStopperLeft.SetCenterOrStartX(0, CSCoordinate.Start);
                FloorStopperLeft.SetCenterOrStartY(FixedGlassDraw1.ShapeCenterY + FixedGlassDraw1.BoundingBoxHeight / 2d - stopper.HeightView, CSCoordinate.Start);

                // At (Right most of the Glass minus How much the Stopper appears inside glass , Bottom of the Glass - height of the floor stopper)
                FloorStopperRight.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX + FixedGlassDraw1.BoundingBoxLength / 2d - (stopper.LengthView - stopper.OutOfGlassLength), CSCoordinate.Start);
                FloorStopperRight.SetCenterOrStartY(FixedGlassDraw1.ShapeCenterY + FixedGlassDraw1.BoundingBoxHeight / 2d - stopper.HeightView, CSCoordinate.Start);
            }
            if (hasFloorProfile)
            {
                BottomProfile.SetCenterOrStartX(FixedGlassDraw1.ShapeCenterX, CSCoordinate.Center);
                BottomProfile.SetCenterOrStartY(FixedGlassDraw1.GetBoundingBoxRectangle().EndY - floorProfile.ThicknessView, CSCoordinate.Start);  
            }

            // At (Glass Start Position + Distance of Support Bar from Glass , At Glass Start Y - Height of SupportBar)
            SupportBarLeft.SetCenterOrStartX(
                FixedGlassDraw1.GetBoundingBoxRectangle().StartX 
                + cabin.Parts.SupportBar.ClampCenterDistanceFromGlass
                , CSCoordinate.Center);
            SupportBarLeft.SetCenterOrStartY(
                FixedGlassDraw1.GetBoundingBoxRectangle().StartY 
                - cabin.Parts.SupportBar.OutOfGlassHeight 
                , CSCoordinate.Start);

            // At (Glass End Position - Distance of Support Bar from Glass , At Glass Start Y - Height of SupportBar)
            SupportBarRight.SetCenterOrStartX(
                FixedGlassDraw1.GetBoundingBoxRectangle().EndX 
                - cabin.Parts.SupportBar.ClampCenterDistanceFromGlass
                , CSCoordinate.Center);
            SupportBarRight.SetCenterOrStartY(
                FixedGlassDraw1.GetBoundingBoxRectangle().StartY 
                - cabin.Parts.SupportBar.OutOfGlassHeight
                , CSCoordinate.Start);

        }

        protected override void PlaceDrawNames()
        {
            SupportBarLeft.Name = nameof(SupportBarLeft);
            SupportBarRight.Name = nameof(SupportBarRight);
            if (FloorStopperLeft is not null && FloorStopperRight is not null)
            {
                FloorStopperLeft.Name = nameof(FloorStopperLeft);
                FloorStopperRight.Name = nameof(FloorStopperRight);
            }
            if (BottomProfile is not null)
            {
                BottomProfile.Name = nameof(BottomProfile);
            }
            FixedGlassDraw1.Name = nameof(FixedGlassDraw1);
        }


        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(SupportBarLeft);
            draws.Add(SupportBarRight);
            draws.AddNotNull(FloorStopperLeft);
            draws.AddNotNull(FloorStopperRight);
            draws.AddNotNull(BottomProfile);
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
            draws.Add(SupportBarLeft);
            draws.Add(SupportBarRight);
            draws.AddNotNull(FloorStopperLeft);
            draws.AddNotNull(FloorStopperRight);
            draws.AddNotNull(BottomProfile);
            return draws;
        }

        public override List<DrawShape> GetPolycarbonicsDraws()
        {
            return new();
        }

    }
}

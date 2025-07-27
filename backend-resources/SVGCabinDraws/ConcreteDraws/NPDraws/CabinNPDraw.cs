using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGDrawingLibrary.Models.DrawShape;

#nullable disable

namespace SVGCabinDraws.ConcreteDraws.NPDraws
{
    public class CabinNPDraw : CabinDraw<CabinNP>
    {
        public override CabinFinishEnum MetalFinish => cabin.MetalFinish ?? CabinFinishEnum.NotSet;
        public override double SingleDoorOpening => cabin.Opening;

        public DrawShape WallAluminium1 { get; set; }
        public DrawShape RotatingProfile { get; set; }
        
        /// <summary>
        /// The First Part of an Aluminium Profile Hinge
        /// </summary>
        public DrawShape MiddleProfileHinge1 { get; set; }
        /// <summary>
        /// The Second Part of an Aluminium Profile Hinge
        /// </summary>
        public DrawShape MiddleProfileHinge2 { get; set; }
        public DrawShape MagnetStrip { get; set; }
        public DrawShape MagnetAluminium { get; set; }
        public DrawShape DoorHandle1 { get; set; }
        public DrawShape HingeTop { get; set; }
        public DrawShape HingeBottom { get; set; }
        public DrawShape DoorGlassDraw1 { get; set; }
        public DrawShape DoorGlassDraw2 { get; set; }

        private Glass DoorGlass1 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.DoorGlass).FirstOrDefault(); }
        private Glass DoorGlass2 { get => cabin.Glasses.Where(g => g.GlassType is GlassTypeEnum.DoorGlass).Skip(1).FirstOrDefault(); }


        public CabinNPDraw(CabinNP cabin) : base(cabin) { }

        protected override void InitilizeDraw()
        {
            //Draw Two Profiles to Represent the HingeProfile
            WallAluminium1 = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallHinge.ThicknessView
                - cabin.Parts.WallHinge.InnerThicknessView,
                cabin.Parts.WallHinge.CutLength);
            RotatingProfile = DrawsFactory.BuildProfileDraw(
                cabin.Parts.WallHinge.InnerThicknessView,
                cabin.Parts.WallHinge.CutLength);

            DoorGlassDraw1 = DrawsFactory.BuildGlassDraw(DoorGlass1);
            DoorGlassDraw2 = DrawsFactory.BuildGlassDraw(DoorGlass2);

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

            if (cabin.Parts.MiddleHinge is CabinHinge hinge)
            {
                HingeTop = DrawsFactory.HingeDraws.BuildHingeNew(hinge);
                HingeBottom = DrawsFactory.HingeDraws.BuildHingeNew(hinge);
            }
            else if (cabin.Parts.MiddleHinge is ProfileHinge profileHinge)
            {
                //The Profile Hinge gets divided in two Pieces
                MiddleProfileHinge1 = DrawsFactory.BuildProfileDraw(
                    profileHinge.ThicknessView/2d,
                    profileHinge.CutLength);
                MiddleProfileHinge2 = DrawsFactory.BuildProfileDraw(
                    profileHinge.ThicknessView/2d,
                    profileHinge.CutLength);
            }

            if (cabin.Parts.Handle is not null)
            {
                DoorHandle1 = DrawsFactory.HandleDraws.BuildHandleNew(cabin.Parts.Handle);
            }

            DoorGlassDraw1.LayerNo = 1;
            DoorGlassDraw2.LayerNo = 1;
        }

        protected override void PlaceParts()
        {
            //Gets placed Before Y=0 , because of the extra Height of the Hinge
            WallAluminium1.SetCenterOrStartX(0, CSCoordinate.Start);
            WallAluminium1.SetCenterOrStartY(0, CSCoordinate.Start);
            
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
                + cabin.Parts.WallHinge.TopHeightAboveGlass,
                CSCoordinate.Start);

            double doorGlass1StartX = DoorGlassDraw1.GetBoundingBoxRectangle().StartX;
            double doorGlass1StartY = DoorGlassDraw1.GetBoundingBoxRectangle().StartY;
            double doorGlass1EndX = DoorGlassDraw1.GetBoundingBoxRectangle().EndX;
            double doorGlass1EndY = DoorGlassDraw1.GetBoundingBoxRectangle().EndY;

            if (cabin.Parts.MiddleHinge is CabinHinge hinge)
            {
                //At (EndX of Glass1 - The Distance Inside the Glass , StartY of Glass1 + Distance from Top)
                HingeTop.SetCenterOrStartX(
                    doorGlass1EndX 
                    - (hinge.LengthView - hinge.GlassGapAER)/2d,
                    CSCoordinate.Start);
                HingeTop.SetCenterOrStartY(
                    doorGlass1StartY 
                    + GlassProcessesConstants.ProcessesNP.HingeHoleTopDistanceDP1,
                    CSCoordinate.Center);

                //At (EndX of Glass1 - The Distance Inside the Glass , EndY of Glass1 - Distance from Bottom)
                HingeBottom.SetCenterOrStartX(
                    doorGlass1EndX
                    - (hinge.LengthView - hinge.GlassGapAER) / 2d,
                    CSCoordinate.Start);
                HingeBottom.SetCenterOrStartY(
                    doorGlass1EndY 
                    - GlassProcessesConstants.ProcessesNP.HingeHoleBottomDistanceDP1,
                    CSCoordinate.Center);

                //At (EndX of Glass1 + GAP , StartY of Glass1)
                DoorGlassDraw2.SetCenterOrStartX(
                    doorGlass1EndX 
                    + hinge.GlassGapAER, 
                    CSCoordinate.Start);
                DoorGlassDraw2.SetCenterOrStartY(
                    doorGlass1StartY, 
                    CSCoordinate.Start);
            }
            else if(cabin.Parts.MiddleHinge is ProfileHinge profileHinge)
            {
                //At (EndX of Glass1 , StartY of Glass1)
                MiddleProfileHinge1.SetCenterOrStartX(
                    doorGlass1EndX
                    -profileHinge.GlassInProfileDepth, 
                    CSCoordinate.Start);
                MiddleProfileHinge1.SetCenterOrStartY(
                    doorGlass1StartY
                    -profileHinge.TopHeightAboveGlass, 
                    CSCoordinate.Start);

                //At (EndX of Glass1 + Length of Previous Connector , StartY of Glass1)
                MiddleProfileHinge2.SetCenterOrStartX(
                    MiddleProfileHinge1.GetBoundingBoxRectangle().EndX, 
                    CSCoordinate.Start);
                MiddleProfileHinge2.SetCenterOrStartY(
                    doorGlass1StartY
                    - profileHinge.TopHeightAboveGlass, 
                    CSCoordinate.Start);

                //At (EndX of Second Connector , StartY of Glass1)
                DoorGlassDraw2.SetCenterOrStartX(
                    MiddleProfileHinge2.GetBoundingBoxRectangle().EndX
                    - profileHinge.GlassInProfileDepth, 
                    CSCoordinate.Start);
                DoorGlassDraw2.SetCenterOrStartY(
                    doorGlass1StartY, 
                    CSCoordinate.Start);
            }

            double doorGlass2StartX = DoorGlassDraw2.GetBoundingBoxRectangle().StartX;
            double doorGlass2StartY = DoorGlassDraw2.GetBoundingBoxRectangle().StartY;
            double doorGlass2EndX = DoorGlassDraw2.GetBoundingBoxRectangle().EndX;
            double doorGlass2EndY = DoorGlassDraw2.GetBoundingBoxRectangle().EndY;

            if (DoorHandle1 is not null)
            {
                //At (End Of Glass - Distance from End , Middle of Glass)
                DoorHandle1.SetCenterOrStartX(
                    doorGlass2EndX 
                    - cabin.Parts.Handle.GetHandleCenterDistanceFromEdge(), 
                    CSCoordinate.Center);
                DoorHandle1.SetCenterOrStartY(
                    DoorGlassDraw2.ShapeCenterY, 
                    CSCoordinate.Center);
            }

            if (MagnetStrip != null)
            {
                //At (EndX of Glass2 , StartY of Glass2
                MagnetStrip.SetCenterOrStartX(
                    doorGlass2EndX, 
                    CSCoordinate.Start);
                MagnetStrip.SetCenterOrStartY(
                    doorGlass2StartY, 
                    CSCoordinate.Start);
                
                if (MagnetAluminium != null)
                {
                    //At (EndX of Magnet Strip , Start Y of Door Glass)
                    MagnetAluminium.SetCenterOrStartX(
                        MagnetStrip.GetBoundingBoxRectangle().EndX, 
                        CSCoordinate.Start);
                    MagnetAluminium.SetCenterOrStartY(
                        doorGlass2StartY, 
                        CSCoordinate.Start);
                }
            }
        }

        protected override void PlaceDrawNames()
        {
            WallAluminium1.Name = nameof(WallAluminium1);
            RotatingProfile.Name = nameof(RotatingProfile);
            DoorGlassDraw1.Name = nameof(DoorGlassDraw1);
            DoorGlassDraw2.Name = nameof(DoorGlassDraw2);

            if (DoorHandle1 is not null)
            {
                DoorHandle1.Name = nameof(DoorHandle1);
            }

            if (MagnetStrip != null)
            {
                MagnetStrip.Name = nameof(MagnetStrip);
            }

            if (MagnetAluminium != null)
            {
                MagnetAluminium.Name = nameof(MagnetAluminium);
            }
            if (HingeTop != null && HingeBottom != null)
            {
                HingeTop.Name = nameof(HingeTop);
                HingeBottom.Name = nameof(HingeBottom);
            }
            if (MiddleProfileHinge1 != null && MiddleProfileHinge2 != null)
            {
                MiddleProfileHinge1.Name = nameof(MiddleProfileHinge1);
                MiddleProfileHinge2.Name = nameof(MiddleProfileHinge2);
            }
        }

        public override List<DrawShape> GetAllDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(WallAluminium1);
            draws.Add(RotatingProfile);
            draws.AddNotNull(DoorHandle1);
            draws.Add(DoorGlassDraw1);
            draws.Add(DoorGlassDraw2);
            draws.AddNotNull(MagnetStrip);
            draws.AddNotNull(MagnetAluminium);
            draws.AddNotNull(HingeTop);
            draws.AddNotNull(HingeBottom);
            draws.AddNotNull(MiddleProfileHinge1);
            draws.AddNotNull(MiddleProfileHinge2);

            //Arrange Draw Order according to LayerNo (Layers at '0' drwan Last)
            draws = draws.OrderByDescending(d => d.LayerNo).ToList();
            //Will return list only if Available otherwise returns an Empty List
            return draws;
        }

        public override List<DrawShape> GetGlassesDraws()
        {
            List<DrawShape> draws = new();
            draws.Add(DoorGlassDraw1);
            draws.Add(DoorGlassDraw2);
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
            draws.AddNotNull(HingeTop);
            draws.AddNotNull(HingeBottom);
            draws.AddNotNull(MiddleProfileHinge1);
            draws.AddNotNull(MiddleProfileHinge2);
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

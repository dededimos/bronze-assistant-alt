using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using SVGCabinDraws.Enums;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGCabinDraws.ConcreteDraws;
using SVGCabinDraws.ConcreteDraws.B6000Draws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using SVGCabinDraws.ConcreteDraws.DBDraws;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using SVGCabinDraws.ConcreteDraws.FreeDraws;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using SVGCabinDraws.ConcreteDraws.Inox304Draws;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using SVGCabinDraws.ConcreteDraws.HBDraws;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using SVGCabinDraws.ConcreteDraws.NBDraws;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using SVGCabinDraws.ConcreteDraws.NPDraws;
using SVGCabinDraws.ConcreteDraws.WSDraws;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.AngleModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;

#nullable disable

namespace SVGCabinDraws
{
    /// <summary>
    /// Contains Methods to Create Various Cabin Part Draws 
    /// </summary>
    public static class DrawsFactory
    {

        /// <summary>
        /// Builds the Draw of a Cabin
        /// </summary>
        /// <param name="cabin">The Cabin that needs Drawing</param>
        /// <returns>The Draw or Null</returns>
        public static CabinDraw BuildCabinDraw(Cabin cabin)
        {
            ArgumentNullException.ThrowIfNull(cabin);
            return cabin.Model switch
            {
                CabinModelEnum.Model9A => new Cabin9ADraw((Cabin9A)cabin),
                CabinModelEnum.Model9S => new Cabin9SDraw((Cabin9S)cabin),
                CabinModelEnum.Model94 => new Cabin94Draw((Cabin94)cabin),
                CabinModelEnum.Model9F => new Cabin9FDraw((Cabin9F)cabin),
                CabinModelEnum.Model9B => new Cabin9BDraw((Cabin9B)cabin),
                CabinModelEnum.ModelDB => new CabinDBDraw((CabinDB)cabin),
                CabinModelEnum.ModelE => new CabinEDraw((CabinE)cabin),
                CabinModelEnum.ModelW => new CabinWDraw((CabinW)cabin),
                CabinModelEnum.ModelWFlipper => new CabinWFlipperDraw((CabinWFlipper)cabin),
                CabinModelEnum.ModelVS => new CabinVSDraw((CabinVS)cabin),
                CabinModelEnum.ModelV4 => new CabinV4Draw((CabinV4)cabin),
                CabinModelEnum.ModelVF => new CabinVFDraw((CabinVF)cabin),
                CabinModelEnum.ModelVA => new CabinVADraw((CabinVA)cabin),
                CabinModelEnum.ModelHB => new CabinHBDraw((CabinHB)cabin),
                CabinModelEnum.ModelNB => new CabinNBDraw((CabinNB)cabin),
                CabinModelEnum.ModelQB => new CabinNBDraw((CabinNB)cabin),
                CabinModelEnum.ModelNP => new CabinNPDraw((CabinNP)cabin),
                CabinModelEnum.ModelQP => new CabinNPDraw((CabinNP)cabin),
                CabinModelEnum.ModelWS => new CabinWSDraw((CabinWS)cabin),
                CabinModelEnum.Model8W40 => new CabinWDraw((CabinW)cabin),
                CabinModelEnum.ModelNV => new CabinNBDraw((CabinNB)cabin),
                CabinModelEnum.ModelMV2 => new CabinNPDraw((CabinNP)cabin),
                CabinModelEnum.ModelNV2 => new CabinNPDraw((CabinNP)cabin),
                CabinModelEnum.Model9C => null,
                _ => new EmptyCabinDraw(cabin),
                // Substituted this for the below one
                //throw new InvalidOperationException($"Draw was not generated - Unrecognized {nameof(CabinModelEnum)}=>{cabin.Model}")
            };
        }

        /// <summary>
        /// Returns the 2d Front Shape of an Aluminium Profile
        /// </summary>
        /// <param name="length">The Length of the Profile</param>
        /// <param name="height">The Height of the Profile</param>
        /// <returns>The DrawShape of an Aluminium Profile</returns>
        public static DrawShape BuildProfileDraw(double length, double height)
        {
            RectangleDraw rect = new RectangleDraw(length,height);
            return rect;
        }

        /// <summary>
        /// Returns the 2d Front Draw of a Glass
        /// </summary>
        /// <param name="glass">The Glass to Draw</param>
        /// <param name="radiusCut">The Radius The The top Right Corner Should Get</param>
        /// <param name="shouldRoundTwoCorners">Wheather the Top left Corner Should be Also Rounded</param>
        /// <returns>The Glass Draw</returns>
        public static DrawShape BuildGlassDraw(Glass glass,CabinSynthesisModel synthesisModel = CabinSynthesisModel.Primary)
        {
            CornersToRound cornersToRound;
            int cornerRadius;

            if (glass.CornerRadiusTopLeft != 0 && glass.CornerRadiusTopRight != 0)
            {
                cornersToRound = CornersToRound.UpperCorners;
                cornerRadius = glass.CornerRadiusTopRight;
            }
            else if (glass.CornerRadiusTopRight != 0)
            {
                //Reverse the Rounding if its not Primary - because the Draw is not actually flipped - it is translated (because its supposed to be symmetrical)
                cornersToRound = synthesisModel is CabinSynthesisModel.Primary ? CornersToRound.TopRightCorner : CornersToRound.TopLeftCorner;
                cornerRadius = glass.CornerRadiusTopRight;
            }
            else if (glass.CornerRadiusTopLeft != 0)
            {
                cornersToRound = synthesisModel is CabinSynthesisModel.Primary ? CornersToRound.TopLeftCorner : CornersToRound.TopRightCorner;
                cornerRadius = glass.CornerRadiusTopLeft;
            }
            else
            {
                cornersToRound = CornersToRound.None;
                cornerRadius = 0;
            }
            RectangleDraw glassRectangle = new(glass.Length, glass.Height, cornersToRound, cornerRadius);
            
            return glassRectangle;
        }

        /// <summary>
        /// Returns the Draw of a Magnet Strip
        /// </summary>
        /// <param name="length">The Length/Width of the Strip</param>
        /// <param name="height">The Height of the Strip - Usually the same as the glass it fits</param>
        /// <returns></returns>
        public static DrawShape BuildStripDraw(double length , double height)
        {
            RectangleDraw strip = new(length, height);
            return strip;
        }

        /// <summary>
        /// Returns the Draw of the Angle of a Cabin9A
        /// </summary>
        /// <param name="angleDistance"></param>
        /// <returns></returns>
        public static DrawShape BuildAngleConnector(CabinAngle angle)
        {
            RectangleDraw rect = new RectangleDraw(angle.AngleLengthL0, angle.AngleHeight);
            return rect;
        }

        /// <summary>
        /// Builds the Clamp of a Support Bar
        /// </summary>
        /// <returns></returns>
        public static DrawShape BuildSupportBarClamp(SupportBar supportBar)
        {
            RectangleDraw clamp = new(supportBar.ClampViewLength,
                                      supportBar.ClampViewHeight);
            return clamp;
        }

        /// <summary>
        /// Returns the Draw of the Floor Stopper used in 8W and 8E Glasses
        /// </summary>
        /// <returns></returns>
        public static DrawShape BuildFloorStopper(FloorStopperW floorStopper)
        {
            RectangleDraw stopperDraw = new(floorStopper.LengthView,
                                            floorStopper.HeightView);
            return stopperDraw;
        }

        /// <summary>
        /// Defines Methods to Build Handle Draws
        /// </summary>
        public static class HandleDraws
        {
            public static DrawShape BuildHandleNew(CabinHandle handle)
            {
                //Without Hole
                if (handle.HandleOuterThickness == 0)
                {
                    RectangleDraw handleDraw = new(handle.HandleLengthToGlass,
                                               handle.HandleHeightToGlass,
                                               CornersToRound.All,
                                               handle.HandleEdgesCornerRadius);
                    return handleDraw;
                }
                //With Hole in Middle (Rectangle shape or Circular According to passed Parameters)
                else
                {
                    HoledRectangleDraw handleDraw = new(
                        handle.HandleLengthToGlass,
                        handle.HandleHeightToGlass,
                        handle.HandleOuterThickness,
                        CornersToRound.All,
                        handle.HandleEdgesCornerRadius);

                    return handleDraw;
                }
                
            }
            /// <summary>
            /// Returns the Shape of a B6000 Default Handle
            /// </summary>
            /// <returns>Draw Shape of Handle B6000 Default</returns>
            public static DrawShape BuildB6000Handle()
            {
                RectangleDraw handle = new(CabinPartsDimensions.Handles.B6000HandleWidth, 
                                           CabinPartsDimensions.Handles.B6000HandleHeight,
                                           CornersToRound.All, 
                                           CabinPartsDimensions.Handles.B6000HandleCornerRadius);
                return handle;
            }
            /// <summary>
            /// Returns the Shape of a Simple Knob ABS Handle
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildKnobABSHandle()
            {
                CircleDraw handle = new(CabinPartsDimensions.Handles.KnobABSHandleDiameter);
                return handle;
            }
            /// <summary>
            /// Returns the Draw of a Holed Handle
            /// </summary>
            /// <returns></returns>
            public static DrawShape HoledHandle()
            {
                HoledCircleDraw handle = new(CabinPartsDimensions.Handles.HoledHandleDiameter,
                                             CabinPartsDimensions.Handles.HoledHandleInnerDiameter);
                return handle;
            }
        }

        /// <summary>
        /// Defines Methods to Build Hinge Draws
        /// </summary>
        public static class HingeDraws
        {
            /// <summary>
            /// Returns the Draw of a 9B Hinge
            /// </summary>
            /// <returns></returns>
            public static DrawShape Build9BHinge(HingePosition hingePosition,Hinge9B hinge9B)
            {
                //Fillet upper side of Hinge when on top , else Bottom area of hinge
                CornersToRound cornersToBeRounded = hingePosition is HingePosition.Top ? CornersToRound.BottomCorners : CornersToRound.UpperCorners;

                RectangleDraw rect = new RectangleDraw(hinge9B.LengthView,
                                                       hinge9B.HeightView,
                                                       cornersToBeRounded,
                                                       hinge9B.CornerRadiusInGlass);
                return rect;
            }

            /// <summary>
            /// Returns the Draw of the Small Tube Support of 9BHinge
            /// </summary>
            /// <returns></returns>
            public static DrawShape Build9BHingeSupport(Hinge9B hinge9B)
            {
                RectangleDraw rect = new (
                    hinge9B.SupportTubeLength,
                    hinge9B.SupportTubeHeight);
                return rect;
            }

            /// <summary>
            /// Build a Simple Rectangle from A Hinge Object
            /// </summary>
            /// <param name="hinge">The Hinge</param>
            /// <returns>The Rectangle DrawShape</returns>
            public static DrawShape BuildHingeNew(CabinHinge hinge)
            {
                RectangleDraw hingeDraw = new(hinge.LengthView,
                                              hinge.HeightView);
                return hingeDraw;
            }

            /// <summary>
            /// Build The Main Body of a DB Hinge (The Rotating Part)
            /// </summary>
            /// <param name="hingeDB">The Hinge Object</param>
            /// <returns>The Rectangle of the Main Body</returns>
            public static DrawShape BuildHingeDBMainBody(HingeDB hingeDB)
            {
                RectangleDraw hingeDraw = new(hingeDB.LengthView - hingeDB.WallPlateThicknessView,
                                              hingeDB.InnerHeight);
                return hingeDraw;
            }

            /// <summary>
            /// Build the Wall Plate of a DB Hinge
            /// </summary>
            /// <param name="hingeDB">The hinge Object</param>
            /// <returns>The Rectangle of the Wall Part</returns>
            public static DrawShape BuildHingeDBWallPlate(HingeDB hingeDB)
            {
                RectangleDraw hingeDraw = new(hingeDB.WallPlateThicknessView,
                                              hingeDB.HeightView);
                return hingeDraw;
            }

            /// <summary>
            /// Returns the Draw of the Wall to Glass Heavy Duty Hinge
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildDBHinge()
            {
                RectangleDraw hinge = new(CabinPartsDimensions.Hinges.WallToGlassHingeWidth,
                                          CabinPartsDimensions.Hinges.WallToGlassHingeHeight);
                return hinge;
            }

            /// <summary>
            /// Returns the Draw of the Glass To Glass Heavy Duty Hinge
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildHBHinge() 
            {
                RectangleDraw hinge = new(CabinPartsDimensions.Hinges.GlassToGlassHingeWidth,
                                          CabinPartsDimensions.Hinges.GlassToGlassHingeHeight);
                return hinge;
            }

            /// <summary>
            /// Returns the Draw of the Glass To Glass NP Hinge
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildNPHinge()
            {
                RectangleDraw hinge = new(CabinPartsDimensions.Hinges.GlassToGlassNPHingeWidth,
                                          CabinPartsDimensions.Hinges.GlassToGlassNPHingeHeight);
                return hinge;
            }

            /// <summary>
            /// Returns the Draw of a GlassToGlass small Angular Support
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildGlassToGlassSupport()
            {
                RectangleDraw support = new(CabinPartsDimensions.Hinges.GlassToGlassSupportLength,
                                            CabinPartsDimensions.Hinges.GlassToGlassSupportHeight);
                return support;
            }

            /// <summary>
            /// Build the Draw of a cabin Support
            /// </summary>
            /// <param name="support">The Support Object</param>
            /// <returns>The Support Rectangle</returns>
            public static DrawShape BuildGlassSupport(CabinSupport support)
            {
                RectangleDraw supportDraw = new(support.LengthView,
                                                support.HeightView);
                return supportDraw;
            }

            /// <summary>
            /// Returns the Draw of a Flipper Hinge
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildFlipperHinge()
            {
                RectangleDraw hinge = new(CabinPartsDimensions.Hinges.FlipperHingeLength,
                                          CabinPartsDimensions.Hinges.FlipperHingeHeight);
                return hinge;
            }
        }

        /// <summary>
        /// Defines Methods to Build Various Inox 304 Parts Draws
        /// </summary>
        public static class Inox304Parts
        {
            /// <summary>
            /// Returns the Draw of an Inox304 Wheel
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildWheel()
            {
                CircleDraw wheel = new(CabinPartsDimensions.Inox304Parts.WheelDiameter);
                return wheel;
            }

            /// <summary>
            /// Returns the Draw of an Inox304 Wheel Lock
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildWheelLock()
            {
                CircleDraw wheelLock = new(CabinPartsDimensions.Inox304Parts.WheelLockDiameter);
                return wheelLock;
            }

            /// <summary>
            /// Returns the Draw of an Inox304 Fixed Glass Lock
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildFixedGlassLock()
            {
                CircleDraw fixLock = new(CabinPartsDimensions.Inox304Parts.FixedLockDiameter);
                return fixLock;
            }

            /// <summary>
            /// Returns the Draw of an Inox304 Guide Part
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildGuide()
            {
                RectangleDraw guide = new(CabinPartsDimensions.Inox304Parts.GuideFrontWidth,
                                          CabinPartsDimensions.Inox304Parts.GuideFrontHeight);
                return guide;
            }

            /// <summary>
            /// Returns the Draw of an Inox304's Main Bar
            /// </summary>
            /// <param name="length">The Bar's Length</param>
            /// <returns>The Main Bar Draw</returns>
            public static DrawShape BuildMainBar(double length)
            {
                RectangleDraw mainBar = new(length,CabinPartsDimensions.Inox304Parts.BarHeight);
                return mainBar;
            }

            /// <summary>
            /// Returns the Draw of an Inox304's Wall Support Clamp
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildWallSupport()
            {
                RectangleDraw wallSupport = new(CabinPartsDimensions.Inox304Parts.WallSupportWidth,
                                                CabinPartsDimensions.Inox304Parts.WallSupportHeight);
                return wallSupport;
            }

            /// <summary>
            /// Returns the Draw of an Inox304's Door Stopper
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildDoorStopper() 
            {
                RectangleDraw doorStopper = new(CabinPartsDimensions.Inox304Parts.DoorStopperWidth,
                                                CabinPartsDimensions.Inox304Parts.DoorStopperHeight);
                return doorStopper;
            }

            /// <summary>
            /// Returns the Draw of an Inox304's Stopper's Bumper
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildStopperBumper()
            {
                RectangleDraw bumper = new(CabinPartsDimensions.Inox304Parts.StopperBumperRubberLength,
                                           CabinPartsDimensions.Inox304Parts.StopperBumperRubberHeight);
                bumper.Fill = "black";
                return bumper;
            }

            /// <summary>
            /// Returns the Draw of an Inox304's Bar Base
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildBarBase()
            {
                RectangleDraw barBase = new(CabinPartsDimensions.Inox304Parts.BarBaseWidth,
                                            CabinPartsDimensions.Inox304Parts.BarBaseHeight);
                return barBase;
            }

            /// <summary>
            /// Returns the Draw of an Inox304's Support for the Base of the Bar -- Placed on a VF Fixed Glass
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildBarBaseFixedGlassSupport()
            {
                RectangleDraw support = new(CabinPartsDimensions.Inox304Parts.BarBaseDepth,
                                            CabinPartsDimensions.Inox304Parts.BarBaseHeight);
                return support;
            }
        }

        public static class SmartInoxParts
        {
            /// <summary>
            /// Builds the Metal Part of the Door Stopper for a Smart Inox Model
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildDoorStopper()
            {
                RectangleDraw doorStopper = new(CabinPartsDimensions.SmartInoxParts.DoorStopperWidth - CabinPartsDimensions.SmartInoxParts.DoorStopperBumperWidth,
                                                CabinPartsDimensions.SmartInoxParts.DoorStopperHeight);
                return doorStopper;
            }

            /// <summary>
            /// Returns the Bumper Part of the Door Stopper of a Smart Inox Model
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildStopperBumper()
            {
                RectangleDraw bumper = new(CabinPartsDimensions.SmartInoxParts.DoorStopperBumperWidth,
                                           CabinPartsDimensions.SmartInoxParts.DoorStopperBumperHeight);
                bumper.Fill = "black";
                return bumper;
            }

            /// <summary>
            /// Returns the Draw of a SmartInox Wheel
            /// </summary>
            /// <returns></returns>
            public static DrawShape BuildWheel()
            {
                CircleDraw wheel = new(CabinPartsDimensions.SmartInoxParts.WheelDiameter);
                return wheel;
            }

        }

        /// <summary>
        /// Defines Methods To Build Various Helper Draws (LikeWalls)
        /// </summary>
        public static class HelperDraws
        {
            /// <summary>
            /// Builds a wall Rectangle
            /// </summary>
            /// <param name="length">Length of Wall</param>
            /// <param name="height">Height of Wall</param>
            /// <returns></returns>
            public static DrawShape BuildWall(double length , double height)
            {
                RectangleDraw wall = new(length, height);
                return wall;
            }

            /// <summary>
            /// Returns a Line with Arrows representing a Dimension (Horizontal or Vertical)
            /// The Consumer of this Object should Place the StartX , StartY of the DImension
            /// </summary>
            /// <param name="size"></param>
            /// <param name="isHorizontal"></param>
            /// <returns></returns>
            public static DrawShape BuildDimension(double size , bool isHorizontal)
            {
                DimensionLineDraw dimension = new();
                dimension.StartX = 0;
                dimension.StartY = 0;
                dimension.EndX = isHorizontal ? size : 0;
                dimension.EndY = isHorizontal ? 0 : size;
                dimension.ArrowThickness = 20;
                dimension.ArrowLength = 30;
                dimension.Stroke = "var(--customDark)";
                dimension.Fill = "var(--customDark)";
                dimension.Name = isHorizontal ? DrawShape.LENGTHDIM : DrawShape.HEIGHTDIM;
                dimension.AngleWithAxisX = isHorizontal ? 0 : 90;
                return dimension;
            }
        }
    }
}

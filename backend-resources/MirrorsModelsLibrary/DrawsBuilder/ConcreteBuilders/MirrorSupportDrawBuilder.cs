using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Helpers;
using MirrorsModelsLibrary.DrawsBuilder.Models.DrawShapes.MirrorSpecificDraws;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder.ConcreteBuilders
{
    /// <summary>
    /// The Builder for the Mirrors Supports and Frames . Builds a Support and Places it at the Default Position
    /// </summary>
    public class MirrorSupportDrawBuilder : IDrawShapeBuilder
    {
        DrawShape supportDraw;
        DrawShape supportBoundary;

        private double supportBoundaryLength;       //The Length of the Support
        private double supportBoundaryHeight;       //The Height of the Support
        private double supportBoundaryDiameter;     //The Diameter of the Support
        private double supportBoundaryCenterX;
        private double supportBoundaryCenterY;

        private DrawnSide? sideToDraw;           //Which Side is being Drawn
        private MirrorSupport? supportToDraw;    //Which Support should be Drawn
        private MirrorShape? mirrorShape;       //The Shape of the Mirror
        private bool hasLight;                  //Wheather the Mirror has Light (Used in Circular Mirrors to Build Frame for Light

        public MirrorSupportDrawBuilder(MirrorSupport? supportToDraw,
                                           DrawnSide sideToDraw,
                                           DrawShape supportBoundary , 
                                           MirrorShape shape)
        {
            if (shape is MirrorShape.Rectangular or MirrorShape.Capsule)
            {
                RectangleDraw boundary = supportBoundary as RectangleDraw;
                supportBoundaryLength = boundary.Length;
                supportBoundaryHeight = boundary.Height;
                supportBoundaryCenterX = boundary.ShapeCenterX;
                supportBoundaryCenterY = boundary.ShapeCenterY;
            }
            else if(shape is MirrorShape.Ellipse)
            {
                EllipseDraw boundary = supportBoundary as EllipseDraw;
                supportBoundaryLength = boundary.Length;
                supportBoundaryHeight = boundary.Height;
                supportBoundaryCenterX = boundary.ShapeCenterX;
                supportBoundaryCenterY = boundary.ShapeCenterY;
            }
            else
            {
                throw new NotSupportedException("Current SupportDrawBuilder Constructor is only available for Capsule,Recangular or Ellipse Shapes");
            }
            this.supportBoundary = supportBoundary;
            this.supportToDraw = supportToDraw;
            this.sideToDraw = sideToDraw;
            mirrorShape = shape;
        }

        public MirrorSupportDrawBuilder(MirrorSupport? supportToDraw,
                                           DrawnSide sideToDraw,
                                           DrawShape supportBoundary,
                                           bool hasLight)
        {
            CircleDraw boundary = supportBoundary as CircleDraw;
            supportBoundaryDiameter = boundary.Diameter;
            supportBoundaryCenterX = boundary.ShapeCenterX;
            supportBoundaryCenterY = boundary.ShapeCenterY;
            this.supportToDraw = supportToDraw;
            this.sideToDraw = sideToDraw;
            mirrorShape = MirrorShape.Circular;
            this.hasLight = hasLight;
        }

        /// <summary>
        /// Builds the Shape of the Mirror Support
        /// </summary>
        /// <returns>Null if there is no Support</returns>
        public DrawShape BuildShape()
        {
            switch (supportToDraw)
            {
                case MirrorSupport.Double:
                    BuildDoubleSupport();
                    break;
                case MirrorSupport.Perimetrical:
                    BuildPerimetricalSupport();
                    break;
                case MirrorSupport.Frame:
                    BuildVisibleFrame();
                    break;
                case MirrorSupport.FrontSupports:
                case MirrorSupport.Without:
                default:
                    supportDraw = null;
                    break;
            }

            if (supportDraw is not null)
            {
                //All the Supports Are Placed to the Center of the SupportBoundary

                supportDraw.SetCenterOrStartX(supportBoundaryCenterX, DrawShape.CSCoordinate.Center);
                supportDraw.SetCenterOrStartY(supportBoundaryCenterY, DrawShape.CSCoordinate.Center);
                DrawShape shapeToReturn = supportDraw.CloneSelf();
                if (sideToDraw is DrawnSide.Rear)
                {
                    shapeToReturn.Fill = "URL(#hatchPattern)";
                    shapeToReturn.Stroke = "black";
                    shapeToReturn.Name = "Support";
                }
                else
                {
                    shapeToReturn.Fill = "black";
                    shapeToReturn.Stroke = "ghostwhite";
                    shapeToReturn.Name = "Support";
                }

                ResetBuilder();
                return shapeToReturn;
            }
            else
            {
                ResetBuilder();
                return null;
            }
        }

        /// <summary>
        /// Resets the Builder Object
        /// </summary>
        public void ResetBuilder()
        {
            supportDraw = null;
            supportToDraw = null;
            sideToDraw = null;
            mirrorShape = null;
            supportBoundaryHeight = 0;
            supportBoundaryLength = 0;
            supportBoundaryDiameter = 0;
            supportBoundaryCenterX = 0;
            supportBoundaryCenterY = 0;

            hasLight = false;
        }

        #region 1.Build Visible Frame Methods
        private void BuildVisibleFrame()
        {
            switch (mirrorShape)
            {
                case MirrorShape.Rectangular:
                    BuildRectangularVisibleFrame();
                    break;
                case MirrorShape.Circular:
                    BuildCircularVisibleFrame();
                    break;
                case MirrorShape.Special:
                    throw new NotImplementedException("Special Mirror Shape Not Implemented -- Frame was not Drawn");
                default:
                    throw new InvalidOperationException($"Could Not Recognize MirrorShape :{mirrorShape} or Null -- Visible Frame could not be Drawn");
            }
        }
        private void BuildRectangularVisibleFrame()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    HoledRectangleFrameLinesDraw frontFrameDraw = new();
                    frontFrameDraw.RectangleThickness = MirrorStandards.Frames.FrameFrontThickness;
                    frontFrameDraw.Length = supportBoundaryLength;
                    frontFrameDraw.Height = supportBoundaryHeight;
                    supportDraw = frontFrameDraw;
                    break;
                case DrawnSide.Rear:
                    RectangularVisibleRearFrame rearFrameDraw = new();
                    //The Total Thickness of the Frame
                    rearFrameDraw.RectangleThickness = MirrorStandards.Frames.FrameFrontThickness + MirrorStandards.Frames.FrameRearThickness;
                    //Only the Front Part
                    rearFrameDraw.MiddleRectangleThickness = MirrorStandards.Frames.FrameFrontThickness;
                    rearFrameDraw.Length = supportBoundaryLength;
                    rearFrameDraw.Height = supportBoundaryHeight;

                    supportDraw = rearFrameDraw;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Visible frame could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Visible Frame could not be drawn ");
            }
        }
        private void BuildCircularVisibleFrame()
        {
            if (sideToDraw is DrawnSide.Front or DrawnSide.Rear)
            {
                HoledCircleDraw circularFrame = new();
                circularFrame.Diameter = supportBoundaryDiameter;
                circularFrame.InnerDiameter = circularFrame.Diameter - 2 * MirrorStandards.Frames.FrameFrontCircularThickness;

                supportDraw = circularFrame;
            }
            else if (sideToDraw is DrawnSide.Side)
            {
                throw new NotImplementedException("Side Draw is Not Implemented -- Visible frame could not be Drawn ");
            }
            else
            {
                throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Visible Frame could not be drawn ");
            }

        }
        #endregion

        #region 2.Build Perimetrical Back Frame Methods

        /// <summary>
        /// Builds the Perimetrical Support of a Mirror Shape
        /// </summary>
        /// <exception cref="NotImplementedException">When Shape is Special</exception>
        /// <exception cref="InvalidOperationException">When SHape is not Recognized</exception>
        private void BuildPerimetricalSupport()
        {
            switch (mirrorShape)
            {
                case MirrorShape.Rectangular:
                    BuildRectangularMirrorPerimetricalSupport();
                    break;
                case MirrorShape.Capsule:
                    BuildCapsuleMirrorPerimetricalSupport();
                    break;
                case MirrorShape.Ellipse:
                    BuildEllipseMirrorPerimetricalSupport();
                    break;
                case MirrorShape.Circular:
                    BuildCircularMirrorPerimetricalSupport();
                    break;
                case MirrorShape.Special:
                    throw new NotImplementedException("Special Mirror Shape Not Implemented -- Perimetrical support was not Drawn");
                default:
                    throw new InvalidOperationException($"Could Not Recognize MirrorShape :{mirrorShape} or Null -- Perimetrical Support could not be Drawn");
            }
        }

        /// <summary>
        /// Builds the Perimetrical Support of a Circular Mirror
        /// </summary>
        /// <exception cref="NotImplementedException">When Side Draw is Chosen</exception>
        /// <exception cref="InvalidOperationException">When DrawnSide is not  Recognized</exception>
        private void BuildCircularMirrorPerimetricalSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //There is not a draw in the Front
                    break;
                case DrawnSide.Rear:
                    if (hasLight)
                    {
                        HoledCircleDraw circularSupport = new();
                        circularSupport.Diameter = supportBoundaryDiameter - 2 * MirrorStandards.Frames.PerimetricalCircularDistanceFromEdge;
                        circularSupport.InnerDiameter = circularSupport.Diameter - MirrorStandards.Frames.PerimetricalCircularThickness;
                        supportDraw = circularSupport;
                    }
                    else
                    {
                        HoledRectangleFrameLinesDraw perimetricalSupport = new();
                        perimetricalSupport.Length = supportBoundaryLength * 3 / 4;
                        perimetricalSupport.Height = supportBoundaryHeight * 3 / 4;
                        perimetricalSupport.RectangleThickness = MirrorStandards.Frames.PerimetricalThickness;
                        supportDraw = perimetricalSupport;
                    }
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        /// <summary>
        /// Builds the Perimetrical Support of a Rectangular Mirror
        /// </summary>
        /// <exception cref="NotImplementedException">When Side Draw is Chosen</exception>
        /// <exception cref="InvalidOperationException">When Drawn Side is not Recognized</exception>
        private void BuildRectangularMirrorPerimetricalSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //There is not a draw in the Front
                    break;
                case DrawnSide.Rear:
                    HoledRectangleFrameLinesDraw perimetricalSupport = new();
                    perimetricalSupport.Length = supportBoundaryLength - 2 * MirrorStandards.Frames.PerimetricalDistanceFromEdge;
                    perimetricalSupport.Height = supportBoundaryHeight - 2 * MirrorStandards.Frames.PerimetricalDistanceFromEdge;
                    perimetricalSupport.RectangleThickness = MirrorStandards.Frames.PerimetricalThickness;
                    supportDraw = perimetricalSupport;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        /// <summary>
        /// Builds the Perimetrical Support of a Capsule Mirror
        /// </summary>
        /// <exception cref="NotImplementedException">When Side Draw is Chosen</exception>
        /// <exception cref="InvalidOperationException">When Drawn Side is not Recognized</exception>
        private void BuildCapsuleMirrorPerimetricalSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //There is not a draw in the Front
                    break;
                case DrawnSide.Rear:
                    HoledCapsuleRectangleDraw perimetricalSupport = new(supportBoundaryLength - 2 * MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge,
                                                                        supportBoundaryHeight - 2 * MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge,
                                                                        MirrorStandards.Frames.PerimetricalCapsuleThickness);
                    supportDraw = perimetricalSupport;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        /// <summary>
        /// Builds the Perimetrical Support of an Ellipse Mirror
        /// </summary>
        /// <exception cref="NotImplementedException">When Side Draw is Chosen</exception>
        /// <exception cref="InvalidOperationException">When Drawn Side is not Recognized</exception>
        private void BuildEllipseMirrorPerimetricalSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //There is not a draw in the Front
                    break;
                case DrawnSide.Rear:
                    HoledEllipseDraw perimetricalSupport = new(supportBoundaryLength - 2 * MirrorStandards.Frames.PerimetricalEllipseDistanceFromEdge,
                                                               supportBoundaryHeight - 2 * MirrorStandards.Frames.PerimetricalEllipseDistanceFromEdge,
                                                               MirrorStandards.Frames.PerimetricalEllipseThickness);
                    supportDraw = perimetricalSupport;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        #endregion

        #region 3.Build Double Supports Methods

        private void BuildDoubleSupport()
        {
            switch (mirrorShape)
            {
                case MirrorShape.Rectangular:
                    BuildRectangularMirrorDoubleSupport();
                    break;
                case MirrorShape.Capsule:
                    BuildCapsuleMirrorDoubleSupport();
                    break;
                case MirrorShape.Ellipse:
                    BuildEllipseMirrorDoubleSupport();
                    break;
                case MirrorShape.Circular:
                    BuildCircularMirrorDoubleSupport();
                    break;
                case MirrorShape.Special:
                    throw new NotImplementedException("Special Mirror Shape Not Implemented -- Perimetrical support was not Drawn");
                default:
                    throw new InvalidOperationException($"Could Not Recognize MirrorShape :{mirrorShape} or Null -- Perimetrical Support could not be Drawn");
            }
        }

        private void BuildEllipseMirrorDoubleSupport()
        {
            supportDraw = new RectangleDraw();
        }

        private void BuildRectangularMirrorDoubleSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //No Draw in the Front
                    break;
                case DrawnSide.Rear:
                    RectangleTwinsDraw doubleSupport = new();
                    doubleSupport.FirstRectangle.Length = supportBoundaryLength * MirrorStandards.Frames.DoubleSupportLengthPercent;
                    doubleSupport.FirstRectangle.Height = MirrorStandards.Frames.PerimetricalThickness;
                    doubleSupport.SecondRectangle.Length = supportBoundaryLength * MirrorStandards.Frames.DoubleSupportLengthPercent;
                    doubleSupport.SecondRectangle.Height = MirrorStandards.Frames.PerimetricalThickness;
                    doubleSupport.AreRightLeftRectangles = false; //The Supports are up and Down
                    //The Distance between the two rectangles is the Boundary height minus half the heights of the Supports minus the distance they have from the boundary *2
                    doubleSupport.RectanglesCentersDistance = supportBoundaryHeight - 2 * MirrorStandards.Frames.PerimetricalDistanceFromEdge - 2 * MirrorStandards.Frames.PerimetricalThickness / 2d;

                    supportDraw = doubleSupport;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        private void BuildCircularMirrorDoubleSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //No Draw in the Front
                    break;
                case DrawnSide.Rear:
                    RectangleTwinsDraw doubleSupport = new();
                    //Get the Distance from the Center and Find the ChordLength then Apply the Length percentage to the Chord 
                    //This way the Support never gets out of the circle
                    doubleSupport.RectanglesCentersDistance = supportBoundaryDiameter - 4 * MirrorStandards.Frames.PerimetricalDistanceFromEdge - 4 * MirrorStandards.Frames.PerimetricalThickness / 2d;
                    double distanceFromCenter = doubleSupport.RectanglesCentersDistance / 2d;
                    double chordLength = MathCalc.GetCircleChordLength(supportBoundaryDiameter/2d, distanceFromCenter);
                    doubleSupport.FirstRectangle.Length = chordLength * MirrorStandards.Frames.DoubleSupportLengthPercent;
                    doubleSupport.FirstRectangle.Height = MirrorStandards.Frames.PerimetricalThickness;
                    doubleSupport.SecondRectangle.Length = chordLength * MirrorStandards.Frames.DoubleSupportLengthPercent;
                    doubleSupport.SecondRectangle.Height = MirrorStandards.Frames.PerimetricalThickness;
                    doubleSupport.AreRightLeftRectangles = false; //The Supports are up and Down
                    //The Distance between the two rectangles is the Boundary height minus half the heights of the Supports minus the distance they have from the boundary *2

                    supportDraw = doubleSupport;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        private void BuildCapsuleMirrorDoubleSupport()
        {
            switch (sideToDraw)
            {
                case DrawnSide.Front:
                    supportDraw = null; //No Draw in the Front
                    break;
                case DrawnSide.Rear:
                    //If the Mirror is Horizontal then get the Chord Length (Combining both semicircles and treating distance from center as the same from center of semicrciles)
                    //Then add to this length the length of the middle rectangle of the Capsule and make a support with percentage
                    //If the Mirror is Vertical . Then find the Chord of the Circle and apply the Percentage
                    RectangleTwinsDraw doubleSupport = new();
                    CapsuleRectangleDraw capsuleBoundary = supportBoundary as CapsuleRectangleDraw ?? throw new InvalidCastException("Support Boundary is Not a Capsule Shape");
                    double distanceFromCenter = capsuleBoundary.CornerRadius - 2 * MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge;
                    double chordLength = MathCalc.GetCircleChordLength(capsuleBoundary.CornerRadius, distanceFromCenter);
                    if (capsuleBoundary.IsHorizontal)
                    {
                        double capsuleMidRectangleLength = capsuleBoundary.Length - 2 * capsuleBoundary.CornerRadius;
                        doubleSupport.FirstRectangle.Length = (chordLength + capsuleMidRectangleLength) * MirrorStandards.Frames.DoubleSupportLengthPercent;
                        doubleSupport.SecondRectangle.Length = (chordLength + capsuleMidRectangleLength) * MirrorStandards.Frames.DoubleSupportLengthPercent;
                        doubleSupport.RectanglesCentersDistance = 2 * capsuleBoundary.CornerRadius - 4 * MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge;
                    }
                    else
                    {
                        double capsuleMidRectangleHeight = capsuleBoundary.Height - 2 * capsuleBoundary.CornerRadius;
                        doubleSupport.FirstRectangle.Length = chordLength * MirrorStandards.Frames.DoubleSupportLengthPercent;
                        doubleSupport.SecondRectangle.Length = chordLength * MirrorStandards.Frames.DoubleSupportLengthPercent;
                        doubleSupport.RectanglesCentersDistance = (2 * capsuleBoundary.CornerRadius + capsuleMidRectangleHeight) - 4 * MirrorStandards.Frames.PerimetricalCapsuleDistanceFromEdge;
                    }

                    doubleSupport.FirstRectangle.Height = MirrorStandards.Frames.PerimetricalThickness;
                    doubleSupport.SecondRectangle.Height = MirrorStandards.Frames.PerimetricalThickness;
                    doubleSupport.AreRightLeftRectangles = false; //The Supports are up and Down
                    //The Distance between the two rectangles is the Boundary height minus half the heights of the Supports minus the distance they have from the boundary *2
                    //Capsules do not have always Distance from Edge on the Y Axis

                    supportDraw = doubleSupport;
                    break;
                case DrawnSide.Side:
                    throw new NotImplementedException("Side Draw is Not Implemented -- Perimetrical Support could not be Drawn ");
                default:
                    throw new InvalidOperationException($"Could not Recognize DrawnSide : -{sideToDraw}- or Null -- Perimetrical Support could not be drawn ");
            }
        }

        #endregion

    }
}

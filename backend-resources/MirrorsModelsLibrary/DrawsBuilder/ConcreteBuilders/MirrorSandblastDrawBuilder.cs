using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGDrawingLibrary.Enums;
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
    /// The Builder for the Sandblast Draw . Builds a Sandblast and Places it at its Default Position
    /// </summary>
    public class MirrorSandblastDrawBuilder : IDrawShapeBuilder
    {
        DrawShape sandblastDraw;

        private double sandblastBoundaryLength;
        private double sandblastBoundaryHeight;
        private double sandblastBoundaryDiameter;
        private double sandblastBoundaryCenterX;
        private double sandblastBoundaryCenterY;

        private MirrorSandblast? sandblastToDraw;
        private bool hasRounding;
        private bool hasVisibleFrame;

        public MirrorSandblastDrawBuilder(MirrorSandblast? sandblastToDraw,
                                             DrawShape sandblastBoundary,
                                             bool hasRounding,
                                             bool hasVisibleFrame)
        {
            RectangleDraw boundary = sandblastBoundary as RectangleDraw;
            sandblastBoundaryLength = boundary.Length;
            sandblastBoundaryHeight = boundary.Height;
            sandblastBoundaryCenterX = boundary.ShapeCenterX;
            sandblastBoundaryCenterY = boundary.ShapeCenterY;

            this.sandblastToDraw = sandblastToDraw;
            this.hasRounding = hasRounding;
            this.hasVisibleFrame = hasVisibleFrame;
        }

        public MirrorSandblastDrawBuilder(MirrorSandblast? sandblastToDraw,
                                             DrawShape sandblastBoundary)
        {
            CircleDraw boundary = sandblastBoundary as CircleDraw;
            sandblastBoundaryDiameter = boundary.Diameter;
            sandblastBoundaryCenterX = boundary.ShapeCenterX;
            sandblastBoundaryCenterY = boundary.ShapeCenterY;
            this.sandblastToDraw = sandblastToDraw;
        }

        public DrawShape BuildShape()
        {
            switch (sandblastToDraw)
            {
                case MirrorSandblast.H8:
                    BuildSandblastH8();
                    break;
                case MirrorSandblast.X6:
                    BuildSandblastX6();
                    break;
                case MirrorSandblast.X4:
                    BuildSandblastX4();
                    break;
                case MirrorSandblast._6000:
                    BuildSandblast6000();
                    break;
                case MirrorSandblast.M3:
                    BuildSandblastM3();
                    break;
                case MirrorSandblast.N6:
                    BuildSandblastN6();
                    break;
                default:
                case MirrorSandblast.Special:
                case MirrorSandblast.N9:
                case MirrorSandblast.H7:
                    sandblastDraw = null;
                    break;
            }
            if (sandblastDraw is not null)
            {
                DrawShape shapeToReturn = sandblastDraw.CloneSelf();
                shapeToReturn.Fill = "lightgray";
                shapeToReturn.Opacity = "0.9";
                shapeToReturn.Stroke = "lightslategray";
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
        /// Builds the N6 Sandblast Draw
        /// </summary>
        private void BuildSandblastN6()
        {
            HoledCircleDraw N6 = new();
            N6.Diameter = sandblastBoundaryDiameter;
            N6.InnerDiameter = N6.Diameter - MirrorStandards.Sandblasts.ThicknessN6;

            //The Center of the Circle will be at the center of the container
            N6.SetCenterOrStartX(sandblastBoundaryCenterX);
            N6.SetCenterOrStartY(sandblastBoundaryCenterY);

            sandblastDraw = N6;
        }

        /// <summary>
        /// Builds the M3 Sandblast Draw at its Default Position
        /// </summary>
        private void BuildSandblastM3()
        {
            RectangleDraw M3 = new();
            M3.Length = sandblastBoundaryLength - MirrorStandards.Sandblasts.DistanceFromSideM3;
            M3.Height = MirrorStandards.Sandblasts.ThicknessM3;
            M3.SetCenterOrStartX(sandblastBoundaryCenterX, DrawShape.CSCoordinate.Center);
            M3.SetCenterOrStartY(sandblastBoundaryCenterY - sandblastBoundaryHeight / 2d + MirrorStandards.Sandblasts.DistanceFromTopM3 + MirrorStandards.Sandblasts.ThicknessM3 / 2d, DrawShape.CSCoordinate.Center);
            sandblastDraw = M3;
        }

        /// <summary>
        /// Builds the 6000 Sandblast Draw at its Default Position
        /// </summary>
        private void BuildSandblast6000()
        {
            RectangleTwinsDraw _6000 = new();
            //Put a small Margin if there is Frame
            _6000.FirstRectangle.Height = sandblastBoundaryHeight - 2 * MirrorStandards.Sandblasts.DistanceFromTop6000;
            _6000.SecondRectangle.Height = sandblastBoundaryHeight - 2 * MirrorStandards.Sandblasts.DistanceFromTop6000;

            _6000.FirstRectangle.Length = MirrorStandards.Sandblasts.Thickness6000;
            _6000.SecondRectangle.Length = MirrorStandards.Sandblasts.Thickness6000;

            _6000.AreRightLeftRectangles = true; //X6 Shape is Top-Bottom Rectangles

            //The Distance of the Centers is not the same when we have Frame
            _6000.RectanglesCentersDistance = sandblastBoundaryLength - 2 * (MirrorStandards.Sandblasts.Thickness6000 / 2d)
                 - 2 * MirrorStandards.Sandblasts.DistanceFromSide6000;

            _6000.SetCenterOrStartX(sandblastBoundaryCenterX, DrawShape.CSCoordinate.Center);
            _6000.SetCenterOrStartY(sandblastBoundaryCenterY, DrawShape.CSCoordinate.Center);

            sandblastDraw = _6000;
        }

        /// <summary>
        /// Builds the X4 Sandblast Draw at its Default Position
        /// </summary>
        private void BuildSandblastX4()
        {
            RectangleTwinsDraw X4 = new();
            //Put a small Margin if there is Frame
            X4.FirstRectangle.Height = sandblastBoundaryHeight - (hasVisibleFrame ? 2 * MirrorStandards.Sandblasts.SandblastFrameMargin : 0);
            X4.SecondRectangle.Height = sandblastBoundaryHeight - (hasVisibleFrame ? 2 * MirrorStandards.Sandblasts.SandblastFrameMargin : 0);

            X4.FirstRectangle.Length = MirrorStandards.Sandblasts.ThicknessX4;
            X4.SecondRectangle.Length = MirrorStandards.Sandblasts.ThicknessX4;

            //When there is Rounding
            X4.FirstRectangle.RoundedCorners = hasRounding ? CornersToRound.LeftCorners : CornersToRound.None;
            X4.SecondRectangle.RoundedCorners = hasRounding ? CornersToRound.RightCorners : CornersToRound.None;
            X4.FirstRectangle.CornerRadius = MirrorStandards.RoundedCornersRadius;
            X4.SecondRectangle.CornerRadius = MirrorStandards.RoundedCornersRadius;

            X4.AreRightLeftRectangles = true; //X6 Shape is Top-Bottom Rectangles

            //The Distance of the Centers is not the same when we have Frame
            X4.RectanglesCentersDistance = sandblastBoundaryLength - 2 * (MirrorStandards.Sandblasts.ThicknessX4 / 2d)
                 - (hasVisibleFrame ? 2 * MirrorStandards.Sandblasts.SandblastFrameMargin : 0);

            X4.SetCenterOrStartX(sandblastBoundaryCenterX, DrawShape.CSCoordinate.Center);
            X4.SetCenterOrStartY(sandblastBoundaryCenterY, DrawShape.CSCoordinate.Center);

            sandblastDraw = X4;
        }

        /// <summary>
        /// Builds the X6 Sandblast Draw
        /// </summary>
        private void BuildSandblastX6()
        {
            RectangleTwinsDraw X6 = new();
            //Put a small Margin if there is Frame
            X6.FirstRectangle.Length = sandblastBoundaryLength - (hasVisibleFrame ? 2 * MirrorStandards.Sandblasts.SandblastFrameMargin : 0);
            X6.SecondRectangle.Length = sandblastBoundaryLength - (hasVisibleFrame ? 2 * MirrorStandards.Sandblasts.SandblastFrameMargin : 0);

            X6.FirstRectangle.Height = MirrorStandards.Sandblasts.ThicknessX6;
            X6.SecondRectangle.Height = MirrorStandards.Sandblasts.ThicknessX6;

            //When there is Rounding
            X6.FirstRectangle.RoundedCorners = hasRounding ? CornersToRound.UpperCorners : CornersToRound.None;
            X6.SecondRectangle.RoundedCorners = hasRounding ? CornersToRound.BottomCorners : CornersToRound.None;
            X6.FirstRectangle.CornerRadius = MirrorStandards.RoundedCornersRadius;
            X6.SecondRectangle.CornerRadius = MirrorStandards.RoundedCornersRadius;

            X6.AreRightLeftRectangles = false; //X6 Shape is Top-Bottom Rectangles

            //The Distance of the Centers is not the same when we have Frame
            X6.RectanglesCentersDistance = sandblastBoundaryHeight - 2 * (MirrorStandards.Sandblasts.ThicknessX6 / 2d)
                 - (hasVisibleFrame ? 2 * MirrorStandards.Sandblasts.SandblastFrameMargin : 0);

            X6.SetCenterOrStartX(sandblastBoundaryCenterX, DrawShape.CSCoordinate.Center);
            X6.SetCenterOrStartY(sandblastBoundaryCenterY, DrawShape.CSCoordinate.Center);

            sandblastDraw = X6;
        }

        /// <summary>
        /// Builds the H8 Sandblast Draw
        /// </summary>
        private void BuildSandblastH8()
        {
            HoledRectangleDraw H8 = new();
            H8.Length = sandblastBoundaryLength - 2 * MirrorStandards.Sandblasts.DistanceFromSideH8;
            H8.Height = sandblastBoundaryHeight - 2 * MirrorStandards.Sandblasts.DistanceFromTopH8;
            H8.RectangleThickness = MirrorStandards.Sandblasts.ThicknessH8;
            H8.RoundedCorners = CornersToRound.None;

            H8.SetCenterOrStartX(sandblastBoundaryCenterX, DrawShape.CSCoordinate.Center);
            H8.SetCenterOrStartY(sandblastBoundaryCenterY, DrawShape.CSCoordinate.Center);

            sandblastDraw = H8;
        }

        public void ResetBuilder()
        {
            sandblastBoundaryLength = 0;
            sandblastBoundaryHeight = 0;
            sandblastBoundaryDiameter = 0;
            sandblastBoundaryCenterX = 0;
            sandblastBoundaryCenterY = 0;
            hasRounding = false;
            hasVisibleFrame = false;
            sandblastToDraw = null;
            sandblastDraw = null;
        }
    }
}

using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models.ConcreteShapes;
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
    /// The Builder for the Mirror Glass . Builds the Glass and Places it at the Default position inside its DrawContainer
    /// </summary>
    public class MirrorGlassDrawBuilder : IDrawShapeBuilder
    {
        private DrawShape builtGlassDraw;
        double mirrorLength;        // The Length of the Mirror
        double mirrorHeight;        // The Height of the Mirror
        double mirrorDiameter;      // The Diameter of the Mirror
        double cornerRadius;        // The Radius of the Corners of the Mirror
        double frameFrontThickness; // The Thickness of the Front Frame (0 when there is no frame)
        double containerMargin;     // The Container MarginX - MarginY
        MirrorShape? mirrorShape;   // The Shape of the Mirror

        /// <summary>
        /// Builds the Glass Draw of a Rectangular Mirror
        /// </summary>
        /// <param name="mirrorLength">The Length of the Mirror</param>
        /// <param name="mirrorHeight">The Height of the Mirror</param>
        /// <param name="hasRounding">If the Glass Edges will be Rounded</param>
        /// <param name="hasVisibleFrame">If the Glass will be Framed</param>
        public MirrorGlassDrawBuilder(double mirrorLength, double mirrorHeight, double containerMargin, bool hasRounding = false, bool hasVisibleFrame = false)
        {
            this.mirrorLength = mirrorLength;
            this.mirrorHeight = mirrorHeight;
            this.containerMargin = containerMargin;
            cornerRadius = hasRounding ? MirrorStandards.RoundedCornersRadius : 0;
            frameFrontThickness = hasVisibleFrame ? MirrorStandards.Frames.FrameFrontThickness : 0;
            mirrorShape = MirrorShape.Rectangular;
        }

        /// <summary>
        /// Builds the Glass Draw of a Capsule or Ellipse Mirror
        /// </summary>
        /// <param name="mirrorLength"></param>
        /// <param name="mirrorHeight"></param>
        /// <param name="containerMargin"></param>
        public MirrorGlassDrawBuilder(double mirrorLength,double mirrorHeight,double containerMargin , MirrorShape mirrorShape)
        {
            if(mirrorShape is not MirrorShape.Capsule and not MirrorShape.Ellipse) { throw new ArgumentException("Elipse-Capsule Constructor cannot take arguments of Other Shapes"); }
            this.mirrorLength = mirrorLength;
            this.mirrorHeight = mirrorHeight;
            this.containerMargin = containerMargin;
            this.mirrorShape = mirrorShape;
        }

        /// <summary>
        /// Builds the Glass Draw of a Circular Mirror
        /// </summary>
        /// <param name="mirrorDiameter">The Diameter of the Mirror</param>
        /// <param name="hasVisibleFrame">If the Glass will be Framed</param>
        public MirrorGlassDrawBuilder(double mirrorDiameter, double containerMargin, bool hasVisibleFrame = false)
        {
            this.mirrorDiameter = mirrorDiameter;
            this.containerMargin = containerMargin;
            frameFrontThickness = hasVisibleFrame ? MirrorStandards.Frames.FrameFrontCircularThickness : 0;
            mirrorShape = MirrorShape.Circular;
        }

        /// <summary>
        /// Builds the Draw of the Glass at the Default Placement Position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public DrawShape BuildShape()
        {
            if (mirrorShape == MirrorShape.Rectangular)
            {
                BuildRectangularGlass();
            }
            else if (mirrorShape == MirrorShape.Circular)
            {
                BuildCircularGlass();
            }
            else if (mirrorShape == MirrorShape.Capsule)
            {
                BuildCapsuleGlass();
            }
            else if (mirrorShape == MirrorShape.Ellipse)
            {
                BuildEllipseGlass();
            }
            else
            {
                throw new ArgumentException($"Could not Draw Glass Not Recognized {nameof(MirrorShape)} , Shape:{mirrorShape} ");
            }
            DrawShape drawToReturn = builtGlassDraw.CloneSelf();
            drawToReturn.Fill = "aliceblue";
            drawToReturn.Stroke = "lightslategray";
            drawToReturn.Name = DrawShape.GLASS;
            ResetBuilder();
            return drawToReturn;
        }

        /// <summary>
        /// Builds an Ellipse Shaped Glass Draw , (Start at 0,0)
        /// </summary>
        private void BuildEllipseGlass()
        {
            EllipseDraw ellipseGlass = new(mirrorLength, mirrorHeight);
            ellipseGlass.SetCenterOrStartX(containerMargin, DrawShape.CSCoordinate.Start);
            ellipseGlass.SetCenterOrStartY(containerMargin + ellipseGlass.Height / 2d, DrawShape.CSCoordinate.Start);
            builtGlassDraw = ellipseGlass;
        }

        /// <summary>
        /// Builds a Capsule Shaped Glass Draw , Start at 0,0)
        /// </summary>
        private void BuildCapsuleGlass()
        {
            CapsuleRectangleDraw capsuleGlass = new(mirrorLength,mirrorHeight);
            capsuleGlass.SetCenterOrStartX(containerMargin, DrawShape.CSCoordinate.Start);
            capsuleGlass.SetCenterOrStartY(containerMargin, DrawShape.CSCoordinate.Start);
            builtGlassDraw = capsuleGlass;
        }

        /// <summary>
        /// Builds a Glass Circular Draw Center at 0,0
        /// </summary>
        private void BuildCircularGlass()
        {
            CircleDraw circularGlass = new();
            circularGlass.Diameter = mirrorDiameter - 2 * frameFrontThickness;
            circularGlass.SetCenterOrStartX(containerMargin + circularGlass.Radius + frameFrontThickness);
            circularGlass.SetCenterOrStartY(containerMargin + circularGlass.Radius + frameFrontThickness);
            builtGlassDraw = circularGlass;
        }

        /// <summary>
        /// Builds a Glass Rectangular Draw Center with Center at 0,0
        /// </summary>
        private void BuildRectangularGlass()
        {
            RectangleDraw rectangularGlass = new();
            rectangularGlass.Length = mirrorLength - 2 * frameFrontThickness;
            rectangularGlass.Height = mirrorHeight - 2 * frameFrontThickness;
            rectangularGlass.RoundedCorners = CornersToRound.All;
            rectangularGlass.CornerRadius = cornerRadius;
            rectangularGlass.SetCenterOrStartX(containerMargin + frameFrontThickness, DrawShape.CSCoordinate.Start);
            rectangularGlass.SetCenterOrStartY(containerMargin + frameFrontThickness, DrawShape.CSCoordinate.Start);
            builtGlassDraw = rectangularGlass;
        }

        /// <summary>
        /// Resets all the Properties of the Builder
        /// </summary>
        public void ResetBuilder()
        {
            builtGlassDraw = null;
            mirrorLength = 0;
            mirrorHeight = 0;
            mirrorDiameter = 0;
            frameFrontThickness = 0;
            cornerRadius = 0;
            containerMargin = 0;
            mirrorShape = null;
        }
    }
}

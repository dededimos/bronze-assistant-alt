using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Helpers;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models
{
    public abstract class DrawShape
    {
        //PreBuild Mirror Shape Names
        public static readonly string MAGN = "MagnifyerDraw";
        public static readonly string MAGNSAND = "MagnifyerSandblastDraw";
        public static readonly string TOUCH = "TouchDraw";
        public static readonly string TOUCHFOG = "TouchFogDraw";
        public static readonly string DIMMER = "DimmerDraw";
        public static readonly string SENSOR = "SensorDraw";
        public static readonly string FOG16 = "Fog16WDraw";
        public static readonly string FOG24 = "Fog24WDraw";
        public static readonly string FOG55 = "Fog55Draw";
        public static readonly string DISPLAY11 = "Display11Draw";
        public static readonly string DISPLAY19 = "Display19Draw";
        public static readonly string DISPLAY20 = "Display20Draw";
        public static readonly string CLOCK = "ClockDraw";
        public static readonly string GLASS = "GlassDraw";
        public static readonly string VISIBLEFRAME = "VisibleFrameDraw";
        public static readonly string SUPPORTPERIM = "PerimetricalSupportDraw";
        public static readonly string SUPPORTDOUBLE = "DoubleSupportDraw";
        public static readonly string SUPPORTSINGLE = "SingleSupportDraw";
        public static readonly string SANDBLAST = "SandbastDraw";
        public static readonly string EXTRASAREA = "ExtrasAreaDraw";
        public static readonly string HEIGHTDIM = "HeightDimension";
        public static readonly string LENGTHDIM = "LengthDimension";
        public static readonly string DIAMETERDIM = "DiameterDimension";

        public double BoundingBoxCenterX { get => GetBoundingBoxCenterX(); }
        public double BoundingBoxCenterY { get => GetBoundingBoxCenterY(); }
        public double BoundingBoxLength { get => GetBoundingBoxLength(); }
        public double BoundingBoxHeight { get => GetBoundingBoxHeight(); }

        /// <summary>
        /// The Center Point X of the Shape (Used For Placement)
        /// </summary>
        public virtual double ShapeCenterX { get; protected set; }
        /// <summary>
        /// The Center Point Y of the Shape (Used For Placement)
        /// </summary>
        public virtual double ShapeCenterY { get; protected set; }

        public string Stroke { get; set; } = "";
        public string Fill { get; set; } = "";
        public string Name { get; set; } = "";
        public string Filter { get; set; } = "";
        public string Opacity { get; set; } = "1"; //0.00 - 1.00
        public string StrokeDashArray { get; set; } = "";
        public string ShapePathDataString { get => GetShapePathData(); }
        public string ClipPathDataString { get => Clip?.GetShapePathData() ?? ""; }

        /// <summary>
        /// The Clip of this Particular Shape
        /// In HTML Clip Path should be Drawn Opposite of ParentDraw Path , as default fill Rule is 'Non Zero'
        /// In WPF Clip Path should be Drawn with the same Direction as the Parent Draw Path.
        /// </summary>
        public DrawShape? Clip { get; set; }

        /// <summary>
        /// The Index Number of the Draw -- When Having multipe Draws this can be used to set which wil be drawn first
        /// 0 means top layer , subsequent numbers indicate layers on the rear of the stack
        /// </summary>
        public int LayerNo { get; set; } = 0;

        /// <summary>
        /// Get the Center X of the Shape's Bounding Box
        /// </summary>
        /// <returns></returns>
        public abstract double GetBoundingBoxCenterX();
        /// <summary>
        /// Get the Center Y of the Shape's Bounding Box
        /// </summary>
        /// <returns></returns>
        public abstract double GetBoundingBoxCenterY();
        /// <summary>
        /// Get the Length of the Shape's Bounding Box
        /// </summary>
        /// <returns></returns>
        public abstract double GetBoundingBoxLength();
        /// <summary>
        /// Get the Height of the Shape's Bounding Box
        /// </summary>
        /// <returns></returns>
        public abstract double GetBoundingBoxHeight();

        /// <summary>
        /// Get the Path Data String of the Shape
        /// </summary>
        /// <returns></returns>
        public abstract string GetShapePathData();

        /// <summary>
        /// Returns the PathData of the Middle Line of the Bounding Box (This way Text Can Be Anchored)
        /// The Line is always HORIZONTAL
        /// </summary>
        /// <returns>The Path Data of the Line</returns>
        public virtual string GetTextAnchorMiddleLinePath()
        {
            double lineStartX = GetBoundingBoxCenterX() - GetBoundingBoxLength() / 2d;
            double lineStartY = GetBoundingBoxCenterY();
            double lineEndX = lineStartX + GetBoundingBoxLength();
            double lineEndY = lineStartY;

            string linePathData = PathDataFactory.Line(lineStartX, lineStartY, lineEndX, lineEndY);
            return linePathData;
        }

        /// <summary>
        /// Returns the Draw Rectangle of the Bounding Box of the Shape
        /// </summary>
        /// <returns>a Rectangle Draw of the Bounding Box</returns>
        public virtual RectangleDraw GetBoundingBoxRectangle()
        {
            RectangleDraw bb = new();
            bb.RoundedCorners = CornersToRound.None;
            bb.Length = GetBoundingBoxLength();
            bb.Height = GetBoundingBoxHeight();
            bb.SetCenterOrStartX(GetBoundingBoxCenterX() - bb.Length / 2d, CSCoordinate.Start);
            bb.SetCenterOrStartY(GetBoundingBoxCenterY() - bb.Height / 2d, CSCoordinate.Start);
            bb.Name = $"{this.Name}BoundingBox";
            bb.Stroke = "red";
            return bb;
        }

        /// <summary>
        /// Deep Clones the Shape returning a new Identical Object with a different Reference
        /// </summary>
        /// <returns>a New Shape identical to the Provided but with a new Reference</returns>
        public abstract DrawShape CloneSelf();

        /// <summary>
        /// Sets the ShapeCenterX or the Draw StartingX Coordinate of a Draw Shape --
        /// e.x.(CenterX is the Starting Draw Coordinate of a Circle , StartX the Starting Draw Coordinate of a Rectangle e.t.c.)
        /// </summary>
        public abstract void SetCenterOrStartX(double newX, CSCoordinate centerOrStart);
        /// <summary>
        /// Sets the ShapeCenterY or the Draw StartingY Coordinate of a Draw Shape --
        /// e.x.(CenterY is the Starting Draw Coordinate of a Circle , StartY the Starting Draw Coordinate of a Rectangle e.t.c.)
        /// </summary>
        public abstract void SetCenterOrStartY(double newY, CSCoordinate centerOrStart);

        /// <summary>
        /// Flips the Draw Horizontally based on the area (BASE METHOD WORKING ONLY FOR SYMMETRICAL SHAPES, for non Symmetrical it must be overriden)
        /// </summary>
        /// <param name="flipOriginX">The X Coordinate about which we want to flip the shape - Putting ShapeCenterX as the origin - the Draw will flip according to its center point</param>
        public virtual void FlipHorizontally(double flipOriginX)
        {
            //Executing this only for a single Shape that is symmetrical will only Traslate it . But if the Origin is of a combined shape it will flip the draws according to the center of the combined shape
            //The New X of the Flipped Shape
            double flippedCenterX = 2 * flipOriginX - ShapeCenterX; //Draw Cartesian Coordinates and do the example for a point ,to understand the Equation
            SetCenterOrStartX(flippedCenterX, CSCoordinate.Center); 
        }

        /// <summary>
        /// Flips the Draw Vertically based on the area (BASE METHOD WORKING ONLY FOR SYMMETRICAL SHAPES, for non Symmetrical it must be overriden)
        /// </summary>
        /// <param name="flipOriginY">The Y Coordinate about which we want to flip the shape - Putting ShapeCenterY as the origin - the Draw will flip according to its center point</param>
        public virtual void FlipVertically(double flipOriginY)
        {
            //Executing this only for a single Shape that is symmetrical will only Traslate it . But if the Origin is of a combined shape it will flip the draws according to the center of the combined shape
            double flippedCenterY = 2 * flipOriginY - ShapeCenterY; //Draw Cartesian Coordinates and do the example for a point ,to understand the Equation
            SetCenterOrStartY(flippedCenterY, CSCoordinate.Center);
        }

        /// <summary>
        /// Translates the Shape by the Give amount on X axis
        /// </summary>
        /// <param name="translateX">the Amount of translation (takes negative values also)</param>
        public virtual void TranslateX(double translateX) 
        { 
            SetCenterOrStartX(ShapeCenterX + translateX, CSCoordinate.Center);
        }

        /// <summary>
        /// Translates the Shape by the Give amount on Y axis
        /// </summary>
        /// <param name="translateY">the Amount of translation (takes negative values also)</param>
        public virtual void TranslateY(double translateY)
        {
            SetCenterOrStartY(ShapeCenterY + translateY, CSCoordinate.Center);
        }

        /// <summary>
        /// Returns the Combined Bounding Box , of this Shape Combined with another 
        /// </summary>
        /// <param name="other">The other Shape</param>
        /// <returns>The new Bounding Box</returns>
        public virtual RectangleDraw GetCombinedBoundingBox(DrawShape other)
        {
            var thisBoundingBox = GetBoundingBoxRectangle();
            var otherBoundingBox = other.GetBoundingBoxRectangle();

            var collectionX = new double[] { thisBoundingBox.StartX, thisBoundingBox.EndX, otherBoundingBox.StartX, otherBoundingBox.EndX };
            var collectionY = new double[] { thisBoundingBox.StartY, thisBoundingBox.EndY, otherBoundingBox.StartY, otherBoundingBox.EndY };

            return new RectangleDraw(collectionX.Min(), collectionY.Min(), collectionX.Max(), collectionY.Max());
        }

        //*********** ITS NOT WORKING AS EXPECTED ***********//*********** ITS NOT WORKING AS EXPECTED ***********
        /// <summary>
        /// If the Start X or Y of the Bounding Box are in Negative Space - then Translates the Whole Shape so it all Points are in Positive Values
        /// </summary>
        public void TranslateShapeToPositiveDimensions()
        {
            var boundingBox = GetBoundingBoxRectangle();
            if (boundingBox.StartX < 0)
            {
                TranslateX(-boundingBox.StartX); //Move Positivly till zero as its negative
            }
            if (boundingBox.StartY < 0)
            {
                TranslateY(-boundingBox.StartY); //Move Positivly till zero as its negative
            }
        }
        //*********** ITS NOT WORKING AS EXPECTED ***********//*********** ITS NOT WORKING AS EXPECTED ***********

        /// <summary>
        /// Refers to the Center or the Start Draw Coordinate of a DrawShape
        /// </summary>
        public enum CSCoordinate
        {
            /// <summary>
            /// The Start Draw Cordinate of a Draw Shape
            /// </summary>
            Start = 0,
            /// <summary>
            /// The ShapeCenterCoordinate of a Draw Shape
            /// </summary>
            Center = 1
        }
    }

}
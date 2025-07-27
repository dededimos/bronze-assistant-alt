using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawingLibrary.Interfaces
{
    public interface IDrawing : IDeepClonable<IDrawing>
    {
        string Name { get; set; }
        /// <summary>
        /// The Location of the Graphic on the X Axis
        /// </summary>
        double LocationX { get; }
        /// <summary>
        /// The Location of the Graphic on the Y Axis
        /// </summary>
        double LocationY { get; }
        /// <summary>
        /// The Layer No of the Graphic
        /// </summary>
        int LayerNo { get; }
        /// <summary>
        /// Any Clipping shape happening on this Graphic
        /// </summary>
        IReadOnlyList<IDrawing>? Clips { get; }

        /// <summary>
        /// The Unique Id of the Graphic
        /// </summary>
        string UniqueId { get; }
        /// <summary>
        /// The Presentation Options
        /// </summary>
        DrawingPresentationOptions PresentationOptions { get; set; }
        /// <summary>
        /// Text embeded with the Drawing
        /// </summary>
        string? DrawingText { get; }
        /// <summary>
        /// The Line that acts as an anchor for the embeded Text
        /// </summary>
        LineInfo? TextAnchorLine { get; }
        /// <summary>
        /// Sets the layer on the Shape
        /// </summary>
        /// <param name="layerNo">The number of the Layer , smaller numbers are top on the visual stack</param>
        void SetLayer(int layerNo);
        /// <summary>
        /// Sets the Text embeded with the Drawing
        /// </summary>
        /// <param name="text"></param>
        void SetText(string? text);
        /// <summary>
        /// Returns the Path Data String of the Graphic
        /// </summary>
        /// <returns></returns>
        string GetPathDataString();
        /// <summary>
        /// Returns the reverse path data string of the Graphic . (Drawing a hole instead of a solid draw)
        /// </summary>
        /// <returns></returns>
        string GetReversePathDataString();
        /// <summary>
        /// Returns the Bounding Box of the Graphic
        /// </summary>
        /// <returns></returns>
        RectangleInfo GetBoundingBox();
        /// <summary>
        /// Sets a new Location for the Graphic
        /// </summary>
        /// <param name="newLocation">The new Location <see cref="PointXY"/></param>
        void SetLocation(PointXY newLocation);
        /// <summary>
        /// Sets a new Location <see cref="PointXY"/> for the Graphic
        /// </summary>
        /// <param name="newX">The X Coordinate</param>
        /// <param name="newY">The Y Coordinate</param>
        void SetLocation(double newX, double newY);
        /// <summary>
        /// Translates the Graphic in the X and Y axis
        /// </summary>
        /// <param name="translateX">The Translation distance on X axis</param>
        /// <param name="translateY">The Translation distance on Y axis</param>
        void Translate(double translateX, double translateY);
        /// <summary>
        /// Translates the Graphic on the X Axis
        /// </summary>
        /// <param name="translateX">The translation distance on the X axis</param>
        void TranslateX(double translateX);
        /// <summary>
        /// Translates the Graphic on the Y axis
        /// </summary>
        /// <param name="translateY">The Translation distance on the Y axis</param>
        void TranslateY(double translateY);
        /// <summary>
        /// Scales the graphic by the designated scale factor
        /// </summary>
        /// <param name="scale">The scale factor . Less than one to shring , more than one to grow</param>
        void Scale(double scale);
        /// <summary>
        /// Scales the Shape from a given OriginXY 
        /// </summary>
        /// <param name="scaleFactor">scale Factor</param>
        /// <param name="origin">The originXY of the Scale transformation (ex. Rectangle Container scales from its center with inside a Circle , the OrginXY for the Scale of the Circle is RectangleCenterX , RectangleCenterY)</param>
        void ScaleFromOrigin(double scaleFactor, PointXY origin);
        /// <summary>
        /// Flips the Graphic horizontally to the specified xOrigin point
        /// </summary>
        /// <param name="flipOriginY">The flipping y Coordinate , defining the X axis around which the flip happens</param>
        void FlipHorizontally(double flipOriginY);
        /// <summary>
        /// Flips the Graphic vertically to the specified yOrigin point
        /// </summary>
        /// <param name="flipOriginX">The flipping x Coordinate , defining the Y axis around which the flip happens</param>
        void FlipVertically(double flipOriginX);
        /// <summary>
        /// Returns a Clone of the Graphic with a new Unique Id
        /// </summary>
        /// <param name="generateUniqueId"></param>
        /// <returns></returns>
        IDrawing GetDeepClone(bool generateUniqueId);
        /// <summary>
        /// Returns a Clone of this Drawing centered to the specified Rectangle
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        IDrawing GetCloneCenteredToContainer(RectangleInfo container);
        /// <summary>
        /// Adds a Clip Draw to this Draw's Clips
        /// </summary>
        /// <param name="clipDraw">The Clip Draw to Add</param>
        void AddClip(IDrawing clipDraw);
    }
}

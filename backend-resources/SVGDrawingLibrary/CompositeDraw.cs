using SVGDrawingLibrary.Helpers;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models;

public class CompositeDraw : DrawShape
{
    /// <summary>
    /// The Bounding Box of the Shape - Get Defined each time a shape is added or removed (otherwise calculation will be too much (Possibly?))
    /// </summary>
    private RectangleDraw boundingBox = new(0, 0);
    private readonly List<DrawShape> draws = new();
    public IEnumerable<DrawShape> Draws { get => draws.OrderBy(d => d.LayerNo); }

    public override double ShapeCenterX { get => boundingBox.ShapeCenterX; }
    public override double ShapeCenterY { get => boundingBox.ShapeCenterY; }

    /// <summary>
    /// Creates an Empty Draw
    /// </summary>
    public CompositeDraw()
    {

    }

    /// <summary>
    /// Adds a Draw
    /// </summary>
    /// <param name="draw"></param>
    public void AddDraw(DrawShape draw)
    {
        // Define the new Bounding Box by combining the old one with the new
        boundingBox = boundingBox.GetCombinedBoundingBox(draw);
        // Set the layer of the draw if not already defined to one more than the rest;
        if (draw.LayerNo is 0) draw.LayerNo = draws.Any() ? draws.Max(d => d.LayerNo) + 1 : 0;
        draws.Add(draw);
    }

    /// <summary>
    /// Adds Multiple Draws
    /// </summary>
    /// <param name="draws"></param>
    public void AddDraw(IEnumerable<DrawShape> draws)
    {
        foreach (var draw in draws)
        {
            AddDraw(draw);
        }
    }

    /// <summary>
    /// Removes a Draw
    /// </summary>
    /// <param name="draw"></param>
    public void RemoveDraw(DrawShape draw)
    {
        draws.Remove(draw);
        //Set the new Bounding Box
        boundingBox = MathCalc.GetShapesBoundingBox(draws);
    }

    /// <summary>
    /// Flips the Draw Horizontally
    /// </summary>
    /// <param name="flipOriginX">The X Coordinate about which we want to flip the Draw - Putting ContainersCenterX as the originX  - the Draw will flip according to the center of the Container</param>
    public override void FlipHorizontally(double flipOriginX)
    {
        //Flip Horizontally all individual Parts
        foreach (var draw in draws)
        {
            draw.FlipHorizontally(flipOriginX);
        }
        //Flip also the Bounding Box
        boundingBox.FlipHorizontally(flipOriginX);
    }

    public override void FlipVertically(double flipOriginY)
    {
        //Flip Horizontally all individual Parts
        foreach (var draw in draws)
        {
            draw.FlipVertically(flipOriginY);
        }
        //Flip also the Bounding Box
        boundingBox.FlipVertically(flipOriginY);
    }

    /// <summary>
    /// Translates the Shape over the X axis by the given amount
    /// </summary>
    /// <param name="translateX">The amount to Translate (takes also negative values)</param>
    public override void TranslateX(double translateX)
    {
        foreach (var draw in draws)
        {
            draw.TranslateX(translateX);
        }
        //Translate also the Bounding Box
        boundingBox.TranslateX(translateX);
    }

    /// <summary>
    /// Translates the Shape over the Y axis by the given amount
    /// </summary>
    /// <param name="translateY">The amount to Translate (takes also negative values)</param>
    public override void TranslateY(double translateY)
    {
        foreach (var draw in draws)
        {
            draw.TranslateY(translateY);
        }
        //Translate also the Bounding Box
        boundingBox.TranslateY(translateY);
    }

    public override RectangleDraw GetBoundingBoxRectangle()
    {
        return boundingBox;
    }
    public override double GetBoundingBoxCenterX()
    {
        return boundingBox.ShapeCenterX;
    }
    public override double GetBoundingBoxCenterY()
    {
        return boundingBox.ShapeCenterY;
    }
    public override double GetBoundingBoxLength()
    {
        return boundingBox.Length;
    }
    public override double GetBoundingBoxHeight()
    {
        return boundingBox.Height;
    }
    public override string GetShapePathData()
    {
        StringBuilder builder = new();
        foreach (var draw in draws.OrderBy(d => d.LayerNo))
        {
            builder.Append(draw.GetShapePathData());
        }
        return builder.ToString();
    }
    public override DrawShape CloneSelf()
    {
        CompositeDraw composite = new();
        foreach (var draw in draws)
        {
            composite.AddDraw(draw.CloneSelf());
        }
        return composite;
    }
    public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart)
    {
        // Find the Difference with the BoundingBox's XCenter or XStart
        double difference = newX - (centerOrStart is CSCoordinate.Center
            ? BoundingBoxCenterX
            : BoundingBoxCenterX - BoundingBoxLength / 2d);
        // Translate the Difference for all shapes
        TranslateX(difference);
        // Translate also the Bounding Box
        boundingBox.TranslateX(difference);
    }
    public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart)
    {
        // Find the Difference with the BoundingBox's YCenter or YStart
        double difference = newY - (centerOrStart is CSCoordinate.Center
            ? BoundingBoxCenterY
            : BoundingBoxCenterY - BoundingBoxHeight / 2d);
        // Translate the Difference for all shapes
        TranslateY(difference);

        // Translate also the Bounding Box
        boundingBox.TranslateY(difference);
    }
}

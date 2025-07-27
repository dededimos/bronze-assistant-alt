using CommonInterfacesBronze;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace DrawingLibrary.Interfaces
{
    public interface ITechnicalDrawing : IDeepClonable<ITechnicalDrawing>
    {
        IReadOnlyList<IDrawing> AllDrawings { get; }
        RectangleInfo BoundingBox { get; }
        DrawContainerOptions ContainerOptions { get; set; }
        IReadOnlyList<DimensionLineDrawing> Dimensions { get; }
        IReadOnlyList<IDrawing> Drawings { get; }
        IReadOnlyList<IDrawing> HelpLines { get; }
        int LayerNo { get; set; }
        string Name { get; set; }
        RectangleInfo TotalBoundingBox { get; }

        void AddDimension(DimensionLineDrawing drawing);
        void AddDrawing(IDrawing drawing);
        void AddHelpLine(IDrawing drawing);
        void CenterDrawToContainer();
        void FlipHorizontally(double flipOriginY = double.NaN);
        void FlipVertically(double flipOriginX = double.NaN);
        RectangleInfo GetBoundingBox();
        RectangleInfo GetTotalBoundingBox();
        void Scale(double scaleFactor, PointXY origin);
        void ScaleDrawToContainer();
        void ScaleTotalDrawToContainer();
        void SetLocation(double newX, double newY);
        void SetLocation(PointXY newLocation);
        void SetTotalDrawLocation(PointXY newLocation);
        void TransformOnlyToPositivePositioning();
        void Translate(double translateX, double translateY);
        void TranslateX(double translateX);
        void TranslateY(double translateY);
    }
}
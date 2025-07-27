using CommonInterfacesBronze;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace ShapesLibrary.Interfaces
{
    public interface IShapeInfo : ILocateable, IDeepClonable<IShapeInfo>
    {
        ShapeInfoType ShapeType { get; }
        void FlipHorizontally(double flipOriginX);
        void FlipVertically(double flipOriginY);
        RectangleInfo GetBoundingBox();
        bool AreBoundingBoxesIntersecting(ShapeInfo other);
        bool Contains(PointXY point);
        bool Contains(ShapeInfo shape);
        bool ContainsSimpleRectangle(RectangleInfo rect);
        bool IntersectsWithPoint(PointXY point);
        bool IntersectsWithShape(ShapeInfo shape);
        ProjectionInfo GetProjectionOntoAxis(Vector2D axis);
        PointXY GetLocation();
        double GetPerimeter();
        ShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent);
        void RotateAntiClockwise();
        void RotateClockwise();
        void Scale(double scaleFactor);
        /// <summary>
        /// Scales the Shape from a given OriginXY 
        /// </summary>
        /// <param name="scaleFactor">scale Factor</param>
        /// <param name="origin">The originXY of the Scale transformation (ex. Rectangle Container scales from its center with inside a Circle , the OrginXY for the Scale of the Circle is RectangleCenterX , RectangleCenterY)</param>
        void ScaleFromOrigin(double scaleFactor, PointXY origin);
        void Translate(double translateX, double translateY);
        void TranslateX(double translateX);
        void TranslateY(double translateY);
        double GetTotalLength();
        double GetTotalHeight();
        void SetTotalLength(double length);
        void SetTotalHeight(double height);
    }
}

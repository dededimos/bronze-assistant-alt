namespace ShapesLibrary.Exceptions
{
    public class NotSupportedIntersectionException(ShapeInfo shape, object shapeOrPoint)
    : Exception($"Intersection Check is not Supported for {shape.GetType().Name} with {shapeOrPoint.GetType().Name}"){ }

}

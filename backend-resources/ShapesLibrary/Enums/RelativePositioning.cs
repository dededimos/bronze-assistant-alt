namespace ShapesLibrary.Enums
{
    /// <summary>
    /// The Position of an Object relative to another object in comparing their Centroids
    /// </summary>
    [Flags]
    public enum RelativePositioning
    {
        Left = 1,
        Right = 2,
        Above = 4,
        Below = 8,
        Center = 16,
    }
}

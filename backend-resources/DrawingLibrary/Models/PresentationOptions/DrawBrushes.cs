namespace DrawingLibrary.Models.PresentationOptions
{
    public static class DrawBrushes
    {
        public static DrawBrush Empty { get; } = new(string.Empty);
        public static DrawBrush Black { get; } = new("#000000");
        public static DrawBrush White { get; } = new("#FFFFFF");
        public static DrawBrush Red { get; } = new("#FF0000");
        public static DrawBrush Gray { get; } = new("#808080");
        public static DrawBrush Transparent { get; } = new("#00000000");
        public static DrawBrush GlassGradientBlue { get; } = new(90, new("#8A9BAE", 0), new("#A0B8D8", 0.3), new("#C0D8E8", 0.5), new("#A0B8D8", 0.7), new("#8A9BAE", 1.0));
        public static DrawBrush SandblastGradientGray { get; } = new(45, new("#B0B0B0", 0), new("#D0D0D0", 0.25), new("#E0E0E0", 0.5), new("#D0D0D0", 0.75), new("#B0B0B0", 1.0));
    }
}

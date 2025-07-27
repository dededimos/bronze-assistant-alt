using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace DrawingLibrary.Models
{
    public abstract class ShapeDrawing<TShape, TShapeDimensionOptions> : IDrawing, IDimensionableDrawing, IDeepClonable<ShapeDrawing<TShape, TShapeDimensionOptions>>
        where TShape : ShapeInfo
        where TShapeDimensionOptions : ShapeDimensionsPresentationOptions, new()
    {
        protected GraphicPathDataBuilder pathDataBuilder = new();
        protected ShapeDrawing(TShape shape)
        {
            Name = this.GetType().Name;
            originalShape = shape;
            Shape = (TShape)shape.GetDeepClone();
        }
        protected ShapeDrawing(TShape shape, DrawingPresentationOptions? presentationOptions) : this(shape)
        {
            if (presentationOptions is not null) PresentationOptions = presentationOptions;
        }
        public string Name { get; set; } = string.Empty;
        public string UniqueId { get; protected set; } = Guid.NewGuid().ToString();
        protected readonly TShape originalShape;
        public TShape Shape { get; protected set; }
        public double LocationX { get => Shape.LocationX; }
        public double LocationY { get => Shape.LocationY; }
        public int LayerNo { get; protected set; }

        private List<IDrawing> clips = [];
        public IReadOnlyList<IDrawing> Clips { get => clips; }

        public DrawingPresentationOptions PresentationOptions { get; set; } = new();
        public abstract TShapeDimensionOptions DimensionsPresentationOptions { get; set; }
        public virtual string? DrawingText { get; private set; } = null;
        public LineInfo? TextAnchorLine { get => GetTextAnchorLine(); }

        protected virtual LineInfo GetTextAnchorLine()
        {
            //Anchor the text in the middle of the bounding box of the Shape
            //Any Drawing can override it to place it elsewhere (ex. Dimension Drawing)
            var bBox = Shape.GetBoundingBox();
            return new(bBox.LocationX - bBox.Length / 2d, bBox.LocationY, bBox.LocationX + bBox.Length / 2d, bBox.LocationY);
        }
        public virtual IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            List<DimensionLineDrawing> dims = [];
            if (DimensionsPresentationOptions.ShowDimensions)
            {
                if (DimensionsPresentationOptions.ShowLength) dims.Add(GetLengthDimension());
                if (DimensionsPresentationOptions.ShowHeight) dims.Add(GetHeightDimension());
                if (DimensionsPresentationOptions.ShowClipsDimensions) dims.AddRange(GetClipsDimensions());
            }
            return dims;
        }
        public TShape GetOriginalShape() => originalShape;
        public void SetLayer(int layerNo)
        {
            LayerNo = layerNo;
        }
        public virtual void SetText(string? text)
        {
            DrawingText = text;
        }
        public virtual RectangleInfo GetBoundingBox()
        {
            return Shape.GetBoundingBox();
        }
        public void Scale(double scaleFactor)
        {
            Shape.Scale(scaleFactor);
            foreach (var clip in Clips)
            {
               clip.Scale(scaleFactor);
            }
        }
        public void ScaleFromOrigin(double scaleFactor, PointXY origin)
        {
            Shape.ScaleFromOrigin(scaleFactor, origin);
            foreach (var clip in Clips)
            {
                clip.ScaleFromOrigin(scaleFactor,origin);
            }
        }
        public void SetLocation(PointXY newLocation)
        {
            SetLocation(newLocation.X, newLocation.Y);
        }
        public void SetLocation(double newX, double newY)
        {
            //Find the Diff with the Clips location and pass also the new location to the clip
            foreach (var clip in Clips)
            {
                var diffX = newX - Shape.LocationX;
                var diffY = newY - Shape.LocationY;
                var newClipX = clip.LocationX + diffX;
                var newClipY = clip.LocationY + diffY;
                clip.SetLocation(newClipX, newClipY);
            }
            Shape.LocationX = newX;
            Shape.LocationY = newY;
        }
        public void Translate(double translateX, double translateY)
        {
            Shape.Translate(translateX, translateY);
            foreach (var clip in Clips)
            {
                clip.Translate(translateX, translateY);
            }
        }
        public void TranslateX(double translateX)
        {
            Shape.TranslateX(translateX);
            foreach (var clip in Clips)
            {
                clip.TranslateX(translateX);
            }
        }
        public void TranslateY(double translateY)
        {
            Shape.TranslateY(translateY);
            foreach (var clip in Clips)
            {
                clip.TranslateY(translateY);
            }
        }
        public void FlipHorizontally(double flipOriginX)
        {
            Shape.FlipHorizontally(flipOriginX);
            foreach (var clip in Clips)
            {
                clip.FlipHorizontally(flipOriginX);
            }
        }
        public void FlipVertically(double flipOriginY)
        {
            Shape.FlipVertically(flipOriginY);
            foreach (var clip in Clips)
            {
                clip.FlipVertically(flipOriginY);
            }
        }

        public virtual ShapeDrawing<TShape, TShapeDimensionOptions> GetDeepClone()
        {
            var clone = (ShapeDrawing<TShape, TShapeDimensionOptions>)this.MemberwiseClone();
            clone.Shape = (TShape)this.Shape.GetDeepClone();
            clone.PresentationOptions = this.PresentationOptions.GetDeepClone();
            clone.DimensionsPresentationOptions = (TShapeDimensionOptions)this.DimensionsPresentationOptions.GetDeepClone();
            clone.clips = this.Clips.Select(c=> c.GetDeepClone()).ToList();
            return clone;
        }
        public virtual ShapeDrawing<TShape, TShapeDimensionOptions> GetDeepClone(bool generateUniqueId)
        {
            var clone = GetDeepClone();
            if (generateUniqueId) clone.UniqueId = Guid.NewGuid().ToString();
            return clone;
        }

        /// <summary>
        /// Builds the Normal Path Data of the Shape
        /// </summary>
        protected abstract void BuildPathData();
        /// <summary>
        /// Builds the Reversed (hole) Path Data of the Shape
        /// </summary>
        protected abstract void BuildReversePathData();
        /// <summary>
        /// Adds the Clip's Path Data to the already built Path Data
        /// </summary>
        protected void AddClipPathData()
        {
            foreach (var clip in Clips)
            {
                pathDataBuilder.AddExternalPathData(" ").AddExternalPathData(clip.GetReversePathDataString());
            }
        }
        /// <summary>
        /// Adds the Clip's Inverse Path Data to the already built Path Data
        /// </summary>
        protected void AddReverseClipPathData()
        {
            foreach (var clip in Clips)
            {
                pathDataBuilder.AddExternalPathData(" ").AddExternalPathData(clip.GetPathDataString());
            }
        }
        /// <summary>
        /// Retrieves the Path Data of this Drawing
        /// </summary>
        public string GetPathDataString()
        {
            pathDataBuilder.ResetBuilder();
            BuildPathData();
            AddClipPathData();
            return pathDataBuilder.GetPathData();
        }
        /// <summary>
        /// Retrieves the Reverse Path Data of this Drawing
        /// </summary>
        public string GetReversePathDataString()
        {
            pathDataBuilder.ResetBuilder();
            BuildReversePathData();
            AddReverseClipPathData();
            return pathDataBuilder.GetPathData();
        }
        /// <summary>
        /// Returns a cloned Drawing centered to the specified Rectangular Container
        /// </summary>
        /// <param name="container">The Rectangular Container where to center the Clone</param>
        /// <returns></returns>
        public virtual ShapeDrawing<TShape, TShapeDimensionOptions> GetCloneCenteredToContainer(RectangleInfo container)
        {
            var clone = this.GetDeepClone();
            var cloneBox = clone.GetBoundingBox();
            var deltaX = container.LocationX - cloneBox.LocationX;
            var deltaY = container.LocationY - cloneBox.LocationY;
            clone.SetLocation(clone.LocationX + deltaX, clone.LocationY + deltaY);
            return clone;
        }

        IDrawing IDeepClonable<IDrawing>.GetDeepClone()
        {
            return GetDeepClone();
        }
        IDrawing IDrawing.GetDeepClone(bool generateUniqueId)
        {
            return GetDeepClone(generateUniqueId);
        }

        /// <summary>
        /// Returns the Height Dimension of any Shape
        /// </summary>
        /// <returns></returns>
        protected DimensionLineDrawing GetHeightDimension()
        {
            var bBox = Shape.GetBoundingBox();
            var builder = DimensionLineDrawing.GetBuilder();

            if (DimensionsPresentationOptions.HeightPosition == Enums.RectangleHeightDimensionPosition.Left) //place to the Left
                builder.SetStart(new(bBox.LeftX - DimensionsPresentationOptions.HeightMarginFromShape, bBox.TopY))
                       .SetEnd(new(bBox.LeftX - DimensionsPresentationOptions.HeightMarginFromShape, bBox.BottomY));
            else
                builder.SetStart(new(bBox.RightX + DimensionsPresentationOptions.HeightMarginFromShape, bBox.TopY)) //place to the Right
                       .SetEnd(new(bBox.RightX + DimensionsPresentationOptions.HeightMarginFromShape, bBox.BottomY));

            var height = builder
               .SetPresentationOptions(DimensionsPresentationOptions.HeightPresentationOptions)
               .SetDimensionLineOptions(DimensionsPresentationOptions.HeightLineOptions)
               .SetDimensionValue(originalShape.GetTotalHeight())
               .SetName($"Height ({this.Name})")
               .BuildDimensionLine();
            return height;
        }
        /// <summary>
        /// Returns the Length Dimension of any Shape
        /// </summary>
        /// <returns></returns>
        protected DimensionLineDrawing GetLengthDimension()
        {
            var bBox = Shape.GetBoundingBox();
            var builder = DimensionLineDrawing.GetBuilder();

            if (DimensionsPresentationOptions.LengthPosition == Enums.RectangleLengthDimensionPosition.Top) //place to the Top
                builder.SetStart(new(bBox.LeftX, bBox.TopY - DimensionsPresentationOptions.LengthMarginFromShape))
                       .SetEnd(new(bBox.RightX, bBox.TopY - DimensionsPresentationOptions.LengthMarginFromShape));
            else //place to the Bottom
                builder.SetStart(new(bBox.LeftX, bBox.BottomY + DimensionsPresentationOptions.LengthMarginFromShape))
                       .SetEnd(new(bBox.RightX, bBox.BottomY + DimensionsPresentationOptions.LengthMarginFromShape));

            var length = builder
               .SetPresentationOptions(DimensionsPresentationOptions.LengthPresentationOptions)
               .SetDimensionLineOptions(DimensionsPresentationOptions.LengthLineOptions)
               .SetDimensionValue(originalShape.GetTotalLength())
               .SetName($"Length ({this.Name})")
               .BuildDimensionLine();
            return length;
        }
        protected LineDrawing GetUnpositionedVerticalCenterHelpline()
        {
            var totalHeight = Shape.GetTotalHeight();
            LineInfo line = new(0, 0, 0, totalHeight);
            return new LineDrawing(line, DimensionsPresentationOptions.HelpLinesPresentationOptions);
        }
        protected LineDrawing GetUnpositionedHorizontalCenterHelpLine()
        {
            var totalLength = Shape.GetTotalLength();
            LineInfo line = new(0, 0, totalLength, 0);
            return new LineDrawing(line, DimensionsPresentationOptions.HelpLinesPresentationOptions);
        }
        private List<DimensionLineDrawing> GetClipsDimensions()
        {
            List<DimensionLineDrawing> dimensions = [];
            foreach (var clip in Clips)
            {
                if (clip is IDimensionableDrawing dimensionable)
                {
                    dimensions.AddRange(dimensionable.GetDimensionsDrawings());
                }
            }
            return dimensions;
        }

        public abstract IEnumerable<IDrawing> GetHelpLinesDrawings();

        IDrawing IDrawing.GetCloneCenteredToContainer(RectangleInfo container)
        {
            return GetCloneCenteredToContainer(container);
        }

        public void AddClip(IDrawing clipDraw)
        {
            clips.Add(clipDraw);
        }
    }
}

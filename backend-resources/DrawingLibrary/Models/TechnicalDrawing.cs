using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models
{
    public class TechnicalDrawing : ITechnicalDrawing , IDeepClonable<TechnicalDrawing>
    {
        public TechnicalDrawing(string name)
        {
            _boundingBoxCached = new(GetBoundingBox);
            _totalBoundingBoxCached = new(GetTotalBoundingBox);
            Name = name;
        }

        public string Name { get; set; } = string.Empty;
        public DrawContainerOptions ContainerOptions { get; set; } = new();

        public IReadOnlyList<IDrawing> AllDrawings { get => [.. drawings, .. drawingsDimensions, .. helpLines]; }

        private List<IDrawing> drawings = [];
        public IReadOnlyList<IDrawing> Drawings { get => drawings; }

        private List<DimensionLineDrawing> drawingsDimensions = [];
        public IReadOnlyList<DimensionLineDrawing> Dimensions { get => [.. drawingsDimensions]; }

        private List<IDrawing> helpLines = [];
        public IReadOnlyList<IDrawing> HelpLines { get => helpLines; }

        private Lazy<RectangleInfo> _boundingBoxCached;
        private Lazy<RectangleInfo> _totalBoundingBoxCached;
        public RectangleInfo BoundingBox { get => _boundingBoxCached.Value; }
        public RectangleInfo TotalBoundingBox { get => _totalBoundingBoxCached.Value; }

        public int LayerNo { get; set; }

        public RectangleInfo GetBoundingBox()
        {
            return MathCalculations.Containment.GetBoundingBox(Drawings.Select(d => d.GetBoundingBox()));
        }
        public RectangleInfo GetTotalBoundingBox()
        {
            return MathCalculations.Containment.GetBoundingBox(AllDrawings.Select(d => d.GetBoundingBox()));
        }
        public virtual void AddDrawing(IDrawing drawing)
        {
            drawings.Add(drawing);
            if (drawing is IDimensionableDrawing dimensionable)
            {
                drawingsDimensions.AddRange(dimensionable.GetDimensionsDrawings());
                helpLines.AddRange(dimensionable.GetHelpLinesDrawings());
            }

            //refresh the bounding box cache as draws have changed
            ReinitilizeCachedBoundingBoxes();
        }
        public virtual void AddDimension(DimensionLineDrawing drawing)
        {
            drawingsDimensions.Add(drawing);
            ReinitilizeCachedBoundingBoxes();
        }
        public virtual void AddHelpLine(IDrawing drawing)
        {
            helpLines.Add(drawing);
            ReinitilizeCachedBoundingBoxes();
        }
        public void SetLocation(PointXY newLocation)
        {
            SetLocation(newLocation.X, newLocation.Y);
        }
        public void SetLocation(double newX, double newY)
        {
            var deltaX = newX - BoundingBox.LocationX;
            var deltaY = newY - BoundingBox.LocationY;

            foreach (var d in AllDrawings)
            {
                d.SetLocation(d.LocationX + deltaX, d.LocationY + deltaY);
            }
            ReinitilizeCachedBoundingBoxes();
        }
        public void SetTotalDrawLocation(PointXY newLocation)
        {
            this.Translate(newLocation.X - TotalBoundingBox.LocationX, newLocation.Y - TotalBoundingBox.LocationY);
        }
        public void Scale(double scaleFactor, PointXY origin)
        {
            foreach (IDrawing drawing in AllDrawings)
            {
                drawing.ScaleFromOrigin(scaleFactor, origin);
            }
            ReinitilizeCachedBoundingBoxes();
        }
        public void Translate(double translateX, double translateY)
        {
            foreach (var d in AllDrawings)
            {
                d.Translate(translateX, translateY);
            }
        }
        public void TranslateX(double translateX)
        {
            foreach (var d in AllDrawings)
            {
                d.TranslateX(translateX);
            }
        }
        public void TranslateY(double translateY)
        {
            foreach (var d in AllDrawings)
            {
                d.TranslateY(translateY);
            }
        }
        public void FlipHorizontally(double flipOriginX = double.NaN)
        {
            if (double.IsNaN(flipOriginX)) flipOriginX = BoundingBox.LocationX;
            foreach (var g in AllDrawings)
            {
                g.FlipHorizontally(flipOriginX);
            }
        }
        public void FlipVertically(double flipOriginY = double.NaN)
        {
            if (double.IsNaN(flipOriginY)) flipOriginY = BoundingBox.LocationY;
            foreach (var g in AllDrawings)
            {
                g.FlipVertically(flipOriginY);
            }
        }


        public void ScaleDrawToContainer()
        {
            if (ContainerOptions.ContainerLength <= 0 || ContainerOptions.ContainerHeight <= 0)
                throw new Exception($"Cannot scale to a negative or zero Container - Please Check that Container Length/Height is properly set");

            if (ContainerOptions.MaxDimensionDepictedToScale > 0)
            {
                //Scale The Draw so that certain dimensions appear Smaller even if not scaled to Container
                //If the Container is 500 x 500 then if we do not scale any bigger draws will appear the same as the 500x500
                //So taking The Max and dividing by the max dimension of the Container we find how much to scale
                var maxContainerDimension = Math.Max(ContainerOptions.ContainerLength, ContainerOptions.ContainerHeight);
                var preScaleFactor = maxContainerDimension / ContainerOptions.MaxDimensionDepictedToScale;
                //Do it only if the MaxDimensionDepictedToScale is actually more than the Container
                if (preScaleFactor < 1)
                {
                    Scale(preScaleFactor, BoundingBox.GetLocation());
                }
            }


            var bBox = GetBoundingBox();
            //test to scale

            //find scale 
            var lengthScale = ContainerOptions.ContainerLength / bBox.Length;
            var heightScale = ContainerOptions.ContainerHeight / bBox.Height;
            //find max number and use as scale
            var scale = Math.Min(lengthScale, heightScale);
            if (scale >= 1) return;
            Scale(scale, BoundingBox.GetLocation());
        }
        public void ScaleTotalDrawToContainer()
        {
            if (ContainerOptions.ContainerLength <= 0 || ContainerOptions.ContainerHeight <= 0) throw new Exception($"Cannot scale to a negative or zero Container - Please Check that Container Length/Height is properly set");

            //The extra size of the Dimensions is taken into account when scaling the total draw .
            //The Extra size of the dimensions is always the same whatever the scale of the main draw (as the lines follow the draw and only their distances change)
            //We need the distances and text not to scale So that the viewer can see them whatever the scale of the Draw

            //These are the differences that when applied to the scaled draw they must equal the Container Dimensions
            //helping to find the scale of the draw to fit its dimensions to the Container.
            double heightDifference = TotalBoundingBox.Height - BoundingBox.Height;
            double lengthDifference = TotalBoundingBox.Length - BoundingBox.Length;

            //find the Container Dimensions that can fit the draw wihout its dimensions 
            var limitedContainerLength = ContainerOptions.ContainerLength - lengthDifference;
            var limitedContainerHeight = ContainerOptions.ContainerHeight - heightDifference;

            //Apply all scalings based on that size

            if (ContainerOptions.MaxDimensionDepictedToScale > 0)
            {
                //Scale The Draw so that certain dimensions appear Smaller even if not scaled to Container
                //If the Container is 500 x 500 then if we do not scale any bigger draws will appear the same as the 500x500
                //So taking The Max and dividing by the max dimension of the Container we find how much to scale
                var maxContainerDimension = Math.Max(limitedContainerLength, limitedContainerHeight);
                var preScaleFactor = maxContainerDimension / ContainerOptions.MaxDimensionDepictedToScale;
                //Do it only if the MaxDimensionDepictedToScale is actually more than the Container
                if (preScaleFactor < 1)
                {
                    Scale(preScaleFactor, BoundingBox.GetLocation());
                }
            }

            //find scale 
            var lengthScale = limitedContainerLength / BoundingBox.Length;
            var heightScale = limitedContainerHeight / BoundingBox.Height;
            //find min number and use as scale
            var scale = Math.Min(lengthScale, heightScale);
            if (scale >= 1) return; //do not increase to fit the container
            Scale(scale, BoundingBox.GetLocation());
        }
        public void CenterDrawToContainer()
        {
            SetLocation(ContainerOptions.ContainerLength / 2d, ContainerOptions.ContainerHeight / 2d);
        }
        public void TransformOnlyToPositivePositioning()
        {
            if (TotalBoundingBox.LeftX < 0)
            {
                TranslateX(-TotalBoundingBox.LeftX);
            }
            if (TotalBoundingBox.TopY < 0)
            {
                TranslateY(-TotalBoundingBox.TopY);
            }
        }
        private void ReinitilizeCachedBoundingBoxes()
        {
            _boundingBoxCached = new(GetBoundingBox());
            _totalBoundingBoxCached = new(GetTotalBoundingBox());
        }

        public TechnicalDrawing GetDeepClone()
        {
            var clone = (TechnicalDrawing)this.MemberwiseClone();
            clone.ContainerOptions = this.ContainerOptions.GetDeepClone();
            clone.drawings = this.drawings.GetDeepClonedList();
            clone.drawingsDimensions = this.drawingsDimensions.GetDeepClonedList();
            clone.helpLines = this.helpLines.GetDeepClonedList();
            clone.ReinitilizeCachedBoundingBoxes();
            return clone;
        }

        ITechnicalDrawing IDeepClonable<ITechnicalDrawing>.GetDeepClone()
        {
            return GetDeepClone();
        }
    }
}

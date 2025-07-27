using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGGlassDrawsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGGlassDrawsLibrary.Helpers.GlassDrawExtensions;

namespace SVGGlassDrawsLibrary.ProcessDraws
{
    public class StepProcessDraw : RectangleDraw
    {
        private readonly StepProcess stepProcess;

        private StepProcessDraw(StepProcess stepProcess):base(stepProcess.StepLength,stepProcess.StepHeight)
        {
            this.stepProcess = stepProcess;
        }

        public IEnumerable<DimensionLineDraw> GenerateDimensionLines()
        {
            //Generate Length Dimension bottom
            DimensionLineDraw lengthDim = new(StartX, EndY, this.EndX, EndY, 0)
            {
                RepresentedDimensionText = Length.ToString()
            };
            lengthDim.TranslateY(100); //Margin from Draw

            //Generate Height Dimension Left
            DimensionLineDraw HeightDim = new(StartX, StartY, StartX, EndY, 90)
            {
                RepresentedDimensionText = Height.ToString()
            };
            HeightDim.TranslateX(-50); //Margin from Draw
            return new DimensionLineDraw[] {lengthDim,HeightDim};
        }

        /// <summary>
        /// Gets the Steps Path Data
        /// </summary>
        /// <returns></returns>
        public override string GetShapePathData()
        {
            PathDataBuilder builder = new();

            return builder.MoveTo(StartX, StartY)
                          .AddRectangleHole(Length,Height,CornerRadius,RoundedCorners)
                          .CloseDraw()
                          .GetPathData();
        }
        public override RectangleDraw CloneSelf()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Creates a StepProcessDraw
        /// </summary>
        /// <param name="stepProcess">the step Process</param>
        /// <param name="ownerGlass">The Glass owning the Step Process</param>
        /// <returns>The Draw</returns>
        public static StepProcessDraw Create(StepProcess stepProcess,Glass ownerGlass)
        {
            var dx = stepProcess.GetProcessDistanceFromEdgeX(ownerGlass);
            var dy = stepProcess.GetProcessDistanceFromEdgeY(ownerGlass);
            StepProcessDraw draw = new(stepProcess);
            draw.CornerRadius = 5;

            if (stepProcess.HorizontalDistancing is HorizDistancing.FromLeft)
            {
                draw.SetCenterOrStartX(dx, CSCoordinate.Start);
            }
            else if (stepProcess.HorizontalDistancing is HorizDistancing.FromRight)
            {
                draw.SetCenterOrStartX(dx - stepProcess.StepLength, CSCoordinate.Start);
            }
            else throw new NotSupportedException($"{nameof(HorizDistancing)} with value : '{stepProcess.HorizontalDistancing}' is not Supported for StepProcess Placement");

            if (stepProcess.VerticalDistancing is VertDistancing.FromBottom)
            {
                draw.SetCenterOrStartY(dy - stepProcess.StepHeight, CSCoordinate.Start);
            }
            else if (stepProcess.VerticalDistancing is VertDistancing.FromTop)
            {
                draw.SetCenterOrStartY(dy, CSCoordinate.Start);
            }
            else throw new NotSupportedException($"{nameof(VertDistancing)} with value : '{stepProcess.VerticalDistancing}' is not Supported for StepProcess Placement");
            
            // Step is on the Left Bottom
            if (draw.StartX is 0 && draw.EndY == ownerGlass.Height) draw.RoundedCorners = CornersToRound.TopRightCorner;
            // Step is on the Right Bottom
            else if (draw.EndX == ownerGlass.Length && draw.EndY == ownerGlass.Height) draw.RoundedCorners = CornersToRound.TopLeftCorner;
            // Step is on the Top Left
            else if (draw.StartX is 0 && draw.StartY is 0) draw.RoundedCorners = CornersToRound.BottomRightCorner;
            // Step is on the Top Right
            else if (draw.StartX == ownerGlass.Length && draw.StartY is 0) draw.RoundedCorners = CornersToRound.BottomLeftCorner;
            else draw.RoundedCorners = CornersToRound.None;
            
            return draw;
        }

    }
}

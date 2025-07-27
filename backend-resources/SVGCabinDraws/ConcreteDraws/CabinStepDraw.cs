using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGCabinDraws.ConcreteDraws
{
    public class CabinStepDraw
    {
        public double WallThicknessPercent { get; set; } = 0.06d;
        public double WallThickness { get; set; }
        public double ArrowThickness { get; set; } = 5;
        public double ArrowLength { get; set; } = 17;
        public (double, double) HelpLinesDashArray { get; set; } = (5, 2);

        public RectangleDraw LeftWall { get; set; }
        public RectangleDraw Floor { get; set; }

        public RectangleDraw Step { get; set; }

        public DimensionLineDraw StepLengthDimension { get; set; }
        public DimensionLineDraw StepHeightDimension { get; set; }

        public LineDraw HelpLine1 { get; set; }
        public LineDraw HelpLine2 { get; set; }

        public CabinStepDraw(double drawLength , double drawHeight)
        {
            WallThickness = drawLength * WallThicknessPercent;

            LeftWall = new()
            {
                Length = WallThickness,
                Height = drawHeight,
                Stroke = "",
                Fill = "",
                Name = "LeftWall"
            };
            LeftWall.SetCenterOrStartX(0, DrawShape.CSCoordinate.Start);
            LeftWall.SetCenterOrStartY(0, DrawShape.CSCoordinate.Start);

            Floor = new()
            {
                Length = drawLength,
                Height = WallThickness,
                Stroke = "",
                Fill = "",
                Name = "Floor"
            };
            Floor.SetCenterOrStartX(0, DrawShape.CSCoordinate.Start);
            Floor.SetCenterOrStartY(drawHeight - WallThickness, DrawShape.CSCoordinate.Start);

            Step = new()
            {
                Length = drawLength * 2 / 3d,
                Height = drawHeight / 2d - WallThickness,
                Stroke = "",
                Fill = "",
                Name = "Step"
            };
            Step.SetCenterOrStartX(WallThickness, DrawShape.CSCoordinate.Start);
            Step.SetCenterOrStartY(drawHeight / 2d, DrawShape.CSCoordinate.Start);

            StepLengthDimension = new()
            {
                StartX = WallThickness,
                StartY = drawHeight / 2.5d,
                EndX = drawLength * 2 / 3d + WallThickness,
                EndY = drawHeight / 2.5d,
                ArrowThickness = ArrowThickness,
                ArrowLength = ArrowLength,
                Stroke = "",
                Fill = "",
                Name = "StepLengthDimension",
                AngleWithAxisX = 0
            };

            StepHeightDimension = new()
            {
                StartX = drawLength * 0.96d,
                StartY = drawHeight / 2d,
                EndX = drawLength * 0.96d,
                EndY = drawHeight - WallThickness,
                ArrowThickness = ArrowThickness,
                ArrowLength = ArrowLength,
                Stroke = "",
                Fill = "",
                Name = "StepHeightDimension",
                AngleWithAxisX = 90
            };

            HelpLine1 = new()
            {
                StartX = StepLengthDimension.EndX,
                StartY = StepLengthDimension.EndY,
                EndX = StepLengthDimension.EndX,
                EndY = drawHeight / 2d,
                Stroke = "",
                Fill = "",
                StrokeDashArray = ""
            };

            HelpLine2 = new()
            {
                StartX = WallThickness + drawLength * 2 / 3d,
                StartY = drawHeight / 2d,
                EndX = StepHeightDimension.StartX,
                EndY = StepHeightDimension.StartY,
                Stroke = "",
                Fill = "",
                StrokeDashArray = ""
            };
        }

    }
}

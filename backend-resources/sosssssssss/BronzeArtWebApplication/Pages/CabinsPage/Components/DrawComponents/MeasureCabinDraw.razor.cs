using Microsoft.AspNetCore.Components;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System.Collections.Generic;
using MirrorsModelsLibrary.DrawsBuilder.Models.DrawShapes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;
using System;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using System.Linq;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.DrawComponents
{
    public partial class MeasureCabinDraw : ComponentBase
    {
        [Inject] public ICabinMemoryRepository repo { get; set; }

        #region 1.Drawing (Default)Parameters

        /// <summary>
        /// The Length of the SVG Draw , Default:250 , For StepMode Set to : 140
        /// </summary>
        [Parameter] public int LengthSVG { get; set; } = 250;
        /// <summary>
        /// The Height of the SVG Draw , Default : 150 , For Step Mode Set to : 220
        /// </summary>
        [Parameter] public int HeightSVG { get; set; } = 150;
        /// <summary>
        /// If the Components will be Bordered , Default = true
        /// </summary>
        [Parameter] public bool IsBordered { get; set; } = true;
        /// <summary>
        /// If the Component Should Include the Image Sketch of the Cabin Draw , Default = true
        /// </summary>
        [Parameter] public bool IsImageSketchVisible { get; set; } = true;
        /// <summary>
        /// The Background of the Component , default = lightgray
        /// </summary>
        [Parameter] public string BackgroundColor { get; set; } = "lightgray";
        /// <summary>
        /// The Stroke and Fill of the Dimension Arrows
        /// </summary>
        [Parameter] public string StrokeFillDimensions { get; set; } = "lightslategray";
        /// <summary>
        /// The Stroke of the Walls
        /// </summary>
        [Parameter] public string WallStroke { get; set; } = "black";
        /// <summary>
        /// The Fill of the Walls -- Default is hatchpattern
        /// </summary>
        [Parameter] public string WallFill { get; set; } = "URL(#hatchPattern)";
        private string hatchPatternIdName; //This has to be set so that each time the component is used elswhere the htach has another id (for cases when svglength =0)

        /// <summary>
        /// The Length Dimension Key of the Currently Focused Dimension
        /// </summary>
        [Parameter] public string FocusedDimensionName { get; set; } //The DimensionName that is being Focused
        /// <summary>
        /// The Currently Focused Model Primary/Secondary/Tertiary/Null
        /// </summary>
        [Parameter] public CabinSynthesisModel? FocusedSynthesisModel { get; set; } // The Focused SynthesisModel
        /// <summary>
        /// The Stroke and Fill of the Arrows on the Focused Dimension
        /// </summary>
        [Parameter] public string FocusedDimensionStrokeFill { get; set; } = "#9D080D";
        /// <summary>
        /// The Stroke and Fill of the Dashed Help Lines
        /// </summary>
        [Parameter] public string HelpLinesStrokeFill { get; set; } = "black";
        /// <summary>
        /// The Dash Array of the Help Lines
        /// </summary>
        [Parameter] public string HelpLinesDashArray { get; set; } = "5 2";

        /// <summary>
        /// The Margin from the Bottom and Top of the Draw
        /// </summary>
        [Parameter] public int VerticalMargin { get; set; } = 15;

        /// <summary>
        /// The Thicknness of the Walls As a Percentage of the Draw's Length
        /// </summary>
        [Parameter] public double WallThicknessPercent { get; set; } = 0.06d;

        /// <summary>
        /// The Margin of the Dimensions from the Edges of the Draw as a Percentage of the Draw's Length
        /// </summary>
        [Parameter] public double DimensionMarginPercent { get; set; } = 0.15d;

        #endregion

        /// <summary>
        /// The Draw Number of the Cabin Synthesis
        /// </summary>
        [Parameter] public CabinDrawNumber DrawNumber { get; set; }
        /// <summary>
        /// The Length Dimension Key of the Primary Model
        /// </summary>
        [Parameter] public string PrimaryDimensionName { get; set; }
        /// <summary>
        /// The Length Dimension Key of the Secondary Model
        /// </summary>
        [Parameter] public string SecondaryDimensionName { get; set; }
        /// <summary>
        /// The Length Dimension Key of the Tertiary Model
        /// </summary>
        [Parameter] public string TertiaryDimensionName { get; set; }
        /// <summary>
        /// The Length Size of the Primary Model
        /// </summary>
        [Parameter] public int? PrimaryDimensionValue { get; set; }
        /// <summary>
        /// The Length Size of the Secondary Model
        /// </summary>
        [Parameter] public int? SecondaryDimensionValue { get; set; }
        /// <summary>
        /// The Length Size of the Tertiary Model
        /// </summary>
        [Parameter] public int? TertiaryDimensionValue { get; set; }

        /// <summary>
        /// The Model Number from the Current Synthesis (First(Primary),Second(Secondary),Third(Tertiary)) , default : Primary
        /// </summary>
        [Parameter] public CabinSynthesisModel SynthesisModelNo { get; set; }

        /// <summary>
        /// The Mode of the Draw (Draw or StepDraw) Default is :Draw
        /// </summary>
        [Parameter] public string Mode { get; set; }

        /// <summary>
        /// Wheather the SVG and IMG are Flipped Horizontally (Indicating an Alignment Change for the Showe Structure)
        /// </summary>
        [Parameter] public bool IsFlipped { get; set; }

        private double wallThickness;
        private double arrowThickness;
        private double arrowLength;
        private string imageSketchSRC; //The Image Path
        private List<DrawShape> shapesToDraw;
        private List<DrawShape> dimensionsToDraw;
        private List<DrawShape> helpLinesToDraw;

        protected override void OnParametersSet()
        {
            shapesToDraw = new();
            dimensionsToDraw = new();
            helpLinesToDraw = new();
            hatchPatternIdName = "hatchPattern";

            //Set Wall and Arrow Measures
            wallThickness = WallThicknessPercent * LengthSVG;
            arrowThickness = 5;
            arrowLength = 17;

            //Pick Initilization Process of Component
            if (Mode == "StepDraw")
            {
                InitilizeStepDrawMode();
            }
            else
            {
                InitilizeDrawMode();
            }
        }

        #region 0.Initialization Methods

        /// <summary>
        /// Initilizes the Component for Length Dimensions
        /// </summary>
        private void InitilizeDrawMode()
        {
            imageSketchSRC = CabinDrawNumberSketchImagePath[DrawNumber]; //Get the Sketch Image Path

            //SetDimension Name if not Set
            if (string.IsNullOrEmpty(PrimaryDimensionName))
            {
                PrimaryDimensionName = "";
            }
            if (string.IsNullOrEmpty(SecondaryDimensionName))
            {
                SecondaryDimensionName = "";
            }
            if (string.IsNullOrEmpty(TertiaryDimensionName))
            {
                TertiaryDimensionName = "";
            }

            SetMeasureShapeDrawMode();
        }

        /// <summary>
        /// Initilizes the Component for Step Dimensions
        /// </summary>
        private void InitilizeStepDrawMode()
        {
            imageSketchSRC = CabinDrawNumberStepImagePath[(DrawNumber, SynthesisModelNo)]; //Get the Sketch-Step Image Path
            PrimaryDimensionName = PrimaryDimensionName != "Empty" ? "StepLength" : "";
            SecondaryDimensionName = SecondaryDimensionName != "Empty" ? "StepHeight" : "";
            TertiaryDimensionName = "N/A";

            //Find which parts can have step from this Draw
            //(bool, bool, bool) canHaveStepTuple = ShowerEnclosuresModelsLibrary.Helpers.HelperMethods.CabinDrawCanHaveStep[DrawNumber];
            bool canHaveStep = false;

            var constraintsKey = repo.AllConstraints.Keys.Where(k => k.Item2 == DrawNumber && k.Item3 == SynthesisModelNo).FirstOrDefault();
            if (repo.AllConstraints.TryGetValue(constraintsKey , out CabinConstraints constraints))
            {
                canHaveStep = constraints.CanHaveStep;
            }
            else
            {
                canHaveStep = false;
            }

            ////According to which part we are Drawing set the canHaveStep
            //switch (SynthesisModelNo)
            //{
            //    case CabinSynthesisModel.Primary:
            //        canHaveStep = canHaveStepTuple.Item1;
            //        break;
            //    case CabinSynthesisModel.Secondary:
            //        canHaveStep = canHaveStepTuple.Item2;
            //        break;
            //    case CabinSynthesisModel.Tertiary:
            //        canHaveStep = canHaveStepTuple.Item3;
            //        break;
            //    default:
            //        break;
            //}

            //If It Has a step Continue Otherwise Do not Draw the SVG
            if (canHaveStep)
            {
                SetMeasureShapeStepDrawMode();
            }
            else
            {
                LengthSVG = 0;
                hatchPatternIdName = ""; //So that the other hatch patterns do not get vanished
            }

        }

        /// <summary>
        /// Populates the ShapesToDraw and the DimensionsToDraw Lists
        /// </summary>
        private void SetMeasureShapeDrawMode()
        {
            switch (DrawNumber)
            {
                case CabinDrawNumber.Draw9S:
                case CabinDrawNumber.Draw94:
                case CabinDrawNumber.Draw9B:
                case CabinDrawNumber.DrawVS:
                case CabinDrawNumber.DrawV4:
                case CabinDrawNumber.DrawWS:
                case CabinDrawNumber.DrawNP44:
                case CabinDrawNumber.DrawQP44:
                case CabinDrawNumber.DrawNB31:
                case CabinDrawNumber.DrawQB31:
                case CabinDrawNumber.DrawDB51:
                case CabinDrawNumber.DrawHB34:
                    SetWallToWallMeasureShape();
                    break;
                case CabinDrawNumber.Draw9S9F:
                case CabinDrawNumber.Draw949F:
                case CabinDrawNumber.Draw9A:
                case CabinDrawNumber.Draw9B9F:
                case CabinDrawNumber.DrawVSVF:
                case CabinDrawNumber.DrawV4VF:
                case CabinDrawNumber.DrawVA:
                case CabinDrawNumber.Draw2CornerNP46:
                case CabinDrawNumber.Draw2CornerQP46:
                case CabinDrawNumber.DrawCornerNP6W45:
                case CabinDrawNumber.DrawCornerQP6W45:
                case CabinDrawNumber.DrawCornerNB6W32:
                case CabinDrawNumber.DrawCornerQB6W32:
                case CabinDrawNumber.Draw2CornerNB33:
                case CabinDrawNumber.Draw2CornerQB33:
                case CabinDrawNumber.DrawCornerDB8W52:
                case CabinDrawNumber.Draw2CornerDB53:
                case CabinDrawNumber.DrawCornerHB8W35:
                case CabinDrawNumber.Draw2CornerHB37:
                case CabinDrawNumber.Draw9C:
                    SetCornerMeasureShape();
                    break;
                case CabinDrawNumber.Draw949F9F:
                case CabinDrawNumber.Draw9S9F9F:
                case CabinDrawNumber.Draw9A9F:
                case CabinDrawNumber.Draw9B9F9F:
                case CabinDrawNumber.Draw9C9F:
                    SetPIMeasureShape();
                    break;
                case CabinDrawNumber.Draw2StraightNP48:
                case CabinDrawNumber.Draw2StraightQP48:
                case CabinDrawNumber.DrawStraightNP6W47:
                case CabinDrawNumber.DrawStraightQP6W47:
                case CabinDrawNumber.DrawStraightNB6W38:
                case CabinDrawNumber.DrawStraightQB6W38:
                case CabinDrawNumber.Draw2StraightNB41:
                case CabinDrawNumber.Draw2StraightQB41:
                case CabinDrawNumber.DrawStraightDB8W59:
                case CabinDrawNumber.Draw2StraightDB61:
                case CabinDrawNumber.DrawStraightHB8W40:
                case CabinDrawNumber.Draw2StraightHB43:
                    SetWallToWallTwoMeasureShape();
                    break;
                case CabinDrawNumber.Draw8W:
                case CabinDrawNumber.Draw9F:
                case CabinDrawNumber.DrawVF:
                    SetWMeasureShape();
                    break;
                case CabinDrawNumber.DrawE:
                    SetEMeasureShape();
                    break;
                case CabinDrawNumber.Draw8WFlipper81:
                    SetDraw81MeasureShape();
                    break;
                case CabinDrawNumber.Draw2Corner8W82:
                    SetDraw82MeasureShape();
                    break;
                case CabinDrawNumber.Draw1Corner8W84:
                    SetDraw84MeasureShape();
                    break;
                case CabinDrawNumber.Draw2Straight8W85:
                    SetDraw85MeasureShape();
                    break;
                case CabinDrawNumber.Draw2CornerStraight8W88:
                    SetDraw88MeasureShape();
                    break;
                case CabinDrawNumber.Draw8W40:
                case CabinDrawNumber.DrawNV:
                case CabinDrawNumber.DrawNV2:
                case CabinDrawNumber.DrawMV2:
                    SetDrawBathtubMeasureShape();
                    break;
                case CabinDrawNumber.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Populates the ShapesToDraw and the DimensionsToDraw Lists
        /// </summary>
        private void SetMeasureShapeStepDrawMode()
        {
            RectangleDraw floor = CreateFloor();
            RectangleDraw leftWall = CreateLeftWall();

            RectangleDraw step = new();
            step.SetCenterOrStartX(wallThickness, DrawShape.CSCoordinate.Start);
            step.SetCenterOrStartY(HeightSVG / 2d, DrawShape.CSCoordinate.Start);
            step.Length = LengthSVG * 2 / 3d;
            step.Height = HeightSVG / 2d - wallThickness;
            step.Stroke = WallStroke;
            step.Fill = WallFill;
            step.Name = "Step";

            DimensionLineDraw dimensionStepLength = new();
            dimensionStepLength.StartX = wallThickness;
            dimensionStepLength.StartY = HeightSVG / 2.5d;
            dimensionStepLength.EndX = LengthSVG * 2 / 3d + wallThickness;
            dimensionStepLength.EndY = HeightSVG / 2.5d;
            dimensionStepLength.ArrowThickness = arrowThickness;
            dimensionStepLength.ArrowLength = arrowLength;
            dimensionStepLength.Stroke = StrokeFillDimensions;
            dimensionStepLength.Fill = StrokeFillDimensions;
            dimensionStepLength.Name = PrimaryDimensionName;
            dimensionStepLength.AngleWithAxisX = 0;

            DimensionLineDraw dimensionStepHeight = new();
            dimensionStepHeight.StartX = LengthSVG * 0.96d;
            dimensionStepHeight.StartY = HeightSVG / 2d;
            dimensionStepHeight.EndX = LengthSVG * 0.96d;
            dimensionStepHeight.EndY = HeightSVG - wallThickness;
            dimensionStepHeight.ArrowThickness = arrowThickness;
            dimensionStepHeight.ArrowLength = arrowLength;
            dimensionStepHeight.Stroke = StrokeFillDimensions;
            dimensionStepHeight.Fill = StrokeFillDimensions;
            dimensionStepHeight.Name = SecondaryDimensionName;
            dimensionStepHeight.AngleWithAxisX = 90;

            LineDraw helpLine1 = new();
            helpLine1.StartX = dimensionStepLength.EndX;
            helpLine1.StartY = dimensionStepLength.EndY;
            helpLine1.EndX = dimensionStepLength.EndX;
            helpLine1.EndY = HeightSVG / 2d;
            helpLine1.Stroke = HelpLinesStrokeFill;
            helpLine1.Fill = HelpLinesStrokeFill;
            helpLine1.StrokeDashArray = HelpLinesDashArray;

            LineDraw helpLine2 = new();
            helpLine2.StartX = wallThickness + LengthSVG * 2 / 3d;
            helpLine2.StartY = HeightSVG / 2d;
            helpLine2.EndX = dimensionStepHeight.StartX;
            helpLine2.EndY = dimensionStepHeight.StartY;
            helpLine2.Stroke = HelpLinesStrokeFill;
            helpLine2.Fill = HelpLinesStrokeFill;
            helpLine2.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(floor);
            shapesToDraw.Add(leftWall);
            shapesToDraw.Add(step);
            dimensionsToDraw.Add(dimensionStepLength);
            dimensionsToDraw.Add(dimensionStepHeight);
            helpLinesToDraw.Add(helpLine1);
            helpLinesToDraw.Add(helpLine2);
        }

        #endregion

        #region 1.Set MeasureShape Methods (Lengths)

        /// <summary>
        /// Creates the Shape for a Wall to Wall Length Measure
        /// The Dimension Name is Picked According to the Primary Model
        /// </summary>
        private void SetWallToWallMeasureShape()
        {
            RectangleDraw leftWall = CreateLeftWall();
            RectangleDraw rightWall = CreateRightWall();
            RectangleDraw topWall = CreateTopWall();

            DimensionLineDraw dimension = new();
            dimension.StartX = wallThickness;
            dimension.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension.EndX = LengthSVG - wallThickness;
            dimension.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension.ArrowThickness = arrowThickness;
            dimension.ArrowLength = arrowLength;
            dimension.Stroke = StrokeFillDimensions;
            dimension.Fill = StrokeFillDimensions;
            dimension.Name = PrimaryDimensionName;
            dimension.AngleWithAxisX = 0;

            shapesToDraw.Add(leftWall);
            shapesToDraw.Add(rightWall);
            shapesToDraw.Add(topWall);
            dimensionsToDraw.Add(dimension);
        }

        /// <summary>
        /// Creates the Shape for a Corner Lengths Measure
        /// The Dimension Name is Picked According to the Primary & Secondary Model
        /// </summary>
        private void SetCornerMeasureShape()
        {
            //When Flipped
            //Create opposite wall
            //Change the side of the one dimension
            //Change the Starting Point of the Other Dimension
            RectangleDraw topWall = CreateTopWall();
            RectangleDraw sideWall = IsFlipped ? CreateRightWall() : CreateLeftWall();

            DimensionLineDraw dimensionHorizontal = new();
            dimensionHorizontal.StartX = IsFlipped ? LengthSVG * DimensionMarginPercent : wallThickness;
            dimensionHorizontal.StartY = HeightSVG * (1d - DimensionMarginPercent);
            dimensionHorizontal.EndX = IsFlipped ? LengthSVG - wallThickness : LengthSVG * (1d - DimensionMarginPercent);
            dimensionHorizontal.EndY = HeightSVG * (1d - DimensionMarginPercent);
            dimensionHorizontal.ArrowThickness = arrowThickness;
            dimensionHorizontal.ArrowLength = arrowLength;
            dimensionHorizontal.Stroke = StrokeFillDimensions;
            dimensionHorizontal.Fill = StrokeFillDimensions;
            dimensionHorizontal.Name = PrimaryDimensionName;
            dimensionHorizontal.AngleWithAxisX = 0;

            DimensionLineDraw dimensionVertical = new();
            dimensionVertical.StartX = IsFlipped ? LengthSVG * DimensionMarginPercent : LengthSVG * (1d - DimensionMarginPercent);
            dimensionVertical.StartY = wallThickness;
            dimensionVertical.EndX = IsFlipped ? LengthSVG * DimensionMarginPercent : LengthSVG * (1d - DimensionMarginPercent);
            dimensionVertical.EndY = HeightSVG * (1d - DimensionMarginPercent);
            dimensionVertical.ArrowThickness = arrowThickness;
            dimensionVertical.ArrowLength = arrowLength;
            dimensionVertical.Stroke = StrokeFillDimensions;
            dimensionVertical.Fill = StrokeFillDimensions;
            dimensionVertical.Name = SecondaryDimensionName;
            dimensionVertical.AngleWithAxisX = 90;

            shapesToDraw.Add(topWall);
            shapesToDraw.Add(sideWall);
            dimensionsToDraw.Add(dimensionHorizontal);
            dimensionsToDraw.Add(dimensionVertical);
        }

        /// <summary>
        /// Creates the Shape for a PI Lengths Measure
        /// The Dimension Name is Picked According to the Primary-Secondary-Tertiary Model
        /// </summary>
        private void SetPIMeasureShape()
        {
            //When Flipped the Shape will be the Same though the Dimensions should be differently Positioned to reflect the Pieces
            //Either the Names in the Dimensions can be changed or their X Positions
            RectangleDraw topWall = CreateTopWall();

            DimensionLineDraw dimensionHorizontal = new();
            dimensionHorizontal.StartX = LengthSVG * DimensionMarginPercent;
            dimensionHorizontal.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionHorizontal.EndX = LengthSVG * (1 - DimensionMarginPercent);
            dimensionHorizontal.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionHorizontal.ArrowThickness = arrowThickness;
            dimensionHorizontal.ArrowLength = arrowLength;
            dimensionHorizontal.Stroke = StrokeFillDimensions;
            dimensionHorizontal.Fill = StrokeFillDimensions;
            dimensionHorizontal.Name = PrimaryDimensionName;
            dimensionHorizontal.AngleWithAxisX = 0;

            DimensionLineDraw dimensionVertical1 = new();
            dimensionVertical1.StartX = LengthSVG * (1 - DimensionMarginPercent);
            dimensionVertical1.StartY = wallThickness;
            dimensionVertical1.EndX = LengthSVG * (1 - DimensionMarginPercent);
            dimensionVertical1.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionVertical1.ArrowThickness = arrowThickness;
            dimensionVertical1.ArrowLength = arrowLength;
            dimensionVertical1.Stroke = StrokeFillDimensions;
            dimensionVertical1.Fill = StrokeFillDimensions;
            dimensionVertical1.Name = IsFlipped ? TertiaryDimensionName : SecondaryDimensionName; //Interchange Names instead of X Position in Flip
            dimensionVertical1.AngleWithAxisX = 90;

            DimensionLineDraw dimensionVertical2 = new();
            dimensionVertical2.StartX = LengthSVG * DimensionMarginPercent;
            dimensionVertical2.StartY = wallThickness;
            dimensionVertical2.EndX = LengthSVG * DimensionMarginPercent;
            dimensionVertical2.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionVertical2.ArrowThickness = arrowThickness;
            dimensionVertical2.ArrowLength = arrowLength;
            dimensionVertical2.Stroke = StrokeFillDimensions;
            dimensionVertical2.Fill = StrokeFillDimensions;
            dimensionVertical2.Name = IsFlipped ? SecondaryDimensionName : TertiaryDimensionName; //Interchange Names instead of X Position in Flip
            dimensionVertical2.AngleWithAxisX = 90;

            shapesToDraw.Add(topWall);
            dimensionsToDraw.Add(dimensionHorizontal);
            dimensionsToDraw.Add(dimensionVertical1);
            dimensionsToDraw.Add(dimensionVertical2);
        }

        /// <summary>
        /// Creates the Shape for a Wall to Wall Length Measure
        /// The Dimension Name is Picked According to the Primary Model
        /// </summary>
        private void SetWallToWallTwoMeasureShape()
        {
            //When Flipped the Shape will be the Same though the Dimensions should be differently Positioned to reflect the Pieces
            //Either the Names in the Dimensions can be changed or their X Positions
            RectangleDraw leftWall = CreateLeftWall();
            RectangleDraw rightWall = CreateRightWall();
            RectangleDraw topWall = CreateTopWall();

            DimensionLineDraw dimension1 = new();
            dimension1.StartX = wallThickness;
            dimension1.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension1.EndX = LengthSVG / 2d;
            dimension1.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension1.ArrowThickness = arrowThickness;
            dimension1.ArrowLength = arrowLength;
            dimension1.Stroke = StrokeFillDimensions;
            dimension1.Fill = StrokeFillDimensions;
            dimension1.Name = IsFlipped ? SecondaryDimensionName : PrimaryDimensionName; //Interchange Names instead of X Position in Flip
            dimension1.AngleWithAxisX = 0;

            DimensionLineDraw dimension2 = new();
            dimension2.StartX = LengthSVG / 2d;
            dimension2.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension2.EndX = LengthSVG - wallThickness;
            dimension2.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension2.ArrowThickness = arrowThickness;
            dimension2.ArrowLength = arrowLength;
            dimension2.Stroke = StrokeFillDimensions;
            dimension2.Fill = StrokeFillDimensions;
            dimension2.Name = IsFlipped ? PrimaryDimensionName : SecondaryDimensionName; //Interchange Names instead of X Position in Flip
            dimension2.AngleWithAxisX = 0;

            shapesToDraw.Add(leftWall);
            shapesToDraw.Add(rightWall);
            shapesToDraw.Add(topWall);
            dimensionsToDraw.Add(dimension1);
            dimensionsToDraw.Add(dimension2);
        }

        /// <summary>
        /// Creates the Shape for the 8W Measure Dimension
        /// </summary>
        private void SetWMeasureShape()
        {
            //When Flipped Create the Opposite Side Wall
            //Change the X Positions for the Horizontal Dimension
            //Change the X Position of the Helper Line

            RectangleDraw topWall = CreateTopWall();
            RectangleDraw sideWall = IsFlipped ? CreateLeftWall() : CreateRightWall();

            DimensionLineDraw dimension = new();
            dimension.StartX = IsFlipped ? wallThickness : LengthSVG / 3d;
            dimension.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG - wallThickness;
            dimension.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension.ArrowThickness = arrowThickness;
            dimension.ArrowLength = arrowLength;
            dimension.Stroke = StrokeFillDimensions;
            dimension.Fill = StrokeFillDimensions;
            dimension.Name = PrimaryDimensionName;
            dimension.AngleWithAxisX = 0;

            LineDraw helpLine = new();
            helpLine.StartX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            helpLine.StartY = wallThickness;
            helpLine.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            helpLine.EndY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine.Stroke = HelpLinesStrokeFill;
            helpLine.Fill = HelpLinesStrokeFill;
            helpLine.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(topWall);
            shapesToDraw.Add(sideWall);
            dimensionsToDraw.Add(dimension);
            helpLinesToDraw.Add(helpLine);
        }

        /// <summary>
        /// Creates the Shape for the 8E Measure Dimension
        /// </summary>
        private void SetEMeasureShape()
        {
            //When Flipped Remains the Same
            RectangleDraw topWall = CreateTopWall();

            DimensionLineDraw dimension = new();
            dimension.StartX = LengthSVG / 6d;
            dimension.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension.EndX = LengthSVG * 5 / 6d;
            dimension.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension.ArrowThickness = arrowThickness;
            dimension.ArrowLength = arrowLength;
            dimension.Stroke = StrokeFillDimensions;
            dimension.Fill = StrokeFillDimensions;
            dimension.Name = PrimaryDimensionName;
            dimension.AngleWithAxisX = 0;

            LineDraw helpLine1 = new();
            helpLine1.StartX = LengthSVG / 6d;
            helpLine1.StartY = wallThickness;
            helpLine1.EndX = LengthSVG / 6d;
            helpLine1.EndY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine1.Stroke = HelpLinesStrokeFill;
            helpLine1.Fill = HelpLinesStrokeFill;
            helpLine1.StrokeDashArray = HelpLinesDashArray;

            LineDraw helpLine2 = new();
            helpLine2.StartX = LengthSVG * 5 / 6d;
            helpLine2.StartY = wallThickness;
            helpLine2.EndX = LengthSVG * 5 / 6d;
            helpLine2.EndY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine2.Stroke = HelpLinesStrokeFill;
            helpLine2.Fill = HelpLinesStrokeFill;
            helpLine2.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(topWall);
            dimensionsToDraw.Add(dimension);
            helpLinesToDraw.Add(helpLine1);
            helpLinesToDraw.Add(helpLine2);
        }

        /// <summary>
        /// Creates the Shapes for the 81Draw Measure Dimension
        /// </summary>
        private void SetDraw81MeasureShape()
        {
            //When Flipped Create the Opposite SideWall
            //Change the Horizontal Line X Position
            //Change the Verical Line X Position
            //Change the Helpline X Position
            RectangleDraw topWall = CreateTopWall();
            RectangleDraw sideWall = IsFlipped ? CreateLeftWall() : CreateRightWall();

            DimensionLineDraw dimension8W = new();
            dimension8W.StartX = IsFlipped ? wallThickness : LengthSVG / 3d;
            dimension8W.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG - wallThickness;
            dimension8W.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W.ArrowThickness = arrowThickness;
            dimension8W.ArrowLength = arrowLength;
            dimension8W.Stroke = StrokeFillDimensions;
            dimension8W.Fill = StrokeFillDimensions;
            dimension8W.Name = PrimaryDimensionName;
            dimension8W.AngleWithAxisX = 0;

            DimensionLineDraw dimensionFlipper = new();
            dimensionFlipper.StartX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            dimensionFlipper.StartY = HeightSVG / 2d;
            dimensionFlipper.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            dimensionFlipper.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionFlipper.ArrowThickness = arrowThickness;
            dimensionFlipper.ArrowLength = arrowLength;
            dimensionFlipper.Stroke = StrokeFillDimensions;
            dimensionFlipper.Fill = StrokeFillDimensions;
            dimensionFlipper.Name = SecondaryDimensionName;
            dimensionFlipper.AngleWithAxisX = 90;

            LineDraw helpLine = new();
            helpLine.StartX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            helpLine.StartY = wallThickness;
            helpLine.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            helpLine.EndY = HeightSVG / 2d;
            helpLine.Stroke = HelpLinesStrokeFill;
            helpLine.Fill = HelpLinesStrokeFill;
            helpLine.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(topWall);
            shapesToDraw.Add(sideWall);
            dimensionsToDraw.Add(dimension8W);
            dimensionsToDraw.Add(dimensionFlipper);
            helpLinesToDraw.Add(helpLine);
        }

        /// <summary>
        /// Creates the Shapes for the 82Draw Measure Dimension
        /// </summary>
        private void SetDraw82MeasureShape()
        {
            //When Flipped create the Oposite side Wall
            //Change X Position of All Elements
            RectangleDraw topWall = CreateTopWall();
            RectangleDraw sideWall = IsFlipped ? CreateLeftWall() : CreateRightWall();

            DimensionLineDraw dimension8W1 = new(); //Horizontal
            dimension8W1.StartX = IsFlipped ? wallThickness : LengthSVG / 3d;
            dimension8W1.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W1.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG - wallThickness;
            dimension8W1.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W1.ArrowThickness = arrowThickness;
            dimension8W1.ArrowLength = arrowLength;
            dimension8W1.Stroke = StrokeFillDimensions;
            dimension8W1.Fill = StrokeFillDimensions;
            dimension8W1.Name = PrimaryDimensionName;
            dimension8W1.AngleWithAxisX = 0;

            DimensionLineDraw dimension8W2 = new();
            dimension8W2.StartX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG * DimensionMarginPercent;
            dimension8W2.StartY = wallThickness;
            dimension8W2.EndX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG * DimensionMarginPercent;
            dimension8W2.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W2.ArrowThickness = arrowThickness;
            dimension8W2.ArrowLength = arrowLength;
            dimension8W2.Stroke = StrokeFillDimensions;
            dimension8W2.Fill = StrokeFillDimensions;
            dimension8W2.Name = SecondaryDimensionName;
            dimension8W2.AngleWithAxisX = 90;

            LineDraw helpLine8W1 = new(); //Vertical
            helpLine8W1.StartX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            helpLine8W1.StartY = wallThickness;
            helpLine8W1.EndX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG / 3d;
            helpLine8W1.EndY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine8W1.Stroke = HelpLinesStrokeFill;
            helpLine8W1.Fill = HelpLinesStrokeFill;
            helpLine8W1.StrokeDashArray = HelpLinesDashArray;

            LineDraw helpLine8W2 = new();
            helpLine8W2.StartX = IsFlipped ? LengthSVG * 2 / 3d : LengthSVG * DimensionMarginPercent;
            helpLine8W2.StartY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine8W2.EndX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG / 3d;
            helpLine8W2.EndY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine8W2.Stroke = HelpLinesStrokeFill;
            helpLine8W2.Fill = HelpLinesStrokeFill;
            helpLine8W2.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(topWall);
            shapesToDraw.Add(sideWall);
            dimensionsToDraw.Add(dimension8W1);
            dimensionsToDraw.Add(dimension8W2);
            helpLinesToDraw.Add(helpLine8W1);
            helpLinesToDraw.Add(helpLine8W2);
        }

        /// <summary>
        /// Creates the Shapes for the 84Draw Measure Dimension
        /// </summary>
        private void SetDraw84MeasureShape()
        {
            //ON Flip create Opposite Wall
            //Change X Dimension of both Dimensions
            //The Helpline remains the Same

            RectangleDraw sideWall = IsFlipped ? CreateLeftWall() : CreateRightWall();
            RectangleDraw topWall = CreateTopWall();

            DimensionLineDraw dimension8W = new(); //VERTICAL
            dimension8W.StartX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG * DimensionMarginPercent;
            dimension8W.StartY = wallThickness;
            dimension8W.EndX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG * DimensionMarginPercent;
            dimension8W.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W.ArrowThickness = arrowThickness;
            dimension8W.ArrowLength = arrowLength;
            dimension8W.Stroke = StrokeFillDimensions;
            dimension8W.Fill = StrokeFillDimensions;
            dimension8W.Name = PrimaryDimensionName;
            dimension8W.AngleWithAxisX = 90;

            DimensionLineDraw dimensionAngle8W = new(); //HORIZONTAL
            dimensionAngle8W.StartX = IsFlipped ? LengthSVG / 2d : LengthSVG * DimensionMarginPercent;
            dimensionAngle8W.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionAngle8W.EndX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG / 2d;
            dimensionAngle8W.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionAngle8W.ArrowThickness = arrowThickness;
            dimensionAngle8W.ArrowLength = arrowLength;
            dimensionAngle8W.Stroke = StrokeFillDimensions;
            dimensionAngle8W.Fill = StrokeFillDimensions;
            dimensionAngle8W.Name = SecondaryDimensionName;
            dimensionAngle8W.AngleWithAxisX = 0;

            LineDraw helpLine1 = new();
            helpLine1.StartX = LengthSVG / 2d;
            helpLine1.StartY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine1.EndX = LengthSVG / 2d;
            helpLine1.EndY = wallThickness;
            helpLine1.Stroke = HelpLinesStrokeFill;
            helpLine1.Fill = HelpLinesStrokeFill;
            helpLine1.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(sideWall);
            shapesToDraw.Add(topWall);
            dimensionsToDraw.Add(dimension8W);
            dimensionsToDraw.Add(dimensionAngle8W);
            helpLinesToDraw.Add(helpLine1);
        }

        /// <summary>
        /// Creates the Shapes for the 88Draw Measure Dimension
        /// </summary>
        private void SetDraw88MeasureShape()
        {
            RectangleDraw sideWall = IsFlipped ? CreateLeftWall() : CreateRightWall();
            RectangleDraw topWall = CreateTopWall();

            DimensionLineDraw dimension8W = new(); //Vertical
            dimension8W.StartX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG * DimensionMarginPercent;
            dimension8W.StartY = wallThickness;
            dimension8W.EndX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG * DimensionMarginPercent;
            dimension8W.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W.ArrowThickness = arrowThickness;
            dimension8W.ArrowLength = arrowLength;
            dimension8W.Stroke = StrokeFillDimensions;
            dimension8W.Fill = StrokeFillDimensions;
            dimension8W.Name = PrimaryDimensionName;
            dimension8W.AngleWithAxisX = 90;

            DimensionLineDraw dimensionAngle8W = new(); //Horizontal 1
            dimensionAngle8W.StartX = IsFlipped ? LengthSVG / 2d : LengthSVG * DimensionMarginPercent;
            dimensionAngle8W.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionAngle8W.EndX = IsFlipped ? LengthSVG * (1d - DimensionMarginPercent) : LengthSVG / 2d;
            dimensionAngle8W.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimensionAngle8W.ArrowThickness = arrowThickness;
            dimensionAngle8W.ArrowLength = arrowLength;
            dimensionAngle8W.Stroke = StrokeFillDimensions;
            dimensionAngle8W.Fill = StrokeFillDimensions;
            dimensionAngle8W.Name = SecondaryDimensionName;
            dimensionAngle8W.AngleWithAxisX = 0;

            DimensionLineDraw dimension8W3 = new(); //Horizontal 2
            dimension8W3.StartX = IsFlipped ? wallThickness : LengthSVG * 1.8 / 3d;
            dimension8W3.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W3.EndX = IsFlipped ? LengthSVG * 1.2 / 3d : LengthSVG - wallThickness;
            dimension8W3.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W3.ArrowThickness = arrowThickness;
            dimension8W3.ArrowLength = arrowLength;
            dimension8W3.Stroke = StrokeFillDimensions;
            dimension8W3.Fill = StrokeFillDimensions;
            dimension8W3.Name = TertiaryDimensionName;
            dimension8W3.AngleWithAxisX = 0;

            LineDraw helpLine1 = new();
            helpLine1.StartX = LengthSVG / 2d;
            helpLine1.StartY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine1.EndX = LengthSVG / 2d;
            helpLine1.EndY = wallThickness;
            helpLine1.Stroke = HelpLinesStrokeFill;
            helpLine1.Fill = HelpLinesStrokeFill;
            helpLine1.StrokeDashArray = HelpLinesDashArray;

            LineDraw helpLine2 = new();
            helpLine2.StartX = IsFlipped ? LengthSVG * 1.2 / 3d : LengthSVG * 1.8 / 3d;
            helpLine2.StartY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine2.EndX = IsFlipped ? LengthSVG * 1.2 / 3d : LengthSVG * 1.8 / 3d;
            helpLine2.EndY = wallThickness;
            helpLine2.Stroke = HelpLinesStrokeFill;
            helpLine2.Fill = HelpLinesStrokeFill;
            helpLine2.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(sideWall);
            shapesToDraw.Add(topWall);
            dimensionsToDraw.Add(dimension8W);
            dimensionsToDraw.Add(dimensionAngle8W);
            dimensionsToDraw.Add(dimension8W3);
            helpLinesToDraw.Add(helpLine1);
            helpLinesToDraw.Add(helpLine2);
        }

        /// <summary>
        /// Creates the Shapes for the 85Draw Measure Dimension
        /// </summary>
        private void SetDraw85MeasureShape()
        {
            //Flip Change only Dimension Names
            RectangleDraw rightWall = CreateRightWall();
            RectangleDraw topWall = CreateTopWall();
            RectangleDraw leftWall = CreateLeftWall();

            DimensionLineDraw dimension8W1 = new();
            dimension8W1.StartX = wallThickness;
            dimension8W1.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W1.EndX = LengthSVG * 1.7d / 4d;
            dimension8W1.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W1.ArrowThickness = arrowThickness;
            dimension8W1.ArrowLength = arrowLength;
            dimension8W1.Stroke = StrokeFillDimensions;
            dimension8W1.Fill = StrokeFillDimensions;
            dimension8W1.Name = IsFlipped ? SecondaryDimensionName : PrimaryDimensionName;
            dimension8W1.AngleWithAxisX = 0;

            DimensionLineDraw dimension8W2 = new();
            dimension8W2.StartX = LengthSVG * 2.3d / 4d;
            dimension8W2.StartY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W2.EndX = LengthSVG - wallThickness;
            dimension8W2.EndY = HeightSVG * (1 - DimensionMarginPercent);
            dimension8W2.ArrowThickness = arrowThickness;
            dimension8W2.ArrowLength = arrowLength;
            dimension8W2.Stroke = StrokeFillDimensions;
            dimension8W2.Fill = StrokeFillDimensions;
            dimension8W2.Name = IsFlipped ? PrimaryDimensionName : SecondaryDimensionName;
            dimension8W2.AngleWithAxisX = 0;

            LineDraw helpLine1 = new();
            helpLine1.StartX = LengthSVG * 1.7d / 4d;
            helpLine1.StartY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine1.EndX = LengthSVG * 1.7d / 4d;
            helpLine1.EndY = wallThickness;
            helpLine1.Stroke = HelpLinesStrokeFill;
            helpLine1.Fill = HelpLinesStrokeFill;
            helpLine1.StrokeDashArray = HelpLinesDashArray;

            LineDraw helpLine2 = new();
            helpLine2.StartX = LengthSVG * 2.3d / 4d;
            helpLine2.StartY = HeightSVG * (1 - DimensionMarginPercent);
            helpLine2.EndX = LengthSVG * 2.3d / 4d;
            helpLine2.EndY = wallThickness;
            helpLine2.Stroke = HelpLinesStrokeFill;
            helpLine2.Fill = HelpLinesStrokeFill;
            helpLine2.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(rightWall);
            shapesToDraw.Add(topWall);
            shapesToDraw.Add(leftWall);
            dimensionsToDraw.Add(dimension8W1);
            dimensionsToDraw.Add(dimension8W2);
            helpLinesToDraw.Add(helpLine1);
            helpLinesToDraw.Add(helpLine2);
        }

        /// <summary>
        /// Creates the Shapes for the 8W40Draw Measure Dimension
        /// </summary>
        private void SetDrawBathtubMeasureShape()
        {
            //When Flipped Create the Opposite Side Wall
            //Change the X Positions for the Horizontal Dimension
            //Change the X Position of the Helper Line

            RectangleDraw topWall = CreateTopWall();
            RectangleDraw sideWall = IsFlipped ? CreateRightWall() : CreateLeftWall();

            RectangleDraw bathtubFront = new(LengthSVG - wallThickness - LengthSVG / 10d, wallThickness / 3d);
            bathtubFront.SetCenterOrStartX(IsFlipped ? LengthSVG - bathtubFront.Length - wallThickness : wallThickness, DrawShape.CSCoordinate.Start);
            bathtubFront.SetCenterOrStartY(HeightSVG / 2d, DrawShape.CSCoordinate.Start);
            bathtubFront.Stroke = WallStroke;
            bathtubFront.Fill = "white";
            bathtubFront.Name = "bathtubLength";

            RectangleDraw bathtubSide = new(wallThickness / 3d, HeightSVG / 2d - wallThickness);
            bathtubSide.SetCenterOrStartX(IsFlipped ? bathtubFront.StartX : bathtubFront.EndX - wallThickness / 3d, DrawShape.CSCoordinate.Start);
            bathtubSide.SetCenterOrStartY(wallThickness, DrawShape.CSCoordinate.Start);
            bathtubSide.Stroke = WallStroke;
            bathtubSide.Fill = "white";
            bathtubSide.Name = "bathtubLength";

            DimensionLineDraw dimension = new();
            dimension.StartX = IsFlipped ? bathtubFront.EndX - (bathtubFront.EndX - bathtubFront.StartX) / 1.5d : bathtubFront.StartX;
            dimension.StartY = bathtubFront.EndY + HeightSVG / 6d;
            dimension.EndX = IsFlipped ? bathtubFront.EndX : (bathtubFront.EndX + bathtubFront.StartX) / 1.5d;
            dimension.EndY = dimension.StartY;
            dimension.ArrowThickness = arrowThickness;
            dimension.ArrowLength = arrowLength;
            dimension.Stroke = StrokeFillDimensions;
            dimension.Fill = StrokeFillDimensions;
            dimension.Name = PrimaryDimensionName;
            dimension.AngleWithAxisX = 0;

            LineDraw helpLine = new();
            helpLine.StartX = IsFlipped ? dimension.StartX : dimension.EndX;
            helpLine.StartY = dimension.StartY;
            helpLine.EndX = helpLine.StartX;
            helpLine.EndY = bathtubFront.EndY;
            helpLine.Stroke = HelpLinesStrokeFill;
            helpLine.Fill = HelpLinesStrokeFill;
            helpLine.StrokeDashArray = HelpLinesDashArray;

            shapesToDraw.Add(topWall);
            shapesToDraw.Add(sideWall);
            shapesToDraw.Add(bathtubFront);
            shapesToDraw.Add(bathtubSide);
            dimensionsToDraw.Add(dimension);
            helpLinesToDraw.Add(helpLine);
        }

        #endregion

        #region 2.Wall and Fixed Shapes Creation Methods

        private RectangleDraw CreateTopWall()
        {
            RectangleDraw topWall = new();
            topWall.SetCenterOrStartX(0, DrawShape.CSCoordinate.Start);
            topWall.SetCenterOrStartY(0, DrawShape.CSCoordinate.Start);
            topWall.Length = LengthSVG;
            topWall.Height = wallThickness;
            topWall.Stroke = WallStroke;
            topWall.Fill = WallFill;
            topWall.Name = "TopWall";

            return topWall;
        }
        private RectangleDraw CreateRightWall()
        {
            RectangleDraw rightWall = new();
            rightWall.SetCenterOrStartX(LengthSVG - wallThickness, DrawShape.CSCoordinate.Start);
            rightWall.SetCenterOrStartY(wallThickness, DrawShape.CSCoordinate.Start);
            rightWall.Length = wallThickness;
            rightWall.Height = HeightSVG - wallThickness;
            rightWall.Stroke = WallStroke;
            rightWall.Fill = WallFill;
            rightWall.Name = "RightWall";

            return rightWall;
        }
        private RectangleDraw CreateLeftWall()
        {
            RectangleDraw leftWall = new();
            leftWall.SetCenterOrStartX(0, DrawShape.CSCoordinate.Start);
            leftWall.SetCenterOrStartY(0, DrawShape.CSCoordinate.Start);
            leftWall.Length = wallThickness;
            leftWall.Height = HeightSVG;
            leftWall.Stroke = WallStroke;
            leftWall.Fill = WallFill;
            leftWall.Name = "LeftWall";

            return leftWall;
        }
        private RectangleDraw CreateFloor()
        {
            RectangleDraw floor = new();
            floor.SetCenterOrStartX(0, DrawShape.CSCoordinate.Start);
            floor.SetCenterOrStartY(HeightSVG - wallThickness, DrawShape.CSCoordinate.Start);
            floor.Length = LengthSVG;
            floor.Height = wallThickness;
            floor.Stroke = WallStroke;
            floor.Fill = WallFill;
            floor.Name = "Floor";

            return floor;
        }

        #endregion
    }
}

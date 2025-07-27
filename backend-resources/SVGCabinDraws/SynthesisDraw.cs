using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Validators;
using SVGCabinDraws.ConcreteDraws.DBDraws;
using SVGCabinDraws.ConcreteDraws.FreeDraws;
using SVGCabinDraws.ConcreteDraws.HBDraws;
using SVGCabinDraws.ConcreteDraws.NBDraws;
using SVGCabinDraws.ConcreteDraws.NPDraws;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGCabinDraws
{
    public class SynthesisDraw
    {

        private CabinSynthesis synthesis;

        /// <summary>
        /// The Draw of the Primary Cabin
        /// </summary>
        public CabinDraw? PrimaryDraw { get; set; }
        /// <summary>
        /// The Draw of the Secondary Cabin
        /// </summary>
        public CabinDraw? SecondaryDraw { get; set; }
        /// <summary>
        /// The Draw of the Tertiary Cabin
        /// </summary>
        public CabinDraw? TertiaryDraw { get; set; }

        /// <summary>
        /// The List of Shapes Forming the Front Draw
        /// </summary>
        public List<DrawShape> FrontSideShapes { get; set; } = new();
        /// <summary>
        /// The List of Shapes Forming the Right Side Draw
        /// </summary>
        public List<DrawShape> RightSideShapes { get; set; } = new();
        /// <summary>
        /// The List of Shapes Forming the Left Side Draw
        /// </summary>
        public List<DrawShape> LeftSideShapes { get; set; } = new();

        /// <summary>
        /// The List of the Shapes Representing the Dimension Arrows for the Front Side
        /// </summary>
        public List<DrawShape> FrontSideDimensions { get; set; } = new();
        /// <summary>
        /// The List of the Shapes Representing the Dimension Arrows for the Right Side
        /// </summary>
        public List<DrawShape> RightSideDimensions { get; set; } = new();
        /// <summary>
        /// The List of the Shapes Representing the Dimension Arrows for the Left Side
        /// </summary>
        public List<DrawShape> LeftSideDimensions { get; set; } = new();

        public bool IsFrontDrawAvailable { get; set; }
        public bool IsRightDrawAvailable { get; set; }
        public bool IsLeftDrawAvailable { get; set; }

        /// <summary>
        /// The Canvas Size of the Front Draw
        /// </summary>
        public (double,double) FrontSideCanvas { get; set; }
        /// <summary>
        /// The Canvas Size of the Right Draw
        /// </summary>
        public (double, double) RightSideCanvas { get; set; }
        /// <summary>
        /// The Canvas Size of the Left Draw
        /// </summary>
        public (double, double) LeftSideCanvas { get; set; }

        /// <summary>
        /// The Number of Sides Available for Drawing
        /// </summary>
        public int NumberOfDrawableSides { get; set; }

        public double PrimaryDoorOpening { get => PrimaryDraw?.SingleDoorOpening ?? 0; }
        public double SecondaryDoorOpening { get => SecondaryDraw?.SingleDoorOpening ?? 0; }

        public SynthesisDraw(CabinSynthesis synthesisToDraw , GlassesBuilderDirector glassBuilder , CabinValidator validator)
        {
            this.synthesis = synthesisToDraw;
            if (synthesis.Primary is not null && validator.Validate(synthesis.Primary).IsValid)
            {
                if (synthesis.Primary.Glasses.Count == 0) glassBuilder.BuildAllGlasses(synthesis.Primary);
                PrimaryDraw = DrawsFactory.BuildCabinDraw(synthesis.Primary);
            }
            if (synthesis.Secondary is not null && validator.Validate(synthesis.Secondary).IsValid)
            {
                if (synthesis.Secondary.Glasses.Count == 0) glassBuilder.BuildAllGlasses(synthesis.Secondary);
                SecondaryDraw = DrawsFactory.BuildCabinDraw(synthesis.Secondary);
            }
            if (synthesis.Tertiary is not null && validator.Validate(synthesis.Tertiary).IsValid)
            {
                if (synthesis.Tertiary.Glasses.Count == 0) glassBuilder.BuildAllGlasses(synthesis.Tertiary);
                TertiaryDraw = DrawsFactory.BuildCabinDraw(synthesis.Tertiary);
            }
            
            DefineDrawableSides();
            DefineCanvasSizePerSide();
            BuildDrawSideShapes();
        }

        /// <summary>
        /// Defines how many Sides of the Synthesis can be Drawn
        /// </summary>
        private void DefineDrawableSides()
        {
            switch (synthesis.DrawNo)
            {
                case CabinDrawNumber.Draw9S:
                case CabinDrawNumber.Draw94:
                case CabinDrawNumber.Draw9B:
                case CabinDrawNumber.DrawVS:
                case CabinDrawNumber.DrawV4:
                case CabinDrawNumber.DrawWS:
                case CabinDrawNumber.DrawNP44:
                case CabinDrawNumber.DrawQP44:
                case CabinDrawNumber.Draw2StraightNP48:
                case CabinDrawNumber.Draw2StraightQP48:
                case CabinDrawNumber.DrawStraightNP6W47:
                case CabinDrawNumber.DrawStraightQP6W47:
                case CabinDrawNumber.DrawNB31:
                case CabinDrawNumber.DrawQB31:
                case CabinDrawNumber.DrawStraightNB6W38:
                case CabinDrawNumber.DrawStraightQB6W38:
                case CabinDrawNumber.Draw2StraightNB41:
                case CabinDrawNumber.Draw2StraightQB41:
                case CabinDrawNumber.DrawDB51:
                case CabinDrawNumber.DrawStraightDB8W59:
                case CabinDrawNumber.Draw2StraightDB61:
                case CabinDrawNumber.DrawHB34:
                case CabinDrawNumber.DrawStraightHB8W40:
                case CabinDrawNumber.Draw2StraightHB43:
                case CabinDrawNumber.Draw8W:
                case CabinDrawNumber.DrawE:
                case CabinDrawNumber.Draw8WFlipper81:
                case CabinDrawNumber.Draw2Straight8W85:
                case CabinDrawNumber.Draw8W40:
                case CabinDrawNumber.DrawNV:
                case CabinDrawNumber.DrawNV2:
                case CabinDrawNumber.DrawMV2:
                case CabinDrawNumber.Draw9F:
                case CabinDrawNumber.DrawVF:
                    NumberOfDrawableSides = 1;
                    break;
                case CabinDrawNumber.Draw9S9F:
                case CabinDrawNumber.Draw949F:
                case CabinDrawNumber.Draw9A:
                case CabinDrawNumber.Draw9C:
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
                case CabinDrawNumber.Draw2Corner8W82:
                case CabinDrawNumber.Draw1Corner8W84:
                case CabinDrawNumber.Draw2CornerStraight8W88:
                    NumberOfDrawableSides = 2;
                    break;
                case CabinDrawNumber.Draw9S9F9F:
                case CabinDrawNumber.Draw949F9F:
                case CabinDrawNumber.Draw9A9F:
                case CabinDrawNumber.Draw9C9F:
                case CabinDrawNumber.Draw9B9F9F:
                    NumberOfDrawableSides = 3;
                    break;
                case CabinDrawNumber.None:
                default:
                    NumberOfDrawableSides = 0;
                    break;
            }
        }

        /// <summary>
        /// Defines the Canvas Size of each DrawSide
        /// </summary>
        private void DefineCanvasSizePerSide()
        {
            switch (synthesis.DrawNo)
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
                case CabinDrawNumber.Draw8W:
                case CabinDrawNumber.DrawE:
                case CabinDrawNumber.Draw8W40:
                case CabinDrawNumber.DrawNV:
                case CabinDrawNumber.DrawNV2:
                case CabinDrawNumber.DrawMV2:
                    FrontSideCanvas = (synthesis.Primary.LengthMin, synthesis.Primary.Height);
                    RightSideCanvas = (0, 0); //Not Available
                    LeftSideCanvas = (0, 0); //Not Available
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
                case CabinDrawNumber.Draw8WFlipper81:
                    FrontSideCanvas = (synthesis.Primary.LengthMin + synthesis.Secondary.LengthMin, synthesis.Primary.Height);
                    RightSideCanvas = (0, 0); //Not Available
                    LeftSideCanvas = (0, 0); //Not Available
                    break;
                case CabinDrawNumber.Draw2Straight8W85:
                    //Correct Position of Second Glass to have a Default Free Opening between the Glasses
                    FrontSideCanvas = (synthesis.Primary.LengthMin + synthesis.Secondary.LengthMin + CabinW.DefaultMinFreeOpening, synthesis.Primary.Height);
                    RightSideCanvas = (0, 0); //Not Available
                    LeftSideCanvas = (0, 0); //Not Available
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
                    FrontSideCanvas = (synthesis.Primary.LengthMin, synthesis.Primary.Height);
                    RightSideCanvas = (synthesis.Secondary.LengthMin, synthesis.Secondary.Height);
                    LeftSideCanvas = (0, 0); //Not Available
                    break;
                case CabinDrawNumber.Draw9S9F9F:
                case CabinDrawNumber.Draw949F9F:
                case CabinDrawNumber.Draw9A9F:
                case CabinDrawNumber.Draw9C9F:
                case CabinDrawNumber.Draw9B9F9F:
                    FrontSideCanvas = (synthesis.Primary.LengthMin, synthesis.Primary.Height);
                    RightSideCanvas = (synthesis.Secondary.LengthMin, synthesis.Secondary.Height);
                    LeftSideCanvas = (synthesis.Tertiary.LengthMin, synthesis.Tertiary.Height);
                    break;
                case CabinDrawNumber.Draw2Corner8W82:
                    FrontSideCanvas = (synthesis.Primary.LengthMin, synthesis.Primary.Height);
                    RightSideCanvas = (0, 0);
                    LeftSideCanvas = (synthesis.Secondary.LengthMin, synthesis.Secondary.Height);
                    break;
                case CabinDrawNumber.Draw1Corner8W84:
                    FrontSideCanvas = (synthesis.Secondary.LengthMin, synthesis.Secondary.Height);
                    RightSideCanvas = (0, 0);
                    LeftSideCanvas = (synthesis.Primary.LengthMin, synthesis.Primary.Height);
                    break;
                case CabinDrawNumber.Draw2CornerStraight8W88:
                    //Have to add an Opening between the Two Glasses
                    FrontSideCanvas = (synthesis.Secondary.LengthMin + synthesis.Tertiary.LengthMin + CabinW.DefaultMinFreeOpening, synthesis.Secondary.Height);
                    RightSideCanvas = (0, 0);
                    LeftSideCanvas = (synthesis.Primary.LengthMin, synthesis.Primary.Height);
                    break;
                case CabinDrawNumber.None:
                case CabinDrawNumber.Draw9C:
                default:
                    FrontSideCanvas = (0, 0);
                    RightSideCanvas = (0, 0);
                    LeftSideCanvas = (0, 0); //Not Available
                    break;

            }
        }

        /// <summary>
        /// Builds the Drawn Sides Shapes for the Current Synthesis
        /// </summary>
        private void BuildDrawSideShapes()
        {
            switch (synthesis.DrawNo)
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
                case CabinDrawNumber.Draw8W:
                case CabinDrawNumber.DrawE:
                case CabinDrawNumber.Draw8W40:
                case CabinDrawNumber.DrawNV:
                case CabinDrawNumber.DrawNV2:
                case CabinDrawNumber.DrawMV2:
                    if (PrimaryDraw != null)
                    {
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    //RightSideShapes Not Available
                    //LeftSideShapes Not Available
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
                    if (PrimaryDraw != null && SecondaryDraw != null)
                    {
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        SecondaryDraw.FlipHorizontally(synthesis.Secondary?.LengthMin / 2d ?? 0); //Flip it by its Center
                        SecondaryDraw.TranslateX(synthesis.Primary?.LengthMin ?? 0);  //Translate it by the Length of the Primary
                        FrontSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw2Straight8W85:
                    if (PrimaryDraw != null && SecondaryDraw != null)
                    {
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        SecondaryDraw.FlipHorizontally(synthesis.Secondary?.LengthMin / 2d ?? 0); //Flip it by its Center
                        SecondaryDraw.TranslateX((synthesis.Primary?.LengthMin ?? 0) + CabinW.DefaultMinFreeOpening);  //Translate it by the Length of the Primary
                        FrontSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw8WFlipper81:
                    if (PrimaryDraw != null && SecondaryDraw != null)
                    {
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        SecondaryDraw.TranslateX(synthesis.Primary?.LengthMin ?? 0); //Move the Flipper to the end Position of Fixed Panel
                        FrontSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw9S9F:
                case CabinDrawNumber.Draw949F:
                case CabinDrawNumber.Draw9A:
                case CabinDrawNumber.Draw9C:
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
                    if (PrimaryDraw != null)
                    {
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    if (SecondaryDraw != null)
                    {
                        SecondaryDraw.FlipHorizontally(RightSideCanvas.Item1 / 2d); //Flip it according to its CenterX to appear Left Installed
                        RightSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsRightDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw9S9F9F:
                case CabinDrawNumber.Draw949F9F:
                case CabinDrawNumber.Draw9A9F:
                case CabinDrawNumber.Draw9C9F:
                case CabinDrawNumber.Draw9B9F9F:
                    if (PrimaryDraw != null)
                    {
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    if (SecondaryDraw != null)
                    {
                        SecondaryDraw.FlipHorizontally(RightSideCanvas.Item1 / 2d); //Flip it according to the Center of the Canvas X
                        RightSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsRightDrawAvailable = true;
                    }
                    if (TertiaryDraw != null)
                    {
                        LeftSideShapes.AddRange(TertiaryDraw.GetAllDraws());
                        IsLeftDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw2Corner8W82:
                    if (PrimaryDraw != null)
                    {
                        PrimaryDraw.FlipHorizontally(FrontSideCanvas.Item1 / 2d);//Flip it according to its CenterX to appear Left Installed
                        FrontSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    if (SecondaryDraw != null)
                    {
                        LeftSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsLeftDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw1Corner8W84:
                    if (SecondaryDraw != null)
                    {
                        FrontSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    if (PrimaryDraw != null)
                    {
                        LeftSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        IsLeftDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.Draw2CornerStraight8W88:
                    if (SecondaryDraw != null && TertiaryDraw != null)
                    {
                        FrontSideShapes.AddRange(SecondaryDraw.GetAllDraws());
                        //Flip about the End of First Glass (in this Case Secondary) plus the Free Opening
                        TertiaryDraw.FlipHorizontally(synthesis.Tertiary?.LengthMin/2d ?? 0);
                        TertiaryDraw.TranslateX((synthesis.Secondary?.LengthMin ?? 0) + CabinW.DefaultMinFreeOpening);
                        FrontSideShapes.AddRange(TertiaryDraw.GetAllDraws());
                        IsFrontDrawAvailable = true;
                    }
                    if (PrimaryDraw != null)
                    {
                        LeftSideShapes.AddRange(PrimaryDraw.GetAllDraws());
                        IsLeftDrawAvailable = true;
                    }
                    break;
                case CabinDrawNumber.None:
                default:
                    FrontSideCanvas = (0, 0);
                    RightSideCanvas = (0, 0);
                    LeftSideCanvas = (0, 0); //Not Available
                    break;
            }
        }

        /// <summary>
        /// Assigns the Given Color to all metal Parts Fill/Stroke Property
        /// </summary>
        /// <param name="CSSFillColor">The Css Color Name,Code or Variable</param>
        public void PaintMetalParts(string CSSFillColor , string CSSStrokeColor)
        {
            PrimaryDraw?.GetMetalFinishPartsDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            SecondaryDraw?.GetMetalFinishPartsDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            TertiaryDraw?.GetMetalFinishPartsDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
        }

        /// <summary>
        /// Assigns the Given Color to all GlassesDraws Fill/Stroke Property
        /// </summary>
        /// <param name="CSSFillColor">The Css Color Name,Code or Variable</param>
        public void PaintGlasses(string CSSFillColor , string CSSStrokeColor) 
        {
            PrimaryDraw?.GetGlassesDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            SecondaryDraw?.GetGlassesDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            TertiaryDraw?.GetGlassesDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
        }

        /// <summary>
        /// Assigns the Given Color to all PolycarbonicDraws Fill/Stroke Property
        /// </summary>
        /// <param name="CSSFillColor">The Css Color Name,Code or Variable</param>
        public void PaintPolycarbonics(string CSSFillColor , string CSSStrokeColor)
        {
            PrimaryDraw?.GetPolycarbonicsDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            SecondaryDraw?.GetPolycarbonicsDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            TertiaryDraw?.GetPolycarbonicsDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
        }

        /// <summary>
        /// Assigns the Given Color to all HelperDraws Fill/Stroke Property
        /// </summary>
        /// <param name="CSSFillColor">The Css Color Name,Code or Variable</param>
        public void PaintHelperDraws(string CSSFillColor, string CSSStrokeColor)
        {
            PrimaryDraw?.GetHelperDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            SecondaryDraw?.GetHelperDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
            TertiaryDraw?.GetHelperDraws().ForEach(d => { d.Fill = CSSFillColor; d.Stroke = CSSStrokeColor; });
        }

        /// <summary>
        /// Returns all the CabinDraws (Available Ones)
        /// </summary>
        /// <returns>The List of Cabin Draws</returns>
        public List<CabinDraw> GetCabinDrawsList()
        {
            List<CabinDraw> draws = new();
            if (PrimaryDraw != null)
            {
                draws.Add(PrimaryDraw);
            }
            if (SecondaryDraw != null)
            {
                draws.Add(SecondaryDraw);
            }
            if (TertiaryDraw != null)
            {
                draws.Add(TertiaryDraw);
            }
            return draws;
        }
    }
}

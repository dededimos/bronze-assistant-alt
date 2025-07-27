using BronzeArtWebApplication.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Validators;
using SVGCabinDraws;
using SVGCabinDraws.ConcreteDraws;
using SVGCabinDraws.ConcreteDraws.B6000Draws;
using SVGCabinDraws.ConcreteDraws.FreeDraws;
using SVGCabinDraws.ConcreteDraws.Inox304Draws;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
/*
Takes in a CabinSynthesis with the Help of SVGDrawLibrary & CabinDrawLibrary builds and Displays Various Draws
-Each Synthesis can have 1-3 draws , with the Prospect of putting also a top Draw
-The Parts of the Cabin are Categoirized in Glass-Metal-Polycarbonics which can be in turn be Painted
-The SVG's ViewBox Length and Height are Determined each time from the Containing Draw Max Height & Max Length -- Meaning the SynthesisDimensions of the DrawSide that is Displayed
-If a Draw is Not Available there is an Option to Display a Not Available Draw Message
*/
namespace BronzeArtWebApplication.Pages.CabinsPage.Components.DrawComponents
{
    /// <summary>
    /// Displays a Draw of the Passed Synthesis -- Wrap in an ErrorBoundary to Display not Available Draws
    /// </summary>
    public partial class CabinDrawSVG : ComponentBase
    {
        [Inject] public SynthesisDrawFactory synthesisDrawFactory { get; set; }

        /// <summary>
        /// The Background of the Draw
        /// </summary>
        [Parameter] public string BackgroundFill { get; set; } = "transparent";

        /// <summary>
        /// The Style of the Svg Containing the Draw
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Shows an Error Message instead of the Draw if the Draw is Null
        /// </summary>
        [Parameter] public bool ShowErrorOnMissingDraw { get; set; }

        /// <summary>
        /// The Padding of the SVG Draw inside its Container in X Axis.
        /// Units are in Real mm (To The Aspect Ratio of The Draw -- Big Dimensions will have small Margin)
        /// </summary>
        [Parameter] public double PaddingX { get; set; } = 0;
        /// <summary>
        /// The Padding of the SVG Draw inside its Container in Y Axis
        /// Units are in Real mm (To The Aspect Ratio of The Draw -- Big Dimensions will have small Margin)
        /// </summary>
        [Parameter] public double PaddingY { get; set; } = 40;

        private double viewBoxLength;
        private double viewBoxHeight;

        /// <summary>
        /// The Synthesis Tha will be Drawn
        /// </summary>
        [Parameter] public CabinSynthesis Synthesis { get; set; }

        /// <summary>
        /// The Draw Selected 1=Front , 2=First Available Side in order Right/Left , 3 = Always Left
        /// </summary>
        [Parameter] public int SelectedDraw { get; set; } = 1;

        /// <summary>
        /// Determines weather the Side Buttons are Visible
        /// </summary>
        [Parameter] public bool AreSideButtonsVisible { get; set; } = true;

        private SynthesisDraw draw;
        /// <summary>
        /// Wheather a Flipped Draw of an Angular Cabin (TypeA) is Active as a Draw. Used to Invert Animation opening for 9A , VA
        /// </summary>
        private bool IsActiveDrawAFlipped;
        private List<DrawShape> activeDraw;
        private bool isDrawAvailable;
        private string errorMessage;

        protected override void OnParametersSet()
        {

            draw = synthesisDrawFactory.CreateSynthesisDraw(Synthesis);
            PaintSVG();
            DefineDraw();

            //Apply Padding for all Draws that are available 
            //The Padding Movement of the Draws happens in the Lifecycle Method
            //Otherwise the Whenever the User changes the Active Draw the Translation is executed again.
            if (PaddingY != 0)
            {
                draw.GetCabinDrawsList().ForEach(d => d.GetAllDraws().ForEach(s => s.TranslateY(PaddingY / 2d)));
            }
            if (PaddingX != 0)
            {
                draw.GetCabinDrawsList().ForEach(d => d.GetAllDraws().ForEach(s => s.TranslateX(PaddingX / 2d)));
            }
        }

        /// <summary>
        /// Defines the Current Active Draw and ViewBox Size
        /// </summary>
        private void DefineDraw()
        {
            try
            {
                switch (SelectedDraw)
                {
                    case 1:
                        (viewBoxLength, viewBoxHeight) = draw.FrontSideCanvas;
                        activeDraw = draw.IsFrontDrawAvailable ? draw.FrontSideShapes : throw new Exception("CabinDrawNotAvailable");
                        IsActiveDrawAFlipped = false;
                        break;
                    case 2:
                        //Take the Canvas of Right first over left (Some Items make have only 2 draws with Primary and Left)
                        (viewBoxLength, viewBoxHeight) = draw.IsRightDrawAvailable ? draw.RightSideCanvas : draw.LeftSideCanvas; ;
                        activeDraw = draw.IsRightDrawAvailable ? draw.RightSideShapes : draw.IsLeftDrawAvailable ? draw.LeftSideShapes : throw new Exception("CabinDrawNotAvailable");

                        //HACKADIN!!!
                        IsActiveDrawAFlipped = draw.SecondaryDraw is Cabin9ADraw or CabinVADraw;
                        break;
                    case 3:
                        (viewBoxLength, viewBoxHeight) = draw.LeftSideCanvas;
                        activeDraw = draw.IsLeftDrawAvailable ? draw.LeftSideShapes : throw new Exception("CabinDrawNotAvailable");
                        IsActiveDrawAFlipped = false;
                        break;
                    default:
                        throw new Exception("CabinDrawNotAvailable");
                }

                //Apply the Padding to the View Box (The Translation of the Shapes happens on the LifeCycle Method
                //Otherwise Each Time the User Changes the active Draw All The SHapes get Translated Again
                viewBoxHeight += PaddingY;
                viewBoxLength += PaddingX;

                isDrawAvailable = true;
            }
            catch (Exception ex)
            {
                isDrawAvailable = false;
                errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Paints the various Parts of the draw
        /// </summary>
        private void PaintSVG()
        {
            string colorMetals = Synthesis.Primary.MetalFinish switch
            {
                CabinFinishEnum.Polished => "url(#ChromeGradient)",
                CabinFinishEnum.Brushed => "var(--BackgroundColor)",
                CabinFinishEnum.BlackMat => "url(#BlackGradient)",
                CabinFinishEnum.WhiteMat => "url(#WhiteGradient)",
                CabinFinishEnum.Bronze => "url(#BronzeGradient)",
                CabinFinishEnum.BrushedGold => "url(#GoldGradient)",
                CabinFinishEnum.Gold => "url(#GoldGradient)",
                CabinFinishEnum.Copper => "url(#CopperGradient)",
                CabinFinishEnum.Special => "ghostwhite",
                CabinFinishEnum.NotSet => "var(--BackgroundColor)",
                _ => "var(--BackgroundColor)",
            };

            string colorGlass = Synthesis.Primary.GlassFinish switch
            {
                GlassFinishEnum.Transparent => "aliceblue",
                GlassFinishEnum.Satin => "url(#sandblastPatternGlass)",
                GlassFinishEnum.Fume => "rgba(0, 0, 0, 0.9)",
                _ => "aliceblue"
            };

            draw.PaintMetalParts(colorMetals, "black");
            draw.PaintGlasses(colorGlass, "black");

            if (Synthesis.Primary.MetalFinish is CabinFinishEnum.BlackMat ||
                Synthesis.Primary.GlassFinish is GlassFinishEnum.Fume)
            {
                draw.PaintPolycarbonics("black", "black");
            }
            else
            {
                draw.PaintPolycarbonics("white", "black");
            }

            draw.PaintHelperDraws("url(#WallPatternStep)", "black");

        }

        /// <summary>
        /// Returns wheather a Draw Exists
        /// </summary>
        /// <returns>True if there is a Draw - False if there is Not</returns>
        public bool IsDrawAvailable()
        {
            return isDrawAvailable;
        }

    }
}

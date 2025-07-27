using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Helpers;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGGlassDrawsLibrary.Helpers;
using SVGGlassDrawsLibrary.ProcessDraws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGGlassDrawsLibrary
{
    public class GlassDraw : CompositeDraw
    {
        private readonly Glass glass;

        /// <summary>
        /// The Perimeter Draw of the Glass - Its always the First Draw in the Draws collection 
        /// Never Changes (When changed the whole draw must be replaced with new Draws inside)
        /// </summary>
        public GlassPerimeterDraw PerimeterDraw { get; private set; }
        public IEnumerable<DrawShape> Processes { get; private set; }
        public IEnumerable<DimensionLineDraw> Dimensions { get => Draws.Skip(Processes.Count() + 1).Cast<DimensionLineDraw>(); }


        public GlassDraw(Glass glass, bool shouldGenerateDimensions = false, string drawFill = "aliceblue")
        {
            this.glass = glass;
            var processes = glass.GetProcesses();
            var processDraws = processes.Select(p => p.GetProcessShape(glass));

            //Segregate Step from the rest if its there
            var processDrawsWithoutStep = processDraws.Where(d => d.GetType() != typeof(StepProcessDraw));
            var stepDraw = processDraws.Where(d => d.GetType() == typeof(StepProcessDraw)).FirstOrDefault();

            var perimeterDraw = new GlassPerimeterDraw(glass);
            if (stepDraw is not null) perimeterDraw.Clip = stepDraw;

            PerimeterDraw = perimeterDraw;
            Processes = processDrawsWithoutStep;
            AddDraw(perimeterDraw); //Add First
            AddDraw(processDrawsWithoutStep);  //Add later (to be designed after the perimeter)

            PerimeterDraw.Fill = drawFill;

            if (shouldGenerateDimensions)
            {
                GenerateDimensionDraws();
            }
        }

        /// <summary>
        /// Returns a Clone of the Glass Object of the Draw
        /// </summary>
        /// <returns></returns>
        public Glass ReadGlassObject()
        {
            return glass.GetDeepClone();
        }

        public override DrawShape CloneSelf()
        {
            throw new NotImplementedException();
        }

        private void GenerateDimensionDraws()
        {
            //Height Dimension Right of Glass
            DimensionLineDraw heightDim = new(PerimeterDraw.EndX, PerimeterDraw.StartY, PerimeterDraw.EndX, PerimeterDraw.EndY, 90)
            {
                RepresentedDimensionText = glass.Height.ToString()
            };
            heightDim.TranslateX(100); //Margin from Draw
            AddDraw(heightDim);

            //Generate Length Dimension Top
            DimensionLineDraw lengthDim = new(PerimeterDraw.StartX, PerimeterDraw.StartY, PerimeterDraw.EndX, PerimeterDraw.StartY, 0)
            {
                RepresentedDimensionText = glass.Length.ToString()
            };
            lengthDim.TranslateY(-50); //Margin from Draw
            AddDraw(lengthDim);

            //Generate the Extra Dimensions If there is Step
            if (glass.HasStep)
            {
                //Left Side Height
                DimensionLineDraw heightDim2 = new(PerimeterDraw.StartX, PerimeterDraw.StartY, PerimeterDraw.StartX, PerimeterDraw.EndY - glass.StepHeight, 90)
                {
                    RepresentedDimensionText = (glass.Height - glass.StepHeight).ToString()
                };
                heightDim2.TranslateX(-50);
                AddDraw(heightDim2);

                //Generate Length Dimension Bottom
                DimensionLineDraw lengthDim2 = new(PerimeterDraw.StartX + glass.StepLength, PerimeterDraw.EndY, PerimeterDraw.EndX, PerimeterDraw.EndY, 0)
                {
                    RepresentedDimensionText = (glass.Length - glass.StepLength).ToString()
                };
                lengthDim2.TranslateY(100); //Margin from Draw
                AddDraw(lengthDim2);
            }
            else
            {
                //Generate Length Dimension Bottom
                DimensionLineDraw lengthBottomDim = new(PerimeterDraw.StartX, PerimeterDraw.EndY, PerimeterDraw.EndX, PerimeterDraw.EndY, 0)
                {
                    RepresentedDimensionText = glass.Length.ToString()
                };
                lengthBottomDim.TranslateY(100); //Margin from Draw
                AddDraw(lengthBottomDim);
            }
            //Add Dimension Lines for the Processes
            //DIMENSIONS FOR ROUNDING 
            if (glass.CornerRadiusTopRight != 0)
            {
                //We need to find the Point where the Curve starts
                var xPointOfCurveMid = PerimeterDraw.EndX - glass.CornerRadiusTopRight / 2d;
                var yPointOfCurveMid = PerimeterDraw.StartY + glass.CornerRadiusTopRight / 3d;
                //Generate Diameter Dimension 
                DimensionLineDraw diameterDim = new(xPointOfCurveMid + 50, yPointOfCurveMid, xPointOfCurveMid + 150, yPointOfCurveMid, 0, arrowheads: DimensionLineDraw.DimensionArrowheads.Start)
                {
                    RepresentedDimensionText = $"Φ{glass.CornerRadiusTopRight}"
                };
                AddDraw(diameterDim);
            }


            //DIMENSIONS FOR STEP
            // HACK -- NEEDS FIXING
            //foreach (StepProcessDraw process in Processes.Where(p => p is StepProcessDraw))
            //{
            //    AddDraw(process.GenerateDimensionLines());
            //}

            if (PerimeterDraw.Clip is not null and StepProcessDraw step)
            {
                AddDraw(step.GenerateDimensionLines());
            }

        }

    }
}

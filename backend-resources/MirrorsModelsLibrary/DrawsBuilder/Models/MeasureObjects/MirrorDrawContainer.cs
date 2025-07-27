using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects
{
    /// <summary>
    /// The Container Holding a Mirror Draw
    /// </summary>
    public class MirrorDrawContainer
    {
        /// <summary>
        /// The Length of the Container
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// The Height of the Container
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// The Horizontal Margin of the Container with the Draw.
        /// Where the Draw Starts in the X Axis ,if the (0,0) is located at the Top Left of the Container
        /// </summary>
        public double MarginX { get; set; }
        /// <summary>
        /// The Verical Margin of the Container with The Draw
        /// Where the Draw Starts in the Y Axis ,if the (0,0) is located at the Top Left of the Container
        /// </summary>
        public double MarginY { get; set; }

        /// <summary>
        ///(DEPRECTAED!!!) The Scale Factor that must be applied to the Real Dimensions of the Contained Draws
        /// This way the draws do not Overflow the container
        ///Should we implement some kind of scale in the future to fit multiple draws inside the container
        ///Only then we can change the Scale factor -- otherwise we should keep the aspect ratio of the contianer the same with the mirror
        /// </summary>
        public double ScaleFactor { get; set; } = 1;

        /// <summary>
        /// Instantiates a draw Container for the Given Mirror Dimensions in mm
        /// </summary>
        /// <param name="drawnLength">The length of the Drawable Area</param>
        /// <param name="drawnHeight">The Height of the drawable area</param>
        /// <param name="margin">The Margin of the Drawable area from the Container</param>
        public MirrorDrawContainer(double drawnLength , double drawnHeight , double margin)
        {
            MarginX = margin;
            MarginY = margin;
            Length = drawnLength + 2 * MarginX;
            Height = drawnHeight + 2 * MarginY;
        }

        /// <summary>
        /// Instantiates a MirrorDrawContainer with the Defined Margin and the Mirror always centered to the Container
        /// </summary>
        /// <param name="mirror">The Mirror that will be drawn in the container</param>
        /// <param name="margin">The X,Y Margins that the mirror will have from the container</param>
        public MirrorDrawContainer(Mirror mirror , double margin)
        {
            switch (mirror.Shape)
            {
                case MirrorShape.Rectangular:
                case MirrorShape.Capsule:
                case MirrorShape.Ellipse:
                    Length = mirror.Lengthmm + 2 * margin;
                    Height = mirror.Heightmm + 2 * margin;
                    break;
                case MirrorShape.Circular:
                    Length = mirror.Diametermm + 2 * margin;
                    Height = mirror.Diametermm + 2 * margin;
                    break;
                case MirrorShape.Special:
                default:
                    Length = 0;
                    Height = 0;
                    break;
            }
            MarginX = margin;
            MarginY = margin;
        }



        /// <summary>
        /// Empty Constructor
        /// </summary>
        public MirrorDrawContainer()
        {
            
        }

        public void CalculateScaleFactor(double drawnLength , double drawnHeight)
        {
            //Check which is the Largest so we can scale according to this 
            double maxDimension = Math.Max(drawnLength, drawnHeight);

            if (maxDimension == drawnLength)
            {
                ScaleFactor = (Length - 2 * MarginX) / drawnLength;
            }
            else
            {
                ScaleFactor = (Height - 2 * MarginY) / drawnHeight;
            }
        }

        /// <summary>
        /// Scales a measure to the Draw Container
        /// </summary>
        /// <param name="measureToScale">The measure that need Scaling</param>
        /// <returns>Scaled measure</returns>
        public double GetScaledMeasure(double measureToScale)
        {
            measureToScale *= ScaleFactor;
            return measureToScale;
        }
    }
}

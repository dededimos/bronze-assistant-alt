using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGCabinDraws
{
    /// <summary>
    /// The SuperClass for Cabin Draws
    /// </summary>
    public abstract class CabinDraw
    {
        /// <summary>
        /// Wheather the Draw has been Flipped Horizontally from its original Drawing
        /// </summary>
        public bool IsFlipped { get; set; }

        /// <summary>
        /// Wheather the Draw has Been Translated from its original Position in X Axis
        /// </summary>
        public bool HasTranslatedX { get; set; }

        /// <summary>
        /// Wheather the Draw has Been Translated from its original Position in Y Axis
        /// </summary>
        public bool HasTranslatedY { get; set; }

        /// <summary>
        /// The metal Finish of the Cabin being drawn
        /// </summary>
        public abstract CabinFinishEnum MetalFinish { get; }
        public abstract double SingleDoorOpening { get; }

        /// <summary>
        /// Returns the List of Draws , If the Draw is Not Available it returns an Empty List
        /// </summary>
        /// <returns>Returns the List of Draws or Empty</returns>
        public abstract List<DrawShape> GetAllDraws();

        /// <summary>
        /// Gets the List of Draws of the Various Parts that have a MetalFinish
        /// </summary>
        /// <returns></returns>
        public abstract List<DrawShape> GetMetalFinishPartsDraws();

        /// <summary>
        /// Gets the List of Draws of the Glasses
        /// </summary>
        /// <returns></returns>
        public abstract List<DrawShape> GetGlassesDraws();

        /// <summary>
        /// Gets the List of Draws of the Polycarbonics
        /// </summary>
        /// <returns></returns>
        public abstract List<DrawShape> GetPolycarbonicsDraws();

        /// <summary>
        /// Gets the List of Helper Draws (Ex.Walls)
        /// </summary>
        /// <returns></returns>
        public virtual List<DrawShape> GetHelperDraws()
        {
            return new();
        }

        /// <summary>
        /// Builds all the Parts of the Draw
        /// </summary>
        protected abstract void InitilizeDraw();
        /// <summary>
        /// Places the Parts Inside the Draw (x,y)
        /// </summary>
        protected abstract void PlaceParts();
        /// <summary>
        /// Assignes Names to the Different Parts of the Draw
        /// </summary>
        protected abstract void PlaceDrawNames();

        /// <summary>
        /// Flips the Draw Horizontally
        /// </summary>
        /// <param name="flipOriginX">The X Coordinate about which we want to flip the Draw - Putting ContainersCenterX as the originX  - the Draw will flip according to the center of the Container</param>
        public void FlipHorizontally(double flipOriginX)
        {
            //Flip Horizontally all individual Parts
            foreach (var draw in GetAllDraws())
            {
                draw.FlipHorizontally(flipOriginX);
            }
            //Mark it as Flipped or not - depending on its previous state
            IsFlipped = !IsFlipped;
        }

        /// <summary>
        /// Translates the Shape over the X axis by the given amount
        /// </summary>
        /// <param name="translateX">The amount to Translate (takes also negative values)</param>
        public void TranslateX(double translateX)
        {
            foreach (var draw in GetAllDraws())
            {
                draw.TranslateX(translateX);
            }
            HasTranslatedX = true;
        }

        /// <summary>
        /// Translates the Shape over the Y axis by the given amount
        /// </summary>
        /// <param name="translateY">The amount to Translate (takes also negative values)</param>
        public void TranslateY(double translateY)
        {
            foreach (var draw in GetAllDraws())
            {
                draw.TranslateY(translateY);
            }
            HasTranslatedY = true;
        }

    }

    public abstract class CabinDraw<T> : CabinDraw 
        where T : Cabin
    {
        protected readonly T cabin;

        public CabinDraw(T cabin)
        {
            this.cabin = cabin;
            InitilizeDraw();
            PlaceParts();
            PlaceDrawNames();
        }
    }

}

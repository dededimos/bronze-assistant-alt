using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using SVGDrawingLibrary.Models;
using SVGGlassDrawsLibrary.ProcessDraws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SVGGlassDrawsLibrary.Helpers
{
    public static class GlassDrawExtensions
    {
        /// <summary>
        /// Returns the Shape of a Glass Process positioned in a certain Glass
        /// </summary>
        /// <typeparam name="T">The Runtime type of the Glass Process</typeparam>
        /// <param name="glassProcess">The Glass Process</param>
        /// <param name="glassOwner">The Glass owning the Process</param>
        /// <returns>The Draw of the Process</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static DrawShape GetProcessShape<T>(this T glassProcess,Glass glassOwner) where T : GlassProcess
        {
            return glassProcess switch
            {
                GlassHole hole => GlassHoleDraw.Create(hole, glassOwner),
                StepProcess step => StepProcessDraw.Create(step,glassOwner),
                CutHotel8000 cut8000 => CutHotel8000Draw.Create(cut8000, glassOwner),
                Cut9B cut9B => Cut9BDraw.Create(cut9B,glassOwner),
                _ => throw new NotImplementedException($"Draw Shape for a {glassProcess.GetType()} has not been implemented"),
            };
        }

        /// <summary>
        /// Returns the dX distance a GlassProcess from the Edge of its owner
        /// </summary>
        /// <param name="process">The Process</param>
        /// <param name="ownerGlass">The Glass owning the Process</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static double GetProcessDistanceFromEdgeX(this GlassProcess process , Glass ownerGlass)
        {
            return process.HorizontalDistancing switch
            {
                HorizDistancing.FromLeft => process.HorizontalDistance,
                HorizDistancing.FromRight => ownerGlass.Length - process.HorizontalDistance,
                HorizDistancing.FromMiddleLeft => ownerGlass.Length / 2d - process.HorizontalDistance,
                HorizDistancing.FromMiddleRight => ownerGlass.Length / 2d + process.HorizontalDistance,
                HorizDistancing.Undefined => throw new InvalidOperationException($"{nameof(process.HorizontalDistancing)} has Not been Set in {process.GetType().Name}"),
                _ => throw new NotSupportedException($"{nameof(process.HorizontalDistancing)} OF VALUE : {process.HorizontalDistancing} is not Supported"),
            };
        }
        /// <summary>
        /// Returns the dY distance a GlassProcess from the Edge of its owner
        /// </summary>
        /// <param name="process">The Process</param>
        /// <param name="ownerGlass">The Glass owning the Process</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static double GetProcessDistanceFromEdgeY(this GlassProcess process , Glass ownerGlass)
        {
            return process.VerticalDistancing switch
            {
                VertDistancing.FromTop => process.VerticalDistance,
                VertDistancing.FromBottom => ownerGlass.Height - process.VerticalDistance,
                VertDistancing.FromMiddleUp => ownerGlass.Height / 2d - process.VerticalDistance,
                VertDistancing.FromMiddleDown => ownerGlass.Height / 2d + process.VerticalDistance,
                VertDistancing.Undefined => throw new InvalidOperationException($"{nameof(process.VerticalDistancing)} has Not been Set in {process.GetType().Name}"),
                _ => throw new NotSupportedException($"{nameof(process.VerticalDistancing)} OF VALUE : {process.VerticalDistancing} is not Supported"),
            };
        }
    }
}

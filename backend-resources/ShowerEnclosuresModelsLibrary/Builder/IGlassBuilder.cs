using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder
{
    public interface IGlassBuilder
    {
        IGlassBuilder SetGlassType();                                       
        IGlassBuilder SetGlassDraw();
        IGlassBuilder SetGlassThickness();
        IGlassBuilder SetGlassFinish();
        IGlassBuilder SetGlassHeight();
        IGlassBuilder SetGlassLength();
        IGlassBuilder SetGlassStepHeight();
        IGlassBuilder SetGlassStepLength();
        IGlassBuilder SetCornerRadius();
        Glass GetGlass(); //AlwaysRuns Last , if we want to build a glass without all of its properties set this just returns what was built

        /// <summary>
        /// Fully Builds the Glass in Sequence
        /// </summary>
        /// <returns>The Built Glass</returns>
        Glass BuildGlass() 
        {
            SetGlassDraw();
            SetGlassFinish();
            SetGlassHeight();
            SetGlassLength();
            SetGlassStepHeight();
            SetGlassStepLength();
            SetCornerRadius();
            SetGlassThickness();
            SetGlassType();
            return GetGlass();
        }
    }
}

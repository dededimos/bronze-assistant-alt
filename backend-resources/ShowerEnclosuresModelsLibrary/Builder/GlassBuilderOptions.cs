using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Builder
{
    /// <summary>
    /// Options on how to Build Glasses on a Builder
    /// </summary>
    public class GlassBuilderOptions
    {
        /// <summary>
        /// Will Override the Length instead of Calculating it
        /// </summary>
        public double LengthOverride { get; set; }
        /// <summary>
        /// Will Override the Length instead of Calculating it
        /// </summary>
        public bool LengthShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Height instead of Calculating it
        /// </summary>
        public double HeightOverride { get; set; }
        /// <summary>
        /// Will Override the Height instead of Calculating it
        /// </summary>
        public bool HeightShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Draw instead of Calculating it
        /// </summary>
        public GlassDrawEnum DrawOverride { get; set; } = GlassDrawEnum.DrawNotSet;
        /// <summary>
        /// Will Override the Draw instead of Calculating it
        /// </summary>
        public bool DrawShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Type instead of Calculating it
        /// </summary>
        public GlassTypeEnum TypeOverride { get; set; } = GlassTypeEnum.GlassTypeNotSet;
        /// <summary>
        /// Will Override the Type instead of Calculating it
        /// </summary>
        public bool TypeShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Thickness instead of Calculating it
        /// </summary>
        public GlassThicknessEnum ThicknessOverride { get; set; } = GlassThicknessEnum.GlassThicknessNotSet;
        /// <summary>
        /// Will Override the Thickness instead of Calculating it
        /// </summary>
        public bool ThicknessShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Finish instead of Calculating it
        /// </summary>
        public GlassFinishEnum FinishOverride { get; set; } = GlassFinishEnum.GlassFinishNotSet;
        /// <summary>
        /// Will Override the Finish instead of Calculating it
        /// </summary>
        public bool FinishShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Radius instead of Calculating it
        /// </summary>
        public int CornerRadiusTopRightOverride { get; set; }
        /// <summary>
        /// Will Override the Radius instead of Calculating it
        /// </summary>
        public bool CornerRadiusTopRightShouldOverride { get; set; }
        /// <summary>
        /// Will Override the Radius instead of Calculating it
        /// </summary>
        public int CornerRadiusTopLeftOverride { get; set; }
        /// <summary>
        /// Will Override the Radius instead of Calculating it
        /// </summary>
        public bool CornerRadiusTopLeftShouldOverride { get; set; }
        /// <summary>
        /// Will Override the StepLength instead of Calculating it
        /// </summary>
        public double StepLengthOverride { get; set; }
        /// <summary>
        /// Will Override the StepLength instead of Calculating it
        /// </summary>
        public bool StepLengthShouldOverride { get; set; }
        /// <summary>
        /// Will Override the StepHeight instead of Calculating it
        /// </summary>
        public double StepHeightOverride { get; set; }
        /// <summary>
        /// Will Override the StepHeight instead of Calculating it
        /// </summary>
        public bool StepHeightShouldOverride { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators
{
    public record GlassValidationErrorCodes
    {
        /// <summary>
        /// Length of Glass must be inside boundaries definied by the Glass Class
        /// </summary>
        public const string GlassLengthOutOfBoundsError = "GlassLengthOutOfBoundsError";
        /// <summary>
        /// Height of Glass must be inside boundaries definied by the Glass Class
        /// </summary>
        public const string GlassHeightOutOfBoundsError = "GlassHeightOutOfBoundsError";
        /// <summary>
        /// Door/Moving Glasses Cannot Have Step
        /// </summary>
        public const string DoorGlassWithStepError = "DoorGlassWithStepError";
        /// <summary>
        /// Step Height and Length must 'both be Zero' or 'both Non Zero'
        /// </summary>
        public const string GlassStepInvalidDimensionsError = "GlassStepInvalidDimensionsError";
        /// <summary>
        /// Step Height or Length cannot Exceed Glass Dimensions
        /// </summary>
        public const string StepGreaterThanGlassError = "StepGreaterThanGlassError";

        public const string CornerRadiusOutOfBoundsError = "CornerRadiusOutOfBoundsError";

        public const string GlassDrawNotSetError = "GlassDrawNotSetError";
        public const string GlassTypeNotSetError = "GlassTypeNotSetError";
        public const string GlassThicknessNotSetError = "GlassThicknessNotSetError";
        public const string GlassFinishNotSetError = "GlassFinishNotSetError";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Validators
{
    public record CabinValidationErrorCodes
    {
        public const string InvalidCabinModel = "InvalidCabinModel";
        public const string InvalidDrawNumber = "InvalidDrawNumber";
        public const string InvalidCabinMetalFinish = "InvalidCabinMetalFinish";
        public const string InvalidCabinThickness = "InvalidCabinThickness";
        public const string CurrentDimensionsNeedBiggerThickness = "CurrentDimensionsNeedBiggerThickness";
        public const string InvalidCabinGlassFinish = "InvalidCabinGlassFinish";
        public const string InvalidCabinLength = "InvalidCabinLength";
        public const string InvalidCabinHeight = "InvalidCabinHeight";
        public const string MissingStepDimension = "MissingStepDimension";
        public const string StepHeightLessThanMinimum = "StepHeightLessThanMinimum";
        public const string StepDimensionsExceedCabin = "StepDimensionsExceedCabin";
        public const string SafeKidsCannotBeAppliedOnSatinGlass = "SafeKidsCannotBeAppliedOnSatinGlass";
        
        /// <summary>
        /// When Overlap is less than Zero
        /// </summary>
        public const string OverlapGreaterThanZero = "OverlapGreaterThanZero";
        /// <summary>
        /// When Handle Distance is Less than Zero
        /// </summary>
        public const string HDGreaterThanZero = "HDGreaterThanZero";
        /// <summary>
        /// When Cover Distance is out of the allowed levels
        /// </summary>
        public const string CoverDistanceOutOfBounds = "CoverDistanceOutOfBounds";
        /// <summary>
        /// When there is no handle in a model that Should have a handle
        /// </summary>
        public const string WithoutHandleError = "WithoutHandleError";

        /// <summary>
        /// When a step is inserted to B Models that do not have Fixed Side
        /// </summary>
        public const string BWithoutFixedCannotHaveStep = "9BWithoutFixedCannotHaveStep";

        /// <summary>
        /// When the Serigraphy Length of 9C is out of Bounds
        /// </summary>
        public const string Cabin9CSerigraphyOutOfRangeLength = "Cabin9CSerigraphyOutOfRangeLength";

        /// <summary>
        /// When 9C Has a step but not Transparent Glass
        /// </summary>
        public const string Cabin9CStepWithoutTransparentGlass = "Cabin9CStepWithoutTransparentGlass";

        /// <summary>
        /// When this Model Cannot take a Step Cut
        /// </summary>
        public const string CannotHaveStepError = "CannotHaveStep";

        /// <summary>
        /// When the Door Distance from bottom is not More than zero
        /// </summary>
        public const string DoorDistanceFromBottomGreaterThanZero = "DoorDistanceFromBottomGreaterThanZero";

        /// <summary>
        /// When there is no Closer in a Structure that must have one
        /// </summary>
        public const string WithoutCloserError = "WithoutCloserError";

        /// <summary>
        /// When Close Profile is Selected but no Handle is Selected
        /// </summary>
        public const string CloseProfileWithoutHandle = "CloseProfileWithoutHandle";

        /// <summary>
        /// When a Framed Glass has Rounding
        /// </summary>
        public const string PerimetricalFrameWithRounding = "PerimetricalFrameWithRounding";

        /// <summary>
        /// When the Length of the Door is out of Bounds
        /// </summary>
        public const string InvalidPartialLengthDoor = "InvalidPartialLengthDoor";
        /// <summary>
        /// When the Length of the Fixed Part is out of Bounds
        /// </summary>
        public const string InvalidPartialLengthFixed = "InvalidPartialLengthFixed";

        /// <summary>
        /// When no middle Hinge is Selected
        /// </summary>
        public const string WithoutMiddleHinge = "WithoutMiddleHinge";
    }

    public static class CabinPartValidationErrorCodes
    {
        /// <summary>
        /// When Part Code is Empty
        /// </summary>
        public const string EmptyPartCode = "EmptyPartCode";
        /// <summary>
        /// When Part Description is Empty
        /// </summary>
        public const string EmptyPartDescription = "EmptyPartDescription";
        /// <summary>
        /// When Part Type is Undefined
        /// </summary>
        public const string EmptyPartType = "EmptyPartType";
        /// <summary>
        /// When Part Material is Undefined
        /// </summary>
        public const string EmptyPartMaterial = "EmptyPartMaterial";
    }

}

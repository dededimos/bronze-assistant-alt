using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices
{
    public class CabinCalculationsService
    {
        private readonly GlassesBuilderDirector glassBuilder;
        private readonly CabinValidator validator;

        public CabinCalculationsService(GlassesBuilderDirector glassBuilder, CabinValidator validator)
        {
            this.glassBuilder = glassBuilder;
            this.validator = validator;
        }

        public ValidationResult BuildGlasses(CabinViewModel? viewModel)
        {
            if (viewModel?.CabinObject is null)
            {
                ValidationFailure failure = new("Empty", "lngEmptyCabin".TryTranslateKey()) { ErrorCode = "lngEmptyCabin".TryTranslateKey() };
                return new(new List<ValidationFailure>() { failure });
            }
            Cabin cabin = viewModel.CabinObject;
            ValidationResult result = validator.Validate(cabin);
            cabin.Glasses.Clear();

            if (result.IsValid)
            {
                glassBuilder.BuildAllGlasses(cabin);
                PartsDimensionsCalculator.CalculatePartsDimensions(cabin);
            }

            return result;
        }

        /// <summary>
        /// Build Only the Parts without Calculating the Glasses If the Cabin is Valid
        /// </summary>
        /// <param name="viewModel">The ViewModel of the Cabin</param>
        /// <returns>Weather the Build was Successful</returns>
        public ValidationResult BuildOnlyPartsAndValidate(CabinViewModel? viewModel)
        {
            if (viewModel?.CabinObject is null)
            {
                ValidationFailure failure = new("Empty", "lngEmptyCabin".TryTranslateKey()) { ErrorCode = "lngEmptyCabin".TryTranslateKey() };
                return new(new List<ValidationFailure>() { failure });
            }
            Cabin cabin = viewModel.CabinObject;
            ValidationResult result = validator.Validate(cabin);
            if (result.IsValid)
            {
                PartsDimensionsCalculator.CalculatePartsDimensions(cabin);
            }
            return result;
        }

        /// <summary>
        /// Validates a Cabin and if Valid Builds its Glasses and Parts
        /// </summary>
        /// <param name="cabin">The Cabin</param>
        /// <returns>The Result of the Validation</returns>
        public ValidationResult TryBuildGlassesAndParts(Cabin cabin)
        {
            ValidationResult result = validator.Validate(cabin);
            cabin.Glasses.Clear();

            if (result.IsValid)
            {
                glassBuilder.BuildAllGlasses(cabin);
                PartsDimensionsCalculator.CalculatePartsDimensions(cabin);
            }
            return result;
        }

        public string TranslateErrorCode(string errorCode , Cabin? cabin)
        {
            StringBuilder builder = new();

            if (cabin is null )
            {
                return "lngEmptyCabin".TryTranslateKey();
            }

            return errorCode switch
            {
                CabinValidationErrorCodes.InvalidCabinModel => builder
                                        .Append("InvalidCabinModel".TryTranslateKey())
                                        .Append(' ')
                                        .Append(cabin.Model?.ToString().TryTranslateKey() ?? "Empty")
                                        .Append(' ')
                                        .Append("InvalidCabinModel2".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.InvalidDrawNumber => builder
                                        .Append("InvalidDrawNumber".TryTranslateKey())
                                        .Append(' ')
                                        .Append('(')
                                        .Append(cabin.IsPartOfDraw.ToString().TryTranslateKey() ?? "Empty")
                                        .Append(')').ToString(),
                CabinValidationErrorCodes.InvalidCabinMetalFinish => builder
                                        .Append("InvalidCabinMetalFinish".TryTranslateKey())
                                        .Append(' ')
                                        .Append('(')
                                        .Append(cabin.MetalFinish?.ToString().TryTranslateKey() ?? "Empty")
                                        .Append(')').ToString(),
                CabinValidationErrorCodes.InvalidCabinThickness => builder
                                        .Append("InvalidCabinThickness".TryTranslateKey())
                                        .Append(' ')
                                        .Append('(')
                                        .Append(cabin.Thicknesses?.ToString().TryTranslateKey() ?? "Empty")
                                        .Append(')').ToString(),
                CabinValidationErrorCodes.CurrentDimensionsNeedBiggerThickness => builder
                                        .Append("CurrentDimensionsNeedBiggerThickness".TryTranslateKey())
                                        .Append(' ')
                                        .Append(Environment.NewLine)
                                        .Append("( >= ")
                                        .Append(cabin.Constraints?.BreakPointMinThickness.ToString().TryTranslateKey() ?? "Empty")
                                        .Append(" )").ToString(),
                CabinValidationErrorCodes.InvalidCabinGlassFinish => builder
                                        .Append("InvalidCabinGlassFinish".TryTranslateKey())
                                        .Append(' ')
                                        .Append('(')
                                        .Append(cabin.GlassFinish?.ToString().TryTranslateKey() ?? "Empty")
                                        .Append(')').ToString(),
                CabinValidationErrorCodes.InvalidCabinLength => builder
                                        .Append("InvalidCabinLength".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("lngLengthLimits".TryTranslateKey())
                                        .Append(' ')
                                        .Append(cabin.Constraints?.MinPossibleLength.ToString() ?? "????")
                                        .Append('-')
                                        .Append(cabin.Constraints?.MaxPossibleLength.ToString() ?? "????")
                                        .Append("mm").ToString(),
                CabinValidationErrorCodes.InvalidCabinHeight => builder
                                        .Append("InvalidCabinHeight".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("lngHeightLimits".TryTranslateKey())
                                        .Append(' ')
                                        .Append(cabin.Constraints?.MinPossibleHeight.ToString() ?? "????")
                                        .Append('-')
                                        .Append(cabin.Constraints?.MaxPossibleHeight.ToString() ?? "????")
                                        .Append("mm").ToString(),
                CabinValidationErrorCodes.MissingStepDimension => builder.Append("MissingStepDimension".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.StepHeightLessThanMinimum => builder.Append("StepHeightLessThanMinimum".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("lngStepHeightLimits".TryTranslateKey())
                                        .Append(" ( >= ")
                                        .Append(cabin.Constraints?.MinPossibleStepHeight.ToString() ?? "????")
                                        .Append(')').ToString(),
                CabinValidationErrorCodes.StepDimensionsExceedCabin => builder.Append("StepDimensionsExceedCabin".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.SafeKidsCannotBeAppliedOnSatinGlass => builder.Append("SafeKidsCannotBeAppliedOnSatinGlass".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.OverlapGreaterThanZero => builder.Append("OverlapGreaterThanZero".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.HDGreaterThanZero => builder.Append("HDGreaterThanZero".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.CoverDistanceOutOfBounds => builder.Append("CoverDistanceOutOfBounds".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("CoverDistanceOutOfBounds2".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.WithoutHandleError => builder.Append("WithoutHandleError".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.BWithoutFixedCannotHaveStep => builder.Append("9BWithoutFixedCannotHaveStep".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.Cabin9CSerigraphyOutOfRangeLength => builder.Append("Cabin9CSerigraphyOutOfRangeLength".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("lngAllowableSerigraphyLength".TryTranslateKey())
                                        .Append(' ')
                                        .Append(Cabin9C.AllowableSerigraphyLength1)
                                        .Append("mm")
                                        .Append(" , ")
                                        .Append(Cabin9C.AllowableSerigraphyLength2)
                                        .Append("mm").ToString(),
                CabinValidationErrorCodes.Cabin9CStepWithoutTransparentGlass => builder.Append("Cabin9CStepWithoutTransparentGlass".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.CannotHaveStepError => builder.Append("CannotHaveStepError".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.DoorDistanceFromBottomGreaterThanZero => builder.Append("DoorDistanceFromBottomGreaterThanZero".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.WithoutCloserError => builder.Append("WithoutCloserError".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.CloseProfileWithoutHandle => builder.Append("CloseProfileWithoutHandle".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.PerimetricalFrameWithRounding => builder.Append("PerimetricalFrameWithRounding".TryTranslateKey()).ToString(),
                CabinValidationErrorCodes.InvalidPartialLengthDoor => builder.Append("InvalidPartialLengthDoor".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("lngLengthLimits".TryTranslateKey())
                                        .Append(' ')
                                        .Append(TryGetConstraintString(nameof(ConstraintsHB.MinDoorLength), cabin))
                                        .Append(" - ")
                                        .Append(TryGetConstraintString(nameof(ConstraintsHB.MaxDoorLength), cabin))
                                        .Append("mm").ToString(),
                CabinValidationErrorCodes.InvalidPartialLengthFixed => builder.Append("InvalidPartialLengthDoor".TryTranslateKey())
                                        .Append(Environment.NewLine)
                                        .Append("lngLengthLimits".TryTranslateKey())
                                        .Append(' ')
                                        .Append(TryGetConstraintString(nameof(ConstraintsHB.MinFixedLength), cabin))
                                        .Append(" - ")
                                        .Append(TryGetConstraintString(nameof(ConstraintsHB.MaxFixedLength), cabin))
                                        .Append("mm").ToString(),
                CabinValidationErrorCodes.WithoutMiddleHinge => builder.Append("WithoutMiddleHinge".TryTranslateKey()).ToString(),
                _ => $"Error Code : '{errorCode}' ",
            };
        }
        /// <summary>
        /// Trys getting a Property from the Current Cabin Constraints based on the Name of the Property
        /// </summary>
        /// <param name="nameOfConstraint">The name of the Constraint</param>
        /// <returns>The Value of the Constraint as a string</returns>
        private static string TryGetConstraintString(string nameOfConstraint , Cabin cabin)
        {
            var constraint = cabin.Constraints?.GetType()?.GetProperty(nameOfConstraint)?.GetValue(cabin.Constraints, null)?.ToString() ?? "N/A";
            return constraint;
        }
    }
}

using FluentValidation;
using MirrorsLib.Validators;
using MirrorsRepositoryMongoDB.Entities;

namespace MirrorsRepositoryMongoDB.Validators
{
    public class MirrorSeriesElementEntityValidator : MirrorElementEntityBaseValidator<MirrorSeriesElementEntity>
    {
        public MirrorSeriesElementEntityValidator(bool includeIdValidation) : base(includeIdValidation, true)
        {
            RuleFor(s=> s.Constraints).SetValidator(new MirrorConstraintsValidator());
            RuleFor(s => s.AllowsTransitionToCustomizedMirror).NotEqual(false).When(s => s.IsCustomizedMirrorSeries, ApplyConditionTo.CurrentValidator).WithErrorCode("CustomizedSeriesMustAlwaysAllowTransitionToCustomizedMirrors");
            RuleFor(s => s.CustomizationTriggers).Must(s => s.Distinct().Count() == s.Count).WithErrorCode("$SeriesCustomizationTriggersDuplicates");
            RuleFor(s => s.StandardMirrors).Must(m => m.Count == 0).When(s => s.IsCustomizedMirrorSeries, ApplyConditionTo.CurrentValidator).WithErrorCode("CustomizedSeriesCannotHaveStandardMirrors");

            RuleForEach(s => s.StandardMirrors).SetValidator(new MirrorSynthesisEntityValidator());

            //Mirrors should always be of the Designated Series Shapes
            RuleForEach(s => s.StandardMirrors).Must((series, mirror) => series.Constraints.ConcerningMirrorShape == mirror.GeneralShapeType).WithErrorCode("MirrorsMustHaveAnAssignedSeriesShapeOnly");

            //Sandblasts in mirrors should always be of the Designated Default Type
            RuleForEach(s => s.StandardMirrors).Must((series, mirror) => (mirror.Sandblast is null && series.Constraints.AcceptsMirrorsWithoutSandblast) || series.Constraints.AllowedSandblasts.Any(s => s == mirror.Sandblast?.DefaultElementRefId)).WithErrorCode("MirrorsMustHaveAnAssignedSeriesSandblastOnly");

            //Supports in mirrors should always be of the Designated Default Type
            RuleForEach(s => s.StandardMirrors).Must((series, mirror) => (mirror.Support is null && series.Constraints.AcceptsMirrorsWithoutSupport) || series.Constraints.AllowedSupports.Any(s => s == mirror.Support?.DefaultElementRefId)).WithErrorCode("MirrorsMustHaveAnAssignedSeriesSupportOnly");

            //All mirrors must get the Id of the Series Parent
            RuleForEach(s => s.StandardMirrors).Must((series, mirror) => mirror.SeriesReferenceId == series.Id).WithErrorCode("AllMirrorsMustHaveSeriesRefIdTheSeriesId");

            //There should not be any Mirrors with the Same Code 
            RuleFor(s => s.StandardMirrors).Must(mirrors => mirrors.Select(m => m.Code).Distinct().Count() == mirrors.Count).WithErrorCode("AllStandardMirrorsMustHaveDifferentCodes");
        }
    }
}

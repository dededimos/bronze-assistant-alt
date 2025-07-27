using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorSeriesElementEntity : MirrorElementEntity, IDeepClonable<MirrorSeriesElementEntity> , IEqualityComparerCreator<MirrorSeriesElementEntity>
    {
        public MirrorSeriesElementEntity()
        {
            
        }
        public bool IsCustomizedMirrorSeries { get; set; }
        public bool AllowsTransitionToCustomizedMirror { get; set; }
        public MirrorConstraints Constraints { get; set; } = MirrorConstraints.EmptyConstraints();
        public List<MirrorSynthesisEntity> StandardMirrors { get; set; } = [];
        public List<MirrorModificationDescriptor> CustomizationTriggers { get; set; } = [];

        static IEqualityComparer<MirrorSeriesElementEntity> IEqualityComparerCreator<MirrorSeriesElementEntity>.GetComparer()
        {
            return new MirrorSeriesElementEntityEqualityComparer();
        }

        public override MirrorSeriesElementEntity GetDeepClone()
        {
            var clone = (MirrorSeriesElementEntity)base.GetDeepClone();
            clone.Constraints = this.Constraints.GetDeepClone();
            clone.StandardMirrors = this.StandardMirrors.GetDeepClonedList();
            clone.CustomizationTriggers = new(this.CustomizationTriggers);
            return clone;
        }

        public MirrorSeries ToSeries(IMirrorsDataProvider dataProvider)
        {
            var elementInfo = this.GetMirrorElementInfo();
            MirrorSeriesInfo seriesInfo = new()
            {
                StandardMirrors = this.StandardMirrors.Select(m=> m.ToMirrorSynthesis(dataProvider)).ToList(),
                CustomizationTriggers = new(this.CustomizationTriggers),
                IsCustomizedMirrorsSeries = this.IsCustomizedMirrorSeries,
                AllowsTransitionToCustomizedMirror = this.AllowsTransitionToCustomizedMirror,
                Constraints = this.Constraints.GetDeepClone(),
            };
            return new(elementInfo, seriesInfo);
        }
    }
    public class MirrorSeriesElementEntityEqualityComparer : IEqualityComparer<MirrorSeriesElementEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly MirrorSynthesisEntityEqualityComparer synthesisComparer = new();
        private readonly MirrorConstraintsEqualityComparer constraintsComparer = new();

        public bool Equals(MirrorSeriesElementEntity? x, MirrorSeriesElementEntity? y)
        {
            return baseComparer.Equals(x, y)
                && x!.IsCustomizedMirrorSeries == y!.IsCustomizedMirrorSeries
                && x.AllowsTransitionToCustomizedMirror == y.AllowsTransitionToCustomizedMirror
                && constraintsComparer.Equals(x.Constraints, y.Constraints)
                && x.CustomizationTriggers.SequenceEqual(y.CustomizationTriggers)
                && x.StandardMirrors.SequenceEqual(y.StandardMirrors,synthesisComparer);
        }

        public int GetHashCode([DisallowNull] MirrorSeriesElementEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

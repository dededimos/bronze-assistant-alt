using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class MirrorSeries : MirrorElementBase, IEqualityComparerCreator<MirrorSeries> , IDeepClonable<MirrorSeries>
    {
        public MirrorSeriesInfo SeriesInfo { get; set; }

        public MirrorSeries(IMirrorElement elementInfo , MirrorSeriesInfo seriesInfo)
            :base(elementInfo)
        {
            SeriesInfo = seriesInfo.GetDeepClone();
        }

        public new static IEqualityComparer<MirrorSeries> GetComparer()
        {
            return new MirrorSeriesEqualityComparer();
        }

        public override MirrorSeries GetDeepClone()
        {
            var clone = (MirrorSeries)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.SeriesInfo = this.SeriesInfo.GetDeepClone();
            return clone;
        }
        public static MirrorSeries UndefinedSeries() => new(Empty(), MirrorSeriesInfo.Undefined());

        public bool IsUndefined()
        {
            var undefined = MirrorSeries.UndefinedSeries();
            return MirrorSeries.GetComparer().Equals(this, undefined);
        }

    }
    public class MirrorSeriesEqualityComparer : IEqualityComparer<MirrorSeries>
    {
        private readonly MirrorElementEqualityComparer elementComparer = new();
        private readonly MirrorSeriesInfoEqualityComparer seriesComparer = new();

        public bool Equals(MirrorSeries? x, MirrorSeries? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementComparer.Equals(x,y) &&
                seriesComparer.Equals(x.SeriesInfo,y.SeriesInfo);
        }

        public int GetHashCode([DisallowNull] MirrorSeries obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    public class MirrorSeriesInfo : IDeepClonable<MirrorSeriesInfo> , IEqualityComparerCreator<MirrorSeriesInfo>
    {
        /// <summary>
        /// If the series is a Customized Mirrors Series
        /// <para>Customized Mirror Series Do not hold Standard Mirrors</para>
        /// </summary>
        public bool IsCustomizedMirrorsSeries { get; set; }
        /// <summary>
        /// Weather after selecting a Standard Mirror the user can customize it in a way that it will be considered a Customized Mirror
        /// <para>Setting this to false does not mean that the standard mirrors cannot take ANY customization whatsover</para>
        /// <para>It simply Prohibits using the Constraints of the equivalent Customized Series</para>
        /// </summary>
        public bool AllowsTransitionToCustomizedMirror { get; set; }

        public MirrorConstraints Constraints { get; set; } = MirrorConstraints.EmptyConstraints();

        /// <summary>
        /// Standard Mirrors for this Series
        /// <para>Standard Mirrors can have Overridden Elements</para>
        /// <para>CHANGING DEFAULT ELEMENTS IN DATABASE DOES NOT AUTO-CHANGE STANDARD MIRRORS DIMENSIONS (Not Intended)</para>
        /// </summary>
        public List<MirrorSynthesis> StandardMirrors { get; set; } = [];
        
        /// <summary>
        /// The List of Modifications that trigger a Customized Mirror instead of a standard one
        /// </summary>
        public List<MirrorModificationDescriptor> CustomizationTriggers { get; set; } = [];

        public static IEqualityComparer<MirrorSeriesInfo> GetComparer()
        {
            return new MirrorSeriesInfoEqualityComparer();
        }

        public static MirrorSeriesInfo Undefined() => new() { };

        public MirrorSeriesInfo GetDeepClone()
        {
            var clone = (MirrorSeriesInfo)this.MemberwiseClone();
            clone.Constraints = this.Constraints.GetDeepClone();
            clone.StandardMirrors = this.StandardMirrors.Select(m => m.GetDeepClone()).ToList();
            return clone;
        }
    }
    public class MirrorSeriesInfoEqualityComparer : IEqualityComparer<MirrorSeriesInfo>
    {
        private readonly MirrorSynthesisEqualityComparer mirrorComparer = new();
        private readonly MirrorConstraintsEqualityComparer constraintsComparer = new();

        public bool Equals(MirrorSeriesInfo? x, MirrorSeriesInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.IsCustomizedMirrorsSeries == y.IsCustomizedMirrorsSeries 
                && x.AllowsTransitionToCustomizedMirror == y.AllowsTransitionToCustomizedMirror
                && constraintsComparer.Equals(x.Constraints, y.Constraints)
                && x.StandardMirrors.SequenceEqual(y.StandardMirrors,mirrorComparer)
                && x.CustomizationTriggers.SequenceEqual(y.CustomizationTriggers);
        }

        public int GetHashCode([DisallowNull] MirrorSeriesInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

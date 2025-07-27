using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib
{
    public class MirrorConstraints : IDeepClonable<MirrorConstraints> , IEqualityComparerCreator<MirrorConstraints>
    {
        public BronzeMirrorShape ConcerningMirrorShape { get; set; } = BronzeMirrorShape.UndefinedMirrorShape;
        public HashSet<MirrorGlassType> AllowedGlassTypes { get; set; } = [];
        public HashSet<MirrorGlassThickness> AllowedGlassThicknesses { get; set; } = [];
        public double MaxMirrorLength { get; set; } = 2000;
        public double MinMirrorLength { get; set; } = 200;
        public double MaxMirrorHeight { get; set; } = 2000;
        public double MinMirrorHeight { get; set; } = 200;
        public double MaxAllowedWattage { get; set; } = 0;
        public HashSet<string> AllowedSandblasts { get; set; } = [];
        public HashSet<string> AllowedSupports { get; set; } = [];
        public HashSet<string> AllowedLights { get; set; } = [];
        public bool AllowsLight { get => AllowedLights.Count != 0; }
        public HashSet<string> AllowedModules { get; set; } = [];
        public HashSet<string> AllowedCustomElements { get; set; } = [];
        public bool CanHaveLight { get; set; }


        public bool AcceptsMirrorsWithoutSandblast { get; set; }
        public bool AcceptsMirrorsWithoutSupport { get; set; }
        public bool AcceptsMirrorsWithoutLight { get; set; }
        public IlluminationOption AllowedIllumination { get; set; }
        public HashSet<string> ObligatoryCustomElements { get; set; } = [];
        public HashSet<string> ObligatoryModules { get; set; } = [];
        /// <summary>
        /// A Collection of Exclusive Sets of Items , defining which items are mutually exclusive or mutually inclusive
        /// </summary>
        public HashSet<ExclusiveSet> ExclusiveSets { get; set; } = [];


        public MirrorConstraints()
        {
            
        }
        public static MirrorConstraints EmptyConstraints() => new();
        public MirrorConstraints GetDeepClone()
        {
            var clone = (MirrorConstraints)this.MemberwiseClone();
            clone.AllowedGlassTypes = new(this.AllowedGlassTypes);
            clone.AllowedGlassThicknesses = new(this.AllowedGlassThicknesses);
            clone.AllowedSandblasts = new(this.AllowedSandblasts);
            clone.AllowedSupports = new(this.AllowedSupports);
            clone.AllowedLights = new(this.AllowedLights);
            clone.AllowedModules = new(this.AllowedModules);
            clone.AllowedCustomElements = new(this.AllowedCustomElements);
            clone.ObligatoryCustomElements = new(this.ObligatoryCustomElements);
            clone.ObligatoryModules = new(this.ObligatoryModules);
            clone.ExclusiveSets = new(this.ExclusiveSets);
            return clone;
        }

        public static IEqualityComparer<MirrorConstraints> GetComparer()
        {
            return new MirrorConstraintsEqualityComparer();
        }
    }

    public class MirrorConstraintsEqualityComparer : IEqualityComparer<MirrorConstraints>
    {

        public bool Equals(MirrorConstraints? x, MirrorConstraints? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.ConcerningMirrorShape == y.ConcerningMirrorShape
                && x.AllowedGlassTypes.SequenceEqual(y.AllowedGlassTypes)
                && x.AllowedGlassThicknesses.SequenceEqual(y.AllowedGlassThicknesses)
                && x.MaxMirrorLength == y.MaxMirrorLength
                && x.MinMirrorLength == y.MinMirrorLength
                && x.MaxMirrorHeight == y.MaxMirrorHeight
                && x.MinMirrorHeight == y.MinMirrorHeight
                && x.MaxAllowedWattage == y.MaxAllowedWattage
                && x.AllowedSandblasts.SequenceEqual(y.AllowedSandblasts)
                && x.AllowedSupports.SequenceEqual(y.AllowedSupports)
                && x.AllowedLights.SequenceEqual(y.AllowedLights)
                && x.AllowedModules.SequenceEqual(y.AllowedModules)
                && x.AllowedCustomElements.SequenceEqual(y.AllowedCustomElements)
                && x.CanHaveLight == y.CanHaveLight
                && x.AllowedIllumination == y.AllowedIllumination
                && x.ObligatoryCustomElements.SequenceEqual(y.ObligatoryCustomElements)
                && x.ObligatoryModules.SequenceEqual(y.ObligatoryModules)
                && x.AcceptsMirrorsWithoutLight == y.AcceptsMirrorsWithoutLight
                && x.AcceptsMirrorsWithoutSandblast == y.AcceptsMirrorsWithoutSandblast
                && x.AcceptsMirrorsWithoutSupport == y.AcceptsMirrorsWithoutSupport
                && x.ExclusiveSets.SetEquals(y.ExclusiveSets);
        }
        public int GetHashCode([DisallowNull] MirrorConstraints obj)
        {
            throw new NotSupportedException($"{typeof(MirrorConstraintsEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

    /// <summary>
    /// Defines a set of items that are either mutually exclusive or mutually inclusive
    /// </summary>
    public record ExclusiveSet
    {
        public ExclusiveSet(string itemId1 , string itemId2 , bool mutuallyExclusive = true)
        {
            ItemId1 = itemId1;
            ItemId2 = itemId2;
            MutuallyExclusive = mutuallyExclusive;
        }
        /// <summary>
        /// The id of the first item of the set
        /// </summary>
        public string ItemId1 { get; set; }
        /// <summary>
        /// The Id of the second item of the set
        /// </summary>
        public string ItemId2 { get; set; }
        /// <summary>
        /// The items are mutually exclusive , meaning that they cannot be both present at the same time
        /// </summary>
        public bool MutuallyExclusive { get; set; }
        /// <summary>
        /// The items are mutually inclusive , meaning that they must be both present at the same time or none of them
        /// </summary>
        public bool MutuallyInclusive { get => !MutuallyExclusive; }
    }
}

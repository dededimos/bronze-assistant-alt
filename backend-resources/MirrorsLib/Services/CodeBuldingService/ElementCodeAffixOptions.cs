using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.Services.CodeBuldingService
{
    /// <summary>
    /// The Options of the Code's Affix generation for a particular element of the Mirror
    /// </summary>
    public class ElementCodeAffixOptions : IDeepClonable<ElementCodeAffixOptions>, IEqualityComparerCreator<ElementCodeAffixOptions>
    {
        /// <summary>
        /// Which part of the Element's Code to use
        /// </summary>
        public MirrorElementAffixCodeType CodeType { get; set; } = MirrorElementAffixCodeType.NoneCode;
        /// <summary>
        /// Overrides any Code with this string
        /// </summary>
        public string? OverrideCodeString { get; set; }
        public bool IsCodeTypeOverridden { get => !string.IsNullOrEmpty(OverrideCodeString); }

        /// <summary>
        /// The Replacement Code Affix when the Element is not present in the Parent
        /// </summary>
        public string? ReplacementCodeAffixWhenEmpty { get; set; }

        /// <summary>
        /// The Replacement affix of this part of the Code based on which is the Mirror Shape (what the affix will be when this Element is missing - different for each shape of the mirror)
        /// </summary>
        public Dictionary<BronzeMirrorShape, string> ReplacementCodeAffixBasedOnShape { get; set; } = [];

        /// <summary>
        /// The Order no with which to place the code affix
        /// </summary>
        public int PositionOrder { get; set; } = 0;
        /// <summary>
        /// Number of Charachters - if the Code is less then it is completed with the Filler Charachter until it reaches the Minimum or its Truncated if Bigger
        /// </summary>
        public int NumberOfCharachters { get; set; } = 0;
        public char FillerCharachter { get; set; } = 'x';

        /// <summary>
        /// Weather to Include the Element's Position Code when the element is a Positionable
        /// </summary>
        public bool IncludeElementPositionCode { get; set; }
        /// <summary>
        /// The CodeType Option of the Elements Position
        /// </summary>
        public MirrorElementAffixCodeType ElementPositionCodeTypeOption { get; set; }
        public int ElementPositionMinimumNumberOfCharachters { get; set; } = 0;
        /// <summary>
        /// The Charachter to Fill for the Position Code when below tha MinimumNumberOfCharachters
        /// </summary>
        public char ElementPositionFillerCharachter { get; set; } = 'p';

        public static ElementCodeAffixOptions EmptyAffix() => new();
        public static ElementCodeAffixOptions DefaultSeriesAffixGlass()
        {
            return new()
            {
                CodeType = MirrorElementAffixCodeType.MinimalElementCode,
                FillerCharachter = 'x',
                NumberOfCharachters = 2,
                PositionOrder = 1,
                ReplacementCodeAffixWhenEmpty = "sr",
            };
        }
        public static ElementCodeAffixOptions DefaultSeriesAffixMirror()
        {
            return new()
            {
                CodeType = MirrorElementAffixCodeType.ShortElementCode,
                FillerCharachter = 'x',
                NumberOfCharachters = 2,
                PositionOrder = 1,
                ReplacementCodeAffixWhenEmpty = "sr",
            };
        }
        public static ElementCodeAffixOptions DefaultSandblastAffixGlass()
        {
            return new()
            {
                CodeType = MirrorElementAffixCodeType.ShortElementCode,
                FillerCharachter = 'x',
                NumberOfCharachters = 2,
                PositionOrder = 1,
                ReplacementCodeAffixWhenEmpty = "-",
                ReplacementCodeAffixBasedOnShape = new()
                {
                    {BronzeMirrorShape.RectangleMirrorShape, "H7" },
                    {BronzeMirrorShape.CircleMirrorShape,"N9" },
                    {BronzeMirrorShape.EllipseMirrorShape,"E5" },
                    {BronzeMirrorShape.CapsuleMirrorShape,"NC" },
                    {BronzeMirrorShape.CircleQuadrantMirrorShape,"CQ" },
                    {BronzeMirrorShape.CircleSegmentMirrorShape,"CS" },
                    {BronzeMirrorShape.EggMirrorShape,"EG" },
                    {BronzeMirrorShape.RegularPolygonMirrorShape,"PL" }
                }
            };
        }
        public static ElementCodeAffixOptions DefaultSandblastedMagnifierAffix()
        {
            return new()
            {
                CodeType = MirrorElementAffixCodeType.MinimalElementCode,
                PositionOrder = 7,
                NumberOfCharachters = 1,
                IncludeElementPositionCode = true,
                ElementPositionCodeTypeOption = MirrorElementAffixCodeType.MinimalElementCode,
                ElementPositionMinimumNumberOfCharachters = 1,
                ElementPositionFillerCharachter = 'p',
                FillerCharachter = 'x',
            };
        }

        public ElementCodeAffixOptions GetDeepClone()
        {
            return (ElementCodeAffixOptions)this.MemberwiseClone();
        }

        public static IEqualityComparer<ElementCodeAffixOptions> GetComparer()
        {
            return new ElementCodeAffixOptionsEqualityComparer();
        }
    }

    public class ElementCodeAffixOptionsEqualityComparer : IEqualityComparer<ElementCodeAffixOptions>
    {
        public bool Equals(ElementCodeAffixOptions? x, ElementCodeAffixOptions? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.CodeType == y.CodeType
                && x.OverrideCodeString == y.OverrideCodeString
                && x.ReplacementCodeAffixWhenEmpty == y.ReplacementCodeAffixWhenEmpty
                && x.ReplacementCodeAffixBasedOnShape.IsEqualToOtherDictionary(y.ReplacementCodeAffixBasedOnShape)
                && x.PositionOrder == y.PositionOrder
                && x.NumberOfCharachters == y.NumberOfCharachters
                && x.FillerCharachter == y.FillerCharachter
                && x.IncludeElementPositionCode == y.IncludeElementPositionCode
                && x.ElementPositionCodeTypeOption == y.ElementPositionCodeTypeOption
                && x.ElementPositionMinimumNumberOfCharachters == y.ElementPositionMinimumNumberOfCharachters
                && x.ElementPositionFillerCharachter == y.ElementPositionFillerCharachter;
        }

        public int GetHashCode([DisallowNull] ElementCodeAffixOptions obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

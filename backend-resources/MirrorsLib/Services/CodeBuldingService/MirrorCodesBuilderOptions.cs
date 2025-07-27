using CommonHelpers.Exceptions;
using CommonHelpers;
using MirrorsLib.Enums;
using System.Diagnostics.CodeAnalysis;
using CommonInterfacesBronze;
using System.Xml.Linq;
using System.Linq;

namespace MirrorsLib.Services.CodeBuldingService
{
    public class MirrorCodesBuilderOptions : MirrorApplicationOptionsBase, IDeepClonable<MirrorCodesBuilderOptions>, IEqualityComparerCreator<MirrorCodesBuilderOptions>
    {
        /// <summary>
        /// A Dictionary of Positions as Keys and Seperator strings as Values
        /// <para>ex. (1,"GF") at the second position (index 1) the code will always have "GF"</para>
        /// </summary>
        public Dictionary<int, string> Separators { get; set; } = [];
        public MirrorCodeDimensionsUnit DimensionsUnit { get; set; } = MirrorCodeDimensionsUnit.cm;
        public int LengthAffixPosition { get; set; }
        public int HeightAffixPosition { get; set; }
        /// <summary>
        /// Weather these Code Options refer Only to the Glass of the Mirror and not the Whole Mirror
        /// </summary>
        public bool GlassOnlyOptions { get; set; }
        /// <summary>
        /// Weather these Code Options are for Complex Code Generation
        /// <para>If these options are Glass Only options , this proeprty is irrelevant</para>
        /// </summary>
        public bool ComplexCodeOptions { get; set; }
        /// <summary>
        /// The Charachter to Truncate if found in the End of the Constructed Code
        /// </summary>
        public char TruncatedTrailingCharachter { get; set; } = '-';

        public Dictionary<MirrorCodeOptionsElementType, ElementCodeAffixOptions> MirrorPropertiesCodeAffix { get; set; } = [];
        /// <summary>
        /// To Override the Default Module Affix
        /// </summary>
        public Dictionary<MirrorModuleType, ElementCodeAffixOptions> SpecificModuleCodeAffix { get; set; } = [];

        public override MirrorCodesBuilderOptions GetDeepClone()
        {
            var clone = (MirrorCodesBuilderOptions)this.MemberwiseClone();
            clone.Separators = new(this.Separators);
            clone.MirrorPropertiesCodeAffix = this.MirrorPropertiesCodeAffix.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetDeepClone());
            clone.SpecificModuleCodeAffix = this.SpecificModuleCodeAffix.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetDeepClone());
            return clone;
        }

        /// <summary>
        /// Resent the <see cref="AffixPresenter"/> objects of this Code , helping to present the Structure of the Code in a string
        /// </summary>
        /// <returns></returns>
        public List<AffixPresenter> GetPresenterAffixes()
        {
            List<AffixPresenter> presenters = [];

            #region 1.Separators
            foreach (var separator in Separators)
            {
                AffixPresenter presenter = new()
                {
                    AffixRefType = AffixPresenter.SeparatorRefType,
                    AffixString = separator.Value,
                    AffixStringReference = string.Empty,
                    Position = separator.Key,
                    StringWhenEmpty = string.Empty,
                };
                presenters.Add(presenter);
            }
            #endregion

            #region 2.Length/Height

            AffixPresenter lengthAffix = new()
            {
                AffixString = DimensionsUnit is MirrorCodeDimensionsUnit.cm ? "123" : "1234",
                AffixStringReference = string.Empty,
                AffixRefType = AffixPresenter.LengthRefType,
                Position = LengthAffixPosition,
                StringWhenEmpty = string.Empty
            };
            AffixPresenter heightAffix = new()
            {
                AffixString = DimensionsUnit is MirrorCodeDimensionsUnit.cm ? "456" : "5678",
                AffixStringReference = string.Empty,
                AffixRefType = AffixPresenter.HeightRefType,
                Position = HeightAffixPosition,
                StringWhenEmpty = string.Empty
            };
            presenters.Add(lengthAffix);
            presenters.Add(heightAffix);

            #endregion

            #region 3. Properties Affixes
            foreach (var affixOption in MirrorPropertiesCodeAffix)
            {
                var affix = affixOption.Value;
                var element = affixOption.Key;
                AffixPresenter presenter = new()
                {
                    AffixString = affix.OverrideCodeString.AddCharachtersIfSmallerTruncateIfBigger(affix.NumberOfCharachters,affix.FillerCharachter),
                    AffixStringReference = affix.CodeType.ToString(),
                    AffixRefType = element.ToString(),
                    Position = affix.PositionOrder,
                    StringWhenEmpty = affix.ReplacementCodeAffixBasedOnShape.Count != 0 ? affix.ReplacementCodeAffixBasedOnShape.First().Value : affix.ReplacementCodeAffixWhenEmpty ?? string.Empty
                };
                presenters.Add(presenter);

                if (affix.IncludeElementPositionCode)
                {
                    AffixPresenter positionPresenter = new()
                    {
                        AffixString = string.Empty.AddCharachtersIfSmallerTruncateIfBigger(affix.ElementPositionMinimumNumberOfCharachters,affix.ElementPositionFillerCharachter),
                        AffixStringReference = $"(Pos){affix.ElementPositionCodeTypeOption}",
                        AffixRefType = "Position",
                        Position = affix.PositionOrder,
                        StringWhenEmpty = string.Empty,
                    };
                    presenters.Add(positionPresenter);
                }

            }
            #endregion

            #region 4. Specific Modules Affixes
            foreach (var affixOption in SpecificModuleCodeAffix)
            {
                var affix = affixOption.Value;
                var module = affixOption.Key;
                AffixPresenter presenter = new()
                {
                    AffixString = affix.OverrideCodeString.AddCharachtersIfSmallerTruncateIfBigger(affix.NumberOfCharachters, affix.FillerCharachter),
                    AffixStringReference = affix.CodeType.ToString(),
                    AffixRefType = module.ToString(),
                    Position = affix.PositionOrder,
                    StringWhenEmpty = affix.ReplacementCodeAffixBasedOnShape.Count != 0 ? affix.ReplacementCodeAffixBasedOnShape.First().Value : affix.ReplacementCodeAffixWhenEmpty ?? string.Empty
                };
                presenters.Add(presenter);

                if (affix.IncludeElementPositionCode)
                {
                    AffixPresenter positionPresenter = new()
                    {
                        AffixString = string.Empty.AddCharachtersIfSmallerTruncateIfBigger(affix.ElementPositionMinimumNumberOfCharachters, affix.ElementPositionFillerCharachter),
                        AffixStringReference = $"(Pos){affix.ElementPositionCodeTypeOption}",
                        AffixRefType = "Position",
                        Position = affix.PositionOrder,
                        StringWhenEmpty = string.Empty,
                    };
                    presenters.Add( positionPresenter);
                }
            }
            #endregion

            return presenters.OrderBy(a => a.Position).ToList();
        }
        /// <summary>
        /// Returns the Affixes that can be Parsed from a Code String
        /// </summary>
        /// <returns></returns>
        public List<ParsableCodeAffix> GetParsableAffixes() 
        {
            List<ParsableCodeAffix> affixes = [];

            #region 1.Separators
            foreach (var separator in Separators)
            {
                ParsableCodeAffix affix = new()
                {
                    Position = separator.Key,
                    ExpectedNumberOfCharachters = separator.Value.Length,
                    Affix = separator.Value,
                    IsSeparatorAffix = true,
                };

                affixes.Add(affix);
            }
            #endregion

            #region 2.Length/Height

            ParsableCodeAffix lengthAffix = new()
            {
                Position = LengthAffixPosition,
                ExpectedNumberOfCharachters = DimensionsUnit is MirrorCodeDimensionsUnit.cm ? 3 : 4,
                IsLengthAffix = true,
                DimensionUnit = this.DimensionsUnit
            };
            ParsableCodeAffix heightAffix = new()
            {
                Position = HeightAffixPosition,
                ExpectedNumberOfCharachters = DimensionsUnit is MirrorCodeDimensionsUnit.cm ? 3 : 4,
                IsHeightAffix = true,
                DimensionUnit = this.DimensionsUnit
            };
            affixes.Add(lengthAffix);
            affixes.Add(heightAffix);

            #endregion

            #region 3. Properties Affixes
            foreach (var affixOptionsWithElement in MirrorPropertiesCodeAffix)
            {
                var affixOptions = affixOptionsWithElement.Value;
                var element = affixOptionsWithElement.Key;
                ParsableCodeAffix affix = new()
                {
                    Position = affixOptions.PositionOrder,
                    ExpectedNumberOfCharachters = affixOptions.NumberOfCharachters,
                    MirrorPropertyCodeAffix = element,
                    CodeType = affixOptions.CodeType,
                    ReplacementCodeAffixBasedOnShape = new(affixOptions.ReplacementCodeAffixBasedOnShape),
                    ReplacementCodeAffixWhenEmptyGeneral = affixOptions.ReplacementCodeAffixWhenEmpty,
                    FillerCharachter = affixOptions.FillerCharachter,
                    IncludesElementPositionCode = affixOptions.IncludeElementPositionCode,
                    ElementPositionCodeType = affixOptions.ElementPositionCodeTypeOption,
                    ElementPositionExpectedNumberOfCharachters = affixOptions.ElementPositionMinimumNumberOfCharachters,
                    ElementPositionFillerCharachter = affixOptions.ElementPositionFillerCharachter,
                };

                affixes.Add(affix);
            }
            #endregion

            #region 4. Specific Modules Affixes
            foreach (var affixOptionsWithModule in SpecificModuleCodeAffix)
            {
                var affixOptions = affixOptionsWithModule.Value;
                var module = affixOptionsWithModule.Key;
                ParsableCodeAffix affix = new()
                {
                    Position = affixOptions.PositionOrder,
                    ExpectedNumberOfCharachters = affixOptions.NumberOfCharachters,
                    MirrorPropertyCodeAffix = MirrorCodeOptionsElementType.Module,
                    SpecificModuleType = module,
                    CodeType = affixOptions.CodeType,
                    ReplacementCodeAffixBasedOnShape = new(affixOptions.ReplacementCodeAffixBasedOnShape),
                    ReplacementCodeAffixWhenEmptyGeneral = affixOptions.ReplacementCodeAffixWhenEmpty,
                    FillerCharachter = affixOptions.FillerCharachter,
                    IncludesElementPositionCode = affixOptions.IncludeElementPositionCode,
                    ElementPositionCodeType = affixOptions.ElementPositionCodeTypeOption,
                    ElementPositionExpectedNumberOfCharachters = affixOptions.ElementPositionMinimumNumberOfCharachters,
                    ElementPositionFillerCharachter = affixOptions.ElementPositionFillerCharachter,
                };
                affixes.Add(affix);
            }
            #endregion

            return affixes;
        }
        public bool AreEmptyOptions()
        {
            return Separators.Count == 0 
                && MirrorPropertiesCodeAffix.Count == 0 
                && SpecificModuleCodeAffix.Count == 0
                && LengthAffixPosition == 0
                && HeightAffixPosition == 0;
        }


        public static MirrorCodesBuilderOptions DefaultGlassCodeOptions()
        {
            MirrorCodesBuilderOptions options = new()
            {
                DimensionsUnit = MirrorCodeDimensionsUnit.mm,
                GlassOnlyOptions = true,
                Separators = new()
                {
                    {0,"60"},
                    {2,"-" },
                    {4, "-" },
                    {6,"-" },
                },
                HeightAffixPosition = 5,
                LengthAffixPosition = 3,
                MirrorPropertiesCodeAffix = new() { { MirrorCodeOptionsElementType.Sandblast, ElementCodeAffixOptions.DefaultSandblastAffixGlass() } },
                SpecificModuleCodeAffix = new() { { MirrorModuleType.MagnifierSandblastedModuleType, ElementCodeAffixOptions.DefaultSandblastedMagnifierAffix() }, },
            };
            return options;
        }
        public static MirrorCodesBuilderOptions DefaultMirrorCodeOptions()
        {
            MirrorCodesBuilderOptions options = new()
            {
                DimensionsUnit = MirrorCodeDimensionsUnit.mm,
                GlassOnlyOptions = false,
                Separators = new()
                {
                    {0,"60"},
                    {2,"-" },
                    {4, "-" },
                    {6,"-" },
                },
                HeightAffixPosition = 5,
                LengthAffixPosition = 3,
                MirrorPropertiesCodeAffix = new() { { MirrorCodeOptionsElementType.Series, ElementCodeAffixOptions.DefaultSeriesAffixMirror() } },
            };
            return options;
        }
        public static MirrorCodesBuilderOptions EmptyCodeBuilderOptions() => new();
        
        static IEqualityComparer<MirrorCodesBuilderOptions> IEqualityComparerCreator<MirrorCodesBuilderOptions>.GetComparer()
        {
            return new MirrorCodesBuilderOptionsEqualityComparer();
        }
    }
    public class MirrorCodesBuilderOptionsEqualityComparer : IEqualityComparer<MirrorCodesBuilderOptions>
    {
        private readonly ElementCodeAffixOptionsEqualityComparer affixOptionsComparer = new();

        public bool Equals(MirrorCodesBuilderOptions? x, MirrorCodesBuilderOptions? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Separators.IsEqualToOtherDictionary(y.Separators)
                && x.DimensionsUnit == y.DimensionsUnit
                && x.LengthAffixPosition == y.LengthAffixPosition
                && x.HeightAffixPosition == y.HeightAffixPosition
                && x.GlassOnlyOptions == y.GlassOnlyOptions
                && x.ComplexCodeOptions == y.ComplexCodeOptions
                && x.TruncatedTrailingCharachter == y.TruncatedTrailingCharachter
                && x.MirrorPropertiesCodeAffix.IsEqualToOtherDictionary(y.MirrorPropertiesCodeAffix, null, affixOptionsComparer)
                && x.SpecificModuleCodeAffix.IsEqualToOtherDictionary(y.SpecificModuleCodeAffix, null, affixOptionsComparer);
        }

        public int GetHashCode([DisallowNull] MirrorCodesBuilderOptions obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    public class AffixPresenter
    {
        public const string SeparatorRefType = "Separator";
        public const string LengthRefType = "Length";
        public const string HeightRefType = "Height";

        public static readonly string[] AllRefTypes =
        [
            SeparatorRefType, LengthRefType , HeightRefType,
            .. Enum.GetValues(typeof(MirrorCodeOptionsElementType)).Cast<MirrorCodeOptionsElementType>().Select(v=> v.ToString()),
            .. Enum.GetValues(typeof(MirrorModuleType)).Cast<MirrorModuleType>().Select(v=> v.ToString()),
        ];

        public string AffixRefType { get; set; } = string.Empty;
        public int Position { get; set; }
        public string StringWhenEmpty { get; set; } = string.Empty;
        /// <summary>
        /// The Actual presented Affix , if known or filled with the Filler Charachter
        /// </summary>
        public string AffixString { get; set; } = string.Empty;
        /// <summary>
        /// The Affix string Reference (Code Type e.t.c.)
        /// </summary>
        public string AffixStringReference { get; set; } = string.Empty;
    }

    public class ParsableCodeAffix
    {
        public int Position { get; set; }
        public int ExpectedNumberOfCharachters { get; set; }

        public string Affix { get; set; } = string.Empty;
        public int DimensionValue { get; set; } = 0;
        public string ElementCodeAffix { get; set; } = string.Empty;
        public string ElementPositionCodeAffix { get; set; } = string.Empty;

        public bool IsSeparatorAffix { get; set; }
        public bool IsLengthAffix { get; set; }
        public bool IsHeightAffix { get; set; }
        public MirrorCodeDimensionsUnit DimensionUnit { get; set; }
        public MirrorCodeOptionsElementType MirrorPropertyCodeAffix { get; set; }
        public MirrorModuleType SpecificModuleType { get; set; }

        public MirrorElementAffixCodeType CodeType { get; set; }
        public Dictionary<BronzeMirrorShape, string> ReplacementCodeAffixBasedOnShape { get; set; } = [];
        public string? ReplacementCodeAffixWhenEmptyGeneral { get; set; }
        public char FillerCharachter { get; set; }
        public bool IncludesElementPositionCode { get; set; }
        public MirrorElementAffixCodeType ElementPositionCodeType { get; set; }
        public int ElementPositionExpectedNumberOfCharachters { get; set; }
        public char ElementPositionFillerCharachter { get; set; }

        /// <summary>
        /// A Flag to indicate that the Affix must be skipped
        /// </summary>
        public bool SkipAffix { get; set; }
    }

}

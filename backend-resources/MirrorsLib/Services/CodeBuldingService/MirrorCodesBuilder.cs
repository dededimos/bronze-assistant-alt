using CommonHelpers;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Services.CodeBuldingService
{
    public class MirrorCodesBuilder : IDisposable
    {
        private readonly IMirrorsDataProvider dataProvider;

        private Lazy<MirrorCodesBuilderOptions> glassCodeOptions;
        private Lazy<MirrorCodesBuilderOptions> mirrorCodeOptions;
        private Lazy<MirrorCodesBuilderOptions> mirrorComplexCodeOptions;


        public MirrorCodesBuilder(IMirrorsDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            glassCodeOptions = new(dataProvider.Options.GetMirrorGlassCodeBuildingOptions());
            mirrorCodeOptions = new(dataProvider.Options.GetMirrorCodeBuildingOptions());
            mirrorComplexCodeOptions = new(dataProvider.Options.GetMirrorComplexCodeBuildingOptions());
            dataProvider.ProviderDataChanged += DataProvider_ProviderDataChanged;
        }

        private void DataProvider_ProviderDataChanged(object? sender, Type e)
        {
            //Reinitilize the code Options if the provider values have changed
            if (e == typeof(MirrorApplicationOptionsBase))
            {
                glassCodeOptions = new(dataProvider.Options.GetMirrorGlassCodeBuildingOptions());
                mirrorCodeOptions = new(dataProvider.Options.GetMirrorCodeBuildingOptions());
            }
        }

        /// <summary>
        /// Generates a Code for a Mirror with the provider generation Options
        /// </summary>
        /// <param name="generationOptions">The Options</param>
        /// <param name="mirror">The Mirror for which to generate the Code for</param>
        /// <returns></returns>
        public string GenerateCode(MirrorCodesBuilderOptions generationOptions, MirrorSynthesis mirror)
        {
            //Get all Affixes sorted by a Position and Flatten the Sorted List
            var affixes = GenerateAllAffixes(generationOptions, mirror).SelectMany(a => a.Value);

            StringBuilder builder = new();
            foreach (var item in affixes)
            {
                builder.Append(item);
            }
            return builder.ToString().Trim(generationOptions.TruncatedTrailingCharachter);
        }
        /// <summary>
        /// Generates the Code for the Glass of a Mirror Synthesis
        /// </summary>
        /// <param name="mirror"></param>
        /// <returns></returns>
        public string GenerateGlassCode(MirrorSynthesis mirror)
        {
            return GenerateCode(glassCodeOptions.Value, mirror);
        }
        /// <summary>
        /// Generates the Code for the Whole Mirror of a Mirror Synthesis
        /// </summary>
        /// <param name="mirror"></param>
        /// <param name="complexCode">Weather to generate the Complex Code rather than the normal one</param>
        /// <returns></returns>
        public string GenerateMirrorCode(MirrorSynthesis mirror, bool complexCode = false)
        {
            if (complexCode)
            {
                return GenerateCode(mirrorComplexCodeOptions.Value, mirror);
            }
            //Add mirror Searching here if sereis is not Special Dimensioning
            else
                return GenerateCode(mirrorCodeOptions.Value, mirror);
        }

        /// <summary>
        /// Generates all Code Affixes for a given Mirror
        /// </summary>
        /// <param name="mirror">The Mirror</param>
        /// <param name="options">The Options of affixes building</param>
        /// <param name="glassOnly">Weather it concerns only the Glass or the whole Mirror</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private SortedList<int, List<string>> GenerateAllAffixes(MirrorCodesBuilderOptions options, MirrorSynthesis mirror)
        {
            //A Sorted List with Positions as Keys and Lists of Affixes as Values.
            //Items with the Same Position are First in First Out
            SortedList<int, List<string>> affixes = [];

            //Local Method for adding to the Sorted List
            void AddAffix(int position, string affix)
            {
                if (affixes.TryGetValue(position, out List<string>? value)) value.Add(affix);
                else affixes.Add(position, [affix]);
            }

            #region 1. Separators
            //Seperators are added First , any items tied to their position are placed afterwards
            foreach (var item in options.Separators)
            {
                AddAffix(item.Key, item.Value);
            }
            #endregion

            #region 2. Length - Height
            //Height Length Afterwards
            var length = options.GlassOnlyOptions ? mirror.MirrorGlassShape.GetTotalLength() : mirror.DimensionsInformation.GetTotalLength();
            var height = options.GlassOnlyOptions ? mirror.MirrorGlassShape.GetTotalHeight() : mirror.DimensionsInformation.GetTotalHeight();

            //Transform to cm if needed - otherwise leave as is double
            if (options.DimensionsUnit is MirrorCodeDimensionsUnit.cm)
            {
                length = Math.Round(length / 10d, 0, MidpointRounding.AwayFromZero);
                height = Math.Round(height / 10d, 0, MidpointRounding.AwayFromZero);
            }
            //Eliminate any decimals (F0) and thousands seperators (invariant culture)
            AddAffix(options.LengthAffixPosition, new(length.ToString("F0", CultureInfo.InvariantCulture)));
            AddAffix(options.HeightAffixPosition, new(height.ToString("F0", CultureInfo.InvariantCulture)));
            #endregion

            #region Mirror Eelements Affixes
            //Get the Affixes of each Property of the Mirror
            foreach (var property in options.MirrorPropertiesCodeAffix.Keys)
            {
                ElementCodeAffixOptions affixOptions = options.MirrorPropertiesCodeAffix[property];
                string elementCode = string.Empty;

                if (string.IsNullOrEmpty(affixOptions.OverrideCodeString))
                {
                    switch (property)
                    {
                        case MirrorCodeOptionsElementType.Series:
                            var series = dataProvider.GetSeries(mirror.SeriesReferenceId).FirstOrDefault();
                            if (series is not null && !series.IsUndefined())
                            {
                                elementCode = GetCodeFromCodeTypeOption(affixOptions.CodeType, series);
                            }
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Support:
                            //if there is no Support check if there is option to Add String when there is no Support or Empty String (will get filled by the filler charachter)
                            //Otherwise do not Add any Affix if replacement is not provided 
                            if (mirror.Support is not null) elementCode = GetCodeFromCodeTypeOption(affixOptions.CodeType, mirror.Support);
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Sandblast:
                            //if there is no Sandblast check if there is option to Add String when there is no Sandblast or Empty String (will get filled by the filler charachter)
                            //Otherwise do not Add any Affix if replacement is not provided 
                            if (mirror.Sandblast is not null) elementCode = GetCodeFromCodeTypeOption(affixOptions.CodeType, mirror.Sandblast);
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Light:
                            //Check if we need to Supplement Code when there is no Light
                            //Add a string foreach one of the lights
                            if (mirror.Lights.Count != 0) foreach (var light in mirror.Lights) elementCode += GetCodeFromCodeTypeOption(affixOptions.CodeType, light.LightElement);
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Module:
                            var allModules = mirror.ModulesInfo.GetAllModules();

                            if (allModules.Any())
                            {
                                //Add a base String for each Module that is not Included in the Specific Module Options (which may add Modules in other Positions of the Code String)
                                foreach (var module in mirror.ModulesInfo.GetAllModules())
                                {
                                    //Add the module
                                    if (!options.SpecificModuleCodeAffix.ContainsKey(module.ModuleInfo.ModuleType))
                                    {
                                        //Add the Code of the Module
                                        elementCode += GetCodeFromCodeTypeOption(affixOptions.CodeType, module);
                                        elementCode = elementCode.AddCharachtersIfSmallerTruncateIfBigger(affixOptions.NumberOfCharachters, affixOptions.FillerCharachter);

                                        //Try to add the position
                                        if (affixOptions.IncludeElementPositionCode && module is IMirrorPositionable)
                                        {
                                            //find the Position
                                            var position = mirror.ModulesInfo.GetPositionOfModule(module);
                                            elementCode += position is not null
                                                ? GetCodeFromCodeTypeOption(affixOptions.ElementPositionCodeTypeOption, position)
                                                : string.Empty;
                                            int newMinimumCharachters = affixOptions.NumberOfCharachters + affixOptions.ElementPositionMinimumNumberOfCharachters;
                                            elementCode = elementCode.AddCharachtersIfSmallerTruncateIfBigger(newMinimumCharachters, affixOptions.ElementPositionFillerCharachter);
                                        }
                                    }
                                }

                                AddAffix(affixOptions.PositionOrder, elementCode);
                                //Add the Affix here and Continue Special Truncating for this Element (all modules are a single String)
                                continue;
                            }
                            //Add a replacement String if there are no Modules and there is an Option for a replacement string
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Finish:
                            if (mirror.Support is not null) elementCode = GetCodeFromCodeTypeOption(affixOptions.CodeType, mirror.Support.Finish);
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.CustomElement:
                            if (mirror.CustomElements.Count != 0) foreach (var element in mirror.CustomElements) elementCode += GetCodeFromCodeTypeOption(affixOptions.CodeType, element);
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        default:
                            throw new NotSupportedException($"{property} is not a supported Mirror Property Type for Code Generation");
                    }
                }
                //With Override String Present
                else
                {
                    switch (property)
                    {
                        case MirrorCodeOptionsElementType.Series:
                            var series = dataProvider.GetSeries(mirror.SeriesReferenceId).FirstOrDefault();
                            if (series is not null) elementCode = affixOptions.OverrideCodeString;
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Support:
                            if (mirror.Support is not null) elementCode = affixOptions.OverrideCodeString;
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Sandblast:
                            if (mirror.Sandblast is not null) elementCode = affixOptions.OverrideCodeString;
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Light:
                            if (mirror.Lights.Count != 0) elementCode = affixOptions.OverrideCodeString;
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Module:
                            if (mirror.ModulesInfo.GetAllModules().Any())
                            {
                                foreach (var module in mirror.ModulesInfo.GetAllModules())
                                {
                                    //Add the module
                                    if (!options.SpecificModuleCodeAffix.ContainsKey(module.ModuleInfo.ModuleType))
                                    {
                                        //Add the Code of the Module
                                        elementCode += affixOptions.OverrideCodeString;
                                        elementCode = elementCode.AddCharachtersIfSmallerTruncateIfBigger(affixOptions.NumberOfCharachters, affixOptions.FillerCharachter);

                                        //Try to add the position
                                        if (affixOptions.IncludeElementPositionCode && module is IMirrorPositionable)
                                        {
                                            //find the Position
                                            var position = mirror.ModulesInfo.GetPositionOfModule(module);
                                            elementCode += position is not null
                                                ? GetCodeFromCodeTypeOption(affixOptions.ElementPositionCodeTypeOption, position)
                                                : string.Empty;
                                            int newMinimumCharachters = affixOptions.NumberOfCharachters + affixOptions.ElementPositionMinimumNumberOfCharachters;
                                            elementCode = elementCode.AddCharachtersIfSmallerTruncateIfBigger(newMinimumCharachters, affixOptions.ElementPositionFillerCharachter);
                                        }
                                    }
                                }
                                AddAffix(affixOptions.PositionOrder, elementCode);
                                //Add the Affix here and Continue Special Truncating for this Element (all modules are a single String)
                                continue;
                            }
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.Finish:
                            if (mirror.Support is not null) elementCode = affixOptions.OverrideCodeString;
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        case MirrorCodeOptionsElementType.CustomElement:
                            if (mirror.CustomElements.Count != 0) elementCode += affixOptions.OverrideCodeString;
                            else if (affixOptions.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                            {
                                elementCode = value;
                            }
                            else if (!string.IsNullOrEmpty(affixOptions.ReplacementCodeAffixWhenEmpty))
                            {
                                elementCode = affixOptions.ReplacementCodeAffixWhenEmpty;
                            }
                            else continue;
                            break;
                        default:
                            throw new NotSupportedException($"{property} is not a supported Mirror Property Type for Code Generation");
                    }
                }

                elementCode = elementCode.AddCharachtersIfSmallerTruncateIfBigger(affixOptions.NumberOfCharachters, affixOptions.FillerCharachter);
                AddAffix(affixOptions.PositionOrder, elementCode);
            }
            #endregion

            #region Specific Modules Affixes For Modules that are Present
            //Add Affixes for the Specific Modules in Builder Options

            //Get All the Modules for which specific Affixes Exist
            var specificModules = mirror.ModulesInfo.GetAllModules().Where(m => options.SpecificModuleCodeAffix.ContainsKey(m.ModuleInfo.ModuleType));
            //Add the Code of each one of the Modules
            foreach (var module in specificModules)
            {
                string moduleCode = string.Empty;
                var affixOptions = options.SpecificModuleCodeAffix[module.ModuleInfo.ModuleType];

                //Get The Code Affix
                moduleCode += string.IsNullOrEmpty(affixOptions.OverrideCodeString)
                    ? GetCodeFromCodeTypeOption(affixOptions.CodeType, module)
                    : affixOptions.OverrideCodeString;
                //Truncate or Fill it
                moduleCode = moduleCode.AddCharachtersIfSmallerTruncateIfBigger(affixOptions.NumberOfCharachters, affixOptions.FillerCharachter);

                //Add any Position Extra part
                if (affixOptions.IncludeElementPositionCode && module.ModuleInfo is IMirrorPositionable)
                {
                    //find the Position
                    var position = mirror.ModulesInfo.GetPositionOfModule(module);
                    moduleCode += position is not null
                        ? GetCodeFromCodeTypeOption(affixOptions.ElementPositionCodeTypeOption, position)
                        : string.Empty;
                    int newMinimumCharachters = affixOptions.NumberOfCharachters + affixOptions.ElementPositionMinimumNumberOfCharachters;
                    moduleCode = moduleCode.AddCharachtersIfSmallerTruncateIfBigger(newMinimumCharachters, affixOptions.ElementPositionFillerCharachter);
                }

                AddAffix(affixOptions.PositionOrder, moduleCode);
            }
            #endregion

            #region Specific Modules Affixes For Modules that are not Present
            var allModulesTypes = mirror.ModulesInfo.GetAllModules().Select(m => m.ModuleInfo.ModuleType).Distinct().ToList();
            //For all module Types present in Affixes Specific Modules but not present in mirror
            foreach (var affix in options.SpecificModuleCodeAffix.Where(a => !allModulesTypes.Contains(a.Key)))
            {
                //If the replacement string Of particular shape is not Empty then apply it as an affix (meaning module not present => add affix to show its not)
                if (affix.Value.ReplacementCodeAffixBasedOnShape.TryGetValue(mirror.GeneralShapeType, out string? value))
                {
                    string elementCode = value.AddCharachtersIfSmallerTruncateIfBigger(affix.Value.NumberOfCharachters, affix.Value.FillerCharachter);
                    AddAffix(affix.Value.PositionOrder, elementCode);
                }
                //If the replacement string is not Empty then apply it as an affix (meaning module not present => add affix to show its not)
                else if (!string.IsNullOrEmpty(affix.Value.ReplacementCodeAffixWhenEmpty))
                {
                    string elementCode = affix.Value.ReplacementCodeAffixWhenEmpty.AddCharachtersIfSmallerTruncateIfBigger(affix.Value.NumberOfCharachters, affix.Value.FillerCharachter);
                    AddAffix(affix.Value.PositionOrder, elementCode);
                }
            }

            #endregion

            return affixes;
        }

        /// <summary>
        /// Returns the Code Affix String for a mirror Element
        /// </summary>
        /// <param name="option">The Code Option</param>
        /// <param name="element">The Element</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static string GetCodeFromCodeTypeOption(MirrorElementAffixCodeType option, IMirrorElement element)
        {
            return option switch
            {
                MirrorElementAffixCodeType.NoneCode => string.Empty,
                MirrorElementAffixCodeType.FullElementCode => element.Code,
                MirrorElementAffixCodeType.ShortElementCode => element.ShortCode,
                MirrorElementAffixCodeType.MinimalElementCode => element.MinimalCode,
                _ => throw new NotSupportedException($"{option} is not a Supported {nameof(MirrorElementAffixCodeType)}"),
            };
        }

        public void Dispose()
        {
            Dispose(true);
            //item already disposed above code , supress finalize
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                dataProvider.ProviderDataChanged -= DataProvider_ProviderDataChanged;
            }

            //object has been disposed
            _disposed = true;
        }
    }

    public class MirrorCodesParser
    {
        private readonly IMirrorsDataProvider dataProvider;
        private readonly MirrorSynthesisBuilder builder;

        public MirrorCodesParser(IMirrorsDataProvider dataProvider, Func<MirrorSynthesisBuilder> builderFactory)
        {
            this.dataProvider = dataProvider;
            this.builder = builderFactory.Invoke();
        }

        public MirrorSynthesis? ParseCodeToMirrorFromStandardSeries(string code)
        {
            string seriesAffix = code.Length >= 4 ? code[..4] : string.Empty;
            if (string.IsNullOrEmpty(seriesAffix)) return null;

            MirrorSynthesis synthesis = new();
            var series = dataProvider.GetAllSeries().FirstOrDefault(s => s.Code.StartsWith(seriesAffix, StringComparison.InvariantCultureIgnoreCase));
            if (series == null) return null;

            //NEEDS A LOT OF WORK.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a Code to a Mirror Synthesis
        /// </summary>
        /// <param name="code">The Code to parse into a Synthesis</param>
        /// <param name="wholeMirrorCodeOptions">Weather to use the Options of the Whole mirror OR just the Glass</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public MirrorSynthesis ParseCodeToMirror(string code, bool wholeMirrorCodeOptions = true)
        {
            var allSeries = dataProvider.GetAllSeries();
            //Normalize code if its in Greek to change into Latin
            code = code.NormalizeGreekToLatin();


            MirrorCodesBuilderOptions options = wholeMirrorCodeOptions 
                ? dataProvider.Options.GetMirrorComplexCodeBuildingOptions() 
                : dataProvider.Options.GetMirrorGlassCodeBuildingOptions();

            var parsedCodeObject = new ParsedMirrorCode(code, options.GetParsableAffixes());

            //find the series Affix with which the shape of the Mirror will be made by the Builder
            var seriesAffix = parsedCodeObject.Affixes.First(a => a.MirrorPropertyCodeAffix == MirrorCodeOptionsElementType.Series);
            var series = allSeries.FirstOrDefault(s =>
            {
                return seriesAffix.CodeType switch
                {
                    MirrorElementAffixCodeType.NoneCode => throw new Exception("The Series Affix Code Type cannot be None"),
                    MirrorElementAffixCodeType.FullElementCode => s.Code.Equals(seriesAffix.Affix, StringComparison.InvariantCultureIgnoreCase),
                    MirrorElementAffixCodeType.ShortElementCode => s.ShortCode.Equals(seriesAffix.Affix, StringComparison.InvariantCultureIgnoreCase),
                    MirrorElementAffixCodeType.MinimalElementCode => s.MinimalCode.Equals(seriesAffix.Affix, StringComparison.InvariantCultureIgnoreCase),
                    _ => throw new NotSupportedException($"The Affix Code Type : {seriesAffix.CodeType} is not Supported"),
                };
            }) ?? throw new Exception("The Series Affix does not match any Series in the Database");

            builder.SetShapeType(series.SeriesInfo.Constraints.ConcerningMirrorShape);
            
            //Set the Sandblast if there is one and if the Mirror does not accept without Sandblast
            if (series.SeriesInfo.Constraints.AcceptsMirrorsWithoutSandblast == false &&
                series.SeriesInfo.Constraints.AllowedSandblasts.FirstOrDefault() is string sandblastId)
            {
                builder.SetSandblast(sandblastId);
            }

            //Set the Support if there is one and if the Mirror does not accept without Support
            if (series.SeriesInfo.Constraints.AcceptsMirrorsWithoutSupport == false &&
                series.SeriesInfo.Constraints.AllowedSupports.FirstOrDefault() is string supportId)
            {
                builder.SetSupport(supportId);
            }

            //Set obligatory Modules if there are any
            foreach (var module in series.SeriesInfo.Constraints.ObligatoryModules)
            {
                builder.AddModule(module);
            }
            //Set obligary CustomElement if there are any
            //TODO : Add Custom Elements to the Mirror with the Builder

            foreach (var affix in parsedCodeObject.Affixes)
            {
                //skip the affixes that need skipping
                if (affix.SkipAffix) continue;
                //skip the series which is already above
                if (affix.MirrorPropertyCodeAffix == MirrorCodeOptionsElementType.Series) continue;
                else if (affix.MirrorPropertyCodeAffix is not MirrorCodeOptionsElementType.UndefinedElementType 
                    && affix.SpecificModuleType == MirrorModuleType.Undefined)
                {
                    switch (affix.MirrorPropertyCodeAffix)
                    {
                        case MirrorCodeOptionsElementType.Support:
                            var support = dataProvider.GetAllSupports().FirstOrDefault(s =>
                            {
                                return affix.CodeType switch
                                {
                                    MirrorElementAffixCodeType.NoneCode => throw new Exception("The Series Affix Code Type cannot be None"),
                                    MirrorElementAffixCodeType.FullElementCode => s.Code.Equals(affix.Affix, StringComparison.InvariantCultureIgnoreCase),
                                    MirrorElementAffixCodeType.ShortElementCode => s.ShortCode.Equals(affix.Affix, StringComparison.InvariantCultureIgnoreCase),
                                    MirrorElementAffixCodeType.MinimalElementCode => s.MinimalCode.Equals(affix.Affix, StringComparison.InvariantCultureIgnoreCase),
                                    _ => throw new NotSupportedException($"The Affix Code Type : {affix.CodeType} is not Supported"),
                                };
                            });
                            builder.SetSupport(support);
                            break;
                        case MirrorCodeOptionsElementType.Sandblast:
                            var sandblast = dataProvider.GetSandblasts().FirstOrDefault(s =>
                            {
                                return affix.CodeType switch
                                {
                                    MirrorElementAffixCodeType.NoneCode => throw new Exception("The Series Affix Code Type cannot be None"),
                                    MirrorElementAffixCodeType.FullElementCode => s.Code.Equals(affix.Affix, StringComparison.InvariantCultureIgnoreCase),
                                    MirrorElementAffixCodeType.ShortElementCode => s.ShortCode.Equals(affix.Affix, StringComparison.InvariantCultureIgnoreCase),
                                    MirrorElementAffixCodeType.MinimalElementCode => s.MinimalCode.Equals(affix.Affix, StringComparison.InvariantCultureIgnoreCase),
                                    _ => throw new NotSupportedException($"The Affix Code Type : {affix.CodeType} is not Supported"),
                                };
                            });
                            builder.SetSandblast(sandblast);
                            break;
                        case MirrorCodeOptionsElementType.Module:
                            //All the Modules are together in One Total String .
                            //Their Number of Charachters must match the CodeType they have (ex. 1 for Minimal , 2 for Short e.t.c.)
                            //Divide the affix with the expected number of charachters
                            //Early escape codeType is set to NoneCode => Exit
                            if(affix.CodeType == MirrorElementAffixCodeType.NoneCode) break;
                            var moduleCodes = affix.Affix.SplitBySize(affix.ExpectedNumberOfCharachters).Distinct().ToList();
                            foreach (var moduleCode in moduleCodes)
                            {
                                switch (affix.CodeType)
                                {
                                    case MirrorElementAffixCodeType.FullElementCode:
                                        var fullCodeModule = dataProvider.GetAllModules().FirstOrDefault(m => m.Code.Equals(moduleCode,StringComparison.InvariantCultureIgnoreCase));
                                        if(fullCodeModule is not null) builder.AddModule(fullCodeModule,null);
                                        break;
                                    case MirrorElementAffixCodeType.ShortElementCode:
                                        var shortCodeModule = dataProvider.GetAllModules().FirstOrDefault(m => m.ShortCode.Equals(moduleCode, StringComparison.InvariantCultureIgnoreCase));
                                        if (shortCodeModule is not null) builder.AddModule(shortCodeModule,null);
                                        break;
                                    case MirrorElementAffixCodeType.MinimalElementCode:
                                        var minmalCodeModule = dataProvider.GetAllModules().FirstOrDefault(m => m.MinimalCode.Equals(moduleCode, StringComparison.InvariantCultureIgnoreCase));
                                        if (minmalCodeModule is not null) builder.AddModule(minmalCodeModule,null);
                                        break;
                                    case MirrorElementAffixCodeType.NoneCode:
                                    default:
                                        throw new NotSupportedException($"{affix.CodeType} is not supported in Code Parsing of Modules");
                                }
                            }
                            break;
                        case MirrorCodeOptionsElementType.Light:
                            //still not implemented
                            break;
                        case MirrorCodeOptionsElementType.Finish:
                            //still not implemented
                            break;
                        case MirrorCodeOptionsElementType.CustomElement:
                            //still not implemented
                            break;
                        default:
                            //still not implemented
                            break;
                    }
                }
                else if (affix.IsLengthAffix)
                {
                    builder.FormulatedMirror.DimensionsInformation.SetTotalLength(affix.DimensionValue);
                }
                else if (affix.IsHeightAffix)
                {
                    builder.FormulatedMirror.DimensionsInformation.SetTotalHeight(affix.DimensionValue);
                }
            }
            var formulatedMirror = builder.FormulateMirror().GetDeepClone();
            builder.ResetBuilder();
            return formulatedMirror;
        }
    }

    /// <summary>
    /// Gets built from a code string .
    /// Saves all information of affixes and the parsed values of the code
    /// </summary>
    public class ParsedMirrorCode
    {
        public ParsedMirrorCode(string code, List<ParsableCodeAffix> parsableAffixes)
        {
            if (parsableAffixes.Any(a => a.MirrorPropertyCodeAffix == MirrorCodeOptionsElementType.Series) is false)
            {
                throw new Exception("MissConfigured Code Options , The Code Options must contain a Series Affix");
            }
            var affixes = parsableAffixes.OrderBy(a => a.Position).ToList();
            //The Actual positions will be the number of parsable affixes
            var numberOfAffixes = affixes.Count;

            var remainingCode = code;

            for (int i = 0; i < numberOfAffixes; i++)
            {
                //Cannot throw we have already counted the items
                var affix = affixes[i];
                if (string.IsNullOrEmpty(remainingCode)) { affix.SkipAffix = true; continue; }

                var previousAffix = i == 0 ? null : affixes[i - 1];
                var nextAffix = i == numberOfAffixes - 1 ? null : affixes[i + 1];

                if (affix.IsSeparatorAffix)
                {
                    //check if the affix of the separator matches the code
                    if (remainingCode.Length >= affix.ExpectedNumberOfCharachters)
                    {
                        var parsedAffix = remainingCode[..affix.ExpectedNumberOfCharachters];
                        if (parsedAffix != affix.Affix) throw new Exception($"The Code does not match the expected Affix of the Separator{Environment.NewLine}Position{affix.Position},Expected Separator: '{affix.Affix}' , Found Value: '{parsedAffix}'");
                        //set the remanining code as the previous remaining code minus the parsed affix

                        //There is no more code to parse
                        if (remainingCode.Length == affix.ExpectedNumberOfCharachters)
                        {
                            remainingCode = string.Empty;
                            continue;
                        }
                        //there are more affixes
                        else remainingCode = remainingCode[affix.ExpectedNumberOfCharachters..];
                    }
                    //If the remaining code is smaller then the provided code does not have all the affixes and we skip the rest of the affixes
                    else
                    {
                        //set the remaining code to empty and go to next iteration (further affixes will be skipped)
                        affix.SkipAffix = true;
                        remainingCode = string.Empty;
                        continue;
                    }
                }
                else if (affix.IsLengthAffix || affix.IsHeightAffix)
                {
                    if (previousAffix != null && !previousAffix.IsSeparatorAffix) throw new Exception("Missconfigured Code Options , The Previous Affix of Length/Height should always be a separator");
                    if (nextAffix != null && !nextAffix.IsSeparatorAffix) throw new Exception("Missconfigured Code Options , The Next Affix of Length/Height should always be a separator");

                    //any previous text should have been removed by the separator
                    //we need to find where the next separator is starting from and this is our dimension 

                    string dimensionString;
                    if (nextAffix is null || remainingCode.Contains(nextAffix.Affix, StringComparison.InvariantCultureIgnoreCase) is false) //there is no next affix
                    {
                        dimensionString = remainingCode;
                        remainingCode = string.Empty;
                    }
                    //There are more affixes afterwards
                    else
                    {
                        var separator = nextAffix.Affix;
                        dimensionString = remainingCode[..remainingCode.IndexOf(separator)];
                        remainingCode = remainingCode[dimensionString.Length..];
                    }

                    affix.DimensionValue = int.TryParse(dimensionString, out int length) ? length : 0;
                    if (affix.DimensionUnit == MirrorCodeDimensionsUnit.cm) affix.DimensionValue *= 10;
                }
                else
                {
                    if (affix.SpecificModuleType != MirrorModuleType.Undefined)
                    {
                        //parse exact number of charachters here , Modules can be without separators on either end 
                        //check if the affix of the separator matches the code
                        if (remainingCode.Length >= affix.ExpectedNumberOfCharachters)
                        {
                            var parsedAffix = remainingCode[..affix.ExpectedNumberOfCharachters];
                            //set the remanining code as the previous remaining code minus the parsed affix
                            //There is no more code to parse
                            if (remainingCode.Length == affix.ExpectedNumberOfCharachters)
                            {
                                remainingCode = string.Empty;
                                continue;
                            }
                            //there are more affixes
                            else remainingCode = remainingCode[affix.ExpectedNumberOfCharachters..];
                        }
                        //If the remaining code is smaller then the provided code does not have all the affixes and we skip the rest of the affixes
                        else
                        {
                            //set the remaining code to empty and go to next iteration (further affixes will be skipped)
                            affix.SkipAffix = true;
                            remainingCode = string.Empty;
                            continue;
                        }
                    }
                    else
                    {
                        //this is a general Module with unspecified ModuleType ,
                        //meaning the affix will have from zero to many Modules inside it , each one represented by a string equal to the number of charachters
                        //if there is no seperator after this affix then there is no possibility to know where it ends ...
                        if (nextAffix != null && !nextAffix.IsSeparatorAffix) throw new Exception($"General ModuleType on a Code which contains all modules except of specific , must have a separator aftrer it , so that to distinguish its end from the next affix");

                        //any previous text should have been removed by the separator
                        //we need to find where the next separator is starting from and this is our Modules String
                        if (nextAffix is null || remainingCode.Contains(nextAffix.Affix, StringComparison.InvariantCultureIgnoreCase) is false) //there is no next affix
                        {
                            affix.Affix = remainingCode;
                            remainingCode = string.Empty;
                        }
                        //there are more affixes afterwards
                        else
                        {
                            var separator = nextAffix.Affix;
                            affix.Affix = remainingCode[..remainingCode.IndexOf(separator)];
                            remainingCode = remainingCode[affix.Affix.Length..];
                        }

                    }
                }
            }

            this.RemainingCodeAfterParsing = remainingCode;
            Affixes = affixes;
        }

        public List<ParsableCodeAffix> Affixes { get; set; } = [];

        /// <summary>
        /// The Part of the provided code that remains after parsing (more charachters than needed typed?)
        /// </summary>
        public string RemainingCodeAfterParsing { get; set; }
    }


}

using Microsoft.Extensions.Logging;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using ShapesLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Services
{
    /// <summary>
    /// An object that is responsible of assigning Series to Mirrors and Codes if they match a Standard Mirror
    /// </summary>
    public class MirrorCustomizationAnalyzer
    {
        public MirrorCustomizationAnalyzer(IMirrorsDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        private readonly IMirrorsDataProvider dataProvider;
        private readonly ShapeInfoEqualityComparer shapeComparer = new();
        private readonly MirrorLightEqualityComparer lightComparer = new(new(true, true, true));
        private readonly MirrorModuleEqualityComparer moduleComparer = new(false, true);
        private readonly MirrorElementPositionEqualityComparer positionComparer = new();


        /// <summary>
        /// Assigns a Series to a Mirror and an Overridden Code if it matches a Standard Mirror
        /// <para>1. Finds all the Series Candiates for this Mirror (Customized or Non Customized) , Checks Shape-Sandblasts-Supports-HasLight </para>
        /// <para>2. Finds the StandardMirrors in the non Customized Series that could potentially match with the mirror (same dimensions,support,sandblast)</para>
        /// <para>3. If matches Are found in the Standard mirrors , checks if there are any Modification Triggers in Lights or Modules</para>
        /// <para>4. If there are no Triggers it assigns the seriesRefId AND Overrides the Code with that of the matching mirror</para>
        /// <para>5. If there are Triggers or if there are not matching Standard Mirrors it assigns the Customized Series</para>
        /// <para>6. If there is No Customized Series , it assigns Undefined</para>
        /// </summary>
        /// <param name="analizedMirror"></param>
        /// <returns></returns>
        public AnalizedMirrorResult AnalizeMirror(MirrorSynthesis analizedMirror)
        {
            AnalizedMirrorResult result = new(analizedMirror);

            //Find the potential series to assign to the Mirror
            var seriesCanditates = FindSeriesCandidates(analizedMirror);
            //Early Escape if there are no series to assign
            if (!seriesCanditates.Any()) return result; 

            //Divide the series into two groups one for customized and one for non customized
            var nonCustomizedSeries = seriesCanditates.Where(s => s.SeriesInfo.IsCustomizedMirrorsSeries == false);
            var customizedSeries = seriesCanditates.Where(s => s.SeriesInfo.IsCustomizedMirrorsSeries);

            MirrorSynthesis? standardMirrorMatch = null;
            MirrorSeries? standardSeriesMatch = null;
            MirrorSeries? customizedNonStandardSeries = customizedSeries.FirstOrDefault();

            //Now we have to check weather the mirror fits in one of the predefined mirrors or if it is customized

            //Check weather the mirror fits in one of the predefined mirrors
            foreach (var s in nonCustomizedSeries)
            {
                //find mirrors with the same Dimensions , Sandblast and Frame in the Standard Mirrors of the Series
                var matchingStandardMirrors = s.SeriesInfo.StandardMirrors.Where(standardMirror => MatchesBasicCriteriaOfStandardMirror(standardMirror, analizedMirror));

                //if there are no matchning mirrors go to the next series there is no match here
                if (!matchingStandardMirrors.Any()) continue;

                //Get the Triggers of the Series that could possibly trigger a Customization
                var triggers = s.SeriesInfo.CustomizationTriggers;

                //For each mirror that matches the dimensions/Sandblast and Support check the modifications of the lights and modules
                foreach (var matchedStandardMirror in matchingStandardMirrors)
                {
                    if (HasTriggeringModificationsLights(matchedStandardMirror, analizedMirror, [.. triggers])
                      || HasTriggeringModificationsModules(matchedStandardMirror, analizedMirror, [.. triggers]))
                        continue; //If there are any triggering modifications then go to the next mirror
                    else
                    {
                        //If there are no triggering modifications then assign the series / the matched Mirror and return the result
                        standardSeriesMatch = s;
                        standardMirrorMatch = matchedStandardMirror;
                    }
                }
            }
            
            return result
                .SetStandardSeriesMatch(standardSeriesMatch)
                .SetCustomizationSeriesMatch(customizedNonStandardSeries)
                .SetMirrorMatch(standardMirrorMatch);
        }

        /// <summary>
        /// Finds the Potential Series that can be assigned to a Mirror
        /// </summary>
        /// <param name="mirror"></param>
        /// <returns></returns>
        private IEnumerable<MirrorSeries> FindSeriesCandidates(MirrorSynthesis mirror)
        {
            return dataProvider.GetAllSeries().Where(s =>
            {
                //Match the shape of the mirror with the series
                return s.SeriesInfo.Constraints.ConcerningMirrorShape == mirror.GeneralShapeType
                //Match weather the series accepts light and the mirror has one
                && ( mirror.Lights.Count != 0 ? s.SeriesInfo.Constraints.AllowsLight : s.SeriesInfo.Constraints.AcceptsMirrorsWithoutLight )
                //Find the sandblasts with the same element Id as the mirrors (overridden sandblasts will have the same element Id as non Overidden ones)
                && (mirror.Sandblast == null ? s.SeriesInfo.Constraints.AcceptsMirrorsWithoutSandblast : s.SeriesInfo.Constraints.AllowedSandblasts.Any(sandblastId => sandblastId == mirror.Sandblast.ElementId))
                //Find the supports with the same element Id as the mirrors (overridden supports will have the same element Id as non Overidden ones)
                && (mirror.Support == null ? s.SeriesInfo.Constraints.AcceptsMirrorsWithoutSupport : s.SeriesInfo.Constraints.AllowedSupports.Any(supportId => supportId == mirror.Support.ElementId));
            });
        }

        /// <summary>
        /// Weather the modified Mirrors has Light Modifications that trigger a Customization
        /// </summary>
        /// <param name="standardMirror"></param>
        /// <param name="modifiedMirror"></param>
        /// <param name="triggers">The Triggers for which to check if there are any between the mirrors</param>
        /// <returns></returns>
        private bool HasTriggeringModificationsLights(MirrorSynthesis standardMirror, MirrorSynthesis modifiedMirror, params MirrorModificationDescriptor[] triggers)
        {
            //Gather all types in the same list (as the lights triggers are the same ) , and combine/normalize the Modification Types
            var modificationTypes = triggers
                .Where(t => t.Modification == MirrorElementModification.LightModification || t.Modification == MirrorElementModification.AnyElementModification)
                .Select(t => t.ModificationType)
                .Distinct();

            //There are no triggering modifications for the Lights Registered 
            if (!modificationTypes.Any()) return false;

            //Find the actual modifications of lights between the mirrors
            var modifications = FindLightsModifications(modifiedMirror, standardMirror);
            //Check them against the provided triggers (usually from a SeriesInfo object)
            return modifications.Any(m => triggers.Any(t => t.IsEqualIncludingAnyTypes(m)));
        }
        /// <summary>
        /// Finds the type of modifications for the Lights of a Mirror
        /// <para>Exlcudes Lights that are for the Magnifiers</para>
        /// </summary>
        /// <param name="modifiedMirror">The Modified Mirror</param>
        /// <param name="standardMirror">The Standard Mirror</param>
        /// <returns></returns>
        private List<MirrorModificationDescriptor> FindLightsModifications(MirrorSynthesis modifiedMirror, MirrorSynthesis standardMirror)
        {
            List<MirrorModificationDescriptor> modifications = [];
            //check the lights  (exclude magnifier only lights)
            var standardLights = standardMirror.Lights.Where(l => !l.AdditionalLightInfo.IsOnlyMagnifierLight).ToList();
            var modifiedLights = modifiedMirror.Lights.Where(l => !l.AdditionalLightInfo.IsOnlyMagnifierLight).ToList();

            //Iterate backwards to be able to remove elements from the list without messing with indices
            for (int i = standardLights.Count - 1; i >= 0; i--)
            {
                var current = standardLights[i]; // Get the current element by index
                var match = modifiedLights.FirstOrDefault(m => lightComparer.Equals(m, current)); // Find match
                if (match != null) // if found remove both from the lists
                {
                    standardLights.Remove(current);
                    modifiedLights.Remove(match);
                }
            }
            //If the counts are equal then we only had change Modifications
            if (standardLights.Count == modifiedLights.Count)
            {
                modifications.Add(new(MirrorModificationType.Change, MirrorElementModification.LightModification));
            }
            else if (standardLights.Count > modifiedLights.Count) //If the standard has more lights than the modified then we had a removal
            {
                modifications.Add(new(MirrorModificationType.Removal, MirrorElementModification.LightModification));
                //if the modified light count is not zero then we also had a change except from the removal
                if (modifiedLights.Count != 0) modifications.Add(new(MirrorModificationType.Change, MirrorElementModification.LightModification));
            }
            else //If the standard has less lights than the modified then we had an addition
            {
                modifications.Add(new(MirrorModificationType.Addition, MirrorElementModification.LightModification));
                //if the standard light count is not zero then we also had a change except from the addition
                if (standardLights.Count != 0) modifications.Add(new(MirrorModificationType.Change, MirrorElementModification.LightModification));
            }
            return modifications;
        }

        /// <summary>
        /// Weather the modified Mirror has Module Modifications that trigger a Customization
        /// </summary>
        /// <param name="standardMirror"></param>
        /// <param name="modifiedMirror"></param>
        /// <param name="triggers"></param>
        /// <returns></returns>
        private bool HasTriggeringModificationsModules(MirrorSynthesis standardMirror, MirrorSynthesis modifiedMirror, params MirrorModificationDescriptor[] triggers)
        {
            //Flatten the descriptors to only Modification Types , the Find Modules will take care of the picking up the types it needs
            var modificationTypes = triggers.Select(t => t.ModificationType).Distinct();
            if (!modificationTypes.Any()) return false;

            //Find all the Modification Types that could possibly trigger a Customization
            var modifications = FindModulesModifications(modifiedMirror, standardMirror, triggers.Select(t => t.Modification).ToArray());
            //Check them against the provided triggers (usually from a SeriesInfo object)
            return modifications.Any(m => triggers.Any(t => t.IsEqualIncludingAnyTypes(m)));
        }
        /// <summary>
        /// Finds the Modifications between the Modules of a Modified Mirror and a Standard Mirror , for the specified ModificationTypes
        /// </summary>
        /// <param name="modifiedMirror">The Modified Mirror</param>
        /// <param name="standardMirror">The Standard Mirror</param>
        /// <param name="modificationsTypes">The Modifications in which we are intrested in</param>
        /// <returns></returns>
        private List<MirrorModificationDescriptor> FindModulesModifications(MirrorSynthesis modifiedMirror, MirrorSynthesis standardMirror, params MirrorElementModification[] modificationsTypes)
        {
            //Check which types of modules we are interested in
            var moduleTypes = GetModuleTypesFromElementModificationTypes(modificationsTypes);

            //Find the Specified Modules in both Mirrors
            var modifiedModules = modifiedMirror.ModulesInfo.GetAllModulesWithPositionIncludingNonPositionables().Where(m => moduleTypes.Any(t => t == m.Module.ModuleInfo.ModuleType)).ToList();
            var standardModules = standardMirror.ModulesInfo.GetAllModulesWithPositionIncludingNonPositionables().Where(m => moduleTypes.Any(t => t == m.Module.ModuleInfo.ModuleType)).ToList();
            List<MirrorModificationDescriptor> modifications = [];

            //Iterate backwards to be able to remove elements from the list without messing with indices
            for (int i = standardModules.Count - 1; i >= 0; i--)
            {
                var current = standardModules[i]; // Get the current element by index

                //Search for a Complete match including the position
                var match = modifiedModules.FirstOrDefault(m => moduleComparer.Equals(m.Module, current.Module) && positionComparer.Equals(m.Position, current.Position)); // Find match
                if (match != null) // if found remove both from the lists
                {
                    standardModules.Remove(current);
                    modifiedModules.Remove(match);
                    continue;//go to the next iteration
                }
            }

            //If the counts are equal then we only had change Modifications
            if (standardModules.Count == modifiedModules.Count)
            {
                foreach (var module in standardModules)
                {
                    var modificationType = GetElementModificationFromModuleType(module.Module.ModuleInfo.ModuleType);
                    modifications.Add(new(MirrorModificationType.Change, modificationType));
                }
            }
            else if (standardModules.Count > modifiedModules.Count) //If the standard has more modules than the modified then we had at least one removal
            {
                //If the modified Modules are not zero , all of them are Changes
                if (modifiedModules.Count != 0)
                {
                    foreach (var module in modifiedModules)
                    {
                        var modificationType = GetElementModificationFromModuleType(module.Module.ModuleInfo.ModuleType);
                        modifications.Add(new(MirrorModificationType.Change, modificationType));
                    }
                }

                //For the standard modules we have to have a logic on how to find which types of Modules have been removed
                //So we will keep the ModuleTypes that remain in the standardMirror after deducting the module types present on the modified mirror
                //this cannot be done with Except Linq method as it will remove duplicates and we need to keep the duplicates
                var remainingModuleTypes = standardModules.Select(m => m.Module.ModuleInfo.ModuleType).ToList();
                foreach (var moduleType in modifiedModules) remainingModuleTypes.Remove(moduleType.Module.ModuleInfo.ModuleType);

                //Now we have the remaining Module Types that are not present in the modified mirror
                foreach (var moduleType in remainingModuleTypes)
                {
                    modifications.Add(new(MirrorModificationType.Removal, GetElementModificationFromModuleType(moduleType)));
                }
            }
            else //If the modified Mirror has more modules than the Standard then we had at least one addition
            {
                //We implement the same logic as per the removals
                if (standardModules.Count != 0)
                {
                    foreach (var module in standardModules)
                    {
                        var modificationType = GetElementModificationFromModuleType(module.Module.ModuleInfo.ModuleType);
                        modifications.Add(new(MirrorModificationType.Change, modificationType));
                    }
                }

                //For the modified modules we have to have a logic on how to find which types of Modules have been added
                //So we will keep the ModuleTypes that remain in the modifiedMirror after deducting the module types present on the standard mirror
                //this cannot be done with Except Linq method as it will remove duplicates and we need to keep the duplicates
                var remainingModuleTypes = modifiedModules.Select(m => m.Module.ModuleInfo.ModuleType).ToList();
                foreach (var moduleType in standardModules) remainingModuleTypes.Remove(moduleType.Module.ModuleInfo.ModuleType);

                //Now we have the remaining Module Types that are not present in the standard mirror
                foreach (var moduleType in remainingModuleTypes)
                {
                    modifications.Add(new(MirrorModificationType.Addition, GetElementModificationFromModuleType(moduleType)));
                }
            }
            return modifications;
        }

        /// <summary>
        /// Weather a modified Mirror Matches the Basic Criteria of a Standard Mirror
        /// <para>Dimensions - Sandblast - Suporrt/Frame Equality</para>
        /// </summary>
        /// <param name="standardMirror"></param>
        /// <param name="modifiedMirror"></param>
        /// <returns></returns>
        private bool MatchesBasicCriteriaOfStandardMirror(MirrorSynthesis standardMirror, MirrorSynthesis modifiedMirror)
        {
            return shapeComparer.Equals(standardMirror.DimensionsInformation, modifiedMirror.DimensionsInformation)
            && (modifiedMirror.Sandblast == null ? standardMirror.Sandblast == null : modifiedMirror.Sandblast.ElementId == standardMirror.Sandblast?.ElementId)
            && (modifiedMirror.Support == null ? standardMirror.Support == null : modifiedMirror.Support.ElementId == standardMirror.Support?.ElementId);
        }

        /// <summary>
        /// Returns the MirrorElement Modification Type from a Module Type
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">When the Module Type is not Supported</exception>
        private MirrorElementModification GetElementModificationFromModuleType(MirrorModuleType moduleType)
        {
            return moduleType switch
            {
                MirrorModuleType.BluetoothModuleType => MirrorElementModification.BluetoothModification,
                MirrorModuleType.MagnifierSandblastedModuleType => MirrorElementModification.MagnifierSandblastedModification,
                MirrorModuleType.MagnifierModuleType => MirrorElementModification.MagnifierModification,
                MirrorModuleType.MirrorBackLidModuleType => MirrorElementModification.LightModification,
                MirrorModuleType.MirrorLampModuleType => MirrorElementModification.MirrorLampModification,
                MirrorModuleType.ResistancePadModuleType => MirrorElementModification.ResistancePadModification,
                MirrorModuleType.ScreenModuleType => MirrorElementModification.ScreenModuleModification,
                MirrorModuleType.TouchButtonModuleType => MirrorElementModification.TouchButtonModification,
                MirrorModuleType.TransformerModuleType => MirrorElementModification.TransformerModification,
                MirrorModuleType.ProcessModuleType => MirrorElementModification.ProcessModification,
                _ => throw new NotSupportedException($"Module of Type : {moduleType} is not supported by any Type of : {nameof(MirrorElementModification)}"),
            };
        }
        /// <summary>
        /// Returns the Module Types from the Element Modification Types
        /// </summary>
        /// <param name="elementModificationTypes"></param>
        /// <returns></returns>
        private IEnumerable<MirrorModuleType> GetModuleTypesFromElementModificationTypes(params MirrorElementModification[] elementModificationTypes)
        {
            var hasAllTypes = elementModificationTypes.Contains(MirrorElementModification.AnyElementModification);
            //If the AnyElementModification is provided then we should return all types of Modules supported by Modifications
            //(meaning all except Undefined and Rounded Corners)
            if (hasAllTypes)
            {
                return Enum.GetValues<MirrorModuleType>().Cast<MirrorModuleType>().Except([MirrorModuleType.Undefined, MirrorModuleType.RoundedCornersModuleType]);
            }
            else
            {
                List<MirrorModuleType> moduleTypes = [];
                foreach (var modificationType in elementModificationTypes)
                {
                    var moduleType = ConvertMirrorElementModificationToMirrorModuleType(modificationType);
                    if (moduleType == MirrorModuleType.Undefined) continue; //skip unefined Types
                    moduleTypes.Add(moduleType);
                }
                return moduleTypes.Distinct();
            }
        }
        /// <summary>
        /// Converts a MirrorElementModification to a MirrorModuleType
        /// </summary>
        /// <param name="elementModificationType"></param>
        /// <returns></returns>
        private static MirrorModuleType ConvertMirrorElementModificationToMirrorModuleType(MirrorElementModification elementModificationType)
        {
            return elementModificationType switch
            {
                MirrorElementModification.BluetoothModification => MirrorModuleType.BluetoothModuleType,
                MirrorElementModification.MagnifierSandblastedModification => MirrorModuleType.MagnifierSandblastedModuleType,
                MirrorElementModification.MagnifierModification => MirrorModuleType.MagnifierModuleType,
                MirrorElementModification.MirrorBackLidModification => MirrorModuleType.MirrorBackLidModuleType,
                MirrorElementModification.MirrorLampModification => MirrorModuleType.MirrorLampModuleType,
                MirrorElementModification.ResistancePadModification => MirrorModuleType.ResistancePadModuleType,
                MirrorElementModification.ScreenModuleModification => MirrorModuleType.ScreenModuleType,
                MirrorElementModification.TouchButtonModification => MirrorModuleType.TouchButtonModuleType,
                _ => MirrorModuleType.Undefined,
            };
        }

    }
    public class AnalizedMirrorResult
    {
        /// <summary>
        /// The Standard Series Matched to the Analized Mirror
        /// <para>If null - There was no match with Standard Mirrors , and the Mirror is in a Customized Series</para>
        /// </summary>
        public MirrorSeries? StandardSeriesMatched { get; set; }
        /// <summary>
        /// The Customization Series Matched to the Analized Mirror
        /// <para>If null - Then the Mirror Cannot transition into a Customized Series</para>
        /// <para>If this has a value does not automatically mean that the mirror is customized , It only means that it can transition to this series if its modified according to the Triggers</para>
        /// </summary>
        public MirrorSeries? CustomizationSeriesMatched { get; set; }

        /// <summary>
        /// The Standard Mirror matched to the analized Mirror
        /// <para>The Standard Mirror which is closer to the analized Mirror , when the analized mirror is not of a customized dimension</para>
        /// <para>Null if no match found</para>
        /// </summary>
        public MirrorSynthesis? StandardMirrorMatched { get; set; }
        [MemberNotNullWhen(false, nameof(StandardMirrorMatched))]
        public bool IsCustomizedDimension { get => StandardMirrorMatched is null; }

        /// <summary>
        /// The Mirror that has been analized
        /// </summary>
        public MirrorSynthesis AnalizedMirror { get; set; }

        public AnalizedMirrorResult(MirrorSynthesis analizedMirror)
        {
            AnalizedMirror = analizedMirror;
        }

        public AnalizedMirrorResult SetMirrorMatch(MirrorSynthesis? standardMirror)
        {
            StandardMirrorMatched = standardMirror;
            return this;
        }
        public AnalizedMirrorResult SetStandardSeriesMatch(MirrorSeries? series)
        {
            StandardSeriesMatched = series;
            return this;
        }
        public AnalizedMirrorResult SetCustomizationSeriesMatch(MirrorSeries? series)
        {
            CustomizationSeriesMatched = series;
            return this;
        }

    }
}

using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.Services.PositionService;
using ShapesLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib
{
    public class MirrorModulesList : IDeepClonable<MirrorModulesList>, IEqualityComparerCreator<MirrorModulesList>
    {
        public Dictionary<string, MirrorModule> Modules { get; set; } = [];
        /// <summary>
        /// A Dictionary of The Positionable Modules Unique Ids , Pointing to the Position Instructions used for their Positioning
        /// </summary>
        public Dictionary<string, MirrorElementPosition> PositionInstructions { get; set; } = [];

        /// <summary>
        /// Adds a module to the List
        /// </summary>
        /// <param name="module">The Module to Add</param>
        /// <param name="positionInstructions">The Position Instructions of the Module or Null if no specific instructions are Set</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public void AddModule(MirrorModule module, MirrorElementPosition? positionInstructions = null)
        {
            //Check that the same Unique Module is not Present , if it is throw something is wrong
            if (Modules.TryGetValue(module.ItemUniqueId, out MirrorModule? existingModule))
            {
                throw new Exception($"There is already a Module with the same Unique Id:{existingModule.ItemUniqueId} - Existing Module --- Code:{existingModule.Code} --- Name :{existingModule.LocalizedDescriptionInfo.Name.DefaultValue}  , Trying to Add Module --- Code: {module.Code} --- Name :{module.LocalizedDescriptionInfo.Name.DefaultValue}");
            }
            else // add the Module
            {
                Modules.Add(module.ItemUniqueId, module);
                if (module.ModuleInfo is IMirrorPositionable)
                {
                    if (PositionInstructions.TryGetValue(module.ItemUniqueId, out MirrorElementPosition? pos))
                    {
                        throw new Exception($"Unexpected Error , There was already a position present for the Module that has been just added ... Added Module{module.Code} - Found Position Code: {pos.Code}, Name:{pos.LocalizedDescriptionInfo.Name.DefaultValue}");
                    }
                    PositionInstructions.Add(module.ItemUniqueId, positionInstructions ?? MirrorElementPosition.DefaultPositionElement());
                }
            }
        }
        /// <summary>
        /// Tries to remove a certain Module from the Module List
        /// <para>Returns True on success , False on failure</para>
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool RemoveModule(MirrorModule module)
        {
            return RemoveModule(module.ItemUniqueId);
        }
        /// <summary>
        /// Removes a Module by its Unique Id , returns false if no MOdules with the specific Id where Found
        /// </summary>
        /// <param name="moduleUniqueId"></param>
        /// <returns></returns>
        public bool RemoveModule(string moduleUniqueId)
        {
            if (Modules.TryGetValue(moduleUniqueId, out MirrorModule? moduleToRemove))
            {
                var removed = Modules.Remove(moduleUniqueId);
                if (removed && moduleToRemove.ModuleInfo is IMirrorPositionable)
                {
                    PositionInstructions.Remove(moduleUniqueId);
                }
                return removed;
            };
            return false;
        }
        /// <summary>
        /// Replaces a Module with the same UniqueId 
        /// </summary>
        /// <param name="replacement">The Replacement</param>
        /// <returns>True when a matching Unique Id is found and Item is replaced , else False if not </returns>
        public bool ReplaceModuleWithSameUniqueId(MirrorModule replacement)
        {
            if (Modules.ContainsKey(replacement.ItemUniqueId))
            {
                Modules[replacement.ItemUniqueId] = replacement;
                return true;
            }
            else
            {
                return false;
            }
        }
        public MirrorModule? GetModuleByUniqueId(string moduleUniqueId)
        {
            if (Modules.TryGetValue(moduleUniqueId, out MirrorModule? module))
            {
                return module;
            }
            return null;
        }
        public IEnumerable<MirrorModule> GetAllModules()
        {
            return Modules.Values;
        }
        /// <summary>
        /// Returns the Modules of a certain <see cref="MirrorModuleInfo"/> type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<MirrorModule> ModulesOfType<T>()
            where T : MirrorModuleInfo
        {
            return Modules.Values.Where(m => m.ModuleInfo is T).ToList();
        }
        /// <summary>
        /// Returns the Mirror Modules of certain <see cref="MirrorModuleType"/>
        /// </summary>
        /// <param name="types">The Various types</param>
        /// <returns></returns>
        public List<MirrorModule> ModulesOfType(params MirrorModuleType[] types)
        {
            return Modules.Values.Where(m => types.Any(t => t == m.ModuleInfo.ModuleType)).ToList();
        }
        /// <summary>
        /// Returns the Modules of a certain Type along with their position in a single object <see cref="ModuleWithPosition"/>
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<ModuleWithPosition> ModulesOfTypeWithPosition(params MirrorModuleType[] types)
        {
            var modules = ModulesOfType(types);
            List<ModuleWithPosition> modulesWithPosition = [];
            foreach (var module in modules)
            {
                if (PositionInstructions.TryGetValue(module.ItemUniqueId, out MirrorElementPosition? position))
                {
                    modulesWithPosition.Add(new(module, position));
                }
                else modulesWithPosition.Add(new(module, MirrorElementPosition.NAPositionElement()));
            }
            return modulesWithPosition;
        }
        /// <summary>
        /// Weather it has a Modules of a certain <see cref="MirrorModuleInfo"/> type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasModuleOfType<T>()
            where T : MirrorModuleInfo
        {
            return Modules.Values.Any(m => m.ModuleInfo is T);
        }
        public bool HasModuleOfType(params MirrorModuleType[] types)
        {
            return Modules.Values.Any(m => types.Any(t => t == m.ModuleInfo.ModuleType));
        }

        /// <summary>
        /// Returns all the Modules with their Position Instructions . If an element is not positionable it has a NA Position Element
        /// </summary>
        /// <returns></returns>
        public List<ModuleWithPosition> GetAllModulesWithPosition()
        {
            List<ModuleWithPosition> modules = [];
            //Gather up all the Modules in a Single List with their instructions , If items are non positionables use a NA Position Element
            foreach (var item in Modules.Values)
            {
                if (item.ModuleInfo is IMirrorPositionable)
                {
                    PositionInstructions.TryGetValue(item.ItemUniqueId, out MirrorElementPosition? position);
                    modules.Add(new(item, position ?? MirrorElementPosition.NAPositionElement()));
                }
            }
            return modules;
        }
        /// <summary>
        /// Returns all the Modules with their Position Instructions .Including the Non Positionable Modules
        /// <para>If an element is not positionable it has a NA Position Element</para>
        /// </summary>
        /// <returns></returns>
        public List<ModuleWithPosition> GetAllModulesWithPositionIncludingNonPositionables()
        {
            var modules = GetAllModulesWithPosition();
            var nonPositionables = GetNonPositionableModules();
            return [..modules, ..nonPositionables.Select(m => new ModuleWithPosition(m, MirrorElementPosition.NAPositionElement()))];
        }
        public List<MirrorModule> GetNonPositionableModules()
        {
            return Modules.Values.Where(m => m.ModuleInfo is not IMirrorPositionable).ToList();
        }

        /// <summary>
        /// Returns the Position of a Module or null if the Module is not Present or the Position does not exist
        /// </summary>
        /// <param name="moduleUniqueId"></param>
        /// <returns></returns>
        public MirrorElementPosition? GetPositionOfModule(string moduleUniqueId)
        {
            PositionInstructions.TryGetValue(moduleUniqueId, out MirrorElementPosition? position);
            return position;
        }
        /// <summary>
        /// Returns the Position of a Module or null if the Module is not Present or the Position does not exist
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public MirrorElementPosition? GetPositionOfModule(MirrorModule? module)
        {
            return module is not null ? GetPositionOfModule(module.ItemUniqueId) : null;
        }


        public static MirrorModulesList Empty() => new();
        public MirrorModulesList GetDeepClone()
        {
            var clone = (MirrorModulesList)this.MemberwiseClone();
            clone.Modules = Modules.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetDeepClone());
            clone.PositionInstructions = PositionInstructions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetDeepClone());
            return clone;
        }
        public static IEqualityComparer<MirrorModulesList> GetComparer()
        {
            return new MirrorModulesListEqualityComparer();
        }

        public void ModifyPosition(string moduleUniqueId, MirrorElementPosition? instructions)
        {
            if (PositionInstructions.TryGetValue(moduleUniqueId, out MirrorElementPosition? positionToChange))
            {
                PositionInstructions[moduleUniqueId] = instructions ?? MirrorElementPosition.DefaultPositionElement();
            }
            else
            {
                throw new Exception($"Module id {moduleUniqueId} was not found in the Position Instructions Dictionary , The Modify Position Operation was interuppted");
            }
        }
        public override string ToString()
        {
            if (Modules.Values.Count == 0) return " - ";
            return string.Join(" , ", Modules.Values.Select(m => m.ToString()));
        }
    }
    public class MirrorModulesListEqualityComparer : IEqualityComparer<MirrorModulesList>
    {
        private readonly MirrorModuleEqualityComparer moduleComparer = new();
        private readonly MirrorElementPositionEqualityComparer positionElementComparer = new();

        public bool Equals(MirrorModulesList? x, MirrorModulesList? y)
        {
            if (ReferenceEquals(x, y))
                return true; // Both are the same instance or both are null

            if (x is null || y is null)
                return false; // One is null, the other is not

            //Check weather the Positions are equal first , if they are continue onto the rest
            bool arePositionsEqual = x.PositionInstructions.IsEqualToOtherDictionary(y.PositionInstructions, valueComparer: positionElementComparer);
            if (arePositionsEqual is false) return false;

            bool areModulesEqual = x.Modules.IsEqualToOtherDictionary(y.Modules, valueComparer: moduleComparer);
            return areModulesEqual;
        }

        public int GetHashCode([DisallowNull] MirrorModulesList obj)
        {
            throw new HashCodeNotSupportedException(this);
        }

    }

    public class ModuleWithPosition(MirrorModule module, MirrorElementPosition position)
    {
        public MirrorModule Module { get; set; } = module;
        public MirrorElementPosition Position { get; set; } = position;
    }
}

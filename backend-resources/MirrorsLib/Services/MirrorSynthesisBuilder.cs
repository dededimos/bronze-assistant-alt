using CommonHelpers.Exceptions;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Services
{
    public class MirrorSynthesisBuilder : IDisposable
    {
        public MirrorSynthesisBuilder(IMirrorsDataProvider dataProvider,
                                      MirrorCodesBuilder codesBuilder,
                                      MirrorCustomizationAnalyzer mirrorAnalizer)
        {
            _currentMirror = MirrorSynthesis.DefaultSynthesis();
            _constraints = MirrorConstraints.EmptyConstraints();
            _shapesBoundariesDirector = new();
            _shapesBoundariesDirector.SetMirror(_currentMirror);
            _positioner = new();
            this.dataProvider = dataProvider;
            this.codesBuilder = codesBuilder;
            this.mirrorAnalizer = mirrorAnalizer;
        }
        private readonly RoundedCornersModuleInfoEqualityComparer roundedCornersComparer = new();
        private readonly MirrorShapesBoundariesDirector _shapesBoundariesDirector;
        private readonly MirrorPositioner _positioner;
        private readonly IMirrorsDataProvider dataProvider;
        private readonly MirrorCodesBuilder codesBuilder;
        private readonly MirrorCustomizationAnalyzer mirrorAnalizer;
        private MirrorSynthesis _currentMirror;
        private MirrorConstraints _constraints;

        private bool generateGlassCode = true;
        private bool generateMirrorCode = true;
        private bool generateMirrorComplexCode = false;

        public MirrorSynthesis FormulatedMirror { get => _currentMirror; }
        public MirrorConstraints Constraints { get => _constraints; }


        public MirrorSynthesisBuilder ResetBuilder()
        {
            _currentMirror = MirrorSynthesis.DefaultSynthesis();
            return this;
        }
        public MirrorSynthesisBuilder GenerateGlassCode(bool generate)
        {
            generateGlassCode = generate;
            return this;
        }
        public MirrorSynthesisBuilder GenerateMirrorCode(bool generate)
        {
            generateMirrorCode = generate;
            return this;
        }
        public MirrorSynthesisBuilder GenerateMirrorComplexCode(bool generate)
        {
            generateMirrorComplexCode = generate;
            return this;
        }
        public MirrorSynthesisBuilder OverrideCode(string overriddenCode)
        {
            _currentMirror.OverriddenCode = overriddenCode;
            return this;
        }
        public MirrorSynthesisBuilder StopOverridingCode()
        {
            _currentMirror.OverriddenCode = string.Empty;
            return this;
        }


        public MirrorSynthesisBuilder SetShapeType(BronzeMirrorShape shape)
        {
            if (MirrorHelperExtensions.ToShapeInfoType(shape) != _currentMirror.DimensionsInformation.ShapeType)
            {
                //Reset everything
                ResetBuilder();
                _currentMirror.DimensionsInformation = MirrorHelperExtensions.ToShapeInfoObject(shape);
                _constraints = dataProvider.GetSpecificConstraint(_currentMirror.GeneralShapeType) ?? MirrorConstraints.EmptyConstraints();
            }
            return this;
        }
        public MirrorSynthesisBuilder SetDimensions(ShapeInfo dimensionsInformation)
        {
            if (dimensionsInformation.ShapeType != _currentMirror.DimensionsInformation.ShapeType)
            {
                ResetBuilder();
                _currentMirror.DimensionsInformation = dimensionsInformation;
                _constraints = dataProvider.GetSpecificConstraint(_currentMirror.GeneralShapeType) ?? MirrorConstraints.EmptyConstraints();
            }
            else
            {
                _currentMirror.DimensionsInformation = dimensionsInformation;
            }

            //Assign Rounded Corners Module if the Dimensions are of a Rectangle and have NonZero Radiuses
            if (_currentMirror.DimensionsInformation is RectangleInfo rect && !rect.HasZeroRadius)
            {
                var roundedCornersFromRectangleDimensions = RoundedCornersModuleInfo.CreateFromRectangle(rect);
                
                //Check weather the new Rounded Corners are already present in the Modules of this Mirror and if yes then just return
                bool alreadyPresent = _currentMirror.ModulesInfo.ModulesOfType(MirrorModuleType.RoundedCornersModuleType)
                                                                .Any(m => m.ModuleInfo is RoundedCornersModuleInfo c 
                                                                          && roundedCornersComparer.Equals(c, roundedCornersFromRectangleDimensions));
                if (alreadyPresent) return this;

                //Check weather there are any Default MATCHING Rounded Corners on the Data Provider 
                var defaultRoundedCornersModules = dataProvider.GetModulesOfType(MirrorModuleType.RoundedCornersModuleType);
                MirrorModule? roundedCornersToAdd = null;

                //If ther is a match add it to the Mirror to get also the Element Info of the Default Corners
                foreach (var roundedCornersModule in defaultRoundedCornersModules)
                {
                    //If one is found match it to the dimensions added
                    if (roundedCornersModule.ModuleInfo is RoundedCornersModuleInfo corners &&
                        roundedCornersComparer.Equals(corners,roundedCornersFromRectangleDimensions))
                    {
                        roundedCornersToAdd = roundedCornersModule;
                    }
                }
                //Else if no match was found add the created from the Rectangle Dimensions with the Custom Element Info and remove any old ones
                roundedCornersToAdd ??= new MirrorModule(MirrorElementBase.CustomRoundedCornersElementInfo(),roundedCornersFromRectangleDimensions);

                //Remove the old ones
                var modulesToRemove = _currentMirror.ModulesInfo.ModulesOfType(MirrorModuleType.RoundedCornersModuleType);
                foreach (var module in modulesToRemove)
                {
                    _currentMirror.ModulesInfo.RemoveModule(module);
                }
                //Add the new ones
                AddModule(roundedCornersToAdd, null);
            }
            //Remove Rounded Corners if the Dimensions are of a Rectangle and have Zero Radiuses
            else if ( _currentMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.RoundedCornersModuleType)
                && (_currentMirror.DimensionsInformation is RectangleInfo rect2 && rect2.HasZeroRadius 
                    || _currentMirror.DimensionsInformation is not RectangleInfo))
            {
                //Remove all Rounded Corners Modules
                var modulesToRemove = _currentMirror.ModulesInfo.ModulesOfType(MirrorModuleType.RoundedCornersModuleType);
                foreach (var module in modulesToRemove)
                {
                    _currentMirror.ModulesInfo.RemoveModule(module);
                }
            }


            return this;
        }
        public MirrorSynthesisBuilder SetGlassType(MirrorGlassType glassType)
        {
            _currentMirror.GlassType = glassType;
            return this;
        }
        public MirrorSynthesisBuilder SetGlassThickness(MirrorGlassThickness glassThickness)
        {
            _currentMirror.GlassThickness = glassThickness;
            return this;
        }

        public MirrorSynthesisBuilder SetSandblast(MirrorSandblast? sandblast)
        {
            //The Sandblast is not Formulated here (caller must order the formulation of the mirror to get the shape of the sandblast)
            _currentMirror.Sandblast = sandblast is null ? null : new(sandblast, null);
            return this;
        }
        public MirrorSynthesisBuilder SetSandblast(string sandblastId)
        {
            var sandblast = dataProvider.GetSandblasts(sandblastId).FirstOrDefault();
            return SetSandblast(sandblast);
        }
        public MirrorSynthesisBuilder RemoveSandblast() { _currentMirror.Sandblast = null; return this; }
        public bool CanHaveSandblast()
        {
            var sandblasts = dataProvider.GetAllSandblasts();
            return sandblasts.Any(s => _constraints.AllowedSandblasts.Contains(s.ElementId));
        }

        public MirrorSynthesisBuilder SetSupport(MirrorSupport? support)
        {
            //Same as Sandblast
            _currentMirror.Support = support is null ? null : new(support, null, null);
            return this;
        }
        public MirrorSynthesisBuilder SetSupport(string supportId)
        {
            var support = dataProvider.GetSupports(supportId).FirstOrDefault();
            return SetSupport(support);
        }
        public MirrorSynthesisBuilder RemoveSupport() { _currentMirror.Support = null; return this; }
        public bool CanHaveSupport()
        {
            var sandblasts = dataProvider.GetAllSupports();
            return sandblasts.Any(s => _constraints.AllowedSupports.Contains(s.ElementId));
        }

        /// <summary>
        /// Adds a Module to the Mirror , Assigns a position if one is not provided
        /// </summary>
        /// <param name="module">The Module to add</param>
        /// <param name="position">The Position of the Module , If null auto assigned</param>
        /// <returns></returns>
        public MirrorSynthesisBuilder AddModule(MirrorModule module, MirrorElementPosition? position)
        {
            MirrorElementPosition? assignedPosition = null;
            //Assign if its a positionable
            if (module.ModuleInfo is IMirrorPositionable)
            {
                //If the Position is Null automatically assign a new one
                if (position is null) assignedPosition = GetModuleAssignedPosition(module);
                else assignedPosition = position;
            }
            //Remove existing Rounded Corners if the Module is Rounded Corners and Mirror has already other Rounded Corners
            if(module.ModuleInfo.ModuleType == MirrorModuleType.RoundedCornersModuleType 
                && _currentMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.RoundedCornersModuleType))
                RemoveAllModulesOfType(MirrorModuleType.RoundedCornersModuleType);

            //Add the Module and its instructions
            _currentMirror.ModulesInfo.AddModule(module, assignedPosition);

            if (module.ModuleInfo is RoundedCornersModuleInfo roundedCorners
                && _currentMirror.DimensionsInformation is RectangleInfo rect)
            {
                rect.TopLeftRadius = roundedCorners.TopLeft;
                rect.TopRightRadius = roundedCorners.TopRight;
                rect.BottomLeftRadius = roundedCorners.BottomLeft;
                rect.BottomRightRadius = roundedCorners.BottomRight;
            }

            return this;
        }
        public MirrorSynthesisBuilder AddModule(string moduleId)
        {
            var module = dataProvider.GetModule(moduleId);
            if (module == null) return this;
            return AddModule(module, null);
        }


        /// <summary>
        /// Modifys the module with the Same UniqueId as the Argument's module
        /// </summary>
        /// <param name="modifiedModule"></param>
        /// <returns></returns>
        /// <exception cref="Exception">When the modified Module's Unique id does not have a match with any of the Modules of the Current Mirror</exception>
        public MirrorSynthesisBuilder ModifyModuleWithSameUniqueId(MirrorModule modifiedModule)
        {
            var replaced = _currentMirror.ModulesInfo.ReplaceModuleWithSameUniqueId(modifiedModule);
            if (!replaced)
            {
                throw new Exception($"Could not use {modifiedModule.Code}-{modifiedModule.LocalizedDescriptionInfo.Name.DefaultValue} as replacement , there was no Matching Id {modifiedModule.ItemUniqueId} found in the Current Mirrors Module List ");
            }
            else
            {
                //If the Module was replaced and it was a Rounded Corners Module , then also update the Dimensions
                if (modifiedModule.ModuleInfo is RoundedCornersModuleInfo corners && _currentMirror.DimensionsInformation is RectangleInfo rect)
                {
                    rect.TopLeftRadius = corners.TopLeft;
                    rect.TopRightRadius = corners.TopRight;
                    rect.BottomLeftRadius = corners.BottomLeft;
                    rect.BottomRightRadius = corners.BottomRight;
                }
                return this;
            }
        }
        /// <summary>
        /// Ammends the Position of an already Positioned Module
        /// </summary>
        /// <param name="module">The Module that has been positioned</param>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public MirrorSynthesisBuilder ModifyModulePosition(string positionedModuleUniqueId, MirrorElementPosition? position)
        {
            _currentMirror.ModulesInfo.ModifyPosition(positionedModuleUniqueId, position);
            return this;
        }
        /// <summary>
        /// Removes a Module from the Mirror
        /// </summary>
        /// <param name="itemUniqueId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public MirrorSynthesisBuilder RemoveModule(string itemUniqueId)
        {
            bool removed = _currentMirror.ModulesInfo.RemoveModule(itemUniqueId);

            //Remove any Corner Radius if there are no Rounded Corners in the Modules after the Removal
            if (_currentMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.RoundedCornersModuleType) == false
                && _currentMirror.DimensionsInformation is RectangleInfo rect) rect.SetCornerRadius(0);

            if (!removed) throw new Exception($"The Modules Remove Operation has Failed...");
            return this;
        }
        /// <summary>
        /// Returns the Selectable Modules of the specified Type for this Mirror 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<MirrorModule> GetSelectableModules(params MirrorModuleType[] types)
        {
            //Return only the Allowable Modules
            return dataProvider.GetModulesOfType(types).Where(module => _constraints.AllowedModules.Contains(module.ElementId)).OrderBy(m => m.ModuleInfo.ModuleType);
        }
        /// <summary>
        /// Removes all the Modules of the selected type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MirrorSynthesisBuilder RemoveAllModulesOfType(MirrorModuleType type)
        {
            var modulesToRemove = _currentMirror.ModulesInfo.ModulesOfType(type);
            foreach (var module in modulesToRemove)
            {
                _currentMirror.ModulesInfo.RemoveModule(module);
            }

            //Remove the Rounded Corners from the Dimensions Also if the Type is Rounded Corners
            if (type == MirrorModuleType.RoundedCornersModuleType && _currentMirror.DimensionsInformation is RectangleInfo rect && !rect.HasZeroRadius)
            {
                rect.SetCornerRadius(0);
            }

            return this;
        }
        /// <summary>
        /// Removes all the Modules
        /// </summary>
        /// <returns></returns>
        public MirrorSynthesisBuilder RemoveAllModules()
        {
            _currentMirror.ModulesInfo.Modules.Clear();
            return this;
        }


        /// <summary>
        /// Adds the Default Module for this Mirror (The First Allowed in the Modules dictated by its Constraints)
        /// <para>At the Default position</para>
        /// </summary>
        /// <exception cref="Exception"></exception>
        public MirrorSynthesisBuilder AddDefaultModule(MirrorModuleType moduleType)
        {
            var allowedModules = dataProvider.GetModules([.. _constraints.AllowedModules]);
            var defaultModule = allowedModules.FirstOrDefault(m => m.ModuleInfo.ModuleType == moduleType);
            if (defaultModule != null)
            {
                var clone = defaultModule.GetDeepClone();
                clone.AssignNewUniqueId();
                AddModule(clone, null);
            }
            else throw new Exception($"Constraints of Mirror Shape: {_currentMirror.GeneralShapeType} do not allow Modules of Type :{moduleType}");

            //Assign the Rounded Corners also to the Dimensions if the Module is Rounded Corners
            if (moduleType == MirrorModuleType.RoundedCornersModuleType 
                && defaultModule.ModuleInfo is RoundedCornersModuleInfo corners
                && _currentMirror.DimensionsInformation is RectangleInfo rect)
            {
                rect.TopLeftRadius = corners.TopLeft;
                rect.TopRightRadius = corners.TopRight;
                rect.BottomLeftRadius = corners.BottomLeft;
                rect.BottomRightRadius = corners.BottomRight;
            }

            return this;
        }
        public bool CanHaveModuleOfType(MirrorModuleType type)
        {
            var modules = dataProvider.GetModulesOfType(type);
            return modules.Any(m => _constraints.AllowedModules.Contains(m.ElementId));
        }

        public MirrorSynthesis FormulateMirror()
        {
            //Formulate the Mirror with the Current Setup and return it. 
            _shapesBoundariesDirector.SetMirror(_currentMirror);
            _shapesBoundariesDirector.CreateMirrorShapesAndBoundaries();

            ShapeInfo boundary;
            switch (_currentMirror.ModulesBoundary)
            {
                case MirrorBoundaryOption.BoundaryFormingFromMirrorGlass:
                    boundary = _currentMirror.MirrorGlassShape;
                    break;
                case MirrorBoundaryOption.BoundaryFormingFromSandblast:
                    if (_currentMirror.Sandblast is null) throw new Exception($"Unexpected Error , Sandblast is not Present in the Mirror when its {nameof(MirrorPlacedSandblast.FormedBoundary)} is Needed as the Modules Boundary");
                    boundary = _currentMirror.Sandblast.FormedBoundary
                        ?? throw new Exception($"The Sandblast Boundary has not been Formed , Modules could not be Positioned");
                    break;
                case MirrorBoundaryOption.BoundaryFormingFromSupport:
                    if (_currentMirror.Support is null) throw new Exception($"Unexpected Error , Support is not Present in the Mirror when its {nameof(MirrorPlacedSupport.FormedBoundary)} is Needed as the Modules Boundary");
                    boundary = _currentMirror.Support.FormedBoundary
                        ?? throw new Exception($"The Suport Boundary has not been Formed, Modules could not be Positioned");
                    break;
                default:
                    throw new EnumValueNotSupportedException(_currentMirror.ModulesBoundary);
            }
            var positionableModules = _currentMirror.ModulesInfo.GetAllModulesWithPosition();
            if (positionableModules.Count != 0) _positioner.PositionModules(positionableModules, boundary);

            //Assign Codes and Series
            var analizeResult = mirrorAnalizer.AnalizeMirror(_currentMirror);
            if (analizeResult.IsCustomizedDimension)
            {
                _currentMirror.SeriesReferenceId = analizeResult.CustomizationSeriesMatched?.ElementId ?? MirrorSeries.UndefinedSeries().ElementId;
            }
            else
            {
                _currentMirror.SeriesReferenceId = analizeResult.StandardSeriesMatched?.ElementId ?? MirrorSeries.UndefinedSeries().ElementId;
            }

            if (generateGlassCode)
            {
                _currentMirror.GlassCode = codesBuilder.GenerateGlassCode(_currentMirror);
            }
            if (generateMirrorCode)
            {
                _currentMirror.ConstructedCode = analizeResult.IsCustomizedDimension
                    ? codesBuilder.GenerateMirrorCode(_currentMirror)
                    : analizeResult.StandardMirrorMatched.Code;
            }
            if (generateMirrorComplexCode)
            {
                _currentMirror.ComplexCode = codesBuilder.GenerateMirrorCode(_currentMirror, true);
            }

            return _currentMirror;
        }
        public void SetFormulatedMirror(MirrorSynthesis mirror) => _currentMirror = mirror;

        /// <summary>
        /// Assigns a position to the Module based on the Shape of the mirror and the already positioned Modules
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        private MirrorElementPosition GetModuleAssignedPosition(MirrorModule module)
        {
            if (module.ModuleInfo is not IMirrorPositionable) throw new ArgumentException($"Provided {nameof(MirrorModule)} does not implement {nameof(IMirrorPositionable)}");

            //Get the All the PositionOptions for this Element-Module
            var posOptions = dataProvider.GetPositionOptionsOfElement(module.ElementId);
            //Assign the default position for this element. If there is none , assign the DefaultPositionElement
            var assignedPosition = posOptions.GetDefaultPositionElement(_currentMirror.ShapeType) ?? MirrorElementPosition.DefaultPositionElement();
            if (module.ModuleInfo.ModuleType is MirrorModuleType.MagnifierModuleType or MirrorModuleType.MagnifierSandblastedModuleType
                && _currentMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.MagnifierSandblastedModuleType, MirrorModuleType.MagnifierModuleType))
            {
                //Get the Already placed Magnifiers if any 
                var currentMagnifiers = _currentMirror.ModulesInfo.ModulesOfTypeWithPosition(MirrorModuleType.MagnifierSandblastedModuleType, MirrorModuleType.MagnifierModuleType);
                var allAvailablePositions = posOptions.GetAllAvailablePositionElements(_currentMirror.ShapeType);
                var allAvailablePositionsNotSet = allAvailablePositions.Where(p => currentMagnifiers.Select(m => m.Position.ElementId).All(id => id != p.ElementId));
                if (allAvailablePositionsNotSet.Any(p => p.ElementId == assignedPosition.ElementId) is false) //meaning the assigned position is already in use
                {
                    assignedPosition = allAvailablePositionsNotSet.FirstOrDefault() ?? MirrorElementPosition.DefaultPositionElement();
                }
            }

            return assignedPosition;
        }
        private class MirrorPositioner
        {
            private Dictionary<string, int> ModulesPositioningOrder { get; } = new()
        {
            { typeof(MagnifierSandblastedModuleInfo).Name ,1},
            { typeof(MagnifierModuleInfo).Name ,2},
            { typeof(TouchButtonModuleInfo).Name ,3},
            { typeof(ScreenModuleInfo).Name ,4},
            { typeof(BluetoothModuleInfo).Name ,5},
            { typeof(ResistancePadModuleInfo).Name ,6},
            { typeof(MirrorLampModuleInfo).Name ,7},
            { typeof(TransformerModuleInfo).Name ,8},
            { typeof(MirrorBackLidModuleInfo).Name,9 },
        };

            /// <summary>
            /// Sets the Order in which Modules will be Positioned in the Mirror
            /// </summary>
            /// <param name="type"></param>
            /// <param name="order"></param>
            /// <exception cref="Exception"></exception>
            /// <exception cref="ArgumentException"></exception>
            public void SetPositionOrderGravity(Type type, int order)
            {
                if (type.GetInterfaces().Contains(typeof(IMirrorPositionable)))
                {
                    if (!ModulesPositioningOrder.TryAdd(type.Name, order))
                    {
                        ModulesPositioningOrder[type.Name] = order;
                    }
                }
                else throw new ArgumentException($"{type.Name} does not Implement {nameof(IMirrorPositionable)} and cannot be added to the Position Order Gravity List");
            }

            /// <summary>
            /// Positions Modules in order based on the OrderNo they have .
            /// <para>Types with small OrderNo are positioned First</para>
            /// <para>Types with the same OrderNo will be positioned in a first Come first Served bases (whichever is first in the provided List of Modules)</para>
            /// <para>Types not present in the OrderNo Dictionary will not be positioned and will be placed at the end</para>
            /// <para>Positionable Modules in the OrderNo Dictionary with Null Instructions will be placed in the center of the boundary</para>
            /// </summary>
            /// <param name="modulesWithPosition">The Modules along with their position Instructions</param>
            /// <param name="boundary">The Boundary within which to position the Modules</param>
            public void PositionModules(IEnumerable<ModuleWithPosition> modulesWithPosition, ShapeInfo boundary)
            {
                // Order the Positionables based on the Gravity No of each Type 
                // If a Type is not found in the dictionary, place it at the end of the List
                var orderedmodulesWithPosition = modulesWithPosition
                    .OrderBy(helper =>
                        ModulesPositioningOrder.TryGetValue(helper.Module.ModuleInfo.GetType().Name, out int gravity)
                        ? gravity
                        : int.MaxValue)
                    .ToList();

                // Now you can iterate through orderedHelpers in the correct order
                foreach (var moduleHelper in orderedmodulesWithPosition)
                {
                    var moduleInfo = moduleHelper.Module.ModuleInfo;
                    if (moduleInfo is IMirrorPositionable positionable)
                    {
                        //If the Instructions are null then place it at the center
                        var position = moduleHelper.Position ?? MirrorElementPosition.DefaultPositionElement();
                        PositionSingleModule(positionable, position.Instructions, boundary);
                    }
                    else break; //Break the loop once a non positionable is encountered (all non positionables or not Recognized types will be at the end of the List)
                }
            }

            /// <summary>
            /// Positions an item within a boundary based on the Provided Instructions
            /// </summary>
            /// <param name="positionableModule"></param>
            /// <param name="instructions"></param>
            /// <param name="boundary"></param>
            public static void PositionSingleModule(IMirrorPositionable positionableModule, IPositionInstructions instructions, ShapeInfo boundary)
            {
                var position = instructions.GetPosition(boundary);
                positionableModule.SetPosition(position, boundary.GetCentroid());
            }

        }

        public void Dispose()
        {
            Dispose(true);
            //item already disposed above code , supress finalize
            GC.SuppressFinalize(this);
        }
        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                //Nothing to Dispose
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }
}

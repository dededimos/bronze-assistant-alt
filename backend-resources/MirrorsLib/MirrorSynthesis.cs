using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MirrorsLib.Helpers.MirrorHelperExtensions;

namespace MirrorsLib
{
    public class MirrorSynthesis : ICodeable,IPowerable, IDeepClonable<MirrorSynthesis>
    {
        public string Code 
        {
            get => string.IsNullOrWhiteSpace(OverriddenCode)
                ? (string.IsNullOrWhiteSpace(ConstructedCode)
                    ? "Undefined-Code" : ConstructedCode )
                : OverriddenCode;   
        }
        /// <summary>
        /// The Code constructed for the Mirror
        /// </summary>
        public string ConstructedCode { get; set; } = string.Empty;
        /// <summary>
        /// A set code by the user or Developer overriding any constructed code
        /// </summary>
        public string OverriddenCode { get; set; } = string.Empty;
        /// <summary>
        /// The Code of the Glass used in the Mirror
        /// </summary>
        public string GlassCode { get; set; } = "Undefined-Code";
        /// <summary>
        /// The Code String Used for Parsing the mirror
        /// Contains all information about the Mirror in a single String
        /// </summary>
        public string ComplexCode { get; set; } = "Undefined-ComplexCode";

        public MirrorOrientedShape ShapeType { get => DimensionsInformation.ToMirrorOrientedShape(); }
        public BronzeMirrorShape GeneralShapeType { get => ShapeType.ToBronzeMirrorShape(); }
        public ShapeInfo DimensionsInformation { get; set; } = ShapeInfo.Undefined();

        public ShapeInfo MirrorGlassShape { get; set; } = ShapeInfo.Undefined();

        public MirrorGlassType GlassType { get; set; } = MirrorGlassType.AGCMirror;
        public MirrorGlassThickness GlassThickness { get; set; } = MirrorGlassThickness.MirrorThickness5mm;
        /// <summary>
        /// The Boundary of the Sandblast
        /// </summary>
        public MirrorBoundaryOption SandblastBoundary { get; set; }
        /// <summary>
        /// The Boundary of the Support
        /// </summary>
        public MirrorBoundaryOption SupportBoundary { get; set; } 
        /// <summary>
        /// The Boundary of the 
        /// </summary>
        public MirrorBoundaryOption ModulesBoundary { get; set; } 
        public MirrorPlacedSandblast? Sandblast { get; set; }
        public MirrorPlacedSupport? Support { get; set; }
        public MirrorModulesList ModulesInfo { get; set; } = MirrorModulesList.Empty();
        public List<MirrorLight> Lights { get; set; } = [];
        public List<CustomMirrorElement> CustomElements { get; set; } = [];
        public string SeriesReferenceId { get; set; } = string.Empty;
        public MirrorSynthesis GetDeepClone()
        {
            var clone = (MirrorSynthesis)this.MemberwiseClone();
            clone.DimensionsInformation = this.DimensionsInformation.GetDeepClone();
            clone.Sandblast = this.Sandblast?.GetDeepClone();
            clone.Support = this.Support?.GetDeepClone();
            clone.ModulesInfo = this.ModulesInfo.GetDeepClone();
            clone.CustomElements = this.CustomElements.GetDeepClonedList();
            return clone;
        }

        public bool HasVisibleFrame()
        {
            return Support?.SupportInfo.SupportType == MirrorSupportType.MirrorVisibleFrameSupport;
        }

        public RectangleInfo GetMirrorGlassSideView()
        {
            ShapeInfo glass;
            if (MirrorGlassShape == null) glass = GetGlassShape();
            else glass = MirrorGlassShape;

            var bBox = glass.GetBoundingBox();
            double depth = GlassThickness switch
            {
                MirrorGlassThickness.MirrorThickness5mm => 5d,
                MirrorGlassThickness.MirrorThickness6mm => 6d,
                _ => throw new EnumValueNotSupportedException(GlassThickness)
            };

            return new RectangleInfo(depth, bBox.Height, 0, 0, 0);
        }
        public ShapeInfo GetGlassShape()
        {
            ShapeInfo glassShapeInfo;
            if (Support?.SupportInfo is MirrorVisibleFrameSupport frame)
            {
                glassShapeInfo = DimensionsInformation.GetReducedPerimeterClone(frame.GetGlassShrink(), true);
            }
            else glassShapeInfo = DimensionsInformation;
            return glassShapeInfo;
        }
        public MirrorGlass GetGlassObject()
        {
            MirrorGlass glass = new()
            {
                Code = this.GlassCode,
                DimensionsInformation = this.MirrorGlassShape.GetDeepClone(),
                GlassType = this.GlassType,
                Thickness = this.GlassThickness,
                Processes = this.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType).Select(m => ((MirrorProcessModuleInfo)m.ModuleInfo).ProcessShape.GetDeepClone()).ToList(),
                Sandblasts = this.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType).Select(m => ((MagnifierSandblastedModuleInfo)m.ModuleInfo).SandblastDimensions).Cast<ShapeInfo>().ToList(),
            };
            if (this.Sandblast?.SandblastShape is not null)
            {
                glass.Sandblasts.Add(this.Sandblast.SandblastShape.GetDeepClone());
            }
            return glass;
        }

        public static MirrorSynthesis DefaultSynthesis() => new();

        public double GetTransformerNominalPower()
        {
            var modulesPower = ModulesInfo.GetAllModules().OfType<IPowerable>().Sum(p => p.GetTransformerNominalPower());
            var lightsPower = Lights.Sum(l => l.GetTransformerNominalPower());
            return modulesPower + lightsPower;
        }

        public double GetEnergyConsumption()
        {
            var modulesPower = ModulesInfo.GetAllModules().OfType<IPowerable>().Sum(p => p.GetEnergyConsumption());
            var lightsPower = Lights.Sum(l => l.GetEnergyConsumption());
            return modulesPower;
        }
    }
    public class MirrorSynthesisEqualityComparer : IEqualityComparer<MirrorSynthesis>
    {
        private readonly ShapeInfoEqualityComparer shapeComparer = new(true);
        private readonly MirrorPlacedSandblastEqualityComparer sandblastComparer = new();
        private readonly MirrorPlacedSupportEqualityComparer supportComparer = new();
        private readonly MirrorModulesListEqualityComparer modulesComparer = new();
        private readonly MirrorLightEqualityComparer lightComparer = new();
        private readonly CustomMirrorElementEqualityComparer customElementComparer = new();

        public bool Equals(MirrorSynthesis? x, MirrorSynthesis? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Code == y.Code 
                && x.GlassCode == y.GlassCode 
                && x.ComplexCode == y.ComplexCode 
                && shapeComparer.Equals(x.DimensionsInformation,y.DimensionsInformation) 
                && shapeComparer.Equals(x.MirrorGlassShape,y.MirrorGlassShape) 
                && x.GlassThickness == y.GlassThickness 
                && x.GlassType == y.GlassType 
                && x.SandblastBoundary == y.SandblastBoundary 
                && x.SupportBoundary == y.SupportBoundary 
                && x.ModulesBoundary == y.ModulesBoundary 
                && sandblastComparer.Equals(x.Sandblast,y.Sandblast) 
                && supportComparer.Equals(x.Support , y.Support) 
                && modulesComparer.Equals(x.ModulesInfo,y.ModulesInfo) 
                && x.Lights.SequenceEqual(y.Lights,lightComparer) 
                && x.CustomElements.SequenceEqual(y.CustomElements,customElementComparer) 
                && x.SeriesReferenceId == y.SeriesReferenceId;
        }

        public int GetHashCode([DisallowNull] MirrorSynthesis obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    /// <summary>
    /// A Mirror synthesis Comparer to compare wheather two Synthesis have the same Glass 
    /// </summary>
    public class MirrorSynthesisHasEqualGlassComparer : IEqualityComparer<MirrorSynthesis>
    {
        private readonly ShapeInfoEqualityComparer shapeComparer = new(false);
        private readonly MirrorPlacedSandblastEqualityComparer sandblastComparer = new();
        public bool Equals(MirrorSynthesis? x, MirrorSynthesis? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            bool areEqual = x.GlassCode == y.GlassCode
                && x.GlassThickness == y.GlassThickness
                && x.GlassType == y.GlassType
                && shapeComparer.Equals(x.MirrorGlassShape, y.MirrorGlassShape)
                && sandblastComparer.Equals(x.Sandblast, y.Sandblast);

            //We further need to check weather the Two Mirrors have the same Sandblasted Magnifiers and Processes
            //Order of the Modules is important , so order them by Locations as two modules will never be in the same area in the same mirror
            bool areEqualMagnifiersSandblasted = 
                x.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType)
                .Select(m => ((MagnifierSandblastedModuleInfo)m.ModuleInfo).SandblastDimensions)
                .OrderBy(d => d.LocationX)
                .ThenBy(d => d.LocationY)
                .SequenceEqual(
                    y.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType)
                    .Select(m => ((MagnifierSandblastedModuleInfo)m.ModuleInfo).SandblastDimensions)
                    .OrderBy(d=>d.LocationX)
                    .ThenBy(d=>d.LocationY), shapeComparer);

            bool areEqualProcesses =
                x.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType)
                .Select(m => ((MirrorProcessModuleInfo)m.ModuleInfo).ProcessShape)
                .OrderBy(d => d.LocationX)
                .ThenBy(d => d.LocationY)
                .SequenceEqual(
                    y.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType)
                    .Select(m => ((MirrorProcessModuleInfo)m.ModuleInfo).ProcessShape)
                    .OrderBy(d => d.LocationX)
                    .ThenBy(d => d.LocationY)
                    , shapeComparer);

            return areEqual && areEqualMagnifiersSandblasted && areEqualProcesses;
        }

        public int GetHashCode([DisallowNull] MirrorSynthesis obj)
        {
            int hash = HashCode.Combine(obj.GlassCode,
                                        obj.GlassThickness,
                                        obj.GlassType,
                                        shapeComparer.GetHashCode(obj.MirrorGlassShape),
                                        obj.Sandblast is null ? 41 : sandblastComparer.GetHashCode(obj.Sandblast));

            var magnifiers = obj.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType).Select(m => ((MagnifierSandblastedModuleInfo)m.ModuleInfo).SandblastDimensions);
            foreach (var magn in magnifiers)
            {
                unchecked
                {
                    hash = HashCode.Combine(hash, shapeComparer.GetHashCode(magn));
                }
            }
            var processes = obj.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType).Select(m => ((MirrorProcessModuleInfo)m.ModuleInfo).ProcessShape);
            foreach (var proc in processes)
            {
                unchecked
                {
                    hash = HashCode.Combine(hash, shapeComparer.GetHashCode(proc));
                }
            }
            return hash;
        }
    }

}

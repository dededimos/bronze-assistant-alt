using CommonInterfacesBronze;
using MirrorsLib.Enums;
using static MirrorsLib.Helpers.MirrorHelperExtensions;
using MirrorsLib.MirrorElements;
using MongoDbCommonLibrary.CommonEntities;
using ShapesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib;
using MirrorsLib.Repositories;
using CommonHelpers.Exceptions;
using System.Diagnostics.CodeAnalysis;
using CommonHelpers;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorSynthesisEntity : MongoDatabaseEntity , IDeepClonable<MirrorSynthesisEntity> ,IEqualityComparerCreator<MirrorSynthesisEntity>
    {
        public MirrorSynthesisEntity()
        {
            
        }

        public static MirrorSynthesisEntity CreateFromModel(MirrorSynthesis synthesis)
        {
            MirrorSynthesisEntity entity = new()
            {
                Code = synthesis.Code,
                ConstructedCode = synthesis.ConstructedCode,
                OverriddenCode = synthesis.OverriddenCode,
                GlassCode = synthesis.GlassCode,
                ComplexCode = synthesis.ComplexCode,
                DimensionsInformation = synthesis.DimensionsInformation.GetDeepClone(),
                MirrorGlassShape = synthesis.MirrorGlassShape.GetDeepClone(),
                GlassType = synthesis.GlassType,
                GlassThickness = synthesis.GlassThickness,
                SandblastBoundary = synthesis.SandblastBoundary,
                SupportBoundary = synthesis.SupportBoundary,
                ModulesBoundary = synthesis.ModulesBoundary,
                Sandblast = MirrorPlacedSandblastDTO.Create(synthesis.Sandblast),
                Support = MirrorPlacedSupportDTO.Create(synthesis.Support),
                Modules = synthesis.ModulesInfo.GetAllModules().Select(m => new MirrorPlacedModuleDTO(m)).ToList(),
                ModulesPositions = synthesis.ModulesInfo.PositionInstructions.ToDictionary(kvp => kvp.Key, kvp => new MirrorPlacedElementPositionDTO(kvp.Value)),
                CustomElements = synthesis.CustomElements.Select(ce => new PlacedCustomMirrorElementDTO(ce)).ToList(),
                SeriesReferenceId = synthesis.SeriesReferenceId,
            };
            return entity;
        }

        public string Code { get; set; } = string.Empty;
        public string ConstructedCode { get; set; } = string.Empty;
        public string OverriddenCode { get; set; } = string.Empty;
        public string GlassCode { get; set; } = string.Empty;
        public string ComplexCode { get; set; } = string.Empty;
        public MirrorOrientedShape ShapeType { get => DimensionsInformation.ToMirrorOrientedShape(); }
        public BronzeMirrorShape GeneralShapeType { get => ShapeType.ToBronzeMirrorShape(); }
        public ShapeInfo DimensionsInformation { get; set; } = ShapeInfo.Undefined();
        public ShapeInfo MirrorGlassShape { get; set; } = ShapeInfo.Undefined();
        public MirrorGlassType GlassType { get; set; }
        public MirrorGlassThickness GlassThickness { get; set; }
        public MirrorBoundaryOption SandblastBoundary { get; set; }
        public MirrorBoundaryOption SupportBoundary { get; set; }
        public MirrorBoundaryOption ModulesBoundary { get; set; }
        public MirrorPlacedSandblastDTO? Sandblast { get; set; }
        public MirrorPlacedSupportDTO? Support { get; set; }
        public List<MirrorPlacedLightDTO> Lights { get; set; } = [];
        public List<MirrorPlacedModuleDTO> Modules { get; set; } = [];
        public Dictionary<string, MirrorPlacedElementPositionDTO> ModulesPositions { get; set; } = [];
        public List<PlacedCustomMirrorElementDTO> CustomElements { get; set; } = [];
        public string SeriesReferenceId { get; set; } = string.Empty;

        public override MirrorSynthesisEntity GetDeepClone()
        {
            var clone = (MirrorSynthesisEntity)this.MemberwiseClone();
            clone.DimensionsInformation = this.DimensionsInformation.GetDeepClone();
            clone.MirrorGlassShape = this.MirrorGlassShape.GetDeepClone();
            Sandblast = this.Sandblast?.GetDeepClone();
            Support = this.Support?.GetDeepClone();
            Lights = this.Lights.GetDeepClonedList();
            Modules = this.Modules.GetDeepClonedList();
            ModulesPositions = this.ModulesPositions.ToDictionary(kvp=> kvp.Key, kvp => kvp.Value.GetDeepClone());
            CustomElements = this.CustomElements.GetDeepClonedList();
            return clone;
        }

        public MirrorSynthesis ToMirrorSynthesis(IMirrorsDataProvider dataProvider)
        {
            MirrorSynthesis synthesis = new()
            {
                ConstructedCode = this.ConstructedCode,
                OverriddenCode = this.OverriddenCode,
                GlassCode = this.GlassCode,
                ComplexCode = this.ComplexCode,
                DimensionsInformation = this.DimensionsInformation.GetDeepClone(),
                MirrorGlassShape = this.MirrorGlassShape.GetDeepClone(),
                GlassType = this.GlassType,
                GlassThickness = this.GlassThickness,
                SandblastBoundary = this.SandblastBoundary,
                SupportBoundary = this.SupportBoundary,
                ModulesBoundary = this.ModulesBoundary,
                Sandblast = this.Sandblast?.ToPlacedSandblast(dataProvider),
                Support = this.Support?.ToPlacedSupport(dataProvider),
                Lights = this.Lights.Select(l=> l.ToMirrorLight(dataProvider)).ToList(),
                ModulesInfo = new() { Modules = this.Modules.ToDictionary(m=> m.UniqueId,m=> m.ToMirrorModule(dataProvider)),PositionInstructions = ModulesPositions.ToDictionary(mp=> mp.Key,mp=> mp.Value.ToMirrorElementPosition(dataProvider))},
                CustomElements = this.CustomElements.Select(c=> c.ToCustomMirrorElement(dataProvider)).ToList(),
                SeriesReferenceId = this.SeriesReferenceId,
            };
            return synthesis;
        }
        static IEqualityComparer<MirrorSynthesisEntity> IEqualityComparerCreator<MirrorSynthesisEntity>.GetComparer()
        {
            return new MirrorSynthesisEntityEqualityComparer();
        }
    }
    public class MirrorSynthesisEntityEqualityComparer : IEqualityComparer<MirrorSynthesisEntity>
    {
        private readonly ShapeInfoEqualityComparer shapeComparer = new(false);
        private readonly MirrorPlacedSandblastDTOEqualityComparer sandblastComparer = new();
        private readonly MirrorPlacedSupportDTOEqualityComparer supportComparer = new();
        private readonly MirrorPlacedLightDTOEqualityComparer lightComparer = new();
        private readonly MirrorPlacedModuleDTOEqualityComparer moduleComparer = new();
        private readonly MirrorPlacedElementPositionDTOEqualityComparer positionComparer = new();
        private readonly PlacedCustomMirrorElementDTOEqualityComparer customElementComparer = new();

        public bool Equals(MirrorSynthesisEntity? x, MirrorSynthesisEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Code == y.Code
                && x.SeriesReferenceId == y.SeriesReferenceId
                && x.ConstructedCode == y.ConstructedCode
                && x.OverriddenCode == y.OverriddenCode
                && x.GlassCode == y.GlassCode
                && x.ComplexCode == y.ComplexCode
                && x.GlassType == y.GlassType
                && x.GlassThickness == y.GlassThickness
                && x.SupportBoundary == y.SupportBoundary
                && x.ModulesBoundary == y.ModulesBoundary
                && x.SandblastBoundary == y.SandblastBoundary
                && x.Notes == y.Notes
                && shapeComparer.Equals(x.DimensionsInformation,y.DimensionsInformation)
                && shapeComparer.Equals(x.MirrorGlassShape,y.MirrorGlassShape)
                && sandblastComparer.Equals(x.Sandblast,y.Sandblast)
                && supportComparer.Equals(x.Support,y.Support)
                && x.Lights.SequenceEqual(y.Lights,lightComparer)
                && x.Modules.SequenceEqual(y.Modules,moduleComparer)
                && x.ModulesPositions.IsEqualToOtherDictionary(y.ModulesPositions,null,positionComparer)
                && x.CustomElements.SequenceEqual(y.CustomElements,customElementComparer);

        }

        public int GetHashCode([DisallowNull] MirrorSynthesisEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

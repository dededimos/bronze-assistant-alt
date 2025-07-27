using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Services.PositionService;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonInterfaces;
using ShapesLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Entities
{
    public interface IMirrorElementEntity : IDatabaseEntity, ILocalizedDescription, ICodeable, IDeepClonable<IMirrorElementEntity>
    {
        public string PhotoUrl { get; set; }
        public string PhotoUrl2 { get; set; }
        public string IconUrl { get; set; }
        public bool IsDefaultElement { get; set; }
        public MirrorElementBase GetMirrorElementInfo();
    }

    public class MirrorElementEntity : MongoDatabaseEntity, IMirrorElementEntity, IDeepClonable<MirrorElementEntity>, IEqualityComparerCreator<MirrorElementEntity>
    {
        public string Code { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string MinimalCode { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string PhotoUrl2 { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public bool IsDefaultElement { get; set; }
        public LocalizedDescription LocalizedDescriptionInfo { get; set; } = LocalizedDescription.Empty();

        public override MirrorElementEntity GetDeepClone()
        {
            var clone = (MirrorElementEntity)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            CopyBaseEntityProperties(clone);
            return clone;
        }

        protected override void CopyBaseEntityProperties(MongoDatabaseEntity entity)
        {
            base.CopyBaseEntityProperties(entity);
            //When using this method , the item will ALWAYS be A Mirror ElementEntity as the method is protected
            var mirrorElementEntity = (MirrorElementEntity)entity;
            mirrorElementEntity.Code = this.Code;
            mirrorElementEntity.ShortCode = this.ShortCode;
            mirrorElementEntity.MinimalCode = this.MinimalCode;
            mirrorElementEntity.PhotoUrl = this.PhotoUrl;
            mirrorElementEntity.IsDefaultElement = this.IsDefaultElement;
            mirrorElementEntity.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
        }

        public MirrorElementBase GetMirrorElementInfo()
        {
            return new MirrorElementBase()
            {
                Code = Code,
                ShortCode = this.ShortCode,
                MinimalCode = this.MinimalCode,
                PhotoUrl = PhotoUrl,
                PhotoUrl2 = this.PhotoUrl2,
                IconUrl = this.IconUrl,
                IsOverriddenElement = !IsDefaultElement,
                LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone(),
                ElementId = this.Id
            };
        }


        IMirrorElementEntity IDeepClonable<IMirrorElementEntity>.GetDeepClone()
        {
            return GetDeepClone();
        }

        static IEqualityComparer<MirrorElementEntity> IEqualityComparerCreator<MirrorElementEntity>.GetComparer()
        {
            return new MirrorElementEntityEqualityComparer();
        }
    }
    public class MirrorElementEntityEqualityComparer : IEqualityComparer<MirrorElementEntity>
    {
        public bool Equals(MirrorElementEntity? x, MirrorElementEntity? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case MirrorSandblastEntity sandblast:
                    var sandblastComparer = new MirrorSandblastEntityEqualityComparer();
                    return sandblastComparer.Equals(sandblast, (MirrorSandblastEntity)y);
                case MirrorSupportEntity support:
                    var supportComparer = new MirrorSupportEntityEqualityComparer();
                    return supportComparer.Equals(support, (MirrorSupportEntity)y);
                case MirrorLightElementEntity light:
                    var lightComparer = new MirrorLightElementEntityEqualityComparer();
                    return lightComparer.Equals(light, (MirrorLightElementEntity)y);
                case MirrorSeriesElementEntity series:
                    var seriesComparer = new MirrorSeriesElementEntityEqualityComparer();
                    return seriesComparer.Equals(series, (MirrorSeriesElementEntity)y);
                case MirrorElementPositionEntity position:
                    var positionComparer = new MirrorElementPositionEntityEqualityComparer();
                    return positionComparer.Equals(position, (MirrorElementPositionEntity)y);
                case MirrorModuleEntity module:
                    var moduleComparer = new MirrorModuleEntityEqualityComparer();
                    return moduleComparer.Equals(module, (MirrorModuleEntity)y);
                case CustomMirrorElementEntity customElement:
                    var customElementComparer = new CustomMirrorElementEntityEqualityComparer();
                    return customElementComparer.Equals(customElement, (CustomMirrorElementEntity)y);
                case MirrorFinishElementEntity finishElement:
                    var finishElementComparer = new MirrorFinishElementEntityEqualityComparer();
                    return finishElementComparer.Equals(finishElement, (MirrorFinishElementEntity)y);
                case MirrorElementTraitEntity trait:
                    var traitComparer = new MirrorElementTraitEntityEqualityComparer();
                    return traitComparer.Equals(trait, (MirrorElementTraitEntity)y);
                default:
                    var genericComparer = new MirrorElementEntityBaseEqualityComparer();
                    return genericComparer.Equals(x, y);
            }
        }

        public int GetHashCode([DisallowNull] MirrorElementEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
    public class MirrorElementEntityBaseEqualityComparer : IEqualityComparer<MirrorElementEntity>
    {
        public bool Equals(MirrorElementEntity? x, MirrorElementEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            var baseEntityComparer = new DatabaseEntityEqualityComparer();
            var ldComparer = LocalizedDescription.GetComparer();

            return baseEntityComparer.Equals(x, y)
                   && x.Code == y.Code
                   && x.ShortCode == y.ShortCode
                   && x.MinimalCode == y.MinimalCode
                   && x.PhotoUrl == y.PhotoUrl
                   && x.PhotoUrl2 == y.PhotoUrl2
                   && x.IconUrl == y.IconUrl
                   && x.IsDefaultElement == y.IsDefaultElement
                   && ldComparer.Equals(x.LocalizedDescriptionInfo, y.LocalizedDescriptionInfo);
        }

        public int GetHashCode([DisallowNull] MirrorElementEntity obj)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support a Get Hash Code Implementation");
        }
    }

    
}

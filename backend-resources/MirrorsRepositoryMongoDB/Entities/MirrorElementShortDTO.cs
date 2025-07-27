using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    /// <summary>
    /// An interface for Objects that represent Elements Usually placed on the Mirror , do not include PhotoUrls
    /// </summary>
    public interface IMirrorElementShortDTO
    {
        string Code { get; }
        string ShortCode { get; }
        string MinimalCode { get; }
        bool IsDefaultElement { get; }
        string DefaultElementRefId { get; }
        LocalizedDescription LocalizedDescriptionInfo { get; }
    }
    /// <summary>
    /// The base Class for Objects that represent Elements Usually placed on the Mirror , do not include PhotoUrls
    /// </summary>
    public abstract class MirrorElementShortDTO : IMirrorElementShortDTO , IDeepClonable<MirrorElementShortDTO>
    {
        protected MirrorElementShortDTO()
        {
            
        }
        protected MirrorElementShortDTO(IMirrorElement element)
        {
            this.Code = element.Code;
            this.ShortCode = element.ShortCode;
            this.MinimalCode = element.MinimalCode;
            this.IsDefaultElement = !element.IsOverriddenElement;
            this.DefaultElementRefId = element.ElementId;
            this.LocalizedDescriptionInfo = element.LocalizedDescriptionInfo.GetDeepClone();
        }
        public string Code { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string MinimalCode { get; set; } = string.Empty;
        public bool IsDefaultElement { get; set; }
        public string DefaultElementRefId { get; set; } = string.Empty;
        public LocalizedDescription LocalizedDescriptionInfo { get; set; } = LocalizedDescription.Empty();

        public virtual MirrorElementShortDTO GetDeepClone()
        {
            var clone = (MirrorElementShortDTO)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            return clone;
        }

        /// <summary>
        /// Returns the Base Element (Only base Properties , Including PhotoUrls)
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        protected MirrorElementBase GetBaseElementInfoWithoutPhotos()
        {
            return new MirrorElementBase()
            {
                Code = this.Code,
                ShortCode = this.ShortCode,
                MinimalCode = this.MinimalCode,
                IsOverriddenElement = !this.IsDefaultElement,
                ElementId = this.DefaultElementRefId,
                LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone(),
            };
        }
    }
    public class MirrorElementShortDTOBaseEqualityComparer : IEqualityComparer<MirrorElementShortDTO>
    {
        private readonly LocalizedDescriptionEqualityComparer descriptionComparer = new();

        public bool Equals(MirrorElementShortDTO? x, MirrorElementShortDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Code == y.Code
                && x.ShortCode == y.ShortCode
                && x.MinimalCode == y.MinimalCode
                && x.IsDefaultElement == y.IsDefaultElement
                && x.DefaultElementRefId == y.DefaultElementRefId
                && descriptionComparer.Equals(x.LocalizedDescriptionInfo, y.LocalizedDescriptionInfo);
        }

        public int GetHashCode([DisallowNull] MirrorElementShortDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

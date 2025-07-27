using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System.Diagnostics.CodeAnalysis;

namespace AccessoriesRepoMongoDB.Entities
{
    public class AccessoryFinishInfo : IDeepClonable<AccessoryFinishInfo>
    {
        public string FinishId { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public List<string> ExtraPhotosUrl { get; set; } = new();
        public string PdfUrl { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;

        public AccessoryFinishInfo GetDeepClone()
        {
            var clone = (AccessoryFinishInfo)this.MemberwiseClone();
            clone.ExtraPhotosUrl = new(ExtraPhotosUrl);
            return clone;
        }
    }

    public class AccessoryFinishInfoEqualityComparer : IEqualityComparer<AccessoryFinishInfo>
    {
        public bool Equals(AccessoryFinishInfo? x, AccessoryFinishInfo? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return  
            x!.FinishId == y!.FinishId &&
            x.PhotoUrl == y.PhotoUrl &&
            x.PdfUrl == y.PdfUrl &&
            x.DimensionsPhotoUrl == y.DimensionsPhotoUrl &&
            x.ExtraPhotosUrl.SequenceEqual(y.ExtraPhotosUrl);
        }

        public int GetHashCode([DisallowNull] AccessoryFinishInfo obj)
        {
            throw new NotSupportedException($"{typeof(AccessoryFinishInfoEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

}

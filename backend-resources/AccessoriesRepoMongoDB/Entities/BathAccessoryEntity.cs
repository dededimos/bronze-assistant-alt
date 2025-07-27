using BathAccessoriesModelsLibrary;
using MongoDB.Bson;
using static MongoDbCommonLibrary.ExtensionMethods.MongoDbExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using MongoDbCommonLibrary.CommonEntities;
using CommonInterfacesBronze;
using System.Diagnostics.CodeAnalysis;

namespace AccessoriesRepoMongoDB.Entities
{
    public class BathAccessoryEntity : DescriptiveEntity, IDeepClonable<BathAccessoryEntity>
    {
        /// <summary>
        /// Indicated the Order in which the Accessory should Appear , a Lower Number Appears first in results
        /// </summary>
        public int SortNo { get; set; } = 99999;
        public bool IsOnline { get; set; } = false;
        /// <summary>
        /// The General Code of this Accessory (Does not include Finish Part)
        /// </summary>
        public string Code { get => GenerateGeneralCode(MainCode, ExtraCode, UsesOnlyMainCode); }

        /// <summary>
        /// Returns the General Code of the Accessory without Including the Finish of the Accessory
        /// </summary>
        /// <param name="mainCode">The Main code of the Accessory</param>
        /// <param name="extraCode">The Extra Code of the Accessory</param>
        /// <param name="usesOnlyMainCode">Weather the Accessory only used the Main code or Also the rest of the Options</param>
        /// <returns></returns>
        public static string GenerateGeneralCode(string mainCode, string extraCode, bool usesOnlyMainCode)
        {
            if (string.IsNullOrWhiteSpace(mainCode))
            {
                return "??";
            }
            if (usesOnlyMainCode)
            {
                return mainCode;
            }
            return string.Join('-', mainCode, extraCode).TrimEnd('-');
        }
        /// <summary>
        /// Returns the Code of the Accessory based on the Provided Options
        /// </summary>
        /// <param name="mainCode">The Main code of the Accessory</param>
        /// <param name="extraCode">The Extra Code of the Accessory</param>
        /// <param name="finishCode">The Finish Code of the Accessory</param>
        /// <param name="usesOnlyMainCode">Weather the Accessory only used the Main code or Also the rest of the Options</param>
        /// <returns></returns>
        public static string GenerateSpecificCode(string mainCode, string finishCode, string extraCode, bool usesOnlyMainCode)
        {
            if (string.IsNullOrWhiteSpace(mainCode))
            {
                return "??";
            }
            if (usesOnlyMainCode)
            {
                return mainCode;
            }
            return string.Join('-', mainCode, finishCode, extraCode).TrimEnd('-');
        }
        /// <summary>
        /// Returns all the Codes that an accessory can have based on the Available Finishes it can get
        /// </summary>
        /// <param name="entity">The Accessory</param>
        /// <param name="finishTraits">All the Finish Traits</param>
        /// <returns></returns>
        public static List<string> GetAllAvailableSpecificCodes(BathAccessoryEntity entity,IEnumerable<TraitEntity> finishTraits)
        {
            if (entity == null) { return []; }
            if (string.IsNullOrWhiteSpace(entity.MainCode)) { return []; }
            if (entity.UsesOnlyMainCode) { return [entity.MainCode]; }

            List<string> codes = [];
            foreach (var finish in entity.AvailableFinishes)
            {
                var finishCode = finishTraits.FirstOrDefault(t => t.IdAsString == finish.FinishId)?.Code ?? "??";
                var code = GenerateSpecificCode(entity.MainCode, finishCode, entity.ExtraCode, false);
                codes.Add(code);
            }
            return codes;
        }


        public string MainCode { get; set; } = string.Empty;
        public string ExtraCode { get; set; } = string.Empty;
        public bool UsesOnlyMainCode { get; set; }
        public string MainPhotoURL { get; set; } = string.Empty;
        public List<string> ExtraPhotosURL { get; set; } = [];
        public string PdfURL { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;
        /// <summary>
        /// The ObjectId of the Finish
        /// </summary>
        public string Finish { get; set; } = string.Empty;
        /// <summary>
        /// The Available Finishes of the Item
        /// </summary>
        public List<AccessoryFinishInfo> AvailableFinishes { get; set; } = [];
        /// <summary>
        /// The Object Id of the Material
        /// </summary>
        public string Material { get; set; } = string.Empty;
        /// <summary>
        /// The Object Id of the Size
        /// </summary>
        public string Size { get; set; } = string.Empty;
        /// <summary>
        /// The ObjectIds of other Accessories in different Finish
        /// </summary>
        public List<string> SizeVariations { get; set; } = [];
        /// <summary>
        /// The Object Id of the Shape
        /// </summary>
        public string Shape { get; set; } = string.Empty;
        /// <summary>
        /// The Object Ids of the Primary Types
        /// </summary>
        public List<string> PrimaryTypes { get; set; } = [];
        /// <summary>
        /// The Object Ids of the Secondary Types
        /// </summary>
        public List<string> SecondaryTypes { get; set; } = [];
        /// <summary>
        /// The Object Ids of the Categories
        /// </summary>
        public List<string> Categories { get; set; } = [];
        /// <summary>
        /// The Object Ids of the Series
        /// </summary>
        public List<string> Series { get; set; } = [];
        /// <summary>
        /// The Object Ids of the Mounting Types
        /// </summary>
        public List<string> MountingTypes { get; set; } = [];
        /// <summary>
        /// The Object Ids of the Accessories that are a mounting variation of this Accessory
        /// </summary>
        public List<string> MountingVariations { get; set; } = [];
        /// <summary>
        /// The Object Ids of the Dimensions paired with its value
        /// </summary>
        public Dictionary<string, double> Dimensions { get; set; } = [];
        /// <summary>
        /// A Dictionary Containing Prices info per Added Price Trait . Key=> PriceTraitId
        /// </summary>
        public List<PriceInfo> PricesInfo { get; set; } = [];


        public BathAccessoryEntity()
        {

        }
        public BathAccessoryEntity GetDeepClone()
        {
            BathAccessoryEntity clone = new()
            {
                Id = Id,
                LastModified = LastModified,
                Notes = Notes,
                Name = Name.GetDeepClone(),
                Description = Description.GetDeepClone(),
                ExtendedDescription = ExtendedDescription.GetDeepClone(),
                IsOnline = IsOnline,
                MainCode = MainCode,
                ExtraCode = ExtraCode,
                UsesOnlyMainCode = UsesOnlyMainCode,
                MainPhotoURL = MainPhotoURL,
                ExtraPhotosURL = new(ExtraPhotosURL),
                DimensionsPhotoUrl = DimensionsPhotoUrl,
                PdfURL = PdfURL,
                Finish = this.Finish,
                AvailableFinishes = new(AvailableFinishes.Select(af => af.GetDeepClone())),
                Material = this.Material,
                Size = this.Size,
                SizeVariations = new(SizeVariations),
                Shape = this.Shape,
                PrimaryTypes = new(this.PrimaryTypes),
                SecondaryTypes = new(this.SecondaryTypes),
                Categories = new(this.Categories),
                Series = new(this.Series),
                MountingTypes = new(this.MountingTypes),
                Dimensions = new(this.Dimensions),
                PricesInfo = PricesInfo.Select(pi => pi.GetDeepClone()).ToList(),
            };
            return clone;
        }
    }

    public class BathAccessoryEntityEqualityComparer : IEqualityComparer<BathAccessoryEntity>
    {
        public bool Equals(BathAccessoryEntity? x, BathAccessoryEntity? y)
        {
            var descriptiveEntityComparer = new DescriptiveEntityEqualityComparer();
            var finishInfoComparer = new AccessoryFinishInfoEqualityComparer();
            var priceInfoComparer = new PriceInfoEqualityComparer();

            return descriptiveEntityComparer.Equals(x, y) &&
            // The base comparer checks for nullability 
            x!.IsOnline == y!.IsOnline &&
            x.SortNo == y.SortNo &&
            x.Code == y.Code &&
            x.MainCode == y.MainCode &&
            x.ExtraCode == y.ExtraCode &&
            x.UsesOnlyMainCode == y.UsesOnlyMainCode &&
            x.MainPhotoURL == y.MainPhotoURL &&
            x.ExtraPhotosURL.SequenceEqual(y.ExtraPhotosURL) &&
            x.PdfURL == y.PdfURL &&
            x.DimensionsPhotoUrl == y.DimensionsPhotoUrl &&
            x.Finish == y.Finish &&
            x.AvailableFinishes.SequenceEqual(y.AvailableFinishes, finishInfoComparer) &&
            x.Material == y.Material &&
            x.Size == y.Size &&
            x.SizeVariations.SequenceEqual(y.SizeVariations) &&
            x.Shape == y.Shape &&
            x.PrimaryTypes.SequenceEqual(y.PrimaryTypes) &&
            x.SecondaryTypes.SequenceEqual(y.SecondaryTypes) &&
            x.Categories.SequenceEqual(y.Categories) &&
            x.Series.SequenceEqual(y.Series) &&
            x.MountingTypes.SequenceEqual(y.MountingTypes) &&
            x.MountingVariations.SequenceEqual(y.MountingVariations) &&
            x.Dimensions.Keys.SequenceEqual(y.Dimensions.Keys) &&
            x.Dimensions.Values.SequenceEqual(y.Dimensions.Values) &&
            x.PricesInfo.SequenceEqual(y.PricesInfo, priceInfoComparer);
        }

        public int GetHashCode([DisallowNull] BathAccessoryEntity obj)
        {
            throw new NotSupportedException($"{typeof(BathAccessoryEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

    public class PriceInfo : IDeepClonable<PriceInfo>
    {
        public decimal PriceValue { get; set; } = 0;
        public string PriceTraitId { get; set; } = string.Empty;
        public string RefersToFinishId { get; set; } = string.Empty;
        public string RefersToFinishGroupId { get; set; } = string.Empty;

        public PriceInfo GetDeepClone()
        {
            return (PriceInfo)this.MemberwiseClone();
        }
    }

    public class PriceInfoEqualityComparer : IEqualityComparer<PriceInfo>
    {
        public bool Equals(PriceInfo? x, PriceInfo? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return
            x!.PriceValue == y!.PriceValue &&
            x.PriceTraitId == y.PriceTraitId &&
            x.RefersToFinishId == y.RefersToFinishId &&
            x.RefersToFinishGroupId == y.RefersToFinishGroupId;
        }

        public int GetHashCode([DisallowNull] PriceInfo obj)
        {
            throw new NotSupportedException($"{typeof(PriceInfoEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

}

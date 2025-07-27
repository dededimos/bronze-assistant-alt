using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary.CommonEntities
{
    public class DescriptiveEntity : DbEntity
    {
        public LocalizedString Name { get; set; } = LocalizedString.Undefined();
        public LocalizedString Description { get; set; } = LocalizedString.Undefined();
        public LocalizedString ExtendedDescription { get; set; } = LocalizedString.Undefined();

        /// <summary>
        /// Gets a specific language Description Info from the <see cref="DescriptiveEntity"/>
        /// </summary>
        /// <param name="langIdentifier">The Language Identifier</param>
        /// <param name="returnDefaultWhenEmpty">Weather to return the Default value when the Language value is Empty</param>
        /// <returns></returns>
        public ObjectDescriptionInfo GetDescriptionInfo(string langIdentifier, bool returnDefaultWhenEmpty = true)
        {
            return new ObjectDescriptionInfo(
                this.Name.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty),
                this.Description.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty),
                this.ExtendedDescription.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty));
        }
    }
    public class DescriptiveEntityEqualityComparer : IEqualityComparer<DescriptiveEntity>
    {
        public bool Equals(DescriptiveEntity? x, DescriptiveEntity? y)
        {
            if (x is null || y is null)
            {
                return false;
            }
            var baseEntityComparer = new DbEntityEqualityComparer();
            var localizedStringComparer = new LocalizedStringEqualityComparer();

            return baseEntityComparer.Equals(x, y) &&
                //nullity excluded from baseEntity Comparer
                localizedStringComparer.Equals(x!.Name,y!.Name) &&
                localizedStringComparer.Equals(x.Description,y.Description) &&
                localizedStringComparer.Equals(x.ExtendedDescription,y.ExtendedDescription);
                
            //The base comparer checks for nullability 
        }

        public int GetHashCode([DisallowNull] DescriptiveEntity obj)
        {
            throw new NotSupportedException($"{typeof(DescriptiveEntityEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

    public class ItemDescription : IDeepClonable<ItemDescription>
    {
        public LocalizedString Name { get; set; } = LocalizedString.Undefined();
        public LocalizedString Description { get; set; } = LocalizedString.Undefined();
        public LocalizedString ExtendedDescription { get; set; } = LocalizedString.Undefined();

        /// <summary>
        /// Gets a specific language Description Info from the <see cref="DescriptiveEntity"/>
        /// </summary>
        /// <param name="langIdentifier">The Language Identifier</param>
        /// <param name="returnDefaultWhenEmpty">Weather to return the Default value when the Language value is Empty</param>
        /// <returns></returns>
        public ObjectDescriptionInfo GetDescriptionInfo(string langIdentifier, bool returnDefaultWhenEmpty = true)
        {
            return new ObjectDescriptionInfo(
                this.Name.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty),
                this.Description.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty),
                this.ExtendedDescription.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty));
        }

        public static ItemDescription Empty() => new();

        public ItemDescription GetDeepClone()
        {
            var clone = (ItemDescription)this.MemberwiseClone();
            clone.Name = this.Name.GetDeepClone();
            clone.Description = this.Description.GetDeepClone();
            clone.ExtendedDescription = this.ExtendedDescription.GetDeepClone();
            return clone;
        }
    }
    public class ItemDescriptionEqualityComparer : IEqualityComparer<ItemDescription>
    {
        public bool Equals(ItemDescription? x, ItemDescription? y)
        {
            if (x is null || y is null)
            {
                return false;
            }
            var localizedStringComparer = new LocalizedStringEqualityComparer();

            return localizedStringComparer.Equals(x!.Name, y!.Name) &&
                localizedStringComparer.Equals(x.Description, y.Description) &&
                localizedStringComparer.Equals(x.ExtendedDescription, y.ExtendedDescription);
        }

        public int GetHashCode([DisallowNull] ItemDescription obj)
        {
            throw new NotSupportedException($"{typeof(ItemDescriptionEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }

}

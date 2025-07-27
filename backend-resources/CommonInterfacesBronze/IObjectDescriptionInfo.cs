using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    /// <summary>
    /// The Base Description Info
    /// </summary>
    public class ObjectDescriptionInfo : IDeepClonable<ObjectDescriptionInfo>
    {
        /// <summary>
        /// The Name of the Item
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The Description of the Item
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// An Extended Description of the Item
        /// </summary>
        public string ExtendedDescription { get; }

        public ObjectDescriptionInfo(string name, string description, string extendedDescription)
        {
            Name = name;
            Description = description;
            ExtendedDescription = extendedDescription;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ObjectDescriptionInfo o)
            {
                return o.Name == this.Name && o.Description == this.Description && o.ExtendedDescription == this.ExtendedDescription;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name,Description, ExtendedDescription);
        }

        public static ObjectDescriptionInfo Empty() => new(string.Empty, string.Empty, string.Empty);

        public ObjectDescriptionInfo GetDeepClone()
        {
            return (ObjectDescriptionInfo)this.MemberwiseClone();
        }

    }

    public interface IDescriptive
    {
        public string Name { get; }
        public string Description { get; }
        public string ExtendedDescription { get; }
    }
}

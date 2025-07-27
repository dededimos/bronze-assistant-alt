using MirrorsLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Helpers
{
    /// <summary>
    /// A record object describing a Modification to a Mirror
    /// </summary>
    /// <param name="ModificationType"></param>
    /// <param name="Modification"></param>
    public record MirrorModificationDescriptor(MirrorModificationType ModificationType, MirrorElementModification Modification)
    {
        public MirrorModificationType ModificationType { get; set; } = ModificationType;
        public MirrorElementModification Modification { get; set; } = Modification;

        /// <summary>
        /// Checks if the ModificationDescriptor is equal to another one, including the AnyElementModification and AnyModificationType in the comparisons
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEqualIncludingAnyTypes(MirrorModificationDescriptor other)
        {
            //Their types are equal if any of them have the AnyModificationType or if they are the same
            bool areTypesEqual = ModificationType == MirrorModificationType.AnyModificationType
                              || other.ModificationType == MirrorModificationType.AnyModificationType
                              || ModificationType == other.ModificationType;
            bool areModificationsEqual = Modification == MirrorElementModification.AnyElementModification
                                      || other.Modification == MirrorElementModification.AnyElementModification
                                      || Modification == other.Modification;
            return areTypesEqual && areModificationsEqual;
        }

    }
}

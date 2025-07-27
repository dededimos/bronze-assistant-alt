using CommonInterfacesBronze;
using MirrorsLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.MirrorElements.Charachteristics
{
    public interface IMirrorElementTrait : IDeepClonable<IMirrorElementTrait>
    {
        bool IsAssignableToAll { get; set; }
        List<string> TargetElementIds { get; set; }
        List<Type> TargetTypes { get; set; }
    }

    public class MirrorElementTraitBase : MirrorElementBase, IDeepClonable<MirrorElementTraitBase>, IMirrorElementTrait
    {
        public MirrorElementTraitBase()
        {

        }
        public MirrorElementTraitBase(IMirrorElement elementInfo)
            : base(elementInfo)
        {

        }
        /// <summary>
        /// Weather this trait can be assigned to any Type of Mirror Element/Mirror
        /// </summary>
        public bool IsAssignableToAll { get; set; }
        /// <summary>
        /// To Which type this trait can be assigned to
        /// </summary>
        public List<Type> TargetTypes { get; set; } = [];
        /// <summary>
        /// Specific Element Ids where this can be assigned to
        /// </summary>
        public List<string> TargetElementIds { get; set; } = [];

        public override MirrorElementTraitBase GetDeepClone()
        {
            var clone = (MirrorElementTraitBase)base.GetDeepClone();
            clone.TargetTypes = new List<Type>(TargetTypes);
            clone.TargetElementIds = new List<string>(TargetElementIds);
            return clone;
        }

        IMirrorElementTrait IDeepClonable<IMirrorElementTrait>.GetDeepClone()
        {
            return GetDeepClone();
        }
    }
    public class CustomMirrorTrait : MirrorElementTraitBase, IDeepClonable<CustomMirrorTrait>
    {
        public CustomMirrorTrait()
        {
            
        }
        public CustomMirrorTrait(MirrorElementTraitBase trait , LocalizedString customTraitType) : base(trait)
        {
            CustomTraitType = customTraitType.GetDeepClone();
        }
        public LocalizedString CustomTraitType { get; set; } = LocalizedString.Undefined();

        public override CustomMirrorTrait GetDeepClone()
        {
            var clone = (CustomMirrorTrait)base.GetDeepClone();
            clone.CustomTraitType = this.CustomTraitType.GetDeepClone();
            return clone;
        }
    }



}

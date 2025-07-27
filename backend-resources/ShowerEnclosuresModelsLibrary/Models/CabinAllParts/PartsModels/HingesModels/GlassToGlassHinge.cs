using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels
{
    public class GlassToGlassHinge : CabinHinge
    {

        /// <summary>
        /// The Length of the Hinge that is Inside the Door Part
        /// </summary>
        public int InDoorLength { get; set; }

        public override CabinHingeType HingeType { get => CabinHingeType.GlassToGlassHinge; }

        public GlassToGlassHinge()
        {

        }
        
        public override GlassToGlassHinge GetDeepClone()
        {
            return (GlassToGlassHinge)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), InDoorLength);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is GlassToGlassHinge otherHinge)
        //    {
        //        return base.Equals(otherHinge) &&
        //            InDoorLength == otherHinge.InDoorLength;
        //    }
        //    return false;
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels
{
    public class Hinge9B : CabinHinge
    {
        /// <summary>
        /// How many mm is the Hinge outside of the Glass
        /// The ViewHeight added to the glass From the Hinge (Without Including its Supports)
        /// </summary>
        public int HingeOverlappingHeight { get; set; }

        /// <summary>
        /// The View Length of the support tube , extending after the Hinge and connecting to the L0 Profile
        /// </summary>
        public int SupportTubeLength { get; set; }

        /// <summary>
        /// The View Height of the support tube , extending after the Hinge and connecting to the L0 Profile
        /// </summary>
        public int SupportTubeHeight { get; set; }

        /// <summary>
        /// The Corner Radius of the Hinge side that is inside the Glass
        /// Bottom for the upper Hinge , Top for the Bottom Hinge
        /// </summary>
        public int CornerRadiusInGlass { get; set; }

        public override CabinHingeType HingeType { get => CabinHingeType.Hinge9B; }
        
        public override Hinge9B GetDeepClone()
        {
            return (Hinge9B)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), HingeOverlappingHeight, SupportTubeLength, SupportTubeHeight, CornerRadiusInGlass);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is Hinge9B otherHinge)
        //    {
        //        return base.Equals(otherHinge) &&
        //            HingeOverlappingHeight == otherHinge.HingeOverlappingHeight &&
        //            SupportTubeLength == otherHinge.SupportTubeLength &&
        //            SupportTubeHeight == otherHinge.SupportTubeHeight &&
        //            CornerRadiusInGlass == otherHinge.CornerRadiusInGlass;
        //    }
        //    return false;
        //}
    }
}

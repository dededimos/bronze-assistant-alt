using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels
{
    public class HingeDB : CabinHinge
    {
        /// <summary>
        /// The Height of the Hinge After its Wall Plate
        /// How high is in the View The hinge Part which is connected with the Glass
        /// This is NOT THE TOTAL : The Total Hinge Height Includes also the Wall Plate
        /// </summary>
        public int InnerHeight { get; set; }
        public int WallPlateThicknessView { get; set; }

        public override CabinHingeType HingeType { get => CabinHingeType.HingeDB; }
        
        public override HingeDB GetDeepClone()
        {
            return (HingeDB)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), InnerHeight, WallPlateThicknessView);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is HingeDB otherHinge)
        //    {
        //        return base.Equals(otherHinge) &&
        //            InnerHeight == otherHinge.InnerHeight &&
        //            WallPlateThicknessView == otherHinge.WallPlateThicknessView;
        //    }
        //    return false;
        //}
    }
}

using ShowerEnclosuresModelsLibrary.Attributes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels
{
    public class CabinHinge : CabinPart , IDeductableGlassesLength
    {
        public override CabinPartType Part { get => CabinPartType.Hinge; }
        public virtual CabinHingeType HingeType { get => CabinHingeType.GenericHinge; }
        /// <summary>
        /// The front View Length of the Hinge
        /// </summary>
        public int LengthView { get; set; }
        /// <summary>
        /// The front View Height of the Hinge
        /// </summary>
        public int HeightView { get; set; }

        /// <summary>
        /// The Air from the Glass to the Hinge Connecting Point
        /// Ex. If it is a wall Hinge , this measure is the Distance of the glass from the Wall
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int GlassGapAER { get; set; }

        public CabinHinge()
        {

        }

        public override CabinHinge GetDeepClone()
        {
            return (CabinHinge)base.GetDeepClone();
        }

        /// <summary>
        /// Returns the Length that will be Deducted from the Structure's LengthMin to determine the Total Glasses Length
        /// </summary>
        /// <param name="model">The Model of the Structure</param>
        /// <returns>The Deductible Length</returns>
        public double GetDeductableLength(CabinModelEnum model)
        {
            return GlassGapAER;
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), HingeType, LengthView, HeightView, GlassGapAER);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is CabinHinge otherHinge)
        //    {
        //        return Equals(otherHinge) &&
        //            HingeType == otherHinge.HingeType &&
        //            LengthView == otherHinge.LengthView &&
        //            HeightView == otherHinge.HeightView &&
        //            GlassGapAER == otherHinge.GlassGapAER;
        //    }
        //    return false;
        //}
    }

    public enum CabinHingeType
    {
        GenericHinge,
        GlassToGlassHinge,
        Hinge9B,
        HingeDB
    }


}

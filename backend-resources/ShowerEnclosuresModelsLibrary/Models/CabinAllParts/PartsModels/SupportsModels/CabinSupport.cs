using ShowerEnclosuresModelsLibrary.Attributes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels
{
    public class CabinSupport : CabinPart , IDeductableGlassesLength
    {
        public override CabinPartType Part { get => CabinPartType.SmallSupport; }

        /// <summary>
        /// The Length View of the Support (when observed from the Installation position)
        /// </summary>
        public int LengthView { get; set; }
        /// <summary>
        /// The Height View of the Support (when observed from the Installation position)
        /// </summary>
        public int HeightView { get; set; }
        /// <summary>
        /// The Air that the Support leaves between the Glass and whatever the glass is attached to (wall,floor,another glass e.t.c.)
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int GlassGapAER { get; set; }
        /// <summary>
        /// The Tollerance offered by the support (this might actually be wrong and depend solely on the difference of the supports hole with the glasses holes)
        /// </summary>
        [Impact(ImpactOn.Tollerances)]
        [Impact(ImpactOn.Glasses)]
        public int Tollerance { get; set; }

        /// <summary>
        /// Returns the Length that will be Deducted from the Structure's LengthMin to determine the Total Glasses Length
        /// </summary>
        /// <param name="model">The Model of the Structure</param>
        /// <returns>The Deductible Length</returns>
        public double GetDeductableLength(CabinModelEnum model)
        {
            return GlassGapAER;
        }

        public override CabinSupport GetDeepClone()
        {
            return (CabinSupport)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), LengthView, HeightView, GlassGapAER, Tollerance);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is CabinSupport otherSupport)
        //    {
        //        return base.Equals(otherSupport) &&
        //            LengthView == otherSupport.LengthView &&
        //            HeightView == otherSupport.HeightView &&
        //            GlassGapAER == otherSupport.GlassGapAER &&
        //            Tollerance == otherSupport.Tollerance;
        //    }
        //    return false;
        //}
    }
}

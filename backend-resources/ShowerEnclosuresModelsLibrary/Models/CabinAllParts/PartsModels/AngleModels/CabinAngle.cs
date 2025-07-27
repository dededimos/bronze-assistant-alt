using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Attributes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.AngleModels
{
    public class CabinAngle : CabinPart , IDeductableGlassesLength
    {
        public override CabinPartType Part { get => CabinPartType.AnglePart; }

        /// <summary>
        /// The Distance that the Edge of the Door Has from the end of the Angle (EndX of Door with EndX of Angle -- angle Length is Included) . 
        /// Consequently the Cabin Length ends at the end of the Angle though the door glass ends at the 
        /// this distance before the angle
        /// This is used to Calculate the Length of the Glasses . It does not affect the length of the L0 Horizontal Profile.
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int AngleDistanceFromDoor { get; set; }

        /// <summary>
        /// The Length of the Angle Part .This is how much length the angle takes from the LO horizontal profile.
        /// This is Used to calculate only the Length of the LO profile . It does not affect the Length of the glasses.
        /// </summary>
        [Impact(ImpactOn.L0)]
        public int AngleLengthL0 { get; set; }

        public int AngleHeight { get; set; }

        public CabinAngle()
        {
            
        }
        
        // Returns the Clone as ICabinPart
        public override CabinAngle GetDeepClone()
        {
            return (CabinAngle)base.GetDeepClone();
        }

        /// <summary>
        /// Returns the Length that will be Deducted from the Structure's LengthMin to determine the Total Glasses Length
        /// </summary>
        /// <param name="model">The Model of the Structure</param>
        /// <returns>The Deductible Length</returns>
        public double GetDeductableLength(CabinModelEnum model)
        {
            return AngleDistanceFromDoor;
        }

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode() + 23 * HashCode.Combine(AngleDistanceFromDoor, AngleLengthL0, AngleHeight);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is CabinAngle otherAngle)
        //    {
        //        return Equals(otherAngle) &&
        //            AngleDistanceFromDoor == otherAngle.AngleDistanceFromDoor &&
        //            AngleLengthL0 == otherAngle.AngleLengthL0 &&
        //            AngleHeight == otherAngle.AngleHeight;
        //    }
        //    return false;
        //}
    }
}

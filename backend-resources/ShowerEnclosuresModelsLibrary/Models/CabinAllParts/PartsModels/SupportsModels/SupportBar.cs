using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels
{
    public class SupportBar : CabinPart
    {
        public override CabinPartType Part { get => CabinPartType.BarSupport; }

        /// <summary>
        /// The Height of the Support Bar that is Out of the Glass
        /// </summary>
        public int OutOfGlassHeight { get; set; }

        /// <summary>
        /// How the Support Bar is Placed , compared to the current structure that is used on
        /// </summary>
        public SupportBarPlacement Placement { get; set; }

        public int ClampViewLength { get; set; }
        public int ClampViewHeight { get; set; }
        /// <summary>
        /// The Default Distance of the Clamp Center from the Edge of the Glass
        /// </summary>
        public double ClampCenterDistanceFromGlassDefault { get; set; }
        /// <summary>
        /// The Current Distance of the Clamp Center from the Edge of the Glass
        /// Might be different from default Changes at Calculations of parts.
        /// </summary>
        public double ClampCenterDistanceFromGlass { get; set; }
        
        public override SupportBar GetDeepClone()
        {
            return (SupportBar)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), OutOfGlassHeight, Placement, ClampViewLength, ClampViewHeight, ClampCenterDistanceFromGlassDefault, ClampCenterDistanceFromGlass);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is SupportBar otherSupport)
        //    {
        //        return base.Equals(otherSupport) &&
        //            OutOfGlassHeight == otherSupport.OutOfGlassHeight &&
        //            Placement == otherSupport.Placement &&
        //            ClampViewLength == otherSupport.ClampViewLength &&
        //            ClampViewHeight == otherSupport.ClampViewHeight &&
        //            ClampCenterDistanceFromGlassDefault == otherSupport.ClampCenterDistanceFromGlassDefault &&
        //            ClampCenterDistanceFromGlass == otherSupport.ClampCenterDistanceFromGlass;
        //    }
        //    return false;
        //}
    }

    public enum SupportBarPlacement
    {
        UndefinedPlacement = 0,
        VarticallyToStructure = 1,
        DiagonallyToStructure = 2,
        ParallelToStructure = 3,
        VerticalOrDiagonal = 4,
    };


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels
{
    public class ProfileHinge : Profile
    {
        public int TopHeightAboveGlass { get; set; }
        public int BottomHeightBelowGlass { get; set; }

        public ProfileHinge(CabinPartType type , CabinProfileType profileType) : base(type,profileType) { }
        
        public override ProfileHinge GetDeepClone()
        {
            return (ProfileHinge)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), TopHeightAboveGlass, BottomHeightBelowGlass);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is ProfileHinge otherProfile)
        //    {
        //        return base.Equals(otherProfile) &&
        //            TopHeightAboveGlass == otherProfile.TopHeightAboveGlass &&
        //            BottomHeightBelowGlass == otherProfile.BottomHeightBelowGlass;
        //    }
        //    return false;
        //}
    }
}

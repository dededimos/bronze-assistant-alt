using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Attributes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.AngleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels
{
    public class CabinHandle : CabinPart
    {
        public override CabinPartType Part { get => CabinPartType.Handle; }
        public CabinHandleType HandleType { get; private set; }

        /// <summary>
        /// The Length that the Handle Occupies in a glass
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int HandleLengthToGlass { get; set; }

        /// <summary>
        /// The Height that the Handle Occupies in a glass
        /// </summary>
        public int HandleHeightToGlass { get; set; }
        /// <summary>
        /// The Corner Radius of all four Edges of the Handle
        /// Setting the Height and Length to equal and The Corner Radius to half their value will return a Circular Handle
        /// </summary>
        public double HandleEdgesCornerRadius { get; set; }

        /// <summary>
        /// If the Middle of the Handle is Holed , then this is the Thickness of the Bodyaround the Hole
        /// If Outer Thickness is Zero it means there is no Hole in the Middle
        /// </summary>
        public double HandleOuterThickness { get; set; }

        /// <summary>
        /// The Distance the Handle must have from the Fixed Side when Door is open
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int HandleComfortDistance { get; set; }

        /// <summary>
        /// The minimum Distance the Handle must Have from the Edge of the glass it is placed on
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int MinimumDistanceFromEdge { get; set; }

        /// <summary>
        /// Weather the Handle is Circular
        /// Asserts whether Height=Length and CornerRadius * 2 = Length
        /// </summary>
        public bool IsCircularHandle
        {
            get => (HandleLengthToGlass == HandleHeightToGlass && HandleLengthToGlass == HandleEdgesCornerRadius * 2);
        }

        public CabinHandle(CabinHandleType typeOfHandle)
        {
            HandleType = typeOfHandle;
        }

        /// <summary>
        /// The Distance of the Sliding Door that should remain out , so that the Handle can be useable
        /// </summary>
        /// <returns></returns>
        public int GetSlidingDoorAirDistance()
        {
            return HandleComfortDistance 
                 + HandleLengthToGlass
                 + MinimumDistanceFromEdge;
        }

        /// <summary>
        /// Gets the distance the Handle Center Must Have from the Edge
        /// </summary>
        /// <returns></returns>
        public double GetHandleCenterDistanceFromEdge()
        {
            return HandleLengthToGlass / 2d 
                 + MinimumDistanceFromEdge;
        }

        public override CabinHandle GetDeepClone()
        {
            return (CabinHandle)base.GetDeepClone();
        }

        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(base.GetHashCode(), HandleType, HandleLengthToGlass, HandleHeightToGlass, HandleEdgesCornerRadius, HandleOuterThickness, HandleComfortDistance, MinimumDistanceFromEdge);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is CabinHandle otherHandle)
        //    {
        //        return Equals(otherHandle) &&
        //            HandleType == otherHandle.HandleType &&
        //            HandleLengthToGlass == otherHandle.HandleLengthToGlass &&
        //            HandleHeightToGlass == otherHandle.HandleHeightToGlass &&
        //            HandleEdgesCornerRadius == otherHandle.HandleEdgesCornerRadius &&
        //            HandleOuterThickness == otherHandle.HandleOuterThickness &&
        //            HandleComfortDistance == otherHandle.HandleComfortDistance &&
        //            MinimumDistanceFromEdge == otherHandle.MinimumDistanceFromEdge;
        //    }
        //    return false;
        //}

    }

}

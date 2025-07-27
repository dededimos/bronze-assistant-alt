using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Attributes;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels
{
    public class Profile : CabinPart , IWithCutLengthStepPart , IDeductableGlassesLength
    {
        [Impact(ImpactOn.Glasses)]
        public CabinProfileType ProfileType { get; private set; }

        /// <summary>
        /// The Depth of the Glass That is Inside the Profile
        /// Either because it gets inside the Cavity or because it is inside the L0 height
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int GlassInProfileDepth { get; set; }

        /// <summary>
        /// The Distance a Sliding Glass Must Have from the top or Bottom of the Profile
        /// Valid only for Horizontal L0 Profiles
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int SliderDistance { get; set; }

        /// <summary>
        /// The Thickness of the Glass Cavity, What Type of Glass can be inserted inside the Cavity
        /// </summary>
        public int GlassCavityThickness { get; set; }

        /// <summary>
        /// The Width of the Profile at the Size its Touching the Wall or Connection with another Piece . 
        /// Example 17mm for 8W
        /// </summary>
        public int PlacementWidth { get; set; }
        /// <summary>
        /// The Length at which the Profile is Cut
        /// </summary>
        public double CutLength { get; set; }
        /// <summary>
        /// The Extra Part Of the Profile when there is a Step
        /// </summary>
        public double CutLengthStepPart { get; set; }
        /// <summary>
        /// The Thickness Width of the Profile . How wide is the view (Never Cut) . For Example 8W is 34mm
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int ThicknessView { get; set; }
        /// <summary>
        /// The ThicknessView of the Inner Profile - If its there
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        public int InnerThicknessView { get; set; }
        /// <summary>
        /// The Max Tollerance that this Profile can Give to a Structure
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        [Impact(ImpactOn.Tollerances)]
        public int Tollerance { get; set; }

        /// <summary>
        /// The Tollerance that this profile can give to any connected Pieces
        /// </summary>
        [Impact(ImpactOn.Glasses)]
        [Impact(ImpactOn.Tollerances)]
        public int SideTollerance { get; set; }

        /// <summary>
        /// The Strip that goes along with this Profile , if Any
        /// </summary>
        public string ContainedStripCode { get; set; } = string.Empty;

        public Profile(CabinPartType type , CabinProfileType profileType)
        {
            if (type is not CabinPartType.ProfileHinge
                    and not CabinPartType.Profile
                    and not CabinPartType.MagnetProfile)
            {
                throw new ArgumentException($"{type} is Invalid for a Profile", $"{nameof(type)}");
            }
            Part = type;
            ProfileType = profileType;
        }


        public override Profile GetDeepClone()
        {
            return (Profile)base.GetDeepClone();
        }

        /// <summary>
        /// Returns the Length that will be Deducted from the Structure's LengthMin to determine the Total Glasses Length
        /// </summary>
        /// <param name="model">The Model of the Structure</param>
        /// <returns>The Deductible Length (takes into account GlassInProfileDepth Also)</returns>
        public double GetDeductableLength(CabinModelEnum model)
        {
            return ProfileType switch
            {
                CabinProfileType.Undefined => 0,
                CabinProfileType.WallProfile => (ThicknessView - GlassInProfileDepth),
                CabinProfileType.GlassProfile => (ThicknessView - GlassInProfileDepth),
                CabinProfileType.GuideProfile => 0,
                CabinProfileType.WaterSealerProfile => ThicknessView,
                CabinProfileType.MagnetProfile => ThicknessView,
                CabinProfileType.InbetweenHingeProfile => (ThicknessView - GlassInProfileDepth),
                CabinProfileType.HingeProfile => ThicknessView - GlassInProfileDepth,
                CabinProfileType.ConnectorProfile => (model == CabinModelEnum.Model9F ? ThicknessView - GlassInProfileDepth : InnerThicknessView - GlassInProfileDepth),
                CabinProfileType.BottomGlassProfile => 0,
                CabinProfileType.HorizontalBarProfile => 0,
                _ => (double)0,
            };
        }
    }

    public enum CabinProfileType
    {
        Undefined = 0,
        WallProfile = 1,
        GlassProfile = 2,
        GuideProfile = 3,
        WaterSealerProfile = 4,
        MagnetProfile = 5,
        InbetweenHingeProfile = 6,
        HingeProfile = 7,
        ConnectorProfile = 8,
        BottomGlassProfile = 9,
        HorizontalBarProfile = 10,
        Other = 100,
    }

}

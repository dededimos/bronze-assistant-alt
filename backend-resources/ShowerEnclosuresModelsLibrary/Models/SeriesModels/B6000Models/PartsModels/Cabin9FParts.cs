using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels
{
    public class Cabin9FParts : CabinPartsList , IDoubleWallProfile , IHorizontalProfile
    {
#nullable enable
        public Profile? WallProfile1
        {
            get => GetPartOrNull<Profile>(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
        }
        public Profile? WallProfile2
        {
            get => GetPartOrNull<Profile>(PartSpot.NonWallSide);
            set => SetPart(PartSpot.NonWallSide, value);
        }
        public Profile? HorizontalProfileTop
        {
            get => GetPartOrNull<Profile>(PartSpot.HorizontalGuideTop);
            set => SetPart(PartSpot.HorizontalGuideTop, value);
        }
        public Profile? HorizontalProfileBottom
        {
            get => GetPartOrNull<Profile>(PartSpot.HorizontalGuideBottom);
            set => SetPart(PartSpot.HorizontalGuideBottom, value);
        }
        /// <summary>
        /// The Profile that is put in the First Part of the step (Not an LO Profile)
        /// </summary>
        public Profile? StepBottomProfile
        {
            get => GetPartOrNull<Profile>(PartSpot.StepBottomSide);
            set => SetPart(PartSpot.StepBottomSide, value);
        }
        
        public Cabin9FParts()
        {

        }
        public override Cabin9FParts GetDeepClone()
        {
            return (Cabin9FParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public Cabin9FParts(Cabin9FParts parts)
        //{
        //    this.WallProfile1 = parts.WallProfile1?.GetDeepClone() as Profile;
        //    this.WallProfile2 = parts.WallProfile2?.GetDeepClone() as Profile;
        //    this.HorizontalProfileTop = parts.HorizontalProfileTop?.GetDeepClone() as Profile;
        //    this.StepBottomProfile = parts.StepBottomProfile?.GetDeepClone() as Profile;
        //    this.HorizontalProfileBottom = parts.HorizontalProfileBottom?.GetDeepClone() as Profile;
        //}

    }
}

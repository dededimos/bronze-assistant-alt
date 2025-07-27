using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels
{
    public class Cabin9SParts : CabinPartsList , IHorizontalProfile , ICloseStrip , IHandle , IDoubleWallProfile
    {
#nullable enable
        public Profile? WallProfile1
        {
            get => GetPartOrNull<Profile>(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
        }
        public Profile? WallProfile2
        {
            get => GetPartOrNull<Profile>(PartSpot.WallSide2);
            set => SetPart(PartSpot.WallSide2, value);
        }
        /// <summary>
        /// The Profile that is put in the First Part of the step (Not an LO Profile)
        /// </summary>
        public Profile? StepBottomProfile
        {
            get => GetPartOrNull<Profile>(PartSpot.StepBottomSide);
            set => SetPart(PartSpot.StepBottomSide, value);
        }
        public GlassStrip? CloseStrip
        {
            get => GetPartOrNull<GlassStrip>(PartSpot.CloseStrip);
            set => SetPart(PartSpot.CloseStrip, value);
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
        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value);
        }

        public Cabin9SParts()
        {

        }
        public override Cabin9SParts GetDeepClone()
        {
            return (Cabin9SParts)base.GetDeepClone();
        }
    }
}

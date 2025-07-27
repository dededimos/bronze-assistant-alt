using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels
{
    public class Cabin9CParts : CabinPartsList, IDoubleWallProfile, IHandle, IHorizontalProfile, ICloseStrip
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
        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value);
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
        public GlassStrip? CloseStrip
        {
            get => GetPartOrNull<GlassStrip>(PartSpot.CloseStrip);
            set => SetPart(PartSpot.CloseStrip, value);
        }

        public Cabin9CParts()
        {

        }
        public override Cabin9CParts GetDeepClone()
        {
            return (Cabin9CParts)base.GetDeepClone();
        }
        //public Cabin9CParts(Cabin9CParts parts)
        //{
        //    this.WallProfile1 = parts.WallProfile1?.GetDeepClone();
        //    this.WallProfile2 = parts.WallProfile2?.GetDeepClone();

        //    this.Handle = parts.Handle.GetDeepClone();

        //    this.HorizontalProfileTop = parts.HorizontalProfileTop.GetDeepClone();
        //    this.HorizontalProfileBottom = parts.HorizontalProfileBottom.GetDeepClone();
        //    this.CloseStrip = parts.CloseStrip.GetDeepClone();
        //}


    }
}

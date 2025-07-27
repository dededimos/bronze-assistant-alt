using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels
{
    public class CabinVSParts : CabinPartsList , IHandle , IWallSideFixer , ICloseProfile
    {
#nullable enable
        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value);
        }
        public CabinPart? WallSideFixer
        {
            get => GetPartOrNull(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
        }
        public SupportBar? SupportBar
        {
            get => GetPartOrNull<SupportBar>(PartSpot.SupportBar);
            set => SetPart(PartSpot.SupportBar, value);
        }
        public CabinPart? BottomFixer
        {
            get => GetPartOrNull(PartSpot.BottomSide1);
            set => SetPart(PartSpot.BottomSide1, value);
        }
        public GlassStrip? CloseStrip
        {
            get => GetPartOrNull<GlassStrip>(PartSpot.CloseStrip);
            set => SetPart(PartSpot.CloseStrip, value);
        }
        public Profile? CloseProfile
        {
            get => GetPartOrNull<Profile>(PartSpot.CloseProfile);
            set => SetPart(PartSpot.CloseProfile, value);
        }
        public Profile? HorizontalBar
        {
            get => GetPartOrNull<Profile>(PartSpot.HorizontalGuideTop);
            set => SetPart(PartSpot.HorizontalGuideTop, value);
        }

        public CabinVSParts()
        {

        }
        public override CabinVSParts GetDeepClone()
        {
            return (CabinVSParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinVSParts(CabinVSParts parts)
        //{
        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();
        //    this.CloseProfile = parts.CloseProfile?.GetDeepClone();
        //    this.Handle = parts.Handle?.GetDeepClone();
        //    this.WallSideFixer = parts.WallSideFixer?.GetDeepClone();
        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //    this.BottomFixer = parts.BottomFixer?.GetDeepClone();
        //    this.HorizontalBar = parts.HorizontalBar?.GetDeepClone();
        //}
    }
}

using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels
{
    public class CabinWSParts : CabinPartsList , IHandle , ICloseProfile
    {
#nullable enable
        public Profile? WallFixer
        {
            get => GetPartOrNull<Profile>(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
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

        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value);
        }

        public SupportBar? SupportBar
        {
            get => GetPartOrNull<SupportBar>(PartSpot.SupportBar);
            set => SetPart(PartSpot.SupportBar, value);
        }

        public CabinWSParts()
        {

        }
        public override CabinWSParts GetDeepClone()
        {
            return (CabinWSParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinWSParts(CabinWSParts parts)
        //{
        //    this.WallFixer = parts.WallFixer?.GetDeepClone();

        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();

        //    this.CloseProfile = parts.CloseProfile?.GetDeepClone();

        //    this.Handle = parts.Handle?.GetDeepClone();

        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //}
    }
}

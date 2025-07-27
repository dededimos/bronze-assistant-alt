using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels
{
    public class CabinHBParts : CabinPartsList , IBottomFixer , IHandle , ICloseProfile , IWallSideFixer
    {
#nullable enable
        public GlassToGlassHinge? Hinge
        {
            get => GetPartOrNull<GlassToGlassHinge>(PartSpot.MiddleHinge);
            set => SetPart(PartSpot.MiddleHinge, value);
        }
        public CabinPart? WallSideFixer
        {
            get => GetPartOrNull(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
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
        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value);
        }
        public Profile? CloseProfile
        {
            get => GetPartOrNull<Profile>(PartSpot.CloseProfile);
            set => SetPart(PartSpot.CloseProfile, value);
        }
        public SupportBar? SupportBar
        {
            get => GetPartOrNull<SupportBar>(PartSpot.SupportBar);
            set => SetPart(PartSpot.SupportBar, value);
        }

        public CabinHBParts()
        {

        }
        public override CabinHBParts GetDeepClone()
        {
            return (CabinHBParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinHBParts(CabinHBParts parts)
        //{
        //    this.Hinge = parts.Hinge?.GetDeepClone();

        //    this.WallSideFixer = parts.WallSideFixer?.GetDeepClone();

        //    this.BottomFixer = parts.BottomFixer?.GetDeepClone();

        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();

        //    this.CloseProfile = parts.CloseProfile?.GetDeepClone();

        //    this.Handle = parts.Handle?.GetDeepClone();            
        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //}

    }
}

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
    public class CabinV4Parts : CabinPartsList , IHandle , IWallSideFixer
    {
#nullable enable
        public GlassStrip? CloseStrip
        {
            get => GetPartOrNull<GlassStrip>(PartSpot.CloseStrip);
            set => SetPart(PartSpot.CloseStrip, value);
        }
        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value)
                  .SetPart(PartSpot.Handle2, value);
        }
        public CabinPart? WallSideFixer
        {
            get => GetPartOrNull(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
        }
        public CabinPart? WallFixer2
        {
            get => GetPartOrNull(PartSpot.WallSide2);
            set => SetPart(PartSpot.WallSide2, value);
        }
        public SupportBar? SupportBar
        {
            get => GetPartOrNull<SupportBar>(PartSpot.SupportBar);
            set => SetPart(PartSpot.SupportBar, value);
        }
        public CabinPart? BottomFixer1
        {
            get => GetPartOrNull(PartSpot.BottomSide1);
            set => SetPart(PartSpot.BottomSide1, value);
        }
        public CabinPart? BottomFixer2
        {
            get => GetPartOrNull(PartSpot.BottomSide2);
            set => SetPart(PartSpot.BottomSide2, value);
        }
        public Profile? HorizontalBar
        {
            get => GetPartOrNull<Profile>(PartSpot.HorizontalGuideTop);
            set => SetPart(PartSpot.HorizontalGuideTop, value);
        }

        public CabinV4Parts()
        {

        }
        public override CabinV4Parts GetDeepClone()
        {
            return (CabinV4Parts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinV4Parts(CabinV4Parts parts)
        //{
        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();
        //    this.Handle = parts.Handle?.GetDeepClone();
        //    this.WallSideFixer = parts.WallSideFixer?.GetDeepClone();
        //    this.WallFixer2 = parts.WallFixer2?.GetDeepClone();
        //    this.BottomFixer1 = parts.BottomFixer1?.GetDeepClone();
        //    this.BottomFixer2 = parts.BottomFixer2?.GetDeepClone();
        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //    this.HorizontalBar = parts.HorizontalBar?.GetDeepClone();
        //}

    }
}

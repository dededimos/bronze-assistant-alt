using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels
{
    public class CabinWParts : CabinPartsList , IPerimetricalFixer
    {
        #nullable enable
        public CabinPart? WallSideFixer 
        {
            get => GetPartOrNull(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
        }
        public CabinPart? TopFixer
        {
            get => HasSpot(PartSpot.TopSide) ? GetPartOrNull(PartSpot.TopSide) : null;
            set => SetPart(PartSpot.TopSide, value);
        }
        public CabinPart? BottomFixer
        {
            get => GetPartOrNull(PartSpot.BottomSide1);
            set => SetPart(PartSpot.BottomSide1, value);
        }
        public CabinPart? SideFixer
        {
            get => HasSpot(PartSpot.NonWallSide) ? GetPartOrNull(PartSpot.NonWallSide) : null;
            set => SetPart(PartSpot.NonWallSide, value);
        }
        public SupportBar? SupportBar
        {
            get => GetPartOrNull<SupportBar>(PartSpot.SupportBar);
            set => SetPart(PartSpot.SupportBar, value);
        }
        public GlassStrip? CloseStrip
        {
            get => HasSpot(PartSpot.CloseStrip) ? GetPartOrNull<GlassStrip>(PartSpot.CloseStrip) : null;
            set => SetPart(PartSpot.CloseStrip, value);
        }


        public CabinWParts()
        {

        }
        public override CabinWParts GetDeepClone()
        {
            return (CabinWParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinWParts(CabinWParts parts)
        //{
        //    this.WallSideFixer = parts.WallSideFixer?.GetDeepClone();

        //    this.TopFixer = parts.TopFixer?.GetDeepClone();

        //    this.BottomFixer = parts.BottomFixer?.GetDeepClone();
        //    this.SideFixer = parts.SideFixer?.GetDeepClone();
        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();
        //}

    }
}

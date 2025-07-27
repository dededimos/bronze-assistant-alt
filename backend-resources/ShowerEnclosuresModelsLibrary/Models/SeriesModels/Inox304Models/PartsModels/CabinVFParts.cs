using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels
{
    public class CabinVFParts : CabinPartsList , IWallSideFixer
    {
#nullable enable
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
        public CabinPart? SideFixer
        {
            get => GetPartOrNull(PartSpot.NonWallSide);
            set => SetPart(PartSpot.NonWallSide, value);
        }
        public SupportBar? SupportBar
        {
            get => GetPartOrNull<SupportBar>(PartSpot.SupportBar);
            set => SetPart(PartSpot.SupportBar, value);
        }

        public CabinVFParts()
        {
            
        }
        public override CabinVFParts GetDeepClone()
        {
            return (CabinVFParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinVFParts(CabinVFParts parts)
        //{
        //    this.WallSideFixer = parts.WallSideFixer?.GetDeepClone();
        //    this.BottomFixer = parts.BottomFixer?.GetDeepClone();
        //    this.SideFixer = parts.SideFixer?.GetDeepClone();
        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //}
    }
}

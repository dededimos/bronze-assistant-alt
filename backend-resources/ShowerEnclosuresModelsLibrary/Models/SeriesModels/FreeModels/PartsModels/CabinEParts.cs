using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels
{
    public class CabinEParts : CabinPartsList , IBottomFixer , ISupportBar
    {
#nullable enable
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

        public CabinEParts()
        {

        }
        public override CabinEParts GetDeepClone()
        {
            return (CabinEParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinEParts(CabinEParts parts)
        //{
        //    this.SupportBar = parts.SupportBar?.GetDeepClone();
        //    this.BottomFixer = parts.BottomFixer?.GetDeepClone();
        //}

    }
}

using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels
{
    public class CabinWFlipperParts : CabinPartsList
    {
#nullable enable
        public GlassToGlassHinge? Hinge 
        {
            get => GetPartOrNull<GlassToGlassHinge>(PartSpot.MiddleHinge);
            set => SetPart(PartSpot.MiddleHinge, value);
        }

        public CabinWFlipperParts()
        {

        }
        public override CabinWFlipperParts GetDeepClone()
        {
            return (CabinWFlipperParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinWFlipperParts(CabinWFlipperParts parts)
        //{
        //    this.Hinge = parts.Hinge?.GetDeepClone();
        //}

    }
}

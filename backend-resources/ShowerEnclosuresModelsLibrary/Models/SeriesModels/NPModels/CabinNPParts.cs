using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels
{
    public class CabinNPParts : CabinPartsList , IMiddleHinge , IHandle , ICloseProfile
    {
#nullable enable
        public ProfileHinge? WallHinge
        {
            get => GetPartOrNull<ProfileHinge>(PartSpot.WallHinge);
            set => SetPart(PartSpot.WallHinge, value);
        }
        public CabinPart? MiddleHinge
        {
            get => GetPartOrNull(PartSpot.MiddleHinge);
            set => SetPart(PartSpot.MiddleHinge, value);
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

        public CabinNPParts()
        {

        }
        public override CabinNPParts GetDeepClone()
        {
            return (CabinNPParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinNPParts(CabinNPParts parts)
        //{
        //    this.WallHinge = parts.WallHinge?.GetDeepClone();

        //    this.MiddleHinge = parts.MiddleHinge?.GetDeepClone();

        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();

        //    this.CloseProfile = parts.CloseProfile?.GetDeepClone();

        //    this.Handle = parts.Handle?.GetDeepClone();
        //}

    }
}

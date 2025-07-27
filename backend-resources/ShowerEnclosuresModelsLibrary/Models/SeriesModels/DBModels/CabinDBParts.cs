using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels
{
    public class CabinDBParts : CabinPartsList , ICloseProfile , ICabinHinge<HingeDB> , IHandle
    {
#nullable enable
        public HingeDB? Hinge 
        {
            get => GetPartOrNull<HingeDB>(PartSpot.WallHinge);
            set => SetPart(PartSpot.WallHinge, value);
        }
        public Profile? CloseProfile 
        { 
            get => GetPartOrNull<Profile>(PartSpot.CloseProfile);
            set => SetPart(PartSpot.CloseProfile, value);
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

        CabinHinge? ICabinHinge.Hinge 
        { 
            get => Hinge;
            set 
            {
                if (value is HingeDB hingeDB)
                {
                    Hinge = hingeDB;
                }
                else
                {
                    throw new NotSupportedException($"{nameof(CabinDBParts)} supports hinges only of type {nameof(HingeDB)}");
                }
            } 
        }
        public CabinDBParts()
        {

        }
        public override CabinDBParts GetDeepClone()
        {
            return (CabinDBParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public CabinDBParts(CabinDBParts parts)
        //{
        //    this.Hinge = parts.Hinge?.GetDeepClone() as HingeDB;

        //    this.CloseProfile = parts.CloseProfile?.GetDeepClone();

        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();

        //    this.Handle = parts.Handle?.GetDeepClone();
        //}
    }
}

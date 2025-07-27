using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels
{
    public class Cabin9BParts : CabinPartsList , IDoubleWallProfile , IHandle , ICloseStrip , ICabinHinge<Hinge9B>
    {
        #nullable enable
        public Profile? WallProfile1
        {
            get => GetPartOrNull<Profile>(PartSpot.WallSide1);
            set => SetPart(PartSpot.WallSide1, value);
        }
        public Profile? WallProfile2
        {
            get => GetPartOrNull<Profile>(PartSpot.WallSide2);
            set => SetPart(PartSpot.WallSide2, value);
        }
        /// <summary>
        /// The Profile that is put in the First Part of the step (Not an LO Profile)
        /// </summary>
        public Profile? StepBottomProfile
        {
            get => GetPartOrNull<Profile>(PartSpot.StepBottomSide);
            set => SetPart(PartSpot.StepBottomSide, value);
        }
        public GlassStrip? CloseStrip
        {
            get => GetPartOrNull<GlassStrip>(PartSpot.CloseStrip);
            set => SetPart(PartSpot.CloseStrip, value);
        }
        public Profile? HorizontalProfileTop
        {
            get => GetPartOrNull<Profile>(PartSpot.HorizontalGuideTop);
            set => SetPart(PartSpot.HorizontalGuideTop, value);
        }
        public Profile? HorizontalProfileBottom
        {
            get => GetPartOrNull<Profile>(PartSpot.HorizontalGuideBottom);
            set => SetPart(PartSpot.HorizontalGuideBottom, value);
        }
        public CabinHandle? Handle
        {
            get => GetPartOrNull<CabinHandle>(PartSpot.Handle1);
            set => SetPart(PartSpot.Handle1, value);
        }
        public Hinge9B? Hinge 
        {
            get => GetPartOrNull<Hinge9B>(PartSpot.PivotHinge);
            set => SetPart(PartSpot.PivotHinge, value);
        }
        CabinHinge? ICabinHinge.Hinge 
        { 
            get => Hinge; 
            set 
            {
                if (value is Hinge9B hinge9B)
                {
                    Hinge = hinge9B;
                }
                else
                {
                    throw new NotSupportedException($"{nameof(Cabin9BParts)} supports hinges only of type {nameof(Hinge9B)}");
                }
            } 
        }

        public Cabin9BParts()
        {

        }
        public override Cabin9BParts GetDeepClone()
        {
            return (Cabin9BParts)base.GetDeepClone();
        }
        ///// <summary>
        ///// Instantiates this Part's instance by deep cloning another one
        ///// </summary>
        ///// <param name="parts">The Parts to Clone</param>
        //public Cabin9BParts(Cabin9BParts parts)
        //{
        //    this.WallProfile1 = parts.WallProfile1?.GetDeepClone();
        //    this.WallProfile2 = parts.WallProfile2?.GetDeepClone();

        //    this.CloseStrip = parts.CloseStrip?.GetDeepClone();

        //    this.HorizontalProfileTop = parts.HorizontalProfileTop?.GetDeepClone();

        //    this.Handle = parts.Handle?.GetDeepClone();

        //    this.Hinge = parts.Hinge?.GetDeepClone();
        //    this.StepBottomProfile = parts.StepBottomProfile?.GetDeepClone();
        //    this.HorizontalProfileBottom = parts.HorizontalProfileBottom?.GetDeepClone();
        //}


    }
}

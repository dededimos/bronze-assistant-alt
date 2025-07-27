using DataAccessLib.NoSQLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class EditProfileViewModel : EditPartViewModel
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProfileHinge))]
        private CabinProfileType profileType;

        [ObservableProperty]
        private int glassInProfileDepth;
        [ObservableProperty]
        private int sliderDistance;
        [ObservableProperty]
        private int glassCavityThickness;
        [ObservableProperty]
        private int placementWidth;
        [ObservableProperty]
        private double cutLength;
        [ObservableProperty]
        private double cutLengthStepPart;
        [ObservableProperty]
        private int thicknessView;
        [ObservableProperty]
        private int innerThicknessView;
        [ObservableProperty]
        private int tollerance;
        [ObservableProperty]
        private string containedStripCode = string.Empty;
        [ObservableProperty]
        private int sideTollerance;

        //Profile Hinge Specific Props
        [ObservableProperty]
        private int topHeightAboveGlass;
        [ObservableProperty]
        private int bottomHeightBelowGlass;

        [ObservableProperty]
        private IEnumerable<string> selectableContainedStrips = new List<string>();

        /// <summary>
        /// Weather the New or Edited profile is a Profile Hinge
        /// </summary>
        public bool IsProfileHinge { get => ProfileType is CabinProfileType.HingeProfile; }

        public EditProfileViewModel(bool isHingeProfile = false):base(isHingeProfile ? CabinPartType.ProfileHinge : CabinPartType.Profile)
        {
            
        }

        /// <summary>
        /// Constructs a Profile View Model for Editions 
        /// </summary>
        /// <param name="entity">The Entity from which to extract the Profile for edition</param>
        /// <param name="selecteableContainedStrips">The Available selectable strips to be attached to the Profile - Only Valid for Close Profiles</param>
        /// <exception cref="ArgumentException">When the passed entity Does not Represent a Profile</exception>
        public EditProfileViewModel(CabinPartEntity entity , IEnumerable<string>? selectableContainedStrips, bool isEdit = true) : base(entity,isEdit)
        {
            Profile profile = entity.Part as Profile ?? throw new ArgumentException($"{nameof(EditProfileViewModel)} accepts Only CabinPartEntities of a type {nameof(Profile)}");
            InitilizeProfileViewmodel(profile,selectableContainedStrips);
        }
        private void InitilizeProfileViewmodel(Profile part , IEnumerable<string>? selectableContainedStrips)
        {
            this.ProfileType = part.ProfileType;
            this.GlassInProfileDepth = part.GlassInProfileDepth;
            this.SliderDistance = part.SliderDistance;
            this.GlassCavityThickness = part.GlassCavityThickness;
            this.PlacementWidth = part.PlacementWidth;
            this.CutLength = part.CutLength;
            this.CutLengthStepPart = part.CutLengthStepPart;
            this.ThicknessView = part.ThicknessView;
            this.InnerThicknessView = part.InnerThicknessView;
            this.Tollerance = part.Tollerance;
            this.SideTollerance = part.SideTollerance;
            this.ContainedStripCode = part.ContainedStripCode;

            if (part is ProfileHinge hp)
            {
                this.TopHeightAboveGlass = hp.TopHeightAboveGlass;
                this.BottomHeightBelowGlass = hp.BottomHeightBelowGlass;
            }

            //If the passed strip codes that can be selected are not null and not empty => give them to the viewmodel
            //otherwise set only one as selectable , (the one present)
            //otherwise leave the list empty
            if (selectableContainedStrips is not null && selectableContainedStrips.Any())
            {
                this.SelectableContainedStrips = new List<string>(selectableContainedStrips);
            }
            else if (!string.IsNullOrWhiteSpace(ContainedStripCode))
            {
                this.SelectableContainedStrips = new List<string>() { ContainedStripCode };
            }

        }

        public override Profile GetPart()
        {
            Profile profile = ProfileType switch
            {
                CabinProfileType.HingeProfile => new ProfileHinge(PartType, ProfileType)
                {
                    TopHeightAboveGlass = this.TopHeightAboveGlass,
                    BottomHeightBelowGlass = this.BottomHeightBelowGlass,
                },
                _ => new Profile(PartType, ProfileType),
            };

            profile.GlassInProfileDepth     = this.GlassInProfileDepth;
            profile.SliderDistance          = this.SliderDistance;
            profile.GlassCavityThickness    = this.GlassCavityThickness;
            profile.PlacementWidth          = this.PlacementWidth;
            profile.CutLength               = this.CutLength;
            profile.CutLengthStepPart       = this.CutLengthStepPart;
            profile.ThicknessView           = this.ThicknessView;
            profile.InnerThicknessView      = this.InnerThicknessView;
            profile.Tollerance              = this.Tollerance;
            profile.ContainedStripCode      = this.ContainedStripCode;
            profile.SideTollerance          = this.SideTollerance;

            ExtractPropertiesForBasePart(profile);

            return profile;
        }
    }
}

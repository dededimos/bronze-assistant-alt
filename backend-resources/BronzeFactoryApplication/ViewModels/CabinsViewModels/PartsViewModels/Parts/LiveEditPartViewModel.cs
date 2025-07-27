using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class LiveEditPartViewModel : BaseViewModel
    {
        private readonly CabinPart part;
        private readonly CabinPart _undoStore;

        public string Code
        {
            get => part.Code;
            set 
            {
                if (CanEditCode)
                {
                    SetProperty(part.Code, value, part, (p, c) => p.Code = c);
                }
            }
        }

        [ObservableProperty]
        private bool canEditCode;

        public string Description { get => part.Description; set => SetProperty(part.Description, value, part, (m, v) => m.Description = v); }
        public string PhotoPath { get => part.PhotoPath; set => SetProperty(part.PhotoPath, value, part, (m, v) => m.PhotoPath = v); }
        public CabinPartType PartType { get => part.Part; }
        public MaterialType Material { get => part.Material; set => SetProperty(part.Material, value, part, (m, v) => m.Material = v); }

        public LiveEditPartViewModel(CabinPart part)
        {
            this.part = part;
            this._undoStore = this.part.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditPartViewModel()
        {
            this.part = new();
            this._undoStore = this.part.GetDeepClone();
        }

        /// <summary>
        /// Weather any changes where made to the Part
        /// </summary>
        /// <returns></returns>
        public virtual bool HasChanges()
        {
            return part.Code != _undoStore.Code ||
                part.Description != _undoStore.Description ||
                part.PhotoPath != _undoStore.PhotoPath ||
                part.Part != _undoStore.Part ||
                part.Material != _undoStore.Material;
        }

    }

    public partial class LiveEditAngleViewModel : LiveEditPartViewModel
    {
        private readonly CabinAngle angle;
        private readonly CabinAngle _undoStore;

        public int AngleDistanceFromDoor { get => angle.AngleDistanceFromDoor; set => SetProperty(angle.AngleDistanceFromDoor, value, angle, (m, v) => m.AngleDistanceFromDoor = v); }
        public int AngleLengthL0 { get => angle.AngleLengthL0; set => SetProperty(angle.AngleLengthL0, value, angle, (m, v) => m.AngleLengthL0 = v); }
        public int AngleHeight { get => angle.AngleHeight; set => SetProperty(angle.AngleHeight, value, angle, (m, v) => m.AngleHeight = v); }

        public LiveEditAngleViewModel(CabinAngle angle) : base(angle)
        {
            this.angle = angle;
            _undoStore = angle.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditAngleViewModel() : base() { angle = new();_undoStore = angle.GetDeepClone(); }

        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                angle.AngleDistanceFromDoor != _undoStore.AngleDistanceFromDoor ||
                angle.AngleLengthL0 != _undoStore.AngleLengthL0 ||
                angle.AngleHeight != _undoStore.AngleHeight);
        }
    }

    public partial class LiveEditHandleViewModel : LiveEditPartViewModel
    {
        private readonly CabinHandle handle;
        private readonly CabinHandle _undoStore;

        public int HandleLengthToGlass { get => handle.HandleLengthToGlass; set => SetProperty(handle.HandleLengthToGlass, value, handle, (m, v) => m.HandleLengthToGlass = v); }
        public int HandleHeightToGlass { get => handle.HandleHeightToGlass; set => SetProperty(handle.HandleHeightToGlass, value, handle, (m, v) => m.HandleHeightToGlass = v); }
        public double HandleEdgesCornerRadius { get => handle.HandleEdgesCornerRadius; set => SetProperty(handle.HandleEdgesCornerRadius, value, handle, (m, v) => m.HandleEdgesCornerRadius = v); }
        public double HandleOuterThickness { get => handle.HandleOuterThickness; set => SetProperty(handle.HandleOuterThickness, value, handle, (m, v) => m.HandleOuterThickness = v); }
        public int HandleComfortDistance { get => handle.HandleComfortDistance; set => SetProperty(handle.HandleComfortDistance, value, handle, (m, v) => m.HandleComfortDistance = v); }
        public int MinimumDistanceFromEdge { get => handle.MinimumDistanceFromEdge; set => SetProperty(handle.MinimumDistanceFromEdge, value, handle, (m, v) => m.MinimumDistanceFromEdge = v); }
        public bool IsCircularHandle { get => handle.IsCircularHandle; }
        public CabinHandleType HandleType { get => handle.HandleType; }

        public LiveEditHandleViewModel(CabinHandle handle):base(handle)
        {
            this.handle = handle;
            _undoStore = handle.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditHandleViewModel() : base() { handle = new(CabinHandleType.Generic); _undoStore = handle.GetDeepClone(); }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                handle.HandleLengthToGlass != _undoStore.HandleLengthToGlass ||
                handle.HandleHeightToGlass != _undoStore.HandleHeightToGlass ||
                handle.HandleEdgesCornerRadius != _undoStore.HandleEdgesCornerRadius ||
                handle.HandleOuterThickness != _undoStore.HandleOuterThickness ||
                handle.HandleComfortDistance != _undoStore.HandleComfortDistance ||
                handle.MinimumDistanceFromEdge != _undoStore.MinimumDistanceFromEdge);
        }
    }

    public partial class LiveEditHingeViewModel : LiveEditPartViewModel
    {
        private readonly CabinHinge hinge;
        private readonly CabinHinge _undoStore;
        public int LengthView { get => hinge.LengthView; set => SetProperty(hinge.LengthView, value, hinge, (m, v) => m.LengthView = v); }
        public int HeightView { get => hinge.HeightView; set => SetProperty(hinge.HeightView, value, hinge, (m, v) => m.HeightView = v); }
        public int GlassGapAER { get => hinge.GlassGapAER; set => SetProperty(hinge.GlassGapAER, value, hinge, (m, v) => m.GlassGapAER = v); }
        public CabinHingeType HingeType { get => hinge.HingeType; }

        public LiveEditHingeViewModel(CabinHinge hinge) : base(hinge)
        {
            this.hinge = hinge;
            _undoStore = hinge.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditHingeViewModel() : base() { hinge = new(); _undoStore = hinge.GetDeepClone(); }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                hinge.LengthView != _undoStore.LengthView ||
                hinge.HeightView != _undoStore.HeightView ||
                hinge.GlassGapAER != _undoStore.GlassGapAER ||
                hinge.HingeType != _undoStore.HingeType);
        }
    }

    public partial class LiveEditGlassToGlassHingeViewModel : LiveEditHingeViewModel
    {
        private readonly GlassToGlassHinge hinge;
        private readonly GlassToGlassHinge _undoStore;

        public int InDoorLength { get => hinge.InDoorLength; set => SetProperty(hinge.InDoorLength, value, hinge, (m, v) => m.InDoorLength = v); }

        public LiveEditGlassToGlassHingeViewModel(GlassToGlassHinge hinge) : base(hinge)
        {
            this.hinge = hinge;
            _undoStore = hinge.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditGlassToGlassHingeViewModel() : base()
        {
            hinge = new();
            _undoStore = hinge.GetDeepClone();
        }

        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                hinge.InDoorLength != _undoStore.InDoorLength);
        }
    }

    public partial class LiveEditHinge9BHingeViewModel : LiveEditHingeViewModel
    {
        private readonly Hinge9B hinge;
        private readonly Hinge9B _undoStore;

        public int HingeOverlappingHeight { get => hinge.HingeOverlappingHeight; set => SetProperty(hinge.HingeOverlappingHeight, value, hinge, (m, v) => m.HingeOverlappingHeight = v); }
        public int SupportTubeLength { get => hinge.SupportTubeLength; set => SetProperty(hinge.SupportTubeLength, value, hinge, (m, v) => m.SupportTubeLength = v); }
        public int SupportTubeHeight { get => hinge.SupportTubeHeight; set => SetProperty(hinge.SupportTubeHeight, value, hinge, (m, v) => m.SupportTubeHeight = v); }
        public int CornerRadiusInGlass { get => hinge.CornerRadiusInGlass; set => SetProperty(hinge.CornerRadiusInGlass, value, hinge, (m, v) => m.CornerRadiusInGlass = v); }

        public LiveEditHinge9BHingeViewModel(Hinge9B hinge) : base(hinge)
        {
            this.hinge = hinge;
            _undoStore = hinge.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditHinge9BHingeViewModel() : base()
        {
            hinge = new();
            _undoStore = hinge.GetDeepClone();
        }

        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                hinge.HingeOverlappingHeight != _undoStore.HingeOverlappingHeight ||
                hinge.SupportTubeLength != _undoStore.SupportTubeLength ||
                hinge.SupportTubeHeight != _undoStore.SupportTubeHeight ||
                hinge.CornerRadiusInGlass != _undoStore.CornerRadiusInGlass);
        }
    }

    public partial class LiveEditHingeDBViewModel : LiveEditHingeViewModel
    {
        private readonly HingeDB hinge;
        private readonly HingeDB _undoStore;
        public int InnerHeight { get => hinge.InnerHeight; set => SetProperty(hinge.InnerHeight, value, hinge, (m, v) => m.InnerHeight = v); }
        public int WallPlateThicknessView { get => hinge.WallPlateThicknessView; set => SetProperty(hinge.WallPlateThicknessView, value, hinge, (m, v) => m.WallPlateThicknessView = v); }

        public LiveEditHingeDBViewModel(HingeDB hinge) : base(hinge)
        {
            this.hinge = hinge;
            _undoStore = hinge.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditHingeDBViewModel() : base()
        {
            hinge = new();
            _undoStore = hinge.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                hinge.InnerHeight != _undoStore.InnerHeight ||
                hinge.WallPlateThicknessView != _undoStore.WallPlateThicknessView);
        }
    }

    public partial class LiveEditProfileViewModel : LiveEditPartViewModel
    {
        private readonly Profile profile;
        private readonly Profile _undoStore;
        public int GlassInProfileDepth { get => profile.GlassInProfileDepth; set => SetProperty(profile.GlassInProfileDepth, value, profile, (m, v) => m.GlassInProfileDepth = v); }
        public int SliderDistance { get => profile.SliderDistance; set => SetProperty(profile.SliderDistance, value, profile, (m, v) => m.SliderDistance = v); }
        public int GlassCavityThickness { get => profile.GlassCavityThickness; set => SetProperty(profile.GlassCavityThickness, value, profile, (m, v) => m.GlassCavityThickness = v); }
        public int PlacementWidth { get => profile.PlacementWidth; set => SetProperty(profile.PlacementWidth, value, profile, (m, v) => m.PlacementWidth = v); }
        public double CutLength { get => profile.CutLength; set => SetProperty(profile.CutLength, value, profile, (m, v) => m.CutLength = v); }
        public double CutLengthStepPart { get => profile.CutLengthStepPart; set => SetProperty(profile.CutLengthStepPart, value, profile, (m, v) => m.CutLengthStepPart = v); }
        public int ThicknessView { get => profile.ThicknessView; set => SetProperty(profile.ThicknessView, value, profile, (m, v) => m.ThicknessView = v); }
        public int InnerThicknessView { get => profile.InnerThicknessView; set => SetProperty(profile.InnerThicknessView, value, profile, (m, v) => m.InnerThicknessView = v); }
        public int Tollerance { get => profile.Tollerance; set => SetProperty(profile.Tollerance, value, profile, (m, v) => m.Tollerance = v); }
        public int SideTollerance { get => profile.SideTollerance; set => SetProperty(profile.SideTollerance, value, profile, (m, v) => m.SideTollerance = v); }
        public string ContainedStripCode { get => profile.ContainedStripCode; set => SetProperty(profile.ContainedStripCode, value, profile, (m, v) => m.ContainedStripCode = v); }
        public CabinProfileType ProfileType { get => profile.ProfileType; }

        public LiveEditProfileViewModel(Profile profile) : base(profile)
        {
            this.profile = profile;
            _undoStore = profile.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditProfileViewModel() : base()
        {
            profile = new(CabinPartType.Profile,CabinProfileType.WallProfile);
            _undoStore = profile.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                profile.GlassInProfileDepth != _undoStore.GlassInProfileDepth ||
                profile.SliderDistance != _undoStore.SliderDistance ||
                profile.GlassCavityThickness != _undoStore.GlassCavityThickness ||
                profile.PlacementWidth != _undoStore.PlacementWidth ||
                profile.CutLength != _undoStore.CutLength ||
                profile.CutLengthStepPart != _undoStore.CutLengthStepPart ||
                profile.ThicknessView != _undoStore.ThicknessView ||
                profile.InnerThicknessView != _undoStore.InnerThicknessView ||
                profile.Tollerance != _undoStore.Tollerance ||
                profile.SideTollerance != _undoStore.SideTollerance ||
                profile.ContainedStripCode != _undoStore.ContainedStripCode);
        }
    }

    public partial class LiveEditProfileHingeViewModel : LiveEditProfileViewModel
    {
        private readonly ProfileHinge profile;
        private readonly ProfileHinge _undoStore;
        public int TopHeightAboveGlass { get => profile.TopHeightAboveGlass; set => SetProperty(profile.TopHeightAboveGlass, value, profile, (m, v) => m.TopHeightAboveGlass = v); }
        public int BottomHeightBelowGlass { get => profile.BottomHeightBelowGlass; set => SetProperty(profile.BottomHeightBelowGlass, value, profile, (m, v) => m.BottomHeightBelowGlass = v); }
        public LiveEditProfileHingeViewModel(ProfileHinge profile) : base(profile)
        {
            this.profile = profile;
            _undoStore = profile.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditProfileHingeViewModel() :base()
        {
            profile = new(CabinPartType.ProfileHinge,CabinProfileType.HingeProfile);
            _undoStore = profile.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                profile.TopHeightAboveGlass != _undoStore.TopHeightAboveGlass ||
                profile.BottomHeightBelowGlass != _undoStore.BottomHeightBelowGlass);
        }
    }

    public partial class LiveEditStripViewModel : LiveEditPartViewModel
    {
        private readonly GlassStrip strip;
        private readonly GlassStrip _undoStore;
        public CabinStripType StripType { get => strip.StripType; }
        public double CutLength { get => strip.CutLength; set => SetProperty(strip.CutLength, value, strip, (m, v) => m.CutLength = v); }
        public int OutOfGlassLength { get => strip.OutOfGlassLength; set => SetProperty(strip.OutOfGlassLength, value, strip, (m, v) => m.OutOfGlassLength = v); }
        public int InGlassLength { get => strip.InGlassLength; set => SetProperty(strip.InGlassLength, value, strip, (m, v) => m.InGlassLength = v); }
        public int MetalLength { get => strip.MetalLength; set => SetProperty(strip.MetalLength, value, strip, (m, v) => m.MetalLength = v); }
        public int PolycarbonicLength { get => strip.PolycarbonicLength; set => SetProperty(strip.PolycarbonicLength, value, strip, (m, v) => m.PolycarbonicLength = v); }

        public LiveEditStripViewModel(GlassStrip strip) : base(strip)
        {
            this.strip = strip;
            _undoStore = strip.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditStripViewModel() :base()
        {
            strip = new(CabinStripType.GenericPolycarbonic);
            _undoStore = strip.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                strip.CutLength != _undoStore.CutLength ||
                strip.OutOfGlassLength != _undoStore.OutOfGlassLength ||
                strip.InGlassLength != _undoStore.InGlassLength ||
                strip.MetalLength != _undoStore.MetalLength ||
                strip.PolycarbonicLength != _undoStore.PolycarbonicLength);
        }
    }

    public partial class LiveEditSupportBarViewModel : LiveEditPartViewModel
    {
        private readonly SupportBar support;
        private readonly SupportBar _undoStore;
        public int OutOfGlassHeight { get => support.OutOfGlassHeight; set => SetProperty(support.OutOfGlassHeight, value, support, (m, v) => m.OutOfGlassHeight = v); }
        public SupportBarPlacement Placement { get => support.Placement; set => SetProperty(support.Placement, value, support, (m, v) => m.Placement = v); }
        public int ClampViewLength { get => support.ClampViewLength; set => SetProperty(support.ClampViewLength, value, support, (m, v) => m.ClampViewLength = v); }
        public int ClampViewHeight { get => support.ClampViewHeight; set => SetProperty(support.ClampViewHeight, value, support, (m, v) => m.ClampViewHeight = v); }
        public double ClampCenterDistanceFromGlassDefault { get => support.ClampCenterDistanceFromGlassDefault; set => SetProperty(support.ClampCenterDistanceFromGlassDefault, value, support, (m, v) => m.ClampCenterDistanceFromGlassDefault = v); }
        public double ClampCenterDistanceFromGlass { get => support.ClampCenterDistanceFromGlass; set => SetProperty(support.ClampCenterDistanceFromGlass, value, support, (m, v) => m.ClampCenterDistanceFromGlass = v); }


        public LiveEditSupportBarViewModel(SupportBar support) : base(support)
        {
            this.support = support;
            _undoStore = support.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditSupportBarViewModel() : base()
        {
            support = new();
            _undoStore = support.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                support.OutOfGlassHeight != _undoStore.OutOfGlassHeight ||
                support.Placement != _undoStore.Placement ||
                support.ClampViewLength != _undoStore.ClampViewLength ||
                support.ClampViewHeight != _undoStore.ClampViewHeight ||
                support.ClampCenterDistanceFromGlassDefault != _undoStore.ClampCenterDistanceFromGlassDefault ||
                support.ClampCenterDistanceFromGlass != _undoStore.ClampCenterDistanceFromGlass);
        }
    }

    public partial class LiveEditSupportViewModel : LiveEditPartViewModel
    {
        private readonly CabinSupport support;
        private readonly CabinSupport _undoStore;
        public int LengthView { get => support.LengthView; set => SetProperty(support.LengthView, value, support, (m, v) => m.LengthView = v); }
        public int HeightView { get => support.HeightView; set => SetProperty(support.HeightView, value, support, (m, v) => m.HeightView = v); }
        public int GlassGapAER { get => support.GlassGapAER; set => SetProperty(support.GlassGapAER, value, support, (m, v) => m.GlassGapAER = v); }
        public int Tollerance { get => support.Tollerance; set => SetProperty(support.Tollerance, value, support, (m, v) => m.Tollerance = v); }

        public LiveEditSupportViewModel(CabinSupport support) : base(support)
        {
            this.support = support;
            _undoStore = support.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditSupportViewModel() :base()
        {
            support = new();
            _undoStore = support.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                support.LengthView != _undoStore.LengthView ||
                support.HeightView != _undoStore.HeightView ||
                support.GlassGapAER != _undoStore.GlassGapAER ||
                support.Tollerance != _undoStore.Tollerance);
        }
    }

    public partial class LiveEditFloorStopperViewModel : LiveEditSupportViewModel
    {
        private readonly FloorStopperW stopper;
        private readonly FloorStopperW _undoStore;
        public int OutOfGlassLength { get => stopper.OutOfGlassLength; set => SetProperty(stopper.OutOfGlassLength, value, stopper, (m, v) => m.OutOfGlassLength = v); }
        public LiveEditFloorStopperViewModel(FloorStopperW stopper) : base(stopper)
        {
            this.stopper = stopper;
            _undoStore = stopper.GetDeepClone();
        }
        /// <summary>
        /// Only For Design Time Purposes
        /// </summary>
        public LiveEditFloorStopperViewModel() : base()
        {
            stopper = new();
            _undoStore = stopper.GetDeepClone();
        }
        public override bool HasChanges()
        {
            return (base.HasChanges() ||
                stopper.OutOfGlassLength != _undoStore.OutOfGlassLength);
        }
    }

}

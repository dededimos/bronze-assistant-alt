using BronzeRulesPricelistLibrary.Models;
using FluentValidation.Results;
using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models;
using ShowerEnclosuresModelsLibrary.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;

namespace BronzeArtWebApplication.Shared.ViewModels
{
    public class CabinPropertiesViewModel : INotifyPropertyChanged
    {
        private readonly CabinFactory cabinsFactory;
        private readonly ICabinMemoryRepository repo;
        private readonly CabinValidator validator;
        private readonly GlassesBuilderDirector glassBuilder;
        private ValidationResult validationResult;
        private Cabin cabin;
        public Cabin CabinObject { get => cabin; }

        private CabinIdentifier identifier = new(CabinModelEnum.Model9S, CabinDrawNumber.None, CabinSynthesisModel.Primary);
        public CabinIdentifier Identifier
        {
            get => identifier;
            set
            {
                if (identifier != value)
                {
                    identifier = value;
                    OnPropertyChanged(nameof(Identifier));
                }
            }
        }


        #region 1.Main Properties Cabin

        public CabinSeries? Series { get => cabin?.Series; }

        /// <summary>
        /// The Model Order of the Cabin Synthesis this ViewModel Represents (Primary/Secondary/Tertiary)
        /// </summary>
        public CabinSynthesisModel SynthesisModel { get; private set; }

        public CabinDrawNumber? IsPartOfDraw
        {
            get => cabin?.IsPartOfDraw;
            set
            {
                if (value != cabin?.IsPartOfDraw)
                {
                    cabin = cabinsFactory.CreateCabin(value ?? CabinDrawNumber.None, SynthesisModel);
                    Identifier = new(cabin?.Model ?? CabinModelEnum.Model9S, cabin?.IsPartOfDraw ?? CabinDrawNumber.None, cabin?.SynthesisModel ?? CabinSynthesisModel.Primary);
                    OnPropertyChanged(); //All Properties are Changed the Object is new;
                }
            }
        }

        /// <summary>
        /// The Direction Input for the Cabin
        /// </summary>
        public CabinDirection? Direction
        {
            get => cabin?.Direction;
            set
            {
                if (cabin != null && value != cabin?.Direction)
                {
                    cabin.Direction = value ?? CabinDirection.Undefined;
                    OnPropertyChanged(nameof(Direction));
                }
            }
        }

        public CabinModelEnum? Model
        {
            get
            {
                return cabin?.Model;
            }
        }

        public CabinFinishEnum? MetalFinish
        {
            get { return cabin?.MetalFinish; }
            set
            {
                if (cabin != null && value != cabin.MetalFinish)
                {
                    cabin.MetalFinish = value;
                    OnPropertyChanged(nameof(MetalFinish));
                }
            }
        }

        public CabinThicknessEnum? Thicknesses
        {
            get { return cabin?.Thicknesses; }
            set
            {
                if (cabin != null && value != Thicknesses)
                {
                    //For Double(6-8) Thickness put at the Sides The Greater thickness 
                    if (value is CabinThicknessEnum.Thick6mm8mm && Model is CabinModelEnum.Model9F)
                    {
                        cabin.Thicknesses = CabinThicknessEnum.Thick8mm;
                    }
                    if (value is CabinThicknessEnum.Thick8mm10mm && Model is CabinModelEnum.ModelVF)
                    {
                        cabin.Thicknesses = CabinThicknessEnum.Thick10mm;
                    }
                    else
                    {
                        cabin.Thicknesses = value;
                    }
                    OnPropertyChanged(nameof(Thicknesses));
                }
            }
        }

        public GlassFinishEnum? GlassFinish
        {
            get { return cabin?.GlassFinish; }
            set
            {
                if (cabin != null && value != GlassFinish)
                {
                    cabin.GlassFinish = value;
                    OnPropertyChanged(nameof(GlassFinish));
                }
            }
        }

        /// <summary>
        /// The Input Length is the NOMINAL LENGTH of each Cabin
        /// </summary>
        public int? InputLength
        {
            get
            {
                return cabin?.NominalLength;
            }
            set
            {
                if (cabin != null && value != cabin.NominalLength)
                {
                    if (value is not null)
                    {
                        cabin.NominalLength = (int)value;
                    }
                    else
                    {
                        cabin.NominalLength = 0;
                    }
                    OnPropertyChanged(nameof(InputLength));
                    OnPropertyChanged(nameof(MinLength));
                }
            }
        }

        public int? InputHeight
        {
            get
            {
                if (cabin != null && cabin.Height != 0)
                {
                    return cabin.Height;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (cabin != null && value != InputHeight)
                {
                    cabin.Height = value ?? 0;
                    OnPropertyChanged(nameof(InputHeight));
                }
            }
        }

        public int? InputStepLength
        {
            get
            {
                if (cabin != null && cabin.HasStep)
                {
                    return cabin.GetStepCut()?.StepLength ?? 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (cabin != null)
                {
                    if (cabin.HasStep == false)
                    {
                        cabin.AddExtra(CabinExtraType.StepCut);
                    }
                    cabin.GetStepCut().StepLength = value ?? 0;
                    OnPropertyChanged(nameof(InputStepLength));
                }
            }
        }

        public int? InputStepHeight
        {
            get
            {
                if (cabin != null && cabin.HasStep)
                {
                    return cabin.GetStepCut()?.StepHeight ?? 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (cabin != null)
                {
                    if (cabin.HasStep == false)
                    {
                        cabin.AddExtra(CabinExtraType.StepCut);
                    }
                    cabin.GetStepCut().StepHeight = value ?? 0;
                    OnPropertyChanged(nameof(InputStepHeight));
                }
            }
        }

        #endregion

        #region 2. Constraints

        /// <summary>
        /// The Current MaxLength (After Calculating Nominal with Tollerance)
        /// </summary>
        public int MaxLength { get => cabin?.LengthMax ?? 0; }
        /// <summary>
        /// The Current Min Length
        /// </summary>
        public int MinLength { get => cabin?.LengthMin ?? 0; }

        /// <summary>
        /// The NominalMaxLength
        /// </summary>
        public int MaxLengthLimit
        {
            get
            {
                if (cabin is not null)
                {
                    return cabin.Constraints.MaxPossibleLength + cabin.TolleranceMinus;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// The Nominal Min Length
        /// </summary>
        public int MinLengthLimit
        {
            get
            {
                if (cabin is not null)
                {
                    return cabin.Constraints.MinPossibleLength + cabin.TolleranceMinus;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int MaxHeightLimit { get { return cabin?.Constraints.MaxPossibleHeight ?? 0; } }
        public int MinHeightLimit { get { return cabin?.Constraints.MinPossibleHeight ?? 0; } }
        public bool ShouldHaveHandle { get => cabin?.Constraints.ShouldHaveHandle ?? false; }
        public CabinThicknessEnum BreakPointThickness { get => cabin?.Constraints.BreakPointMinThickness ?? CabinThicknessEnum.NotSet; }
        public int HeightBreakpointGlassThickness { get => cabin?.Constraints.HeightBreakPointGlassThickness ?? 5000; }
        public int LengthBreakpointGlassThickness { get => cabin?.Constraints.LengthBreakPointGlassThickness ?? 5000; }
        public bool CanHavePerimatricalFrame { get => cabin?.Constraints is ConstraintsW constraints && constraints.CanHavePerimetricalFrame; }

        public List<CabinThicknessEnum> ValidThicknesses { get => cabin?.Constraints.ValidThicknesses ?? new(); }
        public List<GlassFinishEnum> ValidGlassFinishes { get => cabin?.Constraints.ValidGlassFinishes ?? new(); }
        public List<CabinFinishEnum> ValidMetalFinishes { get => cabin?.Constraints.ValidMetalFinishes ?? new(); }
        #endregion

        #region 3. Parts Options
        /*
         Each Option has A Boolean which checks if there are parts of this option but also if there are Values to Choose From
         This helps as SynthesisModels where their options have no Selectable values will not inherit values from the rest SynthesisModels 
         */


        public bool HasWallFixingOption { get => repo.GetSpotValids(identifier, PartSpot.WallSide1).Count() > 1; }
        public CabinPart WallSideFixerOption
        {
            get => HasWallFixingOption ? (cabin?.Parts as IWallSideFixer).WallSideFixer : null;
            set
            {
                if (cabin?.Parts is IWallSideFixer wallSideFixerOption)
                {
                    wallSideFixerOption.WallSideFixer = value;
                    if (cabin is CabinV4 v4)
                    {
                        v4.Parts.WallFixer2 = value.GetDeepClone();
                    }
                    OnPropertyChanged(nameof(WallSideFixerOption));
                }
            }
        }
        public string DefaultWallFixer { get => repo.GetDefault(identifier, PartSpot.WallSide1); }
        public IEnumerable<string> SelectableWallFixers { get => repo.GetSpotValids(identifier, PartSpot.WallSide1); }
        public IPerimetricalFixer PerimetricalFixer { get => cabin?.Parts as IPerimetricalFixer ?? null; }
        public bool HasPerimetricalFrame
        {
            get => CanHavePerimatricalFrame && (cabin?.Parts as IPerimetricalFixer).HasPerimetricalFrame;
            set
            {
                if (CanHavePerimatricalFrame && value != PerimetricalFixer.HasPerimetricalFrame)
                {
                    if (value is true)
                    {
                        PerimetricalFixer.WallSideFixer = repo.GetDefaultPart(identifier, PartSpot.WallSide1);
                        PerimetricalFixer.TopFixer = repo.GetDefaultPart(identifier, PartSpot.WallSide1);
                        PerimetricalFixer.SideFixer = repo.GetDefaultPart(identifier, PartSpot.WallSide1);
                        PerimetricalFixer.BottomFixer = repo.GetDefaultPart(identifier, PartSpot.WallSide1);
                        (cabin.Constraints as ConstraintsW).CornerRadiusTopEdge = 0;
                    }
                    else
                    {
                        PerimetricalFixer.WallSideFixer = repo.GetDefaultPart(identifier, PartSpot.WallSide1);
                        PerimetricalFixer.TopFixer = null;
                        PerimetricalFixer.BottomFixer = repo.GetDefaultPart(identifier, PartSpot.BottomSide1);
                        PerimetricalFixer.SideFixer = null;
                        (cabin.Constraints as ConstraintsW).CornerRadiusTopEdge = Model != CabinModelEnum.Model8W40 ? 0 : 200;
                    }
                    OnPropertyChanged(nameof(HasPerimetricalFrame));
                }
            }
        }


        public bool HasHandleOption { get => repo.GetSpotValids(identifier, PartSpot.Handle1).Where(c => !string.IsNullOrEmpty(c)).Any(); }
        public CabinHandle HandleOption
        {
            get => HasHandleOption ? (cabin?.Parts as IHandle).Handle : null;
            set
            {
                if (cabin?.Parts is IHandle handleOption)
                {
                    handleOption.Handle = value;
                    OnPropertyChanged(nameof(HandleOption));
                }
            }
        }
        public string DefaultHandle { get => repo.GetDefault(identifier, PartSpot.Handle1); }
        public IEnumerable<string> SelectableHandles { get => repo.GetSpotValids(identifier, PartSpot.Handle1).Where(c => !string.IsNullOrEmpty(c)); }

        public bool HasBottomFixerOption { get => cabin?.Parts is IBottomFixer fixer && repo.GetSpotValids(identifier, PartSpot.BottomSide1).Any(c => !string.IsNullOrEmpty(c)); }
        public CabinPart BottomFixerOption
        {
            get => HasBottomFixerOption ? (cabin?.Parts as IBottomFixer).BottomFixer : null;
            set
            {
                if (cabin?.Parts is IBottomFixer bottomFixerOption)
                {
                    bottomFixerOption.BottomFixer = value;
                    OnPropertyChanged(nameof(BottomFixerOption));
                }
            }
        }

        public string DefaultBottomFixer { get => cabin?.Parts is IBottomFixer ? repo.GetDefault((CabinModelEnum)Model, (CabinDrawNumber)IsPartOfDraw, SynthesisModel, PartSpot.BottomSide1) : string.Empty; }
        public IEnumerable<string> SelectableBottomFixers { get => cabin?.Parts is IBottomFixer ? repo.GetSpotValids((CabinModelEnum)Model, (CabinDrawNumber)IsPartOfDraw, SynthesisModel, PartSpot.BottomSide1) : new List<string>(); }

        public bool HasCloseProfileOption { get => (cabin?.Parts.HasSpot(PartSpot.CloseProfile) ?? false) && (SelectableCloseProfiles.Concat(SelectableCloseStrips).Count() > 1); }
        public Profile CloseProfileOption
        {
            get => HasCloseProfileOption ? (cabin.Parts as ICloseProfile)?.CloseProfile : null;
            set
            {
                if (cabin?.Parts is ICloseProfile closeProfile)
                {
                    closeProfile.CloseProfile = value;
                    OnPropertyChanged(nameof(CloseProfileOption));
                }
            }
        }
        public string DefaultCloseProfile { get => cabin?.Parts is ICloseProfile ? repo.GetDefault(identifier, PartSpot.CloseProfile) : string.Empty; }
        public IEnumerable<string> SelectableCloseProfiles { get => repo.GetSpotValids(identifier, PartSpot.CloseProfile).Where(c => !string.IsNullOrEmpty(c)); }
        public GlassStrip CloseStripOption
        {
            get => HasCloseProfileOption ? (cabin?.Parts as ICloseProfile)?.CloseStrip : null;
            set
            {
                if (cabin?.Parts is ICloseProfile closeProfileWithStrip)
                {
                    closeProfileWithStrip.CloseStrip = value;
                    OnPropertyChanged(nameof(CloseStripOption));
                }
            }
        }
        public string DefaultCloseStrip { get => cabin?.Parts is ICloseProfile ? repo.GetDefault(identifier, PartSpot.CloseStrip) : string.Empty; }
        public IEnumerable<string> SelectableCloseStrips { get => repo.GetSpotValids(identifier, PartSpot.CloseStrip).Where(c => !string.IsNullOrEmpty(c)); }

        public bool HasMiddleHingeOption { get => repo.GetSpotValids(identifier, PartSpot.MiddleHinge).Count() > 1; }
        public CabinPart MiddleHingeOption
        {
            get => HasMiddleHingeOption ? (cabin?.Parts as IMiddleHinge)?.MiddleHinge : null;
            set
            {
                if (cabin?.Parts is IMiddleHinge middleHinge)
                {
                    middleHinge.MiddleHinge = value;
                    OnPropertyChanged(nameof(MiddleHingeOption));
                }
            }
        }
        public string DefaultMiddleHinge { get => repo.GetDefault(identifier, PartSpot.MiddleHinge); }
        public IEnumerable<string> SelectableMiddleHinges { get => repo.GetSpotValids(Identifier, PartSpot.MiddleHinge); }

        #endregion

        public List<CabinExtra> Extras
        {
            get => cabin?.Extras;
        }

        public bool HasBronzeClean
        {
            get => cabin?.HasExtra(CabinExtraType.BronzeClean) ?? false;
            set
            {
                if (value != cabin?.HasExtra(CabinExtraType.BronzeClean) && cabin != null)
                {
                    switch (value)
                    {
                        case true:
                            cabin?.AddExtra(CabinExtraType.BronzeClean);
                            break;
                        case false:
                            cabin?.RemoveExtra(CabinExtraType.BronzeClean);
                            break;
                    }
                    OnPropertyChanged(nameof(HasBronzeClean));
                }
            }
        }
        public bool HasSafeKids
        {
            get => cabin?.HasExtra(CabinExtraType.SafeKids) ?? false;
            set
            {
                if (value != cabin?.HasExtra(CabinExtraType.SafeKids) && cabin != null)
                {
                    switch (value)
                    {
                        case true:
                            cabin?.AddExtra(CabinExtraType.SafeKids);
                            break;
                        case false:
                            cabin?.RemoveExtra(CabinExtraType.SafeKids);
                            break;
                    }
                    OnPropertyChanged(nameof(HasSafeKids));
                }
            }
        }

        public CabinPropertiesViewModel(CabinSynthesisModel synthesisNo,
                                        CabinFactory cabinsFactory,
                                        ICabinMemoryRepository repo,
                                        CabinValidator validator,
                                        GlassesBuilderDirector glassBuilder)
        {
            this.SynthesisModel = synthesisNo;
            this.cabinsFactory = cabinsFactory;
            this.repo = repo;
            this.validator = validator;
            this.glassBuilder = glassBuilder;
        }

        /// <summary>
        /// Resets the CabinProperties ViewModel
        /// </summary>
        public void ResetViewModel()
        {
            cabin = null;
            validationResult = null;
            OnPropertyChanged(nameof(Model));
            OnPropertyChanged(nameof(IsPartOfDraw));
        }

        /// <summary>
        /// Checks if the Properties Selected are Valid
        /// </summary>
        /// <returns></returns>
        public bool IsSelectionValid()
        {
            if (cabin is not null)
            {
                validationResult = validator.Validate(cabin);
                return validationResult.IsValid;
            }
            return false;
        }

        /// <summary>
        /// Caculates the Glasses  (Will run only if Validation has been done to the Cabin) otherWise glass List will remain Empty
        /// </summary>
        public void CalculateGlasses()
        {
            if (cabin is not null && validationResult is not null && validationResult.IsValid)
            {
                cabin.Glasses.Clear();
                glassBuilder.BuildAllGlasses(cabin);
                PartsDimensionsCalculator.CalculatePartsDimensions(cabin);
            }
        }

        /// <summary>
        /// Returns a List of the Error Codes of the Validated Cabin . If those Errors Exist
        /// </summary>
        /// <returns>List of Error Codes or an Empty List</returns>
        public List<string> GetValidationErrorCodes()
        {
            List<string> errorCodes = validationResult?.Errors?.Select(errors => errors.ErrorCode).Where(errorCode => string.IsNullOrEmpty(errorCode) == false).ToList();

            return errorCodes ?? new();

        }

        /// <summary>
        /// Returns the Description keys for this Cabin -- 9C Must Provide Combined Dimensions
        /// </summary>
        /// <returns></returns>
        public List<string> GetDescriptionKeys()
        {
            List<string> keys = new();
            string series = CabinSeriesDescKey[cabin.Series];
            string model = Model != null ? CabinModelEnumDescKey[(CabinModelEnum)Model] : "N/A Model";
            string metalFinish = MetalFinish != null ? CabinFinishEnumDescKey[(CabinFinishEnum)MetalFinish] : "N/A Cabin Finish";
            string thickness = Thicknesses != null ? CabinThicknessesEnumDescKey[(CabinThicknessEnum)Thicknesses] : "N/A Thickness";
            string glassFinish = GlassFinish != null ? GlassFinishEnumDescKey[(GlassFinishEnum)GlassFinish] : "N/A Glass Finish";
            string dimensions = $"{cabin.NominalLength}x{cabin.Height}mm";
            string direction = CabinDirectionDescKey[Direction ?? CabinDirection.Undefined];
            keys.Add(series);
            keys.Add(model);
            keys.Add(metalFinish);
            keys.Add(thickness);
            keys.Add(glassFinish);
            if (Model != CabinModelEnum.Model9C) { keys.Add(dimensions); }; //Otherwise the Transforming method must Provide this DescKey
            return keys;
        }

        /// <summary>
        /// Remove the Step From the Cabin , Used when Selection of Step is Closed with zero Values
        /// </summary>
        public void RemoveStep()
        {
            cabin.RemoveExtra(CabinExtraType.StepCut);
            OnPropertyChanged(nameof(InputStepHeight));
            OnPropertyChanged(nameof(InputStepLength));
        }

        /// <summary>
        /// THIS METHOD IS NOR IMPLEMENTED CORRECTLY -- IT BYPASSES CABIN DRAW AND IDENTIFIER ... WHICH MUST BE FIRST SET IF THIS ONE IS USED....
        /// </summary>
        /// <param name="cabin"></param>
        public void SetCabin(Cabin cabin, bool notify = true)
        {
            this.cabin = cabin;
            if (notify)
            {
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Weather this thickness is Allowed for the Current State of the ViewModel
        /// Checks weather the Height/Length Breakpoint is hit and wheather current thickness is bigger/equal than the BreakPoint Thickness
        /// </summary>
        /// <param name="thickness">The Thickness to Check</param>
        /// <returns></returns>
        public bool IsThicknessAllowed(CabinThicknessEnum thickness)
        {
            if (InputHeight > cabin.Constraints.HeightBreakPointGlassThickness ||
                    InputLength > cabin.Constraints.LengthBreakPointGlassThickness)
            {
                return (int)thickness >= (int)cabin.Constraints.BreakPointMinThickness;
            }
            return true;
        }

        #region Z.Property Changed Event

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

using AKSoftware.Localization.MultiLanguages;
using Blazored.LocalStorage;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.Validators;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.Builders;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using FluentValidation.Results;
using MirrorsModelsLibrary.DrawsBuilder.Models;
using MirrorsModelsLibrary.DrawsBuilder.Models.MeasureObjects;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MirrorsModelsLibrary.StaticData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoMirror;

namespace BronzeArtWebApplication.Shared.ViewModels
{
    public class AssembleMirrorViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Mirror mirror = new();
        private readonly ILanguageContainerService lc;
        private readonly BronzeUser loggedUser;
        private readonly BronzeItemsPriceBuilder priceBuilder;

        private string notesText;
        public string NotesText
        {
            get => notesText;
            set
            {
                if (notesText != value)
                {
                    notesText = value;
                    OnPropertyChanged(nameof(NotesText));
                }
            }
        }

        #region 1.Extra Options Properties

        private bool isFogDouble;
        private bool isMagnifyerDouble;

        /// <summary>
        /// The Extras Of the Mirror
        /// </summary>
        public List<MirrorExtra> Extras { get { return mirror.Extras; } }

        public bool HasSwitch
        {
            get { return Extras.Any(e => e.Option == MirrorOption.TouchSwitch); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.TouchSwitch);
                    HasDimmer = false;
                    HasSensor = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.TouchSwitch);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasSwitch));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasDimmer
        {
            get { return Extras.Any(e => e.Option == MirrorOption.DimmerSwitch); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.DimmerSwitch);
                    HasSwitch = false;
                    HasSensor = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.DimmerSwitch);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasDimmer));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasSensor
        {
            get { return Extras.Any(e => e.Option == MirrorOption.SensorSwitch); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.SensorSwitch);
                    HasSwitch = false;
                    HasDimmer = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.SensorSwitch);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasSensor));
                OnPropertyChanged(nameof(mirror));
            }
        }

        /// <summary>
        /// Gets changed By HasFog16/24/55 or Nothing at all
        /// </summary>
        public bool HasFogSwitch
        {
            get { return Extras.Any(e => e.Option == MirrorOption.TouchSwitchFog); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.TouchSwitchFog);
                    SetMirrorSeriesCode();
                }
                else if (value is false)
                {
                    RemoveExtra(MirrorOption.TouchSwitchFog);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasFogSwitch));
                OnPropertyChanged(nameof(mirror));
            }
        }

        public bool HasEcoFogSwitch
        {
            get { return Extras.Any(e => e.Option == MirrorOption.EcoTouch); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.EcoTouch);
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.EcoTouch);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasEcoFogSwitch));
                OnPropertyChanged(nameof(mirror));
            }
        }

        //Fog Options --> When true turns the Rest Fogs False. // When all False FogSwitch is Removed
        public bool HasFog16
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Fog16W); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Fog16W);
                    HasFog24 = false;
                    HasFog55 = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Fog16W);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasFog16));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasFog24
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Fog24W); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Fog24W);
                    HasFog16 = false;
                    HasFog55 = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Fog24W);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasFog24));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasFog55
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Fog55W); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Fog55W);
                    HasFog16 = false;
                    HasFog24 = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Fog55W);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasFog55));
                OnPropertyChanged(nameof(mirror));
            }
        }

        public bool IsFogDouble
        {
            get { return isFogDouble; }
            set
            {
                if (value != isFogDouble)
                {
                    isFogDouble = value;
                    OnPropertyChanged(nameof(IsFogDouble));
                }
            }
        }
        public bool HasMagnifyer
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Zoom); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Zoom);
                    HasMagnifyerLed = false;
                    HasMagnifyerLedTouch = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Zoom);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasMagnifyer));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasMagnifyerLed
        {
            get { return Extras.Any(e => e.Option == MirrorOption.ZoomLed); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.ZoomLed);
                    HasMagnifyer = false;
                    HasMagnifyerLedTouch = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.ZoomLed);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasMagnifyerLed));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasMagnifyerLedTouch
        {
            get { return Extras.Any(e => e.Option == MirrorOption.ZoomLedTouch); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.ZoomLedTouch);
                    HasMagnifyer = false;
                    HasMagnifyerLed = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.ZoomLedTouch);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasMagnifyerLedTouch));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool IsMagnifyerDouble
        {
            get { return isMagnifyerDouble; }
            set
            {
                if (value != isMagnifyerDouble)
                {
                    isMagnifyerDouble = value;
                    OnPropertyChanged(nameof(IsMagnifyerDouble));
                }
            }
        }
        public bool HasClock
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Clock); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Clock);
                    HasDisplay11Black = false;
                    HasDisplay19 = false;
                    HasDisplay20 = false;
                    HasDisplay11 = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Clock);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasClock));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasBluetooth
        {
            get { return Extras.Any(e => e.Option == MirrorOption.BlueTooth); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.BlueTooth);
                    HasDisplay11Black = false;
                    HasDisplay11 = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.BlueTooth);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasBluetooth));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasDisplay11
        {
            get { return Extras.Any(e => e.Option == MirrorOption.DisplayRadio); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.DisplayRadio);
                    HasDisplay11Black = false;
                    HasDisplay19 = false;
                    HasDisplay20 = false;
                    HasBluetooth = false;
                    HasClock = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.DisplayRadio);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasDisplay11));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasDisplay11Black
        {
            get { return Extras.Any(e => e.Option == MirrorOption.DisplayRadioBlack); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.DisplayRadioBlack);
                    HasDisplay11 = false;
                    HasDisplay19 = false;
                    HasDisplay20 = false;
                    HasBluetooth = false;
                    HasClock = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.DisplayRadioBlack);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasDisplay11Black));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasDisplay19
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Display19); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Display19);
                    HasDisplay11Black = false;
                    HasDisplay20 = false;
                    HasDisplay11 = false;
                    HasClock = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Display19);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasDisplay19));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasDisplay20
        {
            get { return Extras.Any(e => e.Option == MirrorOption.Display20); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.Display20);
                    HasDisplay11Black = false;
                    HasDisplay19 = false;
                    HasDisplay11 = false;
                    HasClock = false;
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.Display20);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasDisplay20));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasLid
        {
            get { return Extras.Any(e => e.Option == MirrorOption.IPLid); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.IPLid);
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.IPLid);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasLid));
                OnPropertyChanged(nameof(mirror));
            }
        }
        public bool HasRounding
        {
            get { return Extras.Any(e => e.Option == MirrorOption.RoundedCorners); }
            set
            {
                if (value == true)
                {
                    AddExtra(MirrorOption.RoundedCorners);
                    SetMirrorSeriesCode();
                }
                else if (value == false)
                {
                    RemoveExtra(MirrorOption.RoundedCorners);
                    SetMirrorSeriesCode();
                }
                OnPropertyChanged(nameof(HasRounding));
                OnPropertyChanged(nameof(mirror));
            }
        }

        #endregion

        #region 2.General Properties of Mirror

        public MirrorSeries? Series
        {
            get { return mirror.Series; }
            set
            {
                if (value != mirror.Series)
                {
                    mirror.Series = value;
                    OnPropertyChanged(nameof(Series));
                    SetMirrorSeriesCode();
                }
            }
        }

        public MirrorShape? Shape
        {
            get { return mirror.Shape; }
            set
            {
                if (value != mirror.Shape)
                {
                    mirror.Shape = value;
                    OnPropertyChanged(nameof(Shape));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        public MirrorSandblast? Sandblast
        {
            get { return mirror.Sandblast; }
            set
            {
                if (value != mirror.Sandblast)
                {
                    mirror.Sandblast = value;
                    OnPropertyChanged(nameof(Sandblast));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        public MirrorLight? Light
        {
            get { return mirror.Lighting.Light; }
            set
            {
                if (value != mirror.Lighting.Light)
                {
                    if (mirror.Lighting.Light == null) InitilizeNullLightMirror(value);
                    mirror.Lighting.Light = value;
                    OnPropertyChanged(nameof(Light));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }
        /// <summary>
        /// Assigns Default Values to Mirror Properties , when the user makes the Mirror for the First Time
        /// </summary>
        /// <param name="previousLight"></param>
        /// <param name="newLight"></param>
        private void InitilizeNullLightMirror(MirrorLight? newLight)
        {
            //Check if the User has Chosen a Light or WithoutLight Mirror
            if (newLight == MirrorLight.WithoutLight) //User chose Mirror Without Light
            {
                Support = MirrorSupport.Double;
                Sandblast = Shape == MirrorShape.Circular ? MirrorSandblast.N9 : MirrorSandblast.H7;
            }
            else //User chose Mirror WITH Light
            {
                //Default Support to Perimetrical
                Support = MirrorSupport.Perimetrical;

                //If the light is Triple or Double , put a Dimmer otherwise a touch switch
                if (newLight is MirrorLight.Warm_Cold or MirrorLight.Warm_Cold_Day or MirrorLight.Warm_Cold_Day_16W or MirrorLight.Warm_Cold_Day_COB)
                {
                    HasDimmer = true;
                }
                else HasSwitch = true;

                //put the default Sandblast
                if (Shape is MirrorShape.Circular)
                {
                    Sandblast = MirrorSandblast.N9;
                }
                else
                {
                    Sandblast = MirrorSandblast.H7;
                }
            }

        }

        public int? Length
        {
            get { return mirror.Length; }
            set
            {
                if (value != mirror.Length)
                {
                    mirror.Length = value;
                    OnPropertyChanged(nameof(Length));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        /// <summary>
        /// Returns the Length in mm
        /// </summary>
        public int Lengthmm

        {
            get
            {
                if (Length != null)
                {
                    int mm = (int)Length * 10;
                    return mm;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns the Real Glass Length in mm
        /// </summary>
        public int GlassLengthmm

        {
            get
            {
                if (Length != null)
                {
                    int mm = (int)Length * 10;
                    if (Support == MirrorSupport.Frame)
                    {
                        mm -= (MirrorStandards.Frames.FrameFrontThickness * 2);
                    }
                    return mm;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int? Height
        {
            get { return mirror.Height; }
            set
            {
                if (value != mirror.Height)
                {
                    mirror.Height = value;
                    OnPropertyChanged(nameof(Height));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }
        /// <summary>
        /// returns the Height in mm
        /// </summary>
        public int Heightmm
        {
            get
            {
                if (Height != null)
                {
                    int mm = (int)Height * 10;
                    return mm;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns the Real Glass Length in mm
        /// </summary>
        public int GlassHeightmm

        {
            get
            {
                if (Height != null)
                {
                    int mm = (int)Height * 10;
                    if (Support == MirrorSupport.Frame)
                    {
                        mm -= (MirrorStandards.Frames.FrameFrontThickness * 2);
                    }
                    return mm;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int? Diameter
        {
            get { return mirror.Diameter; }
            set
            {
                if (value != mirror.Diameter)
                {
                    mirror.Diameter = value;
                    OnPropertyChanged(nameof(Diameter));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }
        /// <summary>
        /// returns the Diameter in mm
        /// </summary>
        public int Diametermm
        {
            get
            {
                if (Diameter != null)
                {
                    int mm = (int)Diameter * 10;
                    return mm;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Returns the Real Glass Length in mm
        /// </summary>
        public int GlassDiametermm

        {
            get
            {
                if (Diameter != null)
                {
                    int mm = (int)Diameter * 10;
                    if (Support == MirrorSupport.Frame)
                    {
                        mm -= (MirrorStandards.Frames.FrameFrontThickness * 2);
                    }
                    return mm;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int GlassRadiusmm
        {
            get
            {
                int radius = GlassDiametermm / 2;
                return radius;
            }
        }


        /// <summary>
        /// Mirror Code gets Set From all the Other Properties - Whenever One Changes || it Also Changes if we Add or Remove Touch Switch
        /// </summary>
        public string Code
        {
            get
            {
                return mirror.Code;
            }
            set
            {
                if (value != mirror.Code)
                {
                    mirror.Code = value;
                    OnPropertyChanged(nameof(Code));
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }
        #endregion

        #region 3.Mirror Support Properties
        public MirrorSupport? Support
        {
            get { return mirror.Support.SupportType; }
            set
            {
                if (value != mirror.Support.SupportType)
                {
                    mirror.Support.SupportType = value;
                    OnPropertyChanged(nameof(Support));
                    SetMirrorSeriesCode();
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        /// <summary>
        /// The Finish Type of the Frame -- Changes Values of Paint / Electroplated Finish to Null according to selection
        /// </summary>
        public SupportFinishType? FinishType
        {
            get { return mirror.Support.FinishType; }
            set
            {
                if (value != mirror.Support.FinishType)
                {
                    mirror.Support.FinishType = value;
                    switch (value) // Change the Value of the Other Finishes to null
                    {
                        case SupportFinishType.Painted:
                            ElectroplatedFinish = null;
                            break;
                        case SupportFinishType.Electroplated:
                            PaintFinish = null;
                            break;
                        case SupportFinishType.Simple:
                        default:
                            PaintFinish = null;
                            ElectroplatedFinish = null;
                            break;
                    }
                    OnPropertyChanged(nameof(FinishType));
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        public SupportPaintFinish? PaintFinish
        {
            get { return mirror.Support.PaintFinish; }
            set
            {
                if (mirror.Support.PaintFinish != value)
                {
                    mirror.Support.PaintFinish = value;
                    OnPropertyChanged(nameof(PaintFinish));
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        public SupportElectroplatedFinish? ElectroplatedFinish
        {
            get { return mirror.Support.ElectroplatedFinish; }
            set
            {
                if (mirror.Support.ElectroplatedFinish != value)
                {
                    mirror.Support.ElectroplatedFinish = value;
                    OnPropertyChanged(nameof(ElectroplatedFinish));
                    OnPropertyChanged(nameof(mirror));
                }
            }
        }

        #endregion

        #region 4.IsChosen Help Properties

        /// <summary>
        /// Checks if the User has Chosen Dimensions
        /// </summary>
        public bool IsDimensionsChosen
        {
            get
            {
                if (Shape != null)
                {
                    if ((Length != null && Height != null) || Diameter != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region 5.Drawing Properties
        public MirrorDraw Draw { get => GetMirrorDraw(); }

        #endregion

        /// <summary>
        /// Constructor -- Needs to Exist So Program.cs Instantiates this with the Language Container
        /// </summary>
        /// <param name="lc"></param>
        public AssembleMirrorViewModel(ILanguageContainerService lc, BronzeUser loggedUser, BronzeItemsPriceBuilder priceBuilder)
        {
            //Inject Language Container Dependency
            this.lc = lc;

            //Inject User Dependency
            this.loggedUser = loggedUser;

            this.priceBuilder = priceBuilder;
            //Subscribe to AuthenticationState Changes so to Change User Discounts and Properties on the ViewModel
            //this.loggedUser.OnAuthenticationStateChanged += GetLoggedUserClaims;

        }

        #region M1.Reset/Change ViewModel Methods

        /// <summary>
        /// Resets All Except the MirrorShape
        /// </summary>
        public void ResetExceptShape()
        {
            mirror.Series = null;
            mirror.Diameter = null;
            mirror.Lighting.Light = null;
            mirror.Height = null;
            mirror.Length = null;
            mirror.Sandblast = null;
            mirror.Support.SupportType = null;
            mirror.Support.FinishType = null;
            mirror.Support.PaintFinish = null;
            mirror.Support.ElectroplatedFinish = null;
            ResetExtras();
        }

        /// <summary>
        /// Resets the ViewModel
        /// </summary>
        public void ResetViewModel()
        {
            mirror.Series = null;
            mirror.Shape = null;
            mirror.Diameter = null;
            mirror.Lighting.Light = null;
            mirror.Height = null;
            mirror.Length = null;
            mirror.Sandblast = null;
            mirror.Support.SupportType = null;
            mirror.Support.FinishType = null;
            mirror.Support.PaintFinish = null;
            mirror.Support.ElectroplatedFinish = null;
            ResetExtras();
            NotesText = string.Empty;
        }

        #endregion

        #region M2.Extras Manipulation Methods (Add/Remove/Reset)

        /// <summary>
        /// Removes All Extras from the Mirror
        /// </summary>
        public void ResetExtras()
        {
            Extras.Clear();
            //inform Allmost All Properties Changed
            OnPropertyChanged();
        }

        /// <summary>
        /// Adds the Option to the Extras of the Mirror
        /// </summary>
        /// <param name="option"></param>
        public void AddExtra(MirrorOption option)
        {
            if (Extras.Any(e => e.Option == option) == false)
            {
                mirror.Extras.Add(new MirrorExtra(option));
                OnPropertyChanged(nameof(Extras));
            }
        }

        /// <summary>
        /// Removes the option from the Extras of the Mirror
        /// </summary>
        /// <param name="option"></param>
        private void RemoveExtra(MirrorOption option)
        {
            MirrorExtra extra = Extras.SingleOrDefault(e => e.Option == option);
            if (extra != null)
            {
                mirror.Extras.Remove(extra);
                OnPropertyChanged(nameof(Extras));
            }
        }

        #endregion

        #region M3.CodeMethods

        /// <summary>
        /// Get the Code of the Light
        /// </summary>
        /// <returns></returns>
        public string GetMirrorLightCode()
        {
            return mirror.Lighting.Code;
        }

        /// <summary>
        /// Returns the Code of the Mirror
        /// </summary>
        /// <returns></returns>
        public void SetMirrorSeriesCode()
        {
            mirror.SetCode();
            OnPropertyChanged(nameof(Code));
            mirror.SetSeries();
            OnPropertyChanged(nameof(Series));

            //Add the Light Channel if the Mirror is with frame and Light and without any sandblast
            //Otherwise if there is an Channel remove it
            if (mirror.Sandblast == MirrorSandblast.H7
                && mirror.Series != MirrorSeries.P8
                && mirror.Support.SupportType == MirrorSupport.Frame
                && mirror.Lighting.Light != MirrorLight.WithoutLight)
            {
                //Nested if otherwise it falls to the else if case and removes the channel when its already there
                if (!mirror.HasExtra(MirrorOption.LightAluminumChannel))
                {
                    mirror.Extras.Add(new MirrorExtra(MirrorOption.LightAluminumChannel));
                    OnPropertyChanged(nameof(Extras));
                }
            }
            //Remove the channel if there in any other occasion
            else if (mirror.HasExtra(MirrorOption.LightAluminumChannel))
            {
                mirror.Extras.Remove(mirror.Extras.FirstOrDefault(e => e.Option == MirrorOption.LightAluminumChannel));
                OnPropertyChanged(nameof(Extras));
            }
        }

        #endregion

        #region M4.Table Methods

        // DEPRECATED Gets a List of Product Rows (The Mirror Divided by its Components with Prices/Descriptions/Codes/Discount)
        //public PricingTableData GetMirrorTableData()
        //{
        //    TableRowsBuilder builder = new(mirror, 
        //                                   lc, 
        //                                   loggedUser.CombinedDiscount,
        //                                   loggedUser.SelectedPriceIncreaseFactor,
        //                                   loggedUser.SelectedVatFactor);
        //    return builder.BuildTableData();
        //}

        /// <summary>
        /// Gets the Mirror Priceables
        /// </summary>
        /// <returns></returns>
        public List<IPriceable> GetPriceables()
        {
            List<IPriceable> list = [];
            list = priceBuilder.GetPriceables(GetMirrorObject());
            //Create the Cabin Products
            PricingRulesOptionsMirrors options = new() { UserCombinedDiscountMirrors = loggedUser.CombinedDiscount };
            if (loggedUser.SelectedAppMode is BronzeAppMode.Retail)
            {
                options.WithIncreasePriceRule = true;
                options.PriceIncreaseFactor = loggedUser.SelectedPriceIncreaseFactor;
            }
            RulesDirector rules = new(options);
            rules.ApplyRulesToMultiple(list);
            list.ForEach(priceable => priceable.VatFactor = loggedUser.VatFactor);
            return list;
        }

        #endregion

        #region M5.CodeBoxMethods

        /// <summary>
        /// Enters the Mirror Properties to the ViewModel
        /// </summary>
        /// <param name="passedMirror">The Mirror to Pass to the Model</param>
        public void PassMirrorToVM(Mirror passedMirror)
        {
            if (mirror is null)
            {
                return;
            }

            Shape = passedMirror.Shape;
            Light = passedMirror.Lighting.Light;
            Length = passedMirror.Length;
            Height = passedMirror.Height;
            Diameter = passedMirror.Diameter;
            Support = passedMirror.Support.SupportType;
            FinishType = passedMirror.Support.FinishType;
            if (FinishType == SupportFinishType.Painted)
            {
                PaintFinish = passedMirror.Support.PaintFinish;
            }
            else if (FinishType == SupportFinishType.Electroplated)
            {
                ElectroplatedFinish = passedMirror.Support.ElectroplatedFinish;
            }
            Sandblast = passedMirror.Sandblast;
            foreach (MirrorExtra extra in passedMirror.Extras)
            {
                AddExtra(extra.Option);
            }
            SetMirrorSeriesCode();
            //In Order to Update the State of the Main Page
            OnPropertyChanged(nameof(mirror));
        }

        #endregion

        #region M6.Validation Methods

        /// <summary>
        /// Test Validate the Mirror and Write  Error Message Results to Console
        /// </summary>
        /// <param name="withDrawBoundsValidation">Wheather to Validate also the Extras Being out Of Bounds</param>
        /// <returns>The Validation Result</returns>
        public ValidationResult ValidateMirrorInputs(bool withDrawBoundsValidation = true)
        {
            MirrorValidator validator = new(lc, withDrawBoundsValidation);
            ValidationResult result = validator.Validate(this);
            return result;
        }

        /// <summary>
        /// Validates Mirror Inputs and Returns the the Error Codes it Acted Upon when state is invalid
        /// </summary>
        /// <returns>The Error Codes of the Validation Failures on Which the method Has Acted</returns>
        public List<string> ValidateAndReturnErrorCodes()
        {
            var result = ValidateMirrorInputs();
            List<string> correctedCodes = new();

            if (result.IsValid == false)
            {
                // Collect all the Error Codes that are not Null or Empty to a List
                var errorCodes = result.Errors?.Select(errors => errors.ErrorCode).Where(errorCode => string.IsNullOrEmpty(errorCode) == false);
                foreach (var errorCode in errorCodes)
                {
                    // For each non Empty code Correct the Invalid State of the Model According to the Provided Code
                    bool stateCorrected = CorrectInvalidState(errorCode);
                    // If the Code was Corrected add it to the correctedCodes List
                    if (stateCorrected)
                    {
                        correctedCodes.Add(errorCode);
                    }
                }
            }
            // Return the List of the corrected Codes
            return correctedCodes;
        }

        /// <summary>
        /// If the Error Code is Recognizable , Corrects the Invalid State of the ViewModel and Returns true
        /// Otherwise returns false
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns>True if Corrections where Made</returns>
        private bool CorrectInvalidState(string errorCode)
        {
            switch (errorCode)
            {
                case "LightWithDoubleSupport":
                case "LightWithFrontSupport":
                case "LightWithoutSupport":
                    Support = MirrorSupport.Perimetrical;
                    break;
                case "SimpleRectangularWithSandblast":
                    Sandblast = MirrorSandblast.H7;
                    break;
                case "SimpleCircularWithSandblast":
                    Sandblast = MirrorSandblast.N9;
                    break;
                case "SimpleWithTouch":
                    HasSwitch = false;
                    break;
                case "SimpleWithDimmer":
                    HasDimmer = false;
                    break;
                case "SimpleWithSensor":
                    HasSensor = false;
                    break;
                case "SimpleWithMagnifyer":
                    HasMagnifyer = false;
                    break;
                case "SimpleWithMagnifyerLed":
                    HasMagnifyerLed = false;
                    break;
                case "SimpleWithMagnifyerLedTouch":
                    HasMagnifyerLedTouch = false;
                    break;
                case "SimpleWithLid":
                    HasLid = false;
                    break;
                case "SimpleWithRounding":
                case "LightWithRounding":
                    HasRounding = false;
                    break;
                case "SimpleWithFog16":
                    HasFog16 = false;
                    break;
                case "SimpleWithFog24":
                    HasFog24 = false;
                    break;
                case "SimpleWithFog55":
                    HasFog55 = false;
                    break;
                case "SimpleWithFogSwitch":
                    HasFogSwitch = false;
                    HasEcoFogSwitch = false;
                    break;
                case "SimpleWithClock":
                    HasClock = false;
                    break;
                case "SimpleWithBluetooth":
                    HasBluetooth = false;
                    break;
                case "SimpleWithDisplay11":
                    HasDisplay11 = false;
                    HasDisplay11Black = false;
                    break;
                case "SimpleWithDisplay19":
                    HasDisplay19 = false;
                    break;
                case "SimpleWithDisplay20":
                    HasDisplay20 = false;
                    break;
                case "SimpleCircularCapsulePerimetrical":
                    Support = MirrorSupport.Double;
                    break;
                case "FinishTypeWithoutFrame":
                    FinishType = null;
                    break;
                default:
                    return false; // Return false (Method Did No Corrections)
            }

            return true; //Return True When Corrections where Made
        }

        #endregion

        #region M7.Drawing Methods & Collisions

        /// <summary>
        /// Returns the Mirror Draw Object
        /// </summary>
        private MirrorDraw GetMirrorDraw()
        {
            MirrorDraw draw = new(mirror, 100);
            return draw;
        }


        #endregion

        #region Photo Paths Methods (Return Paths of Images)

        /// <summary>
        /// Returns the Path of the Selected Mirror's Equivalent Photo
        /// </summary>
        /// <returns></returns>
        public string GetSelectedMirrorPhotoPath()
        {
            if (Shape is null || Series is null) return "/Images/MirrorsImages/MakeYourMirrorButton.png";

            switch (Series)
            {
                case MirrorSeries.H7:
                case MirrorSeries.H8:
                case MirrorSeries.X6:
                case MirrorSeries.X4:
                case MirrorSeries._6000:
                case MirrorSeries.M3:
                case MirrorSeries.M8:
                case MirrorSeries.ND:
                case MirrorSeries.NC:
                case MirrorSeries.NL:
                case MirrorSeries.P8:
                case MirrorSeries.R7:
                case MirrorSeries.NS:
                case MirrorSeries.N1:
                case MirrorSeries.N2:
                case MirrorSeries.EL:
                case MirrorSeries.ES:
                case MirrorSeries.IA:
                case MirrorSeries.NCCustom:
                case MirrorSeries.IC:
                case MirrorSeries.NLCustom:
                case MirrorSeries.IL:
                    if (mirror.Length is null || mirror.Height is null)
                    {
                        return StaticInfoMirror.SeriesImagePaths[(MirrorSeries)mirror.Series];//return NoImage
                    }
                    else if (mirror.Length >= mirror.Height)
                    {
                        return StaticInfoMirror.SeriesImagePaths[(MirrorSeries)mirror.Series];
                    }
                    else
                    {
                        return StaticInfoMirror.SeriesImagePathsVertical[(MirrorSeries)mirror.Series];
                    }
                case MirrorSeries.N9:
                case MirrorSeries.N6:
                case MirrorSeries.N7:
                case MirrorSeries.A7:
                case MirrorSeries.A9:
                case MirrorSeries.P9:
                case MirrorSeries.R9:
                case MirrorSeries.NA:
                    return SeriesImagePaths[(MirrorSeries)Series];
                case MirrorSeries.IM:
                    switch (Sandblast)
                    {
                        case MirrorSandblast.H7:
                            if (Support == MirrorSupport.Frame) return H7FramedMirror;
                            else if (mirror.Length is null || mirror.Height is null || mirror.Length >= mirror.Height)
                            {
                                return StaticInfoMirror.SeriesImagePaths[MirrorSeries.H7];
                            }
                            else
                            {
                                return StaticInfoMirror.SeriesImagePathsVertical[MirrorSeries.H7];
                            }
                        case MirrorSandblast.H8:
                            if (Support == MirrorSupport.Frame) return H8FramedMirror;
                            else if (mirror.Length is null || mirror.Height is null || mirror.Length >= mirror.Height)
                            {
                                return StaticInfoMirror.SeriesImagePaths[MirrorSeries.H8];
                            }
                            else
                            {
                                return StaticInfoMirror.SeriesImagePathsVertical[MirrorSeries.H8];
                            }
                        case MirrorSandblast.X6:
                            if (mirror.Length is null || mirror.Height is null || mirror.Length >= mirror.Height)
                            {
                                return StaticInfoMirror.SeriesImagePaths[MirrorSeries.X6];
                            }
                            else
                            {
                                return StaticInfoMirror.SeriesImagePathsVertical[MirrorSeries.X6];
                            }
                        case MirrorSandblast.X4:
                            if (mirror.Length is null || mirror.Height is null || mirror.Length >= mirror.Height)
                            {
                                return StaticInfoMirror.SeriesImagePaths[MirrorSeries.X4];
                            }
                            else
                            {
                                return StaticInfoMirror.SeriesImagePathsVertical[MirrorSeries.X4];
                            }
                        case MirrorSandblast._6000:
                            if (mirror.Length is null || mirror.Height is null || mirror.Length >= mirror.Height)
                            {
                                return StaticInfoMirror.SeriesImagePaths[MirrorSeries._6000];
                            }
                            else
                            {
                                return StaticInfoMirror.SeriesImagePathsVertical[MirrorSeries._6000];
                            }
                        case MirrorSandblast.M3:
                            if (mirror.Length is null || mirror.Height is null || mirror.Length >= mirror.Height)
                            {
                                return StaticInfoMirror.SeriesImagePaths[MirrorSeries.M3];
                            }
                            else
                            {
                                return StaticInfoMirror.SeriesImagePathsVertical[MirrorSeries.M3];
                            }
                        //these are not possible to exist
                        case MirrorSandblast.N9:
                        case MirrorSandblast.N6:
                        case MirrorSandblast.Special:
                        case null:
                        default:
                            return "/Images/MirrorsImages/MakeYourMirrorButton.png";//This key returns No available photo
                    }
                case MirrorSeries.N9Custom:
                    switch (Sandblast)
                    {
                        case MirrorSandblast.N9:
                            return SeriesImagePaths[MirrorSeries.N9];
                        case MirrorSandblast.N6:
                            return Support == MirrorSupport.Frame
                                ? SeriesImagePaths[MirrorSeries.N7]
                                : SeriesImagePaths[MirrorSeries.N6];

                        //these are not possible to exist
                        case MirrorSandblast.H7:
                        case MirrorSandblast.H8:
                        case MirrorSandblast.X6:
                        case MirrorSandblast.X4:
                        case MirrorSandblast._6000:
                        case MirrorSandblast.M3:
                        case MirrorSandblast.Special:
                        case null:
                        default:
                            return "/Images/MirrorsImages/MakeYourMirrorButton.png";//This key returns No available photo
                    }
                case MirrorSeries.Custom:
                case null:
                default:
                    return "/Images/MirrorsImages/MakeYourMirrorButton.png";//This key returns No available photo
            }
        }

        /// <summary>
        /// Gets the Alternative String for the Selected Mirror's Equivalent Photo
        /// When the Photo cannot be retrieved
        /// </summary>
        /// <returns></returns>
        public string GetAltSelectedMirrorPhotoPath()
        {
            string alt;

            if (Sandblast != null)
            {
                alt = lc.Keys[MirrorSandblastDescKey[(MirrorSandblast)Sandblast]];
            }
            else
            {
                if (Shape != null)
                {
                    alt = lc.Keys[MirrorShapeDescKey[(MirrorShape)Shape]];
                }
                else
                {
                    alt = "IMirrors (Photo Not Loaded)";
                }
            }

            return alt;
        }

        /// <summary>
        /// Returns the List of Photo Paths for the Selected Options of the Mirror
        /// </summary>
        /// <returns></returns>
        public List<string> GetSelectedOptionsPhotoPath()
        {
            List<string> photoPaths = [];

            //if (HasSwitch)      { photoPaths.Add(OptionsImagePaths[MirrorOption.TouchSwitch]); }
            //if (HasDimmer)      { photoPaths.Add(OptionsImagePaths[MirrorOption.DimmerSwitch]); }
            //if (HasSensor)      { photoPaths.Add(OptionsImagePaths[MirrorOption.SensorSwitch]); }
            //if (HasFog16)       { photoPaths.Add(OptionsImagePaths[MirrorOption.Fog16W]); }
            //if (HasFog24)       { photoPaths.Add(OptionsImagePaths[MirrorOption.Fog24W]); }
            //if (HasFog55)       { photoPaths.Add(OptionsImagePaths[MirrorOption.Fog55W]); }
            //if (IsFogDouble)    { /*DO NOTHING WE HAVE TO ADD ICON*/ }
            //if (HasMagnifyer)   { photoPaths.Add(OptionsImagePaths[MirrorOption.Zoom]); }
            //if (HasMagnifyerLed){ photoPaths.Add(OptionsImagePaths[MirrorOption.ZoomLed]); }
            //if (HasMagnifyerLedTouch) { photoPaths.Add(OptionsImagePaths[MirrorOption.ZoomLedTouch]); }
            //if (IsMagnifyerDouble) { /*DO NOTHING WE HAVE TO ADD ICON*/ }
            //if (HasClock)       { photoPaths.Add(OptionsImagePaths[MirrorOption.Clock]); }
            //if (HasBluetooth)   { photoPaths.Add(OptionsImagePaths[MirrorOption.BlueTooth]); }
            //if (HasDisplay11)   { photoPaths.Add(OptionsImagePaths[MirrorOption.DisplayRadio]); }
            //if (HasDisplay19)   { photoPaths.Add(OptionsImagePaths[MirrorOption.Display19]); }
            //if (HasDisplay20)   { photoPaths.Add(OptionsImagePaths[MirrorOption.Display20]); }
            //if (HasLid)         { photoPaths.Add(OptionsImagePaths[MirrorOption.IPLid]); }
            //if (HasRounding)    { photoPaths.Add(OptionsImagePaths[MirrorOption.RoundedCorners]); }
            //if (HasFogSwitch)   { photoPaths.Add(OptionsImagePaths[MirrorOption.TouchSwitchFog]); }

            foreach (var extra in Extras)
            {
                photoPaths.Add(OptionsImagePaths[extra.Option]);
            }

            return photoPaths;
        }

        #endregion

        public Mirror GetMirrorObject()
        {
            return mirror;
        }

        #region Z.Property Changed Event
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Z1.Dispose
        public void Dispose()
        {

        }
        #endregion

    }
}



using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.AngleModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HingesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.Inox304Models.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.NPModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations
{
    public class CabinMemoryRepository : ICabinMemoryRepository
    {
#nullable enable
        public CommonPartsCodes CommonCodes { get; }

        private readonly Dictionary<string, CabinPart> AllParts = new();
        private readonly Dictionary<string, Profile> AllProfiles = new();
        private readonly Dictionary<string, CabinHinge> AllHinges = new();
        private readonly Dictionary<string, GlassStrip> AllStrips = new();
        private readonly Dictionary<string, CabinAngle> AllAngles = new();
        private readonly Dictionary<string, SupportBar> AllSupportBars = new();
        private readonly Dictionary<string, CabinSupport> AllSupports = new();
        private readonly Dictionary<string, CabinHandle> AllHandles = new();

        public Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinConstraints> AllConstraints { get; } = new();
        public Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), CabinSettings> AllSettings { get; } = new();
        public Dictionary<(CabinModelEnum, CabinDrawNumber, CabinSynthesisModel), DefaultPartsList> DefaultLists { get; } = new();
        public List<CabinIdentifier> ValidIdentifiers { get; private set; }

        /// <summary>
        /// NOT IMPLEMENTED IN THIS REPO
        /// </summary>
        public Dictionary<string, Dictionary<CabinIdentifier, List<CabinPart>>> AdditionalPartsLists { get; private set; } = new();

        public List<CabinPart> GetAllParts()
        {
            return AllParts.Values.Select(p => p.GetDeepClone()).ToList();
        }
        public IEnumerable<CabinPart> GetAllPartsOriginal()
        {
            return AllParts.Values.ToList();
        }
        public CabinPart GetPart(string partCode)
        {
            if (AllParts.TryGetValue(partCode, out CabinPart? part))
            {
                return part.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {partCode} was not found in the Repository");
            }
        }

        #region -1.Get Default Overloads

        public CabinPart? GetDefaultPart(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot)
        {
            //Get the DefaultsList for the Part
            DefaultLists.TryGetValue((model, draw, synthesisModel), out DefaultPartsList? defaults);
            if (defaults is null) return null;

            //Get the Code from the Defaults
            defaults.SpotDefaults.TryGetValue(spot, out PartSpotDefaults? spotDefaults);
            if (spotDefaults is null || string.IsNullOrEmpty(spotDefaults.DefaultCode)) return null;
            
            var partToReturn = GetPart(spotDefaults.DefaultCode);
            if (partToReturn is not null) partToReturn.Quantity = spotDefaults.DefaultQuantity;

            return partToReturn;
        }
        public T? GetDefaultPart<T>(CabinModelEnum model, CabinDrawNumber draw , CabinSynthesisModel synthesisModel , PartSpot spot) 
            where T : CabinPart
        {
            return GetDefaultPart(model, draw, synthesisModel, spot) as T;
        }
        public T? GetDefaultPart<T>(CabinIdentifier identifier, PartSpot spot) where T : CabinPart
        {
            return GetDefaultPart(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot) as T;
        }
        public CabinPart? GetDefaultPart(CabinIdentifier identifier, PartSpot spot)
        {
            return GetDefaultPart(identifier.Model,identifier.DrawNumber, identifier.SynthesisModel, spot);
        }

        /// <summary>
        /// Gets the Default Part code for a sepcific structure and Position
        /// </summary>
        /// <param name="model"></param>
        /// <param name="draw"></param>
        /// <param name="synthesisModel"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        public string GetDefault(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot)
        {
            //Get the DefaultsList for the Part
            DefaultLists.TryGetValue((model, draw, synthesisModel), out DefaultPartsList? defaults);
            if (defaults is null) return "";

            //Get the Code from the Defaults
            defaults.SpotDefaults.TryGetValue(spot, out PartSpotDefaults? spotDefaults);
            if (spotDefaults is null || string.IsNullOrEmpty(spotDefaults.DefaultCode)) return "";

            return spotDefaults.DefaultCode;
        }
        public string GetDefault(CabinIdentifier identifier, PartSpot spot)
        {
            return GetDefault(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot);
        }

        /// <summary>
        /// Returns the Codes that are valid for a specific spot in the designated structure
        /// </summary>
        /// <param name="model"></param>
        /// <param name="draw"></param>
        /// <param name="synthesisModel"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSpotValids(CabinModelEnum model, CabinDrawNumber draw, CabinSynthesisModel synthesisModel, PartSpot spot)
        {
            //Get the DefaultsList for the Part
            DefaultLists.TryGetValue((model, draw, synthesisModel), out DefaultPartsList? defaults);
            if (defaults is null) return Enumerable.Empty<string>();

            //Get the Code from the Defaults
            defaults.SpotDefaults.TryGetValue(spot, out PartSpotDefaults? spotDefaults);
            if (spotDefaults is null) return Enumerable.Empty<string>();

            return spotDefaults.ValidCodes;
        }
        /// <summary>
        /// Returns the Codes that are valid for a specific spot in the designated structure
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        public IEnumerable<string> GetSpotValids(CabinIdentifier identifier, PartSpot spot) 
        {
            return GetSpotValids(identifier.Model, identifier.DrawNumber, identifier.SynthesisModel, spot);
        }
        #endregion

        public CabinMemoryRepository()
        {
            //Create the Codes of Common Parts
            CommonCodes = CreateCommonCodes();

            //Create the Parts
            CreateProfiles();
            CreateStrips();
            CreateHandles();
            CreateAngles();
            CreateHinges();
            CreateSupports();

            //Segragate The Parts into Categories
            AllParts.Values.ToList().ForEach(v =>
            {
                switch (v)
                {
                    case Profile p:
                        AllProfiles.Add(p.Code, p);
                        break;
                    case GlassStrip strip:
                        AllStrips.Add(strip.Code, strip);
                        break;
                    case CabinHinge hinge:
                        AllHinges.Add(hinge.Code, hinge);
                        break;
                    case CabinAngle angle:
                        AllAngles.Add(angle.Code, angle);
                        break;
                    case CabinSupport support:
                        AllSupports.Add(support.Code, support);
                        break;
                    case SupportBar supportBar:
                        AllSupportBars.Add(supportBar.Code, supportBar);
                        break;
                    case CabinHandle handle:
                        AllHandles.Add(handle.Code, handle);
                        break;
                    default:
                        break;
                }
            });

            CreateSettings();

            //Add All Valid Identifiers
            ValidIdentifiers = AllSettings.Keys.Select(k => new CabinIdentifier(k.Item1, k.Item2, k.Item3)).ToList();
        }

        #region 0.Get Methods

        private static CommonPartsCodes CreateCommonCodes()
        {
            CommonProfileCodes profiles = new()
            {
                Wall9S = "9SAL-10-WALL",
                Magnet9S = "9SAL-10-MAG",
                Magnet9B = "9BAL-10-MAG",
                WallSmartWS = "WSAL-10-WALL",
                WallW = "8WAL-10-WALL",
                Connector9FNoTollerance = "9FConnectorProfileNoTol",
                Connector9FWithTollerance = "9FConnectorProfileWithTol",
                HorizontalL0TypeA = "9SL0-10-A",
                HorizontalL0TypeB = "9SL0-10-B",
                HorizontalL0TypeQ = "9SL0-10-Q",
                HingeProfileNB = "NBAL-10-HINGE",
                HingeProfileQB = "QBAL-10-HINGE",
                MiddleHingeProfileNB = "NPAL-10-MHINGE",
                MagnetProfileUsual = "8WAL-10-MAG",
                FloorProfileLid = "0000-AL-FLOOR",
                HorizontalBarInox304 = "0000-10-VBAR",
            };
            CommonAngleCodes angles = new()
            {
                AngleA = "AngleA",
                AngleB = "AngleB",
                AngleQ = "AngleQ",
                AngleVA = "AngleVA",
            };
            CommonHandleCodes handles = new()
            {
                KnobHandle = "NA00-10-P",
                Inox304Handle = "VA00-10-P",
                B6000Handle = "9A00-10-P145",
            };
            CommonHingeCodes hinges = new()
            {
                Metal9BHinge = "Hinge9BMetal",
                Abs9BHinge = "Hinge9BAbs",
                HingeDB = "HingeDB",
                HingeHB = "HingeHB",
                HingeFlipper = "HingeFlipper",
                HingeMiddleNP = "MTMH-02",
                HingeMiddleMV = "MTMH-MV",
            };
            CommonStripCodes strips = new()
            {
                MagnetStripStraight = "8APL-MA-G18",
                MagnetStrip45Degrees = "8APL-MA-G90",
                MagnetStrip9B = "MagnetStrip9B",
                BumperStrip = "8APL-00-FU"
            };
            CommonSupportBarCodes supportBars = new()
            {
                SupportBarDefault = "8WAN-10-6010",
                SupportBarSmart = "SupportBarSmart",
            };
            CommonSupportCodes supports = new()
            {
                FloorStopper = "8WPO-10-SXDOW",
                SmallWallSupport = "0000-10-SUPPORT",
                Inox304DriverBottom = "BottomDriverInox304"
            };
            CommonPartsCodes parts = new()
            {
                Angles = angles,
                Handles = handles,
                SupportBars = supportBars,
                Hinges = hinges,
                Profiles = profiles,
                Strips = strips,
                Supports = supports
            };
            return parts;
        }

        public Profile GetProfile(string profileCode)
        {
            if (AllProfiles.TryGetValue(profileCode, out Profile? profile))
            {
                return profile.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {profileCode} was not found in the Repository-Profiles");
            }
        }
        public Profile GetProfileOriginal(string profileCode)
        {
            if (AllProfiles.TryGetValue(profileCode, out Profile? profile))
            {
                return profile;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {profileCode} was not found in the Repository-Profiles");
            }
        }

        public CabinHinge GetHinge(string hingeCode)
        {
            if (AllHinges.TryGetValue(hingeCode, out CabinHinge? hinge))
            {
                return hinge.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {hingeCode} was not found in the Repository-Hinges");
            }
        }
        public CabinHinge GetHingeOriginal(string hingeCode)
        {
            if (AllHinges.TryGetValue(hingeCode, out CabinHinge? hinge))
            {
                return hinge;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {hingeCode} was not found in the Repository-Hinges");
            }
        }

        public GlassStrip GetStrip(string stripCode)
        {
            if (AllStrips.TryGetValue(stripCode, out GlassStrip? strip))
            {
                return strip.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {stripCode} was not found in the Repository-Glass Strips");
            }
        }
        public GlassStrip GetStripOriginal(string stripCode)
        {
            if (AllStrips.TryGetValue(stripCode, out GlassStrip? strip))
            {
                return strip;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {stripCode} was not found in the Repository-Glass Strips");
            }
        }

        public CabinAngle GetAngle(string angleCode)
        {
            if (AllAngles.TryGetValue(angleCode, out CabinAngle? angle))
            {
                return angle.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {angleCode} was not found in the Repository-Angles");
            }
        }
        public CabinAngle GetAngleOriginal(string angleCode)
        {
            if (AllAngles.TryGetValue(angleCode, out CabinAngle? angle))
            {
                return angle;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {angleCode} was not found in the Repository-Angles");
            }
        }

        public SupportBar GetSupportBar(string supportBarCode)
        {
            if (AllSupportBars.TryGetValue(supportBarCode, out SupportBar? supportBar))
            {
                return supportBar.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {supportBarCode} was not found in the Repository-SupportBars");
            }
        }
        public SupportBar GetSupportBarOriginal(string supportBarCode)
        {
            if (AllSupportBars.TryGetValue(supportBarCode, out SupportBar? supportBar))
            {
                return supportBar;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {supportBarCode} was not found in the Repository-SupportBars");
            }
        }

        public CabinSupport GetSupport(string supportCode)
        {
            if (AllSupports.TryGetValue(supportCode, out CabinSupport? support))
            {
                return support.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {supportCode} was not found in the Repository-Supports");
            }
        }
        public CabinSupport GetSupportOriginal(string supportCode)
        {
            if (AllSupports.TryGetValue(supportCode, out CabinSupport? support))
            {
                return support;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {supportCode} was not found in the Repository-Supports");
            }
        }

        public CabinHandle GetHandle(string handleCode)
        {
            if (AllHandles.TryGetValue(handleCode, out CabinHandle? handle))
            {
                return handle.GetDeepClone();
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {handleCode} was not found in the Repository-Handles");
            }
        }
        public CabinHandle GetHandleOriginal(string handleCode)
        {
            if (AllHandles.TryGetValue(handleCode, out CabinHandle? handle))
            {
                return handle;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {handleCode} was not found in the Repository-Handles");
            }
        }

        #endregion

        #region 1. Parts Creation

        private void CreateProfiles()
        {
            #region 1.Connector Profiles

            Profile connector9F = new(CabinPartType.Profile, CabinProfileType.ConnectorProfile)
            {
                Code = CommonCodes.Profiles.Connector9FNoTollerance,
                Description = "Connector 9F Without Tollerance",
                ThicknessView = 39,
                InnerThicknessView = 39,
                PlacementWidth = 0,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                GlassInProfileDepth = 10,
                GlassCavityThickness = 9,
                Tollerance = 0,
                SideTollerance = -25,
            };
            AllParts.Add(connector9F.Code, connector9F);

            Profile connector9FWithTol = new(CabinPartType.Profile, CabinProfileType.ConnectorProfile)
            {
                Code = CommonCodes.Profiles.Connector9FWithTollerance,
                Description = "Connector 9F With Tollerance",
                ThicknessView = 45,
                InnerThicknessView = 39,
                PlacementWidth = 0,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                GlassInProfileDepth = 10,
                GlassCavityThickness = 9,
                Tollerance = 0,
                SideTollerance = 25,
            };
            AllParts.Add(connector9FWithTol.Code, connector9FWithTol);

            #endregion

            #region 2.L0 Profiles
            Profile l0TypeA = new(CabinPartType.Profile, CabinProfileType.GuideProfile)
            {
                Code = CommonCodes.Profiles.HorizontalL0TypeA,
                Description = "Horizontal Profile A",
                ThicknessView = 45, //Along with its Magnet Length
                PlacementWidth = 35,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                GlassInProfileDepth = 9,
                SliderDistance = 10,
                Tollerance = 0
            };
            AllParts.Add(l0TypeA.Code, l0TypeA);

            Profile l0TypeB = new(CabinPartType.Profile , CabinProfileType.GuideProfile)
            {
                Code = CommonCodes.Profiles.HorizontalL0TypeB,
                Description = "Horizontal Profile B",
                ThicknessView = 50, //Along with its Magnet Length
                PlacementWidth = 35,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                GlassInProfileDepth = 14,
                SliderDistance = 10,
                Tollerance = 0
            };
            AllParts.Add(l0TypeB.Code, l0TypeB);

            Profile l0TypeQ = new(CabinPartType.Profile, CabinProfileType.GuideProfile)
            {
                Code = CommonCodes.Profiles.HorizontalL0TypeQ,
                Description = "Horizontal Profile Q",
                ThicknessView = 50, //Along with its Magnet Length
                PlacementWidth = 35,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                GlassInProfileDepth = 14,
                SliderDistance = 10,
                Tollerance = 0
            };
            AllParts.Add(l0TypeQ.Code, l0TypeQ);

            Profile horizontalBarInox304 = new(CabinPartType.Profile, CabinProfileType.HorizontalBarProfile)
            {
                Code = CommonCodes.Profiles.HorizontalBarInox304,
                Description = "Horizontal Bar for Inox 304 Models",
                ThicknessView = 30, //Along with its Magnet Length
                PlacementWidth = 20,
                CutLength = 0,
                Material = MaterialType.Inox304,
                GlassInProfileDepth = 0,
                SliderDistance = 0,
                Tollerance = 0
            };
            AllParts.Add(horizontalBarInox304.Code, horizontalBarInox304);

            #endregion

            #region 3.Wall Profiles

            Profile wall9S = new(CabinPartType.Profile, CabinProfileType.WallProfile)
            {
                Code = CommonCodes.Profiles.Wall9S,
                Description = "Wall Profile 9S",
                ThicknessView = 40,
                PlacementWidth = 40,
                InnerThicknessView = 35,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 25,
                GlassInProfileDepth = 10,
            };
            AllParts.Add(wall9S.Code, wall9S);

            Profile magnet9S = new(CabinPartType.MagnetProfile, CabinProfileType.WallProfile)
            {
                Code = CommonCodes.Profiles.Magnet9S,
                Description = "Magnet Profile 9S",
                ThicknessView = 49, //Along with its Magnet Length
                PlacementWidth = 35,
                InnerThicknessView = 35,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 25,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Profiles/9SMagnetClosure.jpg",
            };
            AllParts.Add(magnet9S.Code, magnet9S);

            Profile magnet9B = new(CabinPartType.MagnetProfile, CabinProfileType.WallProfile)
            {
                Code = CommonCodes.Profiles.Magnet9B,
                Description = "Magnet Profile 9B",
                ThicknessView = 48, //Along with its Magnet Length
                PlacementWidth = 35,
                InnerThicknessView = 35,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 25
            };
            AllParts.Add(magnet9B.Code, magnet9B);

            Profile wallSmart = new(CabinPartType.Profile, CabinProfileType.WallProfile)
            {
                Code = CommonCodes.Profiles.WallSmartWS,
                Description = "Wall Profile WS",
                ThicknessView = 32,
                PlacementWidth = 50,
                InnerThicknessView = 0,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 0,
                GlassInProfileDepth = 23
            };
            AllParts.Add(wallSmart.Code, wallSmart);

            Profile wallW = new(CabinPartType.Profile, CabinProfileType.WallProfile)
            {
                Code = CommonCodes.Profiles.WallW,
                Description = "Wall Profile W",
                ThicknessView = 34,
                PlacementWidth = 17,
                InnerThicknessView = 30,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 15,
                GlassInProfileDepth = 10,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Profiles/Profile8W.png",
            };
            AllParts.Add(wallW.Code, wallW);

            #endregion

            #region 4.Profile Hinges
            ProfileHinge nbProfileHinge = new(CabinPartType.ProfileHinge, CabinProfileType.HingeProfile)
            {
                Code = CommonCodes.Profiles.HingeProfileNB,
                Description = "Profile Hinge NB",
                ThicknessView = 75,
                PlacementWidth = 40,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 25,
                BottomHeightBelowGlass = 20,
                GlassCavityThickness = 7,
                GlassInProfileDepth = 20,
                InnerThicknessView = 35,
                SliderDistance = 0,
                TopHeightAboveGlass = 20,
                PhotoPath = "Images/CabinImages/Series/ImgNB/Charachteristics/NiagaraProfile.jpg"

            };
            AllParts.Add(nbProfileHinge.Code, nbProfileHinge);

            ProfileHinge qbProfileHinge = new(CabinPartType.ProfileHinge, CabinProfileType.HingeProfile)
            {
                Code = CommonCodes.Profiles.HingeProfileQB,
                Description = "Profile Hinge QB",
                ThicknessView = 54,
                PlacementWidth = 38,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 17,
                BottomHeightBelowGlass = 11,
                GlassCavityThickness = 7,
                GlassInProfileDepth = 10,
                InnerThicknessView = 27,
                SliderDistance = 0,
                TopHeightAboveGlass = 14,
                PhotoPath = "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QProfile.jpg"
            };
            AllParts.Add(qbProfileHinge.Code, qbProfileHinge);

            ProfileHinge middleProfileHinge = new(CabinPartType.ProfileHinge, CabinProfileType.HingeProfile)
            {
                Code = CommonCodes.Profiles.MiddleHingeProfileNB,
                Description = "Middle Profile Hinge NB",
                ThicknessView = 44,
                PlacementWidth = 30,
                CutLength = 0,
                Material = MaterialType.Aluminium,
                Tollerance = 0,
                BottomHeightBelowGlass = 10,
                GlassCavityThickness = 7,
                GlassInProfileDepth = 8,
                TopHeightAboveGlass = 10,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Hinges/NVMiddleHinge.png",
            };
            AllParts.Add(middleProfileHinge.Code, middleProfileHinge);

            #endregion

            #region 5.CloseProfiles
            Profile magnetProfileUsual = new(CabinPartType.MagnetProfile, CabinProfileType.MagnetProfile)
            {
                Code = CommonCodes.Profiles.MagnetProfileUsual,
                ContainedStripCode = CommonCodes.Strips.MagnetStripStraight,
                Description = "Usual Magnet Profile Inox304",
                ThicknessView = 30,
                PlacementWidth = 20,
                CutLength = 0,
                Material = MaterialType.Inox304,
                Tollerance = 10,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Profiles/MagnetProfileStrip.jpg",
            };
            AllParts.Add(magnetProfileUsual.Code, magnetProfileUsual);

            #endregion

            #region 6.Bottom Profiles

            //This is Also Added to Supports 
            Profile floorProfileWithLid = new(CabinPartType.Profile, CabinProfileType.BottomGlassProfile)
            {
                Code = CommonCodes.Profiles.FloorProfileLid,
                Description = "Small Floor Profile With Lid",
                CutLength = 0,
                GlassInProfileDepth = 8,
                GlassCavityThickness = 9,
                Material = MaterialType.Aluminium,
                PhotoPath= "https://storagebronze.blob.core.windows.net/cabins-images/Parts/BottomFixers/0000-AL-FLOOR.jpg",
                PlacementWidth = 10,
                ThicknessView = 15,
                Tollerance = 0
            };
            AllParts.Add(floorProfileWithLid.Code, floorProfileWithLid);

            #endregion
        }

        private void CreateStrips()
        {
            GlassStrip straightMagnetStrip = new(CabinStripType.PolycarbonicMagnet)
            {
                Code = CommonCodes.Strips.MagnetStripStraight,
                Description = "Strip of Straight Magnet",
                CutLength = 0,
                Material = MaterialType.Polycarbonic,
                MetalLength = 0,
                InGlassLength = 10,
                OutOfGlassLength = 10,
                PolycarbonicLength = 20,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Strips/StraightMagnet.jpg",
            };
            AllParts.Add(straightMagnetStrip.Code, straightMagnetStrip);

            GlassStrip angularMagnetStrip = new(CabinStripType.PolycarbonicMagnet)
            {
                Code = CommonCodes.Strips.MagnetStrip45Degrees,
                Description = "Strip of Angular 45 Degrees Magnet",
                CutLength = 0,
                Material = MaterialType.Polycarbonic,
                MetalLength = 0,
                InGlassLength = 10,
                OutOfGlassLength = 10,
                PolycarbonicLength = 20,
                PhotoPath= "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Strips/CornerStrip.jpg",
            };
            AllParts.Add(angularMagnetStrip.Code, angularMagnetStrip);

            GlassStrip magnet9BStrip = new(CabinStripType.PolycarbonicMagnet)
            {
                Code = CommonCodes.Strips.MagnetStrip9B,
                Description = "Magnet Strip for 9B",
                CutLength = 0,
                Material = MaterialType.Aluminium,
                MetalLength = 20,
                InGlassLength = 10,
                OutOfGlassLength = 10,
                PolycarbonicLength = 0
            };
            AllParts.Add(magnet9BStrip.Code, magnet9BStrip);

            GlassStrip bumperStrip = new(CabinStripType.PolycarbonicBumper)
            {
                Code = CommonCodes.Strips.BumperStrip,
                Description = "Bumper Strip",
                CutLength = 0,
                Material = MaterialType.Polycarbonic,
                MetalLength = 0,
                InGlassLength = 10,
                OutOfGlassLength = 10,
                PolycarbonicLength = 20,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Strips/BumperStrip.jpg",
            };
            AllParts.Add(bumperStrip.Code, bumperStrip);
        }

        private void CreateHandles()
        {
            CabinHandle handleB6000 = new(CabinHandleType.HandleKnob)
            {
                Code = CommonCodes.Handles.B6000Handle,
                Description = "Handle for B6000",
                Material = MaterialType.Metallic,
                HandleComfortDistance = 20,
                HandleLengthToGlass = 20,
                HandleEdgesCornerRadius = 10,
                HandleHeightToGlass = 165,
                MinimumDistanceFromEdge = 30,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Handles/B6000Handle.jpg",
            };
            AllParts.Add(handleB6000.Code, handleB6000);

            CabinHandle inox304Handle = new(CabinHandleType.SingleKnob)
            {
                Code = CommonCodes.Handles.Inox304Handle,
                Description = "Handle for Inox304",
                Material = MaterialType.Inox304,
                HandleComfortDistance = 5,
                HandleLengthToGlass = 60,
                HandleHeightToGlass = 60,
                HandleEdgesCornerRadius = 30d,
                HandleOuterThickness = 10d,
                MinimumDistanceFromEdge = 30,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Handles/Inox304Handle.jpg",
            };
            AllParts.Add(inox304Handle.Code, inox304Handle);

            CabinHandle knobHandle = new(CabinHandleType.DoubleKnob)
            {
                Code = CommonCodes.Handles.KnobHandle,
                Description = "Knob Handle",
                Material = MaterialType.Abs,
                HandleComfortDistance = 20,
                HandleLengthToGlass = 50,
                HandleEdgesCornerRadius = 25,
                HandleHeightToGlass = 50,
                MinimumDistanceFromEdge = 30,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Handles/ABSKnobHandle.jpg",
            };
            AllParts.Add(knobHandle.Code, knobHandle);

            CabinHandle knobMetalHandle = new(CabinHandleType.DoubleKnob)
            {
                Code = "00NA-10-PM",
                Description = "Knob Metal Handle",
                Material = MaterialType.Brass,
                HandleComfortDistance = 20,
                HandleLengthToGlass = 45,
                HandleEdgesCornerRadius = 0,
                HandleHeightToGlass = 70,
                MinimumDistanceFromEdge = 30,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Handles/MetalDoubleKnobHandle.jpg",
            };
            AllParts.Add(knobMetalHandle.Code, knobMetalHandle);

            CabinHandle squareHandle = new(CabinHandleType.HandleKnob)
            {
                Code = "46PA-10-40",
                Description = "Single Square Handle",
                Material = MaterialType.Inox304,
                HandleComfortDistance = 20,
                HandleLengthToGlass = 20,
                HandleEdgesCornerRadius = 0,
                HandleHeightToGlass = 400,
                MinimumDistanceFromEdge = 30,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Handles/46PA-10-40.jpg",
            };
            AllParts.Add(squareHandle.Code, squareHandle);

            CabinHandle singleCircularHeavyDuty = new(CabinHandleType.HandleKnob)
            {
                Code = "VS00-10-P551",
                Description = "Single Heavy Duty",
                Material = MaterialType.Brass,
                HandleComfortDistance = 20,
                HandleLengthToGlass = 30,
                HandleEdgesCornerRadius = 15,
                HandleHeightToGlass = 552,
                MinimumDistanceFromEdge = 30,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Handles/VS00-10-P551.jpg",
            };
            AllParts.Add(singleCircularHeavyDuty.Code, singleCircularHeavyDuty);
        }

        private void CreateAngles()
        {
            CabinAngle angleA = new()
            {
                Code = CommonCodes.Angles.AngleA,
                Description = "9A Angle A",
                AngleDistanceFromDoor = 40,
                AngleLengthL0 = 33,
                Material = MaterialType.Abs,
                AngleHeight = 45,
            };
            AllParts.Add(angleA.Code, angleA);

            CabinAngle angleB = new()
            {
                Code = CommonCodes.Angles.AngleB,
                Description = "9A Angle B",
                AngleDistanceFromDoor = 40,
                AngleLengthL0 = 23,
                Material = MaterialType.Abs,
                AngleHeight = 50,
            };
            AllParts.Add(angleB.Code, angleB);

            CabinAngle angleQ = new()
            {
                Code = CommonCodes.Angles.AngleQ,
                Description = "9A Angle Q",
                AngleDistanceFromDoor = 40,
                AngleLengthL0 = 18,
                Material = MaterialType.Abs,
                AngleHeight = 50,
            };
            AllParts.Add(angleQ.Code, angleQ);
            CabinAngle angleVA = new()
            {
                Code = CommonCodes.Angles.AngleVA,
                Description = "VA Angle",
                AngleDistanceFromDoor = 40,
                AngleLengthL0 = 18,
                Material = MaterialType.Inox304
            };
            AllParts.Add(angleVA.Code, angleVA);
        }

        private void CreateHinges()
        {
            Hinge9B hinge9bMetal = new()
            {
                Code = CommonCodes.Hinges.Metal9BHinge,
                Description = "Metal 9B Hinge",
                CornerRadiusInGlass = 20,
                HeightView = 60,
                LengthView = 40,
                HingeOverlappingHeight = 2,
                Material = MaterialType.Brass,
                SupportTubeHeight = 40,
                SupportTubeLength = 15,
                GlassGapAER = 0, //Irrelevant aer is set from cabin Level here
            };
            AllParts.Add(hinge9bMetal.Code, hinge9bMetal);

            Hinge9B hinge9bAbs = new()
            {
                Code = CommonCodes.Hinges.Abs9BHinge,
                Description = "Abs 9B Hinge",
                CornerRadiusInGlass = 20,
                HeightView = 60,
                LengthView = 40,
                HingeOverlappingHeight = 2,
                Material = MaterialType.Abs,
                SupportTubeHeight = 40,
                SupportTubeLength = 15,
                GlassGapAER = 0, //Irrelevant aer is set from cabin Level here
            };
            AllParts.Add(hinge9bAbs.Code, hinge9bAbs);

            HingeDB hingeDB = new()
            {
                Code = CommonCodes.Hinges.HingeDB,
                Description = "Wall to Glass Hinge DB",
                HeightView = 76,
                LengthView = 59,
                Material = MaterialType.Inox304,
                InnerHeight = 68,
                GlassGapAER = 5,
                WallPlateThicknessView = 3,
            };
            AllParts.Add(hingeDB.Code, hingeDB);

            GlassToGlassHinge hingeFlipper = new()
            {
                Code = CommonCodes.Hinges.HingeFlipper,
                Description = "Flipper Panel Hinge",
                HeightView = 40,
                LengthView = 70,
                GlassGapAER = 5,
                InDoorLength = 45,
                Material = MaterialType.Brass,
            };
            AllParts.Add(hingeFlipper.Code, hingeFlipper);

            GlassToGlassHinge hingeHB = new()
            {
                Code = CommonCodes.Hinges.HingeHB,
                Description = "Glass to Glass HB Hinge",
                HeightView = 76,
                LengthView = 110,
                GlassGapAER = 5,
                InDoorLength = 50,
                Material = MaterialType.Inox304,
            };
            AllParts.Add(hingeHB.Code, hingeHB);

            GlassToGlassHinge hingeNP = new()
            {
                Code = CommonCodes.Hinges.HingeMiddleNP,
                Description = "Glass to Glass NP Hinge",
                HeightView = 60,
                LengthView = 130,
                GlassGapAER = 5,
                InDoorLength = 45,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Hinges/NPHinge.jpg",
                Material = MaterialType.Brass,
            };
            AllParts.Add(hingeNP.Code, hingeNP);

            GlassToGlassHinge hingeMV = new()
            {
                Code = CommonCodes.Hinges.HingeMiddleMV,
                Description = "Glass to Glass MV Hinge",
                HeightView = 60,
                LengthView = 130,
                GlassGapAER = 5,
                InDoorLength = 45,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Hinges/MVHinge.jpg",
                Material = MaterialType.Brass,
            };
            AllParts.Add(hingeMV.Code, hingeMV);
        }

        private void CreateSupports()
        {
            FloorStopperW floorStopper = new()
            {
                Code = CommonCodes.Supports.FloorStopper,
                Description = "Floor Stopper",
                HeightView = 12,
                LengthView = 30,
                OutOfGlassLength = 3,
                Material = MaterialType.Brass,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/BottomFixers/8WPO-10-SXDOW.png",
                GlassGapAER = 0,
                Tollerance = 0
            };
            AllParts.Add(floorStopper.Code, floorStopper);

            SupportBar supportBarDefault = new()
            {
                Code = CommonCodes.SupportBars.SupportBarDefault,
                Description = "SupportBar Default",
                ClampCenterDistanceFromGlassDefault = 80,
                ClampViewHeight = 40,
                ClampViewLength = 30,
                Material = MaterialType.Inox304,
                OutOfGlassHeight = 20,
                Placement = SupportBarPlacement.VarticallyToStructure
            };
            AllParts.Add(supportBarDefault.Code, supportBarDefault);

            SupportBar supportBarSmart = new()
            {
                Code = CommonCodes.SupportBars.SupportBarSmart,
                Description = "SupportBar for WS",
                ClampCenterDistanceFromGlassDefault = 80,
                ClampViewHeight = 40,
                ClampViewLength = 30,
                Material = MaterialType.Inox304,
                OutOfGlassHeight = 20,
                Placement = SupportBarPlacement.VarticallyToStructure
            };
            AllParts.Add(supportBarSmart.Code, supportBarSmart);

            CabinSupport smallWallSupport = new()
            {
                Code = CommonCodes.Supports.SmallWallSupport,
                Description = "Small Wall Support",
                HeightView = 45,
                LengthView = 45,
                Material = MaterialType.Inox304,
                GlassGapAER = 0,
                PhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Supports/SmallWallSupport.jpg",
            };
            AllParts.Add(smallWallSupport.Code, smallWallSupport);

            CabinSupport inox304Driver = new()
            {
                Code = CommonCodes.Supports.Inox304DriverBottom,
                Description = "Bottom Driver Inox 304",
                HeightView = 20,
                LengthView = 20,
                Material = MaterialType.Inox304,
                GlassGapAER = 0,
                Tollerance = 0,
            };
            AllParts.Add(inox304Driver.Code, inox304Driver);

        }

        #endregion

        #region 2. Settings Creation
        private void CreateSettings()
        {
                CreateSettings9S();
                CreateSettings94();
                CreateSettings9A();
                CreateSettings9C();
                CreateSettings9B();
                CreateSettings9F();
                CreateSettingsDB();
                CreateSettingsE();
                CreateSettingsW();
                CreateSettings8W40();
                CreateSettingsWFlipper();
                CreateSettingsHB();
                CreateSettingsV4();
                CreateSettingsVA();
                CreateSettingsVF();
                CreateSettingsVS();
                CreateSettingsNB();
                CreateSettingsQB();
                CreateSettingsNV();
                CreateSettingsNP();
                CreateSettingsQP();
                CreateSettingsMV2();
                CreateSettingsNV2();
                CreateSettingsWS();
        }
        private void CreateSettings9S()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw9S,
                CabinDrawNumber.Draw9S9F,
                CabinDrawNumber.Draw9S9F9F
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Serigraphy,
                GlassFinishEnum.Fume
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick6mm8mm,
                CabinThicknessEnum.Thick8mm,
            };
            Constraints9S constraints = new()
            {
                MaxDoorGlassLength = 784,
                MaxPossibleHeight = 2100,
                MaxPossibleLength = 1885,
                MinPossibleHeight = 600,
                MinPossibleLength = 885,
                MinPossibleStepHeight = 60,
                HeightBreakPointGlassThickness = 2000,
                LengthBreakPointGlassThickness = 1600,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm8mm,
                TolleranceMinusDefaultMinimum = 15,
                MinDoorDistanceFromWallOpened = 9,
                CoverDistance = 27,
                Overlap = 7,
                StepHeightTollerance = 9,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                ShouldHaveHandle = true,
                CanHaveStep = true,
            };

            Constraints9S constraints9S9F9F = new(constraints)
            {
                CanHaveStep = false
            };

            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip,false,CommonCodes.Strips.MagnetStripStraight,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.B6000Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults horizontalTop = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults horizontalBottom = new(PartSpot.HorizontalGuideBottom, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults wallProfile2 = new(PartSpot.WallSide2, false, CommonCodes.Profiles.Magnet9S,1);
            PartSpotDefaults wallProfile1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults wallProfile1Tol = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Connector9FWithTollerance,1);
            PartSpotDefaults stepBottom = new(PartSpot.StepBottomSide, true, CommonCodes.Profiles.FloorProfileLid,1);

            DefaultPartsList defaultList9S = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2 },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };
            DefaultPartsList defaultList9SF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2 },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };
            DefaultPartsList defaultList9SFF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1Tol},
                    { PartSpot.WallSide2,               wallProfile2 },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };

            CabinSettings settings9S = new()
            {
                Model = CabinModelEnum.Model9S,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1850,
                NominalLength = 1200,
                IsReversible = true,
                MetalFinish = CabinFinishEnum.Polished
            };

            AllConstraints.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S9F, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Primary), constraints9S9F9F);

            DefaultLists.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S, CabinSynthesisModel.Primary), defaultList9S);
            DefaultLists.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S9F, CabinSynthesisModel.Primary), defaultList9SF);
            DefaultLists.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Primary), defaultList9SFF);

            AllSettings.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S, CabinSynthesisModel.Primary), settings9S);
            AllSettings.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S9F, CabinSynthesisModel.Primary), settings9S);
            AllSettings.Add((CabinModelEnum.Model9S, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Primary), settings9S);
        }
        private void CreateSettings94()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw94,
                CabinDrawNumber.Draw949F,
                CabinDrawNumber.Draw949F9F
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Serigraphy,
                GlassFinishEnum.Fume
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick6mm8mm,
                CabinThicknessEnum.Thick8mm,
            };
            Constraints94 constraints = new()
            {
                MaxDoorGlassLength = 430,
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 2300,
                MinPossibleHeight = 600,
                MinPossibleLength = 1085,
                MinPossibleStepHeight = 60,
                CoverDistanceMaxOpening = 28,
                CoverDistanceSameGlasses = -90,
                TolleranceMinusDefaultMinimum = 15,
                MinDoorDistanceFromWallOpened = 9,
                HeightBreakPointGlassThickness = 2000,
                LengthBreakPointGlassThickness = 2000,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm8mm,
                CoverDistance = 28,
                Overlap = 7,
                StepHeightTollerance = 9,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                ShouldHaveHandle = true,
                CanHaveStep = true,
                SameGlassesLengths = new List<int>() {1600,1700,1800 },
            };
            Constraints94 constraints949F9F = new(constraints)
            {
                CanHaveStep = false,
            };

            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.B6000Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults handle2 = new(PartSpot.Handle2, false, CommonCodes.Handles.B6000Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults horizontalTop = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults horizontalBottom = new(PartSpot.HorizontalGuideBottom, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults wallProfile2 = new(PartSpot.WallSide2, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults wallProfile1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults wallProfile2Tol = new(PartSpot.WallSide2, false, CommonCodes.Profiles.Connector9FWithTollerance,1);
            PartSpotDefaults stepBottom = new(PartSpot.StepBottomSide, true, CommonCodes.Profiles.FloorProfileLid,1);

            DefaultPartsList defaultList94 = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.Handle2,                 handle2 },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2 },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };
            DefaultPartsList defaultList94F = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.Handle2,                 handle2 },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2Tol },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };
            DefaultPartsList defaultList94FF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.Handle2,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile2Tol },
                    { PartSpot.WallSide2,               wallProfile2Tol },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };

            CabinSettings settings94 = new()
            {
                Model = CabinModelEnum.Model94,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1850,
                NominalLength = 1600,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw94, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw949F, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Primary), constraints949F9F);
            
            DefaultLists.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw94, CabinSynthesisModel.Primary), defaultList94);
            DefaultLists.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw949F, CabinSynthesisModel.Primary), defaultList94F);
            DefaultLists.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Primary), defaultList94FF);

            AllSettings.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw94, CabinSynthesisModel.Primary), settings94);
            AllSettings.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw949F, CabinSynthesisModel.Primary), settings94);
            AllSettings.Add((CabinModelEnum.Model94, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Primary), settings94);

        }
        private void CreateSettings9A()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw9A,
                CabinDrawNumber.Draw9A9F
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Serigraphy,
                GlassFinishEnum.Fume
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick6mm8mm,
                CabinThicknessEnum.Thick8mm,
            };
            Constraints9A constraints = new()
            {
                MaxDoorGlassLength = 550,
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1400,
                MinPossibleHeight = 600,
                MinPossibleLength = 285,
                MinPossibleStepHeight = 60,
                TolleranceMinusDefaultMinimum = 15,
                MinDoorDistanceFromWallOpened = 9,
                HeightBreakPointGlassThickness = 2000,
                LengthBreakPointGlassThickness = 1250,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm8mm,
                CoverDistance = 30,
                Overlap = 7,
                StepHeightTollerance = 9,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                ShouldHaveHandle = true,
                CanHaveStep = true,
            };

            Constraints9A constraints9A9FPrimary = new(constraints)
            {
                CanHaveStep = false,
            };

            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.B6000Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults horizontalTop = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults horizontalBottom = new(PartSpot.HorizontalGuideBottom, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults wallProfile1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults wallProfile1Tol = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Connector9FWithTollerance,1);
            PartSpotDefaults stepBottom = new(PartSpot.StepBottomSide, true, CommonCodes.Profiles.FloorProfileLid,1);
            PartSpotDefaults angle = new(PartSpot.Angle, false, CommonCodes.Angles.AngleA,1);

            DefaultPartsList defaultList9A = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.StepBottomSide,          stepBottom},
                    { PartSpot.Angle ,                  angle},
                }
            };
            DefaultPartsList defaultList9AF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1Tol},
                    { PartSpot.StepBottomSide,          stepBottom},
                    { PartSpot.Angle ,                  angle},
                }
            };

            CabinSettings settings9A = new()
            {
                Model = CabinModelEnum.Model9A,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1850,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Primary), constraints9A9FPrimary);
            AllConstraints.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A, CabinSynthesisModel.Primary), defaultList9A);
            DefaultLists.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A, CabinSynthesisModel.Secondary), defaultList9A);
            DefaultLists.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Primary), defaultList9A);
            DefaultLists.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Secondary), defaultList9AF);

            AllSettings.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A, CabinSynthesisModel.Primary), settings9A);
            AllSettings.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A, CabinSynthesisModel.Secondary), settings9A);
            AllSettings.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Primary), settings9A);
            AllSettings.Add((CabinModelEnum.Model9A, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Secondary), settings9A);
        }
        private void CreateSettings9B()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw9B,
                CabinDrawNumber.Draw9B9F,
                CabinDrawNumber.Draw9B9F9F
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Serigraphy,
                GlassFinishEnum.Fume
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            Constraints9B constraints = new()
            {
                MaxPossibleHeight = 1900,
                MaxPossibleLength = 1500,
                MinPossibleHeight = 1200,
                MinPossibleLength = 685,
                MinPossibleStepHeight = 60,
                AddedFixedGlassLengthBreakpoint = 950,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                CorrectionOfL0Length = 5,
                GlassGapAERHorizontal = 5,
                GlassGapAERVertical = 5,
                HingeDistanceFromDoorGlass = 100,
                MaxPossibleDoorLength = 755,
                MinPossibleDoorLength = 590,
                MinPossibleFixedLength = 100,
                TolleranceMinusDefaultMinimum = 15,
                StepHeightTollerance = 9,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                ShouldHaveHandle = true,
                CanHaveStep = false,
            };

            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip9B,1);
            PartSpotDefaults hinge = new(PartSpot.PivotHinge, false, CommonCodes.Hinges.Metal9BHinge,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.B6000Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults horizontalTop = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalL0TypeB,1);
            PartSpotDefaults horizontalBottom = new(PartSpot.HorizontalGuideBottom, false, CommonCodes.Profiles.HorizontalL0TypeB,1);
            PartSpotDefaults wallProfile2 = new(PartSpot.WallSide2, false, CommonCodes.Profiles.Magnet9B,1);
            PartSpotDefaults wallProfile1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults wallProfile1Tol = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Connector9FWithTollerance,1);
            PartSpotDefaults stepBottom = new(PartSpot.StepBottomSide, true, CommonCodes.Profiles.FloorProfileLid,1);

            DefaultPartsList defaultList9B = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.PivotHinge,              hinge },        
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2 },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };
            DefaultPartsList defaultList9BF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.PivotHinge,              hinge },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2},
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };
            DefaultPartsList defaultList9BFF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.PivotHinge,              hinge },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1Tol},
                    { PartSpot.WallSide2,               wallProfile2},
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };

            CabinSettings settings9B = new()
            {
                Model = CabinModelEnum.Model9B,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1850,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B9F, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Primary), constraints);
            
            DefaultLists.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B, CabinSynthesisModel.Primary), defaultList9B);
            DefaultLists.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B9F, CabinSynthesisModel.Primary), defaultList9BF);
            DefaultLists.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Primary), defaultList9BFF);

            AllSettings.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B, CabinSynthesisModel.Primary), settings9B);
            AllSettings.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B9F, CabinSynthesisModel.Primary), settings9B);
            AllSettings.Add((CabinModelEnum.Model9B, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Primary), settings9B);

        }
        private void CreateSettings9F()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw9S9F,
                CabinDrawNumber.Draw949F,
                CabinDrawNumber.Draw9B9F,
                CabinDrawNumber.Draw9A9F,
                CabinDrawNumber.Draw9C9F,
                CabinDrawNumber.Draw9S9F9F,
                CabinDrawNumber.Draw949F9F,
                CabinDrawNumber.Draw9B9F9F
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Serigraphy,
                GlassFinishEnum.Fume
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick8mm,
            };
            Constraints9F constraints = new()
            {
                MaxPossibleHeight = 2100,
                MaxPossibleLength = 1400,
                MinPossibleHeight = 600,
                MinPossibleLength = 185,
                MinPossibleStepHeight = 60,
                TolleranceMinusDefaultMinimum = 15,
                HeightBreakPointGlassThickness = 2000,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm8mm,
                StepHeightTollerance = 9,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                CanHaveStep = true,
            };

            PartSpotDefaults horizontalTop = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalL0TypeB,1);
            PartSpotDefaults horizontalBottom = new(PartSpot.HorizontalGuideBottom, false, CommonCodes.Profiles.HorizontalL0TypeB,1);
            PartSpotDefaults wallProfile2NoTol = new(PartSpot.NonWallSide, false, CommonCodes.Profiles.Connector9FNoTollerance,1);
            PartSpotDefaults wallProfile2Tol = new(PartSpot.NonWallSide, false, CommonCodes.Profiles.Connector9FWithTollerance,1);
            PartSpotDefaults wallProfile1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults stepBottom = new(PartSpot.StepBottomSide, true, CommonCodes.Profiles.FloorProfileLid,1);

            DefaultPartsList defaultList9FNoTol = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.NonWallSide,             wallProfile2NoTol },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };

            DefaultPartsList defaultList9FTol = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.NonWallSide,             wallProfile2Tol },
                    { PartSpot.StepBottomSide,          stepBottom},
                }
            };

            CabinSettings settings9F = new()
            {
                Model = CabinModelEnum.Model9F,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1850,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Tertiary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Tertiary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Tertiary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Tertiary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Tertiary), constraints);

            
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F, CabinSynthesisModel.Secondary),     defaultList9FNoTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Secondary),   defaultList9FTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Tertiary),    defaultList9FNoTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Tertiary),      defaultList9FTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F, CabinSynthesisModel.Secondary),     defaultList9FTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Secondary),   defaultList9FTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Tertiary),    defaultList9FTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F, CabinSynthesisModel.Secondary),     defaultList9FNoTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Secondary),   defaultList9FNoTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Tertiary),    defaultList9FTol);
            DefaultLists.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Tertiary),      defaultList9FTol);

            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F, CabinSynthesisModel.Secondary),    settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Secondary),  settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9S9F9F, CabinSynthesisModel.Tertiary),   settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9A9F, CabinSynthesisModel.Tertiary),     settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F, CabinSynthesisModel.Secondary),    settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Secondary),  settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw949F9F, CabinSynthesisModel.Tertiary),   settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F, CabinSynthesisModel.Secondary),    settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Secondary),  settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9B9F9F, CabinSynthesisModel.Tertiary),   settings9F);
            AllSettings.Add((CabinModelEnum.Model9F, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Tertiary),     settings9F);
        }
        private void CreateSettings9C()
        {
            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw9C,
                CabinDrawNumber.Draw9C9F
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Serigraphy
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick6mm8mm
            };
            Constraints9C constraints = new()
            {
                MaxPossibleHeight = 1850,
                MaxPossibleLength = 985,
                MinPossibleHeight = 1850,
                MinPossibleLength = 685,
                MinPossibleStepHeight = 0,
                TolleranceMinusDefaultMinimum = 15,
                AllowableSerigraphyLengths = new List<int>() { 800, 900 },
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ShouldHaveHandle = true,
                CanHaveStep = false
            };

            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.B6000Handle,1, CommonCodes.Handles.B6000Handle);
            PartSpotDefaults horizontalTop = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults horizontalBottom = new(PartSpot.HorizontalGuideBottom, false, CommonCodes.Profiles.HorizontalL0TypeA,1);
            PartSpotDefaults wallProfile2 = new(PartSpot.WallSide2, false, CommonCodes.Profiles.Wall9S,1);
            PartSpotDefaults wallProfileTol = new(PartSpot.WallSide2, false, CommonCodes.Profiles.Connector9FWithTollerance,1);
            PartSpotDefaults wallProfile1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.Wall9S,1);

            DefaultPartsList defaultList9C = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfile2 },
                }
            };
            DefaultPartsList defaultList9CF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalTop },
                    { PartSpot.HorizontalGuideBottom,   horizontalBottom },
                    { PartSpot.WallSide1,               wallProfile1},
                    { PartSpot.WallSide2,               wallProfileTol },
                }
            };

            CabinSettings settings9C = new()
            {
                Model = CabinModelEnum.Model9C,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1850,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C, CabinSynthesisModel.Primary), defaultList9C);
            DefaultLists.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C, CabinSynthesisModel.Secondary), defaultList9C);
            DefaultLists.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Primary), defaultList9CF);
            DefaultLists.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Secondary), defaultList9CF);

            AllSettings.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C, CabinSynthesisModel.Primary), settings9C);
            AllSettings.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C, CabinSynthesisModel.Secondary), settings9C);
            AllSettings.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Primary), settings9C);
            AllSettings.Add((CabinModelEnum.Model9C, CabinDrawNumber.Draw9C9F, CabinSynthesisModel.Secondary), settings9C);
        }
        private void CreateSettingsDB()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawDB51,
                CabinDrawNumber.DrawCornerDB8W52,
                CabinDrawNumber.Draw2CornerDB53,
                CabinDrawNumber.DrawStraightDB8W59,
                CabinDrawNumber.Draw2StraightDB61,
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm
            };
            ConstraintsDB constraints = new()
            {
                MaxPossibleHeight = 2050,
                MaxPossibleLength = 800,
                MinPossibleHeight = 700,
                MinPossibleLength = 200,
                TolleranceMinusDefaultMinimum = 15,
                MaxDoorGlassLength = 850,
                DoorDistanceFromBottom = 15,
                ValidDrawNumbers = validDraws,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                CanHaveStep = false,
            };

            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, CommonCodes.Strips.MagnetStripStraight,1, CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.KnobHandle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults hinge = new(PartSpot.WallHinge, false, CommonCodes.Hinges.HingeDB,1);
            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);

            DefaultPartsList defaultDBParts = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.WallHinge,               hinge },
                    { PartSpot.CloseProfile,            closeProfile},
                }
            };

            PartSpotDefaults closeStripStraight = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            DefaultPartsList defaultDBPartsStraight = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStripStraight },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.WallHinge,               hinge },
                    { PartSpot.CloseProfile,            closeProfileEmpty},
                    //Without Close Profile
                }
            };

            PartSpotDefaults closeStripAngle = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            DefaultPartsList defaultDBPartsComboAngle = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseStrip ,             closeStripAngle },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.WallHinge,               hinge },
                    { PartSpot.CloseProfile,            closeProfileEmpty},
                    //Without Close Profile
                }
            };
            
            CabinSettings settingsDB = new()
            {
                Model = CabinModelEnum.ModelDB,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 650,
                IsReversible = true,
                MetalFinish = CabinFinishEnum.Polished,
            };

            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawDB51, CabinSynthesisModel.Primary),             constraints);
            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawCornerDB8W52, CabinSynthesisModel.Primary),     constraints);
            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2CornerDB53, CabinSynthesisModel.Primary),      constraints);
            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2CornerDB53, CabinSynthesisModel.Secondary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawStraightDB8W59, CabinSynthesisModel.Primary),   constraints);
            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2StraightDB61, CabinSynthesisModel.Primary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2StraightDB61, CabinSynthesisModel.Secondary),  constraints);

            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawDB51, CabinSynthesisModel.Primary),             defaultDBParts);
            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawCornerDB8W52, CabinSynthesisModel.Primary),     defaultDBPartsComboAngle);
            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2CornerDB53, CabinSynthesisModel.Primary),      defaultDBPartsComboAngle);
            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2CornerDB53, CabinSynthesisModel.Secondary),    defaultDBPartsComboAngle);
            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawStraightDB8W59, CabinSynthesisModel.Primary),   defaultDBPartsStraight);
            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2StraightDB61, CabinSynthesisModel.Primary),    defaultDBPartsStraight);
            DefaultLists.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2StraightDB61, CabinSynthesisModel.Secondary),  defaultDBPartsStraight);

            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawDB51, CabinSynthesisModel.Primary),             settingsDB);
            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawCornerDB8W52, CabinSynthesisModel.Primary),     settingsDB);
            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2CornerDB53, CabinSynthesisModel.Primary),      settingsDB);
            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2CornerDB53, CabinSynthesisModel.Secondary),    settingsDB);
            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.DrawStraightDB8W59, CabinSynthesisModel.Primary),   settingsDB);
            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2StraightDB61, CabinSynthesisModel.Primary),    settingsDB);
            AllSettings.Add((CabinModelEnum.ModelDB, CabinDrawNumber.Draw2StraightDB61, CabinSynthesisModel.Secondary),  settingsDB);
        }
        private void CreateSettingsE()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawE
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm
            };
            ConstraintsE constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1800,
                MinPossibleHeight = 250,
                MinPossibleLength = 200,
                TolleranceMinusDefaultMinimum = 0,
                CornerRadiusTopEdge = 0,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm,
                CanHaveStep = false,
            };

            PartSpotDefaults bottomFixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.FloorStopper,1, CommonCodes.Profiles.FloorProfileLid);
            PartSpotDefaults supportBar = new(PartSpot.SupportBar, false, CommonCodes.SupportBars.SupportBarDefault,1);

            DefaultPartsList defaultListE = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            CabinSettings settingsE = new()
            {
                Model = CabinModelEnum.ModelE,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 2000,
                NominalLength = 800,
                IsReversible = true,
                MetalFinish = CabinFinishEnum.Polished,
            };
            
            AllConstraints.Add((CabinModelEnum.ModelE, CabinDrawNumber.DrawE, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelE, CabinDrawNumber.DrawE, CabinSynthesisModel.Primary), defaultListE);

            AllSettings.Add((CabinModelEnum.ModelE, CabinDrawNumber.DrawE, CabinSynthesisModel.Primary), settingsE);
        }
        private void CreateSettingsW()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw8W,
                CabinDrawNumber.Draw8WFlipper81,
                CabinDrawNumber.Draw2Corner8W82,
                CabinDrawNumber.Draw1Corner8W84,
                CabinDrawNumber.Draw2Straight8W85,
                CabinDrawNumber.Draw2CornerStraight8W88,
                CabinDrawNumber.DrawCornerNP6W45,
                CabinDrawNumber.DrawCornerQP6W45,
                CabinDrawNumber.DrawStraightNP6W47,
                CabinDrawNumber.DrawStraightQP6W47,
                CabinDrawNumber.DrawCornerNB6W32,
                CabinDrawNumber.DrawCornerQB6W32,
                CabinDrawNumber.DrawStraightNB6W38,
                CabinDrawNumber.DrawStraightQB6W38,
                CabinDrawNumber.DrawCornerDB8W52,
                CabinDrawNumber.DrawStraightDB8W59,
                CabinDrawNumber.DrawCornerHB8W35,
                CabinDrawNumber.DrawStraightHB8W40,
                CabinDrawNumber.Draw8W40
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm,
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm
            };

            ConstraintsW constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1592,
                MinPossibleHeight = 200,
                MinPossibleLength = 142,
                TolleranceMinusDefaultMinimum = 8,
                MinimumFreeOpening = 600,
                CornerRadiusTopEdge = 0,
                StepHeightTollerance = 9,
                StepLengthTolleranceMin = 5,
                FinalHeightCorrection = 0,
                LengthBreakPointGlassThickness = 1092,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm,
                ValidDrawNumbers = validDraws,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                MinPossibleStepHeight = 0,
                CanHavePerimetricalFrame = true,
                CanHaveStep = true,
            };
            ConstraintsW constraintsCombo = new(constraints) { CanHavePerimetricalFrame = false };
            ConstraintsW constraintsComboWithHeightCorrection = new(constraints) { CanHavePerimetricalFrame = false ,FinalHeightCorrection = 20};
            ConstraintsW constraintsComboQWithHeightCorrection = new(constraints) { CanHavePerimetricalFrame = false, FinalHeightCorrection = 14 };
            ConstraintsW constraintsComboNoStep = new(constraintsCombo) { CanHaveStep = false };

            PartSpotDefaults bottomFixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.FloorStopper,1, CommonCodes.Profiles.FloorProfileLid);
            PartSpotDefaults bottomFixerWithWallW = new(PartSpot.BottomSide1, false, CommonCodes.Supports.FloorStopper,1, CommonCodes.Profiles.FloorProfileLid,CommonCodes.Profiles.WallW);
            PartSpotDefaults sideFixer = new(PartSpot.NonWallSide, true, "",1, CommonCodes.Profiles.WallW);
            PartSpotDefaults topFixer = new(PartSpot.TopSide, true, "",1, CommonCodes.Profiles.WallW);
            PartSpotDefaults wallFixer = new(PartSpot.WallSide1, false, CommonCodes.Profiles.WallW,1, CommonCodes.Supports.SmallWallSupport);
            PartSpotDefaults supportBar = new(PartSpot.SupportBar, true, CommonCodes.SupportBars.SupportBarDefault,1);
            
            PartSpotDefaults closeStripEmpty = new(PartSpot.CloseStrip, true, "",1);
            PartSpotDefaults sideFixerEmpty = new(PartSpot.NonWallSide, true, "",1);
            PartSpotDefaults topFixerEmpty = new(PartSpot.TopSide, true, "",1);

            DefaultPartsList defaultListW = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixerWithWallW },
                    { PartSpot.NonWallSide ,            sideFixer },
                    { PartSpot.TopSide ,                topFixer },
                    { PartSpot.WallSide1 ,              wallFixer },
                    { PartSpot.CloseStrip,              closeStripEmpty },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            DefaultPartsList defaultListWwithFlipper = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.NonWallSide ,            sideFixerEmpty },
                    { PartSpot.TopSide ,                topFixerEmpty },
                    { PartSpot.WallSide1 ,              wallFixer },
                    { PartSpot.CloseStrip,              closeStripEmpty },
                    { PartSpot.SupportBar,              supportBar },
                }
            };
            
            PartSpotDefaults sideFixer2 = new(PartSpot.NonWallSide, false, CommonCodes.Supports.SmallWallSupport,1);
            DefaultPartsList defaultListWwithCorner = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.NonWallSide ,            sideFixer2 },
                    { PartSpot.TopSide ,                topFixerEmpty },
                    { PartSpot.WallSide1 ,              wallFixer },
                    { PartSpot.CloseStrip,              closeStripEmpty },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            PartSpotDefaults wallFixerCorner = new(PartSpot.WallSide1, false, CommonCodes.Supports.SmallWallSupport,1);
            DefaultPartsList defaultListWbeingCorner = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.NonWallSide ,            sideFixerEmpty },
                    { PartSpot.TopSide ,                topFixerEmpty },
                    { PartSpot.WallSide1 ,              wallFixerCorner },
                    { PartSpot.CloseStrip,              closeStripEmpty },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            PartSpotDefaults closeStripStraight = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            DefaultPartsList defaultListWwithStraightDoor = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.NonWallSide ,            sideFixerEmpty },
                    { PartSpot.TopSide ,                topFixerEmpty },
                    { PartSpot.WallSide1 ,              wallFixer },
                    { PartSpot.CloseStrip,              closeStripStraight },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            PartSpotDefaults closeStripCorner = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            DefaultPartsList defaultListWwithCornerDoor = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.NonWallSide ,            sideFixerEmpty },
                    { PartSpot.TopSide ,                topFixerEmpty },
                    { PartSpot.WallSide1 ,              wallFixer },
                    { PartSpot.CloseStrip,              closeStripCorner },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            CabinSettings settingsW = new()
            {
                Model = CabinModelEnum.ModelW,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 2000,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true
            };
            CabinSettings settingsWFlipper = new()
            {
                Model = CabinModelEnum.ModelW,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 2000,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = false,
            };

            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8W, CabinSynthesisModel.Primary),                      constraints);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Primary),             constraintsCombo);

            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Corner8W82, CabinSynthesisModel.Primary),             constraintsCombo);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Corner8W82, CabinSynthesisModel.Secondary),           constraintsCombo);

            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw1Corner8W84, CabinSynthesisModel.Primary),             constraintsCombo);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw1Corner8W84, CabinSynthesisModel.Secondary),           constraintsComboNoStep);

            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Straight8W85, CabinSynthesisModel.Primary),           constraints);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Straight8W85, CabinSynthesisModel.Secondary),         constraints);

            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Primary),     constraintsCombo);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Secondary),   constraintsComboNoStep);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Tertiary),    constraintsCombo);

            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerNP6W45, CabinSynthesisModel.Secondary),          constraintsComboWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerQP6W45, CabinSynthesisModel.Secondary),          constraintsComboQWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightNP6W47, CabinSynthesisModel.Secondary),        constraintsComboWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightQP6W47, CabinSynthesisModel.Secondary),        constraintsComboQWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerNB6W32, CabinSynthesisModel.Secondary),          constraintsComboWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerQB6W32, CabinSynthesisModel.Secondary),          constraintsComboQWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightNB6W38, CabinSynthesisModel.Secondary),        constraintsComboWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightQB6W38, CabinSynthesisModel.Secondary),        constraintsComboQWithHeightCorrection);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerDB8W52, CabinSynthesisModel.Secondary),          constraintsCombo);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightDB8W59, CabinSynthesisModel.Secondary),        constraintsCombo);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerHB8W35, CabinSynthesisModel.Secondary),          constraintsCombo);
            AllConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightHB8W40, CabinSynthesisModel.Secondary),        constraintsCombo);
            
            //declared in 8W
            //allConstraints.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8W40, CabinSynthesisModel.Primary),                  constraints);

            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8W, CabinSynthesisModel.Primary),                      defaultListW);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Primary),             defaultListWwithFlipper);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Corner8W82, CabinSynthesisModel.Primary),             defaultListW);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Corner8W82, CabinSynthesisModel.Secondary),           defaultListW);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw1Corner8W84, CabinSynthesisModel.Primary),             defaultListWwithCorner);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw1Corner8W84, CabinSynthesisModel.Secondary),           defaultListWbeingCorner);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Straight8W85, CabinSynthesisModel.Primary),           defaultListW);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Straight8W85, CabinSynthesisModel.Secondary),         defaultListW);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Primary),     defaultListWwithCorner);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Secondary),   defaultListWbeingCorner);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Tertiary),    defaultListW);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerNP6W45, CabinSynthesisModel.Secondary),          defaultListWwithCornerDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerQP6W45, CabinSynthesisModel.Secondary),          defaultListWwithCornerDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightNP6W47, CabinSynthesisModel.Secondary),        defaultListWwithStraightDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightQP6W47, CabinSynthesisModel.Secondary),        defaultListWwithStraightDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerNB6W32, CabinSynthesisModel.Secondary),          defaultListWwithCornerDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerQB6W32, CabinSynthesisModel.Secondary),          defaultListWwithCornerDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightNB6W38, CabinSynthesisModel.Secondary),        defaultListWwithStraightDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightQB6W38, CabinSynthesisModel.Secondary),        defaultListWwithStraightDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerDB8W52, CabinSynthesisModel.Secondary),          defaultListWwithCornerDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightDB8W59, CabinSynthesisModel.Secondary),        defaultListWwithStraightDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerHB8W35, CabinSynthesisModel.Secondary),          defaultListWwithCornerDoor);
            DefaultLists.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightHB8W40, CabinSynthesisModel.Secondary),        defaultListWwithStraightDoor);

            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8W, CabinSynthesisModel.Primary),                      settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Primary),             settingsWFlipper);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Corner8W82, CabinSynthesisModel.Primary),             settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Corner8W82, CabinSynthesisModel.Secondary),           settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw1Corner8W84, CabinSynthesisModel.Primary),             settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw1Corner8W84, CabinSynthesisModel.Secondary),           settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Straight8W85, CabinSynthesisModel.Primary),           settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2Straight8W85, CabinSynthesisModel.Secondary),         settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Primary),     settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Secondary),   settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Tertiary),    settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerNP6W45, CabinSynthesisModel.Secondary),          settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerQP6W45, CabinSynthesisModel.Secondary),          settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightNP6W47, CabinSynthesisModel.Secondary),        settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightQP6W47, CabinSynthesisModel.Secondary),        settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerNB6W32, CabinSynthesisModel.Secondary),          settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerQB6W32, CabinSynthesisModel.Secondary),          settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightNB6W38, CabinSynthesisModel.Secondary),        settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightQB6W38, CabinSynthesisModel.Secondary),        settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerDB8W52, CabinSynthesisModel.Secondary),          settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightDB8W59, CabinSynthesisModel.Secondary),        settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawCornerHB8W35, CabinSynthesisModel.Secondary),          settingsW);
            AllSettings.Add((CabinModelEnum.ModelW, CabinDrawNumber.DrawStraightHB8W40, CabinSynthesisModel.Secondary),        settingsW);
        }
        private void CreateSettings8W40()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw8W,
                CabinDrawNumber.Draw8WFlipper81,
                CabinDrawNumber.Draw2Corner8W82,
                CabinDrawNumber.Draw1Corner8W84,
                CabinDrawNumber.Draw2Straight8W85,
                CabinDrawNumber.Draw2CornerStraight8W88,
                CabinDrawNumber.DrawCornerNP6W45,
                CabinDrawNumber.DrawStraightNP6W47,
                CabinDrawNumber.DrawCornerNB6W32,
                CabinDrawNumber.DrawCornerDB8W52,
                CabinDrawNumber.DrawStraightNB6W38,
                CabinDrawNumber.DrawStraightDB8W59,
                CabinDrawNumber.DrawCornerHB8W35,
                CabinDrawNumber.DrawStraightHB8W40,
                CabinDrawNumber.Draw8W40
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm,
            };
            ConstraintsW constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1592,
                MinPossibleHeight = 200,
                MinPossibleLength = 142,
                TolleranceMinusDefaultMinimum = 8,
                MinimumFreeOpening = 600,
                CornerRadiusTopEdge = 200,
                StepHeightTollerance = 9,
                StepLengthTolleranceMin = 5,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                CanHavePerimetricalFrame = true,
                CanHaveStep = true,
            };

            PartSpotDefaults bottomFixer = new(PartSpot.BottomSide1, true, CommonCodes.Supports.FloorStopper,1, CommonCodes.Profiles.FloorProfileLid);
            PartSpotDefaults sideFixer = new(PartSpot.NonWallSide, true, "",1, CommonCodes.Profiles.WallW);
            PartSpotDefaults topFixer = new(PartSpot.TopSide, true, "",1, CommonCodes.Profiles.WallW);
            PartSpotDefaults wallFixer = new(PartSpot.WallSide1, false, CommonCodes.Profiles.WallW,1);
            PartSpotDefaults supportBar = new(PartSpot.SupportBar, true, CommonCodes.SupportBars.SupportBarDefault,1);

            PartSpotDefaults closeStripEmpty = new(PartSpot.CloseStrip, true, "",1);

            DefaultPartsList defaultListW40 = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomFixer },
                    { PartSpot.NonWallSide ,            sideFixer },
                    { PartSpot.TopSide ,                topFixer },
                    { PartSpot.WallSide1 ,              wallFixer },
                    { PartSpot.CloseStrip,              closeStripEmpty },
                    { PartSpot.SupportBar,              supportBar },
                }
            };

            CabinSettings settings8W40 = new()
            {
                Model = CabinModelEnum.Model8W40,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1400,
                NominalLength = 800,
                IsReversible = true,
                MetalFinish = CabinFinishEnum.Polished,
            };

            AllConstraints.Add((CabinModelEnum.Model8W40, CabinDrawNumber.Draw8W40, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.Model8W40, CabinDrawNumber.Draw8W40, CabinSynthesisModel.Primary), defaultListW40);

            AllSettings.Add((CabinModelEnum.Model8W40, CabinDrawNumber.Draw8W40, CabinSynthesisModel.Primary), settings8W40);
        }
        private void CreateSettingsWFlipper()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.Draw8WFlipper81
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsWFlipper constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 350,
                MinPossibleHeight = 200,
                MinPossibleLength = 150,
                TolleranceMinusDefaultMinimum = 0,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                DoorDistanceFromBottom = 15,
                CornerRadiusTopEdge = 0,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                CanHaveStep = false,
            };

            PartSpotDefaults hinge = new(PartSpot.MiddleHinge, false, CommonCodes.Hinges.HingeFlipper,1);

            DefaultPartsList defaultListWFlipper = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.MiddleHinge , hinge },
                }
            };

            CabinSettings settingsWFlipper = new()
            {
                Model = CabinModelEnum.ModelWFlipper,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 2000,
                NominalLength = 320,
                IsReversible = false,
                MetalFinish = CabinFinishEnum.Polished,
            };

            AllConstraints.Add((CabinModelEnum.ModelWFlipper, CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelWFlipper, CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Secondary), defaultListWFlipper);

            AllSettings.Add((CabinModelEnum.ModelWFlipper, CabinDrawNumber.Draw8WFlipper81, CabinSynthesisModel.Secondary), settingsWFlipper);
        }
        private void CreateSettingsHB()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawHB34,
                CabinDrawNumber.DrawCornerHB8W35,
                CabinDrawNumber.Draw2CornerHB37,
                CabinDrawNumber.DrawStraightHB8W40,
                CabinDrawNumber.Draw2StraightHB43
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm
            };
            ConstraintsHB constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1400,
                MinPossibleHeight = 201,
                MinPossibleLength = 700,
                TolleranceMinusDefaultMinimum = 10,
                DoorDistanceFromBottom = 15,
                MaxDoorLength = 800,
                MaxFixedLength = 1385,
                MinDoorLength = 200,
                MinFixedLength = 155,
                HeightBreakPointGlassThickness = 2000,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm10mm,
                CornerRadiusTopEdge = 0,
                StepHeightTollerance = 9,
                LengthCalculation = LengthCalculationOption.ByDoorLength,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                MinPossibleStepHeight = 0,
                PartialLength = 600,
                CanHaveStep = true,
            };

            PartSpotDefaults bottomFixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.FloorStopper,1, CommonCodes.Profiles.FloorProfileLid);
            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual,1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, CommonCodes.Strips.MagnetStripStraight,1,CommonCodes.Strips.BumperStrip);
            PartSpotDefaults wallFixer = new(PartSpot.WallSide1, false, CommonCodes.Profiles.WallW,1, CommonCodes.Supports.SmallWallSupport);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.KnobHandle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults hinge = new(PartSpot.MiddleHinge, false, CommonCodes.Hinges.HingeHB,1);
            PartSpotDefaults supportBar = new(PartSpot.SupportBar, false, CommonCodes.SupportBars.SupportBarDefault,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);

            DefaultPartsList defaultListHB = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1      , bottomFixer },
                    { PartSpot.CloseProfile     , closeProfile},
                    { PartSpot.CloseStrip       , closeStrip },
                    { PartSpot.WallSide1        , wallFixer },
                    { PartSpot.Handle1          , handle },
                    { PartSpot.MiddleHinge      , hinge },
                    { PartSpot.SupportBar       , supportBar },
                }
            };

            PartSpotDefaults closeStripOnlyMagnetStr = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            PartSpotDefaults wallFixerOnlyProfile = new(PartSpot.WallSide1, false, CommonCodes.Profiles.WallW,1);
            DefaultPartsList defaultListHBstraightCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1      , bottomFixer },
                    { PartSpot.CloseProfile     , closeProfileEmpty},
                    { PartSpot.CloseStrip       , closeStripOnlyMagnetStr },
                    { PartSpot.WallSide1        , wallFixerOnlyProfile },
                    { PartSpot.Handle1          , handle },
                    { PartSpot.MiddleHinge      , hinge },
                    { PartSpot.SupportBar       , supportBar },
                }
            };

            PartSpotDefaults closeStripOnlyMagnetCor = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            DefaultPartsList defaultListHBcornerCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1      , bottomFixer },
                    { PartSpot.CloseProfile     , closeProfileEmpty},
                    { PartSpot.CloseStrip       , closeStripOnlyMagnetCor },
                    { PartSpot.WallSide1        , wallFixerOnlyProfile },
                    { PartSpot.Handle1          , handle },
                    { PartSpot.MiddleHinge      , hinge },
                    { PartSpot.SupportBar       , supportBar },
                }
            };

            CabinSettings settingsHB = new()
            {
                Model = CabinModelEnum.ModelHB,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 1000,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawHB34, CabinSynthesisModel.Primary),             constraints);
            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawCornerHB8W35, CabinSynthesisModel.Primary),     constraints);
            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2CornerHB37, CabinSynthesisModel.Primary),      constraints);
            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2CornerHB37, CabinSynthesisModel.Secondary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawStraightHB8W40, CabinSynthesisModel.Primary),   constraints);
            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2StraightHB43, CabinSynthesisModel.Primary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2StraightHB43, CabinSynthesisModel.Secondary),  constraints);

            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawHB34, CabinSynthesisModel.Primary),            defaultListHB);
            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawCornerHB8W35, CabinSynthesisModel.Primary),    defaultListHBstraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2CornerHB37, CabinSynthesisModel.Primary),     defaultListHBcornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2CornerHB37, CabinSynthesisModel.Secondary),   defaultListHBcornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawStraightHB8W40, CabinSynthesisModel.Primary),  defaultListHBstraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2StraightHB43, CabinSynthesisModel.Primary),   defaultListHBstraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2StraightHB43, CabinSynthesisModel.Secondary), defaultListHBstraightCombo);

            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawHB34, CabinSynthesisModel.Primary),            settingsHB);
            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawCornerHB8W35, CabinSynthesisModel.Primary),    settingsHB);
            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2CornerHB37, CabinSynthesisModel.Primary),     settingsHB);
            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2CornerHB37, CabinSynthesisModel.Secondary),   settingsHB);
            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.DrawStraightHB8W40, CabinSynthesisModel.Primary),  settingsHB);
            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2StraightHB43, CabinSynthesisModel.Primary),   settingsHB);
            AllSettings.Add((CabinModelEnum.ModelHB, CabinDrawNumber.Draw2StraightHB43, CabinSynthesisModel.Secondary), settingsHB);
        }
        private void CreateSettingsV4()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawV4,
                CabinDrawNumber.DrawV4VF
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick8mm10mm,
                CabinThicknessEnum.Thick10mm,
            };
            ConstraintsV4 constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 2200,
                MinPossibleHeight = 1000,
                MinPossibleLength = 1200,
                TolleranceMinusDefaultMinimum = 15,
                BarCorrectionLength = 25,
                BreakpointHeight = 2000,
                MaxDoorLengthAfterBreakpoint = 400,
                MaxDoorLengthBeforeBreakpoint = 450,
                HeightBreakPointGlassThickness = 2000,
                LengthBreakPointGlassThickness = 2000,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm10mm,
                MinimumGlassOverlapping = 5,
                DoorDistanceFromBottom = 8,
                MinDoorDistanceFromWallOpened = 20,
                CoverDistance = -20,
                StepHeightTollerance = 5,
                StepLengthTolleranceMin = 5,
                Overlap = 60,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                MinPossibleStepHeight = 0,
                ShouldHaveHandle = true,
                CanHaveStep = true,
            };


            PartSpotDefaults bottomfixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.Inox304DriverBottom,1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.Inox304Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults handle2 = new(PartSpot.Handle2, false, CommonCodes.Handles.Inox304Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults wallFixer1 = new(PartSpot.WallSide1, false, CommonCodes.Supports.SmallWallSupport,1, CommonCodes.Profiles.WallW);
            PartSpotDefaults wallFixer2 = new(PartSpot.WallSide2, false, CommonCodes.Supports.SmallWallSupport,1, CommonCodes.Profiles.WallW);
            PartSpotDefaults horizontalBar = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalBarInox304,1);

            DefaultPartsList defaultListV4 = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.Handle2,                 handle2 },
                    { PartSpot.WallSide1,               wallFixer1},
                    { PartSpot.WallSide2,               wallFixer2},
                    { PartSpot.HorizontalGuideTop,      horizontalBar},
                }
            };
            PartSpotDefaults wallFixer2OnlySupports = new(PartSpot.WallSide2, false, CommonCodes.Supports.SmallWallSupport,1);
            DefaultPartsList defaultListV4Fixed = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.Handle2,                 handle2 },
                    { PartSpot.WallSide1,               wallFixer1},
                    { PartSpot.WallSide2,               wallFixer2OnlySupports},
                    { PartSpot.HorizontalGuideTop,      horizontalBar},
                }
            };

            CabinSettings settingsV4 = new()
            {
                Model = CabinModelEnum.ModelV4,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 1600,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };
            AllConstraints.Add((CabinModelEnum.ModelV4, CabinDrawNumber.DrawV4, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelV4, CabinDrawNumber.DrawV4VF, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelV4, CabinDrawNumber.DrawV4, CabinSynthesisModel.Primary), defaultListV4);
            DefaultLists.Add((CabinModelEnum.ModelV4, CabinDrawNumber.DrawV4VF, CabinSynthesisModel.Primary), defaultListV4Fixed);

            AllSettings.Add((CabinModelEnum.ModelV4, CabinDrawNumber.DrawV4, CabinSynthesisModel.Primary), settingsV4);
            AllSettings.Add((CabinModelEnum.ModelV4, CabinDrawNumber.DrawV4VF, CabinSynthesisModel.Primary), settingsV4);
        }
        private void CreateSettingsVA()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawVA
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick8mm10mm
            };

            ConstraintsVA constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 1200,
                MinPossibleHeight = 500,
                MinPossibleLength = 300,
                TolleranceMinusDefaultMinimum = 15,
                BarCorrectionLength = 25,
                MinimumGlassOverlapping = 5,
                MaxDoorLength = 390,
                DoorDistanceFromBottom = 8,
                MinDoorDistanceFromWallOpened = 20,
                CoverDistance = -20,
                StepHeightTollerance = 5,
                StepLengthTolleranceMin = 5,
                Overlap = 30,
                FinalHeightCorrection = 0,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm,
                MinPossibleStepHeight = 0,
                ShouldHaveHandle = true,
                CanHaveStep = true,
            };
            PartSpotDefaults bottomfixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.Inox304DriverBottom,1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.Inox304Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults wallFixer1 = new(PartSpot.WallSide1, false, CommonCodes.Supports.SmallWallSupport,1, CommonCodes.Profiles.WallW);
            PartSpotDefaults angle = new(PartSpot.Angle, false, CommonCodes.Angles.AngleVA,1);
            PartSpotDefaults horizontalBar = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalBarInox304,1);

            DefaultPartsList defaultListVA = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.WallSide1,               wallFixer1},
                    { PartSpot.Angle,                   angle},
                    { PartSpot.HorizontalGuideTop,      horizontalBar},
                }
            };

            CabinSettings settingsVA = new()
            {
                Model = CabinModelEnum.ModelVA,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelVA, CabinDrawNumber.DrawVA, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelVA, CabinDrawNumber.DrawVA, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelVA, CabinDrawNumber.DrawVA, CabinSynthesisModel.Primary), defaultListVA);
            DefaultLists.Add((CabinModelEnum.ModelVA, CabinDrawNumber.DrawVA, CabinSynthesisModel.Secondary), defaultListVA);

            AllSettings.Add((CabinModelEnum.ModelVA, CabinDrawNumber.DrawVA, CabinSynthesisModel.Primary), settingsVA);
            AllSettings.Add((CabinModelEnum.ModelVA, CabinDrawNumber.DrawVA, CabinSynthesisModel.Secondary), settingsVA);
        }
        private void CreateSettingsVF()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawVSVF,
                CabinDrawNumber.DrawV4VF
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick10mm
            };
            ConstraintsVF constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1500,
                MinPossibleHeight = 201,
                MinPossibleLength = 150,
                LengthBreakPointGlassThickness = 1200,
                HeightBreakPointGlassThickness = 2000,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm10mm,
                TolleranceMinusDefaultMinimum = 15,
                StepHeightTollerance = 5,
                StepLengthTolleranceMin = 5,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0 ,
                FinalHeightCorrection = 0,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                CanHaveStep = true,
            };
            PartSpotDefaults bottomfixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.FloorStopper,1, CommonCodes.Profiles.FloorProfileLid);
            PartSpotDefaults wallFixer = new(PartSpot.WallSide1, false, CommonCodes.Supports.SmallWallSupport,1, CommonCodes.Profiles.WallW);

            DefaultPartsList defaultListVSVF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.WallSide1,               wallFixer},
                }
            };

            PartSpotDefaults sideFixer = new(PartSpot.NonWallSide, false, CommonCodes.Supports.SmallWallSupport,1);
            DefaultPartsList defaultListV4VF = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.NonWallSide ,            sideFixer},
                    { PartSpot.WallSide1,               wallFixer},
                }
            };

            CabinSettings settingsVF = new()
            {
                Model = CabinModelEnum.ModelVF,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelVF, CabinDrawNumber.DrawVSVF, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelVF, CabinDrawNumber.DrawV4VF, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelVF, CabinDrawNumber.DrawVSVF, CabinSynthesisModel.Secondary), defaultListVSVF);
            DefaultLists.Add((CabinModelEnum.ModelVF, CabinDrawNumber.DrawV4VF, CabinSynthesisModel.Secondary), defaultListV4VF);

            AllSettings.Add((CabinModelEnum.ModelVF, CabinDrawNumber.DrawVSVF, CabinSynthesisModel.Secondary), settingsVF);
            AllSettings.Add((CabinModelEnum.ModelVF, CabinDrawNumber.DrawV4VF, CabinSynthesisModel.Secondary), settingsVF);
        }
        private void CreateSettingsVS()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawVS,
                CabinDrawNumber.DrawVSVF
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm,
                CabinThicknessEnum.Thick8mm10mm,
                CabinThicknessEnum.Thick10mm,
            };
            ConstraintsVS constraints = new()
            {
                MaxPossibleHeight = 2200,
                MaxPossibleLength = 1800,
                MinPossibleHeight = 1000,
                MinPossibleLength = 900,
                TolleranceMinusDefaultMinimum = 15,
                BarCorrectionLength = 25,
                BreakpointHeight = 2000,
                MaxDoorLengthAfterBreakpoint = 700,
                MaxDoorLengthBeforeBreakpoint = 760,
                HeightBreakPointGlassThickness = 2000,
                LengthBreakPointGlassThickness = 1600,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm10mm,
                MinimumGlassOverlapping = 5,
                DoorDistanceFromBottom = 8,
                MinDoorDistanceFromWallOpened = 20,
                CoverDistance = -20,
                StepHeightTollerance = 5,
                StepLengthTolleranceMin = 5,
                Overlap = 70,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0 , 
                FinalHeightCorrection = 0,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                ShouldHaveHandle = true,
                CanHaveStep = true,
            };


            PartSpotDefaults wallFixer1 = new(PartSpot.WallSide1, false, CommonCodes.Supports.SmallWallSupport,1, CommonCodes.Profiles.WallW);
            PartSpotDefaults bottomfixer = new(PartSpot.BottomSide1, false, CommonCodes.Supports.Inox304DriverBottom,1);
            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual,1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1, CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.Inox304Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults horizontalBar = new(PartSpot.HorizontalGuideTop, false, CommonCodes.Profiles.HorizontalBarInox304,1);

            DefaultPartsList defaultListVS = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.WallSide1,               wallFixer1},
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.CloseProfile ,           closeProfile },
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalBar},
                }
            };

            PartSpotDefaults closeStripBumper = new(PartSpot.CloseStrip, false, CommonCodes.Strips.BumperStrip,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);
            DefaultPartsList defaultListVSSidePanel = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.WallSide1,               wallFixer1},
                    { PartSpot.BottomSide1 ,            bottomfixer },
                    { PartSpot.CloseProfile ,           closeProfileEmpty },
                    { PartSpot.CloseStrip ,             closeStripBumper },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.HorizontalGuideTop,      horizontalBar},
                }
            };

            CabinSettings settingsVS = new()
            {
                Model = CabinModelEnum.ModelVS,
                Thicknesses = CabinThicknessEnum.Thick8mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 1200,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelVS, CabinDrawNumber.DrawVS, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelVS, CabinDrawNumber.DrawVSVF, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelVS, CabinDrawNumber.DrawVS, CabinSynthesisModel.Primary), defaultListVS);
            DefaultLists.Add((CabinModelEnum.ModelVS, CabinDrawNumber.DrawVSVF, CabinSynthesisModel.Primary), defaultListVSSidePanel);

            AllSettings.Add((CabinModelEnum.ModelVS, CabinDrawNumber.DrawVS, CabinSynthesisModel.Primary), settingsVS);
            AllSettings.Add((CabinModelEnum.ModelVS, CabinDrawNumber.DrawVSVF, CabinSynthesisModel.Primary), settingsVS);
        }
        private void CreateSettingsNB()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawNB31,
                CabinDrawNumber.DrawCornerNB6W32,
                CabinDrawNumber.Draw2CornerNB33,
                CabinDrawNumber.DrawStraightNB6W38,
                CabinDrawNumber.Draw2StraightNB41,
                CabinDrawNumber.DrawNV
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNB constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 935,
                MinPossibleHeight = 201,
                MinPossibleLength = 285,
                TolleranceMinusDefaultMinimum = 15,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                DoorHeightAdjustment = 20,
                CornerRadiusTopEdge = 0,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0 , 
                FinalHeightCorrection = 0 ,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                CanHaveStep = false,
                ShouldHaveHandle = true,
            };
            
            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, CommonCodes.Strips.MagnetStripStraight,1, CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, true, CommonCodes.Handles.KnobHandle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults hinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileNB,1);

            DefaultPartsList defaultListNB = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfile },
                    { PartSpot.CloseStrip,                closeStrip },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            PartSpotDefaults closeStripCorner = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            DefaultPartsList defaultListNBCornerCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripCorner },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            PartSpotDefaults closeStripStraight = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            DefaultPartsList defaultListNBStraightCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripStraight },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            CabinSettings settingsNB = new()
            {
                Model = CabinModelEnum.ModelNB,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 700,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawNB31, CabinSynthesisModel.Primary),             constraints);
            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawCornerNB6W32, CabinSynthesisModel.Primary),     constraints);
            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2CornerNB33, CabinSynthesisModel.Primary),      constraints);
            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2CornerNB33, CabinSynthesisModel.Secondary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawStraightNB6W38, CabinSynthesisModel.Primary),   constraints);
            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2StraightNB41, CabinSynthesisModel.Primary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2StraightNB41, CabinSynthesisModel.Secondary),  constraints);

            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawNB31, CabinSynthesisModel.Primary),             defaultListNB);
            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawCornerNB6W32, CabinSynthesisModel.Primary),     defaultListNBCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2CornerNB33, CabinSynthesisModel.Primary),      defaultListNBCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2CornerNB33, CabinSynthesisModel.Secondary),    defaultListNBCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawStraightNB6W38, CabinSynthesisModel.Primary),   defaultListNBStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2StraightNB41, CabinSynthesisModel.Primary),    defaultListNBStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2StraightNB41, CabinSynthesisModel.Secondary),  defaultListNBStraightCombo);

            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawNB31, CabinSynthesisModel.Primary),             settingsNB);
            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawCornerNB6W32, CabinSynthesisModel.Primary),     settingsNB);
            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2CornerNB33, CabinSynthesisModel.Primary),      settingsNB);
            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2CornerNB33, CabinSynthesisModel.Secondary),    settingsNB);
            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.DrawStraightNB6W38, CabinSynthesisModel.Primary),   settingsNB);
            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2StraightNB41, CabinSynthesisModel.Primary),    settingsNB);
            AllSettings.Add((CabinModelEnum.ModelNB, CabinDrawNumber.Draw2StraightNB41, CabinSynthesisModel.Secondary),  settingsNB);
        }
        private void CreateSettingsQB()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawQB31,
                CabinDrawNumber.DrawCornerQB6W32,
                CabinDrawNumber.Draw2CornerQB33,
                CabinDrawNumber.DrawStraightQB6W38,
                CabinDrawNumber.Draw2StraightQB41
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNB constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 935,
                MinPossibleHeight = 201,
                MinPossibleLength = 285,
                TolleranceMinusDefaultMinimum = 15,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                DoorHeightAdjustment = 0,
                CornerRadiusTopEdge = 0,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                CanHaveStep = false,
                ShouldHaveHandle = true,
            };

            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual, 1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "", 1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, CommonCodes.Strips.MagnetStripStraight, 1, CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, true, CommonCodes.Handles.KnobHandle, 1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults hinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileQB, 1);

            DefaultPartsList defaultListQB = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfile },
                    { PartSpot.CloseStrip,                closeStrip },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            PartSpotDefaults closeStripCorner = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees, 1);
            DefaultPartsList defaultListQBCornerCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripCorner },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            PartSpotDefaults closeStripStraight = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight, 1);
            DefaultPartsList defaultListQBStraightCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripStraight },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            CabinSettings settingsQB = new()
            {
                Model = CabinModelEnum.ModelQB,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 700,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawQB31, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawCornerQB6W32, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2CornerQB33, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2CornerQB33, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawStraightQB6W38, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2StraightQB41, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2StraightQB41, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawQB31, CabinSynthesisModel.Primary), defaultListQB);
            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawCornerQB6W32, CabinSynthesisModel.Primary), defaultListQBCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2CornerQB33, CabinSynthesisModel.Primary), defaultListQBCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2CornerQB33, CabinSynthesisModel.Secondary), defaultListQBCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawStraightQB6W38, CabinSynthesisModel.Primary), defaultListQBStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2StraightQB41, CabinSynthesisModel.Primary), defaultListQBStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2StraightQB41, CabinSynthesisModel.Secondary), defaultListQBStraightCombo);

            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawQB31, CabinSynthesisModel.Primary), settingsQB);
            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawCornerQB6W32, CabinSynthesisModel.Primary), settingsQB);
            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2CornerQB33, CabinSynthesisModel.Primary), settingsQB);
            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2CornerQB33, CabinSynthesisModel.Secondary), settingsQB);
            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.DrawStraightQB6W38, CabinSynthesisModel.Primary), settingsQB);
            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2StraightQB41, CabinSynthesisModel.Primary), settingsQB);
            AllSettings.Add((CabinModelEnum.ModelQB, CabinDrawNumber.Draw2StraightQB41, CabinSynthesisModel.Secondary), settingsQB);
        }
        private void CreateSettingsNV()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawNB31,
                CabinDrawNumber.DrawCornerNB6W32,
                CabinDrawNumber.Draw2CornerNB33,
                CabinDrawNumber.DrawStraightNB6W38,
                CabinDrawNumber.Draw2StraightNB41,
                CabinDrawNumber.DrawNV
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNB constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 935,
                MinPossibleHeight = 201,
                MinPossibleLength = 285,
                TolleranceMinusDefaultMinimum = 15,
                DoorHeightAdjustment = 20,
                CornerRadiusTopEdge = 200,
                FinalHeightCorrection = 0,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ValidDrawNumbers = validDraws,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                MinPossibleStepHeight = 0,
                ShouldHaveHandle= false,
                CanHaveStep = false,
            };

            PartSpotDefaults handle = new(PartSpot.Handle1, true, "",1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults hinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileNB,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);
            PartSpotDefaults closeStripEmpty = new(PartSpot.CloseStrip, true, "",1);

            DefaultPartsList defaultListNV = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripEmpty },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.WallHinge,                 hinge},
                }
            };

            CabinSettings settingsNV = new()
            {
                Model = CabinModelEnum.ModelNV,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1400,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelNV, CabinDrawNumber.DrawNV, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelNV, CabinDrawNumber.DrawNV, CabinSynthesisModel.Primary), defaultListNV);

            AllSettings.Add((CabinModelEnum.ModelNV, CabinDrawNumber.DrawNV, CabinSynthesisModel.Primary), settingsNV);
        }
        private void CreateSettingsNP()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawNP44,
                CabinDrawNumber.DrawCornerNP6W45,
                CabinDrawNumber.Draw2CornerNP46,
                CabinDrawNumber.DrawStraightNP6W47,
                CabinDrawNumber.Draw2StraightNP48,
                CabinDrawNumber.DrawMV2,
                CabinDrawNumber.DrawNV2
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNP constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 980,
                MinPossibleHeight = 201,
                MinPossibleLength = 535,
                TolleranceMinusDefaultMinimum = 15,
                DoorsLengthDifference = 15,
                FoldedLength = 30,
                CornerRadiusTopEdge = 0,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                CanHaveStep = false,
                ShouldHaveHandle = true,
            };

            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual,1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, CommonCodes.Strips.MagnetStripStraight,1, CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.KnobHandle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults middleHinge = new(PartSpot.MiddleHinge, false, CommonCodes.Hinges.HingeMiddleNP,1, CommonCodes.Profiles.MiddleHingeProfileNB);
            PartSpotDefaults wallHinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileNB,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);

            DefaultPartsList defaultListNP = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfile },
                    { PartSpot.CloseStrip,                closeStrip },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            PartSpotDefaults closeStripStraight = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight,1);
            DefaultPartsList defaultListNPStraightCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripStraight },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            PartSpotDefaults closeStripCorner = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees,1);
            DefaultPartsList defaultListNPCornerCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripCorner },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            CabinSettings settingsNP = new()
            {
                Model = CabinModelEnum.ModelNP,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = false,
            };

            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawNP44, CabinSynthesisModel.Primary),             constraints);
            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawCornerNP6W45, CabinSynthesisModel.Primary),     constraints);
            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2CornerNP46, CabinSynthesisModel.Primary),      constraints);
            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2CornerNP46, CabinSynthesisModel.Secondary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawStraightNP6W47, CabinSynthesisModel.Primary),   constraints);
            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2StraightNP48, CabinSynthesisModel.Primary),    constraints);
            AllConstraints.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2StraightNP48, CabinSynthesisModel.Secondary),  constraints);

            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawNP44, CabinSynthesisModel.Primary),             defaultListNP);
            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawCornerNP6W45, CabinSynthesisModel.Primary),     defaultListNPCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2CornerNP46, CabinSynthesisModel.Primary),      defaultListNPCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2CornerNP46, CabinSynthesisModel.Secondary),    defaultListNPCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawStraightNP6W47, CabinSynthesisModel.Primary),   defaultListNPStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2StraightNP48, CabinSynthesisModel.Primary),    defaultListNPStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2StraightNP48, CabinSynthesisModel.Secondary),  defaultListNPStraightCombo);

            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawNP44, CabinSynthesisModel.Primary),             settingsNP);
            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawCornerNP6W45, CabinSynthesisModel.Primary),     settingsNP);
            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2CornerNP46, CabinSynthesisModel.Primary),      settingsNP);
            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2CornerNP46, CabinSynthesisModel.Secondary),    settingsNP);
            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.DrawStraightNP6W47, CabinSynthesisModel.Primary),   settingsNP);
            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2StraightNP48, CabinSynthesisModel.Primary),    settingsNP);
            AllSettings.Add((CabinModelEnum.ModelNP, CabinDrawNumber.Draw2StraightNP48, CabinSynthesisModel.Secondary),  settingsNP);
        }
        private void CreateSettingsQP()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawQP44,
                CabinDrawNumber.DrawCornerQP6W45,
                CabinDrawNumber.Draw2CornerQP46,
                CabinDrawNumber.DrawStraightQP6W47,
                CabinDrawNumber.Draw2StraightQP48
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNP constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 980,
                MinPossibleHeight = 201,
                MinPossibleLength = 535,
                TolleranceMinusDefaultMinimum = 15,
                DoorsLengthDifference = 15,
                FoldedLength = 30,
                CornerRadiusTopEdge = 0,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                FinalHeightCorrection = 0,
                ValidThicknesses = validThicknesses,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                CanHaveStep = false,
                ShouldHaveHandle = true,
            };

            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, CommonCodes.Profiles.MagnetProfileUsual, 1);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, CommonCodes.Strips.MagnetStripStraight, 1, CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, false, CommonCodes.Handles.KnobHandle, 1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults middleHinge = new(PartSpot.MiddleHinge, false, CommonCodes.Hinges.HingeMiddleNP, 1, CommonCodes.Profiles.MiddleHingeProfileNB);
            PartSpotDefaults wallHinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileQB, 1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "", 1);

            DefaultPartsList defaultListQP = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfile },
                    { PartSpot.CloseStrip,                closeStrip },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            PartSpotDefaults closeStripStraight = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStripStraight, 1);
            DefaultPartsList defaultListQPStraightCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripStraight },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            PartSpotDefaults closeStripCorner = new(PartSpot.CloseStrip, false, CommonCodes.Strips.MagnetStrip45Degrees, 1);
            DefaultPartsList defaultListQPCornerCombo = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripCorner },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            CabinSettings settingsQP = new()
            {
                Model = CabinModelEnum.ModelQP,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1900,
                NominalLength = 800,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = false,
            };

            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawQP44, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawCornerQP6W45, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2CornerQP46, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2CornerQP46, CabinSynthesisModel.Secondary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawStraightQP6W47, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2StraightQP48, CabinSynthesisModel.Primary), constraints);
            AllConstraints.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2StraightQP48, CabinSynthesisModel.Secondary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawQP44, CabinSynthesisModel.Primary), defaultListQP);
            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawCornerQP6W45, CabinSynthesisModel.Primary), defaultListQPCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2CornerQP46, CabinSynthesisModel.Primary), defaultListQPCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2CornerQP46, CabinSynthesisModel.Secondary), defaultListQPCornerCombo);
            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawStraightQP6W47, CabinSynthesisModel.Primary), defaultListQPStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2StraightQP48, CabinSynthesisModel.Primary), defaultListQPStraightCombo);
            DefaultLists.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2StraightQP48, CabinSynthesisModel.Secondary), defaultListQPStraightCombo);

            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawQP44, CabinSynthesisModel.Primary), settingsQP);
            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawCornerQP6W45, CabinSynthesisModel.Primary), settingsQP);
            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2CornerQP46, CabinSynthesisModel.Primary), settingsQP);
            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2CornerQP46, CabinSynthesisModel.Secondary), settingsQP);
            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.DrawStraightQP6W47, CabinSynthesisModel.Primary), settingsQP);
            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2StraightQP48, CabinSynthesisModel.Primary), settingsQP);
            AllSettings.Add((CabinModelEnum.ModelQP, CabinDrawNumber.Draw2StraightQP48, CabinSynthesisModel.Secondary), settingsQP);
        }
        private void CreateSettingsNV2()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawNP44,
                CabinDrawNumber.DrawCornerNP6W45,
                CabinDrawNumber.Draw2CornerNP46,
                CabinDrawNumber.DrawStraightNP6W47,
                CabinDrawNumber.Draw2StraightNP48,
                CabinDrawNumber.DrawMV2,
                CabinDrawNumber.DrawNV2
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNP constraints = new()
            {
                MaxPossibleHeight = 1600,
                MaxPossibleLength = 1200,
                MinPossibleHeight = 201,
                MinPossibleLength = 635,
                TolleranceMinusDefaultMinimum = 15,
                DoorsLengthDifference = 145,
                FoldedLength = 30,
                CornerRadiusTopEdge = 200,
                FinalHeightCorrection = 0,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                ValidThicknesses = validThicknesses,
                CanHaveStep = false,
                ShouldHaveHandle = false,
            };

            PartSpotDefaults handle = new(PartSpot.Handle1, true, CommonCodes.Handles.KnobHandle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults middleHinge = new(PartSpot.MiddleHinge, false, CommonCodes.Profiles.MiddleHingeProfileNB,1);
            PartSpotDefaults wallHinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileNB,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);
            PartSpotDefaults closeStripEmpty = new(PartSpot.CloseStrip, true, "",1);

            DefaultPartsList defaultListNV2 = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripEmpty },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };

            CabinSettings settingsNV2 = new()
            {
                Model = CabinModelEnum.ModelNV2,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1400,
                NominalLength = 1200,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = false,
            };

            AllConstraints.Add((CabinModelEnum.ModelNV2, CabinDrawNumber.DrawNV2, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelNV2, CabinDrawNumber.DrawNV2, CabinSynthesisModel.Primary), defaultListNV2);

            AllSettings.Add((CabinModelEnum.ModelNV2, CabinDrawNumber.DrawNV2, CabinSynthesisModel.Primary), settingsNV2);
        }
        private void CreateSettingsMV2()
        {

            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawNP44,
                CabinDrawNumber.DrawCornerNP6W45,
                CabinDrawNumber.Draw2CornerNP46,
                CabinDrawNumber.DrawStraightNP6W47,
                CabinDrawNumber.Draw2StraightNP48,
                CabinDrawNumber.DrawMV2,
                CabinDrawNumber.DrawNV2
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.BrushedGold,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick6mm
            };
            ConstraintsNP constraints = new()
            {
                MaxPossibleHeight = 1600,
                MaxPossibleLength = 1200,
                MinPossibleHeight = 201,
                MinPossibleLength = 635,
                TolleranceMinusDefaultMinimum = 15,
                DoorsLengthDifference = 145,
                FoldedLength = 30,
                CornerRadiusTopEdge = 200,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0 , 
                FinalHeightCorrection = 0,
                BreakPointMinThickness = CabinThicknessEnum.Thick6mm,
                ValidThicknesses = validThicknesses,
                ValidGlassFinishes = validGlassFinishes,
                ValidMetalFinishes = validMetalFinishes,
                CanHaveStep = false,
                ShouldHaveHandle = false,
            };

            PartSpotDefaults handle = new(PartSpot.Handle1, true, CommonCodes.Handles.KnobHandle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults middleHinge = new(PartSpot.MiddleHinge, false, CommonCodes.Hinges.HingeMiddleNP,1);
            PartSpotDefaults wallHinge = new(PartSpot.WallHinge, false, CommonCodes.Profiles.HingeProfileNB,1);
            PartSpotDefaults closeProfileEmpty = new(PartSpot.CloseProfile, true, "",1);
            PartSpotDefaults closeStripEmpty = new(PartSpot.CloseStrip, true, "",1);

            DefaultPartsList defaultListMV2 = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,             closeProfileEmpty },
                    { PartSpot.CloseStrip,                closeStripEmpty },
                    { PartSpot.Handle1,                   handle },
                    { PartSpot.MiddleHinge,               middleHinge },
                    { PartSpot.WallHinge,                 wallHinge},
                }
            };
            
            CabinSettings settingsMV2 = new()
            {
                Model = CabinModelEnum.ModelMV2,
                Thicknesses = CabinThicknessEnum.Thick6mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 1400,
                NominalLength = 1200,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = false,
            };

            AllConstraints.Add((CabinModelEnum.ModelMV2, CabinDrawNumber.DrawMV2, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelMV2, CabinDrawNumber.DrawMV2, CabinSynthesisModel.Primary), defaultListMV2);

            AllSettings.Add((CabinModelEnum.ModelMV2, CabinDrawNumber.DrawMV2, CabinSynthesisModel.Primary), settingsMV2);
        }
        private void CreateSettingsWS()
        {
            List<CabinDrawNumber> validDraws = new()
            {
                CabinDrawNumber.None,
                CabinDrawNumber.DrawWS
            };
            List<GlassFinishEnum> validGlassFinishes = new()
            {
                GlassFinishEnum.Transparent,
                GlassFinishEnum.Satin,
                GlassFinishEnum.Fume,
            };
            List<CabinFinishEnum> validMetalFinishes = new()
            {
                CabinFinishEnum.Polished,
                CabinFinishEnum.Brushed,
                CabinFinishEnum.BlackMat,
                CabinFinishEnum.WhiteMat,
                CabinFinishEnum.Bronze,
                CabinFinishEnum.Gold,
                CabinFinishEnum.BrushedGold,
                CabinFinishEnum.Copper,
            };
            List<CabinThicknessEnum> validThicknesses = new()
            {
                CabinThicknessEnum.Thick8mm10mm,
            };
            ConstraintsWS constraints = new()
            {
                MaxPossibleHeight = 2000,
                MaxPossibleLength = 1300,
                MinPossibleHeight = 500,
                MinPossibleLength = 500,
                TolleranceMinusDefaultMinimum = 10,
                MinimumGlassOverlapping = 1,
                MaxDoorLength = 750,
                DoorDistanceFromBottom = 8,
                CoverDistance = -15,
                Overlap = 100,
                BreakPointMinThickness = CabinThicknessEnum.Thick8mm10mm,
                ValidDrawNumbers = validDraws,
                MinPossibleStepHeight = 0 ,
                FinalHeightCorrection = 0,
                ValidMetalFinishes = validMetalFinishes,
                ValidGlassFinishes = validGlassFinishes,
                ValidThicknesses = validThicknesses,
                ShouldHaveHandle = true,
                CanHaveStep = false,
            };

            PartSpotDefaults closeProfile = new(PartSpot.CloseProfile, true, "",1, CommonCodes.Profiles.MagnetProfileUsual);
            PartSpotDefaults closeStrip = new(PartSpot.CloseStrip, true, "",1,CommonCodes.Strips.BumperStrip);
            PartSpotDefaults handle = new(PartSpot.Handle1, true, CommonCodes.Handles.Inox304Handle,1, AllHandles.Values.Select(h => h.Code).ToList());
            PartSpotDefaults supportBar = new(PartSpot.SupportBar, false, CommonCodes.SupportBars.SupportBarSmart,1);
            PartSpotDefaults wallFixer1 = new(PartSpot.WallSide1, false, CommonCodes.Profiles.WallSmartWS,1);

            DefaultPartsList defaultListWS = new()
            {
                GenericParts = new(),
                SpotDefaults = new()
                {
                    { PartSpot.CloseProfile ,           closeProfile },
                    { PartSpot.CloseStrip ,             closeStrip },
                    { PartSpot.Handle1,                 handle },
                    { PartSpot.SupportBar,              supportBar },
                    { PartSpot.WallSide1,               wallFixer1},
                }
            };

            CabinSettings settingsWS = new()
            {
                Model = CabinModelEnum.ModelWS,
                Thicknesses = CabinThicknessEnum.Thick8mm10mm,
                GlassFinish = GlassFinishEnum.Transparent,
                Height = 2000,
                NominalLength = 1200,
                MetalFinish = CabinFinishEnum.Polished,
                IsReversible = true,
            };

            AllConstraints.Add((CabinModelEnum.ModelWS, CabinDrawNumber.DrawWS, CabinSynthesisModel.Primary), constraints);

            DefaultLists.Add((CabinModelEnum.ModelWS, CabinDrawNumber.DrawWS, CabinSynthesisModel.Primary), defaultListWS);

            AllSettings.Add((CabinModelEnum.ModelWS, CabinDrawNumber.DrawWS, CabinSynthesisModel.Primary), settingsWS);
        }

        #endregion


        public async Task<OperationResult> InitilizeRepo(string languageDescriptor)
        {
            await Task.Delay(1);
            return OperationResult.Success();
        }

        public CabinPart GetPartOriginal(string partCode)
        {
            if (AllParts.TryGetValue(partCode, out CabinPart? part))
            {
                return part;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Requested {partCode} was not found in the Repository");
            }
        }

        public List<CabinPart> GetAllParts(CabinIdentifier identifier)
        {
            return GetAllParts();
            
            // **Not Implemented Here**

            //return GetAllParts().Select(p =>
            //{
            //    AddAdditionalStructureSpecificParts(p, identifier);
            //    return p;
            //}).ToList();
        }

        public CabinPart GetPart(string partCode, CabinIdentifier identifier)
        {
            return GetPart(partCode);
            
            // **Not Implemented Here**

            //var part = GetPart(partCode);
            //AddAdditionalStructureSpecificParts(part, identifier);
            //return part;
        }

        public Profile GetProfile(string profileCode, CabinIdentifier identifier)
        {
            return GetProfile(profileCode);

            // **Not Implemented Here**

            //var profile = GetProfile(profileCode);
            //AddAdditionalStructureSpecificParts(profile, identifier);
            //return profile;
        }

        public CabinHinge GetHinge(string hingeCode, CabinIdentifier identifier)
        {
            return GetHinge(hingeCode);

            // **Not Implemented Here**

            //var hinge = GetHinge(hingeCode);
            //AddAdditionalStructureSpecificParts(hinge, identifier);
            //return hinge;
        }

        public GlassStrip GetStrip(string stripCode, CabinIdentifier identifier)
        {
            return GetStrip(stripCode);
            // **Not Implemented Here**

            //var strip = GetStrip(stripCode);
            //AddAdditionalStructureSpecificParts(strip, identifier);
            //return strip;
        }

        public CabinAngle GetAngle(string angleCode, CabinIdentifier identifier)
        {
            return GetAngle(angleCode);
            // **Not Implemented Here**

            //var angle = GetAngle(angleCode);
            //AddAdditionalStructureSpecificParts(angle, identifier);
            //return angle;
        }

        public SupportBar GetSupportBar(string supportBarCode, CabinIdentifier identifier)
        {
            return GetSupportBar(supportBarCode);
            // **Not Implemented Here**

            //var supportBar = GetSupportBar(supportBarCode);
            //AddAdditionalStructureSpecificParts(supportBar, identifier);
            //return supportBar;
        }

        public CabinSupport GetSupport(string supportCode, CabinIdentifier identifier)
        {
            return GetSupport(supportCode);
            
            // **Not Implemented Here**

            //var support = GetSupport(supportCode);
            //AddAdditionalStructureSpecificParts(support, identifier);
            //return support;
        }

        public CabinHandle GetHandle(string handleCode, CabinIdentifier identifier)
        {
            return GetHandle(handleCode);

            // **Not Implemented Here**

            //var handle = GetHandle(handleCode);
            //AddAdditionalStructureSpecificParts(handle, identifier);
            //return handle;
        }

        public IEnumerable<GlassStrip> GetStrips(CabinStripType stripType, CabinIdentifier identifier)
        {
            return AllStrips.Values.Where(s => s.StripType == stripType).Select(s => s.GetDeepClone());

            // **Not Implemented Here**

            //return AllStrips.Values.Where(s => s.StripType == stripType).Select(s =>
            //{
            //    var strip = s.GetDeepClone();
            //    AddAdditionalStructureSpecificParts(strip, identifier);
            //    return strip;
            //});
        }

        public double GetSpotDefaultQuantity(PartSpot spot, CabinIdentifier identifier)
        {
            if (DefaultLists.TryGetValue((identifier.Model, identifier.DrawNumber, identifier.SynthesisModel), out DefaultPartsList? defaults) && defaults is not null)
            {
                return defaults.GetSpotDefaultQuantity(spot);
            }
            return 0;
        }

        /// <summary>
        /// ***NOT USED HERE NOT NEEDED*** Adds any Structure Additional Parts to the List of ExtraParts of a CabinPart
        /// </summary>
        /// <param name="part">The Part of which the extra parts will be filled</param>
        /// <param name="identifier">The Identifier of the structure</param>
        private void AddAdditionalStructureSpecificParts(CabinPart part, CabinIdentifier identifier)
        {
            //Search if the Provided Code Exists in the Additional PartsList of Codes
            if (AdditionalPartsLists.TryGetValue(part.Code, out Dictionary<CabinIdentifier, List<CabinPart>>? partsDict) && partsDict is not null)
            {
                //Search for the List of Additional Parts for this Particular Structure
                if (partsDict.TryGetValue(identifier, out List<CabinPart>? addParts) && addParts is not null)
                {
                    //If it exists Add the Additional Parts to the list
                    foreach (var addPart in addParts)
                    {
                        part.AdditionalParts.Add(addPart.GetDeepClone());
                    }
                }
            }
        }
    }
}


using BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Shared.Helpers
{
    /// <summary>
    /// Contains Help Dictionaries for Image Paths and Language Container Description Keys
    /// </summary>
    public static class StaticInfoCabins
    {
        #region 1.Cabin Models Categorization Lists

        /// <summary>
        /// Bronze6000 Models List
        /// </summary>
        public static readonly List<CabinModelEnum> ModelsB6000 = new()
        {
            CabinModelEnum.Model9S,
            CabinModelEnum.Model94,
            CabinModelEnum.Model9A,
            CabinModelEnum.Model9C,
            CabinModelEnum.Model9B
        };

        /// <summary>
        /// Inox304 Models List
        /// </summary>
        public static readonly List<CabinModelEnum> ModelsInox304 = new()
        {
            CabinModelEnum.ModelVS,
            CabinModelEnum.ModelV4,
            CabinModelEnum.ModelVA,
        };

        /// <summary>
        /// Hotel8000 Models List
        /// </summary>
        public static readonly List<CabinModelEnum> ModelsHotel8000 = new()
        {
            CabinModelEnum.ModelHB,
            CabinModelEnum.ModelDB
        };

        /// <summary>
        /// Niagara6000 Models List
        /// </summary>
        public static readonly List<CabinModelEnum> ModelsNiagara6000 = new()
        {
            CabinModelEnum.ModelNP,
            CabinModelEnum.ModelQP,
            CabinModelEnum.ModelNB,
            CabinModelEnum.ModelQB,
            CabinModelEnum.ModelMV2,
            CabinModelEnum.ModelNV2,
            CabinModelEnum.ModelNV
        };

        /// <summary>
        /// Free Models List
        /// </summary>
        public static readonly List<CabinModelEnum> ModelsFree = new()
        {
            CabinModelEnum.ModelW,
            CabinModelEnum.ModelE,
            CabinModelEnum.Model8W40,
        };

        /// <summary>
        /// Available Draw Combinations per Model
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, List<CabinDrawNumber>> DrawCombinationsPerModel = new()
        {
            { CabinModelEnum.Model9A,             new() { CabinDrawNumber.Draw9A,CabinDrawNumber.Draw9A9F} },
            { CabinModelEnum.Model9S,             new() { CabinDrawNumber.Draw9S,CabinDrawNumber.Draw9S9F,CabinDrawNumber.Draw9S9F9F} },
            { CabinModelEnum.Model94,             new() { CabinDrawNumber.Draw94,CabinDrawNumber.Draw949F,CabinDrawNumber.Draw949F9F} },
            { CabinModelEnum.Model9F,             new() { CabinDrawNumber.Draw9F,CabinDrawNumber.Draw9S9F,CabinDrawNumber.Draw9S9F9F,CabinDrawNumber.Draw949F,CabinDrawNumber.Draw9C9F,CabinDrawNumber.Draw9A9F,CabinDrawNumber.Draw9B9F,CabinDrawNumber.Draw9B9F9F} },
            { CabinModelEnum.Model9B,             new() { CabinDrawNumber.Draw9B, CabinDrawNumber.Draw9B9F, CabinDrawNumber.Draw9B9F9F } },
            { CabinModelEnum.ModelW,              new() { CabinDrawNumber.Draw8W,CabinDrawNumber.Draw8WFlipper81,CabinDrawNumber.Draw2Corner8W82,CabinDrawNumber.Draw1Corner8W84,CabinDrawNumber.Draw2Straight8W85,CabinDrawNumber.Draw2CornerStraight8W88} },
            { CabinModelEnum.ModelHB,             new() { CabinDrawNumber.DrawHB34,CabinDrawNumber.DrawCornerHB8W35,CabinDrawNumber.Draw2CornerHB37,CabinDrawNumber.DrawStraightHB8W40,CabinDrawNumber.Draw2StraightHB43} },
            { CabinModelEnum.ModelNP,             new() { CabinDrawNumber.DrawNP44, CabinDrawNumber.Draw2CornerNP46, CabinDrawNumber.DrawStraightNP6W47, CabinDrawNumber.Draw2StraightNP48, CabinDrawNumber.DrawCornerNP6W45 } },
            { CabinModelEnum.ModelVS,             new() { CabinDrawNumber.DrawVS,CabinDrawNumber.DrawVSVF} },
            { CabinModelEnum.ModelVF,             new() { CabinDrawNumber.DrawVF, CabinDrawNumber.DrawV4VF, CabinDrawNumber.DrawVSVF } },
            { CabinModelEnum.ModelV4,             new() { CabinDrawNumber.DrawV4,CabinDrawNumber.DrawV4VF} },
            { CabinModelEnum.ModelVA,             new() { CabinDrawNumber.DrawVA } },
            { CabinModelEnum.ModelWS,             new() { CabinDrawNumber.DrawWS } },
            { CabinModelEnum.ModelE,              new() { CabinDrawNumber.DrawE } },
            { CabinModelEnum.ModelWFlipper,       new() { CabinDrawNumber.None } },
            { CabinModelEnum.ModelDB,             new() { CabinDrawNumber.DrawDB51, CabinDrawNumber.DrawCornerDB8W52, CabinDrawNumber.Draw2CornerDB53, CabinDrawNumber.DrawStraightDB8W59, CabinDrawNumber.Draw2StraightDB61 } },
            { CabinModelEnum.ModelNB,             new() { CabinDrawNumber.DrawNB31, CabinDrawNumber.DrawCornerNB6W32, CabinDrawNumber.Draw2CornerNB33, CabinDrawNumber.DrawStraightNB6W38, CabinDrawNumber.Draw2StraightNB41} },
            { CabinModelEnum.ModelNV,             new() { CabinDrawNumber.DrawNV} },
            { CabinModelEnum.ModelMV2,            new() { CabinDrawNumber.DrawMV2 } },
            { CabinModelEnum.ModelNV2,            new() { CabinDrawNumber.DrawNV2 } },
            { CabinModelEnum.Model6WA,            new() { CabinDrawNumber.None } },
            { CabinModelEnum.Model9C,             new() { CabinDrawNumber.Draw9C,CabinDrawNumber.Draw9C9F } },
            { CabinModelEnum.Model8W40,           new() { CabinDrawNumber.Draw8W40} },
            { CabinModelEnum.ModelGlassContainer, new() {CabinDrawNumber.None} },
            { CabinModelEnum.ModelQB,             new() { CabinDrawNumber.DrawQB31, CabinDrawNumber.DrawCornerQB6W32, CabinDrawNumber.Draw2CornerQB33, CabinDrawNumber.DrawStraightQB6W38, CabinDrawNumber.Draw2StraightQB41} },
            { CabinModelEnum.ModelQP,             new() { CabinDrawNumber.DrawQP44, CabinDrawNumber.Draw2CornerQP46, CabinDrawNumber.DrawStraightQP6W47, CabinDrawNumber.Draw2StraightQP48, CabinDrawNumber.DrawCornerQP6W45 } },

        };

        /// <summary>
        /// The CabinDraws that Can Be Selected with the Specific Sliding Type
        /// </summary>
        public static readonly Dictionary<SlidingType, List<CabinDrawNumber>> AvaliableSeriesPerSlidingType = new()
        {
            { SlidingType.SingleSliding,                    new(){ CabinDrawNumber.Draw9S , CabinDrawNumber.DrawVS, CabinDrawNumber.DrawWS } },
            { SlidingType.SingleSlidingWithSidePanel,       new(){ CabinDrawNumber.Draw9S9F , CabinDrawNumber.DrawVSVF} },
            { SlidingType.SingleSlidingWith2SidePanels,     new(){ CabinDrawNumber.Draw9S9F9F } },
            { SlidingType.DoubleSliding,                    new(){ CabinDrawNumber.Draw94 , CabinDrawNumber.DrawV4 } },
            { SlidingType.DoubleSlidingWithSidePanel,       new(){ CabinDrawNumber.Draw949F , CabinDrawNumber.DrawV4VF } },
            { SlidingType.DoubleSlidingWith2SidePanels,     new(){ CabinDrawNumber.Draw949F9F } },
            { SlidingType.CornerSliding,                    new(){ CabinDrawNumber.Draw9A , CabinDrawNumber.DrawVA } },
            { SlidingType.CornerSlidingWithSidePanel,       new(){ CabinDrawNumber.Draw9A9F } },
            { SlidingType.SemiCircularSliding,              new(){ CabinDrawNumber.Draw9C } },
            { SlidingType.SemiCircularSlidingWithSidePanel, new(){ CabinDrawNumber.Draw9C9F } }
        };

        #endregion

        #region 2.Image Paths and Images Description Keys

        /// <summary>
        /// The Image Path for the Cabin Openings
        /// </summary>
        public static readonly Dictionary<OpeningCategory, string> OpeningImagePaths = new()
        {
            { OpeningCategory.FixedPanels,   "/Images/CabinImages/Openings/FixedPanels.png" },
            { OpeningCategory.Sliding,       "/Images/CabinImages/Openings/SlidingOpenings.png" },
            { OpeningCategory.StandardDoor,  "/Images/CabinImages/Openings/StandardDoors.png" },
            { OpeningCategory.DoorOnPanel,   "/Images/CabinImages/Openings/DoorsOnPanel.png" },
            { OpeningCategory.Folding,       "/Images/CabinImages/Openings/FoldingOpenings.png" },
            { OpeningCategory.BathtubPanels, "/Images/CabinImages/Openings/BathtubPanels.png" }
        };

        /// <summary>
        /// Maps the OpeningCategory Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<OpeningCategory, string> OpeningDescKey = new()
        {
            { OpeningCategory.FixedPanels,   "FixedPanels" },
            { OpeningCategory.Sliding,       "SlidingOpening" },
            { OpeningCategory.StandardDoor,  "StandardDoorOpening" },
            { OpeningCategory.DoorOnPanel,   "DoorOnPanelOpening" },
            { OpeningCategory.Folding,       "FoldingDoorOpening" },
            { OpeningCategory.BathtubPanels, "BathtubPanels" },
        };

        /// <summary>
        /// The Image Path for the Sliding Types of a Cabin
        /// </summary>
        public static readonly Dictionary<SlidingType, string> SlidingTypeImagePaths = new()
        {
            { SlidingType.SingleSliding,                    "/Images/CabinImages/Openings/SlidingOpenings/SingleSliding.png" },
            { SlidingType.SingleSlidingWithSidePanel,       "/Images/CabinImages/Openings/SlidingOpenings/SingleSlidingWithSidePanel.png" },
            { SlidingType.SingleSlidingWith2SidePanels,     "/Images/CabinImages/Openings/SlidingOpenings/SingleSlidingWith2SidePanels.png" },
            { SlidingType.DoubleSliding,                    "/Images/CabinImages/Openings/SlidingOpenings/DoubleSliding.png" },
            { SlidingType.DoubleSlidingWithSidePanel,       "/Images/CabinImages/Openings/SlidingOpenings/DoubleSlidingWithSidePanel.png" },
            { SlidingType.DoubleSlidingWith2SidePanels,     "/Images/CabinImages/Openings/SlidingOpenings/DoubleSlidingWith2SidePanels.png" },
            { SlidingType.CornerSliding,                    "/Images/CabinImages/Openings/SlidingOpenings/CornerSliding.png" },
            { SlidingType.CornerSlidingWithSidePanel,       "/Images/CabinImages/Openings/SlidingOpenings/CornerSlidingWithSidePanel.png" },
            { SlidingType.SemiCircularSliding,              "/Images/CabinImages/Openings/SlidingOpenings/SemicircularSliding.png" },
            { SlidingType.SemiCircularSlidingWithSidePanel, "/Images/CabinImages/Openings/SlidingOpenings/SemicircularSlidingWithSidePanel.png" },
        };

        /// <summary>
        /// Maps the SlidingType Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<SlidingType, string> SlidingTypeDescKey = new()
        {
            { SlidingType.SingleSliding,                    "SingleSliding" },
            { SlidingType.SingleSlidingWithSidePanel,       "SingleSlidingSidePanel" },
            { SlidingType.SingleSlidingWith2SidePanels,     "SingleSliding2SidePanels" },
            { SlidingType.DoubleSliding,                    "DoubleSliding" },
            { SlidingType.DoubleSlidingWithSidePanel,       "DoubleSlidingSidePanel" },
            { SlidingType.DoubleSlidingWith2SidePanels,     "DoubleSliding2SidePanels" },
            { SlidingType.CornerSliding,                    "CornerSliding" },
            { SlidingType.CornerSlidingWithSidePanel,       "CornerSlidingSidePanel" },
            { SlidingType.SemiCircularSliding,              "SemicircularSliding" },
            { SlidingType.SemiCircularSlidingWithSidePanel, "SemicircularSlidingSidePanel" },
        };

        /// <summary>
        /// The Image Path for the Folding Types of a Cabin
        /// </summary>
        public static readonly Dictionary<FoldingDoorType, string> FoldingTypeImagePaths = new()
        {
            { FoldingDoorType.SingleDoor44,              "/Images/CabinImages/Openings/FoldingOpenings/FoldingDoor44.png" },
            { FoldingDoorType.SingleDoorSidePanel45,     "/Images/CabinImages/Openings/FoldingOpenings/CornerFoldingDoorSideP45.png" },
            { FoldingDoorType.DoubleDoorCorner46,        "/Images/CabinImages/Openings/FoldingOpenings/DoubleCornerFolding46.png" },
            { FoldingDoorType.SingleDoorStraightPanel47, "/Images/CabinImages/Openings/FoldingOpenings/FoldingDoorwithFixed47.png" },
            { FoldingDoorType.DoubleDoorStraight48,      "/Images/CabinImages/Openings/FoldingOpenings/DoubleFoldDoor48.png" },
        };

        /// <summary>
        /// Maps the FoldingType Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<FoldingDoorType, string> FoldingTypeDescKey = new()
        {
            { FoldingDoorType.SingleDoor44,              "SingleDoor44" },
            { FoldingDoorType.SingleDoorSidePanel45,     "SingleDoorSidePanel45" },
            { FoldingDoorType.DoubleDoorCorner46,        "DoubleDoorCorner46" },
            { FoldingDoorType.SingleDoorStraightPanel47, "SingleDoorStraightPanel47" },
            { FoldingDoorType.DoubleDoorStraight48,      "DoubleDoorStraight48" },
        };

        /// <summary>
        /// The Image Path for the Standard Door Types of a Cabin
        /// </summary>
        public static readonly Dictionary<StandardDoorType, string> StandardDoorTypeImagePaths = new()
        {
            { StandardDoorType.Door3151,                "/Images/CabinImages/Openings/StandardDoorOpenings/Door3151.png" },
            { StandardDoorType.DoorSidePanel3252,       "/Images/CabinImages/Openings/StandardDoorOpenings/Door3252.png" },
            { StandardDoorType.DoubleCornerDoor3353,    "/Images/CabinImages/Openings/StandardDoorOpenings/Door3353.png" },
            { StandardDoorType.DoorStraightPanel3859,   "/Images/CabinImages/Openings/StandardDoorOpenings/Door3859.png" },
            { StandardDoorType.DoubleDoor4161,          "/Images/CabinImages/Openings/StandardDoorOpenings/Door4161.png" }
        };

        /// <summary>
        /// Maps the StandardDoorType Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<StandardDoorType, string> StandardDoorTypeDescKey = new()
        {
            { StandardDoorType.Door3151,                "Door3151" },
            { StandardDoorType.DoorSidePanel3252,       "DoorSidePanel3252" },
            { StandardDoorType.DoubleCornerDoor3353,    "DoubleCornerDoor3353" },
            { StandardDoorType.DoorStraightPanel3859,   "DoorStraightPanel3859" },
            { StandardDoorType.DoubleDoor4161,          "DoubleDoor4161" }
        };

        /// <summary>
        /// The Image Path for the DoorOnPanel Type of a Cabin
        /// </summary>
        public static readonly Dictionary<DoorOnPanelType, string> DoorOnPanelTypeImagePaths = new()
        {
            { DoorOnPanelType.Door34,               "/Images/CabinImages/Openings/DoorOnPanelOpenings/Door34.png" },
            { DoorOnPanelType.DoorSidePanel35,      "/Images/CabinImages/Openings/DoorOnPanelOpenings/Door35.png" },
            { DoorOnPanelType.DoubleCornerDoor37,   "/Images/CabinImages/Openings/DoorOnPanelOpenings/Door37.png" },
            { DoorOnPanelType.DoorStraightPanel40,  "/Images/CabinImages/Openings/DoorOnPanelOpenings/Door40.png" },
            { DoorOnPanelType.DoubleDoor43,         "/Images/CabinImages/Openings/DoorOnPanelOpenings/Door43.png" }
        };

        /// <summary>
        /// Maps the StandardDoorType Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<DoorOnPanelType, string> DoorOnPanelTypeDescKey = new()
        {
            { DoorOnPanelType.Door34,               "Door34" },
            { DoorOnPanelType.DoorSidePanel35,      "DoorSidePanel35" },
            { DoorOnPanelType.DoubleCornerDoor37,   "DoubleCornerDoor37" },
            { DoorOnPanelType.DoorStraightPanel40,  "DoorStraightPanel40" },
            { DoorOnPanelType.DoubleDoor43,         "DoubleDoor43" }
        };

        /// <summary>
        /// The Image Path for the FixedPanel Type of a Cabin
        /// </summary>
        public static readonly Dictionary<FixedPanelType, string> FixedPanelTypeImagePaths = new()
        {
            { FixedPanelType.WallPanel,             "/Images/CabinImages/Openings/FixedPanelsOpenings/8WWallPanel.png" },
            { FixedPanelType.FreeStandingPanel,     "/Images/CabinImages/Openings/FixedPanelsOpenings/8EFreeStandingPanel.png" },
            { FixedPanelType.PanelWithFlipper81,    "/Images/CabinImages/Openings/FixedPanelsOpenings/Flipper81.png" },
            { FixedPanelType.DoubleWallPanel82,     "/Images/CabinImages/Openings/FixedPanelsOpenings/Synthesis82.png" },
            { FixedPanelType.DoubleAngularPanel84,  "/Images/CabinImages/Openings/FixedPanelsOpenings/Synthesis84.png" },
            { FixedPanelType.DoublePanel85,         "/Images/CabinImages/Openings/FixedPanelsOpenings/Synthesis85.png" },
            { FixedPanelType.TriplePanel88,         "/Images/CabinImages/Openings/FixedPanelsOpenings/Synthesis88.png" }
        };

        /// <summary>
        /// Maps the FixedPanelType Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<FixedPanelType, string> FixedPanelTypeDescKey = new()
        {
            { FixedPanelType.WallPanel,             "WallPanel" },
            { FixedPanelType.FreeStandingPanel,     "FreeStandingPanel" },
            { FixedPanelType.PanelWithFlipper81,    "PanelWithFlipper81" },
            { FixedPanelType.DoubleWallPanel82,     "DoubleWallPanel82" },
            { FixedPanelType.DoubleAngularPanel84,  "DoubleAngularPanel84" },
            { FixedPanelType.DoublePanel85,         "DoublePanel85" },
            { FixedPanelType.TriplePanel88,         "TriplePanel88" }
        };

        /// <summary>
        /// The Image Path for the CabinSeries
        /// </summary>
        public static readonly Dictionary<CabinSeries, string> CabinSeriesImagePaths = new()
        {
            { CabinSeries.Bronze6000,      "/Images/CabinImages/Series/Bronze6000.png" },
            { CabinSeries.Inox304,         "/Images/CabinImages/Series/Inox304.png" },
            { CabinSeries.Smart,           "/Images/CabinImages/Series/Smart.png" },
            { CabinSeries.Niagara6000,     "/Images/CabinImages/Series/Niagara6000.png" },
            { CabinSeries.Hotel8000,       "/Images/CabinImages/Series/Hotel8000.png" },
            { CabinSeries.Free,            "/Images/CabinImages/Series/Free.png" },
            { CabinSeries.UndefinedSeries, "/Images/Various/NoImageAvailable.jpg" },
        };

        /// <summary>
        /// Maps the CabinSeries Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<CabinSeries, string> CabinSeriesDescKey = new()
        {
            { CabinSeries.UndefinedSeries,  "Undefined" },
            { CabinSeries.Bronze6000,  "Bronze6000" },
            { CabinSeries.Inox304,     "Inox304" },
            { CabinSeries.Smart,       "Smart"},
            { CabinSeries.Niagara6000, "Niagara6000" },
            { CabinSeries.Hotel8000,   "Hotel8000" },
            { CabinSeries.Free,        "Free" }
        };

        /// <summary>
        /// The Image Path for the Cabin Models
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, string> CabinModelEnumImagePaths = new()
        {
            { CabinModelEnum.Model9A,             "/Images/CabinImages/Series/ImgBronze6000/9A.png" },
            { CabinModelEnum.Model9S,             "/Images/CabinImages/Series/ImgBronze6000/9S.png" },
            { CabinModelEnum.Model94,             "/Images/CabinImages/Series/ImgBronze6000/94.png" },
            { CabinModelEnum.Model9F,             "/Images/CabinImages/Series/ImgBronze6000/9F.png" },
            { CabinModelEnum.Model9B,             "/Images/CabinImages/Series/ImgBronze6000/9B.png" },
            { CabinModelEnum.ModelW,              "/Images/CabinImages/Series/ImgFree/8W.png" },
            { CabinModelEnum.ModelHB,             "/Images/CabinImages/Series/ImgHB/HB34.png" },
            { CabinModelEnum.ModelNP,             "/Images/CabinImages/Series/ImgNP/NP44.png" },
            { CabinModelEnum.ModelVS,             "/Images/CabinImages/Series/ImgInox304/VS.png" },
            { CabinModelEnum.ModelVF,             "/Images/CabinImages/Series/ImgInox304/VF.png" },
            { CabinModelEnum.ModelV4,             "/Images/CabinImages/Series/ImgInox304/V4.png" },
            { CabinModelEnum.ModelVA,             "/Images/CabinImages/Series/ImgInox304/VA.png" },
            { CabinModelEnum.ModelWS,             "/Images/CabinImages/Series/ImgSmart/WS.png" },
            { CabinModelEnum.ModelE,              "/Images/CabinImages/Series/ImgFree/8E.png" },
            { CabinModelEnum.ModelWFlipper,       "/Images/CabinImages/Series/ImgFree/Flipper.png" },
            { CabinModelEnum.ModelDB,             "/Images/CabinImages/Series/ImgDB/DB51.png" },
            { CabinModelEnum.ModelNB,             "/Images/CabinImages/Series/ImgNB/NB31.png" },
            { CabinModelEnum.ModelNV,             "/Images/CabinImages/Series/ImgNVMV/NV.png" },
            { CabinModelEnum.ModelMV2,            "/Images/CabinImages/Series/ImgNVMV/MV2.png" },
            { CabinModelEnum.ModelNV2,            "/Images/CabinImages/Series/ImgNVMV/NV2.png" },
            { CabinModelEnum.Model6WA,            "/Images/CabinImages/Series/ImgNVMV/6WA30.png" },
            { CabinModelEnum.Model9C,             "/Images/CabinImages/Series/ImgBronze6000/9C.png"},
            { CabinModelEnum.Model8W40,           "/Images/CabinImages/Series/ImgNVMV/8W40.png"},
            { CabinModelEnum.ModelGlassContainer, "/Images/Various/NoImageAvailable.jpg" },
            { CabinModelEnum.ModelQB,             "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QB31.jpg"},
            { CabinModelEnum.ModelQP,             "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QP44.jpg"}

        };

        /// <summary>
        /// Maps the CabinModel Enum Value to the Language Key Description
        /// </summary>
        public static readonly Dictionary<CabinModelEnum, string> CabinModelEnumDescKey = new()
        {
            { CabinModelEnum.Model9A,             "Model9A" },
            { CabinModelEnum.Model9S,             "Model9S" },
            { CabinModelEnum.Model94,             "Model94" },
            { CabinModelEnum.Model9F,             "Model9F" },
            { CabinModelEnum.Model9B,             "Model9B" },
            { CabinModelEnum.ModelW,              "ModelW" },
            { CabinModelEnum.ModelHB,             "ModelHB" },
            { CabinModelEnum.ModelNP,             "ModelNP" },
            { CabinModelEnum.ModelVS,             "ModelVS" },
            { CabinModelEnum.ModelVF,             "ModelVF" },
            { CabinModelEnum.ModelV4,             "ModelV4" },
            { CabinModelEnum.ModelVA,             "ModelVA" },
            { CabinModelEnum.ModelWS,             "ModelWS" },
            { CabinModelEnum.ModelE,              "ModelE" },
            { CabinModelEnum.ModelWFlipper,       "ModelWFlipper" },
            { CabinModelEnum.ModelDB,             "ModelDB" },
            { CabinModelEnum.ModelNB,             "ModelNB" },
            { CabinModelEnum.ModelNV,             "ModelNV" },
            { CabinModelEnum.ModelMV2,            "ModelMV" },
            { CabinModelEnum.ModelNV2,            "ModelNV2" },
            { CabinModelEnum.Model6WA,            "Model6WA" },
            { CabinModelEnum.Model9C,             "Model9C" },
            { CabinModelEnum.Model8W40,           "Model8W40" },
            { CabinModelEnum.ModelGlassContainer, "ModelGlassContainer" },
            { CabinModelEnum.ModelQB,             "ModelQB" },
            { CabinModelEnum.ModelQP,             "ModelQP" },
        };

        /// <summary>
        /// Maps the CabinDrawNumber Enum Value to the Sketch Image Path
        /// </summary>
        public static readonly Dictionary<CabinDrawNumber, string> CabinDrawNumberSketchImagePath = new()
        {
            { CabinDrawNumber.None,                     ""},
            { CabinDrawNumber.Draw9S,                   "/Images/CabinImages/AllCabinDrawSketches/Sliding.png"},
            { CabinDrawNumber.Draw9S9F,                 "/Images/CabinImages/AllCabinDrawSketches/SlidingSidePanel.png"},
            { CabinDrawNumber.Draw9S9F9F,               "/Images/CabinImages/AllCabinDrawSketches/Sliding2SidePanels.png"},
            { CabinDrawNumber.Draw94,                   "/Images/CabinImages/AllCabinDrawSketches/DoubleSliding.png"},
            { CabinDrawNumber.Draw949F,                 "/Images/CabinImages/AllCabinDrawSketches/DoubleSlidingSidePanel.png"},
            { CabinDrawNumber.Draw949F9F,               "/Images/CabinImages/AllCabinDrawSketches/DoubleSliding2SidePanels.png"},
            { CabinDrawNumber.Draw9A,                   "/Images/CabinImages/AllCabinDrawSketches/CornerEntry.png"},
            { CabinDrawNumber.Draw9A9F,                 "/Images/CabinImages/AllCabinDrawSketches/CornerEntrySidePanel.png"},
            { CabinDrawNumber.Draw9C,                   "/Images/CabinImages/AllCabinDrawSketches/Semicircular.png"},
            { CabinDrawNumber.Draw9C9F,                 "/Images/CabinImages/AllCabinDrawSketches/SemicircularSidePanel.png"},
            { CabinDrawNumber.Draw9B,                   "/Images/CabinImages/AllCabinDrawSketches/PivotDoor.png"},
            { CabinDrawNumber.Draw9B9F,                 "/Images/CabinImages/AllCabinDrawSketches/PivotDoorSidePanel.png"},
            { CabinDrawNumber.Draw9B9F9F,               "/Images/CabinImages/AllCabinDrawSketches/PivotDoor2SidePanels.png"},
            { CabinDrawNumber.DrawVS,                   "/Images/CabinImages/AllCabinDrawSketches/Sliding.png"},
            { CabinDrawNumber.DrawVSVF,                 "/Images/CabinImages/AllCabinDrawSketches/SlidingSidePanel.png"},
            { CabinDrawNumber.DrawV4,                   "/Images/CabinImages/AllCabinDrawSketches/DoubleSliding.png"},
            { CabinDrawNumber.DrawV4VF,                 "/Images/CabinImages/AllCabinDrawSketches/DoubleSlidingSidePanel.png"},
            { CabinDrawNumber.DrawVA,                   "/Images/CabinImages/AllCabinDrawSketches/CornerEntry.png"},
            { CabinDrawNumber.DrawWS,                   "/Images/CabinImages/AllCabinDrawSketches/SlidingSmartWS.png"},
            { CabinDrawNumber.DrawNP44,                 "/Images/CabinImages/AllCabinDrawSketches/44-Folding-Door.png"},
            { CabinDrawNumber.Draw2CornerNP46,          "/Images/CabinImages/AllCabinDrawSketches/46-Double-Corner-Folding-Do.png"},
            { CabinDrawNumber.Draw2StraightNP48,        "/Images/CabinImages/AllCabinDrawSketches/48DoubleFoldDoor.png"},
            { CabinDrawNumber.DrawCornerNP6W45,         "/Images/CabinImages/AllCabinDrawSketches/45-Corner-Folding-Door-with.png"},
            { CabinDrawNumber.DrawStraightNP6W47,       "/Images/CabinImages/AllCabinDrawSketches/47-Folding-Door-with-Fixed-.png"},
            { CabinDrawNumber.DrawNB31,                 "/Images/CabinImages/AllCabinDrawSketches/31-SingleDoor.png"},
            { CabinDrawNumber.DrawCornerNB6W32,         "/Images/CabinImages/AllCabinDrawSketches/32-Corner-Door-and-Panel.png"},
            { CabinDrawNumber.Draw2CornerNB33,          "/Images/CabinImages/AllCabinDrawSketches/33-DoubleDoor.png"},
            { CabinDrawNumber.DrawStraightNB6W38,       "/Images/CabinImages/AllCabinDrawSketches/38-Door-and-Panel-Straight.png"},
            { CabinDrawNumber.Draw2StraightNB41,        "/Images/CabinImages/AllCabinDrawSketches/41-SaloonDoor.png"},
            { CabinDrawNumber.DrawDB51,                 "/Images/CabinImages/AllCabinDrawSketches/51-SingleDoor.png"},
            { CabinDrawNumber.DrawCornerDB8W52,         "/Images/CabinImages/AllCabinDrawSketches/52 DoorAndSideFixed.png"},
            { CabinDrawNumber.Draw2CornerDB53,          "/Images/CabinImages/AllCabinDrawSketches/53 DoubleDoorCorner.png"},
            { CabinDrawNumber.DrawStraightDB8W59,       "/Images/CabinImages/AllCabinDrawSketches/59 DoorAndFixed.png"},
            { CabinDrawNumber.Draw2StraightDB61,        "/Images/CabinImages/AllCabinDrawSketches/61 DoubleDoorSaloon.png"},
            { CabinDrawNumber.DrawHB34,                 "/Images/CabinImages/AllCabinDrawSketches/34-DoorOnFixed.png"},
            { CabinDrawNumber.DrawCornerHB8W35,         "/Images/CabinImages/AllCabinDrawSketches/35-DoorOnFixedSidePanel.png"},
            { CabinDrawNumber.Draw2CornerHB37,          "/Images/CabinImages/AllCabinDrawSketches/37 DoubleDoorOnFixedCorner.png"},
            { CabinDrawNumber.DrawStraightHB8W40,       "/Images/CabinImages/AllCabinDrawSketches/40 DoorOnFixedExtrPanel.png"},
            { CabinDrawNumber.Draw2StraightHB43,        "/Images/CabinImages/AllCabinDrawSketches/43 DoubleDoorOnFixedStr.png"},
            { CabinDrawNumber.Draw8W,                   "/Images/CabinImages/AllCabinDrawSketches/W.png"},
            { CabinDrawNumber.DrawE,                    "/Images/CabinImages/AllCabinDrawSketches/E.png"},
            { CabinDrawNumber.Draw8WFlipper81,          "/Images/CabinImages/AllCabinDrawSketches/81Flipper.png"},
            { CabinDrawNumber.Draw2Corner8W82,          "/Images/CabinImages/AllCabinDrawSketches/82DoubleFixedCorner.png"},
            { CabinDrawNumber.Draw1Corner8W84,          "/Images/CabinImages/AllCabinDrawSketches/84.png"},
            { CabinDrawNumber.Draw2Straight8W85,        "/Images/CabinImages/AllCabinDrawSketches/85.png"},
            { CabinDrawNumber.Draw2CornerStraight8W88,  "/Images/CabinImages/AllCabinDrawSketches/88.png" },
            { CabinDrawNumber.Draw8W40,                 "/Images/CabinImages/AllCabinDrawSketches/8W40.png" },
            { CabinDrawNumber.DrawNV,                   "/Images/CabinImages/AllCabinDrawSketches/NV.png" },
            { CabinDrawNumber.DrawNV2,                  "/Images/CabinImages/AllCabinDrawSketches/NV2.png" },
            { CabinDrawNumber.DrawMV2,                  "/Images/CabinImages/AllCabinDrawSketches/MV2.png" },
            { CabinDrawNumber.Draw9F,                   ""},
            { CabinDrawNumber.DrawVF,                   ""},
            { CabinDrawNumber.DrawQP44,                 "/Images/CabinImages/AllCabinDrawSketches/44-Folding-Door.png"},
            { CabinDrawNumber.Draw2CornerQP46,          "/Images/CabinImages/AllCabinDrawSketches/46-Double-Corner-Folding-Do.png"},
            { CabinDrawNumber.Draw2StraightQP48,        "/Images/CabinImages/AllCabinDrawSketches/48DoubleFoldDoor.png"},
            { CabinDrawNumber.DrawCornerQP6W45,         "/Images/CabinImages/AllCabinDrawSketches/45-Corner-Folding-Door-with.png"},
            { CabinDrawNumber.DrawStraightQP6W47,       "/Images/CabinImages/AllCabinDrawSketches/47-Folding-Door-with-Fixed-.png"},
            { CabinDrawNumber.DrawQB31,                 "/Images/CabinImages/AllCabinDrawSketches/31-SingleDoor.png"},
            { CabinDrawNumber.DrawCornerQB6W32,         "/Images/CabinImages/AllCabinDrawSketches/32-Corner-Door-and-Panel.png"},
            { CabinDrawNumber.Draw2CornerQB33,          "/Images/CabinImages/AllCabinDrawSketches/33-DoubleDoor.png"},
            { CabinDrawNumber.DrawStraightQB6W38,       "/Images/CabinImages/AllCabinDrawSketches/38-Door-and-Panel-Straight.png"},
            { CabinDrawNumber.Draw2StraightQB41,        "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QB41.png"},
        };

        /// <summary>
        /// Maps the CabinDrawNumber Enum Value to the Sketch Image Path
        /// </summary>
        public static readonly Dictionary<CabinDrawNumber, string> CabinDrawNumberImagePath = new()
        {
            { CabinDrawNumber.None,                     ""},
            { CabinDrawNumber.Draw9S,                   "/Images/CabinImages/Series/ImgBronze6000/9S.png"},
            { CabinDrawNumber.Draw9S9F,                 "/Images/CabinImages/Series/ImgBronze6000/9S9F.png"},
            { CabinDrawNumber.Draw9S9F9F,               "/Images/CabinImages/Series/ImgBronze6000/9S9F.png"},
            { CabinDrawNumber.Draw94,                   "/Images/CabinImages/Series/ImgBronze6000/94.png"},
            { CabinDrawNumber.Draw949F,                 "/Images/CabinImages/Series/ImgBronze6000/949F.png"},
            { CabinDrawNumber.Draw949F9F,               "/Images/CabinImages/Series/ImgBronze6000/949F.png"},
            { CabinDrawNumber.Draw9A,                   "/Images/CabinImages/Series/ImgBronze6000/9A.png"},
            { CabinDrawNumber.Draw9A9F,                 "/Images/CabinImages/Series/ImgBronze6000/9A.png"},
            { CabinDrawNumber.Draw9C,                   "/Images/CabinImages/Series/ImgBronze6000/9C.png"},
            { CabinDrawNumber.Draw9C9F,                 "/Images/CabinImages/Series/ImgBronze6000/9C.png"},
            { CabinDrawNumber.Draw9B,                   "/Images/CabinImages/Series/ImgBronze6000/9B.png"},
            { CabinDrawNumber.Draw9B9F,                 "/Images/CabinImages/Series/ImgBronze6000/9B9F.png"},
            { CabinDrawNumber.Draw9B9F9F,               "/Images/CabinImages/Series/ImgBronze6000/9B9F.png"},
            { CabinDrawNumber.DrawVS,                   "/Images/CabinImages/Series/ImgInox304/VS.png"},
            { CabinDrawNumber.DrawVSVF,                 "/Images/CabinImages/Series/ImgInox304/VSVF.png"},
            { CabinDrawNumber.DrawV4,                   "/Images/CabinImages/Series/ImgInox304/V4.png"},
            { CabinDrawNumber.DrawV4VF,                 "/Images/CabinImages/Series/ImgInox304/V4.png"},
            { CabinDrawNumber.DrawVA,                   "/Images/CabinImages/Series/ImgInox304/VA.png"},
            { CabinDrawNumber.DrawWS,                   "/Images/CabinImages/Series/ImgSmart/WS.png"},
            { CabinDrawNumber.DrawNP44,                 "/Images/CabinImages/Series/ImgNP/NP44.png"},
            { CabinDrawNumber.Draw2CornerNP46,          "/Images/CabinImages/Series/ImgNP/NP46.png"},
            { CabinDrawNumber.Draw2StraightNP48,        "/Images/CabinImages/Series/ImgNP/NP48.png"},
            { CabinDrawNumber.DrawCornerNP6W45,         "/Images/CabinImages/Series/ImgNP/NP45.png"},
            { CabinDrawNumber.DrawStraightNP6W47,       "/Images/CabinImages/Series/ImgNP/NP47.png"},
            { CabinDrawNumber.DrawNB31,                 "/Images/CabinImages/Series/ImgNB/NB31.png"},
            { CabinDrawNumber.DrawCornerNB6W32,         "/Images/CabinImages/Series/ImgNB/NB32.png"},
            { CabinDrawNumber.Draw2CornerNB33,          "/Images/CabinImages/Series/ImgNB/NB33.png"},
            { CabinDrawNumber.DrawStraightNB6W38,       "/Images/CabinImages/Series/ImgNB/NB38.png"},
            { CabinDrawNumber.Draw2StraightNB41,        "/Images/CabinImages/Series/ImgNB/NB41.png"},
            { CabinDrawNumber.DrawDB51,                 "/Images/CabinImages/Series/ImgDB/DB51.png"},
            { CabinDrawNumber.DrawCornerDB8W52,         "/Images/CabinImages/Series/ImgDB/DB51.png"},
            { CabinDrawNumber.Draw2CornerDB53,          "/Images/CabinImages/Series/ImgDB/DB51.png"},
            { CabinDrawNumber.DrawStraightDB8W59,       "/Images/CabinImages/Series/ImgDB/DB51.png"},
            { CabinDrawNumber.Draw2StraightDB61,        "/Images/CabinImages/Series/ImgDB/DB51.png"},
            { CabinDrawNumber.DrawHB34,                 "/Images/CabinImages/Series/ImgHB/HB34.png"},
            { CabinDrawNumber.DrawCornerHB8W35,         "/Images/CabinImages/Series/ImgHB/HB35.png"},
            { CabinDrawNumber.Draw2CornerHB37,          "/Images/CabinImages/Series/ImgHB/HB37.png"},
            { CabinDrawNumber.DrawStraightHB8W40,       "/Images/CabinImages/Series/ImgHB/HB40.png"},
            { CabinDrawNumber.Draw2StraightHB43,        "/Images/CabinImages/Series/ImgHB/HB43.png"},
            { CabinDrawNumber.Draw8W,                   "/Images/CabinImages/Series/ImgFree/8W.png"},
            { CabinDrawNumber.DrawE,                    "/Images/CabinImages/Series/ImgFree/8E.png"},
            { CabinDrawNumber.Draw8WFlipper81,          "/Images/CabinImages/Series/ImgFree/Flipper.png"},
            { CabinDrawNumber.Draw2Corner8W82,          "/Images/CabinImages/Series/ImgFree/82.png"},
            { CabinDrawNumber.Draw1Corner8W84,          "/Images/CabinImages/Series/ImgFree/84.png"},
            { CabinDrawNumber.Draw2Straight8W85,        "/Images/CabinImages/Series/ImgFree/8W.png"},
            { CabinDrawNumber.Draw2CornerStraight8W88,  "/Images/CabinImages/Series/ImgFree/88.png" },
            { CabinDrawNumber.DrawNV,                   "/Images/CabinImages/Series/ImgNVMV/NV.png" },
            { CabinDrawNumber.DrawNV2,                  "/Images/CabinImages/Series/ImgNVMV/NV2.png" },
            { CabinDrawNumber.DrawMV2,                  "/Images/CabinImages/Series/ImgNVMV/MV2.png" },
            { CabinDrawNumber.Draw8W40,                 "/Images/CabinImages/Series/ImgNVMV/8W40.png" },
            { CabinDrawNumber.Draw9F,                   ""},
            { CabinDrawNumber.DrawVF,                   ""},
            { CabinDrawNumber.DrawQP44,                 "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QP44.jpg"},
            { CabinDrawNumber.Draw2CornerQP46,          "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QP46.jpg"},
            { CabinDrawNumber.Draw2StraightQP48,        "/Images/CabinImages/Series/ImgNP/NP48.png"},
            { CabinDrawNumber.DrawCornerQP6W45,         "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QP45.jpg"},
            { CabinDrawNumber.DrawStraightQP6W47,       "/Images/CabinImages/Series/ImgNP/NP47.png"},
            { CabinDrawNumber.DrawQB31,                 "https://storagebronze.blob.core.windows.net/bronzewebapp-images/Cabins/Models/QBQP/QB31.jpg"},
            { CabinDrawNumber.DrawCornerQB6W32,         "/Images/CabinImages/Series/ImgNB/NB32.png"},
            { CabinDrawNumber.Draw2CornerQB33,          "/Images/CabinImages/Series/ImgNB/NB33.png"},
            { CabinDrawNumber.DrawStraightQB6W38,       "/Images/CabinImages/Series/ImgNB/NB38.png"},
            { CabinDrawNumber.Draw2StraightQB41,        "/Images/CabinImages/Series/ImgNB/NB41.png"},
        };

        /// <summary>
        /// Maps the CabinDrawNumber Enum Value to the Description Key
        /// </summary>
        public static readonly Dictionary<CabinDrawNumber, string> CabinDrawNumberDescKey = new()
        {
            { CabinDrawNumber.None,                     "DrawNone"},
            { CabinDrawNumber.Draw9S,                   "Draw9S"},
            { CabinDrawNumber.Draw9S9F,                 "Draw9S9F"},
            { CabinDrawNumber.Draw9S9F9F,               "Draw9S9F9F"},
            { CabinDrawNumber.Draw94,                   "Draw94"},
            { CabinDrawNumber.Draw949F,                 "Draw949F"},
            { CabinDrawNumber.Draw949F9F,               "Draw949F9F"},
            { CabinDrawNumber.Draw9A,                   "Draw9A"},
            { CabinDrawNumber.Draw9A9F,                 "Draw9A9F"},
            { CabinDrawNumber.Draw9C,                   "Draw9C"},
            { CabinDrawNumber.Draw9C9F,                 "Draw9C9F"},
            { CabinDrawNumber.Draw9B,                   "Draw9B"},
            { CabinDrawNumber.Draw9B9F,                 "Draw9B9F"},
            { CabinDrawNumber.Draw9B9F9F,               "Draw9B9F9F"},
            { CabinDrawNumber.DrawVS,                   "DrawVS"},
            { CabinDrawNumber.DrawVSVF,                 "DrawVSVF"},
            { CabinDrawNumber.DrawV4,                   "DrawV4"},
            { CabinDrawNumber.DrawV4VF,                 "DrawV4VF"},
            { CabinDrawNumber.DrawVA,                   "DrawVA"},
            { CabinDrawNumber.DrawWS,                   "DrawWS"},
            { CabinDrawNumber.DrawNP44,                 "DrawNP44"},
            { CabinDrawNumber.Draw2CornerNP46,          "Draw2CornerNP46"},
            { CabinDrawNumber.Draw2StraightNP48,        "Draw2StraightNP48"},
            { CabinDrawNumber.DrawCornerNP6W45,         "DrawCornerNP6W45"},
            { CabinDrawNumber.DrawStraightNP6W47,       "DrawStraightNP6W47"},
            { CabinDrawNumber.DrawNB31,                 "DrawNB31"},
            { CabinDrawNumber.DrawCornerNB6W32,         "DrawCornerNB6W32"},
            { CabinDrawNumber.Draw2CornerNB33,          "Draw2CornerNB33"},
            { CabinDrawNumber.DrawStraightNB6W38,       "DrawStraightNB6W38"},
            { CabinDrawNumber.Draw2StraightNB41,        "Draw2StraightNB41"},
            { CabinDrawNumber.DrawDB51,                 "DrawDB51"},
            { CabinDrawNumber.DrawCornerDB8W52,         "DrawCornerDB8W52"},
            { CabinDrawNumber.Draw2CornerDB53,          "Draw2CornerDB53"},
            { CabinDrawNumber.DrawStraightDB8W59,       "DrawStraightDB8W59"},
            { CabinDrawNumber.Draw2StraightDB61,        "Draw2StraightDB61"},
            { CabinDrawNumber.DrawHB34,                 "DrawHB34"},
            { CabinDrawNumber.DrawCornerHB8W35,         "DrawCornerHB8W35"},
            { CabinDrawNumber.Draw2CornerHB37,          "Draw2CornerHB37"},
            { CabinDrawNumber.DrawStraightHB8W40,       "DrawStraightHB8W40"},
            { CabinDrawNumber.Draw2StraightHB43,        "Draw2StraightHB43"},
            { CabinDrawNumber.Draw8W,                   "Draw8W"},
            { CabinDrawNumber.DrawE,                    "DrawE"},
            { CabinDrawNumber.Draw8WFlipper81,          "Draw8WFlipper81"},
            { CabinDrawNumber.Draw2Corner8W82,          "Draw2Corner8W82"},
            { CabinDrawNumber.Draw1Corner8W84,          "Draw1Corner8W84"},
            { CabinDrawNumber.Draw2Straight8W85,        "Draw2Straight8W85"},
            { CabinDrawNumber.Draw2CornerStraight8W88,  "Draw2CornerStraight8W88" },
            { CabinDrawNumber.DrawNV,                   "DrawNV" },
            { CabinDrawNumber.DrawNV2,                  "DrawNV2" },
            { CabinDrawNumber.DrawMV2,                  "DrawMV2" },
            { CabinDrawNumber.Draw8W40,                 "Draw8W40" },
            { CabinDrawNumber.Draw9F,                   "Draw9F"},
            { CabinDrawNumber.DrawVF,                   "DrawVF"},
            { CabinDrawNumber.DrawQP44,                 "DrawQP44"},
            { CabinDrawNumber.Draw2CornerQP46,          "Draw2CornerQP46"},
            { CabinDrawNumber.Draw2StraightQP48,        "Draw2StraightQP48"},
            { CabinDrawNumber.DrawCornerQP6W45,         "DrawCornerQP6W45"},
            { CabinDrawNumber.DrawStraightQP6W47,       "DrawStraightQP6W47"},
            { CabinDrawNumber.DrawQB31,                 "DrawQB31"},
            { CabinDrawNumber.DrawCornerQB6W32,         "DrawCornerQB6W32"},
            { CabinDrawNumber.Draw2CornerQB33,          "Draw2CornerQB33"},
            { CabinDrawNumber.DrawStraightQB6W38,       "DrawStraightQB6W38"},
            { CabinDrawNumber.Draw2StraightQB41,        "Draw2StraightQB41"},
        };

        /// <summary>
        /// Maps the CabinDrawNumber Enum Value to the Sketch Image Path
        /// </summary>
        public static readonly Dictionary<(CabinDrawNumber,CabinSynthesisModel), string> CabinDrawNumberStepImagePath = new()
        {
            { (CabinDrawNumber.None,                    CabinSynthesisModel.Primary), ""},
            { (CabinDrawNumber.Draw9S,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SStep.png" },
            { (CabinDrawNumber.Draw9S9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFStep.png" },
            { (CabinDrawNumber.Draw9S9F9F,              CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFFStep.png" },
            { (CabinDrawNumber.Draw94,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4Step.png" },
            { (CabinDrawNumber.Draw949F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FStep.png" },
            { (CabinDrawNumber.Draw949F9F,              CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FFStep.png" },
            { (CabinDrawNumber.Draw9A,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AStep.png" },
            { (CabinDrawNumber.Draw9A9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AFFStep.png" },
            { (CabinDrawNumber.Draw9C,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/CNoStep.png" },
            { (CabinDrawNumber.Draw9C9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/CFStep.png" },
            { (CabinDrawNumber.Draw9B,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/BNoStep.png" },
            { (CabinDrawNumber.Draw9B9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/BFStep.png" },
            { (CabinDrawNumber.Draw9B9F9F,              CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/BFFStep.png" },
            { (CabinDrawNumber.DrawVS,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SStep.png" },
            { (CabinDrawNumber.DrawVSVF,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFStep.png" },
            { (CabinDrawNumber.DrawV4,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4Step.png" },
            { (CabinDrawNumber.DrawV4VF,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FStep.png" },
            { (CabinDrawNumber.DrawVA,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AStep.png" },
            { (CabinDrawNumber.DrawWS,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/WSNoStep.png" },
            { (CabinDrawNumber.DrawNP44,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/44NoStep.png" },
            { (CabinDrawNumber.Draw2CornerNP46,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/46NoStep.png" },
            { (CabinDrawNumber.Draw2StraightNP48,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/48NoStep.png" },
            { (CabinDrawNumber.DrawCornerNP6W45,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/45Step.png" },
            { (CabinDrawNumber.DrawStraightNP6W47,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/47Step.png" },
            { (CabinDrawNumber.DrawNB31,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/47Step.png" },
            { (CabinDrawNumber.DrawCornerNB6W32,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/32Step.png" },
            { (CabinDrawNumber.Draw2CornerNB33,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/31NoStep.png" },
            { (CabinDrawNumber.DrawStraightNB6W38,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/33NoStep.png" },
            { (CabinDrawNumber.Draw2StraightNB41,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/41NoStep.png" },
            { (CabinDrawNumber.DrawDB51,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/51NoStep.png" },
            { (CabinDrawNumber.DrawCornerDB8W52,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/52Step.png" },
            { (CabinDrawNumber.Draw2CornerDB53,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/53NoStep.png" },
            { (CabinDrawNumber.DrawStraightDB8W59,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/59Step.png" },
            { (CabinDrawNumber.Draw2StraightDB61,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/61NoStep.png" },
            { (CabinDrawNumber.DrawHB34,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/34Step.png" },
            { (CabinDrawNumber.DrawCornerHB8W35,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/35Step.png" },
            { (CabinDrawNumber.Draw2CornerHB37,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/37Step.png" },
            { (CabinDrawNumber.DrawStraightHB8W40,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/40Step.png" },
            { (CabinDrawNumber.Draw2StraightHB43,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/43Step.png" },
            { (CabinDrawNumber.Draw8W,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/WStep.png" },
            { (CabinDrawNumber.DrawE,                   CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/ENoStep.png" },
            { (CabinDrawNumber.Draw8WFlipper81,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/81Step.png" },
            { (CabinDrawNumber.Draw2Corner8W82,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/82Step.png" },
            { (CabinDrawNumber.Draw1Corner8W84,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/84Step.png" },
            { (CabinDrawNumber.Draw2Straight8W85,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/85Step.png" },
            { (CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/88Step.png" },
            { (CabinDrawNumber.Draw8W40,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/8W40Step.png" },
            { (CabinDrawNumber.DrawNV,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/NVNoStep.png" },
            { (CabinDrawNumber.DrawNV2,                 CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/NV2NoStep.png" },
            { (CabinDrawNumber.DrawMV2,                 CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/MV2NoStep.png" },
            { (CabinDrawNumber.Draw9F,                  CabinSynthesisModel.Primary), ""},
            { (CabinDrawNumber.DrawVF,                  CabinSynthesisModel.Primary), ""},
            { (CabinDrawNumber.DrawQP44,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/44NoStep.png" },
            { (CabinDrawNumber.Draw2CornerQP46,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/46NoStep.png" },
            { (CabinDrawNumber.Draw2StraightQP48,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/48NoStep.png" },
            { (CabinDrawNumber.DrawCornerQP6W45,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/45Step.png" },
            { (CabinDrawNumber.DrawStraightQP6W47,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/47Step.png" },
            { (CabinDrawNumber.DrawQB31,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/47Step.png" },
            { (CabinDrawNumber.DrawCornerQB6W32,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/32Step.png" },
            { (CabinDrawNumber.Draw2CornerQB33,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/31NoStep.png" },
            { (CabinDrawNumber.DrawStraightQB6W38,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/33NoStep.png" },
            { (CabinDrawNumber.Draw2StraightQB41,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/41NoStep.png" },

            { (CabinDrawNumber.None,                    CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw9S,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw9S9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFStepSecond.png" },
            { (CabinDrawNumber.Draw9S9F9F,              CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFFStepSecond.png" },
            { (CabinDrawNumber.Draw94,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw949F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FStepSecond.png" },
            { (CabinDrawNumber.Draw949F9F,              CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FFStepSecond.png" },
            { (CabinDrawNumber.Draw9A,                  CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AStepSecond.png" },
            { (CabinDrawNumber.Draw9A9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AFFStepSecond.png" },
            { (CabinDrawNumber.Draw9C,                  CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/CNoStepSecond.png" },
            { (CabinDrawNumber.Draw9C9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/CFStepSecond.png" },
            { (CabinDrawNumber.Draw9B,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw9B9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/BFStepSecond.png" },
            { (CabinDrawNumber.Draw9B9F9F,              CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/BFFStepSecond.png" },
            { (CabinDrawNumber.DrawVS,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawVSVF,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFStepSecond.png" },
            { (CabinDrawNumber.DrawV4,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawV4VF,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FStepSecond.png" },
            { (CabinDrawNumber.DrawVA,                  CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AStepSecond.png" },
            { (CabinDrawNumber.DrawWS,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawNP44,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw2CornerNP46,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/46NoStepSecond.png" },
            { (CabinDrawNumber.Draw2StraightNP48,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/48NoStepSecond.png" },
            { (CabinDrawNumber.DrawCornerNP6W45,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/45StepSecond.png" },
            { (CabinDrawNumber.DrawStraightNP6W47,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/47StepSecond.png" },
            { (CabinDrawNumber.DrawNB31,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerNB6W32,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/32StepSecond.png" },
            { (CabinDrawNumber.Draw2CornerNB33,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/33NoStepSecond.png" },
            { (CabinDrawNumber.DrawStraightNB6W38,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/38StepSecond.png" },
            { (CabinDrawNumber.Draw2StraightNB41,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/41NoStepSecond.png" },
            { (CabinDrawNumber.DrawDB51,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerDB8W52,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/52StepSecond.png" },
            { (CabinDrawNumber.Draw2CornerDB53,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/53NoStepSecond.png" },
            { (CabinDrawNumber.DrawStraightDB8W59,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/59StepSecond.png" },
            { (CabinDrawNumber.Draw2StraightDB61,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/61NoStepSecond.png" },
            { (CabinDrawNumber.DrawHB34,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerHB8W35,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/35StepSecond.png" },
            { (CabinDrawNumber.Draw2CornerHB37,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/37StepSecond.png" },
            { (CabinDrawNumber.DrawStraightHB8W40,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/40StepSecond.png" },
            { (CabinDrawNumber.Draw2StraightHB43,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/43StepSecond.png" },
            { (CabinDrawNumber.Draw8W,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawE,                   CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw8WFlipper81,         CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw2Corner8W82,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/82StepSecond.png" },
            { (CabinDrawNumber.Draw1Corner8W84,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/84StepSecond.png" },
            { (CabinDrawNumber.Draw2Straight8W85,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/85StepSecond.png" },
            { (CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/88StepSecond.png" },
            { (CabinDrawNumber.Draw8W40,                CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.DrawNV,                  CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.DrawNV2,                 CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.DrawMV2,                 CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.Draw9F,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawVF,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawQP44,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw2CornerQP46,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/46NoStepSecond.png" },
            { (CabinDrawNumber.Draw2StraightQP48,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/48NoStepSecond.png" },
            { (CabinDrawNumber.DrawCornerQP6W45,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/45StepSecond.png" },
            { (CabinDrawNumber.DrawStraightQP6W47,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/47StepSecond.png" },
            { (CabinDrawNumber.DrawQB31,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerQB6W32,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/32StepSecond.png" },
            { (CabinDrawNumber.Draw2CornerQB33,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/33NoStepSecond.png" },
            { (CabinDrawNumber.DrawStraightQB6W38,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/38StepSecond.png" },
            { (CabinDrawNumber.Draw2StraightQB41,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/41NoStepSecond.png" },

            { (CabinDrawNumber.None,                    CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9S,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9S9F,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9S9F9F,              CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/SFFStepTertiary.png" },
            { (CabinDrawNumber.Draw94,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw949F,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw949F9F,              CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/4FFStepTertiary.png" },
            { (CabinDrawNumber.Draw9A,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9A9F,                CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/AFFStepTertiary.png" },
            { (CabinDrawNumber.Draw9C,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9C9F,                CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/CFStepTertiary.png" },
            { (CabinDrawNumber.Draw9B,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9B9F,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9B9F9F,              CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/BFFStepTertiary.png" },
            { (CabinDrawNumber.DrawVS,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawVSVF,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawV4,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawV4VF,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawVA,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawWS,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawNP44,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerNP46,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightNP48,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerNP6W45,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightNP6W47,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawNB31,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerNB6W32,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerNB33,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightNB6W38,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightNB41,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawDB51,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerDB8W52,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerDB53,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightDB8W59,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightDB61,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawHB34,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerHB8W35,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerHB37,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightHB8W40,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightHB43,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw8W,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawE,                   CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw8WFlipper81,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2Corner8W82,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw1Corner8W84,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2Straight8W85,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/StepSketches/88StepTertiary.png" },
            { (CabinDrawNumber.Draw8W40,                CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.DrawNV,                  CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.DrawNV2,                 CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.DrawMV2,                 CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.Draw9F,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawVF,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawQP44,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerQP46,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightQP48,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerQP6W45,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightQP6W47,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawQB31,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerQB6W32,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerQB33,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightQB6W38,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightQB41,       CabinSynthesisModel.Tertiary), ""},
        };

        /// <summary>
        /// Maps the CabinDrawNumber Enum Value to the Sketch Image Path
        /// </summary>
        public static readonly Dictionary<(CabinDrawNumber,CabinSynthesisModel), string> CabinDrawNumberSideImagePath = new()
        {
            { (CabinDrawNumber.None,                    CabinSynthesisModel.Primary), ""},
            { (CabinDrawNumber.Draw9S,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SPrimary.png" },
            { (CabinDrawNumber.Draw9S9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFPrimary.png" },
            { (CabinDrawNumber.Draw9S9F9F,              CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFFPrimary.png" },
            { (CabinDrawNumber.Draw94,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4.png" },
            { (CabinDrawNumber.Draw949F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FPrimary.png" },
            { (CabinDrawNumber.Draw949F9F,              CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FFPrimary.png" },
            { (CabinDrawNumber.Draw9A,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/APrimary.png" },
            { (CabinDrawNumber.Draw9A9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/AFPrimary.png" },
            { (CabinDrawNumber.Draw9C,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/CPrimary.png" },
            { (CabinDrawNumber.Draw9C9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/CFPrimary.png" },
            { (CabinDrawNumber.Draw9B,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/BPrimary.png" },
            { (CabinDrawNumber.Draw9B9F,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/BFPrimary.png" },
            { (CabinDrawNumber.Draw9B9F9F,              CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/BFFPrimary.png" },
            { (CabinDrawNumber.DrawVS,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SPrimary.png" },
            { (CabinDrawNumber.DrawVSVF,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFPrimary.png" },
            { (CabinDrawNumber.DrawV4,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4.png" },
            { (CabinDrawNumber.DrawV4VF,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FPrimary.png" },
            { (CabinDrawNumber.DrawVA,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/APrimary.png" },
            { (CabinDrawNumber.DrawWS,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/WSPrimary.png" },
            { (CabinDrawNumber.DrawNP44,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/44Primary.png" },
            { (CabinDrawNumber.Draw2CornerNP46,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/46Primary.png" },
            { (CabinDrawNumber.Draw2StraightNP48,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/48Primary.png" },
            { (CabinDrawNumber.DrawCornerNP6W45,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/45Primary.png" },
            { (CabinDrawNumber.DrawStraightNP6W47,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/47Primary.png" },
            { (CabinDrawNumber.DrawNB31,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/31Primary.png" },
            { (CabinDrawNumber.DrawCornerNB6W32,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/32Primary.png" },
            { (CabinDrawNumber.Draw2CornerNB33,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/33Primary.png" },
            { (CabinDrawNumber.DrawStraightNB6W38,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/38Primary.png" },
            { (CabinDrawNumber.Draw2StraightNB41,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/41Primary.png" },
            { (CabinDrawNumber.DrawDB51,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/51Primary.png" },
            { (CabinDrawNumber.DrawCornerDB8W52,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/52Primary.png" },
            { (CabinDrawNumber.Draw2CornerDB53,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/53Primary.png" },
            { (CabinDrawNumber.DrawStraightDB8W59,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/59Primary.png" },
            { (CabinDrawNumber.Draw2StraightDB61,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/61Primary.png" },
            { (CabinDrawNumber.DrawHB34,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/34Primary.png" },
            { (CabinDrawNumber.DrawCornerHB8W35,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/35Primary.png" },
            { (CabinDrawNumber.Draw2CornerHB37,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/37Primary.png" },
            { (CabinDrawNumber.DrawStraightHB8W40,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/40Primary.png" },
            { (CabinDrawNumber.Draw2StraightHB43,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/43Primary.png" },
            { (CabinDrawNumber.Draw8W,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/WPrimary.png" },
            { (CabinDrawNumber.DrawE,                   CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/EPrimary.png" },
            { (CabinDrawNumber.Draw8WFlipper81,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/81Primary.png" },
            { (CabinDrawNumber.Draw2Corner8W82,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/82Primary.png" },
            { (CabinDrawNumber.Draw1Corner8W84,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/84Primary.png" },
            { (CabinDrawNumber.Draw2Straight8W85,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/85Primary.png" },
            { (CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/88Primary.png" },
            { (CabinDrawNumber.Draw8W40,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/8W40Primary.png" },
            { (CabinDrawNumber.DrawNV,                  CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/NVPrimary.png" },
            { (CabinDrawNumber.DrawNV2,                 CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/NV2Primary.png" },
            { (CabinDrawNumber.DrawMV2,                 CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/MV2Primary.png" },
            { (CabinDrawNumber.Draw9F,                  CabinSynthesisModel.Primary), ""},
            { (CabinDrawNumber.DrawVF,                  CabinSynthesisModel.Primary), ""},
            { (CabinDrawNumber.DrawQP44,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/44Primary.png" },
            { (CabinDrawNumber.Draw2CornerQP46,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/46Primary.png" },
            { (CabinDrawNumber.Draw2StraightQP48,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/48Primary.png" },
            { (CabinDrawNumber.DrawCornerQP6W45,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/45Primary.png" },
            { (CabinDrawNumber.DrawStraightQP6W47,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/47Primary.png" },
            { (CabinDrawNumber.DrawQB31,                CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/31Primary.png" },
            { (CabinDrawNumber.DrawCornerQB6W32,        CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/32Primary.png" },
            { (CabinDrawNumber.Draw2CornerQB33,         CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/33Primary.png" },
            { (CabinDrawNumber.DrawStraightQB6W38,      CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/38Primary.png" },
            { (CabinDrawNumber.Draw2StraightQB41,       CabinSynthesisModel.Primary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/41Primary.png" },

            { (CabinDrawNumber.None,                    CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw9S,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw9S9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFSecondary.png" },
            { (CabinDrawNumber.Draw9S9F9F,              CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFFSecondary.png" },
            { (CabinDrawNumber.Draw94,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw949F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FSecondary.png" },
            { (CabinDrawNumber.Draw949F9F,              CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FFSecondary.png" },
            { (CabinDrawNumber.Draw9A,                  CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/ASecondary.png" },
            { (CabinDrawNumber.Draw9A9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/AFSecondary.png" },
            { (CabinDrawNumber.Draw9C,                  CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/CSecondary.png" },
            { (CabinDrawNumber.Draw9C9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/CFSecondary.png" },
            { (CabinDrawNumber.Draw9B,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw9B9F,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/BFSecondary.png" },
            { (CabinDrawNumber.Draw9B9F9F,              CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/BFFSecondary.png" },
            { (CabinDrawNumber.DrawVS,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawVSVF,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFSecondary.png" },
            { (CabinDrawNumber.DrawV4,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawV4VF,                CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FSecondary.png" },
            { (CabinDrawNumber.DrawVA,                  CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/ASecondary.png" },
            { (CabinDrawNumber.DrawWS,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawNP44,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw2CornerNP46,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/46Secondary.png" },
            { (CabinDrawNumber.Draw2StraightNP48,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/48Secondary.png" },
            { (CabinDrawNumber.DrawCornerNP6W45,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/45Seconary.png" },
            { (CabinDrawNumber.DrawStraightNP6W47,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/47Secondary.png" },
            { (CabinDrawNumber.DrawNB31,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerNB6W32,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/32Secondary.png" },
            { (CabinDrawNumber.Draw2CornerNB33,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/33Secondary.png" },
            { (CabinDrawNumber.DrawStraightNB6W38,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/38Secondary.png" },
            { (CabinDrawNumber.Draw2StraightNB41,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/41Secondary.png" },
            { (CabinDrawNumber.DrawDB51,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerDB8W52,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/52Secondary.png" },
            { (CabinDrawNumber.Draw2CornerDB53,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/53Secondary.png" },
            { (CabinDrawNumber.DrawStraightDB8W59,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/59Secondary.png" },
            { (CabinDrawNumber.Draw2StraightDB61,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/61Secondary.png" },
            { (CabinDrawNumber.DrawHB34,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerHB8W35,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/35Secondary.png" },
            { (CabinDrawNumber.Draw2CornerHB37,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/37Secondary.png" },
            { (CabinDrawNumber.DrawStraightHB8W40,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/40Secondary.png" },
            { (CabinDrawNumber.Draw2StraightHB43,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/43Secondary.png" },
            { (CabinDrawNumber.Draw8W,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawE,                   CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw8WFlipper81,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/81Secondary.png" },
            { (CabinDrawNumber.Draw2Corner8W82,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/82Secondary.png" },
            { (CabinDrawNumber.Draw1Corner8W84,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/84Secondary.png" },
            { (CabinDrawNumber.Draw2Straight8W85,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/85Secondary.png" },
            { (CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/88Secondary.png" },
            { (CabinDrawNumber.Draw8W40,                CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.DrawNV,                  CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.DrawNV2,                 CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.DrawMV2,                 CabinSynthesisModel.Secondary), "" },
            { (CabinDrawNumber.Draw9F,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawVF,                  CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawQP44,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.Draw2CornerQP46,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/46Secondary.png" },
            { (CabinDrawNumber.Draw2StraightQP48,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/48Secondary.png" },
            { (CabinDrawNumber.DrawCornerQP6W45,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/45Seconary.png" },
            { (CabinDrawNumber.DrawStraightQP6W47,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/47Secondary.png" },
            { (CabinDrawNumber.DrawQB31,                CabinSynthesisModel.Secondary), ""},
            { (CabinDrawNumber.DrawCornerQB6W32,        CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/32Secondary.png" },
            { (CabinDrawNumber.Draw2CornerQB33,         CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/33Secondary.png" },
            { (CabinDrawNumber.DrawStraightQB6W38,      CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/38Secondary.png" },
            { (CabinDrawNumber.Draw2StraightQB41,       CabinSynthesisModel.Secondary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/41Secondary.png" },

            { (CabinDrawNumber.None,                    CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9S,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9S9F,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9S9F9F,              CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/SFFTertiary.png" },
            { (CabinDrawNumber.Draw94,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw949F,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw949F9F,              CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/4FFTertiary.png" },
            { (CabinDrawNumber.Draw9A,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9A9F,                CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/AFTertiary.png" },
            { (CabinDrawNumber.Draw9C,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9C9F,                CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/CFTertiary.png" },
            { (CabinDrawNumber.Draw9B,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9B9F,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw9B9F9F,              CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/BFFTertiary.png" },
            { (CabinDrawNumber.DrawVS,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawVSVF,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawV4,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawV4VF,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawVA,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawWS,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawNP44,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerNP46,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightNP48,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerNP6W45,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightNP6W47,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawNB31,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerNB6W32,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerNB33,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightNB6W38,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightNB41,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawDB51,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerDB8W52,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerDB53,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightDB8W59,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightDB61,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawHB34,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerHB8W35,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerHB37,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightHB8W40,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightHB43,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw8W,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawE,                   CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw8WFlipper81,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2Corner8W82,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw1Corner8W84,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2Straight8W85,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerStraight8W88, CabinSynthesisModel.Tertiary), "/Images/CabinImages/AllCabinDrawSketches/SideSketches/88Tertiary.png" },
            { (CabinDrawNumber.Draw8W40,                CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.DrawNV,                  CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.DrawNV2,                 CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.DrawMV2,                 CabinSynthesisModel.Tertiary), "" },
            { (CabinDrawNumber.Draw9F,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawVF,                  CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawQP44,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerQP46,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightQP48,       CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerQP6W45,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightQP6W47,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawQB31,                CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawCornerQB6W32,        CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2CornerQB33,         CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.DrawStraightQB6W38,      CabinSynthesisModel.Tertiary), ""},
            { (CabinDrawNumber.Draw2StraightQB41,       CabinSynthesisModel.Tertiary), ""},

        };

        /// <summary>
        /// Maps the Cabin Finish Enum Value to the Description Key
        /// </summary>
        public static readonly Dictionary<CabinFinishEnum, string> CabinFinishEnumDescKey = new()
        {
            { CabinFinishEnum.Polished,      "Polished"     },
            { CabinFinishEnum.Brushed,       "Brushed"      },
            { CabinFinishEnum.BlackMat,      "BlackMat"     },
            { CabinFinishEnum.WhiteMat,      "WhiteMat"     },
            { CabinFinishEnum.Bronze,        "Bronze"       },
            { CabinFinishEnum.BrushedGold,   "BrushedGold"  },
            { CabinFinishEnum.Gold,          "Gold"         },
            { CabinFinishEnum.Copper,        "Copper"       },
            { CabinFinishEnum.Special,       "SpecialColor" },
            { CabinFinishEnum.NotSet,        "NotSet"       }
        };

        /// <summary>
        /// Maps the Cabin Finish Enum Value to the Description Key
        /// </summary>
        public static readonly Dictionary<CabinFinishEnum, string> CabinFinishEnumImagePath = new()
        {
            { CabinFinishEnum.Polished,      "../Images/CabinImages/MetalFinishes/ChromeFinish.png" },
            { CabinFinishEnum.Brushed,       "../Images/Finishes/BrushedNickelAnodized.png"         },
            { CabinFinishEnum.BlackMat,      "../Images/Finishes/BlackMatAnodized.png"              },
            { CabinFinishEnum.WhiteMat,      "../Images/CabinImages/MetalFinishes/WhiteMatFinish.png"  },
            { CabinFinishEnum.Bronze,        "../Images/Finishes/BronzeMatAnodized.png"             },
            { CabinFinishEnum.BrushedGold,   "../Images/Finishes/GoldMatAnodized.png"               },
            { CabinFinishEnum.Gold,          "../Images/Finishes/SimilarGoldElectroplated.png"      },
            { CabinFinishEnum.Copper,        "../Images/Finishes/CopperMatAnodized.png"             },
            { CabinFinishEnum.Special,       "../Images/Finishes/"                                  },
            { CabinFinishEnum.NotSet,        "../Images/Finishes/"                                  }
        };

        /// <summary>
        /// Maps the Cabin Finish Enum Value to the Description Key
        /// </summary>
        public static readonly Dictionary<CabinThicknessEnum, string> CabinThicknessesEnumDescKey = new()
        {
            { CabinThicknessEnum.NotSet ,           "ThickNotSet" },
            { CabinThicknessEnum.Thick5mm ,         "Thick5mm" },
            { CabinThicknessEnum.Thick6mm,          "Thick6mm" },
            { CabinThicknessEnum.Thick8mm,          "Thick8mm" },
            { CabinThicknessEnum.Thick10mm,         "Thick10mm" },
            { CabinThicknessEnum.ThickTenplex10mm,  "ThickTenplex10mm" },
            { CabinThicknessEnum.Thick6mm8mm,       "Thick6mm8mm" },
            { CabinThicknessEnum.Thick8mm10mm,      "Thick8mm10mm" },

        };

        /// <summary>
        /// Maps the Cabin Finish Enum Value to the Description Key
        /// </summary>
        public static readonly Dictionary<GlassFinishEnum, string> GlassFinishEnumDescKey = new()
        {
            { GlassFinishEnum.GlassFinishNotSet,"GlassFinishNotSet" },
            { GlassFinishEnum.Transparent,      "Transparent" },
            { GlassFinishEnum.Satin,            "Satin" },
            { GlassFinishEnum.Serigraphy,       "Serigraphy" },
            { GlassFinishEnum.Fume,             "Fume" },
            { GlassFinishEnum.Frosted,          "Frosted" },
            { GlassFinishEnum.Special,          "SpecialFinish" },
        };

        /// <summary>
        /// Maps the GlassFinishEnum Value to the Glass Finish Image Path
        /// </summary>
        public static readonly Dictionary<GlassFinishEnum, string> GlassFinishImagePath = new()
        {
            { GlassFinishEnum.GlassFinishNotSet,"../Images/CabinImages/GlassImages/Transparent.png" },
            { GlassFinishEnum.Transparent,      "../Images/CabinImages/GlassImages/Transparent.png" },
            { GlassFinishEnum.Satin,            "../Images/CabinImages/GlassImages/Satin.png" },
            { GlassFinishEnum.Serigraphy,       "../Images/CabinImages/GlassImages/Serigraphy.png" },
            { GlassFinishEnum.Fume,             "../Images/CabinImages/GlassImages/Fume.png" },
            { GlassFinishEnum.Frosted,          "../Images/CabinImages/GlassImages/Frosted.png" },
            { GlassFinishEnum.Special,          "../Images/CabinImages/GlassImages/Windowed.png" },
        };

        /// <summary>
        /// Maps the CabinExtraType Value to the Extra Image Path
        /// </summary>
        public static readonly Dictionary<CabinExtraType, string> CabinExtraImagePath = new()
        {
            { CabinExtraType.StepCut,     "../Images/CabinImages/CabinExtras/StepCut.png" },
            { CabinExtraType.SafeKids,    "../Images/CabinImages/CabinExtras/SafeKids.png" },
            { CabinExtraType.BronzeClean, "../Images/CabinImages/CabinExtras/BronzeClean.png" }
        };

        /// <summary>
        /// Maps the Cabin ExtraType Value to the Description Key
        /// </summary>
        public static readonly Dictionary<CabinExtraType, string> CabinExtraDescKey = new()
        {
            { CabinExtraType.StepCut,     "StepCut" },
            { CabinExtraType.SafeKids,    "SafeKids" },
            { CabinExtraType.BronzeClean, "BronzeClean" }
        };

        /// <summary>
        /// Maps the Cabin ExtraType Value to the Description Key
        /// </summary>
        public static readonly Dictionary<CabinExtraType, string> CabinExtraFullDescKey = new()
        {
            { CabinExtraType.StepCut,     "StepCutFullDesc" },
            { CabinExtraType.SafeKids,    "SafeKidsFullDesc" },
            { CabinExtraType.BronzeClean, "BronzeCleanFullDesc" }
        };
        #endregion

        #region 3.Various Description Keys

        public static readonly Dictionary<CabinModelEnum, string> PrimaryCabinLengthDescKey = new()
        {
            { CabinModelEnum.Model9A,             "9ALength1" },
            { CabinModelEnum.Model9S,             "9SLength1" },
            { CabinModelEnum.Model94,             "94Length1" },
            { CabinModelEnum.Model9F,             "9FLength1" },
            { CabinModelEnum.Model9B,             "9BLength1" },
            { CabinModelEnum.ModelW,              "WLength1" },
            { CabinModelEnum.ModelHB,             "HBLength1" },
            { CabinModelEnum.ModelNP,             "NPLength1" },
            { CabinModelEnum.ModelVS,             "VSLength1" },
            { CabinModelEnum.ModelVF,             "VFLength1" },
            { CabinModelEnum.ModelV4,             "V4Length1" },
            { CabinModelEnum.ModelVA,             "VALength1" },
            { CabinModelEnum.ModelWS,             "WSLength1" },
            { CabinModelEnum.ModelE,              "ELength1" },
            { CabinModelEnum.ModelWFlipper,       "WFlipperLength1" },
            { CabinModelEnum.ModelDB,             "DBLength1" },
            { CabinModelEnum.ModelNB,             "NBLength1" },
            { CabinModelEnum.ModelNV,             "NVLength1" },
            { CabinModelEnum.ModelMV2,            "MVLength1" },
            { CabinModelEnum.ModelNV2,            "NV2Length1" },
            { CabinModelEnum.Model6WA,            "6WALength1" },
            { CabinModelEnum.Model9C,             "9CLength1" },
            { CabinModelEnum.Model8W40,           "8W40Length1"},
            { CabinModelEnum.ModelGlassContainer, "UndefinedGlassContainerSide" },
            { CabinModelEnum.ModelQB,             "NBLength1" },
            { CabinModelEnum.ModelQP,             "NPLength1" },
        };
        public static readonly Dictionary<CabinModelEnum, string> SecondaryCabinLengthDescKey = new()
        {
            { CabinModelEnum.Model9A,             "9ALength2" },
            { CabinModelEnum.Model9S,             "9SLength2" },
            { CabinModelEnum.Model94,             "94Length2" },
            { CabinModelEnum.Model9F,             "9FLength2" },
            { CabinModelEnum.Model9B,             "9BLength2" },
            { CabinModelEnum.ModelW,              "WLength2" },
            { CabinModelEnum.ModelHB,             "HBLength2" },
            { CabinModelEnum.ModelNP,             "NPLength2" },
            { CabinModelEnum.ModelVS,             "VSLength2" },
            { CabinModelEnum.ModelVF,             "VFLength2" },
            { CabinModelEnum.ModelV4,             "V4Length2" },
            { CabinModelEnum.ModelVA,             "VALength2" },
            { CabinModelEnum.ModelWS,             "WSLength2" },
            { CabinModelEnum.ModelE,              "ELength2" },
            { CabinModelEnum.ModelWFlipper,       "WFlipperLength2" },
            { CabinModelEnum.ModelDB,             "DBLength2" },
            { CabinModelEnum.ModelNB,             "NBLength2" },
            { CabinModelEnum.ModelNV,             "NVLength2" },
            { CabinModelEnum.ModelMV2,            "MVLength2" },
            { CabinModelEnum.ModelNV2,            "NV2Length2" },
            { CabinModelEnum.Model6WA,            "6WALength2" },
            { CabinModelEnum.Model9C,             "9CLength2" },
            { CabinModelEnum.Model8W40,           "8W40Length2"},
            { CabinModelEnum.ModelGlassContainer, "UndefinedGlassContainerSide" },
            { CabinModelEnum.ModelQB,             "NBLength2" },
            { CabinModelEnum.ModelQP,             "NPLength2" },
        };
        public static readonly Dictionary<CabinModelEnum, string> TertiaryCabinLengthDescKey = new()
        {
            { CabinModelEnum.Model9A,             "9ALength3" },
            { CabinModelEnum.Model9S,             "9SLength3" },
            { CabinModelEnum.Model94,             "94Length3" },
            { CabinModelEnum.Model9F,             "9FLength3" },
            { CabinModelEnum.Model9B,             "9BLength3" },
            { CabinModelEnum.ModelW,              "WLength3" },
            { CabinModelEnum.ModelHB,             "HBLength3" },
            { CabinModelEnum.ModelNP,             "NPLength3" },
            { CabinModelEnum.ModelVS,             "VSLength3" },
            { CabinModelEnum.ModelVF,             "VFLength3" },
            { CabinModelEnum.ModelV4,             "V4Length3" },
            { CabinModelEnum.ModelVA,             "VALength3" },
            { CabinModelEnum.ModelWS,             "WSLength3" },
            { CabinModelEnum.ModelE,              "ELength3" },
            { CabinModelEnum.ModelWFlipper,       "WFlipperLength3" },
            { CabinModelEnum.ModelDB,             "DBLength3" },
            { CabinModelEnum.ModelNB,             "NBLength3" },
            { CabinModelEnum.ModelNV,             "NVLength3" },
            { CabinModelEnum.ModelMV2,            "MVLength3" },
            { CabinModelEnum.ModelNV2,            "NV2Length3" },
            { CabinModelEnum.Model6WA,            "6WALength3" },
            { CabinModelEnum.Model9C,             "9CLength3" },
            { CabinModelEnum.Model8W40,           "8W40Length3"},
            { CabinModelEnum.ModelGlassContainer, "UndefinedGlassContainerSide" },
            { CabinModelEnum.ModelQB,             "NBLength3" },
            { CabinModelEnum.ModelQP,             "NPLength3" },
        };
        public static readonly Dictionary<CabinDirection, string> CabinDirectionDescKey = new()
        {
            { CabinDirection.Undefined , "CabinDirectionUndefined"  },
            { CabinDirection.LeftSided , "CabinDirectionLeftSided"  },
            { CabinDirection.RightSided, "CabinDirectionRightSided" }
        };
        /// <summary>
        /// Retrieves the Key for the Descriptions of the CabinPartType
        /// </summary>
        /// <param name="partType"></param>
        /// <returns></returns>
        public static string GetCabinPartTypeDescKey(CabinPartType partType)
        {
            return CabinPartTypeDescKey.TryGetValue(partType, out string key) ? key : $"N/A Key:({partType})";
        }
        public static readonly Dictionary<CabinPartType, string> CabinPartTypeDescKey = new()
        {
            { CabinPartType.Undefined ,         "Undefined"             },
            { CabinPartType.GenericPart,        "GenericPart"           },
            { CabinPartType.Handle ,            "Handle"                },
            { CabinPartType.Hinge ,             "Hinge"                 },
            { CabinPartType.Profile ,           "Profile"               },
            { CabinPartType.MagnetProfile ,     "MagnetProfile"         },
            { CabinPartType.ProfileHinge ,      "HingeProfile"          },
            { CabinPartType.FloorStopperW ,     "FloorStopper"          },
            { CabinPartType.BarSupport ,        "BarSupport"            },
            { CabinPartType.SmallSupport ,      "SmallSupport"          },
            { CabinPartType.Strip ,             "Strip"                 },
            { CabinPartType.AnglePart ,         "AnglePart"             },
        };
        public static readonly Dictionary<CabinHandleType, string> CabinHandleTypeDescKey = new()
        {
            { CabinHandleType.SingleHandle ,      "SingleHandle"          },
            { CabinHandleType.DoubleHandle ,      "DoubleHandle"          },
            { CabinHandleType.SingleKnob ,        "SingleKnob"            },
            { CabinHandleType.DoubleKnob ,        "DoubleKnob"            },
            { CabinHandleType.HandleKnob ,        "HandleKnob"            },
        };
        public static readonly Dictionary<CabinStripType, string> CabinStripTypeDescKey = new()
        {
            { CabinStripType.PolycarbonicMagnet ,"PolycarbonicMagnet"    },
            { CabinStripType.PolycarbonicBumper ,"PolycarbonicBumper"    },
        };
        public static readonly Dictionary<MaterialType, string> MaterialTypeDescKey = new()
        {
            { MaterialType.Undefined ,              "Undefined"},
            { MaterialType.Aluminium ,              "MaterialAluminium"},
            { MaterialType.Inox304 ,                "MaterialInox304"},
            { MaterialType.Inox316 ,                "MaterialInox316"},
            { MaterialType.Inox430 ,                "MaterialInox430"},
            { MaterialType.Brass ,                  "MaterialBrass"},
            { MaterialType.Zink ,                   "MaterialZink"},
            { MaterialType.Abs ,                    "MaterialAbs"},
            { MaterialType.Plastic ,                "MaterialPlastic"},
            { MaterialType.Metallic ,               "MaterialMetallic"},
            { MaterialType.Polycarbonic ,           "MaterialPolycarbonic"},
            { MaterialType.Other ,                  "Other"},
        };

        #endregion

        #region 4.Cabin Sketched Directions

        /// <summary>
        /// The Default Direction of the Primary Model as Depicted in the Cabin Sketches
        /// </summary>
        public static readonly Dictionary<CabinDrawNumber , CabinDirection> DefaultPrimaryCabinDirection = new()
        {
            { CabinDrawNumber.None,                     CabinDirection.Undefined},
            { CabinDrawNumber.Draw9S,                   CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9S9F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9S9F9F,               CabinDirection.RightSided},
            { CabinDrawNumber.Draw94,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw949F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw949F9F,               CabinDirection.Undefined},
            { CabinDrawNumber.Draw9A,                   CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9A9F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9C,                   CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9C9F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9B,                   CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9B9F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9B9F9F,               CabinDirection.LeftSided},
            { CabinDrawNumber.DrawVS,                   CabinDirection.LeftSided},
            { CabinDrawNumber.DrawVSVF,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawV4,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawV4VF,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawVA,                   CabinDirection.LeftSided},
            { CabinDrawNumber.DrawWS,                   CabinDirection.RightSided},
            { CabinDrawNumber.DrawNP44,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerNP46,          CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2StraightNP48,        CabinDirection.LeftSided},
            { CabinDrawNumber.DrawCornerNP6W45,         CabinDirection.LeftSided},
            { CabinDrawNumber.DrawStraightNP6W47,       CabinDirection.LeftSided},
            { CabinDrawNumber.DrawNB31,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawCornerNB6W32,         CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerNB33,          CabinDirection.LeftSided},
            { CabinDrawNumber.DrawStraightNB6W38,       CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2StraightNB41,        CabinDirection.LeftSided},
            { CabinDrawNumber.DrawDB51,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawCornerDB8W52,         CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerDB53,          CabinDirection.LeftSided},
            { CabinDrawNumber.DrawStraightDB8W59,       CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2StraightDB61,        CabinDirection.LeftSided},
            { CabinDrawNumber.DrawHB34,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawCornerHB8W35,         CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerHB37,          CabinDirection.LeftSided},
            { CabinDrawNumber.DrawStraightHB8W40,       CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2StraightHB43,        CabinDirection.LeftSided},
            { CabinDrawNumber.Draw8W,                   CabinDirection.RightSided},
            { CabinDrawNumber.DrawE,                    CabinDirection.Undefined},
            { CabinDrawNumber.Draw8WFlipper81,          CabinDirection.RightSided},
            { CabinDrawNumber.Draw2Corner8W82,          CabinDirection.RightSided},
            { CabinDrawNumber.Draw1Corner8W84,          CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2Straight8W85,        CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerStraight8W88,  CabinDirection.LeftSided},
            { CabinDrawNumber.Draw8W40,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawNV,                   CabinDirection.LeftSided},
            { CabinDrawNumber.DrawNV2,                  CabinDirection.LeftSided},
            { CabinDrawNumber.DrawMV2,                  CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9F,                   CabinDirection.LeftSided},
            { CabinDrawNumber.DrawVF,                   CabinDirection.LeftSided},
            { CabinDrawNumber.DrawQP44,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerQP46,          CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2StraightQP48,        CabinDirection.LeftSided},
            { CabinDrawNumber.DrawCornerQP6W45,         CabinDirection.LeftSided},
            { CabinDrawNumber.DrawStraightQP6W47,       CabinDirection.LeftSided},
            { CabinDrawNumber.DrawQB31,                 CabinDirection.LeftSided},
            { CabinDrawNumber.DrawCornerQB6W32,         CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2CornerQB33,          CabinDirection.LeftSided},
            { CabinDrawNumber.DrawStraightQB6W38,       CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2StraightQB41,        CabinDirection.LeftSided},
        };

        /// <summary>
        /// The Default Direction of the Secondary Model as Depicted in the Cabin Sketches
        /// </summary>
        public static readonly Dictionary<CabinDrawNumber , CabinDirection> DefaultSecondaryCabinDirection = new()
        {
            { CabinDrawNumber.None,                     CabinDirection.Undefined},
            { CabinDrawNumber.Draw9S,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw9S9F,                 CabinDirection.RightSided},
            { CabinDrawNumber.Draw9S9F9F,               CabinDirection.RightSided},
            { CabinDrawNumber.Draw94,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw949F,                 CabinDirection.RightSided},
            { CabinDrawNumber.Draw949F9F,               CabinDirection.RightSided},
            { CabinDrawNumber.Draw9A,                   CabinDirection.RightSided},
            { CabinDrawNumber.Draw9A9F,                 CabinDirection.RightSided},
            { CabinDrawNumber.Draw9C,                   CabinDirection.RightSided},
            { CabinDrawNumber.Draw9C9F,                 CabinDirection.RightSided},
            { CabinDrawNumber.Draw9B,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw9B9F,                 CabinDirection.RightSided},
            { CabinDrawNumber.Draw9B9F9F,               CabinDirection.RightSided},
            { CabinDrawNumber.DrawVS,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawVSVF,                 CabinDirection.RightSided},
            { CabinDrawNumber.DrawV4,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawV4VF,                 CabinDirection.RightSided},
            { CabinDrawNumber.DrawVA,                   CabinDirection.RightSided},
            { CabinDrawNumber.DrawWS,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawNP44,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerNP46,          CabinDirection.RightSided},
            { CabinDrawNumber.Draw2StraightNP48,        CabinDirection.RightSided},
            { CabinDrawNumber.DrawCornerNP6W45,         CabinDirection.RightSided},
            { CabinDrawNumber.DrawStraightNP6W47,       CabinDirection.RightSided},
            { CabinDrawNumber.DrawNB31,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerNB6W32,         CabinDirection.RightSided},
            { CabinDrawNumber.Draw2CornerNB33,          CabinDirection.RightSided},
            { CabinDrawNumber.DrawStraightNB6W38,       CabinDirection.RightSided},
            { CabinDrawNumber.Draw2StraightNB41,        CabinDirection.RightSided},
            { CabinDrawNumber.DrawDB51,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerDB8W52,         CabinDirection.RightSided},
            { CabinDrawNumber.Draw2CornerDB53,          CabinDirection.RightSided},
            { CabinDrawNumber.DrawStraightDB8W59,       CabinDirection.RightSided},
            { CabinDrawNumber.Draw2StraightDB61,        CabinDirection.RightSided},
            { CabinDrawNumber.DrawHB34,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerHB8W35,         CabinDirection.RightSided},
            { CabinDrawNumber.Draw2CornerHB37,          CabinDirection.RightSided},
            { CabinDrawNumber.DrawStraightHB8W40,       CabinDirection.RightSided},
            { CabinDrawNumber.Draw2StraightHB43,        CabinDirection.RightSided},
            { CabinDrawNumber.Draw8W,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawE,                    CabinDirection.Undefined},
            { CabinDrawNumber.Draw8WFlipper81,          CabinDirection.RightSided},
            { CabinDrawNumber.Draw2Corner8W82,          CabinDirection.LeftSided},
            { CabinDrawNumber.Draw1Corner8W84,          CabinDirection.LeftSided},
            { CabinDrawNumber.Draw2Straight8W85,        CabinDirection.RightSided},
            { CabinDrawNumber.Draw2CornerStraight8W88,  CabinDirection.LeftSided},
            { CabinDrawNumber.Draw8W40,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawNV,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawNV2,                  CabinDirection.Undefined},
            { CabinDrawNumber.DrawMV2,                  CabinDirection.Undefined},
            { CabinDrawNumber.Draw9F,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawVF,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawQP44,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerQP46,          CabinDirection.RightSided},
            { CabinDrawNumber.Draw2StraightQP48,        CabinDirection.RightSided},
            { CabinDrawNumber.DrawCornerQP6W45,         CabinDirection.RightSided},
            { CabinDrawNumber.DrawStraightQP6W47,       CabinDirection.RightSided},
            { CabinDrawNumber.DrawQB31,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerQB6W32,         CabinDirection.RightSided},
            { CabinDrawNumber.Draw2CornerQB33,          CabinDirection.RightSided},
            { CabinDrawNumber.DrawStraightQB6W38,       CabinDirection.RightSided},
            { CabinDrawNumber.Draw2StraightQB41,        CabinDirection.RightSided},
        };

        /// <summary>
        /// The Default Direction of the Tertiary Model as Depicted in the Cabin Sketches
        /// </summary>
        public static readonly Dictionary<CabinDrawNumber , CabinDirection> DefaultTertiaryCabinDirection = new()
        {
            { CabinDrawNumber.None,                     CabinDirection.Undefined},
            { CabinDrawNumber.Draw9S,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw9S9F,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw9S9F9F,               CabinDirection.LeftSided},
            { CabinDrawNumber.Draw94,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw949F,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw949F9F,               CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9A,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw9A9F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9C,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw9C9F,                 CabinDirection.LeftSided},
            { CabinDrawNumber.Draw9B,                   CabinDirection.Undefined},
            { CabinDrawNumber.Draw9B9F,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw9B9F9F,               CabinDirection.LeftSided},
            { CabinDrawNumber.DrawVS,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawVSVF,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawV4,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawV4VF,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawVA,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawWS,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawNP44,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerNP46,          CabinDirection.Undefined},
            { CabinDrawNumber.Draw2StraightNP48,        CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerNP6W45,         CabinDirection.Undefined},
            { CabinDrawNumber.DrawStraightNP6W47,       CabinDirection.Undefined},
            { CabinDrawNumber.DrawNB31,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerNB6W32,         CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerNB33,          CabinDirection.Undefined},
            { CabinDrawNumber.DrawStraightNB6W38,       CabinDirection.Undefined},
            { CabinDrawNumber.Draw2StraightNB41,        CabinDirection.Undefined},
            { CabinDrawNumber.DrawDB51,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerDB8W52,         CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerDB53,          CabinDirection.Undefined},
            { CabinDrawNumber.DrawStraightDB8W59,       CabinDirection.Undefined},
            { CabinDrawNumber.Draw2StraightDB61,        CabinDirection.Undefined},
            { CabinDrawNumber.DrawHB34,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerHB8W35,         CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerHB37,          CabinDirection.Undefined},
            { CabinDrawNumber.DrawStraightHB8W40,       CabinDirection.Undefined},
            { CabinDrawNumber.Draw2StraightHB43,        CabinDirection.Undefined},
            { CabinDrawNumber.Draw8W,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawE,                    CabinDirection.Undefined},
            { CabinDrawNumber.Draw8WFlipper81,          CabinDirection.Undefined},
            { CabinDrawNumber.Draw2Corner8W82,          CabinDirection.Undefined},
            { CabinDrawNumber.Draw1Corner8W84,          CabinDirection.Undefined},
            { CabinDrawNumber.Draw2Straight8W85,        CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerStraight8W88,  CabinDirection.RightSided},
            { CabinDrawNumber.Draw8W40,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawNV,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawNV2,                  CabinDirection.Undefined},
            { CabinDrawNumber.DrawMV2,                  CabinDirection.Undefined},
            { CabinDrawNumber.Draw9F,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawVF,                   CabinDirection.Undefined},
            { CabinDrawNumber.DrawQP44,                 CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerQP46,          CabinDirection.Undefined},
            { CabinDrawNumber.Draw2StraightQP48,        CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerQP6W45,         CabinDirection.Undefined},
            { CabinDrawNumber.DrawStraightQP6W47,       CabinDirection.Undefined},
            { CabinDrawNumber.DrawQB31,                 CabinDirection.Undefined},
            { CabinDrawNumber.DrawCornerQB6W32,         CabinDirection.Undefined},
            { CabinDrawNumber.Draw2CornerQB33,          CabinDirection.Undefined},
            { CabinDrawNumber.DrawStraightQB6W38,       CabinDirection.Undefined},
            { CabinDrawNumber.Draw2StraightQB41,        CabinDirection.Undefined},
        };

        #endregion
    }
}


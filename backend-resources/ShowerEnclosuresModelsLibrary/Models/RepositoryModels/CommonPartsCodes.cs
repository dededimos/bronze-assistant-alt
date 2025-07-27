using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations;

namespace ShowerEnclosuresModelsLibrary.Models.RepositoryModels
{
    public class CommonPartsCodes
    {
        public CommonProfileCodes Profiles { get; init; }
        public CommonAngleCodes Angles { get; init; }
        public CommonHandleCodes Handles { get; init; }
        public CommonHingeCodes Hinges { get; init; }
        public CommonStripCodes Strips { get; init; }
        public CommonSupportBarCodes SupportBars { get; init; }
        public CommonSupportCodes Supports { get; init; }

    }

    public class CommonProfileCodes
    {
        public string Wall9S { get; init; }
        public string Magnet9S { get; init; }
        public string Magnet9B { get; init; }
        public string WallSmartWS { get; init; }
        public string WallW { get; init; }
        public string Connector9FNoTollerance { get; init; }
        public string Connector9FWithTollerance { get; init; }
        public string HorizontalL0TypeA { get; init; }
        public string HorizontalL0TypeB { get; init; }
        public string HorizontalL0TypeQ { get; init; }
        public string HingeProfileNB { get; init; }
        public string HingeProfileQB { get; init; }
        public string MiddleHingeProfileNB { get; init; }
        public string MagnetProfileUsual { get; init; }
        public string FloorProfileLid { get; init; }
        public string HorizontalBarInox304 { get; init; }
    }

    public class CommonStripCodes
    {
        public string MagnetStripStraight { get; init; }
        public string MagnetStrip45Degrees { get; init; }
        public string MagnetStrip9B { get; init; }
        public string BumperStrip { get; init; }
    }

    public class CommonHandleCodes
    {
        public string KnobHandle { get; init; }
        public string Inox304Handle { get; init; }
        public string B6000Handle { get; init; }
    }

    public class CommonAngleCodes
    {
        public string AngleA { get; init; }
        public string AngleB { get; init; }
        public string AngleQ { get; init; }
        public string AngleVA { get; init; }
    }

    public class CommonHingeCodes
    {
        public string Metal9BHinge { get; init; }
        public string Abs9BHinge { get; init; }
        public string HingeDB { get; init; }
        public string HingeHB { get; init; }
        public string HingeFlipper { get; init; }
        public string HingeMiddleNP { get; init; }
        public string HingeMiddleMV { get; init; }
    }

    public class CommonSupportBarCodes
    {
        public string SupportBarDefault { get; init; }
        public string SupportBarSmart { get; init; }
    }

    public class CommonSupportCodes
    {
        public string FloorStopper { get; init; }
        public string SmallWallSupport { get; init; }
        public string Inox304DriverBottom { get; init; }
    }

}

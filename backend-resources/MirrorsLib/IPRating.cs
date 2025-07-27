using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib
{
    public class IPRating : IDeepClonable<IPRating> , IEqualityComparerCreator<IPRating>
    {
        public const string SolidsX = "No data available to specify a solids protection rating";
        public const string Solids0 = "No protection against contact and ingress of objects";
        public const string Solids1 = "Effective against any large surface of the body , such as the back of a hand , but no protection against deliberate contact with a body part >50mm/2.0in";
        public const string Solids2 = "Effective against fingers or similar objects >12.5mm/0.49in";
        public const string Solids3 = "Effective against tools thick wires , e.t.c. >2.5mm / 0.098in";
        public const string Solids4 = "Effective against most wires,slender screws , large ants e.t.c. >1mm/0.039in";
        public const string Solids5 = "Effective against ingress of dust is not entirely prevented , but it must not enter in sufficient quantity to interfere with the satisfactory operation of the equipment.Dust protected";
        public const string Solids6 = "Effective against no ingress of dust;complete protection against contact(dust-tight).A vacuum must be applied.Test duration of up to 8hours based on airflow.Dust-tight";
        public const string WaterX = "No data available to specify water protection rating";
        public const string Water0 = "No protection against ingress of water";
        public const string Water1 = "Protection against dripping water.Tested for 10 minutes";
        public const string Water2 = "Protection against dripping water when tilted at 15deg. Tested for 10minutes.Water equivalent to 3mm(0.12in) rainfall per minute";
        public const string Water3 = "Protection from water falling as a spray at any angle up to 60deg from the vertical shall have no harmful effect";
        public const string Water4 = "Protection from water splashing against the enclosure from any direction shall have no harmful effect";
        public const string Water5 = "Protection from water projected by a nozzle (6.3mm - 0.25in) against enclosure from any direction shall have no harmful effects";
        public const string Water6 = "Protection from water projected in powerful jets (12.5mm - 0.49in ) against the enclosure from any direction, shall have no harmful effects";
        public const string Water6K = "Protection from water projected in powerful jets (6.3mm - 0.25in nozzle) against the enclosure from any direction, under elevated pressure shall have no harmful effects";
        public const string Water7 = "Protection from ingress of water in harmful quantity not be possible when enclosure is immersed in water under defined conditions of pressure and time (up to 1 meter - 3ft 3in of submersion)";
        public const string Water8 = "The Equipment is suitable for continuous immersion in water under conditions that shall be specified by the manufacturer";
        public const string Water9K = "Protected against close-range high pressure,high temprature spray downs";
        public const string AdditionalLetterD = "Wire";
        public const string AdditionalLetterF = "OilResistant";
        public const string AdditionalLetterH = "High Voltage Apparatus";
        public const string AdditionalLetterM = "Device is in motion";
        public const string AdditionalLetterS = "Device is standing still";
        public const string AdditionalLetterW = "Certain Weather Conditions";
        public const string AdditionalLetterEmpty = "-";

        public IPSolidsRating SolidsRating { get; set; }
        public IPWaterRating WaterRating { get; set; }
        public IPAdditionalLetter AdditionalLetter { get; set; }
        public string Rating { get => GetIPRating(); }

        public string GetIPRating()
        {
            return $"IP{GetSolidsShortDescription()}{GetWaterShortDescription()}{GetAdditionalLetterShortDescription()}";
        }
        public string GetSolidsShortDescription()
        {
            return SolidsRating switch
            {
                IPSolidsRating.X => "X",
                IPSolidsRating._0 => "0",
                IPSolidsRating._1 => "1",
                IPSolidsRating._2 => "2",
                IPSolidsRating._3 => "3",
                IPSolidsRating._4 => "4",
                IPSolidsRating._5 => "5",
                IPSolidsRating._6 => "6",
                _ => "X",
            };
        }
        public string GetSolidsDescription()
        {
            return GetSolidsDescription(SolidsRating);
        }
        public static string GetSolidsDescription(IPSolidsRating solidsRating)
        {
            return solidsRating switch
            {
                IPSolidsRating.X => SolidsX,
                IPSolidsRating._0 => Solids0,
                IPSolidsRating._1 => Solids1,
                IPSolidsRating._2 => Solids2,
                IPSolidsRating._3 => Solids3,
                IPSolidsRating._4 => Solids4,
                IPSolidsRating._5 => Solids5,
                IPSolidsRating._6 => Solids6,
                _ => SolidsX,
            };
        }

        public string GetWaterShortDescription()
        {
            return WaterRating switch
            {
                IPWaterRating.X => "X",
                IPWaterRating._0 => "0",
                IPWaterRating._1 => "1",
                IPWaterRating._2 => "2",
                IPWaterRating._3 => "3",
                IPWaterRating._4 => "4",
                IPWaterRating._5 => "5",
                IPWaterRating._6 => "6",
                IPWaterRating._6K => "6K",
                IPWaterRating._7 => "7",
                IPWaterRating._8 => "8",
                IPWaterRating._9K => "9K",
                _ => "X",
            };
        }
        public string GetWaterDescription()
        {
            return GetWaterDescription(WaterRating);
        }
        public static string GetWaterDescription(IPWaterRating waterRating)
        {
            return waterRating switch
            {
                IPWaterRating.X => WaterX,
                IPWaterRating._0 => Water0,
                IPWaterRating._1 => Water1,
                IPWaterRating._2 => Water2,
                IPWaterRating._3 => Water3,
                IPWaterRating._4 => Water4,
                IPWaterRating._5 => Water5,
                IPWaterRating._6 => Water6,
                IPWaterRating._6K => Water6K,
                IPWaterRating._7 => Water7,
                IPWaterRating._8 => Water8,
                IPWaterRating._9K => Water9K,
                _ => WaterX,
            };
        }

        public string GetAdditionalLetterShortDescription()
        {
            return AdditionalLetter switch
            {
                IPAdditionalLetter.Empty => string.Empty,
                IPAdditionalLetter.D => "D",
                IPAdditionalLetter.F => "F",
                IPAdditionalLetter.H => "H",
                IPAdditionalLetter.M => "M",
                IPAdditionalLetter.S => "S",
                IPAdditionalLetter.W => "W",
                _ => AdditionalLetterEmpty,
            };
        }
        public string GetAdditionalLetterDescription()
        {
            return GetAdditionalLetterDescription(AdditionalLetter);
        }
        public static string GetAdditionalLetterDescription(IPAdditionalLetter additionalLetter)
        {
            return additionalLetter switch
            {
                IPAdditionalLetter.Empty => string.Empty,
                IPAdditionalLetter.D => AdditionalLetterD,
                IPAdditionalLetter.F => AdditionalLetterF,
                IPAdditionalLetter.H => AdditionalLetterH,
                IPAdditionalLetter.M => AdditionalLetterM,
                IPAdditionalLetter.S => AdditionalLetterS,
                IPAdditionalLetter.W => AdditionalLetterW,
                _ => string.Empty,
            };
        }
        public string GetIPDescription()
        {
            var solids = GetSolidsDescription();
            var water = GetWaterDescription();
            var additional = GetAdditionalLetterDescription();

            return $"Solids :{solids}{Environment.NewLine}Water :{water}{Environment.NewLine}Additional info :{additional}";
        }

        public IPRating GetDeepClone()
        {
            return (IPRating)this.MemberwiseClone();
        }

        public static IEqualityComparer<IPRating> GetComparer()
        {
            return new IPRatingEqualityComparer();
        }
    }
    public class IPRatingEqualityComparer : IEqualityComparer<IPRating>
    {
        public bool Equals(IPRating? x, IPRating? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.SolidsRating == y.SolidsRating &&
                x.WaterRating == y.WaterRating &&
                x.AdditionalLetter == y.AdditionalLetter;
        }

        public int GetHashCode([DisallowNull] IPRating obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
    public enum IPSolidsRating
    {
        //Do not Change strings Saved In Db with their String Value
        X,
        _0,
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
    }
    public enum IPWaterRating
    {
        //Do not Change strings Saved In Db with their String Value
        X,
        _0,
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _6K,
        _7,
        _8,
        _9K
    }
    public enum IPAdditionalLetter
    {
        //Do not Change strings Saved In Db with their String Value
        Empty,
        D,
        F,
        H,
        M,
        S,
        W
    }
}


using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class MirrorLightElement : MirrorElementBase, IDeepClonable<MirrorLightElement> , IEqualityComparerCreator<MirrorLightElement>
    {
        public MirrorLightInfo LightInfo { get; set; } = MirrorLightInfo.Undefined();

        public MirrorLightElement(IMirrorElement elementInfo , MirrorLightInfo lightInfo) :base(elementInfo)
        {
            LightInfo = lightInfo.GetDeepClone();
        }
        public override MirrorLightElement GetDeepClone()
        {
            var clone = (MirrorLightElement)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = LocalizedDescriptionInfo.GetDeepClone();
            clone.LightInfo = LightInfo.GetDeepClone();
            return clone;
        }

        public static MirrorLightElement Undefined() => new(MirrorElementBase.Empty(),MirrorLightInfo.Undefined());

        public new static IEqualityComparer<MirrorLightElement> GetComparer()
        {
            return new MirrorLightElementEqualityComparer();
        }
    }
    public class MirrorLightElementEqualityComparer : IEqualityComparer<MirrorLightElement>
    {
        private readonly MirrorElementEqualityComparer elementComparer = new();
        private readonly MirrorLightInfoEqualityComparer lightComparer = new();

        public bool Equals(MirrorLightElement? x, MirrorLightElement? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementComparer.Equals(x,y) &&
                lightComparer.Equals(x.LightInfo,y.LightInfo);
        }

        public int GetHashCode([DisallowNull] MirrorLightElement obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    public class MirrorLightInfo : IRateableIP , IDeepClonable<MirrorLightInfo> , IEqualityComparerCreator<MirrorLightInfo>
    {
        public List<int> Kelvin { get; set; } = new();
        public double WattPerMeter { get; set; }
        public int Lumen { get; set; }
        public IPRating IP { get; set; } = new();

        public MirrorLightInfo GetDeepClone()
        {
            var clone = (MirrorLightInfo)this.MemberwiseClone();
            clone.IP = this.IP.GetDeepClone();
            clone.Kelvin = new(this.Kelvin);
            return clone;
        }

        public static MirrorLightInfo Undefined() => new();

        public static IEqualityComparer<MirrorLightInfo> GetComparer()
        {
            return new MirrorLightInfoEqualityComparer();
        }
    }
    public class MirrorLightInfoEqualityComparer : IEqualityComparer<MirrorLightInfo>
    {
        public bool Equals(MirrorLightInfo? x, MirrorLightInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            //base Comparer
            var ipComparer = new IPRatingEqualityComparer();

            return x!.Kelvin.SequenceEqual(y!.Kelvin) &&
                ipComparer.Equals(x.IP, y.IP) &&
                x.WattPerMeter == y.WattPerMeter &&
                x.Lumen == y.Lumen;
        }

        public int GetHashCode([DisallowNull] MirrorLightInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    public class MirrorAdditionalLightInfo : IWithButtonRegulation, IDeepClonable<MirrorAdditionalLightInfo> , IEqualityComparerCreator<MirrorAdditionalLightInfo>
    {
        public IlluminationOption Illumination { get; set; }
        /// <summary>
        /// Weather the Light is powered Externally to be Regulated without a Touch Button
        /// </summary>
        public bool IsPoweredExternally { get => !NeedsTouchButton; }
        public double LengthMeters { get; set; }
        public bool NeedsTouchButton { get; set; }
        public List<string> LightedItems { get; set; } = [];

        public bool IsOnlyMagnifierLight { get=> Illumination == IlluminationOption.Magnifyer1Illumination || Illumination == IlluminationOption.Magnifyer2Illumination || Illumination == (IlluminationOption.Magnifyer1Illumination | IlluminationOption.Magnifyer2Illumination); }

        public MirrorAdditionalLightInfo GetDeepClone()
        {
            var clone = (MirrorAdditionalLightInfo)this.MemberwiseClone();
            clone.LightedItems = new(this.LightedItems);
            return clone;
        }
        public static MirrorAdditionalLightInfo Undefined() => new();
        public static IEqualityComparer<MirrorAdditionalLightInfo> GetComparer()
        {
            return new MirrorAdditionalLightInfoEqualityComparer();
        }
    }
    public class MirrorAdditionalLightInfoEqualityComparer : IEqualityComparer<MirrorAdditionalLightInfo>
    {
        private readonly bool diregardLightedItemsList;
        private readonly bool normalizeIluminationWithoutMagnifier;
        private readonly bool disregardLightsLength;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diregardLightedItemsList">Does not compare the Lighted Items List if true</param>
        /// <param name="normalizeIluminationWithoutMagnifier">Disregards MagnifiersIllumination and When There is a PerimeterIllumination Flag diregards also any specific side illumination(ex. MirrorTop,MirrorLeft e.t.c.)</param>
        /// <param name="disregardLightsLength">Does not compare the Length of Lights when true</param>
        public MirrorAdditionalLightInfoEqualityComparer(
            bool diregardLightedItemsList = false,
            bool normalizeIluminationWithoutMagnifier = false,
            bool disregardLightsLength = false)
        {
            this.diregardLightedItemsList = diregardLightedItemsList;
            this.normalizeIluminationWithoutMagnifier = normalizeIluminationWithoutMagnifier;
            this.disregardLightsLength = disregardLightsLength;
        }

        public bool Equals(MirrorAdditionalLightInfo? x, MirrorAdditionalLightInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            
            return (diregardLightedItemsList || x!.LightedItems.SequenceEqual(y!.LightedItems))
                && (normalizeIluminationWithoutMagnifier ? NormalizeIlluminationOption(x.Illumination) == NormalizeIlluminationOption(y.Illumination) : x.Illumination == y.Illumination)
                && (disregardLightsLength || x.LengthMeters == y.LengthMeters)
                && x.NeedsTouchButton == y.NeedsTouchButton;
        }

        public int GetHashCode([DisallowNull] MirrorAdditionalLightInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
        /// <summary>
        /// Excludes the Magnifyer1 and Magnifyer2 from the Illumination Option 
        /// <para>Excludes MirrorTop - MirrorLeft - MirrorRight - MirrorBottom , when there is a MirrorPerimeter Flag</para>
        /// </summary>
        /// <param name="illumination"></param>
        /// <returns></returns>
        private static IlluminationOption NormalizeIlluminationOption(IlluminationOption illumination)
        {
            // Always exclude Magnifyer1 and Magnifyer2
            IlluminationOption toExclude = IlluminationOption.Magnifyer1Illumination | IlluminationOption.Magnifyer2Illumination;
            IlluminationOption result = illumination & ~toExclude;

            // If MirrorPerimeter is present, exclude MirrorTop, MirrorLeft, MirrorRight, MirrorBottom
            if ((result & IlluminationOption.MirrorPerimeterIllumination) == IlluminationOption.MirrorPerimeterIllumination)
            {
                IlluminationOption perimeterExclusions =
                    IlluminationOption.MirrorTopIllumination |
                    IlluminationOption.MirrorLeftIllumination |
                    IlluminationOption.MirrorRightIllumination |
                    IlluminationOption.MirrorBottomIllumination;
                result &= ~perimeterExclusions;
            }

            return result;
        }
    }

    public class MirrorLight : IPowerable, IDeepClonable<MirrorLight> , IUniquelyIdentifiable , IEqualityComparerCreator<MirrorLight>
    {
        public MirrorLightElement LightElement { get; set; } = MirrorLightElement.Undefined();
        public MirrorAdditionalLightInfo AdditionalLightInfo { get; set; } = MirrorAdditionalLightInfo.Undefined();
        public string ItemUniqueId { get; private set; } = Guid.NewGuid().ToString();

        public MirrorLight(IMirrorElement elementInfo,MirrorLightInfo lightInfo,MirrorAdditionalLightInfo additionalInfo)
        {
            LightElement = new(elementInfo, lightInfo);
            AdditionalLightInfo = additionalInfo;
        }
        public double GetEnergyConsumption()
        {
            return LightElement.LightInfo.WattPerMeter * AdditionalLightInfo.LengthMeters;
        }
        public double GetTransformerNominalPower()
        {
            if (AdditionalLightInfo.IsPoweredExternally)
            {
                return 0;
            }
            return GetEnergyConsumption();
        }

        public MirrorLight GetDeepClone()
        {
            var clone = (MirrorLight)this.MemberwiseClone();
            clone.LightElement = this.LightElement.GetDeepClone();
            clone.AdditionalLightInfo = this.AdditionalLightInfo.GetDeepClone();
            return clone;
        }

        public static IEqualityComparer<MirrorLight> GetComparer()
        {
            return new MirrorLightEqualityComparer();
        }

        public void AssignNewUniqueId()=> ItemUniqueId = Guid.NewGuid().ToString();
        public void AssignNewUniqueId(string uniqueId) => ItemUniqueId = uniqueId;
    }

    public class MirrorLightEqualityComparer : IEqualityComparer<MirrorLight>
    {
        private readonly MirrorLightElementEqualityComparer elementComparer = new();
        private readonly MirrorAdditionalLightInfoEqualityComparer lightComparer = new();
        public MirrorLightEqualityComparer()
        {
            
        }
        /// <summary>
        /// Creates the comparer with a custom LightAdditionalInfoComparer
        /// </summary>
        /// <param name="lightAdditionalInfoComparer">The Customized AdditionalLightInfo Comparer</param>
        public MirrorLightEqualityComparer(MirrorAdditionalLightInfoEqualityComparer lightAdditionalInfoComparer)
        {
            lightComparer = lightAdditionalInfoComparer;
        }

        public bool Equals(MirrorLight? x, MirrorLight? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementComparer.Equals(x.LightElement,y.LightElement) &&
                lightComparer.Equals(x.AdditionalLightInfo,y.AdditionalLightInfo) &&
                x.ItemUniqueId == y.ItemUniqueId;
        }

        public int GetHashCode([DisallowNull] MirrorLight obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    [Flags]
    public enum IlluminationOption
    {
        None = 0,
        MirrorPerimeterIllumination = 1,
        SandblastIllumination = 2,
        Magnifyer1Illumination = 4,
        Magnifyer2Illumination = 8,
        MirrorTopIllumination = 16,
        MirrorLeftIllumination = 32,
        MirrorRightIllumination = 64,
        MirrorBottomIllumination = 128
    }
}


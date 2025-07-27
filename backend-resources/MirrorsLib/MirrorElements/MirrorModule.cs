
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class MirrorModule : MirrorElementBase, IUniquelyIdentifiable, IDeepClonable<MirrorModule>, IEqualityComparerCreator<MirrorModule>
    {
        public virtual MirrorModuleInfo ModuleInfo { get; }
        public string ItemUniqueId { get; private set; } = Guid.NewGuid().ToString();

        public MirrorModule(IMirrorElement elementInfo, MirrorModuleInfo moduleInfo) :base(elementInfo)
        {
            ModuleInfo = moduleInfo.GetDeepClone();
        }
        private MirrorModule(MirrorElementBase elementInfo, MirrorModuleInfo moduleInfo, string itemUniqueId)
            : this(elementInfo,moduleInfo)
        {
            ItemUniqueId = itemUniqueId;
        }

        public override MirrorModule GetDeepClone()
        {
            var semiClone = (MirrorModule)this.MemberwiseClone();
            semiClone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            return new(semiClone, ModuleInfo.GetDeepClone(), ItemUniqueId);
        }
        public void AssignNewUniqueId() => ItemUniqueId = Guid.NewGuid().ToString();
        public void AssignNewUniqueId(string specificId) => ItemUniqueId = specificId;

        public new static IEqualityComparer<MirrorModule> GetComparer()
        {
            return new MirrorModuleEqualityComparer();
        }
    }
    public class MirrorModuleEqualityComparer : IEqualityComparer<MirrorModule>
    {
        private readonly bool includeUniqueIdComparison;
        private readonly MirrorElementEqualityComparer elementInfoComparer = new();
        private readonly MirrorModuleInfoEqualityComparer moduleComparer;

        public MirrorModuleEqualityComparer(bool includeUniqueIdComparison = true,bool disreagardCollisionDistances = false)
        {
            this.includeUniqueIdComparison = includeUniqueIdComparison;
            moduleComparer = new(disreagardCollisionDistances);
        }

        public bool Equals(MirrorModule? x, MirrorModule? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementInfoComparer.Equals(x, y) &&
                moduleComparer.Equals(x.ModuleInfo, y.ModuleInfo)
                && (!includeUniqueIdComparison || x.ItemUniqueId == y.ItemUniqueId);
        }

        public int GetHashCode([DisallowNull] MirrorModule obj)
        {
            return HashCode.Combine(elementInfoComparer.GetHashCode(obj),
                                    moduleComparer.GetHashCode(obj.ModuleInfo),
                                    includeUniqueIdComparison ? obj.ItemUniqueId.GetHashCode() : 17);
        }
    }

    public abstract class MirrorModuleInfo : IDeepClonable<MirrorModuleInfo>, IEqualityComparerCreator<MirrorModuleInfo>
    {
        public MirrorModuleType ModuleType { get; protected set; }

        public abstract MirrorModuleInfo GetDeepClone();
        public static MirrorModuleInfo Undefined() => new UndefinedMirrorModuleInfo();

        public static IEqualityComparer<MirrorModuleInfo> GetComparer()
        {
            return new MirrorModuleInfoEqualityComparer();
        }
    }

    public class UndefinedMirrorModuleInfo : MirrorModuleInfo, IDeepClonable<UndefinedMirrorModuleInfo>
    {
        public override UndefinedMirrorModuleInfo GetDeepClone()
        {
            return new();
        }
    }
    public class MirrorModuleInfoEqualityComparer : IEqualityComparer<MirrorModuleInfo>
    {
        private readonly bool disregardCollisionDistances;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public MirrorModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        public MirrorModuleInfoEqualityComparer()
        {

        }
        public bool Equals(MirrorModuleInfo? x, MirrorModuleInfo? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case BluetoothModuleInfo bluetooth:
                    var bluetoothComparer = new BluetoothModuleInfoEqualityComparer(disregardCollisionDistances);
                    return bluetoothComparer.Equals(bluetooth, (BluetoothModuleInfo)y);
                case MagnifierSandblastedModuleInfo sandMagn:
                    var sandMagnComparer = new MagnifierSandblastedModuleInfoEqualityComparer(disregardCollisionDistances);
                    return sandMagnComparer.Equals(sandMagn, (MagnifierSandblastedModuleInfo)y);
                case MagnifierModuleInfo magn:
                    var magnComparer = new MagnifierModuleInfoEqualityComparer(disregardCollisionDistances);
                    return magnComparer.Equals(magn, (MagnifierModuleInfo)y);
                case MirrorBackLidModuleInfo lid:
                    var lidComparer = new MirrorBackLidModuleInfoEqualityComparer(disregardCollisionDistances);
                    return lidComparer.Equals(lid, (MirrorBackLidModuleInfo)y);
                case MirrorLampModuleInfo lamp:
                    var lampComparer = new MirrorLampModuleInfoEqualityComparer(disregardCollisionDistances);
                    return lampComparer.Equals(lamp, (MirrorLampModuleInfo)y);
                case ResistancePadModuleInfo pad:
                    var padComparer = new ResistancePadModuleInfoEqualityComparer(disregardCollisionDistances);
                    return padComparer.Equals(pad, (ResistancePadModuleInfo)y);
                case RoundedCornersModuleInfo corners:
                    var cornersComparer = new RoundedCornersModuleInfoEqualityComparer();
                    return cornersComparer.Equals(corners, (RoundedCornersModuleInfo)y);
                case ScreenModuleInfo screen:
                    var screenComparer = new ScreenModuleInfoEqualityComparer(disregardCollisionDistances);
                    return screenComparer.Equals(screen, (ScreenModuleInfo)y);
                case TouchButtonModuleInfo touch:
                    var touchComparer = new TouchButtonModuleInfoEqualityComparer(disregardCollisionDistances);
                    return touchComparer.Equals(touch, (TouchButtonModuleInfo)y);
                case TransformerModuleInfo transformer:
                    var transformerComparer = new TransformerModuleInfoEqualityComparer(disregardCollisionDistances);
                    return transformerComparer.Equals(transformer, (TransformerModuleInfo)y);
                case MirrorProcessModuleInfo process:
                    var processComparer = new MirrorProcessModuleInfoEqualityComparer(disregardCollisionDistances);
                    return processComparer.Equals(process, (MirrorProcessModuleInfo)y);
                case UndefinedMirrorModuleInfo: return true;
                default:
                    throw new NotSupportedException($"The Provided {nameof(MirrorModuleInfo)} type is not Supported for Equality Comparison : {x.GetType().Name}");
            }
        }

        public int GetHashCode([DisallowNull] MirrorModuleInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            switch (obj)
            {
                case BluetoothModuleInfo bluetooth:
                    var bluetoothComparer = new BluetoothModuleInfoEqualityComparer(disregardCollisionDistances);
                    return bluetoothComparer.GetHashCode(bluetooth);
                case MagnifierSandblastedModuleInfo sandMagn:
                    var sandMagnComparer = new MagnifierSandblastedModuleInfoEqualityComparer(disregardCollisionDistances);
                    return sandMagnComparer.GetHashCode(sandMagn);
                case MagnifierModuleInfo magn:
                    var magnComparer = new MagnifierModuleInfoEqualityComparer(disregardCollisionDistances);
                    return magnComparer.GetHashCode(magn);
                case MirrorBackLidModuleInfo lid:
                    var lidComparer = new MirrorBackLidModuleInfoEqualityComparer(disregardCollisionDistances);
                    return lidComparer.GetHashCode(lid);
                case MirrorLampModuleInfo lamp:
                    var lampComparer = new MirrorLampModuleInfoEqualityComparer(disregardCollisionDistances);
                    return lampComparer.GetHashCode(lamp);
                case ResistancePadModuleInfo pad:
                    var padComparer = new ResistancePadModuleInfoEqualityComparer(disregardCollisionDistances);
                    return padComparer.GetHashCode(pad);
                case RoundedCornersModuleInfo corners:
                    var cornersComparer = new RoundedCornersModuleInfoEqualityComparer();
                    return cornersComparer.GetHashCode(corners);
                case ScreenModuleInfo screen:
                    var screenComparer = new ScreenModuleInfoEqualityComparer(disregardCollisionDistances);
                    return screenComparer.GetHashCode(screen);
                case TouchButtonModuleInfo touch:
                    var touchComparer = new TouchButtonModuleInfoEqualityComparer(disregardCollisionDistances);
                    return touchComparer.GetHashCode(touch);
                case TransformerModuleInfo transformer:
                    var transformerComparer = new TransformerModuleInfoEqualityComparer(disregardCollisionDistances);
                    return transformerComparer.GetHashCode(transformer);
                case MirrorProcessModuleInfo process:
                    var processComparer = new MirrorProcessModuleInfoEqualityComparer(disregardCollisionDistances);
                    return processComparer.GetHashCode(process);
                case UndefinedMirrorModuleInfo: return 37;
                default:
                    throw new NotSupportedException($"The Provided {nameof(MirrorModuleInfo)} type is not Supported for HashCode Generation : {obj.GetType().Name}");
            }
        }
    }
    public class MirrorModuleInfoBaseEqualityComparer : IEqualityComparer<MirrorModuleInfo>
    {
        public bool Equals(MirrorModuleInfo? x, MirrorModuleInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.ModuleType == y.ModuleType;
        }

        public int GetHashCode([DisallowNull] MirrorModuleInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            return obj.ModuleType.GetHashCode();
        }
    }

}

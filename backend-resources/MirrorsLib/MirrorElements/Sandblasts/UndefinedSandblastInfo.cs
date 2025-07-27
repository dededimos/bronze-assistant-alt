using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public class UndefinedSandblastInfo : MirrorSandblastInfo, IDeepClonable<UndefinedSandblastInfo>
    {
        public UndefinedSandblastInfo()
        {
            SandblastType = MirrorSandblastType.Undefined;
        }
        public override UndefinedSandblastInfo GetDeepClone()
        {
            return new();
        }

        public override ShapeInfo GetShapeInfo(ShapeInfo parent)
        {
            return ShapeInfo.Undefined();
        }
    }
}


using CommonInterfacesBronze;
using ShapesLibrary;

namespace MirrorsLib.MirrorElements.Supports
{
    public class UndefinedMirrorSupportInfo : MirrorSupportInfo, IDeepClonable<UndefinedMirrorSupportInfo>
    {
        public override UndefinedMirrorSupportInfo GetDeepClone()
        {
            return new();
        }
    }



}


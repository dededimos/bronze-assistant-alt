using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class MirrorFinishElement : MirrorElementBase, IDeepClonable<MirrorFinishElement> , IEqualityComparerCreator<MirrorFinishElement>
    {
        public MirrorFinishElement()
        {

        }
        public MirrorFinishElement(IMirrorElement elementInfo, DrawBrush finishColorBrush)
            : base(elementInfo)
        {
            FinishColorBrush = finishColorBrush;
        }

        public DrawBrush FinishColorBrush { get; set; } = DrawBrush.Empty;

        public new static IEqualityComparer<MirrorFinishElement> GetComparer()
        {
            return new MirrorFinishElementEqualityComparer();
        }

        public override MirrorFinishElement GetDeepClone()
        {
            var clone = (MirrorFinishElement)base.GetDeepClone();
            clone.FinishColorBrush = FinishColorBrush.GetDeepClone();
            return clone;
        }
        /// <summary>
        /// Returns an Empty Finish
        /// </summary>
        /// <returns></returns>
        public static MirrorFinishElement EmptyFinish()
        {
            return new(MirrorElementBase.Empty(),DrawBrush.Empty);
        }
    }
    public class MirrorFinishElementEqualityComparer : IEqualityComparer<MirrorFinishElement>
    {
        private readonly MirrorElementEqualityComparer elementComparer = new();
        private readonly DrawBrushEqualityComparer brushComparer = new();

        public bool Equals(MirrorFinishElement? x, MirrorFinishElement? y)
        {
            return elementComparer.Equals(x,y) &&
                brushComparer.Equals(x!.FinishColorBrush,y!.FinishColorBrush);
        }

        public int GetHashCode([DisallowNull] MirrorFinishElement obj)
        {
            return HashCode.Combine(elementComparer.GetHashCode(obj), brushComparer.GetHashCode(obj.FinishColorBrush));
        }
    }



}

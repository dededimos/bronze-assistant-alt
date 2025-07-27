using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Interfaces;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class MirrorProcessModuleInfo : MirrorModuleInfo , IMirrorPositionable,IDeepClonable<MirrorProcessModuleInfo> ,IEqualityComparerCreator<MirrorProcessModuleInfo>
    {
        public MirrorProcessModuleInfo()
        {
            ModuleType = Enums.MirrorModuleType.ProcessModuleType;
        }
        public ShapeInfo ProcessShape { get; set; } = ShapeInfo.Undefined();
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }

        static IEqualityComparer<MirrorProcessModuleInfo> IEqualityComparerCreator<MirrorProcessModuleInfo>.GetComparer()
        {
            return new MirrorProcessModuleInfoEqualityComparer();
        }

        public override MirrorProcessModuleInfo GetDeepClone()
        {
            var clone = (MirrorProcessModuleInfo)this.MemberwiseClone();
            clone.ProcessShape = ProcessShape.GetDeepClone();
            return clone;
        }

        public PointXY GetPosition()
        {
            return ProcessShape.GetLocation();
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            ProcessShape.LocationX = point.X;
            ProcessShape.LocationY = point.Y;
        }
    }
    public class MirrorProcessModuleInfoEqualityComparer : IEqualityComparer<MirrorProcessModuleInfo> 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public MirrorProcessModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new();
            shapeComparer = new(disregardCollisionDistances);
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly ShapeInfoEqualityComparer shapeComparer;
        private readonly bool disregardCollisionDistances;

        public bool Equals(MirrorProcessModuleInfo? x, MirrorProcessModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                shapeComparer.Equals(x!.ProcessShape, y!.ProcessShape) &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] MirrorProcessModuleInfo obj)
        {
            int hash = HashCode.Combine(baseComparer.GetHashCode(obj), shapeComparer.GetHashCode(obj.ProcessShape));
            if (!disregardCollisionDistances)
            {
                hash = HashCode.Combine(hash, obj.MinDistanceFromSupport, obj.MinDistanceFromSandblast, obj.MinDistanceFromOtherModules);
            }
            return hash;
        }
    }
}

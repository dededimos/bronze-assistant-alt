using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using MirrorsLib.MirrorElements;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorFinishElementEntity : MirrorElementEntity, IDeepClonable<MirrorFinishElementEntity> , IEqualityComparerCreator<MirrorFinishElementEntity>
    {
        public MirrorFinishElementEntity()
        {
            
        }

        public DrawBrushDTO FinishColorBrush { get; set; } = DrawBrushDTO.Empty();

        static IEqualityComparer<MirrorFinishElementEntity> IEqualityComparerCreator<MirrorFinishElementEntity>.GetComparer()
        {
            return new MirrorFinishElementEntityEqualityComparer();
        }

        public override MirrorFinishElementEntity GetDeepClone()
        {
            var clone = (MirrorFinishElementEntity)base.GetDeepClone();
            clone.FinishColorBrush = this.FinishColorBrush.GetDeepClone();
            return clone;
        }
        public MirrorFinishElement ToFinishElement()
        {
            return new(this.GetMirrorElementInfo(), FinishColorBrush.ToDrawBrush());
        }

    }

    public class DrawBrushDTO : IDeepClonable<DrawBrushDTO>, IEqualityComparerCreator<DrawBrushDTO>
    {
        public DrawBrushDTO()
        {

        }
        public DrawBrushDTO(DrawBrush brush)
        {
            Color = brush.Color;
            foreach (var stop in brush.GradientStops)
            {
                DrawGradientStopDTO stopDTO = new(stop);
                GradientStops.Add(stopDTO);
            }
            GradientAngleDegrees = brush.GradientAngleDegrees;
        }
        public string Color { get; set; } = string.Empty;
        public List<DrawGradientStopDTO> GradientStops { get; set; } = [];
        public double GradientAngleDegrees { get; set; }
        public bool IsSolidColor { get => GradientStops.Count == 0; }

        public DrawBrushDTO GetDeepClone()
        {
            return new()
            {
                Color = Color,
                GradientStops = GradientStops.GetDeepClonedList(),
                GradientAngleDegrees = GradientAngleDegrees,
            };
        }
        public DrawBrush ToDrawBrush()
        {
            if (IsSolidColor)
            {
                return new(Color);
            }
            else
            {
                return new(GradientAngleDegrees, GradientStops.Select(s => s.ToGradientStop()).ToArray());
            }
        }
        public static DrawBrushDTO Empty() => new();

        public static IEqualityComparer<DrawBrushDTO> GetComparer()
        {
            return new DrawBrushDTOEqualityComparer();
        }
    }
    public class DrawBrushDTOEqualityComparer : IEqualityComparer<DrawBrushDTO>
    {
        private readonly DrawGradientStopDTOEqualityComparer gradientComparer = new();

        public bool Equals(DrawBrushDTO? x, DrawBrushDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            return x.Color == y.Color &&
                x.GradientAngleDegrees == y.GradientAngleDegrees &&
                x.GradientStops.SequenceEqual(y.GradientStops, gradientComparer);
        }

        public int GetHashCode([DisallowNull] DrawBrushDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
    public class DrawGradientStopDTO : IDeepClonable<DrawGradientStopDTO>, IEqualityComparerCreator<DrawGradientStopDTO>
    {
        public DrawGradientStopDTO()
        {

        }
        public DrawGradientStopDTO(DrawGradientStop gradientStop)
        {
            Color = gradientStop.Color;
            Offset = gradientStop.Offset;
        }
        public string Color { get; set; } = string.Empty;
        public double Offset { get; set; }

        public static IEqualityComparer<DrawGradientStopDTO> GetComparer()
        {
            return new DrawGradientStopDTOEqualityComparer();
        }

        public DrawGradientStopDTO GetDeepClone()
        {
            return new()
            {
                Color = this.Color,
                Offset = this.Offset
            };
        }

        public DrawGradientStop ToGradientStop()
        {
            return new(Color, Offset);
        }
    }
    public class DrawGradientStopDTOEqualityComparer : IEqualityComparer<DrawGradientStopDTO>
    {
        public bool Equals(DrawGradientStopDTO? x, DrawGradientStopDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            return x.Offset == y.Offset &&
                x.Color == y.Color;
        }

        public int GetHashCode([DisallowNull] DrawGradientStopDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
    public class MirrorFinishElementEntityEqualityComparer : IEqualityComparer<MirrorFinishElementEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly DrawBrushDTOEqualityComparer brushComparer = new();

        public bool Equals(MirrorFinishElementEntity? x, MirrorFinishElementEntity? y)
        {
            return baseComparer.Equals(x, y) && brushComparer.Equals(x?.FinishColorBrush, y?.FinishColorBrush);
        }

        public int GetHashCode([DisallowNull] MirrorFinishElementEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

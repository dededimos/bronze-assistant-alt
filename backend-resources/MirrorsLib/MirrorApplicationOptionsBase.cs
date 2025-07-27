using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Services.CodeBuldingService;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib
{
    /// <summary>
    /// A Class that represents options for anything related to the Mirrors
    /// </summary>
    public class MirrorApplicationOptionsBase : IDeepClonable<MirrorApplicationOptionsBase> , IEqualityComparerCreator<MirrorApplicationOptionsBase>
    {
        public MirrorApplicationOptionsBase()
        {
            
        }
        public static MirrorApplicationOptionsBase Empty() => new();

        public static IEqualityComparer<MirrorApplicationOptionsBase> GetComparer()
        {
            return new MirrorApplicationOptionsBaseEqualityComparer();
        }

        public virtual MirrorApplicationOptionsBase GetDeepClone()
        {
            return (MirrorApplicationOptionsBase)this.MemberwiseClone();
        }
    }
    public class MirrorApplicationOptionsBaseEqualityComparer : IEqualityComparer<MirrorApplicationOptionsBase>
    {
        private readonly MirrorCodesBuilderOptionsEqualityComparer codesBuilderComparer = new();

        public bool Equals(MirrorApplicationOptionsBase? x, MirrorApplicationOptionsBase? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            return x switch
            {
                MirrorCodesBuilderOptions codesBuilderOptions => codesBuilderComparer.Equals(codesBuilderOptions, (MirrorCodesBuilderOptions)y),
                MirrorApplicationOptionsBase => true,
                _ => throw new NotSupportedException($"{x.GetType().Name} is not a Supported {nameof(MirrorApplicationOptionsBase)} type , The Comparison couldn't get through"),
            };
        }

        public int GetHashCode([DisallowNull] MirrorApplicationOptionsBase obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

}

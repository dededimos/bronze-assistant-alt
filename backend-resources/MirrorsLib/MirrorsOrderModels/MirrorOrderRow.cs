using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using CommonOrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.MirrorsOrderModels
{
    public class MirrorOrderRow : BronzeOrderRowBase<MirrorSynthesis>, IDeepClonable<MirrorOrderRow>, IEqualityComparerCreator<MirrorOrderRow>
    {
        public MirrorOrderRow()
        {
            RowUnits = MeasurementUnit.Pieces;
            Created = DateTime.Now;
            LastModified = DateTime.Now;
        }

        public static IEqualityComparer<MirrorOrderRow> GetComparer()
        {
            return new MirrorOrderRowEqualityComparer();
        }

        public override MirrorOrderRow GetDeepClone()
        {
            return (MirrorOrderRow)base.GetDeepClone();
        }

        public bool HasEqualSynthesisWithOther(MirrorOrderRow other)
        {
            if (RowItem is null || other?.RowItem is null) return false;

            return RowItem.Equals(other.RowItem);
        }

    }
    public class MirrorOrderRowEqualityComparer : IEqualityComparer<BronzeOrderRowBase<MirrorSynthesis>>
    {
        private readonly MirrorSynthesisEqualityComparer mirrorComparer = new();

        public bool Equals(BronzeOrderRowBase<MirrorSynthesis>? x, BronzeOrderRowBase<MirrorSynthesis>? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Notes == y.Notes
                && x.Quantity == y.Quantity
                && x.ParentOrderNo == y.ParentOrderNo
                && x.RowId == y.RowId
                && x.FilledQuantity == y.FilledQuantity
                && x.CancelledQuantity == y.CancelledQuantity
                && x.RowUnits == y.RowUnits
                && x.Created == y.Created
                && x.LastModified == y.LastModified
                && x.LineNumber == y.LineNumber
                && x.Metadata.IsEqualToOtherDictionary(y.Metadata)
                && mirrorComparer.Equals(x.RowItem, y.RowItem);
        }
        public int GetHashCode(BronzeOrderRowBase<MirrorSynthesis> obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    /// <summary>
    /// A Comparer that compares two MirrorOrderRow objects and checks if they can be combined
    /// <para>Returns true only for rows with the Same Mirrors and Notes</para>
    /// </summary>
    public class MirrorOrderRowCanCombineComparer : IEqualityComparer<MirrorOrderRow>
    {
        private readonly MirrorSynthesisHasEqualGlassComparer equalMirrorGlassComparer = new();
        public bool Equals(MirrorOrderRow? x, MirrorOrderRow? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            return x.Notes == y.Notes
                && x.RowUnits == y.RowUnits
                && equalMirrorGlassComparer.Equals(x.RowItem, y.RowItem);
        }
        public int GetHashCode(MirrorOrderRow obj)
        {
            return HashCode.Combine(obj.Notes,
                                    obj.RowUnits,
                                    obj.RowItem == null ? 31 : equalMirrorGlassComparer.GetHashCode(obj.RowItem));
        }
    }


}

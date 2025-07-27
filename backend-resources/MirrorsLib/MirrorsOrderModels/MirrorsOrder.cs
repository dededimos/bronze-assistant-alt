using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using CommonOrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MirrorsLib.MirrorsOrderModels
{
    public partial class MirrorsOrder : BronzeOrderBase<MirrorOrderRow, MirrorSynthesis>, IDeepClonable<MirrorsOrder>, IEqualityComparerCreator<MirrorsOrder>
    {
        public MirrorsOrder()
        {
            Created = DateTime.Now;
            LastModified = DateTime.Now;
        }

        public double TotalQuantity { get => Rows.Sum(r => r.Quantity); }
        public double TotalPendingQuantity { get => Rows.Sum(r => r.PendingQuantity); }
        public double TotalFilledQuantity { get => Rows.Sum(r => r.FilledQuantity); }

        public static IEqualityComparer<MirrorsOrder> GetComparer()
        {
            return new MirrorsOrderEqualityComparer();
        }

        public override MirrorsOrder GetDeepClone()
        {
            return (MirrorsOrder)base.GetDeepClone();
        }

        /// <summary>
        /// The Allowed Pattern for the Order No (ex. 1524AC)
        /// Summary: What Does This Regex Do?
        ///<para>This regex validates if a string matches one of three possible formats:</para>
        ///<para>A numeric ID with optional trailing letters:</para>
        ///<para>1 to 4 digits(0-9999) , Optionally followed by 0 to 2 letters , ✅ Examples: 123, 99AB, 4567, 1Z , ❌ Not Allowed: ABCDE, 123456, 99ABCD</para>
        ///<para>Any string ending in "Old" ,✅ Examples: "OrderOld", "SomethingOld", "123Old",❌ Not Allowed: "OldSomething", "Older"</para>
        ///<para>Exactly four question marks '????' )</para>
        /// </summary>
        public static readonly Regex OrderNoRegex = GenerateOrderNoRegex();
        /// <summary>
        /// Generates the Regex at Compile time and not at runtime
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("^(?:[0-9]{1,4}[a-zA-Z]{0,2}|.*Old|\\?\\?\\?\\?)$")]
        private static partial Regex GenerateOrderNoRegex();
    }
    public class MirrorsOrderEqualityComparer : IEqualityComparer<MirrorsOrder>
    {
        private readonly MirrorOrderRowEqualityComparer rowComparer = new();

        public bool Equals(MirrorsOrder? x, MirrorsOrder? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.OrderNo == y.OrderNo
                && x.Created == y.Created
                && x.LastModified == y.LastModified
                && x.Notes == y.Notes
                && x.Metadata.IsEqualToOtherDictionary(y.Metadata)
                && x.Rows.SequenceEqual(y.Rows, rowComparer);
        }
        public int GetHashCode(MirrorsOrder obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

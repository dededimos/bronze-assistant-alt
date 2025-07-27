using CommonHelpers;
using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models
{
    //Info Regarding Synthesis Class: 
    //Whenever a Model is Combined with Other Models various Dimensions/Constraints/Options might need to change from the Default
    //For Example : Tollerances on B6000 Cabins combined with 9F Fixed Panels , Magnet Options on Doors Combined with Fixed 8W or 6W Panels e.t.c.
    //Also Synthesis Class Acts as a prepared stage for Feeding the Cabin Models to the Glass Builder to Calculate the Glasses .
    //The Concrete Glass Builders Do not Know if a given Glass they build , is destined for a Synthesis ,or For a Single Cabin . The Properties of the Cabins Passed to the Builders must be prePrepped.
    //The Above problem arises mostly from the Fact that The Nominal Length of a Cabin , Might be the Same for Various Configurations though the Tollerances or LengthMin Might Change between those Configurations
    //Meaning that the User Mostly Judging from Nominal Length thinks the Cabin is the Same (And it must be for him) , technically though this is not the case.
    //Finally With this Class we Avoid the Not so Nice Implementation of Extra Panels and Cloned 9A models.

    /// <summary>
    /// Represents a Combo of Different Cabin Models forming one Structure
    /// </summary>
    public class CabinSynthesis : IDeepClonable<CabinSynthesis>
    {
        /// <summary>
        /// The Synthesis Defining Draw No / Code
        /// </summary>
        public CabinDrawNumber DrawNo { get; set; }

        /// <summary>
        /// The Primary Cabin of the Synthesis
        /// </summary>
        public Cabin Primary { get; set; }

        /// <summary>
        /// The Secondary Cabin of the Synthesis
        /// </summary>
        public Cabin Secondary { get; set; }

        /// <summary>
        /// The Tertiary Cabin of the Synthesis
        /// </summary>
        public Cabin Tertiary { get; set; }

        /// <summary>
        /// The Direction of the Synthesis as a whole (Always that of the Primary Cabin)
        /// </summary>
        public CabinDirection Direction { get => Primary != null ? Primary.Direction : CabinDirection.Undefined; }

        /// <summary>
        /// Weather the Synthesis is Reversible or Not
        /// </summary>
        public bool IsReversible { get => GetIsReversible();}

        /// <summary>
        /// How Many Sides the Syntheis Has
        /// </summary>
        public int NumberOfSides { get => GetSidesNo(); }

        /// <summary>
        /// What type of Opening this Synthesis Has
        /// </summary>
        public SynthesisTypeOfOpening TypeOfOpening { get => GetOpeningType(); }

        /// <summary>
        /// The total Opening of the Synthesis Structure
        /// </summary>
        public double Opening { get => GetOpening(); }
        public int NumberOfGlasses { get => GetNumberOfGlasses(); }

        /// <summary>
        /// An Empty Synthesis (Cabin Models are all Null and must be Set)
        /// </summary>
        public CabinSynthesis()
        {
            //Empty -- Cabin Models are Null
        }

        public static CabinSynthesis Undefined()
        {
            var synthesis = new CabinSynthesis();
            //synthesis.Primary = Cabin.Empty();
            //synthesis.Secondary = Cabin.Empty();
            //synthesis.Tertiary = Cabin.Empty();
            return synthesis;
        }

        /// <summary>
        /// Returns the Number of Sides of the Synthesis
        /// </summary>
        /// <returns>Number of Sides (0-1-2-3)</returns>
        private int GetSidesNo()
        {
            int sidesNo = 0;
            sidesNo = Primary?.Model != null ? sidesNo + 1 : sidesNo;
            sidesNo = Secondary?.Model != null ? sidesNo + 1 : sidesNo;
            sidesNo = Tertiary?.Model != null ? sidesNo + 1 : sidesNo;

            return sidesNo;
        }

        /// <summary>
        /// Return the Number of Glasses present in the Synthesis
        /// </summary>
        /// <returns>The Number of Glasses</returns>
        public int GetNumberOfGlasses()
        {
            int numberOfGlasses = 0;
            numberOfGlasses += Primary?.Model != null ? Primary.Glasses.Count : 0;
            numberOfGlasses += Secondary?.Model != null ? Secondary.Glasses.Count : 0;
            numberOfGlasses += Tertiary?.Model != null ? Tertiary.Glasses.Count : 0;
            return numberOfGlasses;
        }

        /// <summary>
        /// Determines wheather all the Cabins of the Syunthesis are Reversible
        /// </summary>
        /// <returns></returns>
        private bool GetIsReversible()
        {
            bool isReversible = true;
            if (Primary is not null)
            {
                isReversible = Primary.IsReversible;
            }
            if (Secondary is not null)
            {
                isReversible &= Secondary.IsReversible;
            }
            if (Tertiary is not null)
            {
                isReversible &= Tertiary.IsReversible;
            }
            return isReversible;
        }

        /// <summary>
        /// Returns the Opening of the SYnthesis in mm
        /// </summary>
        /// <returns></returns>
        public double GetOpening()
        {
            double opening = 0;
            switch (DrawNo)
            {
                //SINGLE DOOR
                case CabinDrawNumber.Draw9S:
                case CabinDrawNumber.Draw9S9F:
                case CabinDrawNumber.Draw9S9F9F:
                case CabinDrawNumber.Draw94:
                case CabinDrawNumber.Draw949F:
                case CabinDrawNumber.Draw949F9F:
                case CabinDrawNumber.Draw9B:
                case CabinDrawNumber.Draw9B9F:
                case CabinDrawNumber.Draw9B9F9F:
                case CabinDrawNumber.DrawVS:
                case CabinDrawNumber.DrawVSVF:
                case CabinDrawNumber.DrawV4:
                case CabinDrawNumber.DrawV4VF:
                case CabinDrawNumber.DrawWS:
                case CabinDrawNumber.DrawNP44:
                case CabinDrawNumber.DrawQP44:
                case CabinDrawNumber.DrawCornerNP6W45:
                case CabinDrawNumber.DrawCornerQP6W45:
                case CabinDrawNumber.DrawStraightNP6W47:
                case CabinDrawNumber.DrawStraightQP6W47:
                case CabinDrawNumber.DrawNB31:
                case CabinDrawNumber.DrawQB31:
                case CabinDrawNumber.DrawCornerNB6W32:
                case CabinDrawNumber.DrawCornerQB6W32:
                case CabinDrawNumber.DrawStraightNB6W38:
                case CabinDrawNumber.DrawStraightQB6W38:
                case CabinDrawNumber.DrawDB51:
                case CabinDrawNumber.DrawCornerDB8W52:
                case CabinDrawNumber.DrawStraightDB8W59:
                case CabinDrawNumber.DrawHB34:
                case CabinDrawNumber.DrawCornerHB8W35:
                case CabinDrawNumber.DrawStraightHB8W40:
                    opening = Primary?.Opening ?? 0;
                    break;
                //CORNER DOORS
                case CabinDrawNumber.Draw9A:
                case CabinDrawNumber.Draw9A9F:
                case CabinDrawNumber.DrawVA:
                case CabinDrawNumber.Draw2CornerNP46:
                case CabinDrawNumber.Draw2CornerQP46:
                case CabinDrawNumber.Draw2CornerNB33:
                case CabinDrawNumber.Draw2CornerQB33:
                case CabinDrawNumber.Draw2CornerDB53:
                case CabinDrawNumber.Draw2CornerHB37:
                    double discriminant = (Math.Pow(Primary?.Opening ?? 0, 2) + Math.Pow(Secondary?.Opening ?? 0, 2));
                    opening = Math.Sqrt(discriminant);
                    break;
                //STRAIGHT DOUBLE DOOR (2PIECES)
                case CabinDrawNumber.Draw2StraightNP48:
                case CabinDrawNumber.Draw2StraightQP48:
                case CabinDrawNumber.Draw2StraightNB41:
                case CabinDrawNumber.Draw2StraightQB41:
                case CabinDrawNumber.Draw2StraightDB61:
                case CabinDrawNumber.Draw2StraightHB43:
                    opening = (Primary?.Opening ?? 0) + (Secondary?.Opening ?? 0);
                    break;
                //FLIPPER
                case CabinDrawNumber.Draw8WFlipper81:
                    opening = Secondary?.Opening ?? 0;
                    break;
                //FIXED PANELS ONLY
                case CabinDrawNumber.Draw2Corner8W82:
                case CabinDrawNumber.Draw1Corner8W84:
                case CabinDrawNumber.Draw2Straight8W85:
                case CabinDrawNumber.Draw2CornerStraight8W88:
                case CabinDrawNumber.Draw8W:
                case CabinDrawNumber.DrawE:
                case CabinDrawNumber.Draw9C:
                case CabinDrawNumber.Draw9C9F:
                case CabinDrawNumber.Draw9F:
                case CabinDrawNumber.DrawVF:
                case CabinDrawNumber.None:
                default:
                    //Returns "0"
                    break;
            }
            return opening;
        }

        /// <summary>
        /// Returns the Cabin List of the Synthesis
        /// </summary>
        /// <returns>The List of Cabins</returns>
        public List<Cabin> GetCabinList()
        {
            List<Cabin> cabins = [];
            cabins.AddNotNull(Primary);
            cabins.AddNotNull(Secondary);
            cabins.AddNotNull(Tertiary);
            return cabins;
        }

        private SynthesisTypeOfOpening GetOpeningType()
        {
            switch (DrawNo)
            {
                //SINGLE DOOR
                case CabinDrawNumber.Draw9S:
                case CabinDrawNumber.Draw9S9F:
                case CabinDrawNumber.Draw9S9F9F:
                case CabinDrawNumber.Draw94:
                case CabinDrawNumber.Draw949F:
                case CabinDrawNumber.Draw949F9F:
                case CabinDrawNumber.Draw9B:
                case CabinDrawNumber.Draw9B9F:
                case CabinDrawNumber.Draw9B9F9F:
                case CabinDrawNumber.DrawVS:
                case CabinDrawNumber.DrawVSVF:
                case CabinDrawNumber.DrawV4:
                case CabinDrawNumber.DrawV4VF:
                case CabinDrawNumber.DrawWS:
                case CabinDrawNumber.DrawNP44:
                case CabinDrawNumber.DrawQP44:
                case CabinDrawNumber.DrawCornerNP6W45:
                case CabinDrawNumber.DrawCornerQP6W45:
                case CabinDrawNumber.DrawStraightNP6W47:
                case CabinDrawNumber.DrawStraightQP6W47:
                case CabinDrawNumber.DrawNB31:
                case CabinDrawNumber.DrawQB31:
                case CabinDrawNumber.DrawCornerNB6W32:
                case CabinDrawNumber.DrawCornerQB6W32:
                case CabinDrawNumber.DrawStraightNB6W38:
                case CabinDrawNumber.DrawStraightQB6W38:
                case CabinDrawNumber.DrawDB51:
                case CabinDrawNumber.DrawCornerDB8W52:
                case CabinDrawNumber.DrawStraightDB8W59:
                case CabinDrawNumber.DrawHB34:
                case CabinDrawNumber.DrawCornerHB8W35:
                case CabinDrawNumber.DrawStraightHB8W40:
                    return SynthesisTypeOfOpening.SinglePiece;

                //Diagonal Opening
                case CabinDrawNumber.Draw9A:
                case CabinDrawNumber.Draw9A9F:
                case CabinDrawNumber.DrawVA:
                case CabinDrawNumber.Draw2CornerNP46:
                case CabinDrawNumber.Draw2CornerQP46:
                case CabinDrawNumber.Draw2CornerNB33:
                case CabinDrawNumber.Draw2CornerQB33:
                case CabinDrawNumber.Draw2CornerDB53:
                case CabinDrawNumber.Draw2CornerHB37:
                    return SynthesisTypeOfOpening.Diagonal;

                //StraightOpening
                case CabinDrawNumber.Draw2StraightNP48:
                case CabinDrawNumber.Draw2StraightQP48:
                case CabinDrawNumber.Draw2StraightNB41:
                case CabinDrawNumber.Draw2StraightQB41:
                case CabinDrawNumber.Draw2StraightDB61:
                case CabinDrawNumber.Draw2StraightHB43:
                    return SynthesisTypeOfOpening.StraightTwoPiece;

                //FLIPPER
                case CabinDrawNumber.Draw8WFlipper81:
                    return SynthesisTypeOfOpening.Flipper;

                //Without Opening
                case CabinDrawNumber.Draw2Corner8W82:
                case CabinDrawNumber.Draw1Corner8W84:
                case CabinDrawNumber.Draw2Straight8W85:
                case CabinDrawNumber.Draw2CornerStraight8W88:
                case CabinDrawNumber.Draw8W:
                case CabinDrawNumber.DrawE:
                case CabinDrawNumber.Draw9C:
                case CabinDrawNumber.Draw9C9F:
                case CabinDrawNumber.DrawMV2:
                case CabinDrawNumber.DrawNV2:
                case CabinDrawNumber.DrawNV:
                case CabinDrawNumber.Draw8W40:
                case CabinDrawNumber.None:
                default:
                    return SynthesisTypeOfOpening.None;
            }
        }

        //public override bool Equals(object obj)
        //{
        //    if ((obj is null) || this.GetType().Equals(obj.GetType()) is false)
        //    {
        //        return false;
        //    }
        //    CabinSynthesis synthesis = obj as CabinSynthesis ?? throw new NullReferenceException("Equality Object is Null");
        //    if ((NumberOfSides != synthesis.NumberOfSides) || (DrawNo != synthesis.DrawNo))
        //    {
        //        return false;
        //    }

        //    return NumberOfSides switch
        //    {
        //        0 => true,
        //        1 => (Primary.Equals(synthesis.Primary)),
        //        2 => (Primary.Equals(synthesis.Primary) && Secondary.Equals(synthesis.Secondary)),
        //        3 => (Primary.Equals(synthesis.Primary) && Secondary.Equals(synthesis.Secondary) && Tertiary.Equals(synthesis.Tertiary)),
        //        _ => false,
        //    };
        //}

        /// <summary>
        /// Returns the HashCode for this Item
        /// </summary>
        /// <returns></returns>
        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(Primary?.GetHashCode(), Secondary?.GetHashCode(), Tertiary?.GetHashCode(),DrawNo);
        //}

        public CabinSynthesis GetDeepClone()
        {
            CabinSynthesis clone = new();
            clone.DrawNo = this.DrawNo;
            clone.Primary = this.Primary?.GetDeepClone() ?? null;
            clone.Secondary = this.Secondary?.GetDeepClone() ?? null;
            clone.Tertiary = this.Tertiary?.GetDeepClone() ?? null;
            return clone;
        }
    }
}

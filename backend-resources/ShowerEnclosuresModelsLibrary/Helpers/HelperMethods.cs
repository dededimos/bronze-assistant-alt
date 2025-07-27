using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Helpers
{
    public static class HelperMethods
    {
        /// <summary>
        /// Returns the Description of the Value of the Enumerator
        /// If no Value is Assigned , the Default Value of "0" is Assumed and its Description Returned
        /// If there is no Description at the given value an Exception is Thrown
        /// </summary>
        /// <param name="enumeratorInstance">The Enumerator - If no Value is Assigned , the Default Value of "0" is Assumed</param>
        /// <returns>string</returns>
        public static string GetEnumDescription(Enum enumeratorInstance)
        {
            string descriptionstring = ""; //The string of the Description Attribute (.Description)
            DescriptionAttribute description = Attribute.GetCustomAttribute(
                enumeratorInstance.GetType().GetField(enumeratorInstance.ToString())! //Must Get the Type of Enum and then the Field which contains the Value for which we need the Description (The Value is enumeratorInstance.ToString())
                , typeof(DescriptionAttribute)                                       //The Attribute that we want to get
                ) as DescriptionAttribute;                                           //Because Get Attribute retrieves an Attribute Instace we have to cast to a DescriptionAttribute
            if (description != null) //If the Value of the Enumerator HAS a description attribute 
            {
                descriptionstring = description.Description;
            }
            else //If it does not have a description attribute , we should throw an exception mentioning it
            {
                throw new MissingMemberException($"Missing Description Attribute in {enumeratorInstance.GetType().ToString()} Value");
            }

            return descriptionstring;
        }

        /// <summary>
        /// Returns the Oposite of a direction or UndefinedDirecton
        /// </summary>
        /// <param name="direction">The Direction</param>
        /// <returns>The Opposite direction</returns>
        public static CabinDirection ChangeDirection(CabinDirection direction)
        {
            CabinDirection newDirection = CabinDirection.Undefined;
            if (direction is CabinDirection.LeftSided)
            {
                newDirection = CabinDirection.RightSided;
            }
            else if (direction is CabinDirection.RightSided)
            {
                newDirection = CabinDirection.LeftSided;
            }
            
            return newDirection;
        }

        /// <summary>
        /// Matches the Current Draw Number with the Needed Model
        /// </summary>
        public readonly static Dictionary<CabinDrawNumber , (CabinModelEnum?, CabinModelEnum?, CabinModelEnum?)> CabinDrawsDictionary = new()
        {
            { CabinDrawNumber.None,                     (null,null,null)},
            { CabinDrawNumber.Draw9S,                   (CabinModelEnum.Model9S, null, null) },
            { CabinDrawNumber.Draw9S9F,                 (CabinModelEnum.Model9S, CabinModelEnum.Model9F, null) },
            { CabinDrawNumber.Draw9S9F9F,               (CabinModelEnum.Model9S, CabinModelEnum.Model9F, CabinModelEnum.Model9F) },
            { CabinDrawNumber.Draw94,                   (CabinModelEnum.Model94, null, null) },
            { CabinDrawNumber.Draw949F,                 (CabinModelEnum.Model94, CabinModelEnum.Model9F, null) },
            { CabinDrawNumber.Draw949F9F,               (CabinModelEnum.Model94, CabinModelEnum.Model9F, CabinModelEnum.Model9F) },
            { CabinDrawNumber.Draw9A,                   (CabinModelEnum.Model9A, CabinModelEnum.Model9A, null) },
            { CabinDrawNumber.Draw9A9F,                 (CabinModelEnum.Model9A, CabinModelEnum.Model9A, CabinModelEnum.Model9F) },
            { CabinDrawNumber.Draw9C,                   (CabinModelEnum.Model9C, CabinModelEnum.Model9C, null) },
            { CabinDrawNumber.Draw9C9F,                 (CabinModelEnum.Model9C, CabinModelEnum.Model9C, CabinModelEnum.Model9F) },
            { CabinDrawNumber.Draw9B,                   (CabinModelEnum.Model9B, null, null) },
            { CabinDrawNumber.Draw9B9F,                 (CabinModelEnum.Model9B, CabinModelEnum.Model9F, null) },
            { CabinDrawNumber.Draw9B9F9F,               (CabinModelEnum.Model9B, CabinModelEnum.Model9F, CabinModelEnum.Model9F) },
            { CabinDrawNumber.DrawVS,                   (CabinModelEnum.ModelVS, null, null) },
            { CabinDrawNumber.DrawVSVF,                 (CabinModelEnum.ModelVS, CabinModelEnum.ModelVF, null) },
            { CabinDrawNumber.DrawV4,                   (CabinModelEnum.ModelV4, null, null) },
            { CabinDrawNumber.DrawV4VF,                 (CabinModelEnum.ModelV4, CabinModelEnum.ModelVF, null) },
            { CabinDrawNumber.DrawVA,                   (CabinModelEnum.ModelVA, CabinModelEnum.ModelVA, null) },
            { CabinDrawNumber.DrawWS,                   (CabinModelEnum.ModelWS, null, null) },
            { CabinDrawNumber.DrawNP44,                 (CabinModelEnum.ModelNP, null, null) },
            { CabinDrawNumber.Draw2CornerNP46,          (CabinModelEnum.ModelNP, CabinModelEnum.ModelNP, null) },
            { CabinDrawNumber.Draw2StraightNP48,        (CabinModelEnum.ModelNP, CabinModelEnum.ModelNP, null) },
            { CabinDrawNumber.DrawCornerNP6W45,         (CabinModelEnum.ModelNP, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.DrawStraightNP6W47,       (CabinModelEnum.ModelNP, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.DrawNB31,                 (CabinModelEnum.ModelNB, null, null) },
            { CabinDrawNumber.DrawCornerNB6W32,         (CabinModelEnum.ModelNB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2CornerNB33,          (CabinModelEnum.ModelNB, CabinModelEnum.ModelNB, null) },
            { CabinDrawNumber.DrawStraightNB6W38,       (CabinModelEnum.ModelNB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2StraightNB41,        (CabinModelEnum.ModelNB, CabinModelEnum.ModelNB, null) },
            { CabinDrawNumber.DrawDB51,                 (CabinModelEnum.ModelDB, null, null) },
            { CabinDrawNumber.DrawCornerDB8W52,         (CabinModelEnum.ModelDB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2CornerDB53,          (CabinModelEnum.ModelDB, CabinModelEnum.ModelDB, null) },
            { CabinDrawNumber.DrawStraightDB8W59,       (CabinModelEnum.ModelDB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2StraightDB61,        (CabinModelEnum.ModelDB, CabinModelEnum.ModelDB, null) },
            { CabinDrawNumber.DrawHB34,                 (CabinModelEnum.ModelHB, null, null) },
            { CabinDrawNumber.DrawCornerHB8W35,         (CabinModelEnum.ModelHB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2CornerHB37,          (CabinModelEnum.ModelHB, CabinModelEnum.ModelHB, null) },
            { CabinDrawNumber.DrawStraightHB8W40,       (CabinModelEnum.ModelHB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2StraightHB43,        (CabinModelEnum.ModelHB, CabinModelEnum.ModelHB, null) },
            { CabinDrawNumber.Draw8W,                   (CabinModelEnum.ModelW, null, null) },
            { CabinDrawNumber.DrawE,                    (CabinModelEnum.ModelE, null, null) },
            { CabinDrawNumber.Draw8WFlipper81,          (CabinModelEnum.ModelW, CabinModelEnum.ModelWFlipper, null) },
            { CabinDrawNumber.Draw2Corner8W82,          (CabinModelEnum.ModelW, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw1Corner8W84,          (CabinModelEnum.ModelW, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2Straight8W85,        (CabinModelEnum.ModelW, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2CornerStraight8W88,  (CabinModelEnum.ModelW, CabinModelEnum.ModelW, CabinModelEnum.ModelW) },
            { CabinDrawNumber.Draw8W40,                 (CabinModelEnum.ModelW, null, null) },
            { CabinDrawNumber.DrawNV,                   (CabinModelEnum.ModelNV, null, null) },
            { CabinDrawNumber.DrawNV2,                  (CabinModelEnum.ModelNV2, null, null) },
            { CabinDrawNumber.DrawMV2,                  (CabinModelEnum.ModelMV2, null, null) },
            { CabinDrawNumber.Draw9F,                   (CabinModelEnum.Model9F,null,null)},
            { CabinDrawNumber.DrawVF,                   (CabinModelEnum.ModelVF,null,null)},
            { CabinDrawNumber.DrawQP44,                 (CabinModelEnum.ModelQP, null, null) },
            { CabinDrawNumber.Draw2CornerQP46,          (CabinModelEnum.ModelQP, CabinModelEnum.ModelQP, null) },
            { CabinDrawNumber.Draw2StraightQP48,        (CabinModelEnum.ModelQP, CabinModelEnum.ModelQP, null) },
            { CabinDrawNumber.DrawCornerQP6W45,         (CabinModelEnum.ModelQP, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.DrawStraightQP6W47,       (CabinModelEnum.ModelQP, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.DrawQB31,                 (CabinModelEnum.ModelQB, null, null) },
            { CabinDrawNumber.DrawCornerQB6W32,         (CabinModelEnum.ModelQB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2CornerQB33,          (CabinModelEnum.ModelQB, CabinModelEnum.ModelQB, null) },
            { CabinDrawNumber.DrawStraightQB6W38,       (CabinModelEnum.ModelQB, CabinModelEnum.ModelW, null) },
            { CabinDrawNumber.Draw2StraightQB41,        (CabinModelEnum.ModelQB, CabinModelEnum.ModelQB, null) },
        };

        /// <summary>
        /// Weather this model is a Sliding One
        /// </summary>
        /// <param name="model">The Model</param>
        /// <returns>True if it is , False if it is not</returns>
        public static bool IsSlidingModel(this CabinModelEnum? model)
        {
            if (model is null)
            {
                return false;
            }

            var nonNullModel = (CabinModelEnum)model;
            switch (nonNullModel)
            {
                case CabinModelEnum.Model9A:
                case CabinModelEnum.Model9S:
                case CabinModelEnum.Model94:
                case CabinModelEnum.ModelVS:
                case CabinModelEnum.ModelV4:
                case CabinModelEnum.ModelVA:
                case CabinModelEnum.ModelWS:
                case CabinModelEnum.Model9C:
                    return true;
                default:
                    return false;
            }

        }

        /// <summary>
        /// Weather this Draw is Straight - Closed and Consists of only two Cabins
        /// </summary>
        /// <param name="draw">The Draw</param>
        /// <returns>True if it is , false if it is not</returns>
        public static bool IsStraight2PieceClosedDraw(this CabinDrawNumber draw)
        {
            switch (draw)
            {
                case CabinDrawNumber.Draw2StraightNP48:
                case CabinDrawNumber.Draw2StraightQP48:
                case CabinDrawNumber.DrawStraightNP6W47:
                case CabinDrawNumber.DrawStraightQP6W47:
                case CabinDrawNumber.DrawStraightNB6W38:
                case CabinDrawNumber.DrawStraightQB6W38:
                case CabinDrawNumber.Draw2StraightNB41:
                case CabinDrawNumber.Draw2StraightQB41:
                case CabinDrawNumber.DrawStraightDB8W59:
                case CabinDrawNumber.Draw2StraightDB61:
                case CabinDrawNumber.DrawStraightHB8W40:
                case CabinDrawNumber.Draw2StraightHB43:
                case CabinDrawNumber.Draw8WFlipper81:
                    return true;
                default:
                    return false;
            }
        }
    }
}

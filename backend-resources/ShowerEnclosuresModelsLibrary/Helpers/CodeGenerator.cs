using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.SupportsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.HBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Helpers
{
    public static class CodeGenerator
    {
        //Dictionaries Matching Code to Various Properties
        private static readonly Dictionary<CabinModelEnum, string> ModelToCode = new()
        {
            { CabinModelEnum.Model9A, "9A" },
            { CabinModelEnum.Model9S, "9S" },
            { CabinModelEnum.Model94, "94" },
            { CabinModelEnum.Model9F, "9F" },
            { CabinModelEnum.Model9B, "9B" },
            { CabinModelEnum.ModelW, "W" },
            { CabinModelEnum.ModelWS, "WS" },
            { CabinModelEnum.ModelHB, "HB" },
            { CabinModelEnum.ModelNP, "NP" },
            { CabinModelEnum.ModelVS, "VS" },
            { CabinModelEnum.ModelVF, "VF" },
            { CabinModelEnum.ModelV4, "V4" },
            { CabinModelEnum.ModelVA, "VA" },
            { CabinModelEnum.ModelE, "E" },
            { CabinModelEnum.ModelDB, "DB" },
            { CabinModelEnum.ModelNB, "NB" },
            { CabinModelEnum.ModelMV2, "MV" },
            { CabinModelEnum.ModelNV, "NV" },
            { CabinModelEnum.ModelNV2, "NV" },
            { CabinModelEnum.Model9C, "9C" },
            { CabinModelEnum.Model6WA, "6W" },
            { CabinModelEnum.ModelWFlipper, "FL" },
            { CabinModelEnum.Model8W40, "W"},
            { CabinModelEnum.ModelGlassContainer,"GG" },
            { CabinModelEnum.ModelQB, "QB" },
            { CabinModelEnum.ModelQP, "QP" },
        };
        private static readonly Dictionary<CabinFinishEnum, string> MetalFinishToCode = new()
        {
            { CabinFinishEnum.Polished, "1" },
            { CabinFinishEnum.Brushed, "B" },
            { CabinFinishEnum.BlackMat, "M" },
            { CabinFinishEnum.WhiteMat, "A" },
            { CabinFinishEnum.Bronze, "4" },
            { CabinFinishEnum.BrushedGold, "G" },
            { CabinFinishEnum.Gold, "2" },
            { CabinFinishEnum.Copper, "5" },
            { CabinFinishEnum.Special, "E" },
            { CabinFinishEnum.NotSet, "*" }
        };
        private static readonly Dictionary<GlassFinishEnum, string> GlassFinishToCode = new()
        {
            { GlassFinishEnum.GlassFinishNotSet, "#" },
            { GlassFinishEnum.Transparent, "0" },
            { GlassFinishEnum.Satin, "A" },
            { GlassFinishEnum.Frosted, "F" },
            { GlassFinishEnum.Fume, "M" },
            { GlassFinishEnum.Serigraphy, "S" },
            { GlassFinishEnum.Special, "E" },
        };

        /// <summary>
        /// Generates Cabin Code from its Properties
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns></returns>
        public static string GenerateCabinCode(Cabin cabin)
        {
            //Code Pattern : sXXLL-FG-HHS
            string code = "";
            string specialCharStart = "";    //s
            string modelString = "";         //XX
            string lengthString = "";         //LL
            string metalFinishString = "";   //F
            string glassFinishString = "";   //G
            string heightString = "";        //HH
            string specialCharEnd = "";      //S

            #region 1.Generate Code of Model XX
            //Generate Model Code
            if (cabin.Model != null)
            {
                modelString = ModelToCode[(CabinModelEnum)cabin.Model];
            }
            else
            {
                modelString = "**";
            }
            #endregion

            #region 2.Generate Code of Length LL
            //Generate Length Code
            decimal length = cabin.NominalLength / 10;    //Length to cm
            length = decimal.Round(length);             //Round the Decimal Point
            lengthString = length.ToString();           //Convert to String
            if (length >= 100)
            {
                if (cabin.Model is CabinModelEnum.ModelNV2 or CabinModelEnum.ModelMV2)
                {
                    //firstGlass + secondGlass = length -- firstGlass - SecondGlass = Difference of Glasses 20cm
                    //Solve as X1 = (length + difference) /2
                    decimal firstGlass = (length + 20) / 2;
                    decimal secondGlass = (length - firstGlass);
                    lengthString = $"{firstGlass.ToString().Substring(0, 1)}{secondGlass.ToString().Substring(0,1)}";
                }
                else
                {
                    lengthString = lengthString.Substring(1);              //Start from second charachter Index[1] (Trim the Hundreands)
                }
                
            }
            #endregion

            #region 3.Generate Code of Metal Finish F (From Dictionary)
            //Generate Metal Finish Code
            if (cabin.MetalFinish != null)
            {
                metalFinishString = MetalFinishToCode[(CabinFinishEnum)cabin.MetalFinish];
            }
            else
            {
                metalFinishString = "*";
            }
            #endregion

            #region 4.Generate Code of Glass Finish G (From Dictionary)
            //Generate Code of Glass Finish
            if (cabin.GlassFinish != null)
            {
                glassFinishString = GlassFinishToCode[(GlassFinishEnum)cabin.GlassFinish];
            }
            else
            {
                glassFinishString = "*";
            }
            #endregion

            #region 5.Generate Code of Height HH
            //Generate Height Code
            decimal height = cabin.Height / 10;   //Height to cm
            height = decimal.Round(height);       //Round the Decimal Point
            heightString = height.ToString();     //Convert to String
            if (height >= 100)
            {
                heightString = heightString.Substring(1);        //Start from second charachter Index[1] (Trim the Hundreands)
            }
            #endregion

            #region 6.Generate Special Start Charachter (Valid for W and E Models Only)

            //Generate special Start Charachter (Glass Thickness of W Models)
            if (cabin.Model is CabinModelEnum.ModelW || cabin.Model is CabinModelEnum.ModelE || cabin.Model is CabinModelEnum.Model8W40)
            {
                switch (cabin.Thicknesses)
                {
                    case CabinThicknessEnum.Thick5mm:
                        specialCharStart = "5";
                        break;
                    case CabinThicknessEnum.Thick6mm:
                        specialCharStart = "6";
                        break;
                    case CabinThicknessEnum.Thick8mm:
                        specialCharStart = "8";
                        break;
                    case CabinThicknessEnum.Thick10mm:
                        specialCharStart = "0";
                        break;
                    case CabinThicknessEnum.ThickTenplex10mm:
                        specialCharStart = "0";
                        break;
                    default:
                        specialCharStart = "";
                        break;
                }
            }
            #endregion

            #region 7.Generate Special End Charachter (Valid for Tenplex in W,INOX && for 9A or 8mm B6000 && for W with Supports)

            //Generate special End Charachter (TenPlexGlass for W or 8mm Glass for B6000 or 10mm Glass for V or W With Supports or NV2 for A in the End)
            switch (cabin.Model)
            {
                case CabinModelEnum.Model9A:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick8mm)
                    {
                        specialCharEnd = "28";
                    }
                    else if (cabin.Thicknesses is CabinThicknessEnum.Thick6mm8mm)
                    {
                        specialCharEnd = "268";
                    }
                    else
                    {
                        specialCharEnd = "2";
                    }
                    break;
                case CabinModelEnum.Model9S:
                case CabinModelEnum.Model94:
                case CabinModelEnum.Model9F:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick8mm)
                    {
                        specialCharEnd = "8";
                    }
                    else if (cabin.Thicknesses is CabinThicknessEnum.Thick6mm8mm)
                    {
                        specialCharEnd = "68";
                    }
                    break;
                case CabinModelEnum.ModelVS:
                case CabinModelEnum.ModelVF:
                case CabinModelEnum.ModelV4:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick10mm)
                    {
                        specialCharEnd = "1";
                    }
                    break;
                case CabinModelEnum.ModelE:
                    CabinE cabinE = (CabinE)cabin;
                    if (cabinE.Thicknesses == CabinThicknessEnum.ThickTenplex10mm)
                    {
                        specialCharEnd += "1";
                    }
                    break;
                case CabinModelEnum.ModelW: //R Appended for WallSupports , 1 Appended for TenPlex
                    CabinW cabinW = (CabinW)cabin;
                    if (cabinW.Parts.WallSideFixer is CabinSupport)
                    {
                        specialCharEnd += "R";
                    }
                    if (cabinW.Thicknesses == CabinThicknessEnum.ThickTenplex10mm)
                    {
                        specialCharEnd += "1";
                    }
                    break;
                case CabinModelEnum.ModelVA:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick10mm)
                    {
                        specialCharEnd = "21";
                    }
                    else
                    {
                        specialCharEnd = "2";
                    }
                    break;
                case CabinModelEnum.ModelHB:
                case CabinModelEnum.ModelDB:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick10mm)
                    {
                        specialCharEnd += "1";
                    }
                    break;
                case CabinModelEnum.ModelNV2:
                    specialCharEnd += "A";
                    break;
                case CabinModelEnum.ModelNP:
                case CabinModelEnum.ModelQP:
                case CabinModelEnum.ModelWFlipper:
                case CabinModelEnum.ModelNB:
                case CabinModelEnum.ModelQB:
                case CabinModelEnum.ModelNV:
                case CabinModelEnum.ModelMV2:
                case CabinModelEnum.Model6WA:
                case CabinModelEnum.Model9C:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick6mm8mm)
                    {
                        specialCharEnd += "8";
                    }
                    break;
                case CabinModelEnum.Model9B:
                default:
                    break;
            }
            #endregion

            code = $"{specialCharStart}{modelString}{lengthString}-{metalFinishString}{glassFinishString}-{heightString}{specialCharEnd}";

            return code;
        }

        /// <summary>
        /// Transforms the Code of a Cabin to its Three Digit Dimensions Code (9S150-10-200)
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns></returns>
        public static string GenerateThreeDigitDimensionsCode(Cabin cabin)
        {
            //Code Pattern : sXXLL-FG-HHS
            string code = "";
            string specialCharStart = "";    //s
            string modelString = "";         //XX
            string lengthString = "";         //LL
            string metalFinishString = "";   //F
            string glassFinishString = "";   //G
            string heightString = "";        //HH
            string specialCharEnd = "";      //S

            #region 1.Generate Code of Model XX
            //Generate Model Code
            if (cabin.Model != null)
            {
                modelString = ModelToCode[(CabinModelEnum)cabin.Model];
                //change the model string with an extra 8 if the model is B6000 with 8mm glass
                if (cabin.Series == Enums.CabinCategories.CabinSeries.Bronze6000 && cabin.Thicknesses == CabinThicknessEnum.Thick8mm)
                {
                    modelString += "8";
                }
            }
            else
            {
                modelString = "**";
            }
            #endregion

            #region 2.Generate Code of Length LL
            //Generate Length Code
            decimal length = cabin.NominalLength / 10;    //Length to cm
            length = decimal.Round(length);             //Round the Decimal Point
            lengthString = length.ToString();           //Convert to String
            if (length >= 100)
            {
                if (cabin.Model is CabinModelEnum.ModelNV2 or CabinModelEnum.ModelMV2)
                {
                    //firstGlass + secondGlass = length -- firstGlass - SecondGlass = Difference of Glasses 20cm
                    //Solve as X1 = (length + difference) /2
                    decimal firstGlass = (length + 20) / 2;
                    decimal secondGlass = (length - firstGlass);
                    lengthString = $"{firstGlass.ToString().Substring(0, 1)}{secondGlass.ToString().Substring(0, 1)}";
                }
                //else do nothing it will place 3 charachters for length
            }
            #endregion

            #region 3.Generate Code of Metal Finish F (From Dictionary)
            //Generate Metal Finish Code
            if (cabin.MetalFinish != null)
            {
                metalFinishString = MetalFinishToCode[(CabinFinishEnum)cabin.MetalFinish];
            }
            else
            {
                metalFinishString = "*";
            }
            #endregion

            #region 4.Generate Code of Glass Finish G (From Dictionary)
            //Generate Code of Glass Finish
            if (cabin.GlassFinish != null)
            {
                glassFinishString = GlassFinishToCode[(GlassFinishEnum)cabin.GlassFinish];
            }
            else
            {
                glassFinishString = "*";
            }
            #endregion

            #region 5.Generate Code of Height HH
            //Generate Height Code
            decimal height = cabin.Height / 10;   //Height to cm
            height = decimal.Round(height);       //Round the Decimal Point
            heightString = height.ToString();     //Convert to String

            //Do nothing height is 3 charachters or less if the division gave a number below
            //if (height >= 100)
            //{
            //    heightString = heightString.Substring(1);        //Start from second charachter Index[1] (Trim the Hundreands)
            //}
            #endregion

            #region 6.Generate Special Start Charachter (Valid for W and E Models Only)

            //Generate special Start Charachter (Glass Thickness of W Models)
            if (cabin.Model is CabinModelEnum.ModelW || cabin.Model is CabinModelEnum.ModelE || cabin.Model is CabinModelEnum.Model8W40)
            {
                switch (cabin.Thicknesses)
                {
                    case CabinThicknessEnum.Thick5mm:
                        specialCharStart = "5";
                        break;
                    case CabinThicknessEnum.Thick6mm:
                        specialCharStart = "6";
                        break;
                    case CabinThicknessEnum.Thick8mm:
                        specialCharStart = "8";
                        break;
                    case CabinThicknessEnum.Thick10mm:
                        specialCharStart = "0";
                        break;
                    case CabinThicknessEnum.ThickTenplex10mm:
                        specialCharStart = "0";
                        break;
                    default:
                        specialCharStart = "";
                        break;
                }
            }
            #endregion

            #region 7.Generate Special End Charachter (Valid for Tenplex in W,INOX && for 9A or 8mm B6000 && for W with Supports)

            //Generate special End Charachter (TenPlexGlass for W or 8mm Glass for B6000 or 10mm Glass for V or W With Supports or NV2 for A in the End)
            switch (cabin.Model)
            {
                case CabinModelEnum.Model9A:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick8mm)
                    {
                        specialCharEnd = "2"; //8 is at the begining now
                    }
                    else if (cabin.Thicknesses is CabinThicknessEnum.Thick6mm8mm)
                    {
                        specialCharEnd = "268";
                    }
                    else
                    {
                        specialCharEnd = "2";
                    }
                    break;
                case CabinModelEnum.Model9S:
                case CabinModelEnum.Model94:
                case CabinModelEnum.Model9F:
                    //if (cabin.Thicknesses == CabinThicknessEnum.Thick8mm)
                    //{
                    //    specialCharEnd = "8";
                    //}
                    if (cabin.Thicknesses is CabinThicknessEnum.Thick6mm8mm)
                    {
                        specialCharEnd = "68";
                    }
                    break;
                case CabinModelEnum.ModelVS:
                case CabinModelEnum.ModelVF:
                case CabinModelEnum.ModelV4:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick10mm)
                    {
                        specialCharEnd = "1";
                    }
                    break;
                case CabinModelEnum.ModelE:
                    CabinE cabinE = (CabinE)cabin;
                    if (cabinE.Thicknesses == CabinThicknessEnum.ThickTenplex10mm)
                    {
                        specialCharEnd += "1";
                    }
                    break;
                case CabinModelEnum.ModelW: //R Appended for WallSupports , 1 Appended for TenPlex
                    CabinW cabinW = (CabinW)cabin;
                    if (cabinW.Parts.WallSideFixer is CabinSupport)
                    {
                        specialCharEnd += "R";
                    }
                    if (cabinW.Thicknesses == CabinThicknessEnum.ThickTenplex10mm)
                    {
                        specialCharEnd += "1";
                    }
                    break;
                case CabinModelEnum.ModelVA:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick10mm)
                    {
                        specialCharEnd = "21";
                    }
                    else
                    {
                        specialCharEnd = "2";
                    }
                    break;
                case CabinModelEnum.ModelHB:
                case CabinModelEnum.ModelDB:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick10mm)
                    {
                        specialCharEnd += "1";
                    }
                    break;
                case CabinModelEnum.ModelNV2:
                    specialCharEnd += "A";
                    break;
                case CabinModelEnum.ModelNP:
                case CabinModelEnum.ModelQP:
                case CabinModelEnum.ModelWFlipper:
                case CabinModelEnum.ModelNB:
                case CabinModelEnum.ModelQB:
                case CabinModelEnum.ModelNV:
                case CabinModelEnum.ModelMV2:
                case CabinModelEnum.Model6WA:
                case CabinModelEnum.Model9C:
                    if (cabin.Thicknesses == CabinThicknessEnum.Thick6mm8mm)
                    {
                        specialCharEnd += "8";
                    }
                    break;
                case CabinModelEnum.Model9B:
                default:
                    break;
            }
            #endregion

            code = $"{specialCharStart}{modelString}{lengthString}-{metalFinishString}{glassFinishString}-{heightString}{specialCharEnd}";

            return code;
        }
        public static int RoundToNearestTen(int number)
        {
            int remainder = number % 10;
            if (remainder >= 5)
            {
                return number + (10 - remainder);
            }
            else
            {
                return number - remainder;
            }
        }

    }
}

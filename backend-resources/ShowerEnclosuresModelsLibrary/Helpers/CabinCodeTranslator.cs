using CommonInterfacesBronze;
using FluentValidation.Results;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Helpers
{
    /// <summary>
    /// An Object that Gets Translates a Cabin code string into Cabin Properties / Cabin
    /// Updated an Refactored 16-06-2022
    /// </summary>
    public class CabinCodeTranslator
    {
        protected readonly CabinFactory factory;
        protected readonly ValidatorCabinCode validator = new();
        public string Code { get; private set; } = "";
        public int NominalLength { get; private set; } = 0;
        public int Height { get; private set; } = 0;
        public CabinModelEnum? Model { get; private set; } = null;
        public CabinFinishEnum? MetalFinish { get; private set; } = null;
        public GlassFinishEnum? GlassFinish { get; private set; } = null;
        public CabinThicknessEnum? Thicknesses { get; private set; } = null;

        private string firstChar = "";
        private string modelIndicator = "";
        private string lengthIndicator = "";
        private string metalFinishIndicator = "";
        private string glassFinishIndicator = "";
        private string heightIndicator = "";
        private string firstExtraChar = "";
        private string secondExtraChar = "";
        private string thirdExtraChar = "";

        /// <summary>
        /// Creates an Empty Code Translator
        /// </summary>
        public CabinCodeTranslator(CabinFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Gets the Result of the Code translation and a cabin Generated from it
        /// </summary>
        /// <returns>A cabin or Null - if the provided Code could not generate one</returns>
        /// <returns>A validation Result for the Passed Code and a Cabin if the Result was Valid otherwise Null</returns>
        public (ValidationResult result, Cabin cabin) GenerateCabin(string code)
        {
            ValidationResult result = Translate(code);
            Cabin cabin = factory.CreateCabin(Model);
            if (cabin is not null)
            {
                cabin.MetalFinish = MetalFinish ?? cabin.MetalFinish;
                cabin.Thicknesses = Thicknesses ?? cabin.Thicknesses;
                cabin.NominalLength = NominalLength is not 0 ? NominalLength : cabin.NominalLength;
                cabin.Height = Height is not 0 ? Height : cabin.Height;
                cabin.GlassFinish = GlassFinish ?? cabin.GlassFinish;
            }
            return (result, cabin);
        }

        /// <summary>
        /// Translates a string to Cabin Properties which are saved to the Translator , Resets the Translator every time it is used
        /// </summary>
        /// <param name="code">The Code string to Translate</param>
        public ValidationResult Translate(string code)
        {
            ResetTranslator();
            Code = code;
            ValidationResult result = validator.Validate(Code);

            if (result.IsValid)
            {
                DisassembleCode();
                ExtractModelFromCode();
                if (Model != null)
                {
                    ExtractLengthFromCode();
                    ExtractMetalFinishFromCode();
                    ExtractGlassFinishFromCode();
                    ExtractHeightFromCode();
                    ExtractThicknessFromCode();
                }
            }
            return result;
        }

        /// <summary>
        /// Resets the Translator to its default Values
        /// </summary>
        protected virtual void ResetTranslator()
        {
            Code = "";
            NominalLength = 0;
            Height = 0;
            Model = null;
            MetalFinish = null;
            GlassFinish = null;
            Thicknesses = null;
            firstChar = "";
            modelIndicator = "";
            lengthIndicator = "";
            metalFinishIndicator = "";
            glassFinishIndicator = "";
            heightIndicator = "";
            firstExtraChar = "";
            secondExtraChar = "";
            thirdExtraChar = "";
        }
        /// <summary>
        /// Dissasembles the Cabion Code into Parts
        /// </summary>
        /// <param name="Code">The Code of the Cabin</param>
        protected void DisassembleCode()
        {
            firstChar = Code.FirstOrDefault().ToString() ?? "";
            modelIndicator = Code.Length >= 2 ? Code.Substring(0, 2).ToLower() : "";
            lengthIndicator = Code.Length >= 4 ? Code.Substring(2, 2).ToLower() : "";
            metalFinishIndicator = Code.Length >= 6 ? Code.Substring(5, 1).ToLower() : ""; //Index 4 is a dash (5th Charachter)
            glassFinishIndicator = Code.Length >= 7 ? Code.Substring(6, 1).ToLower() : "";
            heightIndicator = Code.Length >= 10 ? Code.Substring(8, 2).ToLower() : ""; //Index 7 is a dash (8th charachter)
            firstExtraChar = Code.Length >= 11 ? Code.Substring(10, 1).ToLower() : "";
            secondExtraChar = Code.Length >= 12 ? Code.Substring(11, 1).ToLower() : "";
            thirdExtraChar = Code.Length == 13 ? Code.Substring(12, 1).ToLower() : "";
        }
        protected void ExtractModelFromCode()
        {
            switch (modelIndicator)
            {
                case "9a":
                case "9α":
                    Model = CabinModelEnum.Model9A;
                    break;
                case "9s":
                case "9σ":
                    Model = CabinModelEnum.Model9S;
                    break;
                case "94":
                    Model = CabinModelEnum.Model94;
                    break;
                case "9f":
                case "9φ":
                    Model = CabinModelEnum.Model9F;
                    break;
                case "9b":
                case "9β":
                    Model = CabinModelEnum.Model9B;
                    break;
                case "0w":
                case "ow":
                case "0ς":
                case "ος":
                case "6w":
                case "6ς":
                case "8w":
                case "8ς":
                    Model = CabinModelEnum.ModelW;
                    break;
                case "hb":
                case "ηβ":
                    Model = CabinModelEnum.ModelHB;
                    break;
                case "np":
                case "νπ":
                    Model = CabinModelEnum.ModelNP;
                    break;
                case "vs":
                case "ωσ":
                    Model = CabinModelEnum.ModelVS;
                    break;
                case "vf":
                case "ωφ":
                    Model = CabinModelEnum.ModelVF;
                    break;
                case "v4":
                case "ω4":
                    Model = CabinModelEnum.ModelV4;
                    break;
                case "va":
                case "ωα":
                    Model = CabinModelEnum.ModelVA;
                    break;
                case "ws":
                case "ςσ":
                    Model = CabinModelEnum.ModelWS;
                    break;
                case "e8":
                case "ε8":
                case "e0":
                case "eo":
                case "ε0":
                case "εο":
                    Model = CabinModelEnum.ModelE;
                    break;
                case "fl":
                case "φλ":
                    Model = CabinModelEnum.ModelWFlipper;
                    break;
                case "db":
                case "δβ":
                    Model = CabinModelEnum.ModelDB;
                    break;
                case "nb":
                case "νβ":
                    Model = CabinModelEnum.ModelNB;
                    break;
                case "nv":
                case "νω":
                    if (firstExtraChar is "α" or "a")
                    {
                        Model = CabinModelEnum.ModelNV2;
                    }
                    else
                    {
                        Model = CabinModelEnum.ModelNV;
                    }
                    break;
                case "mv":
                case "μω":
                    Model = CabinModelEnum.ModelMV2;
                    break;
                case "9c":
                case "9ψ":
                    Model = CabinModelEnum.Model9C;
                    break;
                case "gg":
                case "γγ":
                    Model = CabinModelEnum.ModelGlassContainer;
                    break;
                case "qp":
                case ";π":
                    Model = CabinModelEnum.ModelQP;
                    break;
                case "qb":
                case ";β":
                    Model = CabinModelEnum.ModelQB;
                    break;
                default:
                    Model = null;
                    break;
            }
        }
        protected void ExtractLengthFromCode()
        {
            //Parse the String
            bool isInt = int.TryParse(lengthIndicator, out int parsedLength);

            if (isInt)
            {
                //Bring Parsed length to Proper Numbering (For below Calculations)
                parsedLength *= 10;

                //Get the Needed Length Value Depending on the Model and Typed Code
                switch (Model)
                {
                    #region 9A case
                    case CabinModelEnum.Model9A:
                        if (parsedLength >= 0 && parsedLength < 410)
                        {//Cases 1000-1400 (9A00-9A40)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 410 && parsedLength <= 990)
                        {//Cases 410-990 (9A41-9A99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region 9S case
                    case CabinModelEnum.Model9S:
                        if (parsedLength >= 0 && parsedLength < 900)
                        {//Cases 1000-1890 (9S00-9S89)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 900 && parsedLength <= 990)
                        {//Cases 410-990 (9S90-9S99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 1200;
                        }
                        break;
                    #endregion

                    #region 94 case
                    case CabinModelEnum.Model94:
                        if (parsedLength >= 0 && parsedLength < 300)
                        {//Cases 2000-2290 (9400-9429)
                            NominalLength = parsedLength + 2000;
                        }
                        else if (parsedLength >= 300 && parsedLength <= 990)
                        {//Cases 1300-1990 (9430-9499)
                            NominalLength = parsedLength + 1000;
                        }
                        else
                        {
                            NominalLength = 1600;
                        }
                        break;
                    #endregion

                    #region 9F case
                    case CabinModelEnum.Model9F:
                        if (parsedLength >= 0 && parsedLength < 510)
                        {//Cases 1000-1500 (9F00-9F50)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 510 && parsedLength <= 990)
                        {//Cases 510-990 (9F51-9F99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region 9B case
                    case CabinModelEnum.Model9B:
                        if (parsedLength >= 0 && parsedLength < 700)
                        {//Cases 1000-1690 (9B00-9B69)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 700 && parsedLength <= 990)
                        {//Cases 700-990 (9B70-9B99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region W  case
                    case CabinModelEnum.ModelW:
                        if (parsedLength >= 0 && parsedLength < 410)
                        {//Cases 1000-1400 (8W00-8W40)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 410 && parsedLength <= 990)
                        {//Cases 410-990 (8W41-8W99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region HB case
                    case CabinModelEnum.ModelHB:
                        if (parsedLength >= 0 && parsedLength < 700)
                        {//Cases 1000-1690 (HB00-HB69)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 700 && parsedLength <= 990)
                        {//Cases 700-990 (HB70-HB99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 1000;
                        }
                        break;
                    #endregion

                    #region NP-QP case
                    case CabinModelEnum.ModelNP:
                    case CabinModelEnum.ModelQP:
                        if (parsedLength >= 0 && parsedLength < 550)
                        {//Cases 1000-1540 (NP00-NP54)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 550 && parsedLength <= 990)
                        {//Cases 550-990 (NP55-NP99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 700;
                        }
                        break;
                    #endregion

                    #region VS case
                    case CabinModelEnum.ModelVS:
                        if (parsedLength >= 0 && parsedLength < 900)
                        {//Cases 1000-1890 (VS00-VS89)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 900 && parsedLength <= 990)
                        {//Cases 900-990 (VS90-VS99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 1200;
                        }
                        break;
                    #endregion

                    #region VF case
                    case CabinModelEnum.ModelVF:
                        if (parsedLength >= 0 && parsedLength < 510)
                        {//Cases 1000-1500 (VF00-VF50)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 510 && parsedLength <= 990)
                        {//Cases 510-990 (VF51-VF99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region V4 case
                    case CabinModelEnum.ModelV4:
                        if (parsedLength >= 0 && parsedLength < 300)
                        {//Cases 2000-2290 (V400-V429)
                            NominalLength = parsedLength + 2000;
                        }
                        else if (parsedLength >= 300 && parsedLength <= 990)
                        {//Cases 1300-1990 (V430-V499)
                            NominalLength = parsedLength + 1000;
                        }
                        else
                        {
                            NominalLength = 1800;
                        }
                        break;
                    #endregion

                    #region VA case
                    case CabinModelEnum.ModelVA:
                        if (parsedLength >= 0 && parsedLength < 410)
                        {//Cases 1000-1400 (VA00-VA40)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 410 && parsedLength <= 990)
                        {//Cases 410-990 (VA41-VA99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region WS case
                    case CabinModelEnum.ModelWS:
                        if (parsedLength >= 0 && parsedLength < 900)
                        {//Cases 1000-1890 (WS00-WS89)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 900 && parsedLength <= 990)
                        {//Cases 900-990 (WS90-WS99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 1200;
                        }
                        break;
                    #endregion

                    #region E  case
                    case CabinModelEnum.ModelE:
                        if (parsedLength >= 0 && parsedLength < 410)
                        {//Cases 1000-1400 (E800-E840)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 410 && parsedLength <= 990)
                        {//Cases 410-990 (E841-E899)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 1000;
                        }
                        break;
                    #endregion

                    #region WFlipper case
                    case CabinModelEnum.ModelWFlipper:
                        if (parsedLength >= 0 && parsedLength < 351)
                        {
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 320;
                        }
                        break;
                    #endregion

                    #region DB-NB-QB case
                    case CabinModelEnum.ModelDB:
                    case CabinModelEnum.ModelNB:
                    case CabinModelEnum.ModelQB:
                        if (parsedLength >= 0 && parsedLength < 300)
                        {//Cases 1000-1290 (DB00-DB40)
                            NominalLength = parsedLength + 1000;
                        }
                        else if (parsedLength >= 300 && parsedLength <= 990)
                        {//Cases 300-990 (DB30-DB99)
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 600;
                        }
                        break;
                    #endregion

                    #region W case

                    case CabinModelEnum.ModelNV:
                        if (parsedLength >= 0 && parsedLength < 1001)
                        {
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    #region MV2-NV2 case
                    //Compromise
                    case CabinModelEnum.ModelMV2:
                    case CabinModelEnum.ModelNV2:
                        if (parsedLength == 750)
                        {
                            NominalLength = 1200;
                        }
                        else if (parsedLength >= 0 && parsedLength < 251)
                        {
                            NominalLength = parsedLength + 1000;

                        }
                        else if (parsedLength >= 251 && parsedLength <= 990)
                        {
                            NominalLength = parsedLength;
                        }
                        else
                        {
                            NominalLength = 1200;
                        }
                        break;
                    #endregion

                    #region 9C case
                    //Compromise
                    case CabinModelEnum.Model9C:
                        if (parsedLength >= 0 && parsedLength < 850)
                        {//Cases 1000-1400 (9A00-9A40)
                            NominalLength = 800;
                        }
                        else if (parsedLength >= 850 && parsedLength <= 990)
                        {//Cases 410-990 (9A41-9A99)
                            NominalLength = 900;
                        }
                        else
                        {
                            NominalLength = 800;
                        }
                        break;
                    #endregion

                    case CabinModelEnum.Model6WA:
                    default:
                        NominalLength = 0;
                        break;
                }
            }
            else
            {
                return;
            }
        }
        protected void ExtractMetalFinishFromCode()
        {
            switch (metalFinishIndicator)
            {
                case "1":
                    MetalFinish = CabinFinishEnum.Polished;
                    break;
                case "b":
                case "β":
                    MetalFinish = CabinFinishEnum.Brushed;
                    break;
                case "m":
                case "μ":
                    MetalFinish = CabinFinishEnum.BlackMat;
                    break;
                case "a":
                case "α":
                    MetalFinish = CabinFinishEnum.WhiteMat;
                    break;
                case "4":
                    MetalFinish = CabinFinishEnum.Bronze;
                    break;
                case "g":
                case "γ":
                    MetalFinish = CabinFinishEnum.BrushedGold;
                    break;
                case "2":
                    MetalFinish = CabinFinishEnum.Gold;
                    break;
                case "5":
                    MetalFinish = CabinFinishEnum.Copper;
                    break;
                case "e":
                case "ε":
                    MetalFinish = CabinFinishEnum.Special;
                    break;
                default:
                    MetalFinish = null;
                    break;
            }
        }
        protected void ExtractGlassFinishFromCode()
        {
            GlassFinish = glassFinishIndicator switch
            {
                "0" or "o" or "ο" => (GlassFinishEnum?)GlassFinishEnum.Transparent,
                "a" or "α" => (GlassFinishEnum?)GlassFinishEnum.Satin,
                "f" or "φ" => (GlassFinishEnum?)GlassFinishEnum.Frosted,
                "m" or "μ" => (GlassFinishEnum?)GlassFinishEnum.Fume,
                "s" or "σ" => (GlassFinishEnum?)GlassFinishEnum.Serigraphy,
                "e" or "ε" => (GlassFinishEnum?)GlassFinishEnum.Special,
                _ => null,
            };
        }
        protected void ExtractHeightFromCode()
        {
            //Parse the String
            bool isInt = int.TryParse(heightIndicator, out int parsedHeight);

            if (isInt)
            {
                //Bring Parsed length to Proper Numbering (For below Calculations)
                parsedHeight *= 10;

                if (parsedHeight >= 0 && parsedHeight < 300)
                {//Cases 2000-2300 (00-30)
                    Height = parsedHeight + 2000;
                }
                else if (parsedHeight >= 300 && parsedHeight <= 990)
                {//Cases 1300-1990 (30-99)
                    Height = parsedHeight + 1000;
                }
                else
                {
                    Height = 0;
                }
            }
            else
            {
                Height = 0;
            }
        }
        protected void ExtractThicknessFromCode()
        {

            string endSpecialCharachters = firstExtraChar + secondExtraChar + thirdExtraChar;

            switch (Model)
            {
                case CabinModelEnum.Model9A:
                    if (endSpecialCharachters is "28" or "8")
                    {
                        Thicknesses = CabinThicknessEnum.Thick8mm;
                    }
                    else if (endSpecialCharachters is "268" or "68")
                    {
                        Thicknesses = CabinThicknessEnum.Thick6mm8mm;
                    }
                    else
                    {
                        Thicknesses = CabinThicknessEnum.Thick6mm;
                    }
                    break;
                case CabinModelEnum.Model9S:
                case CabinModelEnum.Model94:
                case CabinModelEnum.Model9F:
                    if (endSpecialCharachters is "8")
                    {
                        Thicknesses = CabinThicknessEnum.Thick8mm;
                    }
                    else if (endSpecialCharachters is "68")
                    {
                        Thicknesses = CabinThicknessEnum.Thick6mm8mm;
                    }
                    else
                    {
                        Thicknesses = CabinThicknessEnum.Thick6mm;
                    }
                    break;
                case CabinModelEnum.Model9B:
                case CabinModelEnum.ModelNP:
                case CabinModelEnum.ModelNB:
                case CabinModelEnum.ModelQP:
                case CabinModelEnum.ModelQB:
                case CabinModelEnum.ModelNV:
                case CabinModelEnum.ModelMV2:
                case CabinModelEnum.ModelNV2:
                case CabinModelEnum.Model6WA:
                case CabinModelEnum.Model9C:
                case CabinModelEnum.ModelWFlipper:
                    Thicknesses = CabinThicknessEnum.Thick6mm;
                    break;
                case CabinModelEnum.ModelW:
                    if (endSpecialCharachters is "1" or "r1" or "ρ1")
                    {
                        Thicknesses = CabinThicknessEnum.ThickTenplex10mm;
                    }
                    else if (firstChar is "0" or "o" or "ο")
                    {
                        Thicknesses = CabinThicknessEnum.Thick10mm;
                    }
                    else if (firstChar is "6")
                    {
                        Thicknesses = CabinThicknessEnum.Thick6mm;
                    }
                    else //Case 8mm
                    {
                        Thicknesses = CabinThicknessEnum.Thick8mm;
                    }
                    break;
                case CabinModelEnum.ModelHB:
                case CabinModelEnum.ModelE:
                case CabinModelEnum.ModelDB:
                case CabinModelEnum.ModelVF:
                    if (endSpecialCharachters is "1")
                    {
                        Thicknesses = CabinThicknessEnum.Thick10mm;
                    }
                    else
                    {
                        Thicknesses = CabinThicknessEnum.Thick8mm;
                    }
                    break;
                case CabinModelEnum.ModelVS:
                case CabinModelEnum.ModelV4:
                case CabinModelEnum.ModelVA:
                    if (endSpecialCharachters is "1")
                    {
                        Thicknesses = CabinThicknessEnum.Thick10mm;
                    }
                    else if (endSpecialCharachters is "81")
                    {
                        Thicknesses = CabinThicknessEnum.Thick8mm10mm;
                    }
                    else
                    {
                        Thicknesses = CabinThicknessEnum.Thick8mm;
                    }
                    break;
                case CabinModelEnum.ModelWS:
                    Thicknesses = CabinThicknessEnum.Thick8mm10mm;
                    break;
                default:
                    Thicknesses = null;
                    break;
            }
        }
    }

    public class CabinTranslationResult : IDeepClonable<CabinTranslationResult>
    {
        public string Code { get; set; } = "";
        public int NominalLength { get; set; } = 0;
        public int Height { get; set; } = 0;
        public CabinModelEnum? Model { get; set; } = null;
        public CabinFinishEnum? MetalFinish { get; set; } = null;
        public GlassFinishEnum? GlassFinish { get; set; } = null;
        public CabinThicknessEnum? Thicknesses { get; set; } = null;

        public CabinTranslationResult GetDeepClone()
        {
            return this.MemberwiseClone() as CabinTranslationResult;
        }
    }



}


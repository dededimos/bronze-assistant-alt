using FluentValidation.Results;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using static ShowerEnclosuresModelsLibrary.Helpers.HelperMethods;

namespace ShowerEnclosuresModelsLibrary.Helpers
{
    public class SynthesisCodeTranslator : CabinCodeTranslator
    {
#nullable enable
        public SynthesisCodeTranslator(CabinFactory factory) : base(factory)
        {

        }

        public CabinTranslationResult ResultPrimary { get; set; } = new();
        public CabinTranslationResult ResultSecondary { get; set; } = new();
        public CabinTranslationResult ResultTertiary { get; set; } = new();
        public IEnumerable<CabinDrawNumber> PotentialDrawNumbers { get; set; } = new List<CabinDrawNumber>();
        public CabinDrawNumber BestMatchingDraw { get; set; } = CabinDrawNumber.None;

        /// <summary>
        /// Resets the Results and Potential Draw Numbers for this Code Translator
        /// </summary>
        private void ResetResults()
        {
            ResultPrimary = new() ;
            ResultSecondary = new();
            ResultTertiary = new();
            PotentialDrawNumbers = new List<CabinDrawNumber>();
        }

        /// <summary>
        /// Translates 1-3 Codes into a Synthesis Translation Result
        /// </summary>
        /// <param name="codes">The Codes</param>
        /// <returns>The Translation Result</returns>
        public SynthesisTranslationResult TranslateSynthesis((string primaryCode, string secondaryCode, string tertiaryCode) codes)
        {
            ResetResults();

            //Pass translation results for all three potential Cabins
            ValidationResult valCodePrimary = Translate(codes.primaryCode);
            if (valCodePrimary.IsValid) SetResult(CabinSynthesisModel.Primary);

            ValidationResult valCodeSecondary = Translate(codes.secondaryCode);
            if (valCodeSecondary.IsValid) SetResult(CabinSynthesisModel.Secondary);
            
            ValidationResult valCodesTertiary = Translate(codes.tertiaryCode);
            if (valCodesTertiary.IsValid) SetResult(CabinSynthesisModel.Tertiary);

            //Get the Potential Draw Numbers that must be Generated
            DetermineDrawNumbers();

            return new(
                ResultPrimary,
                ResultSecondary,
                ResultTertiary,
                PotentialDrawNumbers,
                BestMatchingDraw);
        }

        /// <summary>
        /// Translates 1-3 settings into a Synthesis Translation Result Rerpresenting Various Synthesis Combos
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <param name="tertiary"></param>
        /// <returns></returns>
        public SynthesisTranslationResult TranslateSynthesis(CabinSettings primary,CabinSettings? secondary , CabinSettings? tertiary)
        {
            ResetResults();

            SetResult(primary, CabinSynthesisModel.Primary);
            if(secondary != null) SetResult(secondary, CabinSynthesisModel.Secondary);
            if (tertiary != null) SetResult(tertiary, CabinSynthesisModel.Tertiary);

            DetermineDrawNumbers();
            return new(ResultPrimary,ResultSecondary,ResultTertiary,PotentialDrawNumbers, BestMatchingDraw);
        }

        /// <summary>
        /// Translates 1-3 Inputs of Code or Cabin Settings into a Synthesis Translation Result
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <param name="tertiary"></param>
        /// <returns></returns>
        public SynthesisTranslationResult TranslateSynthesis((CabinSettings? settings ,string code) primary , (CabinSettings? settings, string code) secondary , (CabinSettings? settings, string code) tertiary)
        {
            if (primary.settings != null)
            {
                SetResult(primary.settings, CabinSynthesisModel.Primary);
            }
            else if (Translate(primary.code).IsValid)
            {
                SetResult(CabinSynthesisModel.Primary);
            }

            if (secondary.settings != null)
            {
                SetResult(secondary.settings, CabinSynthesisModel.Secondary);
            }
            else if (Translate(secondary.code).IsValid)
            {
                SetResult(CabinSynthesisModel.Secondary);
            }

            if (tertiary.settings != null)
            {
                SetResult(tertiary.settings, CabinSynthesisModel.Tertiary);
            }
            else if (Translate(tertiary.code).IsValid)
            {
                SetResult(CabinSynthesisModel.Tertiary);
            }

            //Get the Potential Draw Numbers that must be Generated
            DetermineDrawNumbers();

            return new(
                ResultPrimary,
                ResultSecondary,
                ResultTertiary,
                PotentialDrawNumbers,
                BestMatchingDraw);
        }

        /// <summary>
        /// Determines wheather there are any Combinations for the Generated Models
        /// </summary>
        private void DetermineDrawNumbers()
        {
            BestMatchingDraw = CabinDrawsDictionary
                .Where(kvPair => kvPair.Value == (ResultPrimary.Model, ResultSecondary.Model, ResultTertiary.Model))
                .Select(kvPair => kvPair.Key).FirstOrDefault();
            //If there is no match because of failed input , give the next comparison with 2 models only
            if (BestMatchingDraw is CabinDrawNumber.None) BestMatchingDraw = CabinDrawsDictionary
                .Where(kvPair => kvPair.Value.Item1 == ResultPrimary.Model && kvPair.Value.Item2 == ResultSecondary.Model )
                .Select(kvPair => kvPair.Key).FirstOrDefault();
            //Lastly only with one model , if this has also failed then it remains at None
            if (BestMatchingDraw is CabinDrawNumber.None) BestMatchingDraw = CabinDrawsDictionary
                .Where(kvPair => kvPair.Value.Item1 == ResultPrimary.Model)
                .Select(kvPair => kvPair.Key).FirstOrDefault();

            //Set as PotentialDraws those that match the Model of the Primary Result (All the Draw combinations of the Primary Cabin)
            PotentialDrawNumbers = CabinDrawsDictionary
                .Where(kvPair => kvPair.Value.Item1 == ResultPrimary.Model)
                .Select(kvPair => kvPair.Key);
        }

        /// <summary>
        /// Sets the Result of a Translation for the corresponding Synthesis Model
        /// </summary>
        /// <param name="synthesisModel">The Synthesis Model</param>
        /// <exception cref="NotSupportedException">When the Synthesis model is not Recognized</exception>
        private void SetResult(CabinSynthesisModel synthesisModel)
        {
            CabinTranslationResult result = new()
            {
                Code = this.Code,
                NominalLength = this.NominalLength,
                Height = this.Height,
                Model = this.Model,
                MetalFinish = this.MetalFinish,
                GlassFinish = this.GlassFinish,
                Thicknesses = this.Thicknesses
            };

            switch (synthesisModel)
            {
                case CabinSynthesisModel.Primary:
                    ResultPrimary = result;
                    break;
                case CabinSynthesisModel.Secondary:
                    ResultSecondary = result;
                    break;
                case CabinSynthesisModel.Tertiary:
                    ResultTertiary = result;
                    break;
                default:
                    throw new NotSupportedException($"{nameof(CabinSynthesisModel)} value not recognized :{synthesisModel}");
            }
        }
        /// <summary>
        /// Sets the Result of the Translation from a Settings Object Instead from a Code Translation
        /// </summary>
        /// <param name="settings">The Cabin Settings</param>
        /// <param name="synthesisModel">The Synthesis Model</param>
        /// <exception cref="NotSupportedException"></exception>
        private void SetResult(CabinSettings settings,CabinSynthesisModel synthesisModel)
        {
            CabinTranslationResult result = new()
            {
                Code = "Undefined",
                NominalLength = settings.NominalLength,
                Height = settings.Height,
                Model = settings.Model,
                MetalFinish = settings.MetalFinish,
                GlassFinish = settings.GlassFinish,
                Thicknesses = settings.Thicknesses
            };
            switch (synthesisModel)
            {
                case CabinSynthesisModel.Primary:
                    ResultPrimary = result;
                    break;
                case CabinSynthesisModel.Secondary:
                    ResultSecondary = result;
                    break;
                case CabinSynthesisModel.Tertiary:
                    ResultTertiary = result;
                    break;
                default:
                    throw new NotSupportedException($"{nameof(CabinSynthesisModel)} value not recognized :{synthesisModel}");
            }
        }
    }

    /// <summary>
    /// Contains Models Generated from Code Translation as well as Code string Validations
    /// </summary>
    public class SynthesisTranslationResult
    {
        public CabinTranslationResult ResultPrimary { get; set; } = new();
        public CabinTranslationResult ResultSecondary { get; set; } = new();
        public CabinTranslationResult ResultTertiary { get; set; } = new();
        public IEnumerable<CabinDrawNumber> PotentialDrawNumbers { get; set; } = new List<CabinDrawNumber>();
        public CabinDrawNumber BestMatchingDraw { get; set; }

        public SynthesisTranslationResult(
            CabinTranslationResult resultPrimary,
            CabinTranslationResult resultSecondary,
            CabinTranslationResult resultTertiary,
            IEnumerable<CabinDrawNumber> potentialDraws,
            CabinDrawNumber bestMatchingDraw)
        {
            ResultPrimary = resultPrimary.GetDeepClone();
            ResultSecondary = resultSecondary.GetDeepClone();
            ResultTertiary = resultTertiary.GetDeepClone();
            BestMatchingDraw = bestMatchingDraw;
            PotentialDrawNumbers = new List<CabinDrawNumber>(potentialDraws);
        }


        /// <summary>
        /// Generates all Potential Synthesis Combinations from the PotentialDrawNumbers
        /// </summary>
        /// <returns></returns>
        public IList<CabinSynthesis> GeneratePotentialSynthesis(CabinFactory factory)
        {
            List<CabinSynthesis> synthesisList = new();
            foreach (var draw in PotentialDrawNumbers)
            {
                CabinSynthesis synthesis = factory.CreateSynthesis(draw);

                if (synthesis.Primary is not null)
                {
                    if (ResultPrimary.NominalLength is not 0) synthesis.Primary.NominalLength = ResultPrimary.NominalLength;
                    if (ResultPrimary.Height is not 0) synthesis.Primary.Height = ResultPrimary.Height;
                    if (ResultPrimary.MetalFinish is not 0 and not null) synthesis.Primary.MetalFinish = ResultPrimary.MetalFinish;
                    if (ResultPrimary.GlassFinish is not 0 and not null) synthesis.Primary.GlassFinish = ResultPrimary.GlassFinish;
                    if (ResultPrimary.Thicknesses is not 0 and not null) synthesis.Primary.Thicknesses = ResultPrimary.Thicknesses;
                }

                if (synthesis.Secondary is not null)
                {
                    if (ResultSecondary.NominalLength is not 0) synthesis.Secondary.NominalLength = ResultSecondary.NominalLength;
                    if (ResultSecondary.Height is not 0) synthesis.Secondary.Height = ResultSecondary.Height;
                    if (ResultSecondary.MetalFinish is not 0 and not null) synthesis.Secondary.MetalFinish = ResultSecondary.MetalFinish;
                    if (ResultSecondary.GlassFinish is not 0 and not null) synthesis.Secondary.GlassFinish = ResultSecondary.GlassFinish;
                    if (ResultSecondary.Thicknesses is not 0 and not null) synthesis.Secondary.Thicknesses = ResultSecondary.Thicknesses;
                }

                if (synthesis.Tertiary is not null)
                {
                    if (ResultSecondary.NominalLength is not 0) synthesis.Tertiary.NominalLength = ResultTertiary.NominalLength;
                    if (ResultSecondary.Height is not 0) synthesis.Tertiary.Height = ResultTertiary.Height;
                    if (ResultSecondary.MetalFinish is not 0 and not null) synthesis.Tertiary.MetalFinish = ResultTertiary.MetalFinish;
                    if (ResultSecondary.GlassFinish is not 0 and not null) synthesis.Tertiary.GlassFinish = ResultTertiary.GlassFinish;
                    if (ResultSecondary.Thicknesses is not 0 and not null) synthesis.Tertiary.Thicknesses = ResultTertiary.Thicknesses;
                }
                synthesisList.Add(synthesis);
            }
            return synthesisList;
        }

    }




}


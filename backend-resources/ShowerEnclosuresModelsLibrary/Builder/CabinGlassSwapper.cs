using EnumsNET;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using static ShowerEnclosuresModelsLibrary.Helpers.HelperMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommonInterfacesBronze;

namespace ShowerEnclosuresModelsLibrary.Builder
{
    //THE BUG HERE - IS THAT THE sytnhesis Under Swap and cabin fields are getting altered during the process of the swap
    //they should be readonly and for each swap other fileds or properties should exists that are cloned and produce the desired result.
#nullable enable
    /// <summary>
    /// An Object whoose job is to Swap Glasses on a cabin
    /// </summary>
    public class CabinGlassSwapper
    {
        private readonly ILogger logger;
        private CabinSynthesis? originalSynthesis;
        private CabinSynthesisModel? modelUnderSwap = null;

        /// <summary>
        /// The Synthesis which has its Glasses Swapped
        /// </summary>
        private CabinSynthesis synthesisUnderSwap = new();

        /// <summary>
        /// The Cabin which has its Glasses Swapped
        /// </summary>
        private Cabin cabin = Cabin.Empty();
        private GlassSwap swap = GlassSwap.Empty();
        private Glass glassToSwap = new();

        public CabinGlassSwapper(ILogger<CabinGlassSwapper> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Configures the Swapper with the Cabin where glasses will be Swapped and Changed
        /// </summary>
        /// <param name="cabinUnderSwap">The Cabin to Change Glasses</param>
        /// <param name="swap">The Swap Object</param>
        /// <exception cref="ArgumentException">When something is amiss in the arguments</exception>
        public void SetItemsToSwap(CabinSynthesis synthesisUnderSwap, CabinSynthesisModel modelUnderSwap, GlassSwap swap)
        {
            originalSynthesis = synthesisUnderSwap.GetDeepClone();
            this.modelUnderSwap = modelUnderSwap;
            this.swap = swap;
            ReconfigureSwapper();
        }

        /// <summary>
        /// Resets the Synthesis UnderSwap and Cabin Underswap to the Original Items
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        private void ReconfigureSwapper()
        {
            ArgumentNullException.ThrowIfNull(originalSynthesis?.Primary);
            ArgumentNullException.ThrowIfNull(modelUnderSwap);
            if (swap.IndexOfGlassToReplace == -1) throw new Exception("Glass to Replace in GlassSwap Option cannot be -1");

            // Close the original Synthesis so that the swapper can run multiple times if needed
            synthesisUnderSwap = originalSynthesis.GetDeepClone();
            // find the Cabin under Swap
            var cabinUnderSwap = synthesisUnderSwap.GetCabinList().FirstOrDefault(c => c.SynthesisModel == modelUnderSwap);
            ArgumentNullException.ThrowIfNull(cabinUnderSwap?.Model);
            if (cabinUnderSwap.Glasses.Any() is false) throw new ArgumentException($"Cabin Glasses should be Present when inserted in a Glass Swapper");
            if (cabinUnderSwap.Glasses.Count < swap.IndexOfGlassToReplace + 1) throw new ArgumentException($"The Provided Index for the Glass to Replace is not Correct , or the Structure has fewer Glasses than intended");
            cabin = cabinUnderSwap;

            glassToSwap = cabin.Glasses[swap.IndexOfGlassToReplace];
        }

        /// <summary>
        /// Executes a Glass Swap to the Cabin
        /// </summary>
        /// <param name="swapOption">The Options to Apply</param>
        /// <returns>The Swapped Cabin</returns>
        public CabinSynthesis ExecuteSwap(SwapOption swapOption)
        {
            ReconfigureSwapper();

            if (swapOption == 0)
            {
                // Do nothing
                logger.LogInformation("No Swap Done swap Option - {swapOption}", swapOption.ToString());
                return synthesisUnderSwap;
            }


            if (swapOption.HasFlag(SwapOption.SwapAdjustLengthHeightGlassesCabin))
            {
                // Add Also Height Change
                ApplyLengthDifferenceInCabinUnderSwap();
            }

            // Apply this only if the Above is NOT an OPTION
            else if (swapOption.HasFlag(SwapOption.SwapAndAdjustLengthsInStructure))
            {
                // Add Also Height Change
                ApplyLengthDifferenceInStructure();
            }

            // Apply this ONLY if Any of the Above IS NOT AN Option
            else if (swapOption.HasFlag(SwapOption.SwapKeepGlassesChangeCabinLength))
            {
                // Find the Length Diff of the Cabin
                var lengthDiff = swap.NewGlass.Length - glassToSwap.Length;
                //Apply the Diff to the Cabin
                cabin.LengthMin += Convert.ToInt32(lengthDiff);
            }
            else if (swapOption.HasFlag(SwapOption.SwapOnly))
            {
                //do nothing; will just execute below line
                logger.LogInformation("Only Swapping Glasses without other Changes");
            }

            // Apply Height Change Diffs to Cabins and Glasses
            ApplyHeightDifferences();

            // Execute the Swap
            cabin.Glasses[swap.IndexOfGlassToReplace] = swap.NewGlass;
            logger.LogInformation("Replaced Glass : {oldGlass} -- with new Glass: {newGlass}", cabin.Glasses[swap.IndexOfGlassToReplace].ToString(), swap.NewGlass.ToString());

            // return the Cabin
            return synthesisUnderSwap.GetDeepClone();
        }

        /// <summary>
        /// Applies any Height Difference to the Whole Strucutre based on the Height Diff of the Glass being Swapped
        /// </summary>
        private void ApplyHeightDifferences()
        {
            // Compare the Height Diff of the Existing Glasses with the Swapped Ones
            var heightDiff = swap.NewGlass.Height - glassToSwap.Height;
            if (heightDiff == 0) return;

            // Apply the Height-Diff to all the Cabins and Glasses
            var cabinsList = synthesisUnderSwap.GetCabinList();

            foreach (var cabin in cabinsList)
            {
                cabin.Height += Convert.ToInt32(heightDiff);
                logger.LogInformation("Applied Height Diff : {heightDiff} to Code : {code}", heightDiff, cabin.Code);
                // Apply the Diff to All Glasses Except the One under Swap
                foreach (var glass in cabin.Glasses)
                {
                    if (ReferenceEquals(glass,glassToSwap) is false)
                    {
                        glass.Height += heightDiff;
                        logger.LogInformation("Applied Height Diff : {heightDiff} to Glass {glassType}", heightDiff, glass.GlassType.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Adjustes the Length of the Glasses in the Cabin UnderSwap rather than in the Whole Structure
        /// </summary>
        /// <exception cref="Exception">When the Cabin.Model is Null</exception>
        private void ApplyLengthDifferenceInCabinUnderSwap()
        {
            logger.LogInformation("Applying Length Difference to Glasses Only of Cabin : {cabinCode}", cabin.Code);
            // Find the Length Diff of the Glass being swapped
            var lengthDiff = swap.NewGlass.Length - glassToSwap.Length;

            // According to the Type of Model Apply Changes to the Rest of the Glasses 

            var model = cabin.Model ?? throw new Exception("Cabin Model is Undefinied");
            switch (model)
            {
                case CabinModelEnum.Model9A:
                case CabinModelEnum.Model9S:
                case CabinModelEnum.Model94:
                case CabinModelEnum.ModelVS:
                case CabinModelEnum.ModelV4:
                case CabinModelEnum.ModelVA:
                case CabinModelEnum.ModelWS:
                case CabinModelEnum.ModelHB:
                case CabinModelEnum.Model9B:
                    // Apply the Diff to all Fixed Glasses
                    // Apply the inverseDiff to all Doors
                    foreach (var glass in cabin.Glasses)
                    {
                        // Go to Next Iteration
                        if (ReferenceEquals(glass, glassToSwap)) continue;

                        // If the Glasses are of the Same Type put the Inverse Difference , otherwise put the normal one
                        // This means that if we deduct from doors we add to Fixed Panels and vice versa
                        if (glass.GlassType == glassToSwap.GlassType)
                        {
                            glass.Length += +lengthDiff;
                            logger.LogInformation("Applied INVERSE Length Diff : {lengthDiff} to  Glass: {glassType} - ChangedLength:{length}", -lengthDiff, glass.GlassType.ToString(), glass.Length);
                        }
                        else
                        {
                            glass.Length += -lengthDiff;
                            logger.LogInformation("Applied INVERSE Length Diff : {lengthDiff} to  Glass: {glassType} - ChangedLength:{length}", -lengthDiff, glass.GlassType.ToString(), glass.Length);
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException($"This Model --{model}-- does not Support {SwapOption.SwapAdjustLengthHeightGlassesCabin}");
            }

        }
        /// <summary>
        /// Applies the Length Difference to the Remaining Pieces of the Structure (only valid for two Piece Structures)
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private void ApplyLengthDifferenceInStructure()
        {
            logger.LogInformation("Applying Length Difference to Whole Structure of Draw : {draw}", synthesisUnderSwap.DrawNo.ToString());
            // Find the Length Diff of the Glass being swapped
            var lengthDiff = swap.NewGlass.Length - glassToSwap.Length;


            switch (synthesisUnderSwap.DrawNo)
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
                    //Change lkength of the Cabin with the Swapped Glass
                    var cabinWithSwappedGlass = synthesisUnderSwap.GetCabinList().FirstOrDefault(c => c.SynthesisModel == cabin.SynthesisModel)
                        ?? throw new Exception("Could not Find the Cabin with the Swapped Glass , During Length Change");
                    cabinWithSwappedGlass.LengthMin += Convert.ToInt32(lengthDiff);
                    logger.LogInformation("Applied Length Diff : {lengthDiff} to Cabin : {cabinCode}", lengthDiff, cabinWithSwappedGlass.Code);

                    //Apply the Length Diff to the Other Piece of the Structure where the Glass was not initily swapped
                    var cabinToChangeLength = synthesisUnderSwap.GetCabinList().FirstOrDefault(c => c.SynthesisModel != cabin.SynthesisModel)
                        ?? throw new Exception("There was no other Model Found in the Synthesis to Change Length of the Structure");
                    cabinToChangeLength.LengthMin -= Convert.ToInt32(lengthDiff);
                    logger.LogInformation("Applied Inverse Length Diff : {lengthDiff} to Cabin : {cabinCode}", -lengthDiff, cabinToChangeLength.Code);

                    // Find the First Glass of the Cabin and Change its Length by the Defined Diff (Prefer Fixed Glasses Over Doors)
                    var glassToChangeLength = cabinToChangeLength.Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.FixedGlass);
                    if (glassToChangeLength is not null)
                    {
                        // case where there is a Fixed Glass
                        glassToChangeLength.Length -= lengthDiff;
                        logger.LogInformation("Applied Inverse Length Diff : {lengthDiff} to Fixed Glass of Cabin : {cabinCode}", -lengthDiff, cabinToChangeLength.Code);
                    }
                    else
                    {
                        // case where there is no Fixed Glass
                        glassToChangeLength = cabinToChangeLength.Glasses.FirstOrDefault()
                            ?? throw new Exception("There was a Cabin Without any Glasses");

                        // If its a NP Model Divide the Difference in the two Glasses
                        // else Pick the first Found Glass for all other models and Apply the Change there
                        if (synthesisUnderSwap.DrawNo is CabinDrawNumber.Draw2StraightNP48 
                                                      or CabinDrawNumber.Draw2StraightQP48
                                                      or CabinDrawNumber.DrawStraightNP6W47
                                                      or CabinDrawNumber.DrawStraightQP6W47)
                        {
                            glassToChangeLength.Length -= lengthDiff / 2d;
                            var secondGlassToChangeLength = cabinToChangeLength.Glasses.Skip(1).FirstOrDefault()
                                ?? throw new Exception("There where fewer than 2 Glasses on the NP Structure of the Synthesis");
                            logger.LogInformation("Applied HALF INVERSE Length Diff : {lengthDiffHalf} to Both NP DoorGlasses of Cabin : {cabinCode}", -lengthDiff / 2d, cabinToChangeLength.Code);
                            secondGlassToChangeLength.Length -= lengthDiff / 2d;
                        }
                        else
                        {
                            glassToChangeLength.Length -= lengthDiff;
                            logger.LogInformation("Applied Inverse Length Diff : {lengthDiff} to First Found Glass : {glassType} of Cabin : {cabinCode}", -lengthDiff, glassToChangeLength.GlassType.ToString(), cabinToChangeLength.Code);
                        }


                    }
                    break;
                default:
                    //Do nothing in the Rest of the Draws as their are not Straight to have their Parts Changed in Length
                    throw new NotSupportedException($"This Type of Draw --{synthesisUnderSwap.DrawNo}-- Does not Support Option :{SwapOption.SwapAndAdjustLengthsInStructure}");
            }
        }

        /// <summary>
        /// Gets the available Options for the Cabin Swap
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When the cabin.Model is null</exception>
        public SwapOption GetSwapOptions()
        {
            SwapOption swapOption = SwapOption.DoNotSwap;

            if (cabin is UndefinedCabin || swap.IndexOfGlassToReplace == -1)
            {
                return swapOption;
            }

            // All Structures have this Option Available
            swapOption = swapOption.CombineFlags(SwapOption.SwapKeepGlassesChangeCabinLength);
            swapOption = swapOption.CombineFlags(SwapOption.SwapOnly);

            bool hasDiffLength = glassToSwap.Length != swap.NewGlass.Length;

            // If there is a Diff and the Model is Sliding or HB or 9B then Unlock also the SwapAndAdjustLengthsInSwappedCabin option
            if (hasDiffLength && (cabin.Model.IsSlidingModel() || cabin.Model is CabinModelEnum.ModelHB or CabinModelEnum.Model9B))
            {
                swapOption = swapOption.CombineFlags(SwapOption.SwapAdjustLengthHeightGlassesCabin);
            }

            // Check if Structure is Straight and Can Change its Length (Like Draw No40)
            if (hasDiffLength && synthesisUnderSwap.DrawNo.IsStraight2PieceClosedDraw())
            {
                swapOption = swapOption.CombineFlags(SwapOption.SwapAndAdjustLengthsInStructure);
            }

            return swapOption;
        }

    }

    [Flags]
    /// <summary>
    /// The Type of Swap 
    /// </summary>
    public enum SwapOption
    {
        /// <summary>
        /// Do not Do Any Swaps
        /// </summary>
        DoNotSwap = 0,
        /// <summary>
        /// Adjust the Glasses to Fit the Cabin Size with Inlcuding the Swapped Glass
        /// </summary>
        SwapAdjustLengthHeightGlassesCabin = 1,
        /// <summary>
        /// Only Swap the Glass without changing length of Rest of the Glasses
        /// </summary>
        SwapKeepGlassesChangeCabinLength = 2,
        /// <summary>
        /// Swaps the Glass and Changes Lengths in the Structure Pieces Rather than in the Cabin Under Swap
        /// </summary>
        SwapAndAdjustLengthsInStructure = 4,
        //Only Swaps the Glass without Changing Anything Else
        SwapOnly = 8,
    }

    public class GlassSwap : IDeepClonable<GlassSwap>
    {
        public Glass NewGlass { get; set; }
        public int IndexOfGlassToReplace { get; set; }

        public GlassSwap(int indexOfGlassToReplace, Glass newGlass)
        {
            IndexOfGlassToReplace = indexOfGlassToReplace;
            NewGlass = newGlass;
        }

        public static GlassSwap Empty()
        {
            return new GlassSwap(-1, new());
        }

        public GlassSwap GetDeepClone()
        {
            return new(this.IndexOfGlassToReplace, NewGlass.GetDeepClone());
        }
    }
}

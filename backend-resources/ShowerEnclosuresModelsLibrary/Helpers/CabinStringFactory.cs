using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.StripsModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels.ConstraintsModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Helpers
{
    /// <summary>
    /// Contains Methods to Create a String from a Cabin . Or Deserialize a string to a Cabin
    /// </summary>
    public class CabinStringFactory
    {
        private readonly CabinFactory factory;
        private readonly ICabinMemoryRepository repo;

        /// <summary>
        /// The Fallback String value When the Value is Null
        /// </summary>
        private static readonly string fallbackValue = "null";
        /// <summary>
        /// The various Mirror Attributes Seperator
        /// </summary>
        private static readonly char seperator = '-';
        private static readonly string cabinSeperator = "ccc";
        private static readonly string cabinExtraSeperator = "exx";
        private static readonly string cabinExtraPropertySeperator = "exp";
        private static readonly string cabinPartsSeperator = "prt";
        private static readonly string cabinPartOptionSeperator = "pro";
        private static readonly string cabinPartOptionCodeIdentifier = "cd";

        public CabinStringFactory(CabinFactory factory , ICabinMemoryRepository repo)
        {
            this.factory = factory;
            this.repo = repo;
        }

        /// <summary>
        /// Generates a String from a CabinSynthesis
        /// </summary>
        /// <param name="synthesis">The Synthesis</param>
        /// <returns>The Synthesis String</returns>
        public string GenerateStringFromSynthesis(CabinSynthesis synthesis)
        {
            StringBuilder builder = new();
            if (synthesis.Primary is not null)
            {
                builder.Append(GenerateStringFromCabin(synthesis.Primary)).Append(cabinSeperator);
            }
            else
            {
                builder.Append(fallbackValue).Append(cabinSeperator);
            }

            if (synthesis.Secondary is not null)
            {
                builder.Append(GenerateStringFromCabin(synthesis.Secondary)).Append(cabinSeperator);
            }
            else
            {
                builder.Append(fallbackValue).Append(cabinSeperator);
            }

            if (synthesis.Tertiary is not null)
            {
                builder.Append(GenerateStringFromCabin(synthesis.Tertiary)).Append(cabinSeperator);
            }
            else
            {
                builder.Append(fallbackValue).Append(cabinSeperator);
            }

            //Generate Parts Options String
            
            //First Add PartsSeperator for splitting on deserialization
            builder.Append(cabinPartsSeperator);

            bool hasFrameGeneratedAlready = false;
            #region 1.Frame Option and Wall Side Fixer
            if (synthesis.GetCabinList().Any(c=> c.Parts is IPerimetricalFixer fixer && fixer.HasPerimetricalFrame))
            {
                //if one has frame append frame option and seperator
                builder.Append((int)SelectablePartsOptions.PerimetricalFrameOption)
                       .Append(cabinPartOptionSeperator);
                hasFrameGeneratedAlready = true;
            }
            //Check the Wall Side fixer only after frame is Checked and if not there
            else if (synthesis.GetCabinList().Any(c=>
                    c.Parts is IWallSideFixer fixer 
                    && fixer.WallSideFixer.Code != repo.GetDefault(c.Identifier(),PartSpot.WallSide1)))
            {
                IWallSideFixer wf = synthesis.GetCabinList().FirstOrDefault(c => c.Parts is IWallSideFixer)?.Parts as IWallSideFixer ?? throw new Exception("Could not Retrieve WallFixer , this Cabin Cannot Have One");
                //As it is not the default Handle (checked in above statement)
                builder.Append((int)SelectablePartsOptions.WallFixerOption)
                       .Append(cabinPartOptionCodeIdentifier)
                       .Append(wf.WallSideFixer.Code)
                       .Append(cabinPartOptionCodeIdentifier) //close value identifyer
                       .Append(cabinPartOptionSeperator);     //close option seperator
            }
            #endregion

            #region 2.Handle Option
            if (synthesis.GetCabinList().Any(c => c.Parts is IHandle ho && ho.Handle != null && ho.Handle.Code != repo.GetDefault(c.Identifier(),PartSpot.Handle1)))
            {
                //Get the Handle Option
                IHandle ho = synthesis.GetCabinList().FirstOrDefault(c => c.Parts is IHandle)?.Parts as IHandle ?? throw new Exception("Could not Retrieve Handle , this Cabin Cannot Have One");
                
                //As it is not the default Handle (checked in above statement)
                builder.Append((int)SelectablePartsOptions.HandleOption)
                       .Append(cabinPartOptionCodeIdentifier)
                       .Append(ho.Handle.Code)
                       .Append(cabinPartOptionCodeIdentifier) //close value identifyer
                       .Append(cabinPartOptionSeperator);     //close option seperator
            }
            #endregion

            #region 3.Bottom Fixer Option
            if (hasFrameGeneratedAlready == false && synthesis.GetCabinList().Any(c=> c.Parts is IBottomFixer fixer && fixer.BottomFixer.Code != repo.GetDefault(c.Identifier(),PartSpot.BottomSide1)))
            {
                IBottomFixer bf = synthesis.GetCabinList().FirstOrDefault(c=>c.Parts is IBottomFixer)?.Parts as IBottomFixer ?? throw new Exception("Could not Retrieve BottomFixer , this Cabin Cannot Have One");
                //As it is not the default Handle (checked in above statement)
                builder.Append((int)SelectablePartsOptions.BottomFixerOption)
                       .Append(cabinPartOptionCodeIdentifier)
                       .Append(bf.BottomFixer.Code)
                       .Append(cabinPartOptionCodeIdentifier) //close value identifyer
                       .Append(cabinPartOptionSeperator);     //close option seperator
            }
            #endregion

            #region 4.CloseProfile Option
            if (synthesis.GetCabinList().Any(c => c.Parts is ICloseProfile closer && (
            (repo.GetDefault(c.Identifier(),PartSpot.CloseProfile) != (closer.CloseProfile?.Code ?? string.Empty)) ||
            (repo.GetDefault(c.Identifier(),PartSpot.CloseStrip) != (closer.CloseStrip?.Code ?? string.Empty)) )))
            {
                ICloseProfile cp = synthesis.GetCabinList().FirstOrDefault(c => c.Parts is ICloseProfile)?.Parts as ICloseProfile ?? throw new Exception("Could not Retrieve CloseProfile/Strip , this Cabin Cannot Have One");
                //As it is not the default Handle (checked in above statement)
                builder.Append((int)SelectablePartsOptions.ClosureOption)
                       .Append(cabinPartOptionCodeIdentifier)
                       .Append(cp.CloseProfile?.Code ?? fallbackValue)
                       .Append(cabinPartOptionCodeIdentifier) //close value identifyer
                       .Append(cp.CloseStrip?.Code ?? fallbackValue)
                       .Append(cabinPartOptionCodeIdentifier) //close value identifyer
                       .Append(cabinPartOptionSeperator);     //close option seperator
            }
            #endregion

            #region 5.MiddleHinge Option
            if (synthesis.GetCabinList().Any(c => 
                c.Parts is IMiddleHinge hingeOption 
                && hingeOption.MiddleHinge.Code != repo.GetDefault(c.Identifier(),PartSpot.MiddleHinge)))
            {
                IMiddleHinge mh = synthesis.GetCabinList().FirstOrDefault(c => c.Parts is IMiddleHinge)?.Parts as IMiddleHinge ?? throw new Exception("Could not Retrieve MiddleHinge , this Cabin Cannot Have One");
                //As it is not the default Handle (checked in above statement)
                builder.Append((int)SelectablePartsOptions.MiddleHingeOption)
                       .Append(cabinPartOptionCodeIdentifier)
                       .Append(mh.MiddleHinge.Code)
                       .Append(cabinPartOptionCodeIdentifier) //close value identifyer
                       .Append(cabinPartOptionSeperator);     //close option seperator
            }
            #endregion

            //Close Parts Seperator
            builder.Append(cabinPartsSeperator);
            return builder.ToString();
        }

        /// <summary>
        /// Generates a String from a Cabin
        /// </summary>
        /// <param name="cabin">The Cabin</param>
        /// <returns>The String of the Cabin</returns>
        public string GenerateStringFromCabin(Cabin cabin)
        {
            StringBuilder builder = new();

            //1.CabinModelEnum
            builder.Append(cabin.Model is not null ? (int)cabin.Model : fallbackValue)
                   .Append(seperator);

            //2.CabinFinishEnum
            builder.Append(cabin.MetalFinish is not null ? (int)cabin.MetalFinish : fallbackValue)
                   .Append(seperator);

            //3.CabinThicknessEnum
            builder.Append(cabin.Thicknesses is not null ? (int)cabin.Thicknesses : fallbackValue)
                   .Append(seperator);
            //4.CabinGlassFinish
            builder.Append(cabin.GlassFinish is not null ? (int)cabin.GlassFinish : fallbackValue)
                   .Append(seperator);
            //5.CabinDrawNumber
            builder.Append((int)cabin.IsPartOfDraw)
                   .Append(seperator);

            //6.CabinSynthesisModel
            builder.Append((int)cabin.SynthesisModel)
                   .Append(seperator);

            //7.CabinDirection
            builder.Append((int)cabin.Direction)
                   .Append(seperator);

            //8.Nominal Length
            builder.Append(cabin.NominalLength)
                   .Append(seperator);

            //9.Height
            builder.Append(cabin.Height)
                   .Append(seperator);

            //10+Extras
            foreach (CabinExtra extra in cabin.Extras)
            {
                builder.Append((int)extra.ExtraType).Append(cabinExtraPropertySeperator);
                if (extra is StepCut step)
                {
                    builder.Append(step.StepLength).Append(cabinExtraPropertySeperator);
                    builder.Append(step.StepHeight).Append(cabinExtraSeperator);
                }
                else
                {
                    //Those two null values appear even when there is no step
                    builder.Append(fallbackValue).Append(cabinExtraPropertySeperator);
                    builder.Append(fallbackValue).Append(cabinExtraSeperator);
                }
            }

            //HACK if the Cabin Extras are 0 then the Deserilizer throws index out of Range Exception
            if (cabin.Extras.Count is 0)
            {
                builder.Append(fallbackValue).Append(cabinExtraPropertySeperator);
                builder.Append(fallbackValue).Append(cabinExtraPropertySeperator);
                builder.Append(fallbackValue).Append(cabinExtraSeperator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a Cabin from a String with Values Coming From this FactoryClass
        /// </summary>
        /// <param name="cabinString">The Cabin String</param>
        /// <returns>The Cabin or Null</returns>
        /// <exception cref="ArgumentException">Invalid String Argument</exception>
        public Cabin GenerateCabinFromString(string cabinString)
        {
            Cabin cabin;
            string[] values = cabinString.Split(seperator,StringSplitOptions.RemoveEmptyEntries);
            if (values.Length < 9) { return null; } //Return a Null Cabin
            string model = values[0];
            string metalFinish = values[1];
            string thicknesses = values[2];
            string glassFinish = values[3];
            string drawNumber = values[4];
            string synthesisModel = values[5];
            string direction = values[6];
            string nominalLength = values[7];
            string height = values[8];
            //Extract the Extras as a single String
            string extrasString = values[9];
            //Split the String to the Extras -- Extras need to be Further Split into their Properties
            string[] extras = extrasString.Split(cabinExtraSeperator,StringSplitOptions.RemoveEmptyEntries);

            //Parse the Draw Number and Synthesis Model -- If they are not Parsable throw (Cannot create a Cabin)
            if (Enum.TryParse(drawNumber, out CabinDrawNumber cabinDrawNumber) &&
                Enum.TryParse(synthesisModel, out CabinSynthesisModel cabinSynthesisModel))
            {
                cabin = factory.CreateCabin(cabinDrawNumber,cabinSynthesisModel);
                if (cabin is null) return null;
            }
            else
            {
                return null; //Cannot Create Cabin Return Null
            }
            
            //Extras to Cabin
            foreach (string extra in extras)
            {
                //Split the Properties of the Extras
                string[] extraProperties = extra.Split(cabinExtraPropertySeperator,StringSplitOptions.RemoveEmptyEntries);
                string extraTypeString = extraProperties[0];
                string stepLengthString = extraProperties[1];
                string stepHeightString = extraProperties[2];
                
                //Extract Type 
                bool isParsable = Enum.TryParse(extraTypeString, out CabinExtraType extraType);
                if (isParsable)
                {
                    cabin.AddExtra(extraType);
                    //If THIS extra is a step -- Put its Length and Height -- otherwise Ignore
                    if (cabin.HasStep && extraType is CabinExtraType.StepCut)
                    {
                        cabin.GetStepCut().StepLength = int.TryParse(stepLengthString, out int stepLength) ? stepLength : 0;
                        cabin.GetStepCut().StepHeight = int.TryParse(stepHeightString, out int stepHeight) ? stepHeight : 0;
                    }
                }
            }

            //Rest Values
            cabin.MetalFinish = Enum.TryParse(metalFinish, out CabinFinishEnum cabinMetalFinish) ? cabinMetalFinish : null;
            cabin.Thicknesses = Enum.TryParse(thicknesses, out CabinThicknessEnum cabinThicknesses) ? cabinThicknesses : null;
            
            cabin.GlassFinish = Enum.TryParse(glassFinish, out GlassFinishEnum cabinGlassFinish) ? cabinGlassFinish : null;
            
            //ToSupport Older Enum '0' Value THAT WAS at Transparent instead of not Set
            if (cabin.GlassFinish == GlassFinishEnum.GlassFinishNotSet) cabin.GlassFinish = GlassFinishEnum.Transparent;

            //If could not Parse Leave its Default From the Factory Creation
            cabin.Direction = Enum.TryParse(direction, out CabinDirection cabinDirection) ? cabinDirection : cabin.Direction;
            cabin.NominalLength = int.TryParse(nominalLength, out int cabinNominalLength) ? cabinNominalLength : 0;
            cabin.Height = int.TryParse(height, out int cabinHeight) ? cabinHeight : 0;

            return cabin;
        }

        /// <summary>
        /// Generates a Cabin Synthesis from a String -- If Primary is Null Throws
        /// </summary>
        /// <param name="cabinSynthesisString">The Cabin Synthesis String</param>
        /// <returns>a Cabin Synthesis or Throws an Exception</returns>
        /// <exception cref="ArgumentException">When String cannot Parse the Primary Cabin</exception>
        public CabinSynthesis GenerateCabinSynthesisFromString(string cabinSynthesisString)
        {
            //Seperate Parts from Cabins
            string[] partsAndCabinsString = cabinSynthesisString.Split(cabinPartsSeperator, StringSplitOptions.RemoveEmptyEntries);
            
            //Get Cabins (the should always be cabins string)
            string cabinsString = partsAndCabinsString.Length > 0 ? partsAndCabinsString[0] : string.Empty;
            
            //Get Parts -- or empty if there are none
            string partsString = partsAndCabinsString.Length > 1 ? partsAndCabinsString[1] : string.Empty;
            
            string[] cabins = cabinsString.Split(cabinSeperator,StringSplitOptions.RemoveEmptyEntries);
            if (cabins.Length < 1)
            {
                throw new ArgumentException("Invalid Argument ,string does not Contain any Cabins");
            }

            Cabin primary = GenerateCabinFromString(cabins[0]);
            Cabin secondary = GenerateCabinFromString(cabins[1]);
            Cabin tertiary = GenerateCabinFromString(cabins[2]);

            if (primary != null)
            {
                //Fix The Primary Model
                CabinSynthesis synthesis = factory.CreateSynthesis(primary.IsPartOfDraw);
                synthesis.Primary.MetalFinish = primary.MetalFinish;
                synthesis.Primary.Thicknesses = primary.Thicknesses;
                synthesis.Primary.GlassFinish = primary.GlassFinish;
                foreach (CabinExtra extra in primary.Extras)
                {
                    synthesis.Primary.AddExtra(extra.ExtraType);
                    if (extra is StepCut step)
                    {
                        synthesis.Primary.GetStepCut().StepLength = step.StepLength;
                        synthesis.Primary.GetStepCut().StepHeight = step.StepHeight;
                    }
                }
                synthesis.Primary.NominalLength = primary.NominalLength;
                synthesis.Primary.Height = primary.Height;

                //Fix the Secondary if its there
                if (secondary != null)
                {
                    synthesis.Secondary.MetalFinish = secondary.MetalFinish;
                    synthesis.Secondary.Thicknesses = secondary.Thicknesses;
                    synthesis.Secondary.GlassFinish = secondary.GlassFinish;
                    foreach (CabinExtra extra in secondary.Extras)
                    {
                        synthesis.Secondary.AddExtra(extra.ExtraType);
                        if (extra is StepCut step)
                        {
                            synthesis.Secondary.GetStepCut().StepLength = step.StepLength;
                            synthesis.Secondary.GetStepCut().StepHeight = step.StepHeight;
                        }
                    }
                    synthesis.Secondary.NominalLength = secondary.NominalLength;
                    synthesis.Secondary.Height = secondary.Height;
                }

                //Fix the Tertiary if its there
                if (tertiary != null)
                {
                    synthesis.Tertiary.MetalFinish = tertiary.MetalFinish;
                    synthesis.Tertiary.Thicknesses = tertiary.Thicknesses;
                    synthesis.Tertiary.GlassFinish = tertiary.GlassFinish;
                    foreach (CabinExtra extra in tertiary.Extras)
                    {
                        synthesis.Tertiary.AddExtra(extra.ExtraType);
                        if (extra is StepCut step)
                        {
                            synthesis.Tertiary.GetStepCut().StepLength = step.StepLength;
                            synthesis.Tertiary.GetStepCut().StepHeight = step.StepHeight;
                        }
                    }
                    synthesis.Tertiary.NominalLength = tertiary.NominalLength;
                    synthesis.Tertiary.Height = tertiary.Height;
                }

                //Fix Parts Options
                if (string.IsNullOrEmpty(partsString) is false) //non empty , prt seperator is deleted from split
                {
                    //Get Options
                    string[] partsOptions = partsString.Split(cabinPartOptionSeperator, StringSplitOptions.RemoveEmptyEntries);
                    //for each option Apply Changes Accordingly
                    foreach (var optionString in partsOptions)
                    {
                        //case PerimetricalFrame -- There is frame Option
                        if (optionString == "1")
                        {
                            var cabinsPartsWithFrame = synthesis.GetCabinList().Where(c => c.Parts is IPerimetricalFixer).Select(ca=>ca.Parts as IPerimetricalFixer);
                            if (cabinsPartsWithFrame is null || cabinsPartsWithFrame.Any() is false)
                            {
                                throw new Exception("Could not Find any Cabin in the Synthesis that can have Perimetrical Frame");
                            }
                            else
                            {
                                //Apply the Frame
                                foreach (var frame in cabinsPartsWithFrame)
                                {
                                    if (frame is null) throw new Exception("Cabin Parts Cannot be Null");

                                    //Hack Only W Takes Frame Now and we Apply this one only , by getting the Default code of its Profile
                                    string defaultWallProfileCode = repo.GetDefault(new(CabinModelEnum.ModelW, CabinDrawNumber.Draw8W, CabinSynthesisModel.Primary), PartSpot.WallSide1);
                                    frame.WallSideFixer = repo.GetProfile(defaultWallProfileCode);
                                    frame.TopFixer = repo.GetProfile(defaultWallProfileCode);
                                    frame.SideFixer = repo.GetProfile(defaultWallProfileCode);
                                    frame.BottomFixer = repo.GetProfile(defaultWallProfileCode);
                                    //Set All Corner Radius to zero
                                    synthesis.GetCabinList().Where(c => c is CabinW).ToList().ForEach(c => (c.Constraints as ConstraintsW)!.CornerRadiusTopEdge = 0);
                                }
                            }
                        }
                        //Rest Options
                        else
                        {
                            string[] optionValuesStrings = optionString.Split(cabinPartOptionCodeIdentifier, StringSplitOptions.RemoveEmptyEntries);
                            if (optionValuesStrings.Length < 1) throw new Exception("Defined Options where Zero");
                            string option = optionValuesStrings[0];
                            switch (option)
                            {
                                case "2": // HandleOption
                                    if (optionValuesStrings.Length < 2) throw new Exception("Defined Handle was not Found");
                                    var cabinsPartsWithHandle = synthesis.GetCabinList().Where(c => c.Parts is IHandle).Select(ca => ca.Parts as IHandle);
                                    if (cabinsPartsWithHandle is null || cabinsPartsWithHandle.Any() is false) throw new Exception("Could not Find any Cabin in the Synthesis that can have Handle");
                                    foreach (var ho in cabinsPartsWithHandle)
                                    {
                                        if (ho is null) throw new Exception("Cabin Parts Cannot be Null");
                                        ho.Handle = repo.GetHandle(optionValuesStrings[1]) ?? throw new Exception("Retrieved Part was not a Handle or not Found");
                                    }
                                    break;
                                case "3": // BottomFixerOption
                                    if (optionValuesStrings.Length < 2) throw new Exception("Defined BottomFixer was not Found");
                                    var cabinsPartsWithBottomFixer = synthesis.GetCabinList().Where(c => c.Parts is IBottomFixer).Select(ca => ca.Parts as IBottomFixer);
                                    if (cabinsPartsWithBottomFixer is null || cabinsPartsWithBottomFixer.Any() is false) throw new Exception("Could not Find any Cabin in the Synthesis that can have BottomFixer");
                                    foreach (var bf in cabinsPartsWithBottomFixer)
                                    {
                                        if (bf is null) throw new Exception("Cabin Parts Cannot be Null");
                                        //Needs some kind of verification its a bottom fixer... Otherwise a handle can be placed here by accident ?!
                                        bf.BottomFixer = repo.GetPart(optionValuesStrings[1]) ?? throw new Exception("Retrieved Part was not Found");
                                    }
                                    break;
                                case "4": // WallFixerOption
                                    if (optionValuesStrings.Length < 2) throw new Exception("Defined WallSideFixer was not Found");
                                    var cabinsPartsWithWallFixer = synthesis.GetCabinList().Where(c => c.Parts is IWallSideFixer).Select(ca => ca.Parts as IWallSideFixer);
                                    if (cabinsPartsWithWallFixer is null || cabinsPartsWithWallFixer.Any() is false) throw new Exception("Could not Find any Cabin in the Synthesis that can have WallSideFixer");
                                    foreach (var wf in cabinsPartsWithWallFixer)
                                    {
                                        if (wf is null) throw new Exception("Cabin Parts Cannot be Null");
                                        //Needs some kind of verification its a Wall fixer... Otherwise a handle can be placed here by accident ?!
                                        wf.WallSideFixer = repo.GetPart(optionValuesStrings[1]) ?? throw new Exception("Retrieved Part was not Found");
                                    }
                                    break;
                                case "5": // ClosureOption (always has two properties if present)
                                    if (optionValuesStrings.Length < 3) throw new Exception("Defined Closers were not Found");
                                    var cabinsPartsWithClosers = synthesis.GetCabinList().Where(c => c.Parts is ICloseProfile).Select(ca => ca.Parts as ICloseProfile);
                                    if (cabinsPartsWithClosers is null || cabinsPartsWithClosers.Any() is false) throw new Exception("Could not Find any Cabin in the Synthesis that can have CloseProfile");
                                    foreach (var cp in cabinsPartsWithClosers)
                                    {
                                        if (cp is null) throw new Exception("Cabin Parts Cannot be Null");
                                        //Needs some kind of verification its a Wall fixer... Otherwise a handle can be placed here by accident ?!
                                        cp.CloseProfile = optionValuesStrings[1] == fallbackValue ? null : (repo.GetProfile(optionValuesStrings[1])?.GetDeepClone() as Profile ?? throw new Exception("Retrieved Part was not a Profile or not Found"));
                                        cp.CloseStrip = optionValuesStrings[2] == fallbackValue ? null : (repo.GetStrip(optionValuesStrings[2])?.GetDeepClone() as GlassStrip ?? throw new Exception("Retrieved Part was not a Strip or not Found"));
                                    }
                                    break;
                                case "6": // MiddleHingeOption
                                    if (optionValuesStrings.Length < 2) throw new Exception("Defined MiddleHinge was not Found");
                                    var cabinsPartsWithMidHinge = synthesis.GetCabinList().Where(c => c.Parts is IMiddleHinge).Select(ca => ca.Parts as IMiddleHinge);
                                    if (cabinsPartsWithMidHinge is null || cabinsPartsWithMidHinge.Any() is false) throw new Exception("Could not Find any Cabin in the Synthesis that can have MiddleHinge");
                                    foreach (var mh in cabinsPartsWithMidHinge)
                                    {
                                        if (mh is null) throw new Exception("Cabin Parts Cannot be Null");
                                        //Needs some kind of verification its a Wall fixer... Otherwise a handle can be placed here by accident ?!
                                        mh.MiddleHinge = repo.GetPart(optionValuesStrings[1])?.GetDeepClone() ?? throw new Exception("Retrieved Part was not Found");
                                    }
                                    break;
                                default:
                                    //do nothing
                                    break;
                            }
                        }
                    }
                }

                return synthesis;
            }
            else
            {
                throw new ArgumentException("Invalid Argument , Could not Parse Primary Cabin from Synthesis-String");
            }

        }

        /// <summary>
        /// Tryes to generate a Cabin Synthesis from the given String
        /// </summary>
        /// <param name="cabinSynthesisString">The String representing the Synthesis</param>
        /// <param name="synthesis">The Returned Out Synthesis</param>
        /// <returns>True if Successfully Generated - False Otherwise</returns>
        public bool TryGenerateCabinSynthesisFromString(string cabinSynthesisString, out CabinSynthesis synthesis)
        {
            try
            {
                synthesis = GenerateCabinSynthesisFromString(cabinSynthesisString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("On Trying , could not Generate Cabin Synthesis From String");
                Console.WriteLine($"Failed String : {cabinSynthesisString}");
                Console.WriteLine(ex.Message);
                synthesis = null;
                return false;
            }
        }

        /// <summary>
        /// Helper Enum to Define selectable Options
        /// </summary>
        public enum SelectablePartsOptions
        {
            None = 0,
            PerimetricalFrameOption = 1,
            HandleOption = 2,
            BottomFixerOption = 3,
            WallFixerOption = 4,
            ClosureOption = 5,
            MiddleHingeOption = 6
        }
    }
}

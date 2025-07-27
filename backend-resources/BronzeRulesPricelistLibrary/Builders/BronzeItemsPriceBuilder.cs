using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using CommonHelpers;
using CommonInterfacesBronze;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Builders
{
    /// <summary>
    /// Returns the Bronze Products fully priced for a List of items or a Certain Item
    /// </summary>
    public class BronzeItemsPriceBuilder
    {
        private readonly BronzeItemsPriceBuilderOptions options;
        private readonly ICabinMemoryRepository r;
        private readonly List<Cabin> listOfCabins = new();
        private readonly List<Mirror> listOfMirrors = new();
        private readonly List<IPriceable> priceables = new();

        public BronzeItemsPriceBuilder(BronzeItemsPriceBuilderOptions options, ICabinMemoryRepository r)
        {
            this.options = options;
            this.r = r;
        }

        /// <summary>
        /// Resets the Builder
        /// </summary>
        private void ResetBuilder()
        {
            listOfCabins.Clear();
            listOfMirrors.Clear();
            priceables.Clear();
        }

        /// <summary>
        /// Returns the List of Priceables Generated from a List of ICodeables
        /// </summary>
        /// <param name="items">The List of Items for which we need the Generation of IPraceables</param>
        /// <returns></returns>
        public List<IPriceable> GetPriceables(List<ICodeable> items)
        {
            ResetBuilder();

            //Adds the items to the Proper List
            foreach (ICodeable item in items)
            {
                bool wasAdded = listOfCabins.AddIfSameType(item);
                if (wasAdded is false)
                {
                    listOfMirrors.AddIfSameType(item);
                }
            }

            BuildPriceables();
            return priceables;
        }
        public List<IPriceable> GetPriceables(ICodeable item)
        {
            ResetBuilder();

            bool wasAdded = listOfCabins.AddIfSameType(item);
            if (wasAdded is false)
            {
                listOfMirrors.AddIfSameType(item);
            }

            BuildPriceables();
            return priceables;
        }


        /// <summary>
        /// Builds the IPriceables List for all types of ICodeable
        /// </summary>
        /// <returns></returns>
        private void BuildPriceables()
        {
            BuildCabinProducts();
            BuildMirrorProducts();
        }

        /// <summary>
        /// Build the Cabin Products List
        /// </summary>
        private void BuildCabinProducts()
        {
            if (listOfCabins.Count > 0)
            {
                //When at least one is 9C then both Primary & Secondary are 9C
                //The Count check is needed because on reseting (while wiping Secondary Model Primary Remains before getting wiped also and throws exception)
                //The Skip check so that when reseting removing one of the Pieces throws
                if (listOfCabins.Count >= 2 && listOfCabins.Any(c => c is Cabin9C))
                {
                    Cabin9C? cabin9C1 = (Cabin9C?)listOfCabins.Where(c => c is Cabin9C).FirstOrDefault();
                    Cabin9C? cabin9C2 = (Cabin9C?)listOfCabins.Where(c => c is Cabin9C).Skip(1).FirstOrDefault();

                    if (cabin9C1 is not null && cabin9C2 is not null)
                    {
                        string code;

                        //If 9C1 and 9C2 have different Lengths get a double code else get the Catalogue Code
                        if (cabin9C1.NominalLength != cabin9C2.NominalLength)
                        {
                            //9C80-1085 + 9C92-10-85 => 9C80-92-10-85
                            code = cabin9C1.Code[..5] + cabin9C2.Code[2..];
                        }
                        else
                        {
                            code = cabin9C1.Code;
                        }
                        PriceableCabin9C product9C = new(cabin9C1, cabin9C2, code)
                        {
                            DescriptionKeys = options.Cabin9CDescFunc(cabin9C1, cabin9C2),
                            ThumbnailPhotoPath = options.CabinPhotoPath(cabin9C1)
                        };
                        priceables.Add(product9C);

                        //Step is not Allowed
                        //if (cabin9C1.HasStep)
                        //{
                        //    StepCut step9C1 = (StepCut)cabin9C1.Extras.First(e => e is StepCut);
                        //    PriceableStepCut stepProduct9C1 = new(step9C1);
                        //    stepProduct9C1.DescriptionKeys = options.CabinExtraDescFunc(step9C1);
                        //    stepProduct9C1.ThumbnailPhotoPath = options.CabinExtraPhotoPath(step9C1);
                        //    priceables.Add(stepProduct9C1);
                        //}
                        //if (cabin9C2.HasStep)
                        //{
                        //    StepCut step9C2 = (StepCut)cabin9C2.Extras.First(e => e is StepCut);
                        //    PriceableStepCut stepProduct9C2 = new(step9C2);
                        //    stepProduct9C2.DescriptionKeys = options.CabinExtraDescFunc(step9C2);
                        //    stepProduct9C2.ThumbnailPhotoPath = options.CabinExtraPhotoPath(step9C2);
                        //    priceables.Add(stepProduct9C2);
                        //}

                        //Add one SafeKids for both Cabins 9C
                        if (cabin9C1.HasExtra(CabinExtraType.SafeKids) || cabin9C2.HasExtra(CabinExtraType.SafeKids))
                        {
                            CabinExtra safeKids = new(CabinExtraType.SafeKids);
                            PriceableSafekids safeKidsProduct = new(safeKids, CabinModelEnum.Model9C)
                            {
                                DescriptionKeys = options.CabinExtraDescFunc(safeKids),
                                ThumbnailPhotoPath = options.CabinExtraPhotoPath(safeKids)
                            };
                            priceables.Add(safeKidsProduct);
                        }
                    }
                }

                //Otherwise for the Rest Cabins
                foreach (Cabin cabin in listOfCabins)
                {

                    if (cabin is not Cabin9C)
                    {
                        //Add the Cabin Main Product
                        PriceableCabin cabinProduct = new(cabin);
                        cabinProduct.DescriptionKeys = options.CabinDescFunc(cabin);
                        cabinProduct.ThumbnailPhotoPath = options.CabinPhotoPath(cabin);
                        priceables.Add(cabinProduct);

                        //Add one step for each Cabin
                        if (cabin.HasStep)
                        {
                            StepCut step = (StepCut)cabin.Extras.First(e => e is StepCut);
                            PriceableStepCut stepProduct = new(step);
                            stepProduct.DescriptionKeys = options.CabinExtraDescFunc(step);
                            stepProduct.ThumbnailPhotoPath = options.CabinExtraPhotoPath(step);
                            priceables.Add(stepProduct);
                        }

                        //Add SafeKids for each Cabin
                        if (cabin.HasExtra(CabinExtraType.SafeKids))
                        {
                            CabinExtra safeKids = new(CabinExtraType.SafeKids);
                            PriceableSafekids safeKidsProduct = new(safeKids, cabin.Model ?? throw new ArgumentNullException("Model of Cabin Cannot be Null"))
                            {
                                DescriptionKeys = options.CabinExtraDescFunc(safeKids),
                                ThumbnailPhotoPath = options.CabinExtraPhotoPath(safeKids)
                            };
                            priceables.Add(safeKidsProduct);
                        }

                        //Build Handle Priceable
                        if ((cabin.Parts is IHandle handleOption) && (handleOption.Handle is not null) && (handleOption.Handle.Code != r.GetDefault(cabin.Identifier(),PartSpot.Handle1)))
                        {
                            PriceableCabinPart handle = new(handleOption.Handle)
                            {
                                DescriptionKeys = options.CabinPartDescFunc(handleOption.Handle)
                            };
                            //Photo path is set for parts automatically no need to re-set it
                            priceables.Add(handle);
                            //Add again if there are two handles
                            priceables.AddIf(cabin.Parts.GetPartOrNull<CabinHandle>(PartSpot.Handle2) is not null, handle);
                        }

                        //Build Frame Priceable
                        if ((cabin.Parts is IPerimetricalFixer fixer) && (fixer.HasPerimetricalFrame))
                        {
                            PriceableWFrame frame = new(cabin.MetalFinish ?? CabinFinishEnum.NotSet)
                            {
                                ThumbnailPhotoPath = "https://storagebronze.blob.core.windows.net/cabins-images/Parts/Profiles/WFrame.jpg"
                            };
                            frame.DescriptionKeys.Add("Frame");
                            frame.DescriptionKeys.Add(cabin.MetalFinish.ToString() ?? "N/A");
                            priceables.Add(frame);
                        }
                    }
                }

                //Add one Bronze Clean Product for All the Cabins
                if (listOfCabins.Count > 0 && listOfCabins.Any(c => c.HasExtra(CabinExtraType.BronzeClean)))
                {
                    CabinExtra bronzeClean = new(CabinExtraType.BronzeClean);
                    int numberOfGlasses = listOfCabins.Select(c => c.Glasses.Count).Sum(); //Count the Number of Glasses for Each Cabin on the List
                    //Take the First on the List its always the Primary
                    PriceableBronzeClean bronzeCleanProduct = new(bronzeClean, listOfCabins[0].Model ?? throw new ArgumentNullException("Cabin Model Cannot be Null"));
                    bronzeCleanProduct.DescriptionKeys = options.CabinExtraDescFunc(bronzeClean);
                    bronzeCleanProduct.ThumbnailPhotoPath = options.CabinExtraPhotoPath(bronzeClean);
                    priceables.Add(bronzeCleanProduct);
                }
            }
        }

        /// <summary>
        /// Builds the Mirrors Product List
        /// </summary>
        private void BuildMirrorProducts()
        {
            if (listOfMirrors.Count > 0)
            {
                foreach (Mirror mirror in listOfMirrors)
                {
                    PriceableMirror mirrorProduct = new(mirror);
                    mirrorProduct.DescriptionKeys = options.MirrorDescFunc(mirror);
                    priceables.Add(mirrorProduct);

                    //There is no Reference for Sandblast when Mirror is From Catalogue!!!
                    if (mirror.IsFromCatalogue() is false && mirror.HasSandblast())
                    {
                        PriceableSandblast sandblastProduct = new(mirror.Sandblast ?? MirrorSandblast.H7);
                        sandblastProduct.DescriptionKeys = options.MirrorSandblastDescFunc(mirror.Sandblast ?? MirrorSandblast.H7);
                        priceables.Add(sandblastProduct);
                    }

                    if (mirror.HasLight())
                    {
                        PriceableLighting lightProduct = new(mirror);
                        lightProduct.DescriptionKeys = options.MirrorLightDescFunc(mirror.Lighting);
                        priceables.Add(lightProduct);
                    }

                    if (mirror.HasSupport())
                    {
                        PriceableMirrorSupport supportProduct = new(mirror);
                        supportProduct.DescriptionKeys = options.MirrorSupportDescFunc(mirror.Support);
                        priceables.Add(supportProduct);
                    }

                    foreach (MirrorExtra extra in mirror.Extras)
                    {
                        PriceableMirrorExtra extraProduct = new(mirror, extra);
                        extraProduct.DescriptionKeys = options.MirrorExtraDescFunc(extra);
                        priceables.Add(extraProduct);
                    }
                }
            }
        }

    }
}

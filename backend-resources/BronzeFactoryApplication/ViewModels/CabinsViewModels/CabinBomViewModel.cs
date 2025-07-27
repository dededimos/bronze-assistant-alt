using BronzeFactoryApplication.Helpers.Other;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels
{
    public partial class CabinBomViewModel : BaseViewModel
    {
        private CabinOrderRow row = CabinOrderRow.Empty();
        private readonly Func<GlassViewModel> glassVmFactory;

        public IEnumerable<CabinPart> Parts { get => GetCombinedParts()
                .OrderByDescending(p => p.Part == CabinPartType.Profile || p.Part == CabinPartType.ProfileHinge || p.Part == CabinPartType.MagnetProfile)
                .ThenByDescending(p => p.Part == CabinPartType.Strip)
                .ThenByDescending(p => p.Part == CabinPartType.Hinge)
                .ThenByDescending(p => p.Part == CabinPartType.Handle)
                .ThenByDescending(p => p.Part == CabinPartType.BarSupport)
                .ThenByDescending(p => p.Part == CabinPartType.SmallSupport)
                .ThenByDescending(p => p.Part == CabinPartType.FloorStopperW)
                .ThenByDescending(p => p.Part == CabinPartType.AnglePart)
                .ThenByDescending(p => p.Part == CabinPartType.Wheel)
                .ThenByDescending(p => p.Part == CabinPartType.GenericPart);
        }
        public string Code { get => row.OrderedCabin.Code; }
        public CabinModelEnum? Model { get => row.OrderedCabin.Model; }
        public string CabinDimensionsString { get => row.OrderedCabin.ToString(); }
        public List<GlassDrawWithRowViewModel> GlassesDraws { get => GetGlassDraws(); }
        public string ReferencePA0 { get => row.ReferencePA0; }
        public CabinThicknessEnum Thicknesses { get => row?.OrderedCabin.Thicknesses ?? CabinThicknessEnum.NotSet; }
        public GlassFinishEnum GlassFinish { get => row?.OrderedCabin.GlassFinish ?? GlassFinishEnum.GlassFinishNotSet; }
        public int NominalLength { get => row.OrderedCabin.NominalLength; }
        public int Height { get => row.OrderedCabin.Height; }
        public bool HasStep { get => row.OrderedCabin.HasStep; }
        public StepCut? Step { get => row.OrderedCabin.HasStep ? row.OrderedCabin.GetStepCut() : null; }
        public string Notes { get => row.Notes; }


        public CabinBomViewModel(Func<GlassViewModel> glassVmFactory)
        {
            this.glassVmFactory = glassVmFactory;
        }

        /// <summary>
        /// Constructs the GlassDrawViewModels from the Glasses of the Row
        /// </summary>
        /// <returns></returns>
        public List<GlassDrawWithRowViewModel> GetGlassDraws()
        {
            var glassesRows = row.GlassesRows;
            List<GlassDrawWithRowViewModel> draws = new();
            foreach (var glassRow in glassesRows)
            {
                GlassDrawViewModel draw = new();
                GlassViewModel glassVm = glassVmFactory.Invoke();
                glassVm.SetGlass(glassRow.OrderedGlass,true);
                draw.SetGlassDraw(glassVm);

                draws.Add(new(glassRow,draw));
            }
            return draws;
        }

        public void SetCabinRow(CabinOrderRow row)
        {
            this.row = row;
        }

        private IEnumerable<CabinPart> GetCombinedParts()
        {
            //Get All Parts
            IEnumerable<CabinPart> allParts = row.OrderedCabin.Parts.GetCabinPartsNested(row.OrderedCabin.Identifier());
            return CombinePartsQuantities(allParts);
        }

        /// <summary>
        /// Combines All The Parts of a Stucture into a Single Bill Of Materials List
        /// (Combining Quantites of same parts to a single one)
        /// </summary>
        /// <param name="parts">All the Parts of the Structure to Create the BOM List</param>
        /// <returns>The Combined list</returns>
        private static IEnumerable<CabinPart> CombinePartsQuantities(IEnumerable<CabinPart> parts)
        {
            //PARTS SHOULD BE CLONED BEFORE INSERTED HERE
            IEnumerable<CabinPart> result =
                parts.GroupBy(p => p.Code).SelectMany(group =>
                {
                    //Check wheather the Group is of items with Cut Length
                    if (group.First() is IWithCutLength)
                    {
                        //Group the Parts a secondTime to get those that have the same CutLength in Groups
                        var partsWithSameCutLengthCombined = group.OfType<IWithCutLength>().GroupBy(cl => cl.CutLength)
                        .Select(group2 =>
                        {
                            //From each group find the total quantity
                            var totalQuantity = group2.Cast<CabinPart>().Sum(p => p.Quantity);
                            //DeepClone the First Part and Assign the total quantity to it
                            var firstPart = ((CabinPart)group2.First());
                            firstPart.Quantity = totalQuantity;
                            //Return the modified part containing all the parts as a single one with quantity
                            return firstPart;
                            //Parts that have same cut length and Code are the Same for sure , at least as far as the BOM is concerned
                        })
                        //Return all except the profiles with zero length
                        .Where(p => (p is not Profile || (p is Profile prof && prof.CutLength != 0)));
                        
                        return partsWithSameCutLengthCombined;
                    }
                    else
                    {
                        //All the items should be the same in this case
                        //only cimbine quantity and return
                        var totalQuantity = group.Sum(p => p.Quantity);
                        var firstPart = group.First();
                        firstPart.Quantity = totalQuantity;
                        return new List<CabinPart>() { firstPart };
                    }
                });
            return result;
        }
    }

    public partial class GlassDrawWithRowViewModel : BaseViewModel 
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GlassDrawString))]
        private GlassOrderRow glassRow;
        [ObservableProperty]
        private GlassDrawViewModel glassDraw;

        public string GlassDrawString 
        { 
            get 
            {
                StringBuilder builder = new(GlassRow.OrderedGlass.Draw.ToString().TryTranslateKey());
                if(GlassRow.SpecialDrawString is not null) builder.Append(GlassRow.SpecialDrawString);
                if (GlassRow.SpecialDrawNumber is not null) builder.Append(GlassRow.SpecialDrawNumber);
                return builder.ToString();
            } 
        }

        public GlassDrawWithRowViewModel(GlassOrderRow glassRow , GlassDrawViewModel glassDraw)
        {
            this.glassRow = glassRow;
            this.glassDraw = glassDraw;
        }
    }


}

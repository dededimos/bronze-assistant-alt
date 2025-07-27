using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.ViewModels;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.ConcreteRules;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components
{
    public partial class CabinsPricingTable : ComponentBase, IDisposable
    {
        [Parameter] public bool WithPrintStyle { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public int PrintingTopPaddingPX { get; set; }

        private List<IPriceable> items = [];

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
            user.PropertyChanged += User_PropertyChanged;
            vm.PrimaryCabin.CalculateGlasses();
            vm.SecondaryCabin.CalculateGlasses();
            vm.TertiaryCabin.CalculateGlasses();
            items = vm.GetProductsList();
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            vm.PrimaryCabin.CalculateGlasses();
            vm.SecondaryCabin.CalculateGlasses();
            vm.TertiaryCabin.CalculateGlasses();
            items = vm.GetProductsList();
            StateHasChanged();
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(AssembleCabinViewModel.PrimaryCabin)
                               or nameof(AssembleCabinViewModel.SecondaryCabin)
                               or nameof(AssembleCabinViewModel.TertiaryCabin)
                               or nameof(AssembleCabinViewModel.SelectedCabinDraw))
            {
                vm.PrimaryCabin.CalculateGlasses();
                vm.SecondaryCabin.CalculateGlasses();
                vm.TertiaryCabin.CalculateGlasses();
                items = vm.GetProductsList();
                StateHasChanged();
            }
        }

        /// <summary>
        /// Makes the Catalogue Total PriceText StrikenThrough when in Retail Mode
        /// </summary>
        private string CataloguePriceStyle
        {
            get
            {
                string style = "";
                if (user.SelectedAppMode == BronzeAppMode.Retail)
                {
                    style += "text-decoration:line-through;";
                }
                return style;
            }
        }

        /// <summary>
        /// Applies the Css "flipHorizontal" class to an img element 
        /// </summary>
        /// <param name="priceable"></param>
        /// <returns></returns>
        private string FlipHorizontalOnNonDefautlDirection(IPriceable priceable)
        {
            if (priceable is PriceableCabin c)
            {
                if (c.CabinProperties.SynthesisModel is CabinSynthesisModel.Primary && c.CabinProperties.Direction != DefaultPrimaryCabinDirection[c.CabinProperties.IsPartOfDraw]
                   || c.CabinProperties.SynthesisModel is CabinSynthesisModel.Secondary && c.CabinProperties.Direction != DefaultSecondaryCabinDirection[c.CabinProperties.IsPartOfDraw]
                   || c.CabinProperties.SynthesisModel is CabinSynthesisModel.Tertiary && c.CabinProperties.Direction != DefaultTertiaryCabinDirection[c.CabinProperties.IsPartOfDraw])
                {
                    //The CSS Class that flips an Image Horizontally
                    return "flipHorizontal";
                }
            }
            return "";
        }

        private AppliedRule GetComplexRule(IEnumerable<AppliedRule> rules) 
        {
            return rules.FirstOrDefault(r => r.IsComplexRule);
        }
        private AppliedRule GetSecondComplexRule(IEnumerable<AppliedRule> rules)
        {
            return rules.Where(r => r.IsComplexRule).Skip(1).FirstOrDefault();
        }

        private bool HasComplexRule(IEnumerable<AppliedRule> rules)
        {
            var isComplex = rules.Any(r => r.IsComplexRule);
            return isComplex;
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
            user.PropertyChanged -= User_PropertyChanged;
        }

    }
}

using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    public class ExrtaHingePriceRule : IPricingRule
    {
        public string RuleName { get => nameof(ExrtaHingePriceRule); }
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public string RuleApplicationDescription { get; set; } = "";
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalPriceValue; }

        private readonly decimal flatIncreasePerExtraHinge;

        public ExrtaHingePriceRule(decimal flatIncreasePerExtraHinge)
        {
            this.flatIncreasePerExtraHinge = flatIncreasePerExtraHinge;
        }

        public void AddError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ApplyRule(IPriceable product)
        {
            //Special Exception
            //DB Model Catalogue takes into account pricing for extra hinges (Regarding Length)
            //So when length goes above a threshold it calculates the 3rd Hinge
            //The Cases that must be taken into account from the Rule are then Only : 
            //Height >205 Length >50 Length <65 (3Hinges)
            //Length > 70 , Height >2150 4xHinges should take into account only +1 Hinge In Price

            PriceableCabin cabin = product as PriceableCabin ?? throw new NotSupportedException("Invalid product for Rule Application");

            if (cabin.CabinProperties is IWithHingeCuts cabinWithHinges)
            {
                decimal priceIncrease = 0;
                //When not DB normally apply extra Hinges
                if (cabinWithHinges is not CabinDB dbCabin)
                {
                    //Hinges not Taken Into Account always 2!!!
                    priceIncrease = flatIncreasePerExtraHinge * (cabinWithHinges.NumberOfHinges - 2);
                }
                else //When its CabinDB
                {
                    if (cabinWithHinges.NumberOfHinges == 4)
                    {
                        //TAKE INTO ACCOUNT ONLY ONE HINGE OF THE FOUR
                        priceIncrease = flatIncreasePerExtraHinge;
                    }
                    //Case with 3 Hinges not taken into account by the Catalogue (Rest Cases with 3 are inside Catalogue Price)
                    else if (dbCabin.Height > 2050 && dbCabin.NominalLength < 650)
                    {
                        //Hinges not Taken Into Account always 2!!!
                        priceIncrease = flatIncreasePerExtraHinge * (cabinWithHinges.NumberOfHinges - 2);
                    }
                }

                cabin.StartingPrice += priceIncrease;
                RuleApplicationDescription = $"+{priceIncrease.ToString("0.00\u20AC")}";
            }


        }

        public bool IsApplicable(IPriceable product)
        {
            //Apply the Rule to all Models that take Cuts for Hinges when Those cuts are >2
            if (product is PriceableCabin c && c.CabinProperties is IWithHingeCuts cabinWithHinges)
            {
                if (cabinWithHinges.NumberOfHinges > 2)
                {
                    return true;
                }
            }
            return false;
        }



    }
}

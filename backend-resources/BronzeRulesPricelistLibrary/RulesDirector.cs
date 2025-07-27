using BronzeRulesPricelistLibrary.ConcreteRules;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary
{
    public class RulesDirector
    {
        private List<IPricingRule> rules = new();

        public RulesDirector(PricingRulesOptionsCabins options)
        {
            rules = RulesFactory.GetCabinsRuleList(options);
        }

        public RulesDirector(PricingRulesOptionsMirrors options)
        {
            rules = RulesFactory.GetMirrorsRuleList(options);
        }

        public RulesDirector(PricingRulesOptionsAccessories options)
        {
            rules = RulesFactory.GetAccessoriesRuleList(options);
        }

        public RulesDirector()
        {
            
        }

        /// <summary>
        /// Apply Pricing Rules to a Single Product
        /// </summary>
        /// <param name="product">The Product to which the Pricing rules will Apply</param>
        public void ApplyRules(IPriceable product,bool clearPreviousRules = false)
        {
            if (clearPreviousRules)
            {
                //Clear any Previous Applied Rules
                product.ClearAppliedRules();
            }

            foreach (IPricingRule rule in rules)
            {
                if (rule.IsApplicable(product))
                {
                    rule.ApplyRule(product);
                    //To maintain its application we have to add a clone of the Rule , or of the Rules Information
                    //Otherwise all the Application and Errors stored in the Rule are lost , when the rule is reapplied somewhere else
                    product.AppliedRules.Add(rule.GetRuleInfoObject());
                }
            }
        }

        /// <summary>
        /// Apply Pricing Rules to Multiple Products
        /// </summary>
        /// <param name="products">The List of Products to Which the Rules will apply</param>
        public void ApplyRulesToMultiple(List<IPriceable> products , bool clearPreviousRules = false)
        {
            foreach (IPriceable product in products) 
            {
                ApplyRules(product,clearPreviousRules);
            }
        }

        /// <summary>
        /// Returns the Names of All the Rules that Can be Applied
        /// </summary>
        /// <returns></returns>
        public List<string> GetRuleListNames()
        {
            List<string> list = new();
            foreach (var rule in rules)
            {
                list.Add(rule.RuleName);
            }
            list = list.Distinct().ToList();
            return list;
        }

        public void GenerateNewRules(PricingRulesOptionsAccessories options)
        {
            rules.Clear();
            rules = RulesFactory.GetAccessoriesRuleList(options);
        }
        public void GenerateNewRules(PricingRulesOptionsMirrors options)
        {
            rules.Clear();
            rules = RulesFactory.GetMirrorsRuleList(options);
        }
        public void GenerateNewRules(PricingRulesOptionsCabins options)
        {
            rules.Clear();
            rules = RulesFactory.GetCabinsRuleList(options);
        }

    }
}

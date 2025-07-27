using BronzeRulesPricelistLibrary.ConcreteRules;
using BronzeRulesPricelistLibrary.Models.Priceables;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary
{
    public interface IPricingRule
    {
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string RuleName { get;}
        public string RuleApplicationDescription { get; }
        public RuleModification ModificationType { get; }

        bool IsApplicable(IPriceable product);
        void ApplyRule(IPriceable product);
        void AddError(string message);

        /// <summary>
        /// Creates an Info Object out of this Rule. The Rule must have already Run for the Object to have any Info in it
        /// </summary>
        /// <returns>An AppliedRule Info Object</returns>
        public AppliedRule GetRuleInfoObject()
        {
            return new(this);
        }
    }
}

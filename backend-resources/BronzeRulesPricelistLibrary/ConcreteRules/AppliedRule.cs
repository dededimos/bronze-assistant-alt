using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules
{
    public class AppliedRule : IPricingRule
    {
        public string ErrorMessage { get; set ; }
        public bool HasError { get; set; }
        public Type AppliedRuleType { get; set; }

        public string RuleName { get; private set; }

        public string RuleApplicationDescription {get; private set; }
        public bool IsComplexRule { get => CalculationsDescriptors.Any(); }
        public List<CalculationDescriptor> CalculationsDescriptors { get; }
        public RuleModification ModificationType { get; private set; }

        public void AddError(string message)
        {
            throw new NotSupportedException($"Rules Already Applied , have their Errors Defined Already");
        }

        public void ApplyRule(IPriceable product)
        {
            throw new NotSupportedException($"Rules Already Applied cannot run Again");
        }

        public bool IsApplicable(IPriceable product)
        {
            //Has Aready Applied so always true;
            return true;
        }

        /// <summary>
        /// Creates a Rule Info Object from an already applied Rule. Copies the Relevant Info of the Applied Rule to this Object.
        /// </summary>
        /// <param name="ruleAlreadyApplied">The Rule that has been applied</param>
        public AppliedRule(IPricingRule ruleAlreadyApplied)
        {
            this.ErrorMessage = ruleAlreadyApplied.ErrorMessage;
            this.HasError = ruleAlreadyApplied.HasError;
            this.RuleName = ruleAlreadyApplied.RuleName;
            this.RuleApplicationDescription = ruleAlreadyApplied.RuleApplicationDescription;
            this.AppliedRuleType = ruleAlreadyApplied.GetType();
            this.ModificationType = ruleAlreadyApplied.ModificationType;

            //If Applied Rule was a complex one then add the complex description to the AppliedRule Object
            if (ruleAlreadyApplied is IPricingRuleComplex complex)
            {
                CalculationsDescriptors = complex.CalculationsDescriptors.Select(d => new CalculationDescriptor(d)).ToList();
            }
            else
            {
                CalculationsDescriptors = new();
            }
        }
    }
}

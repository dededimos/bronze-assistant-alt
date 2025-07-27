using BronzeRulesPricelistLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary
{
    /// <summary>
    /// The Same as a Pricing Rule but with Extra Descriptor Keys Targeting Calculations
    /// </summary>
    public interface IPricingRuleComplex : IPricingRule
    {
        public List<CalculationDescriptor> CalculationsDescriptors { get; }
    }
}

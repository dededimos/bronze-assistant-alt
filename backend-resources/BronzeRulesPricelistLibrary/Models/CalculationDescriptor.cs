using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models
{
    public class CalculationDescriptor
    {
        /// <summary>
        /// The Title string or string Key of the Calculation
        /// </summary>
        public string TitleKey { get; set; } = string.Empty;
        /// <summary>
        /// The String Description of the Calculation - Formula
        /// </summary>
        public string Description { get; set; } = string.Empty;

        public CalculationDescriptor(string titleKey,string description)
        {
            TitleKey = titleKey;
            Description = description;
        }

        public CalculationDescriptor(CalculationDescriptor other)
        {
            this.TitleKey = other.TitleKey;
            this.Description = other.Description;
        }

    }
}

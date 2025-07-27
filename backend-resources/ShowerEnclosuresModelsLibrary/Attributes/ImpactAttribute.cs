using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Attributes
{
    /// <summary>
    /// This Property has an Impact On The Defined Calculations (ex. ImpactOn.Glasses)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ImpactAttribute : Attribute 
    {
        private ImpactOn impactedType;

        public ImpactAttribute(ImpactOn impactedType)
        {
            this.impactedType = impactedType;
        }
    }

    public enum ImpactOn
    {
        Glasses,
        L0,
        Tollerances
    }
    
}

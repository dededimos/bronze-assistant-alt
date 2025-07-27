using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models
{
    public class CabinExtra : ICodeable , IDeepClonable<CabinExtra>
    {
        public string Code { get => GetCode(); }

        private CabinExtraType extraType;
        public CabinExtraType ExtraType 
        {
            get => extraType;
            set
            {
                if (value != extraType)
                {
                    extraType = value;
                }
            }
        }

        public CabinExtra(CabinExtraType type)
        {
            ExtraType = type;
        }

        private string GetCode()
        {
            string code = ExtraType switch
            {
                CabinExtraType.StepCut          => "0000-STEP",
                CabinExtraType.BronzeClean      => "0000-BR-CLEAN",
                CabinExtraType.SafeKids         => "0000-SA-FEKID",
                CabinExtraType.BronzeCleanNano  => "0000-NA-CLEAN",
                _                               => "N/A",
            };
            return code;
        }

        public virtual CabinExtra GetDeepClone()
        {
            return (CabinExtra)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return Code;
        }
    }
}

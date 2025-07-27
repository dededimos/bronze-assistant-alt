using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models
{
    public class StepCut : CabinExtra
    {
        
        public int StepLength { get; set; }
        public int StepHeight { get; set; }

        public StepCut() : base(CabinExtraType.StepCut)
        {
        }

        public override StepCut GetDeepClone()
        {
            return (StepCut)this.MemberwiseClone();
        }
    }
}

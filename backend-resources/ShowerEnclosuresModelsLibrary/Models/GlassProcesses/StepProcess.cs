using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses
{
    public class StepProcess : GlassProcess
    {
        public double StepHeight { get; set; }
        public double StepLength { get; set; }
        public override GlassProcessType ProcessType => GlassProcessType.StepCut;

        public StepProcess(double stepLength, double stepHeight)
        {
            StepLength = stepLength;
            StepHeight = stepHeight;
            VerticalDistancing = VertDistancing.FromBottom;
            VerticalDistance = 0;
            HorizontalDistancing = HorizDistancing.FromLeft; 
            HorizontalDistance = 0;
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShowerEnclosuresModelsLibrary.Models.GlassProcesses.ConstantValues.GlassProcessesConstants;

namespace ShowerEnclosuresModelsLibrary.Models.GlassProcesses
{
    public class Cut9B : GlassProcess
    {
        public int Length { get; set; }
        public int Height { get; set; }
        public override GlassProcessType ProcessType { get => GlassProcessType.Cut9B; }

        public Cut9B()
        {
            Height = ProcessesB6000.HingeCutHeight;
            Length = ProcessesB6000.HingeCutLength;
        }
        
        public Cut9B(int length, int height)
        {
            Length = length;
            Height = height;
        }

    }
}

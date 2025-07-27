using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A message that holds the Information of a Glass Swap
    /// </summary>
    public class SynthesisSwapMessage
    {
        public CabinSynthesis? NewSynthesis { get; set; }
        public CabinSynthesis OldSynthesis { get; set; }
        public GlassSwap Swap { get; set; }

        public SynthesisSwapMessage(CabinSynthesis oldSynthesis , CabinSynthesis? newSynthesis , GlassSwap swap)
        {
            OldSynthesis = oldSynthesis;
            NewSynthesis = newSynthesis;
            Swap = swap;
        }
    }
}

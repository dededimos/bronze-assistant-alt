using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// Whenever the Code of a Structure Changes Because the User Manipulated the Structures Properties
    /// </summary>
    public class CabinCodeChangedMessage
    {
        public string NewCode { get; set; } = string.Empty;
        public CabinSynthesisModel SynthesisModel { get; set; }

        public CabinCodeChangedMessage(string newCode, CabinSynthesisModel synthesisModel)
        {
            NewCode = newCode;
            SynthesisModel = synthesisModel;
        }
    }
}

using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Helpers.Custom_Exceptions
{
    public class InvalidSynthesisDrawsException : Exception
    {
#nullable enable
        public const string baseMessage = "The Draws of the Various Cabins of the Synthesis where not Consistent";

        public InvalidSynthesisDrawsException():base(baseMessage) { }

        public InvalidSynthesisDrawsException(CabinDrawNumber? primaryDraw , CabinDrawNumber? secondaryDraw , CabinDrawNumber? tertiaryDraw) 
            : base(string.Format("{0}{1}PrimaryDraw: {2}{1}SecondaryDraw: {3}{1}TertiaryDraw: {4}"
                ,baseMessage
                ,Environment.NewLine
                ,primaryDraw?.ToString() ?? "null"
                ,secondaryDraw?.ToString() ?? "null"
                ,tertiaryDraw?.ToString() ?? "null")) 
        { }
    }
}

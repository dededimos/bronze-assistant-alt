using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces
{
    public interface IWithCutLength
    {
        /// <summary>
        /// The Cut Length of the piece
        /// </summary>
        public double CutLength { get; set; }
    }
    public interface IWithCutLengthStepPart : IWithCutLength
    {
        /// <summary>
        /// The Extra Part Length when there is Step
        /// </summary>
        public double CutLengthStepPart { get; set; }
    }
}

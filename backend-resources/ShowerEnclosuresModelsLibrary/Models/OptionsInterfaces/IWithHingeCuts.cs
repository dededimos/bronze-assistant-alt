using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces
{
    public interface IWithHingeCuts
    {
        /// <summary>
        /// The Number of Hinges
        /// </summary>
        public int NumberOfHinges { get => GetHingesNumber();}

        /// <summary>
        /// Determines the Number of Hinges
        /// </summary>
        /// <returns></returns>
        public int GetHingesNumber();
    }
}

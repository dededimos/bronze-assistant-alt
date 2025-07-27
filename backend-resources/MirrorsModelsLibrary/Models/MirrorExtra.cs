using CommonInterfacesBronze;
using MirrorsModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MirrorsModelsLibrary.StaticData.MirrorsStaticData;

namespace MirrorsModelsLibrary.Models
{
    public class MirrorExtra : ICodeable
    {
        public MirrorOption Option { get; set; }
        public string Code { get => GetCode(); }

        public MirrorExtra(MirrorOption option)
        {
            Option = option;
        }

        /// <summary>
        /// Gets the Price for the Extra - if its included in the overall Price it returns 0
        /// </summary>
        /// <param name="isIncluded"></param>
        /// <returns></returns>
        public decimal GetPrice(bool isIncluded = false)
        {
            return GetOptionPrice(Option, isIncluded);
        }
        /// <summary>
        /// Gets the Price for the Extra - if its included in the overall Price it returns 0
        /// </summary>
        /// <param name="option"></param>
        /// <param name="isIncluded"></param>
        /// <returns></returns>
        public static decimal GetOptionPrice(MirrorOption option, bool isIncluded = false)
        {
            if (isIncluded)
            {
                return 0m;
            }
            else
            {
                //Return the Price for this Extra
                return MirrorOptionsPriceCodeDictionary[option].Item2;
            }
        }

        private string GetCode()
        {
            //Return the Code for this Light
            return MirrorOptionsPriceCodeDictionary[Option].Item1;
        }

    }
}

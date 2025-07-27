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
    public class LightingModel : ICodeable
    {
        public MirrorLight? Light { get; set; }
        public string Code { get => GetCode();}

        /// <summary>
        /// Gets teh Price of the Light depending on whether its changes from default or not
        /// </summary>
        /// <param name="isChangedFromDefault">Whether this , is a Light we Changed from the Default </param>
        /// <returns></returns>
        public decimal GetPrice(bool areBasicLightsExcluded = true)
        {
            switch (Light)
            {
                case MirrorLight.Warm:
                case MirrorLight.Cold:
                case MirrorLight.Daylight:
                    if (areBasicLightsExcluded)
                    {
                        return 0m;
                    }
                    else
                    {
                        return MirrorOptionsPriceCodeDictionary[Light].Item2;
                    }
                case MirrorLight.Warm_Cold:
                case MirrorLight.Warm_Cold_Day:
                case MirrorLight.Day_COB:
                case MirrorLight.Warm_COB:
                case MirrorLight.Cold_COB:
                case MirrorLight.Warm_Cold_Day_COB:
                case MirrorLight.Day_16W:
                case MirrorLight.Warm_16W:
                case MirrorLight.Cold_16W:
                case MirrorLight.Warm_Cold_Day_16W:
                case MirrorLight.WithoutLight:
                    return MirrorOptionsPriceCodeDictionary[Light].Item2;
                default:
                    return 0m;
            }
        }

        private string GetCode()
        {
            if (Light != null)
            {
                //Return the Code for this Light
                return MirrorOptionsPriceCodeDictionary[Light].Item1;
            }
            else
            {
                return "-";
            }
        }
    }
}

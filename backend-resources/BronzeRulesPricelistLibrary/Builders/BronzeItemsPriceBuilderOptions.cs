using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Builders
{
    /// <summary>
    /// Provides the Options to the ItemsPriceBuilder
    /// An initializer must be Provided to Create these options when used Depending on the Settings of the App using it.
    /// </summary>
    public class BronzeItemsPriceBuilderOptions
    {
        #region 1.Cabin Functions

        /// <summary>
        /// The Function Generating the Description Keys for a Cabin
        /// </summary>
        public Func<Cabin, List<string>> CabinDescFunc { get; set; } = cabin => throw new NotImplementedException("Cabin Description Function has Not been Set");

        /// <summary>
        /// Special Case for 9C Cabins - They Get Merged into one Product
        /// </summary>
        public Func<Cabin9C, Cabin9C, List<string>> Cabin9CDescFunc { get; set; } = (c1, c2) => throw new NotImplementedException("Cabin9C Description Function has Not been Set");

        /// <summary>
        /// The Function Generating the Description Keys for a CabinExtra
        /// </summary>
        public Func<CabinExtra, List<string>> CabinExtraDescFunc { get; set; } = extra => throw new NotImplementedException("CabinExtra Description Function has Not been Set");

        /// <summary>
        /// The Function Generating the Pathstring for the Thumbnail Photo of a Cabin
        /// </summary>
        public Func<Cabin, string> CabinPhotoPath { get; set; } = cabin => throw new NotImplementedException("Cabin PhotoPath Function has Not been Set");

        /// <summary>
        /// The Function Generating the Pathstring for the Thumbnail Photo of a CabinExtra
        /// </summary>
        public Func<CabinExtra, string> CabinExtraPhotoPath { get; set; } = extra => throw new NotImplementedException("CabinExtra PhotoPath Function has Not been Set");

        public Func<CabinPart, List<string>> CabinPartDescFunc { get; set; } = part => throw new NotImplementedException("CabinPart Description Function has Not been Set");

        #endregion

        #region 2.Mirror Functions

        /// <summary>
        /// The Function Generating the Description Keys of a Mirror
        /// </summary>
        public Func<Mirror, List<string>> MirrorDescFunc { get; set; } = mirror => throw new NotImplementedException("Mirror Description Function has Not been Set");

        /// <summary>
        /// The Function Generating the Descriptions Strings of a Sandblast
        /// </summary>
        public Func<MirrorSandblast, List<string>> MirrorSandblastDescFunc { get; set; } = sandblast => throw new NotImplementedException("MirrorSandblast Description Function has Not been Set");

        /// <summary>
        /// The Function Generating the Description String of A Mirrors Light
        /// </summary>
        public Func<LightingModel, List<string>> MirrorLightDescFunc { get; set; } = light => throw new NotImplementedException("MirrorLight Description Function has Not been Set");

        /// <summary>
        /// The Function Generating the Description String of a Mirrors Support
        /// </summary>
        public Func<SupportModel, List<string>> MirrorSupportDescFunc { get; set; } = support => throw new NotImplementedException("MirrorSupport Description Function has Not been Set");

        /// <summary>
        /// The Function Generating the Description string of A Mirror Extra
        /// </summary>
        public Func<MirrorExtra, List<string>> MirrorExtraDescFunc { get; set; } = extra => throw new NotImplementedException("MirrorExtra Description Function has Not been Set");

        #endregion

        public BronzeItemsPriceBuilderOptions()
        {
            
        }

    }
}

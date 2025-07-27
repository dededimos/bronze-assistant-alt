using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteApplicationSettings.DTOs
{
    public class GlassesStockServiceSettingsDTO : DTO
    {
        /// <summary>
        /// Weather this Setting is Selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Weather this is the Default Setting
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Weather the Heights of glasses should be compared for matching
        /// </summary>
        public bool ShouldCompareHeight { get; set; }
        /// <summary>
        /// The Max Difference the heights can have
        /// </summary>
        public double AllowedHeightDifference { get; set; }
        /// <summary>
        /// Weather the Lengths of glasses should be compared for matching
        /// </summary>
        public bool ShouldCompareLength { get; set; }
        /// <summary>
        /// The Max Difference the Lengths can have
        /// </summary>
        public double AllowedLengthDifference { get; set; }
        /// <summary>
        /// Weather the Thicknesses of glasses should be compared for matching
        /// </summary>
        public bool ShouldCompareThickness { get; set; }
        /// <summary>
        /// Weather the Finishes of glasses should be compared for matching
        /// </summary>
        public bool ShouldCompareFinish { get; set; }
        /// <summary>
        /// The Model that these Settings Concern
        /// </summary>
        public CabinModelEnum ConcernsModel { get; set; }
    }
}

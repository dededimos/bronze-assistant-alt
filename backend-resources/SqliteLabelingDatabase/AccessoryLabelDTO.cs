using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteLabelingDatabase
{
    /// <summary>
    /// An accessory Data Transfer Object for Labeling Database
    /// </summary>
    public class AccessoryLabelDTO
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.MinValue;
        public DateTime LastModified { get; set; } = DateTime.MinValue;
        public string Code { get; set; } = string.Empty;
        public string DescriptionGreek { get; set; } = string.Empty;
        public string DescriptionEnglish { get; set; } = string.Empty;
        public byte[] Photo { get; set; } = [];
        public string FinishGreek { get; set; } = string.Empty;
        public string FinishEnglish { get; set; } = string.Empty;
        public string PrimaryTypeGreek { get; set; } = string.Empty;
        public string PrimaryTypeEnglish { get; set; } = string.Empty;
        public string SecondaryTypeGreek { get; set; } = string.Empty;
        public string SecondaryTypeEnglish { get; set; } = string.Empty;
        public string SeriesGreek { get; set; } = string.Empty;
        public string SeriesEnglish { get; set; } = string.Empty;
        public string MountingTypeGreek { get; set; } = string.Empty;
        public string MountingTypeEnglish { get; set; } = string.Empty;
    }
}

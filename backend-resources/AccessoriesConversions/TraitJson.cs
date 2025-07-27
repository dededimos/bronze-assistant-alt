using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessoriesConversions
{
    public class TraitJson
    {
        /// <summary>
        /// A Dictionary Determining weather each of the properties of this class Should be Serilized
        /// </summary>
        private static Dictionary<string, bool> shouldSerilizeProperty = typeof(TraitJson).GetProperties().ToDictionary(p => p.Name, p => true);
        public static void SetSerilizableProperties(params string[] propertyNames)
        {
            foreach (var propName in shouldSerilizeProperty.Keys)
            {
                if (propertyNames.Contains(propName) is false)
                {
                    shouldSerilizeProperty[propName] = false;
                }
            }
        }

        private int sortNo = 99999;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SortNo
        {
            get => ShouldSerilizeSortNo()
                ? (sortNo == 0 ? 99999 : sortNo)
                : 0;
            set => sortNo = value;
        }
        public bool ShouldSerilizeSortNo()
        {
            return shouldSerilizeProperty[nameof(SortNo)];
        }

        private string? traitType = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TraitType
        {
            get => ShouldSerilizeTraitType()
                ? (string.IsNullOrEmpty(traitType) ? "Undefined" : traitType)
                : null;
            set => traitType = value;
        }
        public bool ShouldSerilizeTraitType()
        {
            return shouldSerilizeProperty[nameof(TraitType)];
        }

        private string? traitGroup = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TraitGroup
        {
            get => ShouldSerilizeTraitGroup()
                ? (string.IsNullOrEmpty(traitGroup) ? "Undefined" : traitGroup)
                : null;
            set => traitGroup = value;
        }
        public bool ShouldSerilizeTraitGroup()
        {
            return shouldSerilizeProperty[nameof(TraitGroup)];
        }

        private string? code = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Code
        {
            get => ShouldSerilizeCode()
                ? (string.IsNullOrEmpty(code) ? "Undefined" : code)
                : null;
            set => code = value;
        }
        public bool ShouldSerilizeCode()
        {
            return shouldSerilizeProperty[nameof(Code)];
        }

        private string? photoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PhotoUrl
        {
            get => ShouldSerilizePhotoUrl()
                ? (string.IsNullOrEmpty(photoUrl) ? "Undefined" : photoUrl)
                : null;
            set => photoUrl = value;
        }
        public bool ShouldSerilizePhotoUrl()
        {
            return shouldSerilizeProperty[nameof(PhotoUrl)];
        }

        private string? traitValue = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TraitValue
        {
            get => ShouldSerilizeTraitValue()
                ? (string.IsNullOrEmpty(traitValue) ? "Undefined" : traitValue)
                : null;
            set => traitValue = value;
        }
        public bool ShouldSerilizeTraitValue()
        {
            return shouldSerilizeProperty[nameof(TraitValue)];
        }

        private string? traitTooltip = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TraitTooltip
        {
            get => ShouldSerilizeTraitTooltip()
                ? (string.IsNullOrEmpty(traitTooltip) ? "Undefined" : traitTooltip)
                : null;
            set => traitTooltip = value;
        }
        public bool ShouldSerilizeTraitTooltip()
        {
            return shouldSerilizeProperty[nameof(TraitTooltip)];
        }
    }
}

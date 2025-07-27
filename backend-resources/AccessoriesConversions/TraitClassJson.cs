namespace AccessoriesConversions
{
    public class TraitClassJson
    {
        private static Dictionary<string, bool> shouldSerilizeProperty = typeof(TraitClassJson).GetProperties().ToDictionary(p => p.Name, p => true);
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


        private string? photoUrl = string.Empty;
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

        private string? name = string.Empty;
        public string? Name
        {
            get => ShouldSerilizeName()
                ? (string.IsNullOrEmpty(name) ? "Undefined" : name)
                : null;
            set => name = value;
        }
        public bool ShouldSerilizeName()
        {
            return shouldSerilizeProperty[nameof(Name)];
        }

        private string? description = string.Empty;
        public string? Description
        {
            get => ShouldSerilizeDescription()
                ? (string.IsNullOrEmpty(description) ? "Undefined" : description)
                : null;
            set => description = value;
        }
        public bool ShouldSerilizeDescription()
        {
            return shouldSerilizeProperty[nameof(Description)];
        }

        private string? extendedDescription = string.Empty;
        public string? ExtendedDescription
        {
            get => ShouldSerilizeExtendedDescription()
                ? (string.IsNullOrEmpty(extendedDescription) ? "Undefined" : extendedDescription)
                : null;
            set => extendedDescription = value;
        }
        public bool ShouldSerilizeExtendedDescription()
        {
            return shouldSerilizeProperty[nameof(ExtendedDescription)];
        }

        private List<string>? traitValues = new();
        public List<string>? TraitValues
        {
            get => ShouldSerilizeTraitValues()
                ? (traitValues is null ? new() : traitValues)
                : null;
            set => traitValues = value;
        }
        public bool ShouldSerilizeTraitValues()
        {
            return shouldSerilizeProperty[nameof(TraitValues)];
        }

    }
}

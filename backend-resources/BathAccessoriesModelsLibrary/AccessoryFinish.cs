namespace BathAccessoriesModelsLibrary
{
    public class AccessoryFinish
    {
        public AccessoryTrait Finish { get; set; } = AccessoryTrait.Empty(TypeOfTrait.FinishTrait);
        public string PhotoUrl { get; set; } = string.Empty;
        public string PdfUrl { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;
        public List<string> ExtraPhotosUrl { get; set; } = new();

        public static AccessoryFinish Empty() => new();
    }
}

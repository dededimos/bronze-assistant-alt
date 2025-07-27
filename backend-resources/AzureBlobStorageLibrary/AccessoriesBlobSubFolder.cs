using System.ComponentModel;

namespace AzureBlobStorageLibrary
{
    /// <summary>
    /// The Blob SubFolder for the Various Accessories Files (ex. Images for accessories , traits e.t.c.)
    /// </summary>
    public enum AccessoriesBlobSubFolder
    {
        [Description("")]
        None,
        [Description("AccessoriesPhotos")]
        AccessoriesPhotosFolder,
        [Description("TraitClasses")]
        TraitClassesFolder,
        [Description("Sheets")]
        SheetsFolder,
        [Description("TraitClasses/Finishes")]
        FinishesFolder,
        [Description("TraitClasses/Materials")]
        MaterialsFolder,
        [Description("TraitClasses/Sizes")]
        SizesFolder,
        [Description("TraitClasses/Shapes")]
        ShapesTypesFolder,
        [Description("TraitClasses/PrimaryTypes")]
        PrimaryTypesFolder,
        [Description("TraitClasses/SecondaryTypes")]
        SecondaryTypesFolder,
        [Description("TraitClasses/Categories")]
        CategoriesFolder,
        [Description("TraitClasses/Series")]
        SeriesFolder,
        [Description("TraitClasses/MountingTypes")]
        MountingTypesFolder,
        [Description("TraitClasses/Dimensions")]
        DimensionsFolder,
        [Description("TraitClasses/Prices")]
        PricesFolder
    }
}

using SqliteApplicationSettings.DTOs;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService.GlassesStockSettingsService
{
    public partial class GlassesStockSettings : ObservableObject
    {
        /// <summary>
        /// Weather the Heights of glasses should be compared for matching
        /// </summary>
        [ObservableProperty]
        private bool shouldCompareHeight;
        /// <summary>
        /// The Max Difference the heights can have
        /// </summary>
        [ObservableProperty]
        private double allowedHeightDifference;
        /// <summary>
        /// Weather the Lengths of glasses should be compared for matching
        /// </summary>
        [ObservableProperty]
        private bool shouldCompareLength;
        /// <summary>
        /// The Max Difference the Lengths can have
        /// </summary>
        [ObservableProperty]
        private double allowedLengthDifference;
        /// <summary>
        /// Weather the Thicknesses of glasses should be compared for matching
        /// </summary>
        [ObservableProperty]
        private bool shouldCompareThickness;
        /// <summary>
        /// Weather the Finishes of glasses should be compared for matching
        /// </summary>
        [ObservableProperty]
        private bool shouldCompareFinish;
        /// <summary>
        /// The Model that these Settings Concern
        /// </summary>
        [ObservableProperty]
        private CabinModelEnum concernsModel;

        [ObservableProperty]
        private int id;
        [ObservableProperty]
        private DateTime created;
        [ObservableProperty]
        private DateTime lastModified;

        public GlassesStockSettings()
        {

        }
        public GlassesStockSettings(GlassesStockServiceSettingsDTO dto)
        {
            Id = dto.Id;
            Created = dto.Created;
            LastModified= dto.LastModified;
            ConcernsModel= dto.ConcernsModel;
            AllowedHeightDifference= dto.AllowedHeightDifference;
            AllowedLengthDifference= dto.AllowedLengthDifference;
            ShouldCompareHeight= dto.ShouldCompareHeight;
            ShouldCompareLength= dto.ShouldCompareLength;
            ShouldCompareThickness= dto.ShouldCompareThickness;
            ShouldCompareFinish= dto.ShouldCompareFinish;
        }

        /// <summary>
        /// Produces the DTO for this Settings
        /// </summary>
        /// <returns></returns>
        public GlassesStockServiceSettingsDTO ToDto()
        {
            return new()
            {
                Id = Id,
                LastModified = LastModified,
                Created = Created,
                IsDefault = false, //never change the defaults of db
                IsSelected = true, //always is selected as a User Setting
                ConcernsModel = ConcernsModel,
                AllowedHeightDifference= AllowedHeightDifference,
                ShouldCompareHeight= ShouldCompareHeight,
                AllowedLengthDifference= AllowedLengthDifference,
                ShouldCompareLength= ShouldCompareLength,
                ShouldCompareThickness= ShouldCompareThickness,
                ShouldCompareFinish= ShouldCompareFinish,
                InfoStringObject = $"UserSettings-{ConcernsModel}"
            };
        }
    }
}

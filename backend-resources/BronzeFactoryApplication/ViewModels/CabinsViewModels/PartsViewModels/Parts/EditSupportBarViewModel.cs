using DataAccessLib.NoSQLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class EditSupportBarViewModel : EditPartViewModel
    {
        [ObservableProperty]
        private int outOfGlassHeight;
        [ObservableProperty]
        private SupportBarPlacement placement;
        [ObservableProperty]
        private int clampViewLength;
        [ObservableProperty]
        private int clampViewHeight;
        [ObservableProperty]
        private double clampCenterDistanceFromGlassDefault;
        [ObservableProperty]
        private double clampCenterDistanceFromGlass;

        private void InitilizeSupportBarViewModel(SupportBar supportBar)
        {
            this.OutOfGlassHeight= supportBar.OutOfGlassHeight;
            this.Placement= supportBar.Placement;
            this.ClampViewLength = supportBar.ClampViewLength;
            this.ClampViewHeight = supportBar.ClampViewHeight;
            this.ClampCenterDistanceFromGlassDefault = supportBar.ClampCenterDistanceFromGlassDefault;
            this.ClampCenterDistanceFromGlass = supportBar.ClampCenterDistanceFromGlass;
        }

        public EditSupportBarViewModel():base(CabinPartType.BarSupport)
        {

        }

        public EditSupportBarViewModel(CabinPartEntity entity, bool isEdit = true) : base(entity,isEdit)
        {
            SupportBar supportBar = entity.Part as SupportBar ?? throw new ArgumentException($"{nameof(EditSupportBarViewModel)} accepts Only CabinPartEntities of a type {nameof(SupportBar)}");
            InitilizeSupportBarViewModel(supportBar);
        }

        public override SupportBar GetPart()
        {
            SupportBar supportBar = new()
            {
                OutOfGlassHeight= this.OutOfGlassHeight,
                Placement= this.Placement,
                ClampViewLength = this.ClampViewLength,
                ClampViewHeight = this.ClampViewHeight,
                ClampCenterDistanceFromGlassDefault = this.ClampCenterDistanceFromGlassDefault,
                ClampCenterDistanceFromGlass = this.ClampCenterDistanceFromGlass,
            };
            ExtractPropertiesForBasePart(supportBar);
            return supportBar;
        }


    }
}

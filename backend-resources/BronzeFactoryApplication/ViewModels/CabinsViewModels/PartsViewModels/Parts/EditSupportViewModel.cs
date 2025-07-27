using DataAccessLib.NoSQLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class EditSupportViewModel : EditPartViewModel
    {
        [ObservableProperty]
        private int lengthView;
        [ObservableProperty]
        private int heightView;
        [ObservableProperty]
        private int glassGapAER;
        [ObservableProperty]
        private int tollerance;

        //Floor Stopper Specific Properties
        [ObservableProperty]
        private int outOfGlassLength;

        //(Gets Notified with subscribing in PropChange in Constructor)
        public bool IsFloorStopper { get => PartType is CabinPartType.FloorStopperW; }

        private void InitilizeSupportViewModel(CabinSupport support)
        {
            this.LengthView = support.LengthView;
            this.HeightView = support.HeightView;
            this.GlassGapAER = support.GlassGapAER;
            this.Tollerance = support.Tollerance;

            if (support is FloorStopperW fs)
            {
                this.OutOfGlassLength = fs.OutOfGlassLength;
            }
        }

        public EditSupportViewModel(bool isFloorStopper = false):base(isFloorStopper ? CabinPartType.FloorStopperW : CabinPartType.SmallSupport)
        {
            this.PropertyChanged += EditSupportViewModel_PropertyChanged;
        }

        public EditSupportViewModel(CabinPartEntity entity, bool isEdit = true) :base(entity,isEdit)
        {
            CabinSupport support = entity.Part as CabinSupport ?? throw new ArgumentException($"{nameof(EditSupportViewModel)} accepts Only CabinPartEntities of a type {nameof(CabinSupport)}");
            this.PropertyChanged += EditSupportViewModel_PropertyChanged;
            InitilizeSupportViewModel(support);
        }

        public override CabinSupport GetPart()
        {
            CabinSupport support;
            if (PartType is CabinPartType.FloorStopperW)
            {
                support = new FloorStopperW() { OutOfGlassLength = this.OutOfGlassLength };
            }
            else
            {
                support = new CabinSupport();
            }
            support.LengthView = this.LengthView;
            support.HeightView = this.HeightView;
            support.GlassGapAER = this.GlassGapAER;
            support.Tollerance = this.Tollerance;

            ExtractPropertiesForBasePart(support);
            return support;
        }


        /// <summary>
        /// Subscribes to its own property changes (for the parent property Part Type to inform also the IsFloorStopper property )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditSupportViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(PartType))
            {
                OnPropertyChanged(nameof(IsFloorStopper));
            }
        }


    }
}

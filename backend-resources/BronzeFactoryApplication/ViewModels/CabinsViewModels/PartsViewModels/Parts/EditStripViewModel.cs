using DataAccessLib.NoSQLModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts
{
    public partial class EditStripViewModel : EditPartViewModel
    {
        [ObservableProperty]
        private CabinStripType stripType;
        [ObservableProperty]
        private double cutLength;
        [ObservableProperty]
        private int outOfGlassLength;
        [ObservableProperty]
        private int inGlassLength;
        [ObservableProperty]
        private int metalLength;
        [ObservableProperty]
        private int polycarbonicLength;

        public EditStripViewModel():base(CabinPartType.Strip)
        {

        }

        public EditStripViewModel(CabinPartEntity entity, bool isEdit = true) : base(entity,isEdit)
        {
            GlassStrip strip = entity.Part as GlassStrip ?? throw new ArgumentException($"{nameof(EditStripViewModel)} accepts Only CabinPartEntities of a type {nameof(GlassStrip)}");
            InitilizeStripViewmodel(strip);
        }

        private void InitilizeStripViewmodel(GlassStrip strip)
        {
            this.StripType = strip.StripType;
            this.CutLength = strip.CutLength;
            this.OutOfGlassLength = strip.OutOfGlassLength;
            this.InGlassLength = strip.InGlassLength;
            this.MetalLength = strip.MetalLength;
            this.PolycarbonicLength = strip.PolycarbonicLength;
        }

        public override GlassStrip GetPart()
        {
            GlassStrip strip = new(StripType)
            {
                CutLength = this.CutLength,
                OutOfGlassLength = this.OutOfGlassLength,
                InGlassLength = this.InGlassLength,
                MetalLength = this.MetalLength,
                PolycarbonicLength = this.PolycarbonicLength,
            };
            
            ExtractPropertiesForBasePart(strip);
            
            return strip;
        }

    }
}

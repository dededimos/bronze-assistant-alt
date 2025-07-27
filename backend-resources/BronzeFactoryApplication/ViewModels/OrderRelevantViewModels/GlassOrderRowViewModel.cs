using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.OrderRelevantViewModels
{
    public partial class GlassOrderRowViewModel : BaseViewModel
    {
        private GlassOrderRow? _row;

        public string OrderId { get => _row!.OrderId; }
        public string ReferencePA0 { get => _row!.ReferencePA0; }
        public GlassDrawEnum Draw { get => _row!.OrderedGlass.Draw; }
        public double Length { get => _row!.OrderedGlass.Length; }
        public double Height { get => _row!.OrderedGlass.Height; }
        public GlassTypeEnum GlassType { get => _row!.OrderedGlass.GlassType; }
        public GlassThicknessEnum GlassThickness { get => _row!.OrderedGlass.Thickness ?? GlassThicknessEnum.GlassThicknessNotSet; }
        public string Notes { get => _row!.Notes; }
        public int Quantity { get => _row!.Quantity; }
        public Guid CabinRowKey { get => _row!.CabinRowKey; }
        public bool HasStep { get => _row!.OrderedGlass.HasStep; }
        public double StepLength { get => _row!.OrderedGlass.StepLength; }
        public double StepHeight { get => _row!.OrderedGlass.StepHeight; }
        public string StepRepresentation { get => _row!.OrderedGlass.HasStep ? $"{_row!.OrderedGlass.StepLength}x{_row!.OrderedGlass.StepHeight}mm" : "N/A"; }
        public int? SpecialDrawNumber { get => _row!.SpecialDrawNumber; }
        public string? SpecialDrawString { get => _row!.SpecialDrawString; }

        public int FilledQuantity
        {
            get => _row!.FilledQuantity;
            set
            {
                if (SetProperty(_row!.FilledQuantity, value, _row, (r, fQ) => r.FilledQuantity = fQ))
                    OnPropertyChanged(nameof(IsFilled));
            }
        }

        public bool IsFilled { get => _row!.FilledQuantity >= _row!.Quantity; }

        public GlassOrderRowViewModel()
        {
            
        }

        public void InitilizeViewModel(GlassOrderRow row)
        {
            _row = row;
        }

        public GlassOrderRow GetModel() => _row!;
    }
}

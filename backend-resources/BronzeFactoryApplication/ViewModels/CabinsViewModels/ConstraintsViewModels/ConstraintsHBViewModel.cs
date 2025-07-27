using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsHBViewModel : ConstraintsViewModel<ConstraintsHB>
    {
        public int? MaxDoorLength
        {
            get => ConstraintsObject?.MaxDoorLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxDoorLength != value)
                {
                    ConstraintsObject.MaxDoorLength = value ?? 0;
                    OnPropertyChanged(nameof(MaxDoorLength));
                }
            }
        }
        public int? MinDoorLength
        {
            get => ConstraintsObject?.MinDoorLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinDoorLength != value)
                {
                    ConstraintsObject.MinDoorLength = value ?? 0;
                    OnPropertyChanged(nameof(MinDoorLength));
                }
            }
        }
        public int? MaxFixedLength
        {
            get => ConstraintsObject?.MaxFixedLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxFixedLength != value)
                {
                    ConstraintsObject.MaxFixedLength = value ?? 0;
                    OnPropertyChanged(nameof(MaxFixedLength));
                }
            }
        }
        public int? MinFixedLength
        {
            get => ConstraintsObject?.MinFixedLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinFixedLength != value)
                {
                    ConstraintsObject.MinFixedLength = value ?? 0;
                    OnPropertyChanged(nameof(MinFixedLength));
                }
            }
        }
        public int? DoorDistanceFromBottom
        {
            get => ConstraintsObject?.DoorDistanceFromBottom;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.DoorDistanceFromBottom != value)
                {
                    ConstraintsObject.DoorDistanceFromBottom = value ?? 0;
                    OnPropertyChanged(nameof(DoorDistanceFromBottom));
                }
            }
        }
        public int? CornerRadiusTopEdge
        {
            get => ConstraintsObject?.CornerRadiusTopEdge;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CornerRadiusTopEdge != value)
                {
                    ConstraintsObject.CornerRadiusTopEdge = value ?? 0;
                    OnPropertyChanged(nameof(CornerRadiusTopEdge));
                }
            }
        }
        public int? StepHeightTollerance
        {
            get => ConstraintsObject?.StepHeightTollerance;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.StepHeightTollerance != value)
                {
                    ConstraintsObject.StepHeightTollerance = value ?? 0;
                    OnPropertyChanged(nameof(StepHeightTollerance));
                }
            }
        }
        public int? PartialLength
        {
            get => ConstraintsObject?.PartialLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.PartialLength != value)
                {
                    ConstraintsObject.PartialLength = value ?? 0;
                    OnPropertyChanged(nameof(PartialLength));
                }
            }
        }
        public int? DoorSealerLengthCorrection
        {
            get => ConstraintsObject?.DoorSealerLengthCorrection;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.DoorSealerLengthCorrection != value)
                {
                    ConstraintsObject.DoorSealerLengthCorrection = value ?? 0;
                    OnPropertyChanged(nameof(DoorSealerLengthCorrection));
                }
            }
        }
        public LengthCalculationOption LengthCalculation
        {
            get => ConstraintsObject?.LengthCalculation ?? LengthCalculationOption.ByDoorLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.LengthCalculation != value)
                {
                    ConstraintsObject.LengthCalculation = value;
                    OnPropertyChanged(nameof(LengthCalculation));
                }
            }
        }
        public IEnumerable<LengthCalculationOption> SelectableLengthCalculationOptions { get => Enum.GetValues(typeof(LengthCalculationOption)).Cast<LengthCalculationOption>(); }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsHB ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsHB)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

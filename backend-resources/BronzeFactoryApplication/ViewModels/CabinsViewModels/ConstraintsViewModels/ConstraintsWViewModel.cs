using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsWViewModel : ConstraintsViewModel<ConstraintsW>
    {
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
        public int? StepLengthTolleranceMin
        {
            get => ConstraintsObject?.StepLengthTolleranceMin;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.StepLengthTolleranceMin != value)
                {
                    ConstraintsObject.StepLengthTolleranceMin = value ?? 0;
                    OnPropertyChanged(nameof(StepLengthTolleranceMin));
                }
            }
        }
        public bool CanHavePerimetricalFrame 
        {
            get => ConstraintsObject?.CanHavePerimetricalFrame ?? false;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CanHavePerimetricalFrame != value)
                {
                    ConstraintsObject.CanHavePerimetricalFrame = value;
                    OnPropertyChanged(nameof(CanHavePerimetricalFrame));
                }
            }
        }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsW ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsW)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

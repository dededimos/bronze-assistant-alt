using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsVFViewModel : ConstraintsViewModel<ConstraintsVF>
    {
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
        /// <summary>
        /// The Extra Tollerance on Step Length Cutting , when there is no Tollerance from Profile
        /// </summary>
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

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsVF ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsVF)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

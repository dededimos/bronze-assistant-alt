using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class Constraints9FViewModel : ConstraintsViewModel<Constraints9F>
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

        public int? SealerL0LengthCorrection
        {
            get => ConstraintsObject?.SealerL0LengthCorrection;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.SealerL0LengthCorrection != value)
                {
                    ConstraintsObject.SealerL0LengthCorrection = value ?? 0;
                    OnPropertyChanged(nameof(SealerL0LengthCorrection));
                }
            }
        }
        /// <summary>
        /// Always 0 , setter is Empty
        /// </summary>
        public override int FinalHeightCorrection { get => 0; set { } }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as Constraints9F ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(Constraints9F)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

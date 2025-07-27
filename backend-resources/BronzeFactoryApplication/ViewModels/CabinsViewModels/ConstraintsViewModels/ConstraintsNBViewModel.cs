using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsNBViewModel : ConstraintsViewModel<ConstraintsNB>
    {
        public int? DoorHeightAdjustment
        {
            get => ConstraintsObject?.DoorHeightAdjustment;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.DoorHeightAdjustment != value)
                {
                    ConstraintsObject.DoorHeightAdjustment = value ?? 0;
                    OnPropertyChanged(nameof(DoorHeightAdjustment));
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
        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsNB ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsNB)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

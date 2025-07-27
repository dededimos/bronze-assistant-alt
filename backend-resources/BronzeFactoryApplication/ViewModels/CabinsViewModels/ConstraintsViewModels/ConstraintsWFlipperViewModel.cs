using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsWFlipperViewModel : ConstraintsViewModel<ConstraintsWFlipper>
    {
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

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsWFlipper ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsWFlipper)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

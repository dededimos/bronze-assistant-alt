using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsDBViewModel : ConstraintsViewModel<ConstraintsDB>
    {
        public int? MaxDoorGlassLength
        {
            get => ConstraintsObject?.MaxDoorGlassLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxDoorGlassLength != value)
                {
                    ConstraintsObject.MaxDoorGlassLength = value ?? 0;
                    OnPropertyChanged(nameof(MaxDoorGlassLength));
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
            constraintsObject = constraints as ConstraintsDB ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsDB)}");
            //Inform all Changed in the Cabin ViewModel
            
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

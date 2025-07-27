using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsWSViewModel : ConstraintsViewModel<ConstraintsWS>
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

        /// <summary>
        /// The Minimum Possible Overlap of Glasses with the Fixed Parts
        /// </summary>
        public int? MinimumGlassOverlapping
        {
            get => ConstraintsObject?.MinimumGlassOverlapping;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinimumGlassOverlapping != value)
                {
                    ConstraintsObject.MinimumGlassOverlapping = value ?? 0;
                    OnPropertyChanged(nameof(MinimumGlassOverlapping));
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
        public int? Overlap
        {
            get => ConstraintsObject?.Overlap;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.Overlap != value)
                {
                    ConstraintsObject.Overlap = value ?? 0;
                    OnPropertyChanged(nameof(Overlap));
                }
            }
        }
        public int? CoverDistance
        {
            get => ConstraintsObject?.CoverDistance;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CoverDistance != value)
                {
                    ConstraintsObject.CoverDistance = value ?? 0;
                    OnPropertyChanged(nameof(CoverDistance));
                }
            }
        }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as ConstraintsWS ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsWS)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

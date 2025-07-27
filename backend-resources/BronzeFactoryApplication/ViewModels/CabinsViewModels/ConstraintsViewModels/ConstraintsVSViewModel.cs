using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class ConstraintsVSViewModel : ConstraintsViewModel<ConstraintsVS>
    {
        /// <summary>
        /// The Height Breakpoint at which the MaxDoor Length Changes
        /// </summary>
        public int? BreakpointHeight
        {
            get => ConstraintsObject?.BreakpointHeight;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.BreakpointHeight != value)
                {
                    ConstraintsObject.BreakpointHeight = value ?? 0;
                    OnPropertyChanged(nameof(BreakpointHeight));
                }
            }
        }
        /// <summary>
        /// The Maximum Door Length when Height is Before or Equal to the beakpoint Height
        /// </summary>
        public int? MaxDoorLengthBeforeBreakpoint
        {
            get => ConstraintsObject?.MaxDoorLengthBeforeBreakpoint;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxDoorLengthBeforeBreakpoint != value)
                {
                    ConstraintsObject.MaxDoorLengthBeforeBreakpoint = value ?? 0;
                    OnPropertyChanged(nameof(MaxDoorLengthBeforeBreakpoint));
                }
            }
        }
        /// <summary>
        /// The Maximum Door Length when Height is Greater than the Brakpoint height
        /// </summary>
        public int? MaxDoorLengthAfterBreakpoint
        {
            get => ConstraintsObject?.MaxDoorLengthAfterBreakpoint;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxDoorLengthAfterBreakpoint != value)
                {
                    ConstraintsObject.MaxDoorLengthAfterBreakpoint = value ?? 0;
                    OnPropertyChanged(nameof(MaxDoorLengthAfterBreakpoint));
                }
            }
        }
        /// <summary>
        /// The Correction Length of the Main Bar (Used in Glass Calculations)
        /// </summary>
        public int? BarCorrectionLength
        {
            get => ConstraintsObject?.BarCorrectionLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.BarCorrectionLength != value)
                {
                    ConstraintsObject.BarCorrectionLength = value ?? 0;
                    OnPropertyChanged(nameof(BarCorrectionLength));
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
        public int? MinDoorDistanceFromWallOpened
        {
            get => ConstraintsObject?.MinDoorDistanceFromWallOpened;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinDoorDistanceFromWallOpened != value)
                {
                    ConstraintsObject.MinDoorDistanceFromWallOpened = value ?? 0;
                    OnPropertyChanged(nameof(MinDoorDistanceFromWallOpened));
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
            constraintsObject = constraints as ConstraintsVS ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(ConstraintsVS)}");
            //Inform all Changed in the Cabin ViewModel
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

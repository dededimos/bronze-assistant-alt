using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class Constraints9BViewModel : ConstraintsViewModel<Constraints9B>
    {
        public int? MinPossibleDoorLength
        {
            get => ConstraintsObject?.MinPossibleDoorLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinPossibleDoorLength != value)
                {
                    ConstraintsObject.MinPossibleDoorLength = value ?? 0;
                    OnPropertyChanged(nameof(MinPossibleDoorLength));
                }
            }
        }
        public int? MinPossibleFixedLength
        {
            get => ConstraintsObject?.MinPossibleFixedLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinPossibleFixedLength != value)
                {
                    ConstraintsObject.MinPossibleFixedLength = value ?? 0;
                    OnPropertyChanged(nameof(MinPossibleFixedLength));
                }
            }
        }
        public int? MaxPossibleDoorLength
        {
            get => ConstraintsObject?.MaxPossibleDoorLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxPossibleDoorLength != value)
                {
                    ConstraintsObject.MaxPossibleDoorLength = value ?? 0;
                    OnPropertyChanged(nameof(MaxPossibleDoorLength));
                }
            }
        }

        public int? AddedFixedGlassLengthBreakpoint
        {
            get => ConstraintsObject?.AddedFixedGlassLengthBreakpoint;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.AddedFixedGlassLengthBreakpoint != value)
                {
                    ConstraintsObject.AddedFixedGlassLengthBreakpoint = value ?? 0;
                    OnPropertyChanged(nameof(AddedFixedGlassLengthBreakpoint));
                }
            }
        }

        public int? HingeDistanceFromDoorGlass
        {
            get => ConstraintsObject?.HingeDistanceFromDoorGlass;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.HingeDistanceFromDoorGlass != value)
                {
                    ConstraintsObject.HingeDistanceFromDoorGlass = value ?? 0;
                    OnPropertyChanged(nameof(HingeDistanceFromDoorGlass));
                }
            }
        }

        public int? GlassGapAERHorizontal
        {
            get => ConstraintsObject?.GlassGapAERHorizontal;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.GlassGapAERHorizontal != value)
                {
                    ConstraintsObject.GlassGapAERHorizontal = value ?? 0;
                    OnPropertyChanged(nameof(GlassGapAERHorizontal));
                }
            }
        }

        public int? GlassGapAERVertical
        {
            get => ConstraintsObject?.GlassGapAERVertical;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.GlassGapAERVertical != value)
                {
                    ConstraintsObject.GlassGapAERVertical = value ?? 0;
                    OnPropertyChanged(nameof(GlassGapAERVertical));
                }
            }
        }

        public int? CorrectionOfL0Length
        {
            get => ConstraintsObject?.CorrectionOfL0Length;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CorrectionOfL0Length != value)
                {
                    ConstraintsObject.CorrectionOfL0Length = value ?? 0;
                    OnPropertyChanged(nameof(CorrectionOfL0Length));
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
        /// Always zero , Setter is Empty
        /// </summary>
        public override int FinalHeightCorrection { get => 0; set { } }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as Constraints9B ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(Constraints9B)}");
            //Inform all Changed in the Cabin ViewModel
            
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

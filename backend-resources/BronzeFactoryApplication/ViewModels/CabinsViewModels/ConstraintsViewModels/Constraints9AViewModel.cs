using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class Constraints9AViewModel : ConstraintsViewModel<Constraints9A>
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
        public int? OverlapEPIK
        {
            get => ConstraintsObject?.Overlap;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.Overlap != value)
                {
                    ConstraintsObject.Overlap = value ?? 0;
                    OnPropertyChanged(nameof(OverlapEPIK));
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
        /// Always zero , Setter is empty
        /// </summary>
        public override int FinalHeightCorrection { get => 0; set { } }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as Constraints9A ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(Constraints9A)}");
            //Inform all Changed in the Cabin ViewModel

            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
}

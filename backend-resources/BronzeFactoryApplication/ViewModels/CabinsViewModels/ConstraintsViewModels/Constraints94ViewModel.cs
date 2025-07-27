using Microsoft.AspNetCore.Identity;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels
{
    public partial class Constraints94ViewModel : ConstraintsViewModel<Constraints94>
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
                    OnPropertyChanged(nameof(CoverDistanceOptionSelection));
                }
            }
        }
        public int? CoverDistanceMaxOpening
        {
            get => ConstraintsObject?.CoverDistanceMaxOpening;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CoverDistanceMaxOpening != value)
                {
                    ConstraintsObject.CoverDistanceMaxOpening = value ?? 0;
                    OnPropertyChanged(nameof(CoverDistanceMaxOpening));
                }
            }
        }
        public int? CoverDistanceSameGlasses
        {
            get => ConstraintsObject?.CoverDistanceSameGlasses;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.CoverDistanceSameGlasses != value)
                {
                    ConstraintsObject.CoverDistanceSameGlasses = value ?? 0;
                    OnPropertyChanged(nameof(CoverDistanceSameGlasses));
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
        /// Changes Cover Distance According to Selection
        /// </summary>
        public CoverDistanceOption CoverDistanceOptionSelection
        {
            get
            {
                if (CoverDistance == CoverDistanceSameGlasses)
                {
                    return CoverDistanceOption.SameGlasses;
                }
                else if (CoverDistance == CoverDistanceMaxOpening)
                {
                    return CoverDistanceOption.MaxOpening;
                }
                return CoverDistanceOption.CustomCoverDistance;
            }
            set
            {
                if (value == CoverDistanceOption.SameGlasses)
                {
                    CoverDistance = CoverDistanceSameGlasses;
                }
                else if (value == CoverDistanceOption.MaxOpening)
                {
                    CoverDistance = CoverDistanceMaxOpening;
                }
            }
        }
        public IEnumerable<CoverDistanceOption> SelectableCoverDistanceOptions { get => new List<CoverDistanceOption>() { CoverDistanceOption.SameGlasses, CoverDistanceOption.MaxOpening,CoverDistanceOption.CustomCoverDistance }; }

        /// <summary>
        /// Always zero , setter is empty
        /// </summary>
        public override int FinalHeightCorrection { get => 0; set{ } }

        public IEnumerable<int> SameGlassesLengths { get => constraintsObject?.SameGlassesLengths ?? new List<int>(); }

        public override void SetNewConstraints(CabinConstraints constraints)
        {
            base.SetNewConstraints(constraints);
            constraintsObject = constraints as Constraints94 ?? throw new InvalidOperationException($"Provided Constraints where of type {constraints.GetType().Name} -- and not of the expected type : {nameof(Constraints94)}");
            
            //copy to store defaults
            defaults = new(constraintsObject);
        }
    }
    public enum CoverDistanceOption
    {
        /// <summary>
        /// When the Cover Distance is Set to a Custom Value
        /// </summary>
        CustomCoverDistance,
        /// <summary>
        /// When the Cover Distance is Set to Same Glasses
        /// </summary>
        SameGlasses,
        /// <summary>
        /// When the Cover Distance is Set to Max Opening
        /// </summary>
        MaxOpening
    }

}

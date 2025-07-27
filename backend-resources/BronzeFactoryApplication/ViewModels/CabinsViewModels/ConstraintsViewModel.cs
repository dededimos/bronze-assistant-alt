using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels
{
    public partial class ConstraintsViewModel : BaseViewModel
        
    {
        private CabinConstraints? constraintsObject;
        protected virtual CabinConstraints? ConstraintsObject  => constraintsObject;
        public virtual CabinConstraints? Defaults { get; }

        public int? MinPossibleLength
        { 
            get => ConstraintsObject?.MinPossibleLength; 
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinPossibleLength != value)
                {
                    ConstraintsObject.MinPossibleLength = value ?? 0;
                    OnPropertyChanged(nameof(MinPossibleLength));
                }
            }
        }
        public int? MaxPossibleLength 
        { 
            get => ConstraintsObject?.MaxPossibleLength;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxPossibleLength != value)
                {
                    ConstraintsObject.MaxPossibleLength = value ?? 0;
                    OnPropertyChanged(nameof(MaxPossibleLength));
                }
            }
        }
        public int? MaxPossibleHeight 
        { 
            get => ConstraintsObject?.MaxPossibleHeight;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MaxPossibleHeight != value)
                {
                    ConstraintsObject.MaxPossibleHeight = value ?? 0;
                    OnPropertyChanged(nameof(MaxPossibleHeight));
                }
            }
        }
        public int? MinPossibleHeight 
        { 
            get => ConstraintsObject?.MinPossibleHeight;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinPossibleHeight != value)
                {
                    ConstraintsObject.MinPossibleHeight = value ?? 0;
                    OnPropertyChanged(nameof(MinPossibleHeight));
                }
            }
        }
        public int? MinPossibleStepHeight 
        { 
            get => ConstraintsObject?.MinPossibleStepHeight;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.MinPossibleStepHeight != value)
                {
                    ConstraintsObject.MinPossibleStepHeight = value ?? 0;
                    OnPropertyChanged(nameof(MinPossibleStepHeight));
                }
            }
        }
        public int? TolleranceMinusDefaultMinimum 
        { 
            get => ConstraintsObject?.TolleranceMinusDefaultMinimum;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.TolleranceMinusDefaultMinimum != value)
                {
                    ConstraintsObject.TolleranceMinusDefaultMinimum = value ?? 0;
                    OnPropertyChanged(nameof(TolleranceMinusDefaultMinimum));
                }
            }
        }
        public int? HeightBreakPointGlassThickness 
        { 
            get => ConstraintsObject?.HeightBreakPointGlassThickness;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.HeightBreakPointGlassThickness != value)
                {
                    ConstraintsObject.HeightBreakPointGlassThickness = value ?? 0;
                    OnPropertyChanged(nameof(HeightBreakPointGlassThickness));
                }
            }
        }
        public int? LengthBreakPointGlassThickness 
        { 
            get => ConstraintsObject?.LengthBreakPointGlassThickness;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.LengthBreakPointGlassThickness != value)
                {
                    ConstraintsObject.LengthBreakPointGlassThickness = value ?? 0;
                    OnPropertyChanged(nameof(LengthBreakPointGlassThickness));
                }
            }
        }
        public CabinThicknessEnum BreakPointMinThickness 
        { 
            get => ConstraintsObject?.BreakPointMinThickness ?? CabinThicknessEnum.NotSet;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.BreakPointMinThickness != value)
                {
                    ConstraintsObject.BreakPointMinThickness = value;
                    OnPropertyChanged(nameof(BreakPointMinThickness));
                }
            }
        }
        public bool ShouldHaveHandle 
        { 
            get => ConstraintsObject?.ShouldHaveHandle ?? false;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.ShouldHaveHandle != value)
                {
                    ConstraintsObject.ShouldHaveHandle = value;
                    OnPropertyChanged(nameof(ShouldHaveHandle));
                }
            }
        }
        public bool CanHaveStep => constraintsObject?.CanHaveStep ?? false;
        public virtual int FinalHeightCorrection 
        { 
            get => ConstraintsObject?.FinalHeightCorrection ?? 0;
            set
            {
                if (ConstraintsObject is not null && ConstraintsObject.FinalHeightCorrection != value)
                {
                    ConstraintsObject.FinalHeightCorrection = value;
                    OnPropertyChanged(nameof(FinalHeightCorrection));
                }
            }
        }

        /// <summary>
        /// Checks weather the Final Height Correction is Overriden , if it is then it cannot be edited !!!
        /// </summary>
        public bool CanEditFinalHeight => this?.GetType().GetProperty(nameof(FinalHeightCorrection))?.DeclaringType == typeof(ConstraintsViewModel);
        

        public IEnumerable<CabinDrawNumber> ValidDrawNumbers { get => ConstraintsObject?.ValidDrawNumbers ?? new List<CabinDrawNumber>(); }
        public IEnumerable<CabinThicknessEnum> ValidThicknesses { get => ConstraintsObject?.ValidThicknesses ?? new List<CabinThicknessEnum>(); }
        public IEnumerable<GlassFinishEnum> ValidGlassFinishes { get => ConstraintsObject?.ValidGlassFinishes ?? new List<GlassFinishEnum>(); }
        public IEnumerable<CabinFinishEnum> ValidMetalFinishes { get => ConstraintsObject?.ValidMetalFinishes ?? new List<CabinFinishEnum>(); }



        /// <summary>
        /// Initilize the ViewModel
        /// </summary>
        /// <param name="constraints"></param>
        public virtual void SetNewConstraints(CabinConstraints constraints)
        {
            this.constraintsObject = constraints;
        }

        public CabinConstraints? GetConstraintsObject()
        {
            return constraintsObject;
        }
    }

    public partial class ConstraintsViewModel<T> : ConstraintsViewModel
        where T : CabinConstraints
    {
        protected T? constraintsObject;
        protected T? defaults;
        protected override T? ConstraintsObject => constraintsObject;
        public override T? Defaults => defaults;
    }
}

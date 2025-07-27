using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Models.SeriesModels
{
    public abstract class CabinConstraints : IDeepClonable<CabinConstraints>
    {
        public int MinPossibleLength { get; set; }
        public int MaxPossibleLength { get; set; }
        public int MaxPossibleHeight { get; set; }
        public int MinPossibleHeight { get; set; }
        public int MinPossibleStepHeight { get; set; }
        /// <summary>
        /// The Default Tollerance Minus At least 'this number' from the Available or less if the Available is less
        /// </summary>
        public int TolleranceMinusDefaultMinimum { get; set; }
        /// <summary>
        /// The Breakpoint Height at which the Glass Thickness must be above a certain level
        /// </summary>
        public int HeightBreakPointGlassThickness { get; set; }
        /// <summary>
        /// The Breakpoint Length at which the Glass Thickness must be above a certain level
        /// </summary>
        public int LengthBreakPointGlassThickness { get; set; }
        /// <summary>
        /// The Minimum Thickness after the Height or Length Glass Thickness Breakpoints are surpassed
        /// </summary>
        public CabinThicknessEnum BreakPointMinThickness { get; set; }
        /// <summary>
        /// Weather the Model must Have a Handle
        /// </summary>
        public bool ShouldHaveHandle { get; set;}
        /// <summary>
        /// Weather the Model can Have a Step Cut
        /// </summary>
        public bool CanHaveStep { get; set; }
        /// <summary>
        /// The Glasses Allowed by this Constraints object
        /// </summary>
        public List<AllowedGlass> AllowedGlasses { get; set; } = new();
        /// <summary>
        /// The Maximum Glasses Allowed by this Constraints object
        /// </summary>
        public virtual int MaximumAllowedGlasses { get => AllowedGlasses.Sum(ag => ag.Quantity); }

        /// <summary>
        /// Increasing this will make the Glasses Smaller from their original Height Size
        /// Useful to pass corrections between glasses
        /// </summary>
        public virtual int FinalHeightCorrection { get; set; }
        public List<CabinDrawNumber> ValidDrawNumbers { get; set; } = new();
        public List<CabinThicknessEnum> ValidThicknesses { get; set; } = new();
        public List<GlassFinishEnum> ValidGlassFinishes { get; set; } = new();
        public List<CabinFinishEnum> ValidMetalFinishes { get; set; } = new();


        public CabinConstraints()
        {
            //Set Big Values so it never hits the Break points for models where not Relevant
            HeightBreakPointGlassThickness = 5000;
            LengthBreakPointGlassThickness = 5000;
            BreakPointMinThickness = CabinThicknessEnum.NotSet;
            ConfigureAllowedGlasses();
        }

        /// <summary>
        /// Generates a new constraints reference by Cloning the Properties of the Passed Object
        /// </summary>
        /// <param name="constraints">The Constraints from which to generate the New Reference</param>
        public CabinConstraints(CabinConstraints constraints)
        {
            MinPossibleLength = constraints.MinPossibleLength;
            MaxPossibleLength = constraints.MaxPossibleLength;
            MaxPossibleHeight = constraints.MaxPossibleHeight;
            MinPossibleHeight = constraints.MinPossibleHeight;
            MinPossibleStepHeight = constraints.MinPossibleStepHeight;
            FinalHeightCorrection = constraints.FinalHeightCorrection;
            HeightBreakPointGlassThickness = constraints.HeightBreakPointGlassThickness;
            LengthBreakPointGlassThickness = constraints.LengthBreakPointGlassThickness;
            BreakPointMinThickness = constraints.BreakPointMinThickness;
            TolleranceMinusDefaultMinimum = constraints.TolleranceMinusDefaultMinimum;
            ShouldHaveHandle = constraints.ShouldHaveHandle;
            ValidDrawNumbers = new(constraints.ValidDrawNumbers);
            ValidThicknesses = new(constraints.ValidThicknesses);
            ValidGlassFinishes = new(constraints.ValidGlassFinishes);
            ValidMetalFinishes = new(constraints.ValidMetalFinishes);
            CanHaveStep = constraints.CanHaveStep;
            ConfigureAllowedGlasses();
        }

        public abstract CabinConstraints GetDeepClone();
        protected abstract void ConfigureAllowedGlasses();
        
        public static CabinConstraints Empty()
        {
            return new UndefinedConstraints();
        }
    }

    public class UndefinedConstraints : CabinConstraints
    {
        public override CabinConstraints GetDeepClone()
        {
            throw new NotSupportedException($"{nameof(GetDeepClone)} is not Supported for {nameof(UndefinedConstraints)}");
        }

        protected override void ConfigureAllowedGlasses()
        {
            return;
        }
    }

}

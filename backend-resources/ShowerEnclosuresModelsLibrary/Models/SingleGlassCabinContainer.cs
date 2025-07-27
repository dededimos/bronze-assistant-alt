using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace ShowerEnclosuresModelsLibrary.Models;

/// <summary>
/// A Cabin Container for a Single Glass
/// </summary>
public class SingleGlassCabinContainer : Cabin
{
    public override CabinPartsList Parts { get; } = new();
    public override CabinConstraints Constraints { get; } = new UndefinedConstraints();
    public override CabinSeries Series { get; } = CabinSeries.UndefinedSeries;
    public override bool HasStep => Glasses.FirstOrDefault()?.HasStep ?? false;
    public SingleGlassCabinContainer(UndefinedConstraints constraints , CabinPartsList parts)
    {
        //Do nothing - this is only for The Deserilization Process of the Database here . ActivatorInstance is Used
    }

    public SingleGlassCabinContainer(Glass glass)
    {
        Model = CabinModelEnum.ModelGlassContainer;
        IsPartOfDraw = Enums.ShowerDrawEnums.CabinDrawNumber.None;
        SynthesisModel = Enums.ShowerDrawEnums.CabinSynthesisModel.Primary;
        MetalFinish = CabinFinishEnum.Polished;
        Glasses.Add(glass);
        NominalLength = Convert.ToInt32(glass.Length);
        Height = Convert.ToInt32(glass.Height);
        GlassFinish = glass.Finish;
        Thicknesses = glass.Thickness switch
        {
            GlassThicknessEnum.Thick5mm => CabinThicknessEnum.Thick5mm,
            GlassThicknessEnum.Thick6mm => CabinThicknessEnum.Thick6mm,
            GlassThicknessEnum.Thick8mm => CabinThicknessEnum.Thick8mm,
            GlassThicknessEnum.Thick10mm => CabinThicknessEnum.Thick10mm,
            GlassThicknessEnum.ThickTenplex10mm => CabinThicknessEnum.ThickTenplex10mm,
            _ => CabinThicknessEnum.NotSet,
        };
        
        if (glass.HasStep)
        {
            this.AddExtra(CabinExtraType.StepCut);
            this.GetStepCut().StepLength = Convert.ToInt32(glass.StepLength);
            this.GetStepCut().StepHeight = Convert.ToInt32(glass.StepHeight);
        }
    }

    public override Cabin GetDeepClone()
    {
        var clone = new SingleGlassCabinContainer(this.Glasses.First().GetDeepClone());
        return clone;
    }

    protected override double GetOpening()
    {
        return 0;
    }
}


namespace BronzeFactoryApplication.ViewModels.CabinsViewModels;

public partial class GlassViewModel : BaseViewModel
{
    [ObservableProperty]
    private GlassDrawEnum draw;
    [ObservableProperty]
    private GlassTypeEnum glassType;
    [ObservableProperty]
    private GlassThicknessEnum thickness;
    [ObservableProperty]
    private GlassFinishEnum finish;
    [ObservableProperty]
    private int cornerRadiusTopRight;
    [ObservableProperty]
    private int cornerRadiusTopLeft;
    [ObservableProperty]
    private double height;
    [ObservableProperty]
    private double length;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStep))]
    private double stepLength;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStep))]
    private double stepHeight;

    public bool HasStep { get => StepLength > 0 && StepHeight > 0; }


    public GlassViewModel()
    {
        
    }

    public GlassViewModel SetGlass(Glass glass , bool supressPropertyChange)
    {
        Draw = glass.Draw;
        GlassType = glass.GlassType;
        Thickness = glass.Thickness ?? GlassThicknessEnum.GlassThicknessNotSet;
        Finish = glass.Finish ?? GlassFinishEnum.GlassFinishNotSet;
        CornerRadiusTopRight = glass.CornerRadiusTopRight;
        CornerRadiusTopLeft = glass.CornerRadiusTopLeft;
        Height = glass.Height;
        Length = glass.Length;
        StepLength = glass.StepLength;
        StepHeight = glass.StepHeight;    
        
        if (!supressPropertyChange)
        {
            OnPropertyChanged(string.Empty);
        }
        return this;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(Length).Append(" x ").Append(Height).Append(" x ");
        string thickness = Thickness switch
        {
            GlassThicknessEnum.GlassThicknessNotSet => "N/A",
            GlassThicknessEnum.Thick5mm => "5mm",
            GlassThicknessEnum.Thick6mm => "6mm",
            GlassThicknessEnum.Thick8mm => "8mm",
            GlassThicknessEnum.Thick10mm => "10mm",
            GlassThicknessEnum.ThickTenplex10mm => "10mm TenPlex",
            _ => "N/A"
        };
        builder.Append(thickness);
        return builder.ToString();
    }

    /// <summary>
    /// Returns the Glass Represented by this ViewModel
    /// </summary>
    /// <returns></returns>
    public Glass GetGlass()
    {
        return new Glass()
        {
            CornerRadiusTopLeft = CornerRadiusTopLeft,
            CornerRadiusTopRight = CornerRadiusTopRight,
            Draw = Draw,
            Finish = Finish,
            GlassType= GlassType,
            Height= Height,
            Length= Length,
            Thickness= Thickness,
            StepHeight= StepHeight,
            StepLength= StepLength,
        };
    }

}

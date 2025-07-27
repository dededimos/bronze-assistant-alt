using DataAccessLib.NoSQLModels;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;

public partial class EditAngleViewModel : EditPartViewModel
{
    [ObservableProperty]
    private int angleDistanceFromDoor;
    [ObservableProperty]
    private int angleLengthL0;
    [ObservableProperty]
    private int angleHeight;

    public EditAngleViewModel() :base(CabinPartType.AnglePart)
    {

    }
    public EditAngleViewModel(CabinPartEntity entity, bool isEdit = true) :base(entity,isEdit)
    {
        CabinAngle angle = entity.Part as CabinAngle ?? throw new ArgumentException($"{nameof(EditAngleViewModel)} accept Only CabinPartEntities of a type {nameof(CabinAngle)}");
        InitilizeAngleViewModel(angle);
    }

    private void InitilizeAngleViewModel(CabinAngle part)
    {
        this.AngleDistanceFromDoor = part.AngleDistanceFromDoor;
        this.AngleLengthL0 = part.AngleLengthL0;
        this.AngleHeight = part.AngleHeight;
    }

    public override CabinAngle GetPart()
    {
        CabinAngle angle = new();
        ExtractPropertiesForBasePart(angle);
        
        angle.AngleDistanceFromDoor = AngleDistanceFromDoor;
        angle.AngleLengthL0 = AngleLengthL0;
        angle.AngleHeight = AngleHeight;
        
        return angle;
    }

}

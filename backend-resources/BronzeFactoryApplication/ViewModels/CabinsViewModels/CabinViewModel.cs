using BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels;

public partial class CabinViewModel : BaseViewModel
{
    private Cabin? cabinObject;
    public virtual Cabin? CabinObject { get => cabinObject; }

    private readonly ConstraintsViewModel constraints;
    public virtual ConstraintsViewModel Constraints { get => constraints; }

    private readonly PartsViewModel parts;
    public virtual PartsViewModel Parts { get => parts; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsInSwapGlassMode))]
    private GlassSwap? glassSwap;

    public bool IsInSwapGlassMode { get => GlassSwap is not null; }

    public CabinIdentifier Identifier { get => cabinObject?.Identifier() ?? new(); }

    public List<Glass> Glasses { get => cabinObject?.Glasses ?? new(); }


    public string Code { get => CabinObject?.Code ?? string.Empty; }
    public CabinModelEnum? Model { get => CabinObject?.Model; }
    public CabinSynthesisModel? SynthesisModel { get => CabinObject?.SynthesisModel; }
    public bool IsPrimaryModel { get => SynthesisModel == CabinSynthesisModel.Primary; }

    public CabinDrawNumber? DrawNumber { get => CabinObject?.IsPartOfDraw; }
    public CabinDirection? Direction { get => CabinObject?.Direction; }
    public CabinFinishEnum? MetalFinish
    {
        get => CabinObject?.MetalFinish;
        set 
        {
            if (CabinObject != null && CabinObject.MetalFinish != value)
            {
                CabinObject.MetalFinish = value;
                OnPropertyChanged(nameof(MetalFinish));
            }
        }
    }

    
    private bool isThicknessesAsPrimary = true;
    /// <summary>
    /// Weather the structures glass Thicknesses should be the same as in Primary
    /// </summary>
    public bool IsThicknessesAsPrimary 
    {
        get => CanConnectWithPrimary && isThicknessesAsPrimary;
        set
        {
            if (isThicknessesAsPrimary != value)
            {
                isThicknessesAsPrimary = value;
                OnPropertyChanged(nameof(IsThicknessesAsPrimary));
            }
        }
    }
    public virtual CabinThicknessEnum? Thicknesses
    {
        get 
        {
            return CabinObject?.Thicknesses; 
        }
        set
        {
            if (CabinObject != null && value != CabinObject.Thicknesses)
            {
                CabinObject.Thicknesses = value;
                OnPropertyChanged(nameof(Thicknesses));
                OnPropertyChanged(nameof(Code));
            }
        }
    }

    private bool isGlassFinishAsPrimary = true;
    /// <summary>
    /// Weather the glass Finish should be the same as the Primary
    /// </summary>
    public bool IsGlassFinishAsPrimary 
    {
        get => CanConnectWithPrimary && isGlassFinishAsPrimary;
        set
        {
            if (isGlassFinishAsPrimary != value)
            {
                isGlassFinishAsPrimary = value;
                OnPropertyChanged(nameof(IsGlassFinishAsPrimary));
            }
        }
    }
    public GlassFinishEnum? GlassFinish
    {
        get { return cabinObject?.GlassFinish; }
        set
        {
            if (cabinObject != null && value != cabinObject.GlassFinish)
            {
                cabinObject.GlassFinish = value;
                OnPropertyChanged(nameof(GlassFinish));
                OnPropertyChanged(nameof(Code));
            }
        }
    }

    public int? InputNominalLength
    {
        //Return null for the TextBox to remain Empty
        get => (cabinObject?.NominalLength is null or 0) ? null : cabinObject.NominalLength;
        
        set
        {
            if (cabinObject != null && value != cabinObject.NominalLength)
            {
                if (value is not null)
                {
                    cabinObject.NominalLength = (int)value;
                }
                else
                {
                    cabinObject.NominalLength = 0;
                }
                OnPropertyChanged(nameof(InputNominalLength));
                OnPropertyChanged(nameof(InputMinLength));
                OnPropertyChanged(nameof(InputMaxLength));
                OnPropertyChanged(nameof(Code));
            }
        }
    }
    public int? InputMinLength
    {
        //Return null for the TextBox to remain Empty (Only when Nominal Length is 0)
        get => (cabinObject?.NominalLength is null or 0) ? null : cabinObject.LengthMin;
        set
        {
            if (cabinObject != null && value != cabinObject.LengthMin)
            {
                if (value is not null)
                {
                    cabinObject.LengthMin = (int)value;
                }
                else
                {
                    cabinObject.LengthMin = 0;
                }
                OnPropertyChanged(nameof(InputNominalLength));
                OnPropertyChanged(nameof(InputMinLength));
                OnPropertyChanged(nameof(InputMaxLength));
            }
        }
    }
    public int? InputMaxLength
    {
        //Return null for the TextBox to remain Empty (Only when Nominal Length is 0)
        get => (cabinObject?.NominalLength is null or 0) ? null : cabinObject.LengthMax;
        set
        {
            if (cabinObject != null && value != cabinObject.LengthMax)
            {
                if (value is not null)
                {
                    cabinObject.LengthMax = (int)value;
                }
                else
                {
                    cabinObject.LengthMax = 0;
                }
                OnPropertyChanged(nameof(InputNominalLength));
                OnPropertyChanged(nameof(InputMinLength));
                OnPropertyChanged(nameof(InputMaxLength));
            }
        }
    }

    /// <summary>
    /// Weather the Input height should be the Same with the Primary Model
    /// </summary>
    private bool isInputHeightAsPrimary = true;
    public bool IsInputHeightAsPrimary 
    {
        get => CanConnectWithPrimary && isInputHeightAsPrimary;
        set
        {
            if (isInputHeightAsPrimary != value)
            {
                isInputHeightAsPrimary = value;
                OnPropertyChanged(nameof(IsInputHeightAsPrimary));
            }
        }
    }
    public int? InputHeight
    {
        get
        {
            if (cabinObject != null && cabinObject.Height != 0)
            {
                return cabinObject.Height;
            }
            return null;
        }
        set
        {
            if (cabinObject != null && value != cabinObject.Height)
            {
                cabinObject.Height = value ?? 0;
                OnPropertyChanged(nameof(InputHeight));
                OnPropertyChanged(nameof(Code));
            }
        }
    }

    public bool HasStep { get => cabinObject?.HasStep ?? false; }

    public int? InputStepLength
    {
        get => HasStep ? cabinObject!.GetStepCut().StepLength : null;
        set
        {
            if (cabinObject != null)
            {
                if (value is not null and not 0)
                {
                    if (!cabinObject.HasStep) cabinObject.AddExtra(CabinExtraType.StepCut);
                    cabinObject.GetStepCut().StepLength = value ?? 0;
                }
                else
                {
                    if (!cabinObject.HasStep) return; //If does not have step simply return
                    //if it has and height of step is zero also , remove it
                    if (cabinObject.GetStepCut().StepHeight == 0) cabinObject.RemoveExtra(CabinExtraType.StepCut);
                    //else set length to zero without removing
                    else cabinObject.GetStepCut().StepLength = value ?? 0;
                }
                OnPropertyChanged(nameof(InputStepLength));
                OnPropertyChanged(nameof(HasStep));
            }
        }
    }
    public int? InputStepHeight
    {
        get => HasStep ? cabinObject!.GetStepCut().StepHeight : null;
        set
        {
            if (cabinObject != null)
            {
                if (value is not null and not 0)
                {
                    if (!cabinObject.HasStep) cabinObject.AddExtra(CabinExtraType.StepCut);
                    cabinObject.GetStepCut().StepHeight = value ?? 0;
                }
                else
                {
                    if (!cabinObject.HasStep) return; //If does not have step simply return
                    //if it has and length of step is zero also , remove it
                    if (cabinObject.GetStepCut().StepLength == 0) cabinObject.RemoveExtra(CabinExtraType.StepCut);
                    //else set length to zero without removing
                    else cabinObject.GetStepCut().StepHeight = value ?? 0;
                }
                OnPropertyChanged(nameof(InputStepHeight));
                OnPropertyChanged(nameof(HasStep));
            }
        }
    }

    /// <summary>
    /// Weather the Step is being edited
    /// </summary>
    [ObservableProperty]
    private bool isStepUnderEdit = false;

    private bool canConnectWithPrimary = true;
    /// <summary>
    /// Declares weather this CabinViewModel can Connect some of its Connectable Properties with the Primary Model
    /// </summary>
    public bool CanConnectWithPrimary 
    {
        get => !IsPrimaryModel && canConnectWithPrimary;
        set
        {
            if (canConnectWithPrimary != value)
            {
                canConnectWithPrimary = value;
                OnPropertyChanged(nameof(CanConnectWithPrimary));
                OnPropertyChanged(nameof(IsThicknessesAsPrimary));
                OnPropertyChanged(nameof(IsGlassFinishAsPrimary));
                OnPropertyChanged(nameof(IsInputHeightAsPrimary));
            }
        }
    }

    public CabinViewModel(
        ConstraintsViewModel constraintsVM , 
        PartsViewModel partsVM)
    {
        constraints = constraintsVM;
        constraints.PropertyChanged += Cabin_PropertyChanged;
        parts = partsVM;
        parts.PropertyChanged += Cabin_PropertyChanged;
        parts.PartChanged += Parts_PartChanged;
        this.PropertyChanged += Cabin_PropertyChanged;
    }

    private void Parts_PartChanged(object? sender, PartChangedEventsArgs e)
    {
        //Hook to this so that the Lengths are changed when tollerances are also changed
        if (e.Spot is PartSpot.CloseProfile or PartSpot.WallSide1 or PartSpot.WallSide2 or PartSpot.NonWallSide or PartSpot.WallHinge)
        {
            OnPropertyChanged(nameof(InputMinLength));
            OnPropertyChanged(nameof(InputMaxLength));
            OnPropertyChanged(nameof(InputNominalLength));
        }
    }

    /// <summary>
    /// Informs Cabin has Changed whenever a Constraint a Part or A CabinProp Changes Value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Cabin_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        //Do not Raise Cabin Changed when the Mode Has Changed , otherwise it will automatically Apply Calculations 
        //As soon as Changed
        if (e.PropertyName == nameof(IsInSwapGlassMode)) return;
        RaiseCabinChanged();
    }

#nullable disable
    public CabinViewModel()
    {

    }
#nullable enable
    public virtual void SetNewCabin(Cabin cabin)
    {
        cabinObject = cabin;
        constraints.SetNewConstraints(cabin.Constraints);
        parts.SetNewPartsList(cabin.Parts,cabin.Identifier());
    }

    public event EventHandler<Cabin>? CabinChanged;
    /// <summary>
    /// Informs Listeners Cabin Has Changed
    /// </summary>
    private void RaiseCabinChanged()
    {
        if (cabinObject is not null)
        {
            CabinChanged?.Invoke(this, cabinObject);
        }
    }

    [RelayCommand]
    private void OpenEditStep()
    {
        IsStepUnderEdit = true;
    }

    [RelayCommand]
    private void CloseEditStep()
    {
        IsStepUnderEdit = false;
    }

    [RelayCommand]
    private void RemoveStep()
    {
        InputStepLength = 0;
        InputStepHeight = 0;
    }

    public override string ToString()
    {
        return cabinObject?.ToString() ?? "Undefined";
    }


    //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
    private bool _disposed;
    public override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)//Managed Resources
        {
            constraints.PropertyChanged -= Cabin_PropertyChanged;
            parts.PropertyChanged -= Cabin_PropertyChanged;
            parts.PartChanged -= Parts_PartChanged;
            this.PropertyChanged -= Cabin_PropertyChanged;
        }

        //object has been disposed
        _disposed = true;

        //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
        //The subclasses only implement the virtual method and a field '_disposed'
        //Call the base Dispose(bool)
        base.Dispose(disposing);
    }


}

public partial class CabinViewModel<T> : CabinViewModel
    where T : Cabin
{
    protected T? cabinObject;
    public override T? CabinObject => cabinObject;

    public CabinViewModel(ConstraintsViewModel constraintsVM , PartsViewModel parts) : base(constraintsVM,parts)
    {
    }
    public CabinViewModel()
    {

    }
}


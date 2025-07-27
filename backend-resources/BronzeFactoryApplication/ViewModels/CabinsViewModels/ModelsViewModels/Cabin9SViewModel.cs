using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels;

public partial class Cabin9SViewModel : CabinViewModel<Cabin9S>
{
    private readonly Constraints9SViewModel constraints;
    public override Constraints9SViewModel Constraints => constraints;

    private readonly Parts9SViewModel parts;
    public override Parts9SViewModel Parts => parts;

    public Cabin9SViewModel(Constraints9SViewModel constraints9SVM , Parts9SViewModel partsVM) 
        : base(constraints9SVM, partsVM)
    {
        constraints = constraints9SVM;
        parts = partsVM;
    }
#nullable disable
    public Cabin9SViewModel()
    {

    }
#nullable enable
    public override void SetNewCabin(Cabin cabin)
    {
        base.SetNewCabin(cabin);
        cabinObject = cabin as Cabin9S ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(Cabin9S)}");
        constraints.SetNewConstraints(cabin.Constraints);
        parts.SetNewPartsList(cabin.Parts, cabin.Identifier());
        OnPropertyChanged(string.Empty); //Inform all Listeners
    }
}


using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ShowerEnclosuresModelsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels
{
    public partial class Cabin94ViewModel : CabinViewModel<Cabin94>
    {
        private readonly Constraints94ViewModel constraints;
        public override Constraints94ViewModel Constraints => constraints;

        private readonly Parts94ViewModel parts;
        public override Parts94ViewModel Parts => parts;

        private bool isCoverDistanceAutomatic;
        /// <summary>
        /// Weather the CoverDistance Constraint is automatically set for this Structure
        /// </summary>
        public bool IsCoverDistanceAutomatic 
        {
            get => isCoverDistanceAutomatic;
            set
            {
                if (isCoverDistanceAutomatic != value)
                {
                    isCoverDistanceAutomatic = value;
                    
                    if (isCoverDistanceAutomatic)
                    {
                        //If automatic handle CoverDistance Value based on Constraints
                        //by subscribing to changes in length
                        this.PropertyChanged += Cabin94ViewModel_PropertyChanged;
                    }
                    else
                    {
                        //When false unsubscribe
                        this.PropertyChanged -= Cabin94ViewModel_PropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Checkes weather Length has Changed to Apply the Correct Cover Distance when in Auto Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cabin94ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(InputNominalLength) or "")
            {
                if (this.Constraints.SameGlassesLengths.Any(l=> l == InputNominalLength))
                {
                    Constraints.CoverDistance = Constraints.CoverDistanceSameGlasses;
                }
                else
                {
                    Constraints.CoverDistance = Constraints.CoverDistanceMaxOpening;
                }
            }
        }

        public Cabin94ViewModel(Constraints94ViewModel constraints94VM, Parts94ViewModel partsVM) 
            : base(constraints94VM, partsVM)
        {
            constraints = constraints94VM;
            parts = partsVM;
            IsCoverDistanceAutomatic = true;
        }
#nullable disable
        public Cabin94ViewModel()
        {

        }
#nullable enable
        public override void SetNewCabin(Cabin cabin)
        {
            base.SetNewCabin(cabin);
            cabinObject = cabin as Cabin94 ?? throw new InvalidOperationException($"Provided Cabin was of type {cabin.GetType().Name} -- and not of the expected type : {nameof(Cabin94)}");
            constraints.SetNewConstraints(cabin.Constraints);
            parts.SetNewPartsList(cabin.Parts,cabin.Identifier());
            OnPropertyChanged(string.Empty); //Inform all Listeners
        }
    }
}

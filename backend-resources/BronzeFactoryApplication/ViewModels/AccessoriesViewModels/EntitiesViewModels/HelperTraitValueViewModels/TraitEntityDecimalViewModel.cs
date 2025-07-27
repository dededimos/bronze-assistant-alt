using AccessoriesRepoMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels.HelperTraitValueViewModels
{
    public partial class TraitEntityDecimalViewModel : ObservableObject
    {
        [ObservableProperty]
        private TraitEntity trait;
        [ObservableProperty]
        private decimal value;

        public TraitEntityDecimalViewModel(TraitEntity trait , decimal value)
        {
            Trait = trait;
            Value = value;
        }
    }
}

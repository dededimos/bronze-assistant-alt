using AccessoriesRepoMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels.HelperTraitValueViewModels
{
    public partial class TraitEntityDoubleViewModel : ObservableObject
    {
        [ObservableProperty]
        private TraitEntity trait;
        [ObservableProperty]
        private double value;

        public TraitEntityDoubleViewModel(TraitEntity trait , double value)
        {
            Trait = trait;
            Value = value;
        }
    }
}

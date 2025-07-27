using BronzeFactoryApplication.ViewModels.CabinsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Helpers.StartupHelpers
{
    public class CabinViewModelFactory
    {
        private readonly Dictionary<string, Func<CabinViewModel>> factories = new();

        public CabinViewModelFactory()
        {
            
        }

        /// <summary>
        /// Creates a ViewModel for the Passed Cabin Object
        /// The Viewmodel Properties all Align with those of the Cabin
        /// </summary>
        /// <param name="cabin">The Cabin for which to Create the ViewModel</param>
        /// <returns></returns>
        public CabinViewModel Create(Cabin cabin)
        {
            //Get the Correct Factory and Instantiate the Model
            //Correct Factory is Stored by Type Name to Dictionary
            CabinViewModel vm = factories[cabin.GetType().Name].Invoke();
            vm.SetNewCabin(cabin);
            return vm;
        }

        public void RegisterViewModelFactory<T>(Func<CabinViewModel> factory)
            where T : Cabin
        {
            factories.Add(typeof(T).Name, factory);
        }

    }
}

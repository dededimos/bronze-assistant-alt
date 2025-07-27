using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.ModalsDirectorService
{
    /// <summary>
    /// A Generator of EditModel Modals
    /// </summary>
    public interface IEditModelModalsGenerator
    {
        /// <summary>
        /// Opens the Specified Edit Modal to edit the Property with type <typeparamref name="TModel"/> of the Caller 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="editMessage">The Edit message containing information about the property being edited</param>
        /// <param name="modalTitle">The title of the modal</param>
        void OpenEditModal<TModel>(EditModelMessage<TModel> editMessage, string modalTitle = "") 
            where TModel : class, IDeepClonable<TModel>, IEqualityComparerCreator<TModel>;
    }

    public class EditModelModalsGenerator : IEditModelModalsGenerator
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly IEditModelModalsViewModelFactory modalsFactory;

        public EditModelModalsGenerator(ModalsContainerViewModel modalsContainer, IEditModelModalsViewModelFactory modalsFactory)
        {
            this.modalsContainer = modalsContainer;
            this.modalsFactory = modalsFactory;
        }

        public void OpenEditModal<TModel>(EditModelMessage<TModel> editMessage, string modalTitle = "")
            where TModel : class, IDeepClonable<TModel>, IEqualityComparerCreator<TModel>
        {
            var modal = modalsFactory.Create<TModel>();
            modal.InitilizeModal(editMessage, modalTitle);
            modalsContainer.OpenModal(modal);
        }
    }
    public interface IEditModelModalsViewModelFactory
    {
        IEditModelModalViewModel<TModel> Create<TModel>() where TModel : class,IDeepClonable<TModel>,IEqualityComparerCreator<TModel>;
    }

    public class EditModelModalsViewModelFactory : IEditModelModalsViewModelFactory
    {
        private readonly IServiceProvider serviceProvider;

        public EditModelModalsViewModelFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEditModelModalViewModel<TModel> Create<TModel>()
            where TModel :class , IDeepClonable<TModel> , IEqualityComparerCreator<TModel>
        {
            return serviceProvider.GetRequiredService<Func<IEditModelModalViewModel<TModel>>>().Invoke();
        }
    }

}

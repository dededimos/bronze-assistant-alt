using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenLiveEditPartModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<LiveEditPartModalViewModel> vmFactory;

        public OpenLiveEditPartModalService(ModalsContainerViewModel modalsContainer,
            Func<LiveEditPartModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }
        /// <summary>
        /// Opens a Modal to Edit a Part
        /// </summary>
        /// <param name="part">The Part to Edit</param>
        /// <param name="spotUnderEdit">The Spot under Edit</param>
        /// <param name="sender">The Sender of the Edit Request</param>
        public void OpenModal(CabinPart part,PartSpot spotUnderEdit,PartsViewModel sender)
        {
            //Create the Modal ViewModel
            var vm = vmFactory.Invoke();
            //Set the Part ViewModel 
            vm.SetPart(GetLiveEditPartVm(part),spotUnderEdit,sender);
            // Open the Modal
            modalsContainer.OpenModal(vm);
        }

        /// <summary>
        /// Returns the ViewModel matching the Part to be Edited
        /// </summary>
        /// <param name="part">The Part to Edit</param>
        /// <returns>The ViewModel matching the part argument</returns>
        private static LiveEditPartViewModel GetLiveEditPartVm(CabinPart part)
        {
            return part switch
            {
                CabinAngle angle =>         new LiveEditAngleViewModel(angle),
                CabinHandle handle =>       new LiveEditHandleViewModel(handle),
                GlassToGlassHinge hinge =>  new LiveEditGlassToGlassHingeViewModel(hinge),
                Hinge9B hinge =>            new LiveEditHinge9BHingeViewModel(hinge),
                HingeDB hinge =>            new LiveEditHingeDBViewModel(hinge),
                //Has to be Last so that subclasses are hit first
                CabinHinge hinge =>         new LiveEditHingeViewModel(hinge),
                ProfileHinge profile =>     new LiveEditProfileHingeViewModel(profile),
                //Has to be Last so that subclasses are hit first
                Profile profile =>          new LiveEditProfileViewModel(profile),
                GlassStrip strip =>         new LiveEditStripViewModel(strip),
                SupportBar supportBar =>    new LiveEditSupportBarViewModel(supportBar),
                FloorStopperW stopper =>    new LiveEditFloorStopperViewModel(stopper),
                //Has to be Last so that subclasses are hit first
                CabinSupport support =>     new LiveEditSupportViewModel(support),
                _ =>                        new LiveEditPartViewModel(part),
            };
        }
    }
}

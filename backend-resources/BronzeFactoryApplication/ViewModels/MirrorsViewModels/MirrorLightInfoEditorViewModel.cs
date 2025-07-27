using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorLightInfoEditorViewModel : BaseViewModel , IEditorViewModel<MirrorLightInfo>
    {
        private readonly IEditModelModalsGenerator editModalsGenerator;
        
        [ObservableProperty]
        private double wattPerMeter;
        [ObservableProperty]
        private int lumen;
        [ObservableProperty]
        private IPRating iP = new();

        public ObservableCollection<int> Kelvin { get; } = new();
        
        [ObservableProperty]
        private int kelvinToAdd;

        public MirrorLightInfoEditorViewModel(IEditModelModalsGenerator editModalsGenerator)
        {
            this.editModalsGenerator = editModalsGenerator;
        }

        [RelayCommand]
        private void AddKelvin()
        {
            if (Kelvin.Contains(KelvinToAdd))
            {
                MessageService.Warning($"The Value of : {KelvinToAdd}K your are trying to add , Already Exists in the List",$"Kelving Value: {KelvinToAdd}K  already Exists");
                return;
            }
            else if(KelvinToAdd is 0)
            {
                MessageService.Warning($"Cannot add Kelvin with Zero Value", "Kelvin to add was Zero");
                return;
            }
            else
            {
                Kelvin.Add(KelvinToAdd);
            }
        }

        [RelayCommand]
        private void RemoveKelvin(int kelvin)
        {
            Kelvin.Remove(kelvin);
        }

        [RelayCommand]
        private void EditIPRating()
        {
            EditModelMessage<IPRating> message = new(IP, this);
            editModalsGenerator.OpenEditModal(message);
        }

        public MirrorLightInfo CopyPropertiesToModel(MirrorLightInfo model)
        {
            model.Kelvin = new(this.Kelvin);
            model.WattPerMeter = WattPerMeter;
            model.Lumen = Lumen;
            model.IP = IP.GetDeepClone();
            return model;
        }

        public MirrorLightInfo GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorLightInfo model)
        {
            Kelvin.Clear();
            foreach (var temp in model.Kelvin)
            {
                Kelvin.Add(temp);
            }
            this.WattPerMeter = model.WattPerMeter;
            this.Lumen = model.Lumen;
            this.IP = model.IP.GetDeepClone();
        }
    }
}

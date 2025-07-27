using DataAccessLib.NoSQLModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels.SettingsViewModels
{
    public partial class EditCabinSettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// The Identifier of the Cabin consuming this Settings
        /// </summary>
        [ObservableProperty]
        private CabinIdentifier identifier;

        [ObservableProperty]
        private CabinModelEnum model;
        [ObservableProperty]
        private CabinFinishEnum metalFinish;
        [ObservableProperty]
        private CabinThicknessEnum thicknesses;
        [ObservableProperty]
        private GlassFinishEnum glassFinish;
        [ObservableProperty]
        private int height;
        [ObservableProperty]
        private int nominalLength;
        [ObservableProperty]
        private bool isReversible;
        [ObservableProperty]
        private string notes = string.Empty;

        public EditCabinSettingsViewModel(CabinSettings settings,CabinIdentifier identifier , string notes)
        {
            this.identifier = identifier;
            this.model = settings.Model;
            this.metalFinish = settings.MetalFinish;
            this.thicknesses = settings.Thicknesses;
            this.glassFinish = settings.GlassFinish;
            this.height = settings.Height;
            this.nominalLength= settings.NominalLength;
            this.isReversible = settings.IsReversible;
            this.notes = notes;
        }

        public EditCabinSettingsViewModel(CabinSettingsEntity entity) : this(entity.Settings,new(entity.Model,entity.DrawNumber,entity.SynthesisModel), entity.Notes) {}

        /// <summary>
        /// Returns the Settings Object Represented by this ViewModel
        /// </summary>
        /// <returns></returns>
        public CabinSettings GetSettingsObject()
        {
            var settings = new CabinSettings
            {
                Model =         this.Model,
                MetalFinish =   this.MetalFinish,
                Thicknesses =   this.Thicknesses,
                GlassFinish =   this.GlassFinish,
                Height =        this.Height,
                NominalLength = this.NominalLength,
                IsReversible =  this.IsReversible
            };
            return settings;
        }

        /// <summary>
        /// Returns the Settings Entity Object Represented by this ViewModel
        /// </summary>
        /// <returns></returns>
        public CabinSettingsEntity GetSettingsEntityObject()
        {
            var entity = new CabinSettingsEntity(GetSettingsObject(), Identifier.Model,Identifier.DrawNumber,Identifier.SynthesisModel,Notes);
            
            return entity;
        }

    }
}

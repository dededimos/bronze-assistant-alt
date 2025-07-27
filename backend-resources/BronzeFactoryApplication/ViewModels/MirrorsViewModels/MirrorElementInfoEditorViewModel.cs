using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorElementInfoEditorViewModel : BaseViewModel, IEditorViewModel<MirrorElementBase>
    {
        public MirrorElementInfoEditorViewModel(IEditModelModalsGenerator editModalsGenerator)
        {
            this.editModalsGenerator = editModalsGenerator;
        }
        private string id = ObjectId.Empty.ToString();
        private readonly IEditModelModalsGenerator editModalsGenerator;

        [ObservableProperty]
        private string code = string.Empty;
        [ObservableProperty]
        private string photoUrl = string.Empty;
        [ObservableProperty]
        private string photoUrl2 = string.Empty;
        [ObservableProperty]
        private string iconUrl = string.Empty;
        [ObservableProperty]
        private bool isOverriddenElement;
        [ObservableProperty]
        private LocalizedString name = LocalizedString.Undefined();
        [ObservableProperty]
        private LocalizedString description = LocalizedString.Undefined();
        [ObservableProperty]
        private LocalizedString extendedDescription = LocalizedString.Undefined();
        
        [RelayCommand]
        private void EditLocalizedString(LocalizedString localizedStringToEdit)
        {
            EditModelMessage<LocalizedString> message = new(localizedStringToEdit, this);
            editModalsGenerator.OpenEditModal(message);
        }

        /// <summary>
        /// Marks the Properties of element info as custom
        /// <para>Sets a new Element info Id (item does not exist)</para>
        /// <para>Sets IsDefaultElement to false</para>
        /// <para>Adds a (c) at the end of Code</para>
        /// <para>Adds "(custom)" to Name of all Localizations if they are not empty</para>
        /// </summary>
        public void MarkElementInfoAsCustom()
        {
            SuppressPropertyNotifications();
            //id = ObjectId.GenerateNewId().ToString(); Overriden elements keep the Id of the Default Element
            IsOverriddenElement = true;
            //add a c to the code ad descriptions to mark it as custom and change the IsDefault
            if(!string.IsNullOrWhiteSpace(Code)) Code += "(c)";

            //Change all the Object!
            LocalizedString newName = Name.GetDeepClone();
            newName.DefaultValue = Name.DefaultValue + "(Custom)";
            foreach (var val in newName.LocalizedValues)
            {
                if (!string.IsNullOrWhiteSpace(val.Value))
                {
                    newName.LocalizedValues[val.Key] = val.Value + "(Custom)";
                }
            }
            Name = newName;

            ResumePropertyNotifications();
            OnPropertyChanged("");
        }

        public MirrorElementBase CopyPropertiesToModel(MirrorElementBase model)
        {
            model.ElementId = this.id;
            model.Code = this.Code;
            model.PhotoUrl = this.PhotoUrl;
            model.PhotoUrl2 = this.PhotoUrl2;
            model.IconUrl = this.IconUrl;
            model.IsOverriddenElement = this.IsOverriddenElement;
            model.LocalizedDescriptionInfo = LocalizedDescription.Create(this.Name.GetDeepClone(), this.Description.GetDeepClone(), this.ExtendedDescription.GetDeepClone());
            return model;
        }
        public MirrorElementBase GetModel()
        {
            return CopyPropertiesToModel(new());
        }
        public void SetModel(MirrorElementBase model)
        {
            this.SuppressPropertyNotifications();
            this.id = model.ElementId;
            this.Code = model.Code;
            this.PhotoUrl = model.PhotoUrl;
            this.PhotoUrl2 = model.PhotoUrl2;
            this.IconUrl = model.IconUrl;
            this.IsOverriddenElement = model.IsOverriddenElement;
            this.Name = model.LocalizedDescriptionInfo.Name.GetDeepClone();
            this.Description = model.LocalizedDescriptionInfo.Description.GetDeepClone();
            this.ExtendedDescription = model.LocalizedDescriptionInfo.ExtendedDescription.GetDeepClone();
            this.ResumePropertyNotifications();
            OnPropertyChanged("");
        }
    }
}

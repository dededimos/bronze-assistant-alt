using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels
{
    public partial class LocalizedStringViewModel : BaseViewModel, IEditorViewModel<LocalizedString>
    { 
        public static string[] SupportedIdentifiers { get; } = new string[] { "el-GR", "en-EN", "it-IT" };
        private readonly string currentAppLanguage = ((App)Application.Current).SelectedLanguage;

        /// <summary>
        /// The Default Value of a Localized String
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MinifiedDefaultValue))]
        private string defaultValue = string.Empty;

        public string MinifiedDefaultValue { get => GetMinifiedDefaultValue(); }

        private string GetMinifiedDefaultValue()
        {
            var localizedValue = LocalizedValues.FirstOrDefault(v => v.Language == currentAppLanguage)?.StringValue ?? DefaultValue;
            var minified = localizedValue?.Replace(Environment.NewLine, " ") ?? "";
            // Return the First 50 Chars Followed by '...'
            if (string.IsNullOrWhiteSpace(minified)) return " -Undefined- ";
            return (minified.Length <= 33 ? minified : string.Concat(minified.AsSpan(0, 30), "..."));
        }

        /// <summary>
        /// The Localized Values along with their LanguageKeys
        /// </summary>
        public ObservableCollection<LanguageStringObservable> LocalizedValues { get; } = new();

        [ObservableProperty]
        private string? selectedLanguageToAdd;

        [ObservableProperty]
        private string? selectedLanguageValueToAdd;

        public LocalizedStringViewModel()
        {
            
        }

        public void SetModel(LocalizedString model)
        {
            DefaultValue = model.DefaultValue;
            LocalizedValues.Clear();
            foreach (var kvp in model.LocalizedValues)
            {
                this.LocalizedValues.Add(new(kvp.Key,kvp.Value));
            }
            OnPropertyChanged(nameof(MinifiedDefaultValue));
        }

        public LocalizedString CopyPropertiesToModel(LocalizedString model)
        {
            Dictionary<string, string> localizedValues = LocalizedValues.ToDictionary(lso => lso.Language, iso => iso.StringValue);
            model.DefaultValue = this.DefaultValue;
            model.LocalizedValues.Clear();
            foreach (var kvp in localizedValues)
            {
                model.LocalizedValues.Add(kvp.Key, kvp.Value);
            }
            return model;
        }
        public LocalizedString GetModel()
        {
            var model = new LocalizedString(this.DefaultValue);
            return CopyPropertiesToModel(model);
        }

        [RelayCommand]
        private void AddLanguage()
        {
            if (SelectedLanguageToAdd is null) 
            {
                MessageService.Warning("Selected Language Cannot be Empty...", "Warning");
                return;
            }
            if (string.IsNullOrEmpty(SelectedLanguageValueToAdd))
            {
                MessageService.Warning("Translation Value Cannot be Empty...", "Warning");
                return;
            }
            if (SupportedIdentifiers.Any(id=> id == SelectedLanguageToAdd))
            {
                AddLocalization(SelectedLanguageToAdd, SelectedLanguageValueToAdd);
                OnPropertyChanged(nameof(MinifiedDefaultValue));
            }
            else
            {
                MessageService.Warning($"The Language you are Trying to Add is not Supported{Environment.NewLine}The Currently Supported Languages are :{Environment.NewLine}{string.Join(" - ",SupportedIdentifiers)}", "Warning");
            }
        }
        [RelayCommand]
        private void RemoveLanguage(string languageIdentifier)
        {
            RemoveLocalization(languageIdentifier);
            OnPropertyChanged(nameof(MinifiedDefaultValue));
        }

        /// <summary>
        /// Adds a Value to the Localization Values , if the Language already exists it replaces the older Value
        /// </summary>
        /// <param name="langIdentifier">The Language Identifier</param>
        /// <param name="value">The Value</param>
        public void AddLocalization(string langIdentifier, string value)
        {
            // If there is already such a Language , simply change its Translation Value
            var langToChange = LocalizedValues.FirstOrDefault(lso => lso.Language == langIdentifier);
            if (langToChange is not null)
            {
                langToChange.StringValue = value;
            }
            else
            {
                LocalizedValues.Add(new(langIdentifier, value));
            }
            OnPropertyChanged(nameof(LocalizedValues));
        }
        /// <summary>
        /// Removes the localized value along with its language key , if there is not such key , this method does nothing
        /// </summary>
        /// <param name="langIdentifier">The language identifier</param>
        public void RemoveLocalization(string langIdentifier)
        {
            var langToRemove = LocalizedValues.FirstOrDefault(lso => lso.Language == langIdentifier);
            if (langToRemove is not null)
            {
                LocalizedValues.Remove(langToRemove);
            }
            OnPropertyChanged(nameof(LocalizedValues));
        }
    }

    public partial class LanguageStringObservable : ObservableObject
    {
        /// <summary>
        /// The Language Key
        /// </summary>
        [ObservableProperty]
        private string language = string.Empty;
        /// <summary>
        /// The Localized Value for the certain Language Key
        /// </summary>
        [ObservableProperty]
        private string stringValue = string.Empty;

        public LanguageStringObservable()
        {

        }
        public LanguageStringObservable(string language, string stringValue)
        {
            Language = language;
            StringValue = stringValue;
        }

        public KeyValuePair<string, string> ToKeyValuePair => new(Language, StringValue);
    }


}

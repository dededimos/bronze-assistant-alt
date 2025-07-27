using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using Microsoft.Graph.Education.Classes.Item.Assignments.Item.Submissions.Item.Return;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels
{
    public partial class CustomPriceRuleEntityViewModel : BaseViewModel, IEditorViewModel<CustomPriceRuleEntity>
    {
        private readonly IAccessoryEntitiesRepository repo;

        [ObservableProperty]
        private DescriptiveEntityViewModel baseDescriptiveEntity;
        [ObservableProperty]
        private bool isEnabled;
        [ObservableProperty]
        private int sortNo;
        [ObservableProperty]
        private CustomRuleValueType ruleValueType;

        [ObservableProperty]
        private decimal ruleValue;
        [ObservableProperty]
        private ObservableCollection<PriceRuleConditionDescriptive> conditions = new();

        /// <summary>
        /// Where does the Condition Apply to
        /// </summary>
        private AppliesTo selectedConditionApplication;
        public AppliesTo SelectedConditionApplication
        {
            get => selectedConditionApplication;
            set
            {
                if (value != selectedConditionApplication)
                {
                    selectedConditionApplication = value;
                    //nullify the selected of the Target Condition as List Selection also Changes
                    SelectedTargetConditionId = string.Empty;
                    OnPropertyChanged(nameof(SelectedConditionApplication));
                    OnPropertyChanged(nameof(TargetConditionListAccessories));
                    OnPropertyChanged(nameof(TargetConditionListTraits));
                    OnPropertyChanged(nameof(TargetConditionListTraitGroups));
                    OnPropertyChanged(nameof(IsAccessoriesListEnabled));
                    OnPropertyChanged(nameof(IsTraitsListEnabled));
                    OnPropertyChanged(nameof(IsTraitsGroupsListEnabled));
                    OnPropertyChanged(nameof(ConditionWhen));
                    OnPropertyChanged(nameof(ConditionWhenTargetValue));
                    OnPropertyChanged(nameof(IsConditionWhenNeeded));
                }
            }
        }
        /// <summary>
        /// The Id of the Item for which the Condition Application Applies To
        /// </summary>
        [ObservableProperty]
        private string selectedTargetConditionId = string.Empty;

        /// <summary>
        /// The List of Bathroom Accessories to Select From when the SelectedConfitionApplication is Accessory
        /// </summary>
        public IEnumerable<BathAccessoryEntity> TargetConditionListAccessories { get => SelectedConditionApplication is AppliesTo.AccessorySpecific or AppliesTo.ExceptAccessorySpecific ? Accessories : Enumerable.Empty<BathAccessoryEntity>(); }
        /// <summary>
        /// Weather the List of Accessories should be enabled in the UI
        /// </summary>
        public bool IsAccessoriesListEnabled { get => TargetConditionListAccessories.Any(); }

        private string selectedAccessoryId = string.Empty;
        public string SelectedAccessoryId
        {
            get => selectedAccessoryId;
            set
            {
                if (value != selectedAccessoryId)
                {
                    selectedAccessoryId = value;
                    if(IsAccessoriesListEnabled) SelectedTargetConditionId = selectedAccessoryId;
                    OnPropertyChanged(nameof(SelectedAccessoryId));
                }
            }
        }

        /// <summary>
        /// The List of Traits to Select From when the SelectedConfitionApplication concerns a Trait Entity
        /// </summary>
        public IEnumerable<TraitEntity> TargetConditionListTraits
        {
            get
            {
                return SelectedConditionApplication switch
                {
                    AppliesTo.FinishSpecific or AppliesTo.ExceptFinishSpecific => Finishes,
                    AppliesTo.SeriesMainSpecific or AppliesTo.OtherSeriesSpecific or AppliesTo.ExceptSeriesMainSpecific or AppliesTo.ExceptOtherSeriesSpecific => Series,
                    AppliesTo.SizeSpecific or AppliesTo.ExceptSizeSpecific => Sizes,
                    AppliesTo.ShapeSpecific or AppliesTo.ExceptShapeSpecific => Shapes,
                    AppliesTo.MaterialSpecific or AppliesTo.ExceptMaterialSpecific => Materials,
                    AppliesTo.PrimaryTypeMainSpecific or AppliesTo.OtherPrimaryTypeSpecific or AppliesTo.ExceptPrimaryTypeMainSpecific or AppliesTo.ExceptOtherPrimaryTypeSpecific => PrimaryTypes,
                    AppliesTo.SecondaryTypeMainSpecific or AppliesTo.OtherSecondaryTypeSpecific or AppliesTo.ExceptSecondaryTypeMainSpecific or AppliesTo.ExceptOtherSecondaryTypeSpecific => SecondaryTypes,
                    AppliesTo.CategorySpecific or AppliesTo.ExceptCategorySpecific => Categories,
                    AppliesTo.MountingSpecific or AppliesTo.ExceptMountingSpecific => MountingTypes,
                    AppliesTo.PriceTraitSpecific or AppliesTo.ExceptPriceTraitSpecific => PriceTraits,
                    _ => Enumerable.Empty<TraitEntity>(),
                };
            }
        }
        /// <summary>
        /// Weather the List of Traits should be enabled in the UI
        /// </summary>
        public bool IsTraitsListEnabled { get => TargetConditionListTraits.Any(); }
        
        private string selectedTraitId = string.Empty;
        public string SelectedTraitId
        {
            get => selectedTraitId;
            set
            {
                if (value != selectedTraitId)
                {
                    selectedTraitId = value;
                    if (IsTraitsListEnabled) SelectedTargetConditionId = selectedTraitId;
                    OnPropertyChanged(nameof(SelectedTraitId));
                }
            }
        }

        /// <summary>
        /// The List of Traits to Select From when the SelectedConfitionApplication concerns a TraitGroup Entity
        /// </summary>
        public IEnumerable<TraitGroupEntity> TargetConditionListTraitGroups
        {
            get
            {
                return SelectedConditionApplication switch
                {
                    AppliesTo.FinishTraitGroupSpecific or AppliesTo.ExceptFinishTraitGroupSpecific => FinishTraitGroups,
                    AppliesTo.SeriesMainTraitGroupSpecific or AppliesTo.OtherSeriesTraitGroupSpecific or AppliesTo.ExceptSeriesMainTraitGroupSpecific or AppliesTo.ExceptOtherSeriesTraitGroupSpecific => SeriesTraitGroups,
                    AppliesTo.PriceTraitGroupSpecific or AppliesTo.ExceptPriceTraitGroupSpecific => PriceTraitGroups,
                    _ => Enumerable.Empty<TraitGroupEntity>(),
                };
            }
        }
        /// <summary>
        /// Weather the List of TraitGroups should be enabled in the UI
        /// </summary>
        public bool IsTraitsGroupsListEnabled { get => TargetConditionListTraitGroups.Any(); }

        private string selectedTraitGroupId = string.Empty;
        public string SelectedTraitGroupId
        {
            get => selectedTraitGroupId;
            set
            {
                if (value != selectedTraitGroupId)
                {
                    selectedTraitGroupId = value;
                    if (IsTraitsGroupsListEnabled) SelectedTargetConditionId = selectedTraitGroupId;
                    OnPropertyChanged(nameof(SelectedTraitGroupId));
                }
            }
        }

        private AppliesWhen conditionWhen;
        public AppliesWhen ConditionWhen 
        {
            get => SelectedConditionApplication is AppliesTo.ItemQuantity or AppliesTo.ExceptItemQuantity ? conditionWhen : AppliesWhen.Never;
            set
            {
                if (value != conditionWhen)
                {
                    conditionWhen = value;
                    OnPropertyChanged(nameof(ConditionWhen));
                }
            }
        }

        private decimal conditionWhenTargetValue;
        public decimal ConditionWhenTargetValue 
        {
            get => SelectedConditionApplication is AppliesTo.ItemQuantity or AppliesTo.ExceptItemQuantity ? conditionWhenTargetValue : 0;
            set
            {
                if (value != conditionWhenTargetValue)
                {
                    conditionWhenTargetValue = value;
                    OnPropertyChanged(nameof(ConditionWhenTargetValue));
                }
            }
        }

        public bool IsConditionWhenNeeded { get => SelectedConditionApplication is AppliesTo.ItemQuantity or AppliesTo.ExceptItemQuantity; }


        public IEnumerable<BathAccessoryEntity> Accessories { get => repo.Cache; }
        public IEnumerable<TraitEntity> Finishes { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.FinishTrait).OrderBy(t=> t.SortNo); }
        public IEnumerable<TraitEntity> Series { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SeriesTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> Sizes { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SizeTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> Shapes { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.ShapeTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> Materials { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.MaterialTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> PrimaryTypes { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.PrimaryTypeTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> SecondaryTypes { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SecondaryTypeTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> Categories { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.CategoryTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> MountingTypes { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.MountingTypeTrait).OrderBy(t => t.SortNo); }
        public IEnumerable<TraitEntity> PriceTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.PriceTrait).OrderBy(t=> t.SortNo); }
        public IEnumerable<TraitGroupEntity> FinishTraitGroups { get => repo.Traits.TraitGroups.Cache.Where(tg => tg.PermittedTraitTypes.Any(pt => pt == TypeOfTrait.FinishTrait)); }
        public IEnumerable<TraitGroupEntity> SeriesTraitGroups { get => repo.Traits.TraitGroups.Cache.Where(tg => tg.PermittedTraitTypes.Any(pt => pt == TypeOfTrait.SeriesTrait)); }
        public IEnumerable<TraitGroupEntity> PriceTraitGroups { get => repo.Traits.TraitGroups.Cache.Where(tg => tg.PermittedTraitTypes.Any(pt => pt == TypeOfTrait.PriceTrait)); }

        public CustomPriceRuleEntityViewModel(Func<DescriptiveEntityViewModel> descriptiveEntityVmFactory, IAccessoryEntitiesRepository repo)
        {
            baseDescriptiveEntity = descriptiveEntityVmFactory.Invoke();
            this.repo = repo;
        }

        [RelayCommand]
        private void AddCondition()
        {
            if (string.IsNullOrEmpty(SelectedTargetConditionId) && SelectedConditionApplication is not AppliesTo.AllAccessories and not AppliesTo.Nothing and not AppliesTo.ItemQuantity and not AppliesTo.ExceptItemQuantity)
            {
                MessageService.Warning("Please Select a Target Condition", "Target Condition not Selected");
                return;
            }

            PriceRuleCondition condition = new(SelectedConditionApplication, SelectedTargetConditionId,ConditionWhen,ConditionWhenTargetValue);
            var lngId = ((App)(Application.Current)).SelectedLanguage;
            var descCondition = PriceRuleConditionDescriptive.Create(condition, Accessories, repo.Traits.Cache, repo.Traits.TraitGroups.Cache, lngId);
            Conditions.Add(descCondition);
            //For the Edit Context
            OnPropertyChanged(nameof(Conditions));
        }
        [RelayCommand]
        private void RemoveCondition(PriceRuleConditionDescriptive condition)
        {
            if (MessageService.Question($"Would you like to Remove this Condition ?", "Condition Removal", "Ok", "Cancel") == MessageBoxResult.Cancel)
            {
                return;
            }

            if (Conditions.Remove(condition))
            {
                //For the Edit Context
                OnPropertyChanged(nameof(Conditions));
            }
            else
            {
                MessageService.Error("Condition Was not found in the Current Conditions of the CustomPrice Rule for an unexpected reason", "Condition not Found...");
            }
        }

        public CustomPriceRuleEntity CopyPropertiesToModel(CustomPriceRuleEntity model)
        {
            BaseDescriptiveEntity.CopyPropertiesToModel(model);
            model.IsEnabled = this.IsEnabled;
            model.SortNo = this.SortNo;
            model.RuleValueType = this.RuleValueType;
            model.RuleValue = this.RuleValue;
            model.Conditions = new(this.Conditions.Select(c => c.ToPriceRuleCondition()));
            return model;
        }

        public CustomPriceRuleEntity GetModel()
        {
            CustomPriceRuleEntity model = new();
            return CopyPropertiesToModel(model);
        }

        public void SetModel(CustomPriceRuleEntity model)
        {
            BaseDescriptiveEntity.SetModel(model);
            IsEnabled = model.IsEnabled;
            SortNo = model.SortNo;
            RuleValueType = model.RuleValueType;
            RuleValue = model.RuleValue;
            Conditions.Clear();

            var lngId = ((App)(Application.Current)).SelectedLanguage;
            foreach (var condition in model.Conditions)
            {
                Conditions.Add(PriceRuleConditionDescriptive.Create(condition,Accessories,repo.Traits.Cache,repo.Traits.TraitGroups.Cache, lngId));
            }
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }

    public class PriceRuleConditionDescriptive
    {
        public AppliesTo Condition { get; set; }
        public string ConditionTargetValue { get; set; } = string.Empty;
        public string ConditionTargetDescription { get; set; } = string.Empty;
        public AppliesWhen ConditionWhen { get; set; }
        public decimal ConditionWhenTargetValue { get; set; }

        private PriceRuleConditionDescriptive()
        {
            
        }

        public PriceRuleCondition ToPriceRuleCondition()
        {
            PriceRuleCondition condition = new(this.Condition, this.ConditionTargetValue,this.ConditionWhen,this.ConditionWhenTargetValue);
            return condition;
        }

        /// <summary>
        /// Generates a Descriptive Rule Condition to Include a Description for the Passed Id
        /// There is no other way to read from the Id
        /// </summary>
        /// <param name="condition">The Original Condition</param>
        /// <param name="accessories">All accessories to be able to read their Ids and provide description</param>
        /// <param name="traits">All Traits to be able to read their Ids and provide description</param>
        /// <param name="traitGroups">All Trait Groups to be able to read their Ids and Provide Description</param>
        /// <param name="lngIdentifier">the lng Identifier to provide the description</param>
        /// <returns></returns>
        public static PriceRuleConditionDescriptive Create(PriceRuleCondition condition , 
            IEnumerable<BathAccessoryEntity> accessories, 
            IEnumerable<TraitEntity> traits, 
            IEnumerable<TraitGroupEntity> traitGroups,
            string lngIdentifier)
        {
            PriceRuleConditionDescriptive desc = new();
            desc.Condition = condition.Condition;
            desc.ConditionTargetValue = condition.ConditionTargetValue;

            if (condition.IsWhenConditionNeeded())
            {
                desc.ConditionWhen = condition.ConditionWhen;
                desc.ConditionWhenTargetValue = condition.ConditionWhenTargetValue;
            }
            switch (condition.Condition)
            {
                case AppliesTo.Nothing:
                case AppliesTo.AllAccessories:
                    desc.ConditionTargetDescription = "-";
                    break;
                case AppliesTo.AccessorySpecific:
                case AppliesTo.ExceptAccessorySpecific:
                    var acc = accessories.FirstOrDefault(a => a.IdAsString == condition.ConditionTargetValue);
                    if (acc is not null)
                    {
                        desc.ConditionTargetDescription = acc.MainCode;
                    }
                    else
                    {
                        desc.ConditionTargetDescription = $"{condition.ConditionTargetValue}:AccessoryNotFound";
                    }
                    break;
                case AppliesTo.FinishSpecific:
                case AppliesTo.ExceptFinishSpecific:
                case AppliesTo.SeriesMainSpecific:
                case AppliesTo.ExceptSeriesMainSpecific:
                case AppliesTo.OtherSeriesSpecific:
                case AppliesTo.ExceptOtherSeriesSpecific:
                case AppliesTo.SizeSpecific:
                case AppliesTo.ExceptSizeSpecific:
                case AppliesTo.ShapeSpecific:
                case AppliesTo.ExceptShapeSpecific:
                case AppliesTo.MaterialSpecific:
                case AppliesTo.ExceptMaterialSpecific:
                case AppliesTo.PrimaryTypeMainSpecific:
                case AppliesTo.ExceptPrimaryTypeMainSpecific:
                case AppliesTo.OtherPrimaryTypeSpecific:
                case AppliesTo.ExceptOtherPrimaryTypeSpecific:
                case AppliesTo.SecondaryTypeMainSpecific:
                case AppliesTo.ExceptSecondaryTypeMainSpecific:
                case AppliesTo.OtherSecondaryTypeSpecific:
                case AppliesTo.ExceptOtherSecondaryTypeSpecific:
                case AppliesTo.CategorySpecific:
                case AppliesTo.ExceptCategorySpecific:
                case AppliesTo.MountingSpecific:
                case AppliesTo.ExceptMountingSpecific:
                case AppliesTo.PriceTraitSpecific:
                case AppliesTo.ExceptPriceTraitSpecific:
                    var trait = traits.FirstOrDefault(t => t.IdAsString == condition.ConditionTargetValue);
                    if (trait is not null)
                    {
                        desc.ConditionTargetDescription = trait.Trait.GetLocalizedValue(lngIdentifier);
                    }
                    else
                    {
                        desc.ConditionTargetDescription = $"{condition.ConditionTargetValue}:TraitNotFound";
                    }
                    break;
                case AppliesTo.FinishTraitGroupSpecific:
                case AppliesTo.ExceptFinishTraitGroupSpecific:
                case AppliesTo.SeriesMainTraitGroupSpecific:
                case AppliesTo.ExceptSeriesMainTraitGroupSpecific:
                case AppliesTo.OtherSeriesTraitGroupSpecific:
                case AppliesTo.ExceptOtherSeriesTraitGroupSpecific:
                case AppliesTo.PriceTraitGroupSpecific:
                case AppliesTo.ExceptPriceTraitGroupSpecific:
                    var traitGroup = traitGroups.FirstOrDefault(tg => tg.IdAsString == condition.ConditionTargetValue);
                    if (traitGroup is not null)
                    {
                        desc.ConditionTargetDescription = traitGroup.Name.GetLocalizedValue(lngIdentifier);
                    }
                    else
                    {
                        desc.ConditionTargetDescription = $"{condition.ConditionTargetValue}:TraitGroupNotFound";
                    }
                    break;
                case AppliesTo.ItemQuantity:
                case AppliesTo.ExceptItemQuantity:
                    desc.ConditionTargetDescription = $"{condition.ConditionWhen.ToString().TryTranslateKeyWithoutError()} - {condition.ConditionWhenTargetValue}{"lngPcs".TryTranslateKeyWithoutError()}";
                    break;
                default:
                    desc.ConditionTargetDescription = $"{condition.ConditionTargetValue}:Invalid-AppliesTo";
                    break;
            }
            

            return desc;
        }
    }
}

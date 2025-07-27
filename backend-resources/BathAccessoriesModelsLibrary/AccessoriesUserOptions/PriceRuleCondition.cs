using CommonInterfacesBronze;
using System.Diagnostics.CodeAnalysis;

namespace BathAccessoriesModelsLibrary.AccessoriesUserOptions
{
    /// <summary>
    /// Defines a Condition for when an Ireggular Price Applies 
    /// </summary>
    public class PriceRuleCondition : IDeepClonable<PriceRuleCondition>
    {
        /// <summary>
        /// The Condition where An Iregular Price must Apply
        /// </summary>
        public AppliesTo Condition { get; set; }
        /// <summary>
        /// The Id of the Item That is Defined in the Condition (ex. Trait Id , Accessory Id e.t.c.)
        /// </summary>
        public string ConditionTargetValue { get; set; } = string.Empty;

        /// <summary>
        /// The Condition of when it applies based on a Value
        /// </summary>
        public AppliesWhen ConditionWhen { get; set; }
        /// <summary>
        /// The Value of the When Condition
        /// </summary>
        public decimal ConditionWhenTargetValue { get; set; }

        public PriceRuleCondition(AppliesTo condition, string conditionTargetValue, AppliesWhen conditionWhen, decimal conditionWhenTargetValue)
        {
            Condition = condition;
            ConditionTargetValue = conditionTargetValue;
            ConditionWhen = conditionWhen;
            ConditionWhenTargetValue = conditionWhenTargetValue;
        }

        public bool IsApplicable(BathroomAccessory accessory, string? selectedFinishId = null,decimal quantity = 0)
        {
            switch (Condition)
            {
                case AppliesTo.Nothing:
                    return false;
                case AppliesTo.AllAccessories:
                    return true;
                case AppliesTo.AccessorySpecific:
                    return accessory.Id == ConditionTargetValue;
                case AppliesTo.ExceptAccessorySpecific:
                    return accessory.Id != ConditionTargetValue;
                case AppliesTo.FinishSpecific:
                    return selectedFinishId != null &&
                           selectedFinishId == ConditionTargetValue;
                case AppliesTo.ExceptFinishSpecific:
                    return selectedFinishId != null &&
                           selectedFinishId != ConditionTargetValue;
                case AppliesTo.FinishTraitGroupSpecific:
                    var finish = accessory.GetAvailableFinishByIdOrNull(selectedFinishId ?? string.Empty);
                    return finish?.Finish.Groups.Any(g => g.Id == ConditionTargetValue) ?? false;
                case AppliesTo.ExceptFinishTraitGroupSpecific:
                    var finish2 = accessory.GetAvailableFinishByIdOrNull(selectedFinishId ?? string.Empty);
                    return finish2?.Finish.Groups.Any(g => g.Id == ConditionTargetValue) == false;
                case AppliesTo.SeriesMainSpecific:
                    return accessory.Series.Id == ConditionTargetValue;
                case AppliesTo.ExceptSeriesMainSpecific:
                    return accessory.Series.Id != ConditionTargetValue;
                case AppliesTo.OtherSeriesSpecific:
                    return accessory.Series.Id == ConditionTargetValue || accessory.OtherSeries.Any(s => s.Id == ConditionTargetValue);
                case AppliesTo.ExceptOtherSeriesSpecific:
                    return accessory.Series.Id != ConditionTargetValue && (accessory.OtherSeries.Any(s => s.Id == ConditionTargetValue) == false);
                case AppliesTo.SeriesMainTraitGroupSpecific:
                    return accessory.Series.Groups.Any(g => g.Id == ConditionTargetValue);
                case AppliesTo.ExceptSeriesMainTraitGroupSpecific:
                    return (accessory.Series.Groups.Any(g => g.Id == ConditionTargetValue) == false);
                case AppliesTo.OtherSeriesTraitGroupSpecific:
                    return accessory.Series.Groups.Any(g => g.Id == ConditionTargetValue) || accessory.OtherSeries.Any(s => s.Groups.Any(g => g.Id == ConditionTargetValue));
                case AppliesTo.ExceptOtherSeriesTraitGroupSpecific:
                    return (accessory.Series.Groups.Any(g => g.Id == ConditionTargetValue) == false) && (accessory.OtherSeries.Any(s => s.Groups.Any(g => g.Id == ConditionTargetValue)) == false);
                case AppliesTo.SizeSpecific:
                    return accessory.Size.Id == ConditionTargetValue;
                case AppliesTo.ExceptSizeSpecific:
                    return accessory.Size.Id != ConditionTargetValue;
                case AppliesTo.ShapeSpecific:
                    return accessory.Shape.Id == ConditionTargetValue;
                case AppliesTo.ExceptShapeSpecific:
                    return accessory.Shape.Id != ConditionTargetValue;
                case AppliesTo.MaterialSpecific:
                    return accessory.Material.Id == ConditionTargetValue;
                case AppliesTo.ExceptMaterialSpecific:
                    return accessory.Material.Id != ConditionTargetValue;
                case AppliesTo.PrimaryTypeMainSpecific:
                    return accessory.PrimaryType.Id == ConditionTargetValue;
                case AppliesTo.ExceptPrimaryTypeMainSpecific:
                    return accessory.PrimaryType.Id != ConditionTargetValue;
                case AppliesTo.OtherPrimaryTypeSpecific:
                    return accessory.PrimaryType.Id == ConditionTargetValue || accessory.OtherPrimaryTypes.Any(pt => pt.Id == ConditionTargetValue);
                case AppliesTo.ExceptOtherPrimaryTypeSpecific:
                    return (accessory.PrimaryType.Id != ConditionTargetValue && accessory.OtherPrimaryTypes.Any(pt => pt.Id == ConditionTargetValue) == false);
                case AppliesTo.SecondaryTypeMainSpecific:
                    return accessory.SecondaryType.Id == ConditionTargetValue;
                case AppliesTo.ExceptSecondaryTypeMainSpecific:
                    return accessory.SecondaryType.Id != ConditionTargetValue;
                case AppliesTo.OtherSecondaryTypeSpecific:
                    return accessory.SecondaryType.Id == ConditionTargetValue || accessory.OtherSecondaryTypes.Any(st => st.Id == ConditionTargetValue);
                case AppliesTo.ExceptOtherSecondaryTypeSpecific:
                    return (accessory.SecondaryType.Id != ConditionTargetValue && accessory.OtherSecondaryTypes.Any(st => st.Id == ConditionTargetValue) == false);
                case AppliesTo.CategorySpecific:
                    return accessory.Categories.Any(c => c.Id == ConditionTargetValue);
                case AppliesTo.ExceptCategorySpecific:
                    return (accessory.Categories.Any(c => c.Id == ConditionTargetValue) == false);
                case AppliesTo.MountingSpecific:
                    return accessory.MountingTypes.Any(mt => mt.Id == ConditionTargetValue);
                case AppliesTo.ExceptMountingSpecific:
                    return (accessory.MountingTypes.Any(mt => mt.Id == ConditionTargetValue) == false);
                case AppliesTo.PriceTraitSpecific:
                    return accessory.PricesInfo.Any(pi => pi.PriceTrait.Id == ConditionTargetValue);
                case AppliesTo.ExceptPriceTraitSpecific:
                    return (accessory.PricesInfo.Any(pi => pi.PriceTrait.Id == ConditionTargetValue) == false);
                case AppliesTo.PriceTraitGroupSpecific:
                    return accessory.PricesInfo.Any(pi => pi.PriceTrait.Groups.Any(g => g.Id == ConditionTargetValue));
                case AppliesTo.ExceptPriceTraitGroupSpecific:
                    return (accessory.PricesInfo.Any(pi => pi.PriceTrait.Groups.Any(g => g.Id == ConditionTargetValue)) == false);
                case AppliesTo.ItemQuantity:
                    return IsWhenConditionSatisfied(quantity);
                case AppliesTo.ExceptItemQuantity:
                    return IsWhenConditionSatisfied(quantity) == false;
                default:
                    return false;
            }
        }

        private bool IsWhenConditionSatisfied(decimal value)
        {
            return ConditionWhen switch
            {
                AppliesWhen.Always => true,
                AppliesWhen.Never => false,
                AppliesWhen.Equal => value == ConditionWhenTargetValue,
                AppliesWhen.NotEqual => value != ConditionWhenTargetValue,
                AppliesWhen.LessThan => value < ConditionWhenTargetValue,
                AppliesWhen.LessThanOrEqual => value <= ConditionWhenTargetValue,
                AppliesWhen.GreaterThan => value > ConditionWhenTargetValue,
                AppliesWhen.GreaterThanOrEqual => value >= ConditionWhenTargetValue,
                _ => false,
            };
        }

        /// <summary>
        /// Weather this Rule needs also a When Condition to Function
        /// </summary>
        /// <returns></returns>
        public bool IsWhenConditionNeeded()
        {
            //Applies only to Quantity Currently , more to be added .
            return GetConditionsWithWhen().Any(c=> c == Condition);
        }

        /// <summary>
        /// Get the Conditions that When Conditions are also needed
        /// </summary>
        /// <returns></returns>
        public static AppliesTo[] GetConditionsWithWhen() => new AppliesTo[] { AppliesTo.ItemQuantity,AppliesTo.ExceptItemQuantity };
        /// <summary>
        /// Get the Conditions that Do not Need ConditionTargetValue
        /// </summary>
        /// <returns></returns>
        public static AppliesTo[] ConditionsNotNeedingTargetValue() => new AppliesTo[] { AppliesTo.ItemQuantity, AppliesTo.ExceptItemQuantity, AppliesTo.Nothing, AppliesTo.AllAccessories };

        public PriceRuleCondition GetDeepClone()
        {
            return (PriceRuleCondition)this.MemberwiseClone();
        }
    }

    public class PriceRuleConditionEqualityComparer : IEqualityComparer<PriceRuleCondition>
    {
        public bool Equals(PriceRuleCondition? x, PriceRuleCondition? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return 
            x!.Condition == y!.Condition &&
            x.ConditionTargetValue == y.ConditionTargetValue;
        }

        public int GetHashCode([DisallowNull] PriceRuleCondition obj)
        {
            throw new NotSupportedException($"{typeof(PriceRuleConditionEqualityComparer).Name} does not Support a Get Hash Code Implementation");
        }
    }
}


using AKSoftware.Localization.MultiLanguages;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using System;
using System.Linq;
using XMLClosedReporting;
using static CommonHelpers.CommonExtensions;

namespace BronzeArtWebApplication.Shared.ReportModels
{
    public class AccessoryPriceableReportTemplate : AbstractReportTemplate<AccessoryPriceable>
    {
        /// <summary>
        /// The Padding added to the Header Titles so that the Excel Table-Filter does not clip the headers
        /// </summary>
        private const int padding = 3;

        public AccessoryPriceableReportTemplate(ILanguageContainerService lc,AccessoriesUrlHelper urlHelper,IAccessoriesMemoryRepository repo)
        {
            builder.CreateColumn()
                .SetColumnName(lc.Keys["Code"].AddPadding(padding))
                .AssignColumnValue(a => a.Code)
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(lc.Keys["Description"].AddPadding(padding))
                .AssignColumnValue(a => string.Join(' ',a.DescriptionKeys.Select(k=> lc.Keys[k])))
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(lc.Keys["InitialPrice"].AddPadding(padding))
                .AssignColumnValue(a => a.StartingPrice)
                .ConfigureOptions(o =>
                {
                    o.SetEuroCurrencyFormat(2);
                })
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(lc.Keys["Discount"].AddPadding(padding))
                .AssignColumnValue(a => 1 - ((IPriceable)a).GetTotalDiscountFactor())
                .ConfigureOptions(o =>
                {
                    o.SetPercentageFormat(2);
                })
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(lc.Keys["NetPrice"].AddPadding(padding))
                .CreateFormula()
                .ValueInColumn(lc.Keys["InitialPrice"].AddPadding(padding))
                .Multiply()
                .OpenParenthesis()
                .Value(1).Deduct().ValueInColumn(lc.Keys["Discount"].AddPadding(padding))
                .CloseParenthesis()
                .EndFormula()
                .ConfigureOptions(o =>
                {
                    o.SetEuroCurrencyFormat(2);
                })
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.SeriesTrait).AddPadding(padding))
                .AssignColumnValue(a => a.Product.Series.Trait)
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.PrimaryTypeTrait).AddPadding(padding))
                .AssignColumnValue(a => a.Product.PrimaryType.Trait)
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.SecondaryTypeTrait).AddPadding(padding))
                .AssignColumnValue(a => a.Product.SecondaryType.Trait)
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.FinishTrait).AddPadding(padding))
                .AssignColumnValue(a => a.SelectedFinish.Finish.Trait)
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.ShapeTrait).AddPadding(padding))
                .AssignColumnValue(a => a.Product.Shape.Trait)
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.MountingTypeTrait).AddPadding(padding))
                .AssignColumnValue(a => string.Join(Environment.NewLine, a.Product.MountingTypes.Select(mt=> mt.Trait)))
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.DimensionTrait).AddPadding(padding))
                .AssignColumnValue(a => string.Join(Environment.NewLine, a.Product.Dimensions.Select(dkvp => $"{dkvp.Key.Trait} : {dkvp.Value}mm")))
                .ConfigureOptions(o =>
                {
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(lc.Keys["AppliedRules"].AddPadding(padding))
                .AssignColumnValue(a => string.Join(Environment.NewLine, a.AppliedRules.Select(r => $"{lc.Keys[r.RuleName]} : {r.RuleApplicationDescription}")))
                .ConfigureOptions(o =>
                {
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.SizeTrait).AddPadding(padding))
                .AssignColumnValue(a => a.Product.Size.Trait)
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName(repo.GetTraitClassName(TypeOfTrait.CategoryTrait).AddPadding(padding))
                .AssignColumnValue(a => string.Join(Environment.NewLine, a.Product.Categories.Select(c=> c.Trait)))
                .BuildColumn();

            builder.CreateColumn()
                .SetColumnName($"{lc.Keys["PhotoUrl"]}- Thumbnail - Max:{AccessoriesUrlHelper.ImageThumbSize.width}x{AccessoriesUrlHelper.ImageThumbSize.height}".AddPadding(padding))
                .AssignColumnValue(a => urlHelper.GetPhotoOrDefault(a.Product.GetPhotoUrlFromFinishOrDefault(a.SelectedFinish.Finish.Code,"-"),PhotoSize.Thumbnail,"-"))
                .ConfigureOptions(o =>
                {
                    o.ShouldCreateHyperlinkInValue = true;
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName($"{lc.Keys["PhotoUrl"]}- Small - Max:{AccessoriesUrlHelper.ImageSmallSize.width}x{AccessoriesUrlHelper.ImageSmallSize.height}".AddPadding(padding))
                .AssignColumnValue(a => urlHelper.GetPhotoOrDefault(a.Product.GetPhotoUrlFromFinishOrDefault(a.SelectedFinish.Finish.Code, "-"), PhotoSize.Small, "-"))
                .ConfigureOptions(o =>
                {
                    o.ShouldCreateHyperlinkInValue = true;
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName($"{lc.Keys["PhotoUrl"]}- Medium - Max:{AccessoriesUrlHelper.ImageMediumSize.width}x{AccessoriesUrlHelper.ImageMediumSize.height}".AddPadding(padding))
                .AssignColumnValue(a => urlHelper.GetPhotoOrDefault(a.Product.GetPhotoUrlFromFinishOrDefault(a.SelectedFinish.Finish.Code, "-"), PhotoSize.Medium, "-"))
                .ConfigureOptions(o =>
                {
                    o.ShouldCreateHyperlinkInValue = true;
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName($"{lc.Keys["PhotoUrl"]}- Large - Max:{AccessoriesUrlHelper.ImageLargeSize.width}x{AccessoriesUrlHelper.ImageLargeSize.height}".AddPadding(padding))
                .AssignColumnValue(a => urlHelper.GetPhotoOrDefault(a.Product.GetPhotoUrlFromFinishOrDefault(a.SelectedFinish.Finish.Code, "-"), PhotoSize.Large, "-"))
                .ConfigureOptions(o =>
                {
                    o.ShouldCreateHyperlinkInValue = true;
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();
            builder.CreateColumn()
                .SetColumnName($"{lc.Keys["PhotoUrl"]}- Full - Max:{AccessoriesUrlHelper.ImageFullSize.width}x{AccessoriesUrlHelper.ImageFullSize.height}".AddPadding(padding))
                .AssignColumnValue(a => urlHelper.GetPhotoOrDefault(a.Product.GetPhotoUrlFromFinishOrDefault(a.SelectedFinish.Finish.Code, "-"), PhotoSize.Full, "-"))
                .ConfigureOptions(o =>
                {
                    o.ShouldCreateHyperlinkInValue = true;
                    o.ValueCellsStyle.FontSize = 8;
                })
                .BuildColumn();

            options.ReportTitle = lc.Keys["Accessories"];
            options.FirstRow = 2;
            options.FirstColumn = 2;
            options.UseSumsTable = false;
            options.ProgressTranslations.ColumnTranslation = lc.Keys["Column"];
            options.ProgressTranslations.ReportGeneratedTranslation = lc.Keys["ReportGenerated"];
            options.ProgressTranslations.GeneratingReportTranslation = lc.Keys["Generating..."];
            options.ProgressTranslations.CreatingTableFormatTranslation = lc.Keys["CreatingFilterableTable..."];
        }
    }
}

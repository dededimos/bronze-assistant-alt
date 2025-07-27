using AKSoftware.Localization.MultiLanguages;
using BathAccessoriesModelsLibrary.Services;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using ClosedXML.Excel;
using System;
using XMLClosedReporting;
using static CommonHelpers.CommonExtensions;

namespace BronzeArtWebApplication.Shared.ReportModels
{
    public class BasketItemWholesaleQuoteTemplate : AbstractReportTemplate<BasketItemViewModel>
    {
        private const int PADDING = 4;

        public BasketItemWholesaleQuoteTemplate(ILanguageContainerService lc)
        {
            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Code"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.OveriddenCode)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Description"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.OveriddenDescription)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Quantity"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.Quantity)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.NumberFormat = "0";
                       o.ShouldSumColumnValues = true;
                       o.SumCellTitle = $"{lc.Keys["Quantity"]}{Environment.NewLine}{lc.Keys["Total"]}";
                   })
                   .BuildColumn();

            builder.CreateColumn()
               .SetColumnName($"{lc.Keys["InitialPrice"]}".AddPadding(PADDING))
               .AssignColumnValue(i => i.Priceable.StartingPrice)
               .ConfigureOptions(o =>
               {
                   o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                   o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   o.SetEuroCurrencyFormat(2);
               })
               .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Discount"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.Priceable.GetTotalDiscountPercent() / 100)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.SetPercentageFormat(2);
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["NetPrice"]}".AddPadding(PADDING))
                   .CreateFormula()
                   .OpenParenthesis().Value(1).Deduct().ValueInColumn($"{lc.Keys["Discount"]}".AddPadding(PADDING)).CloseParenthesis()
                   .Multiply()
                   .ValueInColumn($"{lc.Keys["InitialPrice"]}".AddPadding(PADDING))
                   .EndFormula()
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.SetEuroCurrencyFormat(2);
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["NetPrice"]} {lc.Keys["Total"]}".AddPadding(PADDING))
                   .CreateFormula()
                   .ValueInColumn($"{lc.Keys["NetPrice"]}".AddPadding(PADDING))
                   .Multiply()
                   .ValueInColumn($"{lc.Keys["Quantity"]}".AddPadding(PADDING))
                   .EndFormula()
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.SetEuroCurrencyFormat(2);
                       o.ShouldSumColumnValues = true;
                       o.SumCellTitle = lc.Keys["Total"];
                   })
                   .BuildColumn();

            options.ReportTitle = lc.Keys["Quote"];
            options.FirstColumn = 2;
            options.FirstRow = 2;
            options.UseSumsTable = true;
            options.ProgressTranslations.ColumnTranslation = lc.Keys["Column"];
            options.ProgressTranslations.ReportGeneratedTranslation = lc.Keys["ReportGenerated"];
            options.ProgressTranslations.GeneratingReportTranslation = lc.Keys["Generating..."];
            options.ProgressTranslations.CreatingTableFormatTranslation = lc.Keys["CreatingFilterableTable..."];
        }
    }
    public class BasketItemRetailQuoteTemplate : AbstractReportTemplate<BasketItemViewModel>
    {
        private const int PADDING = 4;

        public BasketItemRetailQuoteTemplate(ILanguageContainerService lc)
        {
            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Code"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.OveriddenCode)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Description"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.OveriddenDescription)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Quantity"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.Quantity)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.NumberFormat = "0";
                       o.ShouldSumColumnValues = true;
                       o.SumCellTitle = $"{lc.Keys["Quantity"]}{Environment.NewLine}{lc.Keys["Total"]}";
                   })
                   .BuildColumn();

            builder.CreateColumn()
               .SetColumnName($"{lc.Keys["InitialPrice"]}".AddPadding(PADDING))
               .AssignColumnValue(i => i.RetailPriceable.StartingPrice)
               .ConfigureOptions(o =>
               {
                   o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                   o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   o.SetEuroCurrencyFormat(2);
               })
               .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Discount"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.RetailPriceable.GetTotalDiscountPercent() / 100)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.SetPercentageFormat(2);
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["FinalPrice"]}".AddPadding(PADDING))
                   .CreateFormula()
                   .OpenParenthesis().Value(1).Deduct().ValueInColumn($"{lc.Keys["Discount"]}".AddPadding(PADDING)).CloseParenthesis()
                   .Multiply()
                   .ValueInColumn($"{lc.Keys["InitialPrice"]}".AddPadding(PADDING))
                   .EndFormula()
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.SetEuroCurrencyFormat(2);
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["FinalPrice"]} {lc.Keys["Total"]}".AddPadding(PADDING))
                   .CreateFormula()
                   .ValueInColumn($"{lc.Keys["FinalPrice"]}".AddPadding(PADDING))
                   .Multiply()
                   .ValueInColumn($"{lc.Keys["Quantity"]}".AddPadding(PADDING))
                   .EndFormula()
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.SetEuroCurrencyFormat(2);
                       o.ShouldSumColumnValues = true;
                       o.SumCellTitle = lc.Keys["Total"];
                   })
                   .BuildColumn();

            options.ReportTitle = lc.Keys["Quote"];
            options.FirstColumn = 2;
            options.FirstRow = 2;
            options.UseSumsTable = true;
            options.VatColumnName = $"    {lc.Keys["FinalPrice"]} {lc.Keys["Total"]}    ";
            options.VatSumTitle = lc.Keys["VAT"];
            options.FinalTotalSumTitle = lc.Keys["FinalTotal"];
            options.ProgressTranslations.ColumnTranslation = lc.Keys["Column"];
            options.ProgressTranslations.ReportGeneratedTranslation = lc.Keys["ReportGenerated"];
            options.ProgressTranslations.GeneratingReportTranslation = lc.Keys["Generating..."];
            options.ProgressTranslations.CreatingTableFormatTranslation = lc.Keys["CreatingFilterableTable..."];
        }
    }
    public class BasketItemGuestQuoteTemplate : AbstractReportTemplate<BasketItemViewModel>
    {
        public const int PADDING = 4;
        public BasketItemGuestQuoteTemplate(ILanguageContainerService lc)
        {
            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Code"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.OveriddenCode)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Description"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.OveriddenDescription)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Left;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                   })
                   .BuildColumn();

            builder.CreateColumn()
                   .SetColumnName($"{lc.Keys["Quantity"]}".AddPadding(PADDING))
                   .AssignColumnValue(i => i.Quantity)
                   .ConfigureOptions(o =>
                   {
                       o.ValueCellsStyle.HorizontalAlignment = XLAlignmentHorizontalValues.Right;
                       o.ValueCellsStyle.VerticalAlignment = XLAlignmentVerticalValues.Center;
                       o.NumberFormat = "0";
                       o.ShouldSumColumnValues = true;
                       o.SumCellTitle = $"{lc.Keys["Quantity"]}{Environment.NewLine}{lc.Keys["Total"]}";
                   })
                   .BuildColumn();

            options.ReportTitle = lc.Keys["Request"];
            options.FirstColumn = 2;
            options.FirstRow = 2;
            options.UseSumsTable = false;
            options.ProgressTranslations.ColumnTranslation = lc.Keys["Column"];
            options.ProgressTranslations.ReportGeneratedTranslation = lc.Keys["ReportGenerated"];
            options.ProgressTranslations.GeneratingReportTranslation = lc.Keys["Generating..."];
            options.ProgressTranslations.CreatingTableFormatTranslation = lc.Keys["CreatingFilterableTable..."];
        }
    }
}

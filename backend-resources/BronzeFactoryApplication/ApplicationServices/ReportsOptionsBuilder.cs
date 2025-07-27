using DataAccessLib.NoSQLModels;
using ReportingHelperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices
{
    public static class ReportsOptionsBuilder
    {
        private static Func<string, string> columnNameGeneralManipulationFunc = (x) =>
        {
            // Translate the Column Name
            var stringToReturn = x.TryTranslateKeyWithoutError();
            // Add a new Line for each Word
            stringToReturn = stringToReturn.Replace(" ", Environment.NewLine);
            return stringToReturn;
        };
        private static Func<string, string> enumsValueManipulationFunction = (x) => x.TryTranslateKeyWithoutError();
        private static Func<string, string> dateValuesManipulationFunction = (x) =>
        {
            var isParsed = DateTime.TryParse(x, out DateTime result);
            return isParsed ? result.ToString("dd-MM-yyyy") : x;
        };
        private static Func<string, string> addDashesFunc = (x) => $"- {x} -";


        /// <summary>
        /// Returns the Report Options for the Requested Type
        /// </summary>
        /// <param name="reportedType">The Type of the Reported Object</param>
        /// <returns></returns>
        public static ReportOptions GetReportOptions(Type reportedType)
        {
            switch (reportedType)
            {
                case Type t when t == typeof(GlassOrderRow):
                    return GetGlassOrderRowOptions();
                case Type t when t == typeof(StockedGlassRow):
                    return GetStockedGlassRowOptions();
                case Type t when t == typeof(Glass):
                    return GetGlassOptions();
                case Type t when t == typeof(CabinOrderRow):
                    return GetCabinOrderRowOptions();
                case Type t when t == typeof(Cabin):
                    return GetCabinOptions();
                case Type t when t == typeof(CabinPartEntity):
                    return GetCabinPartEntityOptions();
                default:
                    MessageService.Info($"There is no Built in ReportOptions Generator for this Type :{Environment.NewLine}{reportedType.Name}{Environment.NewLine}{Environment.NewLine}Default Options will be Used", "Missing Report Options...");
                    return new ReportOptions() { 
                        FlattenNestedObjectsProperties = true, 
                        ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc,
                        EnumsValueManipulationFunction = enumsValueManipulationFunction,
                        DatesValueManipulationFunction = dateValuesManipulationFunction,
                    };
            }
        }

        private static ReportOptions GetGlassOrderRowOptions()
        {
            ReportOptions options = new()
            {
                FlattenNestedObjectsProperties = true,
                ExcludedProperties = new() { nameof(GlassOrderRow.ParentCabinRow), nameof(GlassOrderRow.CabinRowKey), nameof(GlassOrderRow.CancelledQuantity), nameof(GlassOrderRow.LastModified), nameof(GlassOrderRow.OrderedGlass.HasStep), nameof(GlassOrderRow.OrderedGlass.HasRounding), nameof(GlassOrderRow.OrderedGlass.GlassType), nameof(GlassOrderRow.RowId), nameof(GlassOrderRow.SpecialDrawNumber), nameof(GlassOrderRow.SpecialDrawString) },
                ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc,
                EnumsValueManipulationFunction = enumsValueManipulationFunction,
                DatesValueManipulationFunction = dateValuesManipulationFunction,
                ColumnSortPriorityList = new() { nameof(GlassOrderRow.OrderedGlass.Draw), nameof(GlassOrderRow.OrderedGlass.Thickness), nameof(GlassOrderRow.OrderedGlass.Finish), nameof(GlassOrderRow.OrderedGlass.Length), nameof(GlassOrderRow.OrderedGlass.Height), nameof(GlassOrderRow.OrderedGlass.StepLength), nameof(GlassOrderRow.OrderedGlass.StepHeight), nameof(GlassOrderRow.OrderedGlass.CornerRadiusTopRight), nameof(GlassOrderRow.OrderedGlass.CornerRadiusTopLeft), nameof(GlassOrderRow.Quantity), nameof(GlassOrderRow.FilledQuantity), nameof(GlassOrderRow.Created), nameof(GlassOrderRow.OrderId), nameof(GlassOrderRow.ReferencePA0), nameof(GlassOrderRow.Status), nameof(GlassOrderRow.Notes) }
            };
            options.SpecificPropertiesManipulationValueFunctions.Add(nameof(GlassOrderRow.ReferencePA0), addDashesFunc);
            options.SpecificPropertiesManipulationValueFunctions.Add(nameof(GlassOrderRow.OrderId), addDashesFunc);
            return options;
        }
        private static ReportOptions GetGlassOptions()
        {
            ReportOptions options = new()
            {
                FlattenNestedObjectsProperties = false,
                ExcludedProperties = new() { nameof(Glass.HasRounding), nameof(Glass.HasStep)},
                ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc,
                EnumsValueManipulationFunction = enumsValueManipulationFunction,
                DatesValueManipulationFunction = dateValuesManipulationFunction,
                ColumnSortPriorityList = new() { nameof(Glass.Draw), nameof(Glass.Thickness), nameof(Glass.Finish), nameof(Glass.Length), nameof(Glass.Height), nameof(Glass.StepLength), nameof(Glass.StepHeight), nameof(Glass.CornerRadiusTopRight), nameof(Glass.CornerRadiusTopLeft)},
            };
            return options;
        }
        private static ReportOptions GetStockedGlassRowOptions()
        {
            ReportOptions options = new()
            {
                FlattenNestedObjectsProperties = true,
                ExcludedProperties = [nameof(StockedGlassRow.Glass.HasRounding), nameof(StockedGlassRow.Glass.HasStep),nameof(StockedGlassRow.RowId), nameof(StockedGlassRow.LastModified)],
                ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc,
                EnumsValueManipulationFunction = enumsValueManipulationFunction,
                DatesValueManipulationFunction = dateValuesManipulationFunction,
                ColumnSortPriorityList = [nameof(StockedGlassRow.Glass.Draw), nameof(StockedGlassRow.Glass.Thickness), nameof(StockedGlassRow.Glass.Finish), nameof(StockedGlassRow.Glass.Length), nameof(StockedGlassRow.Glass.Height), nameof(StockedGlassRow.Glass.StepLength), nameof(StockedGlassRow.Glass.StepHeight), nameof(StockedGlassRow.Glass.CornerRadiusTopRight), nameof(StockedGlassRow.Glass.CornerRadiusTopLeft),nameof(StockedGlassRow.Quantity),nameof(StockedGlassRow.Created)],
            };
            return options;
        }
        private static ReportOptions GetCabinOrderRowOptions()
        {
            ReportOptions options = new()
            {
                FlattenNestedObjectsProperties = true,
                ExcludedProperties = [nameof(CabinOrderRow.SynthesisKey), nameof(CabinOrderRow.CabinKey), nameof(CabinOrderRow.LastModified), nameof(CabinOrderRow.RowId), nameof(CabinOrderRow.OrderedCabin.Parts), nameof(CabinOrderRow.OrderedCabin.Constraints), nameof(CabinOrderRow.OrderedCabin.Glasses), nameof(CabinOrderRow.OrderedCabin.IsCodeOverriden), nameof(CabinOrderRow.OrderedCabin.HasStep), nameof(CabinOrderRow.OrderedCabin.IsReversible), nameof(CabinOrderRow.OrderedCabin.LengthMax), nameof(CabinOrderRow.OrderedCabin.LengthMin)],
                ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc,
                EnumsValueManipulationFunction = enumsValueManipulationFunction,
                DatesValueManipulationFunction = dateValuesManipulationFunction,
                ColumnSortPriorityList = [nameof(CabinOrderRow.OrderedCabin.Code), nameof(CabinOrderRow.OrderedCabin.Model), nameof(CabinOrderRow.OrderedCabin.Thicknesses), nameof(CabinOrderRow.OrderedCabin.GlassFinish), nameof(CabinOrderRow.OrderedCabin.NominalLength), nameof(CabinOrderRow.OrderedCabin.Height), nameof(CabinOrderRow.OrderId), nameof(CabinOrderRow.ReferencePA0), nameof(CabinOrderRow.Notes), nameof(CabinOrderRow.GlassesRows), nameof(CabinOrderRow.OrderedCabin.MetalFinish)],
            };
            options.SpecificPropertiesManipulationValueFunctions.Add(nameof(CabinOrderRow.ReferencePA0), addDashesFunc);
            options.SpecificPropertiesManipulationValueFunctions.Add(nameof(CabinOrderRow.OrderId), addDashesFunc);
            return options;
        }
        private static ReportOptions GetCabinOptions()
        {
            ReportOptions options = new()
            {
                FlattenNestedObjectsProperties = false,
                ExcludedProperties = [nameof(Cabin.Parts), nameof(Cabin.Constraints), nameof(Cabin.IsCodeOverriden), nameof(Cabin.HasStep), nameof(Cabin.IsReversible), nameof(Cabin.LengthMax), nameof(Cabin.LengthMin)],
                ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc,
                EnumsValueManipulationFunction = enumsValueManipulationFunction,
                DatesValueManipulationFunction = dateValuesManipulationFunction,
                ColumnSortPriorityList = [nameof(Cabin.Code), nameof(Cabin.Model), nameof(Cabin.Thicknesses), nameof(Cabin.GlassFinish), nameof(Cabin.NominalLength), nameof(Cabin.Height), nameof(Cabin.MetalFinish), nameof(Cabin.Glasses)],
            };
            return options;
        }
        private static ReportOptions GetCabinPartEntityOptions()
        {
            ReportOptions options = new()
            {
                FlattenNestedObjectsProperties = true,
                ExcludedProperties = [nameof(CabinPartEntity.Uses), nameof(CabinPartEntity.Id), nameof(CabinPartEntity.LastModified),nameof(CabinPartEntity.Part.AdditionalParts),nameof(CabinPartEntity.Part.Quantity)],
                DatesValueManipulationFunction = dateValuesManipulationFunction,
                ColumnSortPriorityList = [nameof(CabinPartEntity.IdAsString),nameof(CabinPartEntity.Part.Code),nameof(CabinPartEntity.LanguageDescriptions)],
                EnumsValueManipulationFunction = enumsValueManipulationFunction,
                ColumnNamesTotalManipulationFunction = columnNameGeneralManipulationFunc
            };
            return options;
        }
    }
}

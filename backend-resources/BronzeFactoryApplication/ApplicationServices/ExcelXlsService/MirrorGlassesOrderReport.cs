using ClosedXML.Excel;
using CommonHelpers;
using CommonOrderModels;
using HandyControl.Tools.Extension;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorsOrderModels;
using ShapesLibrary.Enums;
using ShapesLibrary.ShapeInfoModels;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XMLClosedReporting.AdvancedReportModels;

namespace BronzeFactoryApplication.ApplicationServices.ExcelXlsService
{
    public class MirrorGlassesOrderReport : AdvancedReportTemplate<MirrorOrderRow>
    {
        private const string undefinedRowObject = "UndefinedRowObject";
        private MirrorOrderRowCanCombineComparer combineComparer = new();

        public MirrorGlassesOrderReport()
        {
            var lang = ((App)Application.Current).SelectedLanguage;

            builder.ConfigureOptions(options =>
            {
                options.FirstColumn = 2;
                options.FirstRow = 2;
                options.MarginFromLeft = 10;
                options.MarginFromTop = 20;
                options.ReportTitleRowHeight = 40d;
                options.ReportNotesRowHeight = 80d;
                options.RowsBetweenTables = 1;
                options.SegregateItemsBy = (row) => row.RowItem?.GeneralShapeType ?? BronzeMirrorShape.UndefinedMirrorShape;
                options.Strings.NotesTitleString = "lngOrderNotes".TryTranslateKeyWithoutError();
            })
            .CreateTables(tablesBuilder =>
            {
                tablesBuilder.BuildSharedColumns(sharedBuilder =>
                             {
                                 if (builder.Options.IncludeIncrementalLineNumbers)
                                 {
                                     sharedBuilder.CreateColumn(incrementalLineNumberColumnName, "lngIncrementalNumber".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => 0)
                                              .SetColumnOrderNumber(0)
                                              .BuildColumn();
                                 }

                                 sharedBuilder.CreateColumn("Quantity", "lngQuantity".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => r.Quantity)
                                              .SetColumnOrderNumber(3)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("Thickness", "lngThickness".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => r.RowItem?.GlassThickness.ToString().TryTranslateKeyWithoutError() ?? undefinedRowObject)
                                              .SetColumnOrderNumber(4)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("Type", "lngType".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => r.RowItem?.GlassType.ToString().TryTranslateKeyWithoutError() ?? undefinedRowObject)
                                              .SetColumnOrderNumber(5)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("Draw", "lngDraw".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => r.RowItem?.GeneralShapeType.ToString().TryTranslateKeyWithoutError() ?? undefinedRowObject)
                                              .SetColumnOrderNumber(6)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("Sandblast", "lngSandblast".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r =>
                                              {
                                                  if (r.RowItem == null) return undefinedRowObject;

                                                  string mainSandblast = r.RowItem.Sandblast?.LocalizedDescriptionInfo.Name.GetLocalizedValue(lang) ?? "lngWithoutSandblast".TryTranslateKeyWithoutError();
                                                  var magnifiers = r.RowItem.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType);
                                                  if (magnifiers.Count == 0) return mainSandblast;
                                                  else
                                                  {
                                                      List<string> sandblasts = [];
                                                      if (r.RowItem.Sandblast != null) sandblasts.Add(mainSandblast); //add the sandblast only if there is one 
                                                      sandblasts.AddRange(magnifiers.Select(m => m.LocalizedDescriptionInfo.Name.GetLocalizedValue(lang)));
                                                      return string.Join(Environment.NewLine, sandblasts);
                                                  }
                                              })
                                              .ConfigureOptions(options => options.ValueCellsStyle.WrapText = true)
                                              .SetColumnOrderNumber(7)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("Processes", "lngVariousProcesses".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r =>
                                              {
                                                  if (r.RowItem == null) return undefinedRowObject;
                                                  //Find the rounded corner module if any
                                                  var roundedCornersModule = r.RowItem.ModulesInfo.ModulesOfType(MirrorModuleType.RoundedCornersModuleType).FirstOrDefault();
                                                  //Seperate the ModuleInfo object to use it below in its concrete type RoundedCOrnersModuleInfo
                                                  var roundedCornersInfo = roundedCornersModule != null ? ((RoundedCornersModuleInfo)roundedCornersModule.ModuleInfo) : null;

                                                  //Assign a String value to the roundedCornersInfoString
                                                  string roundedCornersInfoString = string.Empty;

                                                  if(roundedCornersInfo != null )
                                                  {
                                                      roundedCornersInfoString = roundedCornersInfo.HasTotalRadius
                                                                ? $"{roundedCornersInfo.TotalRadius.ToString(CommonConstants.StringFormats.ZeroDecimals)}mm"
                                                                : "lngVariousRadiuses".TryTranslateKeyWithoutError();
                                                  }

                                                  //Assign the string value that will be placed on the Cell for the Rounded Corners
                                                  //Will be like "RoundedCornersModuleTypeTranslation-R{RadiusValue}" or Mixed Radius
                                                  string roundedCorners = roundedCornersModule != null
                                                  ? $"{MirrorModuleType.RoundedCornersModuleType.ToString().TryTranslateKeyWithoutError()}-R: {roundedCornersInfoString}"
                                                  : string.Empty;
                                                  
                                                  var processes = r.RowItem.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType);
                                                  if (processes.Count == 0) return string.IsNullOrEmpty(roundedCorners) ? " - " : roundedCorners;
                                                  else
                                                  {
                                                      //Get All the Processes and make them into a string with "Hole-ShapeInfoTypeTranslation-DimensionsString" , distinct to eliminate duplicates
                                                      List<string> allProcesses = [];
                                                      allProcesses!.AddIf(!string.IsNullOrEmpty(roundedCorners),roundedCorners);
                                                      allProcesses.AddRange(processes.Select(p => $"{"lngHole".TryTranslateKeyWithoutError()}-{((MirrorProcessModuleInfo)(p.ModuleInfo)).ProcessShape.ShapeType.ToString().TryTranslateKeyWithoutError()}-{((MirrorProcessModuleInfo)(p.ModuleInfo)).ProcessShape.DimensionString}").Distinct());
                                                      return string.Join(Environment.NewLine, allProcesses);
                                                  }
                                              })
                                              .ConfigureOptions(options => options.ValueCellsStyle.WrapText = true)
                                              .SetColumnOrderNumber(8)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("PAOPAM", "PAO/PAM")
                                              .AssignCellsValue(r => r.Metadata.TryGetValue(MirrorOrderRow.PaoPamMetadataKey, out string? paoPam)
                                              ? (string.IsNullOrEmpty(paoPam) ? "????" : $"{paoPam}")
                                              : "????")
                                              .ConfigureOptions(options => options.ValueCellsStyle.WrapText = true)
                                              .SetColumnOrderNumber(9)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("LineNotes", "lngNotes".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => !string.IsNullOrEmpty(r.Notes) ? r.Notes : " - ")
                                              .ConfigureOptions(options => options.ValueCellsStyle.WrapText = true)
                                              .SetColumnOrderNumber(10)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("SQM", "lngSquareMeters".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => r.RowItem is null ? undefinedRowObject : r.RowItem.MirrorGlassShape.GetArea() / 1000000d * r.Quantity)
                                              .ConfigureOptions(o=> o.ValueCellsStyle.NumberFormat = CommonConstants.StringFormats.TwoDecimalsSquareMeterXML)
                                              .SetColumnOrderNumber(11)
                                              .BuildColumn();

                                 sharedBuilder.CreateColumn("RunningMeters", "lngRunningMeters".TryTranslateKeyWithoutError())
                                              .AssignCellsValue(r => r.RowItem is null ? undefinedRowObject : r.RowItem.MirrorGlassShape.GetPerimeter() / 1000d * r.Quantity)
                                              .ConfigureOptions(o => o.ValueCellsStyle.NumberFormat = CommonConstants.StringFormats.TwoDecimalsMeterXML)
                                              .SetColumnOrderNumber(12)
                                              .BuildColumn();
                             });

                tablesBuilder.CreateTable("CircularMirrors", "lngCircularMirrors".TryTranslateKeyWithoutError())
                             .SetTableSegregationKey(BronzeMirrorShape.CircleMirrorShape)
                             .UsePreConfiguredOptions()
                             .BuildColumns(columnsBuilder =>
                             {
                                 columnsBuilder.CreateColumn("Diameter", "lngDiameter".TryTranslateKeyWithoutError())
                                               .AssignCellsValue(r => r.RowItem == null ? undefinedRowObject : (r.RowItem.MirrorGlassShape as CircleInfo)?.Diameter ?? 0)
                                               .SetColumnOrderNumber(1)
                                               .ConfigureOptions(o=> o.NumberOfOccupiedColumns = 2)
                                               .BuildColumn();
                             })
                             .SetTableOrderNo(2)
                             .BuildTable();

                tablesBuilder.BuildSharedColumns(sharedBuilder =>
                {
                    sharedBuilder.CreateColumn("Length", "lngLength".TryTranslateKeyWithoutError())
                                 .AssignCellsValue(r => r.RowItem == null ? undefinedRowObject : r.RowItem.MirrorGlassShape.TotalLength)
                                 .SetColumnOrderNumber(1)
                                 .BuildColumn();
                    sharedBuilder.CreateColumn("Height", "lngHeight".TryTranslateKeyWithoutError())
                                 .AssignCellsValue(r => r.RowItem == null ? undefinedRowObject : r.RowItem.MirrorGlassShape.TotalHeight)
                                 .SetColumnOrderNumber(2)
                                 .BuildColumn();
                });


                tablesBuilder.CreateTable("RectangularMirrors", "lngRectangularMirrors".TryTranslateKeyWithoutError())
                         .SetTableSegregationKey(BronzeMirrorShape.RectangleMirrorShape)
                         .UsePreConfiguredOptions()
                         .BuildColumns(columnsBuilder =>
                         {
                             //nothing  
                         })
                         .SetTableOrderNo(1)
                         .BuildTable();



                tablesBuilder.CreateTable("OtherMirrors", "lngOtherShapeMirrors".TryTranslateKeyWithoutError())
                             .SetTableAsLastTable()
                             .UsePreConfiguredOptions()
                             .BuildColumns(columnsBuilder =>
                             {
                                 //nothing
                             })
                             .SetTableOrderNo(3)
                             .BuildTable();
            })
            .BuildReportTemplate();
        }

        public async Task GenerateReport(MirrorsOrder order,
                                         XLWorkbook wb,
                                         IProgress<TaskProgressReport>? progress = null,
                                         CancellationToken? token = null)
        {
            builder.Options.Strings.ReportTitle = $"{"lngOrder".TryTranslateKeyWithoutError()}:{order.OrderNo}{new string(' ', 10)}----{new string(' ', 10)}{"lngDate".TryTranslateKeyWithoutError()}:{DateTime.Now:dd-MM-yyyy}";
            builder.Options.Strings.Notes = !string.IsNullOrEmpty(order.Notes) ? order.Notes : " - ";
            //Combine the rows that are equal
            var combined = CombineRows(order.Rows);

            await GenerateReportFromRows(combined, wb, progress, token);
        }

        /// <summary>
        /// Returns a new List with any Rows that where equal combined
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private List<MirrorOrderRow> CombineRows(List<MirrorOrderRow> rows)
        {
            var groups = rows.GroupBy(r => r, combineComparer).Select(group => group.ToList());
            List<MirrorOrderRow> combinedRows = [];
            foreach (var group in groups)
            {
                var combinedRow = CombineEqualRowsIntoOne(group);
                combinedRows.Add(combinedRow);
            }
            return combinedRows;
        }

        /// <summary>
        /// Combines the rows into one
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private MirrorOrderRow CombineEqualRowsIntoOne(IEnumerable<MirrorOrderRow> rows)
        {
            if (!rows.Any()) throw new ArgumentException("Cannot combine an empty list of rows");
            else if (rows.Count() == 1) return rows.First();
            else
            {
                var first = rows.First();
                var remaining = rows.Skip(1).ToList();

                //Add the PAO/PAM metadata if it does not exist
                if (first.Metadata.ContainsKey(MirrorOrderRow.PaoPamMetadataKey) == false) first.Metadata.Add(MirrorOrderRow.PaoPamMetadataKey, string.Empty);
                //Save the metadata to a new List and add the metadata of the other rows if any
                List<string> combinedMetadata = [first.Metadata[MirrorOrderRow.PaoPamMetadataKey]];
                foreach (var row in remaining)
                {
                    first.Quantity += row.Quantity;
                    first.FilledQuantity += row.FilledQuantity;
                    first.CancelledQuantity += row.CancelledQuantity;
                    string? metadata = row.GetMetadata(MirrorOrderRow.PaoPamMetadataKey);
                    if (!string.IsNullOrEmpty(metadata)) combinedMetadata.Add(metadata);
                }
                //Combine the metadata with Environment.NewLine
                first.Metadata[MirrorOrderRow.PaoPamMetadataKey] = string.Join(Environment.NewLine, combinedMetadata);

                return first;
            }
        }
    }

}

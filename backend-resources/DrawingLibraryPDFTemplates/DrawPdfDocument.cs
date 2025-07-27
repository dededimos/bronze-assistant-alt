using CommonHelpers.Exceptions;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.ConcreteGraphics;
using DrawingLibrary.Models.PresentationOptions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPdfLibrary;
using ShapesLibrary.Services;
using SkiaSharp;

namespace DrawingLibraryPDFTemplates
{
    public class DrawPdfDocument : IDocument
    {
        private readonly ITechnicalDrawing drawing;
        private readonly DrawPdfDocumentOptions options;

        /// <summary>
        /// Creates a pdf Document with the corresponding Drawing ,its best for the Drawing to be already scaled to the container so that the Dimensions and Text appear always the same size
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="options"></param>
        public DrawPdfDocument(ITechnicalDrawing drawing, DrawPdfDocumentOptions? options = null)
        {
            this.drawing = drawing;
            this.options = options ?? new();
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Author = "Bronze Factory Application",
                Title = "Bronze Drawing",
                Subject = "Drawing",
                Keywords = "Drawing",
                Creator = "G.Dededimos",
                Producer = "Bronze Art"
            };
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(new PageSize(options.PageSize.Width, options.PageSize.Height));
                page.Margin(options.PageMargin);
                page.DefaultTextStyle(x => x.FontSize(options.FontSize));
                page.Content()
                .Height(400)
                .BorderBottom(1)
                .PaddingTop((float)drawing.ContainerOptions.ContainerMarginTop)
                .PaddingBottom((float)drawing.ContainerOptions.ContainerMarginBottom)
                .PaddingLeft((float)drawing.ContainerOptions.ContainerMarginLeft)
                .PaddingRight((float)drawing.ContainerOptions.ContainerMarginRight)
                .Element(c => ComposeDraw(c));

                //The footer bugs out to infinite length if anything is extended vertically .
                //because it repeats in each page . So its advisable to put the info table of the draw in the content if ExtendVertical is Needed
                page.Footer().AlignRight().Element(c => ComposeDrawInfoTable(c));
            });
        }

        private void ComposeDrawInfoTable(IContainer container)
        {
            container.Border(1).Table(table =>
            {
                table.ExtendLastCellsToTableBottom();
                // Define the number of columns in the footer table
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(74);
                    columns.ConstantColumn(100);
                    columns.ConstantColumn(173);
                    columns.ConstantColumn(173);
                });
                #region Notes
                table.Cell().RowSpan(4).Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.Notes);
                        column.Item().CellTextValue(options.TableInfo.Notes);
                    });
                });
                #endregion

                #region Responsible Dept
                table.Cell().Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.ResponsibleDep);
                        column.Item().CellTextValue(options.TableInfo.ResponsibleDepartment);
                    });
                });
                #endregion

                #region Technical Reference
                table.Cell().Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.TechnicalRef);
                        column.Item().CellTextValue(options.TableInfo.TechnicalReference);
                    });
                });
                #endregion

                #region Created By - Approved By - Internal Referece
                table.Cell().ColumnSpan(2).Element(StyleCellContainer).Element(c =>
                {
                    c.Row(rowBlock =>
                    {
                        #region Created by
                        rowBlock.ConstantItem(116).BorderRight(0.5f).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.CreatedBy);
                            column.Item().CellTextValue(options.TableInfo.CreatedBy);
                        });
                        #endregion
                        #region Approved By
                        rowBlock.ConstantItem(90).BorderRight(0.5f).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.ApprovedBy);
                            column.Item().CellTextValue(options.TableInfo.ApprovedBy);
                        });
                        #endregion
                        #region Internal Referece
                        rowBlock.ConstantItem(140).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.InternalReference);
                            column.Item().CellTextValue(options.TableInfo.InternalReferenceNotes).FontSize(8);
                        });
                        #endregion
                    });
                });
                #endregion

                #region Legal Owner
                table.Cell().RowSpan(3).ColumnSpan(2).Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.LegalOwner);
                        column.Item().CellTextValue(options.TableInfo.LegalOwner);
                        column.Item().Height(5);
                        column.Item().Height(50).Width(163).Image(options.TableInfo.LogoImageByteArray).FitArea();
                    });
                });
                #endregion

                #region Document Type
                table.Cell().Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.DocumentType);
                        column.Item().CellTextValue(options.TableInfo.DocumentType);
                    });
                });
                #endregion

                #region Document Status
                table.Cell().Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.DocumentStatus);
                        column.Item().CellTextValue(options.TableInfo.DocumentStatus);
                    });
                });
                #endregion

                #region Title , Supplementary Title
                table.Cell().RowSpan(2).Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.DrawTitle);
                        column.Item().CellTextValue(options.TableInfo.DrawItemTitle).FontSize(14);
                    });
                });
                #endregion

                #region Code
                table.Cell().Element(StyleCellContainer).Element(c =>
                {
                    c.StyleCellInnerColumn().Column(column =>
                    {
                        column.Item().CellTextLabel(options.TableInfo.Titles.Code);
                        column.Item().CellTextValue(options.TableInfo.Code);
                    });
                });
                #endregion

                #region Rev , Date of Issue , Lang , Sheet
                table.Cell().Element(StyleCellContainer).Element(c =>
                {
                    c.Row(rowBlock =>
                    {
                        #region Revision
                        rowBlock.ConstantItem(31).BorderRight(0.5f).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.Revision);
                            column.Item().AlignBottom().CellTextValue(options.TableInfo.Revision);
                        });
                        #endregion
                        #region Date of Issue
                        rowBlock.ConstantItem(65).BorderRight(0.5f).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.DateOfIssue);
                            column.Item().CellTextValue(options.TableInfo.DateOfIssue);
                        });
                        #endregion
                        #region Lang
                        rowBlock.ConstantItem(38).BorderRight(0.5f).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.Language);
                            column.Item().CellTextValue(options.TableInfo.LanguageTwoLetterISO);
                        });
                        #endregion
                        #region Sheet
                        rowBlock.ConstantItem(39).StyleCellInnerColumn().Column(column =>
                        {
                            column.Item().CellTextLabel(options.TableInfo.Titles.Sheet);
                            column.Item().CellTextValue("1/1");
                        });
                        #endregion
                    });
                });
                #endregion

            });
        }

        // Style for table cells
        private IContainer StyleCellContainer(IContainer c)
        {
            return c
                .Padding(0)
                .Border(0.5f);
        }

        /// <summary>
        /// Composes the Draw
        /// </summary>
        /// <param name="container"></param>
        private void ComposeDraw(IContainer container)
        {
            container.SkiaSharpCanvas((canvas, size) =>
            {
                //if (((App)Application.Current).SelectedTheme == "Dark")
                //{
                //    var brush = Application.Current.TryFindResource("RegionBrush") as SolidColorBrush ?? Brushes.Black;
                //    var color = SkiaSharpHelpers.SKColorFromHex(brush.Color.ToString());
                //    canvas.Clear(SKColors.Black);
                //}
                /*else */
                canvas.Clear(SKColors.White);

                if (options.ScaleDrawToFit) ScaleDrawToFitCanvas(canvas, size);

                foreach (var draw in drawing.AllDrawings)
                {
                    if (draw.PresentationOptions.Stroke.IsSolidColor && draw.PresentationOptions.Stroke.Color == DrawBrushes.White.Color)
                    {
                        draw.PresentationOptions.Stroke = DrawBrushes.Black;
                    }
                    if (draw.PresentationOptions.Fill.IsSolidColor && draw.PresentationOptions.Fill.Color == DrawBrushes.White.Color)
                    {
                        draw.PresentationOptions.Fill = DrawBrushes.Black;
                    }
                    AddFillStrokeOfDrawingOnCanvas(canvas, draw);
                    AddTextOfDrawingOnCanvas(canvas, draw);
                }
            });
        }
        private void ScaleDrawToFitCanvas(SKCanvas canvas, Size size)
        {
            //Set the Draw Center Point to 0,0
            drawing.SetTotalDrawLocation(new(0, 0));

            //Get the bounding Box including the Sizes of the Dimensions (this does not include the text sizes)
            var bBox = drawing.GetTotalBoundingBox();

            //The Length and Height of the Box are the Drawing's Dimensions
            float drawingWidth = (float)bBox.Length;
            float drawingHeight = (float)bBox.Height;

            //The container Size are the Dimensions of the Canvas
            float canvasWidth = size.Width;
            float canvasHeight = size.Height;

            //Find the scale to fit the Draw on the Canvas , The Draw will always scale until it fits the length or the height of the canvas
            float scaleX = canvasWidth / drawingWidth;
            float scaleY = canvasHeight / drawingHeight;

            float scale = Math.Min(scaleX, scaleY);

            //Offsets to bring the Draw into view from 0,0
            float offsetXBringIntoView = drawingWidth * scale / 2;
            float offsetYBringIntoView = drawingHeight * scale / 2;
            //Offsets to center the Draw on the container
            float offsetCenterX = (canvasWidth - drawingWidth * scale) / 2;
            float offsetCenterY = (canvasHeight - drawingHeight * scale) / 2;

            float offsetX = offsetXBringIntoView + offsetCenterX;
            float offsetY = offsetYBringIntoView + offsetCenterY;

            // Apply offsets translation for centering
            canvas.Translate(offsetX, offsetY);

            // Then Apply scale (cannot go first messes up the translation)
            canvas.Scale(scale);
        }
        private static void AddFillStrokeOfDrawingOnCanvas(SKCanvas canvas, IDrawing draw)
        {
            //Create the paint object
            using var paint = new SKPaint();
            //Convert the SVG Path to skia Path
            var path = SKPath.ParseSvgPathData(draw.GetPathDataString());

            //Set fill properties

            paint.Style = SKPaintStyle.Fill;
            if (draw is DimensionLineDrawing dim && (dim.LineOptions.IncludeEndArrow || dim.LineOptions.IncludeStartArrow))
            {
                //First check weather its a dimension , which only Fills arrows and uses the Stroke of the Dimension to Fill them
                var arrowsPath = SKPath.ParseSvgPathData(dim.GetArrowsPathDataString());
                paint.Color = SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Stroke.Color);
                canvas.DrawPath(arrowsPath, paint);
            }
            else if (draw.PresentationOptions.FillPattern != FillPatternType.NoPattern)
            {
                //SET Fill of paint to none otherwise fills shape
                paint.Color = SKColor.Empty;
                switch (draw.PresentationOptions.FillPattern)
                {
                    case FillPatternType.DotPattern:
                        //The Bitmap pattern blurs and is not good
                        SkiaSharpHelpers.FillShapeWithDotPattern(canvas, path, SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Fill.Color), 3, 0.5f);
                        break;
                    case FillPatternType.HatchLine45DegPattern:
                        SkiaSharpHelpers.FillShapeWithHatchLinePattern(canvas, path, SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Fill.Color), 5, 45, 1);
                        break;
                    case FillPatternType.HatchLine225DegPattern:
                        SkiaSharpHelpers.FillShapeWithHatchLinePattern(canvas, path, SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Fill.Color), 5, 225, 1);
                        break;
                    case FillPatternType.HatchLineHorizontalPattern:
                        SkiaSharpHelpers.FillShapeWithHatchLinePattern(canvas, path, SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Fill.Color), 5, 0, 1);
                        break;
                    case FillPatternType.HatchLineVerticalPattern:
                        SkiaSharpHelpers.FillShapeWithHatchLinePattern(canvas, path, SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Fill.Color), 5, 90, 1);
                        break;
                    case FillPatternType.NoPattern:
                    default:
                        throw new EnumValueNotSupportedException(draw.PresentationOptions.FillPattern);
                }
            }
            else if (draw.PresentationOptions.Fill.IsSolidColor)
            {
                paint.Color = SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Fill.Color);
            }
            else
            {
                paint.Shader = ConvertDrawBrushToLinearGradient(draw.PresentationOptions.Fill, path.Bounds);
            }

            // Draw the Fill of the shape if the color is not Empty (Otherwise it draws Black)
            if (draw is not DimensionLineDrawing && (!paint.Color.Equals(SKColor.Empty) || paint.Shader != null) )
            {
                canvas.DrawPath(path, paint);
            }

            //reset the Paint Style so that the stroke gets applied
            paint.Color = SKColor.Empty;
            paint.Shader = null;

            // Set stroke properties
            paint.Style = SKPaintStyle.Stroke;

            if (draw.PresentationOptions.Stroke.IsSolidColor)
            {
                paint.Color = SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Stroke.Color);
            }
            else
            {
                paint.Shader = ConvertDrawBrushToLinearGradient(draw.PresentationOptions.Stroke, path.Bounds);
            }


            paint.StrokeWidth = (float)draw.PresentationOptions.StrokeThickness;

            if (draw.PresentationOptions.StrokeDashArray.Count != 0)
            {
                paint.PathEffect = SKPathEffect.CreateDash(draw.PresentationOptions.StrokeDashArray.Select(d => (float)d).ToArray(), 0);
            }

            // Draw the outline (stroke)
            if (!paint.Color.Equals(SKColor.Empty) || paint.Shader != null) 
            {
                canvas.DrawPath(path, paint);
            }
        }
        private static void AddTextOfDrawingOnCanvas(SKCanvas canvas, IDrawing draw)
        {
            //early escape
            if (string.IsNullOrEmpty(draw.DrawingText)) return;

            //Set the texts properties
            using var paint = new SKPaint();

            //early escape if color is transparent and there is no Gradient
            if (paint.Color.Equals(SKColor.Empty) && paint.Shader == null) return;

            paint.TextSize = (float)draw.PresentationOptions.TextHeight;
            paint.IsAntialias = true;
            paint.TextAlign = SKTextAlign.Center;
            paint.Typeface = SKTypeface.Default;

            SKRect textBounds = new();
            float textWidth = paint.MeasureText(draw.DrawingText, ref textBounds);

            if (draw.PresentationOptions.Stroke.IsSolidColor)
            {
                paint.Color = SkiaSharpHelpers.SKColorFromHex(draw.PresentationOptions.Stroke.Color);
            }
            else
            {
                paint.Shader = ConvertDrawBrushToLinearGradient(draw.PresentationOptions.Stroke, textBounds);
            }

            //Create the Anchorline's SkiaPath from the string Path
            string anchorLinePath = "";
            if (draw.TextAnchorLine is not null)
            {
                anchorLinePath = new LineDrawing(draw.TextAnchorLine).GetPathDataString();
            }
            var textPath = SKPath.ParseSvgPathData(anchorLinePath);

            //Calculate the rotation angle of the text and check weather the text is Horizontal or Vertical (for the double cases where it always is one of the two)
            var angleRad = draw.TextAnchorLine?.GetNonDirectionalAngleWithXAxis() ?? 0;
            var isAnchorHorizontal = draw.TextAnchorLine?.IsHorizontal ?? true;

            //Unlike WPF , SKIA places the Text Bottom BaseLine at the Point , So we need the inverse logic from WPF (which places it at the TopBaseLine)

            double textYOffset;
            switch (draw.PresentationOptions.TextAnchorLineOption)
            {
                case AnchorLinePreferenceOption.PreferGreaterXAnchorline: //Dimension Line should be on the Left
                case AnchorLinePreferenceOption.PreferSmallerYAnchorline: //Dimension Line should be on Bottom of Text
                case AnchorLinePreferenceOption.PreferGreaterXSmallerYAnchorline: //Dimension Line should be on Left OR on Bottom of Text                                                        
                    textYOffset = 0;
                    break;
                case AnchorLinePreferenceOption.PreferSmallerXAnchorline: //Dimension Line should be on Right of Text
                case AnchorLinePreferenceOption.PreferGreaterYAnchorline: //Dimension Line should be on Top
                case AnchorLinePreferenceOption.PreferSmallerXGreaterYAnchorline: //Dimension Line should be on Right OR on Top of Text
                    textYOffset = +textBounds.Height;
                    break;
                //In the below cases if the Anchor is Horizontal then discard the X and use the Y value for the Option
                case AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline: //Dimension Line should be on Right OR on Bottom of Text
                    textYOffset = isAnchorHorizontal ? 0 : +textBounds.Height;
                    break;
                case AnchorLinePreferenceOption.PreferGreaterXGreaterYAnchorline: //Dimension Line should be on Left OR on Top of Text
                    textYOffset = isAnchorHorizontal ? +textBounds.Height : 0;
                    break;
                default:
                    throw new EnumValueNotSupportedException(draw.PresentationOptions.TextAnchorLineOption);
            }

            //Find the text's position (apply offset to Y)
            float textX;
            float textY;
            if (draw is DimensionLineDrawing dim && dim.ShouldRenderTwoLineDimension(dim.Shape.GetTotalLength()) && !dim.LineOptions.CenterTextOnTwoLineDimension)
            {
                textX = (float)(draw.TextAnchorLine?.StartX + textWidth/2f ?? 0);
                textY = (float)(draw.TextAnchorLine?.StartY ?? 0);
            }
            else
            {
                textX = (float)(draw.TextAnchorLine?.GetLineMidPoint().X ?? 0);
                textY = (float)(draw.TextAnchorLine?.GetLineMidPoint().Y ?? 0);
            }

            if (angleRad != 0)
            {
                //Apply rotation around the textPosition
                canvas.Save();                                             //Save the current canvas state
                canvas.Translate(textX, textY);                            //Move the origin to the text position
                canvas.RotateRadians((float)angleRad);                     //Rotate the canvas
                canvas.Translate(0, (float)textYOffset);                //After rotating apply the offset so that it aligns with the dimension
                canvas.DrawText(draw.DrawingText, 0, 0, paint);       //Draw text at the rotated position (now origin is the midPoint of anchorLine)
                canvas.Restore();                                          //Restore the canvas state to prevent rotating other elements
            }
            else
            {
                canvas.DrawText(draw.DrawingText, textX, textY + (float)textYOffset, paint);   //Draw text at the rotated position (now origin is the midPoint of anchorLine)
            }
        }
        private static SKShader ConvertDrawBrushToLinearGradient(DrawBrush brushWithGradient, SKRect shapeBounds)
        {
            var gradientStops = brushWithGradient.GradientStops.Select(stop => (stop.Color, (float)stop.Offset)).ToArray();
            (PointXY normalizedStart, PointXY normalizedEnd) = DrawBrush.GetGradientAngleNormalizedPoints(brushWithGradient.GradientAngleDegrees);
            SKPoint skNormalizedStart = new((float)normalizedStart.X, (float)normalizedStart.Y);
            SKPoint skNormalizedEnd = new((float)normalizedEnd.X, (float)normalizedEnd.Y);
            return SkiaSharpHelpers.GetLinearGradientShader(shapeBounds, skNormalizedStart, skNormalizedEnd, gradientStops);
        }

        public class DrawPdfDocumentOptions
        {
            /// <summary>
            /// The Size of the Page , Default is A4
            /// <para><see cref="PageSizes"/> can be Used to get predefined Sizes</para>
            /// </summary>
            public PageSize PageSize { get; set; } = PageSizes.A4.Landscape();

            /// <summary>
            /// The Margin of the Page , Default is 20
            /// </summary>
            public float PageMargin { get; set; } = 20;

            /// <summary>
            /// The Size of the Font
            /// </summary>
            public float FontSize { get; set; } = 12;

            /// <summary>
            /// Weather to scale the Draw to Fit the Page
            /// </summary>
            public bool ScaleDrawToFit { get; set; } = true;

            public DrawInfoTableOptions TableInfo { get; set; } = new();
        }
        public class DrawInfoTableOptions
        {
            public class TableTitles
            {
                public string ResponsibleDep { get; set; } = "ResponsibleDepartment";
                public string TechnicalRef { get; set; } = "TechnicalReference";
                public string CreatedBy { get; set; } = "CreatedBy";
                public string ApprovedBy { get; set; } = "ApprovedBy";
                public string DocumentType { get; set; } = "DocumentType";
                public string DocumentStatus { get; set; } = "DocumentStatus";
                public string LegalOwner { get; set; } = "LegalOwner";
                public string DrawTitle { get; set; } = "TitleSupplementaryTitle";
                public string Revision { get; set; } = "Rev";
                public string Language { get; set; } = "Lang";
                public string Sheet { get; set; } = "Sheet";
                public string DateOfIssue { get; set; } = "DateOfIssue";
                public string Notes { get; set; } = "Notes";
                public string Code { get; set; } = "Code";
                public string InternalReference { get; set; } = "InternalReference";
            }

            public TableTitles Titles { get; set; } = new();

            public string LanguageTwoLetterISO { get; set; } = "GR";
            public string ResponsibleDepartment { get; set; } = "ProductionDepartment";
            public string TechnicalReference { get; set; } = "-";
            public string CreatedBy { get; set; } = "Bronze Application";
            public string ApprovedBy { get; set; } = "-";
            public string DocumentType { get; set; } = "Draw";
            public string DocumentStatus { get; set; } = "Draft";
            public string LegalOwner { get; set; } = "Bronze Art Ltd";
            public byte[] LogoImageByteArray { get; set; } = [];
            public string DrawItemTitle { get; set; } = "Mirror Glass";
            public string Revision { get; set; } = "0";
            public string DateOfIssue { get; set; } = DateTime.Now.Date.ToString("dd/MM/yy");
            public string Notes { get; set; } = "";
            public string InternalReferenceNotes { get; set; } = "";
            public string Code { get; set; } = "-";
        }
    }

    public static class DrawPDFExtensions
    {
        public static TextBlockDescriptor CellTextLabel(this IContainer c, string text)
        {
            return c.AlignLeft()
                    .Text($"{text}:")
                    .FontSize(8)
                    .Italic();
        }
        public static TextBlockDescriptor CellTextValue(this IContainer c, string text)
        {
            return c.AlignLeft()
                    .Text(text)
                    .FontSize(10);
        }
        public static IContainer StyleCellInnerColumn(this IContainer c)
        {
            return c.Padding(3);
        }
    }
}

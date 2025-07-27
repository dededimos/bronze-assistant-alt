using BronzeFactoryApplication.Views.ComponentsUC.Various;
using BronzeFactoryApplication.Views.HelperViews;
using HandyControl.Controls;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace BronzeFactoryApplication.Helpers.Other;

public static class WPFHelpers
{
    /// <summary>
    /// Prints a FrameworkElement(control) and Hides the Selected Elements
    /// </summary>
    /// <param name="control">The Control to Print</param>
    /// <param name="elementsToHide">The elements to Hide</param>
    public static void PrintFrameWorkElement(FrameworkElement control, params FrameworkElement[] elementsToHide)
    {
        Transform originalScale = control.LayoutTransform;

        //Hide any elements not needed for printing here
        foreach (var element in elementsToHide)
        {
            element.Visibility = Visibility.Collapsed;
        }

        try
        {
            PrintDialog dialog = new();
            //Predefine to A4 and Portrait
            dialog.PrintTicket.PageMediaSize = new(PageMediaSizeName.ISOA4);
            dialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            if (dialog.ShowDialog() is true)
            {
                //Get the Selected Printers Capabilities
                PrintCapabilities capabilities = dialog.PrintQueue.GetPrintCapabilities(dialog.PrintTicket);
                //Get the scale of the print writer to screen of WPF Visual

                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / control.ActualWidth, capabilities.PageImageableArea.ExtentHeight / control.ActualHeight);
                scale *= 0.95; //Clipping Correction

                //Transform the Visual to Scale
                control.LayoutTransform = new ScaleTransform(scale, scale);

                //Get the Size of the Printer Page
                Size size = new(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //Update the Layout of the Visual on the printer page size
                control.Measure(size);
                control.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), size));

                //Print the UC control Visual
                dialog.PrintVisual(control, $"BronzePrint-{DateTime.Now.Ticks}");
            }
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
        finally
        {
            //Restore the Visual to its Previous State
            control.LayoutTransform = originalScale;
            foreach (var element in elementsToHide)
            {
                element.Visibility = Visibility.Visible;
            }
        }
    }

    /// <summary>
    /// Prints all the Controls Correctly to the Printer - If Printed to a Pdf it prompts to save each page individually
    /// </summary>
    /// <param name="controls">The framework elements List</param>
    public static void PrintFrameWorkElements(IEnumerable<FrameworkElement> controls)
    {
        //Foreach Element Keep its original Layout Values to restore in the end
        IEnumerable<(FrameworkElement control, Transform layoutTransform)> originalScales = controls.Select(c => (c, c.LayoutTransform));

        try
        {

            PrintDialog dialog = new();
            if (dialog.ShowDialog() is true)
            {
                //Get the Selected Printers Capabilities
                PrintQueue printQueue = dialog.PrintQueue;
                PrintTicket printTicket = dialog.PrintTicket;
                PrintCapabilities capabilities = printQueue.GetPrintCapabilities(printTicket);

                //Get the Size of the Printer Page
                Size size = new(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                foreach (var control in controls)
                {
                    //Get the scale of the print to screen of WPF Visual
                    double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / control.ActualWidth, capabilities.PageImageableArea.ExtentHeight / control.ActualHeight);
                    //scale *= 0.9; //Clipping Correction

                    //Transform the Visual to Scale
                    control.LayoutTransform = new ScaleTransform(scale, scale);

                    //Update the Layout of the Visual on the printer page size
                    control.Measure(size);
                    control.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), size));
                }
                //Print after
                foreach (var control in controls)
                {
                    //Print the UC control Visual
                    dialog.PrintVisual(control, "BronzeApplicationPrintingVisual");
                }
            }
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
        finally
        {
            //Restore all the Layout Transforms to the controls
            foreach (var (control, layoutTransform) in originalScales)
            {
                control.LayoutTransform = layoutTransform;
            }
        }
    }

    /// <summary>
    /// Prints Correctly to PDF - But if sent to the Printer Does not Print any Photos for some reason...
    /// </summary>
    /// <param name="vms"></param>
    public static void PrintCabinBoms(IEnumerable<CabinBomViewModel> vms)
    {
        try
        {
            //Create a Dialog to get all Printer Capabilities Staff
            PrintDialog pd = new();

            //PrintServer localPrinterServer = new LocalPrintServer();
            //var printers = localPrinterServer.GetPrintQueues();
            //var defaultPrinter = printers.FirstOrDefault() ?? throw new Exception("There is no Printer...");
            //var ticket = defaultPrinter.DefaultPrintTicket;
            //ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

            //Set Default Paper to A4
            pd.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

            if (pd.ShowDialog() != true) return;

            PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);//pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);
                                                                                                //Get the Size of the Printer Page
            Size size = new(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            //Create first all the Elements before creating the Fixed Document
            List<CabinBomUC> boms = new();
            //Create the Pages
            foreach (var vm in vms)
            {

                //Create the Elements of the Page
                CabinBomUC bom = new()
                {
                    DataContext = vm,
                    Margin = new Thickness(0),
                    Width = 2100,
                    Height = 2970
                };
                bom.UpdateLayout();
                //Measure and arrange to gain Actual Height and Width
                bom.Measure(size);
                bom.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), size));
                //Get the scale of the print writer to screen of WPF Visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / bom.ActualWidth, capabilities.PageImageableArea.ExtentHeight / bom.ActualHeight);
                //scale *= 0.9; //Clipping Correction (Not Needed ?)

                //Transform the Visual to Scale
                bom.LayoutTransform = new ScaleTransform(scale, scale);
                bom.Measure(size);
                bom.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), size));
                boms.Add(bom);
            }


            //Create a Fixed Document (page size is bigger than imagable area)
            FixedDocument document = new();
            document.DocumentPaginator.PageSize = size;

            //Create the Pages
            foreach (var bom in boms)
            {
                //Create a Page
                FixedPage page = new()
                {
                    Width = document.DocumentPaginator.PageSize.Width,
                    Height = document.DocumentPaginator.PageSize.Height
                };
                page.Children.Add(bom);
                PageContent content = new();
                ((IAddChild)content).AddChild(page);
                document.Pages.Add(content);
            }
            pd.PrintDocument(document.DocumentPaginator, "BronzeFactoryAppPrintJob");
            MessageService.Info("Print Job Completed and Send to Printer", "Print Job Completed");
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
    }

    /// <summary>
    /// Converts a <see cref="SolidColorBrush"/> into a hexadecimal string "#...."
    /// </summary>
    /// <param name="brush">The Brush</param>
    /// <returns></returns>
    public static string SolidColorBrushToHexString(SolidColorBrush brush)
    {
        return ColorToHexString(brush.Color);
    }
    /// <summary>
    /// Converts a <see cref="Color"/> into a hexadecimal string "#...."
    /// </summary>
    /// <param name="color">The Color to Convert</param>
    /// <returns></returns>
    public static string ColorToHexString(Color color)
    {
        return color.ToString();
    }
    /// <summary>
    /// Converts a <see cref="LinearGradientBrush"/> into a List of Hexadecimal Colors and their Offsets
    /// </summary>
    /// <param name="brush">The Gradient Brush</param>
    /// <returns></returns>
    public static List<(string,double)> LinearGradientToColorStops(LinearGradientBrush brush)
    {
        List<(string, double)> stops = [];
        foreach (var stop in brush.GradientStops)
        {
            stops.Add((ColorToHexString(stop.Color), stop.Offset));
        }
        return stops;
    }
    /// <summary>
    /// Trys to convert a Hexadecimal String to a Color - If Fails returns Crimson
    /// </summary>
    /// <param name="hexadecimal"></param>
    /// <param name="shouldThrowOnFormatException">Weather to throw an Exception if the Format of the hexadecimal string is not correct <para>when false it returns <see cref="Brushes.Crimson"/></para></param>
    /// <returns></returns>
    public static Color ConvertHexadecimalStringToColor(string hexadecimal,bool shouldThrowOnFormatException = false) 
    {
        try
        {
            return (Color)ColorConverter.ConvertFromString(hexadecimal);
        }
        catch (FormatException ex)
        {
            if (shouldThrowOnFormatException) throw new FormatException(ex.Message, ex);
            else return Colors.Crimson;
        }
    }

    //UNUSED
    /// <summary>
    /// This Method Does not Print Any Images for some reason ... Other than that it Works good 
    /// </summary>
    /// <param name="elements"></param>
    public static void PrintPages(IEnumerable<FrameworkElement> elements)
    {
        //Create XPS Doc in TempFile
        string tempDirectory = Path.GetTempPath();
        string xpsFileName = Path.Combine(tempDirectory, "BronzeFactoryApp_print_preview.xps");
        if (File.Exists(xpsFileName)) File.Delete(xpsFileName);

        //Default Print Q
        LocalPrintServer printServer = new();
        PrintQueue printQueue = printServer.DefaultPrintQueue;
        PrintTicket printTicket = printQueue.DefaultPrintTicket;
        PrintCapabilities capabilities = printQueue.GetPrintCapabilities(printTicket);
        //Get the Size of the Printer Page
        Size size = new(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

        //CreateDoc
        using (XpsDocument doc = new(xpsFileName, FileAccess.ReadWrite))
        {
            XpsDocumentWriter? writer = XpsDocument.CreateXpsDocumentWriter(doc);
            SerializerWriterCollator? outputDocument = writer.CreateVisualsCollator();
            outputDocument.BeginBatchWrite();
            foreach (var element in elements)
            {
                //Get the scale of the print to screen of WPF Visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / element.ActualWidth, capabilities.PageImageableArea.ExtentHeight / element.ActualHeight);
                scale *= 0.9; //Clipping Correction
                              //Transform the Visual to Scale
                element.LayoutTransform = new ScaleTransform(scale, scale);

                //Update the Layout of the Visual on the printer page size
                element.Measure(size);
                element.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), size));

                outputDocument.Write(element);
            }
            outputDocument.EndBatchWrite();

            FixedDocumentSequence preview = doc.GetFixedDocumentSequence();
            doc.Close();
            var window = new System.Windows.Window
            {
                Content = new DocumentViewer { Document = preview }
            };
            window.ShowDialog();
            window.Content = null;
            window.Close();
        };

    }
    //UNUSED
    public static void PrintFrameworkElements(IEnumerable<FrameworkElement> elements)
    {
        PrintDialog printDialog = new();
        //Set Default paper to A4 
        printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

        if (printDialog.ShowDialog() == true)
        {
            double width = printDialog.PrintTicket.PageMediaSize.Width ?? 793.7; //Default to A4 width (in device independent units)
            double height = printDialog.PrintTicket.PageMediaSize.Height ?? 1122.5; //Default to A4 Height (in device independent units)
            Size pageSize = new(width, height);

            Size margin = new(48, 48); //0.5 inch margins (in device independent units)
            FrameworkElementPaginator paginator = new(elements.ToList(), pageSize, margin);
            printDialog.PrintDocument(paginator, "PrintJobBronzeFactoryApp");
        }
    }
}

public class FrameworkElementPaginator : DocumentPaginator
{
    private readonly IList<FrameworkElement> _elements;
    private Size _pageSize;
    private readonly Size _margin;

    public FrameworkElementPaginator(IList<FrameworkElement> elements, Size pageSize, Size margin)
    {
        _elements = elements;
        _pageSize = pageSize;
        _margin = margin;
        PageSize = pageSize;
    }

    public override bool IsPageCountValid => true;
    public override int PageCount => _elements.Count;
    public override Size PageSize { get => _pageSize; set => _pageSize = value; }
    public override IDocumentPaginatorSource Source => null!;

    public override DocumentPage GetPage(int pageNumber)
    {
        //Get the element for the requested page
        FrameworkElement element = _elements[pageNumber];
        //Position it
        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));

        //Find how much to scale the Element to fit the page
        double scaleX = element.ActualWidth != 0 ? (PageSize.Width - _margin.Width * 2) / element.ActualWidth : 0;
        double scaleY = element.ActualHeight != 0 ? (PageSize.Height - _margin.Height * 2) / element.ActualHeight : 0;
        double scale = Math.Min(scaleX, scaleY);

        //Find how much to offset the element to fit the page in the center!
        double offsetX = (PageSize.Width - element.ActualWidth * scale) / 2;
        double offsetY = (PageSize.Height - element.ActualHeight * scale) / 2;

        DrawingVisual visual = new();
        using (DrawingContext context = visual.RenderOpen())
        {
            context.PushTransform(new ScaleTransform(scale, scale, 0, 0));
            context.PushTransform(new TranslateTransform(offsetX, offsetY));
            context.DrawRectangle(new VisualBrush(element), null, new Rect(0, 0, element.ActualWidth, element.ActualHeight));
        }
        return new DocumentPage(visual, PageSize, new Rect(PageSize), new Rect(PageSize));
    }
}


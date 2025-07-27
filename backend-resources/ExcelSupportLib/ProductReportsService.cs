//using ClosedXML.Excel;
//using ClosedXML.Graphics;
//using DocumentFormat.OpenXml.Presentation;
using ClosedXML.Excel;
using ClosedXML.Graphics;
//using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelSupportLib
{
    public class ProductReportsService
    {
        ProductReportsService()
        {

            //Gets the Executing assembly (The compiled project actual file)
            var assembly = Assembly.GetExecutingAssembly();


            //Gets the names of the Embeded Resources (handy to detect full path of the embeded files)
            //var resources = assembly.GetManifestResourceNames();
            //foreach (var resource in resources) Console.WriteLine(resource);


            //Creates a stream of the font file requested , so it can be injected to the Graphics engine!
            using Stream fontStream = assembly.GetManifestResourceStream($"ExcelSupportLib.Fonts.calibri.ttf") 
                ?? throw new Exception("Reosurce not Found 'ExcelSupportLib.Fonts.calibri.ttf' ");

            //Loads the font and generates a new default Graphics Engine
            LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateOnlyWithFonts(fontStream);
        }

        /// <summary>
        /// Get an xls Cabin Report into as a byte array
        /// </summary>
        /// <returns></returns>
        public static byte[] GetCabinReport()
        {
            //using var wb = new XLWorkbook();
            //var ws = wb.Worksheets.Add($"{cabin.Code}");
            //ws.Cell("B3").Value = "test";
            //ws.Cell("B8").Value = "fasolakia me kima kai kima me fasolakia";
            //ws.ColumnsUsed().AdjustToContents();

            //using MemoryStream stream = new();
            //wb.SaveAs(stream);
            //stream.Position = 0;
            //return stream.ToArray();
            throw new NotImplementedException();
        }
    }
}

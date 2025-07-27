using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestPdfLibrary
{
    public static class PdfBomList
    {
        static PdfBomList()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
        public static void CreateBomList()
        {
            
        }
    }

    public class CustomPdfDocument : IDocument
    {
        private readonly byte[] img = [];
        public CustomPdfDocument(string photoUrl)
        {
            try
            {
                img = DownloadImageAsync(photoUrl).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task<byte[]> DownloadImageAsync(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetByteArrayAsync(url);
        }

        public void Compose(IDocumentContainer container)
        {
            container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.Margin(50);

                page.DefaultTextStyle(t=> t.FontSize(12).FontColor(Colors.Black).FontFamily(Fonts.Calibri));


                page.Header().Height(100).Background(Colors.White).Text("asijsaofsijoi");
                page.Content().AlignMiddle().PaddingHorizontal(200).AlignCenter().Image(img);
                
                page.Footer().Height(50).Background(Colors.Grey.Lighten4);
            });
        }
    }

    public class BomListComponent : IComponent
    {
        public void Compose(IContainer container)
        {
            
        }
    }

}



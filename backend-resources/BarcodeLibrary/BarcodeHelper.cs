using BarcodeStandard;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BarcodeLibrary
{
    public static class BarcodeHelper
    {
        public static byte[] GetBarcodeBytes(string barcode , bool includeLabel = true)
        {
            var b = new Barcode();
            b.IncludeLabel = includeLabel;
            var img = b.Encode(BarcodeStandard.Type.Code128, barcode,SKColors.Black,SKColors.White,400,200);

            using (var data = img.Encode(SKEncodedImageFormat.Png, 100))
            {
                return data.ToArray();
            }
        }
    }
}

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System.Text;

namespace QuestPdfLibrary
{
    public static class SkiaSharpHelpers
    {
        /// <summary>
        /// Pass a Draw Action with the Size of the Desired Canvas in order to create an SVG Element in the Container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="drawOnCanvas"></param>
        /// <param name="customSize">Custom size or null to Use container's size</param>
        public static void SkiaSharpCanvas(this IContainer container,Action<SKCanvas,Size> drawOnCanvas,Size? customSize = null)
        {
            //Create an svg Element on the container
            container.Svg(size =>
            {
                //If a custom size is passed , use it , otherwise use the size of the container
                var canvasSize = customSize ?? size;

                //Save the Canvas into a stream
                //The Action passed in the Method is the Draw that will be done on the canvas
                using var stream = new MemoryStream();
                using (var canvas = SKSvgCanvas.Create(new SKRect(0, 0, canvasSize.Width, canvasSize.Height), stream))
                    drawOnCanvas(canvas, canvasSize);
                //Gets the Data in Bytes from the Canvas Stream
                var svgData = stream.ToArray();
                //Returns the SVG Data as a string that can be read by Quest PDF to create the SVG
                return Encoding.UTF8.GetString(svgData);
            });
        }

        /// <summary>
        /// Similar to the SkiaCanvas Method , but it will create a Rasterized Image instead of an SVG
        /// </summary>
        /// <param name="container"></param>
        /// <param name="drawOnCanvas"></param>
        public static void SkiaSharpRasterized(this IContainer container, Action<SKCanvas, Size> drawOnCanvas)
        {
            container.Image(payload =>
            {
                using var bitmap = new SKBitmap(payload.ImageSize.Width, payload.ImageSize.Height);

                using (var canvas = new SKCanvas(bitmap))
                {
                    var scalingFactor = payload.Dpi / (float)DocumentSettings.DefaultRasterDpi;
                    canvas.Scale(scalingFactor);
                    drawOnCanvas(canvas, payload.AvailableSpace);
                }

                return bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
            });
        }

        /// <summary>
        /// Creates an SK color from a hexadecimal Value
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static SKColor SKColorFromHex(string hex)
        {
            // Check for empty string
            if (string.IsNullOrWhiteSpace(hex)) return SKColor.Empty; // Default to fully transparent

            // Remove the '#' character if it exists
            if (hex.StartsWith("#")) hex = hex.Substring(1);


            // Parse the hexadecimal string into an integer
            uint colorValue = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            // If the hex string is 6 characters long, assume full opacity
            if (hex.Length == 6)
            {
                return new SKColor((byte)((colorValue >> 16) & 0xFF), // Red
                                    (byte)((colorValue >> 8) & 0xFF),  // Green
                                    (byte)(colorValue & 0xFF));        // Blue
            }
            // If the hex string is 8 characters long, parse alpha as well
            else if (hex.Length == 8)
            {
                return new SKColor((byte)((colorValue >> 16) & 0xFF), // Red
                                    (byte)((colorValue >> 8) & 0xFF),  // Green
                                    (byte)(colorValue & 0xFF),        // Blue
                                    (byte)((colorValue >> 24) & 0xFF)); // Alpha
            }

            // Return a default color if the format is invalid
            throw new ArgumentException("Invalid hexadecimal color format. Use #RRGGBB or #AARRGGBB.");
        }

        /// <summary>
        /// Returns the <see cref="SKShader"/> with the Linear Gradient for the Selected Bounds
        /// </summary>
        /// <param name="shapeBounds">The Bound of the Shape the Gradient is being applied to</param>
        /// <param name="normalizedStartPoint">The Normalized Start Point min 0 max 1 (Like WPF Gradient Points)</param>
        /// <param name="normalizedEndPoint">The Normalized End Point min 0 max 1 (Like WPF Gradient Points)</param>
        /// <param name="gradientStops">The Gradient stops (color : #AARRGGBB,offset : 0 to 1)</param>
        /// <returns></returns>
        public static SKShader GetLinearGradientShader(SKRect shapeBounds,SKPoint normalizedStartPoint,SKPoint normalizedEndPoint,params (string color,float offset)[] gradientStops)
        {
            SKPoint skStartPoint = new(shapeBounds.Left + shapeBounds.Width * normalizedStartPoint.X,
                                       shapeBounds.Top + shapeBounds.Height * normalizedStartPoint.Y);
            SKPoint skEndPoint = new(shapeBounds.Left + shapeBounds.Width * normalizedEndPoint.X,
                                     shapeBounds.Top + shapeBounds.Height * normalizedEndPoint.Y);

            List<SKColor> colors = new();
            List<float> offsets = new();

            foreach (var stop in gradientStops)
            {
                SKColor color;
                offsets.Add(stop.offset);
                try
                {
                    color = SKColorFromHex(stop.color);
                }
                catch (ArgumentException)
                {

                    color = SKColors.Transparent;
                }
                colors.Add(color);
            }

            return SKShader.CreateLinearGradient(skStartPoint, skEndPoint, [.. colors], [.. offsets], SKShaderTileMode.Clamp);


        }

        /// <summary>
        /// Returns an SK Bitmap of a Dot Pattern . Its better to create it once and reuse the Bitmap from the Consuming Object 
        /// </summary>
        /// <param name="patternSize">The Size of the pattern (a 100pixels pattern with a circle of 25Radius will have 25pixels empty on either side)</param>
        /// <param name="dotRadius">The Radius of the Dot </param>
        /// <param name="dotColor">The Fill color of the Dot</param>
        /// <returns></returns>
        public static SKBitmap CreateDotPattern(int patternSize,int dotRadius, SKColor dotColor)
        {
            SKBitmap patternBitmap = new SKBitmap(patternSize, patternSize);
            using (var canvas = new SKCanvas(patternBitmap))
            {
                canvas.Clear(SKColors.Transparent);

                using var paint = new SKPaint
                {
                    Color = dotColor,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = false, // disable to not introduce blurriness into the pattern as the shapes are small
                };
                // Draw a single dot in the center of the pattern
                canvas.DrawCircle(patternSize / 2f, patternSize / 2f, dotRadius, paint);
            }

            return patternBitmap;
        }

        /// <summary>
        /// Returns an SK Bitmap of a Line Pattern
        /// </summary>
        /// <param name="patternSize">The Size of each tile bigger means less lines</param>
        /// <param name="lineAngleDegrees"></param>
        /// <param name="lineColor"></param>
        /// <param name="lineWidth"></param>
        /// <returns></returns>
        public static SKBitmap CreateHatchPatternBitmap(int patternSize, float lineAngleDegrees, SKColor lineColor, float lineWidth)
        {
            SKBitmap patternBitmap = new SKBitmap(patternSize, patternSize);
            using (var canvas = new SKCanvas(patternBitmap))
            {
                canvas.Clear(SKColors.Transparent);

                using (var paint = new SKPaint
                {
                    Color = lineColor,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = lineWidth,
                    IsAntialias = false, // disable to not introduce blurriness into the pattern as the shapes are small
                })
                {
                    // Calculate the diagonal positions for the lines
                    float diagonalLength = patternSize * (float)Math.Sqrt(2); // Hypotenuse of the square

                    // Calculate angle in radians
                    float angleInRadians = lineAngleDegrees * (float)(Math.PI / 180);

                    // Draw the diagonal line through the center of the tile
                    var startPoint = new SKPoint(-diagonalLength / 2, patternSize / 2);
                    var endPoint = new SKPoint(diagonalLength / 2, patternSize / 2);

                    // Apply rotation to the line by the specified angle
                    var rotatedStartPoint = RotatePoint(startPoint, patternSize / 2, patternSize / 2, angleInRadians);
                    var rotatedEndPoint = RotatePoint(endPoint, patternSize / 2, patternSize / 2, angleInRadians);

                    // Draw the line inside the bitmap tile
                    canvas.DrawLine(rotatedStartPoint, rotatedEndPoint, paint);
                }
            }

            return patternBitmap;
        }

        // Method to rotate a point around a center by a specified angle
        private static SKPoint RotatePoint(SKPoint point, float centerX, float centerY, float angle)
        {
            float dx = point.X - centerX;
            float dy = point.Y - centerY;

            float rotatedX = centerX + (dx * (float)Math.Cos(angle) - dy * (float)Math.Sin(angle));
            float rotatedY = centerY + (dx * (float)Math.Sin(angle) + dy * (float)Math.Cos(angle));

            return new SKPoint(rotatedX, rotatedY);
        }

        /// <summary>
        /// Fills a path with a Dot Pattern
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="shapePath"></param>
        /// <param name="dotColor"></param>
        /// <param name="dotSpacing"></param>
        /// <param name="dotRadius"></param>
        public static void FillShapeWithDotPattern(SKCanvas canvas, SKPath shapePath, SKColor dotColor, float dotSpacing, float dotRadius)
        {
            // Save the current state of the canvas
            canvas.Save();

            // Set the clipping path to the shape
            canvas.ClipPath(shapePath);

            // Get the bounding box of the shape to determine the area to fill
            var bounds = shapePath.Bounds;

            // Calculate the number of dots to draw horizontally and vertically
            int columns = (int)(bounds.Width / dotSpacing) + 1;
            int rows = (int)(bounds.Height / dotSpacing) + 1;

            using (var paint = new SKPaint
            {
                Color = dotColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true, // Enable anti-aliasing if desired
            })
            {
                for (int row = 0; row <= rows; row++)
                {
                    for (int col = 0; col <= columns; col++)
                    {
                        float x = bounds.Left + col * dotSpacing;
                        float y = bounds.Top + row * dotSpacing;

                        // Check if the point is inside the shape
                        if (shapePath.Contains(x, y))
                        {
                            canvas.DrawCircle(x, y, dotRadius, paint);
                        }
                    }
                }
            }

            // Restore the canvas state
            canvas.Restore();
        }

        /// <summary>
        /// Fills a path with a hatch line pattern at a specified angle.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="shapePath">The path of the shape to fill.</param>
        /// <param name="lineColor">The color of the hatch lines.</param>
        /// <param name="lineSpacing">The spacing between lines.</param>
        /// <param name="lineAngleDegrees">The angle of the hatch lines in degrees.</param>
        /// <param name="lineWidth">The width of the hatch lines.</param>
        public static void FillShapeWithHatchLinePattern(
            SKCanvas canvas,
            SKPath shapePath,
            SKColor lineColor,
            float lineSpacing,
            float lineAngleDegrees,
            float lineWidth)
        {
            // Save the current state of the canvas
            canvas.Save();

            // Set the clipping path to the shape
            canvas.ClipPath(shapePath);

            // Get the bounding box of the shape to determine the area to fill
            var bounds = shapePath.Bounds;

            // Calculate the length needed to cover the diagonal of the bounds
            float diagonalLength = (float)Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height) * 2;

            // Calculate angle in radians
            float angleInRadians = lineAngleDegrees * (float)(Math.PI / 180.0);

            // Calculate the sine and cosine of the angle
            float sinAngle = (float)Math.Sin(angleInRadians);
            float cosAngle = (float)Math.Cos(angleInRadians);

            // Calculate the normal (perpendicular) direction components
            float normalX = -sinAngle;
            float normalY = cosAngle;

            // Calculate the number of lines needed to cover the shape
            int numberOfLines = (int)(diagonalLength / lineSpacing) + 1;

            using (var paint = new SKPaint
            {
                Color = lineColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = lineWidth,
                IsAntialias = true,
            })
            {
                for (int i = -numberOfLines; i <= numberOfLines; i++)
                {
                    // Offset along the normal direction
                    float offsetX = i * lineSpacing * normalX;
                    float offsetY = i * lineSpacing * normalY;

                    // Calculate start and end points for the line
                    float startX = bounds.MidX - diagonalLength * cosAngle / 2 + offsetX;
                    float startY = bounds.MidY - diagonalLength * sinAngle / 2 + offsetY;

                    float endX = bounds.MidX + diagonalLength * cosAngle / 2 + offsetX;
                    float endY = bounds.MidY + diagonalLength * sinAngle / 2 + offsetY;

                    // Draw the line
                    canvas.DrawLine(startX, startY, endX, endY, paint);
                }
            }

            // Restore the canvas state
            canvas.Restore();
        }
    }

}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using TextureGenerator.Models;

namespace TextureGenerator.Framework
{
    
    static class ImageHelper
	{
        public static PixelColor[,] CopyPixels(this BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            PixelColor[,] pixels = new PixelColor[source.PixelWidth, source.PixelHeight];
            int stride = source.PixelWidth * ((source.Format.BitsPerPixel + 7) / 8);
            GCHandle pinnedPixels = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            source.CopyPixels(
              new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
              pinnedPixels.AddrOfPinnedObject(),
              pixels.GetLength(0) * pixels.GetLength(1) * 4,
                  stride);
            pinnedPixels.Free();
            return pixels;
        }
        public static void PutPixels(this WriteableBitmap bitmap, PixelColor[,] pixels, int x, int y)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, x, y);
        }
        public static Boolean IsEqualToPixelColor(this Color color, PixelColor pixelColor, bool includeAlpha = false)
        {
            return color.R == pixelColor.Red && color.G == pixelColor.Green && color.B == pixelColor.Blue && (includeAlpha ? color.A == pixelColor.Alpha : true);
        }
        public static bool IsEqualTo(this PixelColor pixelColor, PixelColor otherPixelColor)
        {
            return pixelColor.Red == otherPixelColor.Red &&
                pixelColor.Green == otherPixelColor.Green &&
                pixelColor.Blue == otherPixelColor.Blue &&
                pixelColor.Alpha == otherPixelColor.Alpha;
        }
        public static PixelColor ToPixelColor(this Color color)
        {
            return new PixelColor
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Alpha = color.A
            };
        }
        public static Color ToColor(this PixelColor pixelColor)
        {
            return new Color()
            {
                R = pixelColor.Red,
                G = pixelColor.Green,
                B = pixelColor.Blue,
                A = pixelColor.Alpha
            };
        }
        public static int GetDistance(this Color current, Color match, bool includeAlpha = false)
        {
            int alphaDifference = includeAlpha ? current.A - match.A : 0;
            int redDifference = current.R - match.R;
            int greenDifference = current.G - match.G;
            int blueDifference = current.B - match.B;

            return alphaDifference * alphaDifference + redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }
        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep,
        /// “on top of” <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        public static Color Blend(this Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            //byte a = (byte)((color.A * amount) + backColor.A * (1 - amount));
            byte a = (byte)(255);
            return Color.FromArgb(a, r, g, b);
        }
        public static PixelColor Blend(this PixelColor color, PixelColor backColor, double amount)
        {
            byte r = (byte)((color.Red * amount) + backColor.Red * (1 - amount));
            byte g = (byte)((color.Green * amount) + backColor.Green * (1 - amount));
            byte b = (byte)((color.Blue * amount) + backColor.Blue * (1 - amount));
            //byte a = (byte)((color.A * amount) + backColor.A * (1 - amount));
            byte a = (byte)(255);
            return Color.FromArgb(a, r, g, b).ToPixelColor();
        }
        public static Vector ToVector(this SiblingDirection siblingDirection)
        {
            switch(siblingDirection)
            {
                case SiblingDirection.Up:
                    return new Vector(0, 1);
                case SiblingDirection.Right:
                    return new Vector(1, 0);
                case SiblingDirection.Down:
                    return new Vector(0, -1);
                case SiblingDirection.Left:
                    return new Vector(-1, 0);
                case SiblingDirection.UpRight:
                    return new Vector(1, 1);
                case SiblingDirection.DownRight:
                    return new Vector(1, -1);
                case SiblingDirection.DownLeft:
                    return new Vector(-1, -1);
                case SiblingDirection.UpLeft:
                    return new Vector(-1, 1);
                default:
                    return new Vector(0, 0);
            }
        }
        public static Point ToDirection(this Point point, SiblingDirection direction)
        {
            return point + direction.ToVector();
        }
        public static bool IsWithinBounds<T>(this T[,] thing, Point coordinates)
        {
            return coordinates.X >= 0 && coordinates.Y >= 0 && coordinates.X < thing.GetLength(1) && coordinates.Y < thing.GetLength(0);
        }
        public static string ToHexString(this Color color)
        {
            return $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}{color.A.ToString("X2")}";
        }
        public static T2[,] Convert<T1, T2>(this T1[,] origin, Func<int, int, T1, T2> conversionMethod)
        {
            var yLength = origin.GetLength(0);
            var xLength = origin.GetLength(1);
            var convertedArray = new T2[yLength, xLength];
            for (int y = 0; y < yLength; ++y)
            {
                for (int x = 0; x < xLength; ++x)
                {
                    var t1 = origin[y, x];
                    var converted = conversionMethod(x, y, t1);
                    convertedArray[y, x] = converted;
                }
            }
            return convertedArray;
        }
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public static Point UVToXY(this SharpDX.Vector2 uv, int sourceWidth, int sourceHeight)
        {
            return new Point(uv.X * sourceWidth, uv.Y * sourceHeight);
        }
    }
}

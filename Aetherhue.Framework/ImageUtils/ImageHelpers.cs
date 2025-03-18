using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.RegularExpressions;

namespace Aetherhue.Framework.ImageUtils;

public static class ImageHelpers
{
    public static bool TryParseColor(out Color color, string value)
    {
        color = Color.Black;

        // Hex
        var hexRegex = new Regex(@"^#([0-9A-Fa-f]{6})([0-9A-Fa-f]{2})?$");
        if (hexRegex.IsMatch(value))
        {
            try
            {
                color = Color.Parse(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // RGB
        var rgbRegex = new Regex(@"^(\d{1,3}),(\d{1,3}),(\d{1,3})(?:,(\d{1,3}))?$");
        if (rgbRegex.IsMatch(value))
        {
            var match = rgbRegex.Match(value);
            int r = int.Parse(match.Groups[1].Value);
            int g = int.Parse(match.Groups[2].Value);
            int b = int.Parse(match.Groups[3].Value);
            int a = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 255;  

            if (r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255 && a >= 0 && a <= 255)
            {
                color = Color.FromRgba((byte)r, (byte)g, (byte)b, (byte)a);
                return true;
            }
        }

        return false;
    }

    public static bool ColorsMatch(Color color1, Color color2, bool bIgnoreAlpha)
    {
        var pixel1 = color1.ToPixel<Rgba32>();
        var pixel2 = color2.ToPixel<Rgba32>();
        return ColorsMatch(pixel1, pixel2, bIgnoreAlpha);
    }

    public static bool ColorsMatch(Rgba32 pixel1, Rgba32 pixel2, bool bIgnoreAlpha)
    {
        if (bIgnoreAlpha)
        {
            return pixel1.R == pixel2.R && pixel1.G == pixel2.G && pixel1.B == pixel2.B;
        }
        else
        {
            return pixel1.R == pixel2.R && pixel1.G == pixel2.G && pixel1.B == pixel2.B && pixel1.A == pixel2.A;
        }
    }

    public static bool ColorsMatch(Rgb24 pixel1, Rgb24 pixel2)
    {
        return pixel1.R == pixel2.R && pixel1.G == pixel2.G && pixel1.B == pixel2.B;
    }

    public static List<Rgba32> GetUniqueColors(Image<Rgba32> image, bool bIgnoreAlpha)
    {
        HashSet<Rgba32> uniqueColors = [];

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                uniqueColors.Add(new Rgba32(pixel.R, pixel.G, pixel.B, bIgnoreAlpha ? (byte)255 : pixel.A));
            }
        }

        return [.. uniqueColors];
    }

}

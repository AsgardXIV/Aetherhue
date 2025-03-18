using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Aetherhue.Framework.ImageUtils;

public static class IDMapToXIVIDMap
{
    public static Image<Rgba32> Execute(Image<Rgba32> image, int offset, bool bPack, Color backgroundColor)
    {
        var uniqueColors = ImageHelpers.GetUniqueColors(image, true);

        uniqueColors.Remove(backgroundColor);

        Image<Rgba32> xivIdMap = image.Clone();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                var withoutAlpha = new Rgb24(pixel.R, pixel.G, pixel.B);

                if (ImageHelpers.ColorsMatch(withoutAlpha, backgroundColor))
                {
                    xivIdMap[x, y] = backgroundColor;
                    continue;
                }

                int colorIndex = uniqueColors.IndexOf(pixel);

                int row = colorIndex + offset;
                if(row >= XIVUtils.XIVColorRowCount)
                {
                    throw new Exception("Too many unique colors for the selected row offset.");
                }

                int pair = bPack ? row / 2 : row;
                if(pair >= XIVUtils.XIVColorPairCount)
                {
                    throw new Exception("Too many unique colors for the selected row offset and packing option.");
                }

                int colorBand = pair * 17;
                int blend = bPack ? (row % 2 == 0 ? 0 : 255) : pair;

                xivIdMap[x, y] = new Rgba32((byte)colorBand, (byte)blend, (byte)0, (byte)255);

            }
        }

        return xivIdMap;
    }
}

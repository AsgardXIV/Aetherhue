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

                if (ImageHelpers.ColorsMatch((Color)pixel, backgroundColor, true))
                {
                    xivIdMap[x, y] = backgroundColor;
                    continue;
                }

                int colorIndex = uniqueColors.IndexOf(pixel);

                var (redChannel, greenChannel) = XIVUtils.CalculateXIVRowID(offset, colorIndex, bPack);

                xivIdMap[x, y] = new Rgba32((byte)redChannel, (byte)greenChannel, (byte)0, (byte)255);
            }
        }

        return xivIdMap;
    }
}

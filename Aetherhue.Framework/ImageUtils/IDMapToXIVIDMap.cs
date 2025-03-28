using Aetherhue.Framework.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Aetherhue.Framework.ImageUtils;

public static class IDMapToXIVIDMap
{
    public static (Image<Rgba32>, byte[]) Execute(Image<Rgba32> image, int offset, bool bPack, Color backgroundColor)
    {
        var penumbraColorsetBytes = ResourceUtils.GetBytes("penumbra_colorset.bin");
        using MemoryStream penumbraColorset = new();
        penumbraColorset.Write(penumbraColorsetBytes, 0, penumbraColorsetBytes.Length);
        penumbraColorset.Position = 0;

        var uniqueColors = ImageHelpers.GetUniqueColors(image, true);

        uniqueColors.Remove(backgroundColor);

        for(int i = 0; i < uniqueColors.Count; i++)
        {
            int realOffset = bPack ? i : i * 2;
            XIVUtils.SetColorRow(penumbraColorset, uniqueColors[i], realOffset + offset);
        }

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

        return (xivIdMap, penumbraColorset.ToArray());
    }
}

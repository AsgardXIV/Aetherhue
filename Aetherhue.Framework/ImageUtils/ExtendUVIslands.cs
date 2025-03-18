using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aetherhue.Framework.ImageUtils;

public static class ExtendUVIslands
{
    public static Image<Rgba32> Execute(Image<Rgba32> image, int margin, Color backgroundColor, bool ignoreAlpha)
    {
       
        HashSet<(int, int)> withBackgroundNeighbor = new();

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];

                bool bIsBackground = ImageHelpers.ColorsMatch((Color)pixel, backgroundColor, ignoreAlpha);

                if (!bIsBackground)
                {
                    bool bHasBackgroundNeighbor = HasBackgroundNeighbor(image, x, y, backgroundColor, ignoreAlpha);

                    if (bHasBackgroundNeighbor)
                    {
                        withBackgroundNeighbor.Add((x, y));
                    }
                }
            }
        }

        Image<Rgba32> extendedImage = image.Clone();

        foreach (var (x, y) in withBackgroundNeighbor)
        {
            ExtendUVIsland(extendedImage, x, y, margin, backgroundColor, ignoreAlpha);
        }

        return extendedImage;
    }

    private static void ExtendUVIsland(Image<Rgba32> image, int x, int y, int margin, Color backgroundColor, bool ignoreAlpha)
    {
        // We only extend in the 4 cardinal directions

        for (int dy = -margin; dy <= margin; dy++)
        {
            if (dy == 0) continue;
            int ny = y + dy;
            if (ny >= 0 && ny < image.Height)
            {
                var pixel = image[x, ny];
                bool bIsBackground = ImageHelpers.ColorsMatch((Color)pixel, backgroundColor, ignoreAlpha);
                if (bIsBackground)
                {
                    image[x, ny] = image[x, y];
                }
            }
        }

        for (int dx = -margin; dx <= margin; dx++)
        {
            if (dx == 0) continue;

            int nx = x + dx;
            if (nx >= 0 && nx < image.Width)
            {
                var pixel = image[nx, y];
                bool bIsBackground = ImageHelpers.ColorsMatch((Color)pixel, backgroundColor, ignoreAlpha);
                if (bIsBackground)
                {
                    image[nx, y] = image[x, y];
                }
            }
        }
    }

    private static bool HasBackgroundNeighbor(Image<Rgba32> image, int x, int y, Color backgroundColor, bool ignoreAlpha)
    {
        var offsets = new (int dx, int dy)[]
        {
            (-1, 0), // left
            (1, 0),  // right
            (0, -1), // top
            (0, 1)   // bottom
        };

        foreach (var (dx, dy) in offsets)
        {
            int nx = x + dx;
            int ny = y + dy;

            if (nx >= 0 && nx < image.Width && ny >= 0 && ny < image.Height)
            {
                var neighborPixel = image[nx, ny];
                if (ImageHelpers.ColorsMatch((Color)neighborPixel, backgroundColor, ignoreAlpha))
                {
                    return true;
                }
            }
        }

        return false;

    }
}

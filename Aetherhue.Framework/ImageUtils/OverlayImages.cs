
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Aetherhue.Framework.ImageUtils;

public static class OverlayImages
{

    public static Image<Rgba32> Execute(Image<Rgba32> baseImage, Image<Rgba32>[] overlays)
    {
        Image<Rgba32> newImage = baseImage.Clone();

        foreach (var overlay in overlays)
        {
            for (int y = 0; y < overlay.Height; y++)
            {
                for (int x = 0; x < overlay.Width; x++)
                {
                    var pixel = overlay[x, y];
                    if (pixel.A == 0)
                    {
                        continue;
                    }

                    if(pixel.A == 255)
                    {
                        newImage[x, y] = pixel;
                        continue;
                    }

                    var dstPixel = newImage[x, y];

                    float srcAlpha = pixel.A / 255.0f;
                    float dstAlpha = dstPixel.A / 255.0f;
                    float outAlpha = srcAlpha + dstAlpha * (1 - srcAlpha);

                    byte r = (byte)((pixel.R * srcAlpha + dstPixel.R * dstAlpha * (1 - srcAlpha)) / outAlpha);
                    byte g = (byte)((pixel.G * srcAlpha + dstPixel.G * dstAlpha * (1 - srcAlpha)) / outAlpha);
                    byte b = (byte)((pixel.B * srcAlpha + dstPixel.B * dstAlpha * (1 - srcAlpha)) / outAlpha);

                    var newPixel = new Rgba32
                    {
                        R = r,
                        G = g,
                        B = b,
                        A = (byte)(outAlpha * 255)
                    };

                    newImage[x, y] = newPixel;
                }
            }
        }

        return newImage;
    }
}

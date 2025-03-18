using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aetherhue.Framework.ImageUtils;

public static class SeparateDiffuseAndAlpha
{
    public static (Image<Rgba32> diffuse, Image<Rgba32> alpha) Execute(Image<Rgba32> image)
    {
        var diffuse = new Image<Rgba32>(image.Width, image.Height);
        var alpha = new Image<Rgba32>(image.Width, image.Height);

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                diffuse[x, y] = new(pixel.R, pixel.G, pixel.B, 255);
                alpha[x, y] = new(pixel.A, pixel.A, pixel.A, 255);
            }
        }

        return (diffuse, alpha);
    }
}

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using BCnEncoder.ImageSharp;
using BCnEncoder.Decoder;

namespace Aetherhue.Tools;

static class Utils
{
    public static Image<Rgba32> PathToImage(string path)
    {
        if(path.ToLower().EndsWith(".ktx") || path.ToLower().EndsWith(".dds"))
        {
            using FileStream fs = File.OpenRead(path);
            BcDecoder decoder = new BcDecoder();
            return decoder.DecodeToImageRgba32(fs);
        }

        if(path.ToLower().EndsWith(".tex"))
        {
            throw new Exception("TEX files are not supported yet");
        }

        var image = Image.Load<Rgba32>(path);
        return image;
    }

    public static string SanitizePath(string path)
    {
        return path.Trim().Trim('\"').Trim();
    }
}

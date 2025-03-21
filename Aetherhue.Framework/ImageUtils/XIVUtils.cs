using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Aetherhue.Framework.ImageUtils;
public static class XIVUtils
{
    public const int XIVColorRowCount = 32;
    public const int XIVColorPairCount = XIVColorRowCount / 2;


    public static (byte redChannel, byte greenChannel) CalculateXIVRowID(int rowOffset, int targetRow, bool bPackRows)
    {
        int row = rowOffset + targetRow;
        if (row >= XIVColorRowCount)
        {
            throw new Exception("Cannot calculate row ID for the selected row offset.");
        }

        int pair = bPackRows ? row / 2 : row;
        if (pair >= XIVColorPairCount)
        {
            throw new Exception("Cannot calculate row ID for the selected row offset and packing option.");
        }

        int colorBand = pair * 17;
        int blend = bPackRows ? (row % 2 == 0 ? 255 : 0) : pair;

        return((byte)colorBand, (byte)blend);
    }

    public static void SetColorRow(MemoryStream stream, Color color, int row)
    {
        var colorData = EncodeColorToFormat(color);

        BinaryWriter writer = new BinaryWriter(stream);

        writer.Seek((row * 0x40), SeekOrigin.Begin);

        writer.Write(colorData);

        writer.Flush();
    }

    private static byte[] EncodeColorToFormat(Color color)
    {
        static double Square(double x) => x < 0.00 ? -(x * x) : x * x; // Based on penumbra

        var extractedColor = color.ToPixel<Rgb24>();

        Half rHalf = (Half)Square(extractedColor.R / 255.0);
        Half gHalf = (Half)Square(extractedColor.G / 255.0);
        Half bHalf = (Half)Square(extractedColor.B / 255.0);

        return [.. BitConverter.GetBytes(rHalf), .. BitConverter.GetBytes(gHalf), .. BitConverter.GetBytes(bHalf)];
    }
}
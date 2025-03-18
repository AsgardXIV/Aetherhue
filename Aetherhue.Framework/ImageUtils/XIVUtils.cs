using SixLabors.ImageSharp.PixelFormats;

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
}

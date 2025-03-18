namespace Aetherhue.Framework.ImageUtils;
public static class XIVUtils
{
    public const int XIVColorRowCount = 32;
    public const int XIVColorPairCount = XIVColorRowCount / 2;

    public static int GetFreeRows(int rowOffset, int uniqueColors, bool bPackRows)
    {
        int totalRowCount = bPackRows ? XIVColorRowCount : XIVColorPairCount;
        int finalOffset = bPackRows ? rowOffset * 2 : rowOffset;
        int remainingRows = totalRowCount - finalOffset;
        int requiredRows = remainingRows - uniqueColors;
        return requiredRows;
    }
}

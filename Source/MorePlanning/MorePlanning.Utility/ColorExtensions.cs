using System.Globalization;
using UnityEngine;

namespace MorePlanning.Utility;

public static class ColorExtensions
{
    public static string ColorToHex(this Color c)
    {
        var num = (int)(Mathf.Clamp(c.r, 0f, 1f) * 255f);
        var num2 = (int)(Mathf.Clamp(c.g, 0f, 1f) * 255f);
        var num3 = (int)(Mathf.Clamp(c.b, 0f, 1f) * 255f);
        return $"{num:x2}{num2:x2}{num3:x2}";
    }

    public static Color HexToColor(this string cstr)
    {
        var black = Color.black;
        black.r = int.Parse(cstr[..2], NumberStyles.AllowHexSpecifier) / 255f;
        black.g = int.Parse(cstr.Substring(2, 2), NumberStyles.AllowHexSpecifier) / 255f;
        black.b = int.Parse(cstr.Substring(4, 2), NumberStyles.AllowHexSpecifier) / 255f;
        return black;
    }

    public static bool TryHexToColor(this string cstr, out Color color)
    {
        color = Color.black;
        cstr = cstr.TrimStart(['#']);
        if (!int.TryParse(cstr[..2], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var result))
        {
            return false;
        }

        if (!int.TryParse(
            cstr.Substring(2, 2),
            NumberStyles.AllowHexSpecifier,
            CultureInfo.InvariantCulture,
            out var result2))
        {
            return false;
        }

        if (!int.TryParse(
            cstr.Substring(4, 2),
            NumberStyles.AllowHexSpecifier,
            CultureInfo.InvariantCulture,
            out var result3))
        {
            return false;
        }

        color.r = result / 255f;
        color.g = result2 / 255f;
        color.b = result3 / 255f;
        return true;
    }
}
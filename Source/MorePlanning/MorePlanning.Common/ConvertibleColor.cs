using MorePlanning.Utility;
using UnityEngine;

namespace MorePlanning.Common;

public class ConvertibleColor
{
    private float _a;

    private float _b;

    private float _g;

    private float _h;
    private string _hexColor;

    private float _r;

    private float _s;

    private float _v;

    public ConvertibleColor() { ColorRGB = default; }

    public ConvertibleColor(Color c) { ColorRGB = c; }

    public Color HueOnly => Color.HSVToRGB(_h, 1f, 1f);

    public Color ColorRGB
    {
        get { return new(_r, _g, _b, _a); }
        private set
        {
            _r = value.r;
            _g = value.g;
            _b = value.b;
            _a = value.a;
            Color.RGBToHSV(ColorRGB, out _h, out _s, out _v);
            _hexColor = ColorRGB.ColorToHex();
        }
    }

    public string ColorHex
    {
        get { return _hexColor; }
        set
        {
            if(!value.TryHexToColor(out var color))
            {
                return;
            }

            color.a = _a;
            ColorRGB = color;
        }
    }

    public float H
    {
        get { return _h; }
        set
        {
            _h = value;
            var color = Color.HSVToRGB(_h, _s, _v);
            _r = color.r;
            _g = color.g;
            _b = color.b;
            _hexColor = ColorRGB.ColorToHex();
        }
    }

    public float S
    {
        get { return _s; }
        set
        {
            _s = value;
            var color = Color.HSVToRGB(_h, _s, _v);
            _r = color.r;
            _g = color.g;
            _b = color.b;
            _hexColor = ColorRGB.ColorToHex();
        }
    }

    public float V
    {
        get { return _v; }
        set
        {
            _v = value;
            var color = Color.HSVToRGB(_h, _s, _v);
            _r = color.r;
            _g = color.g;
            _b = color.b;
            _hexColor = ColorRGB.ColorToHex();
        }
    }
}
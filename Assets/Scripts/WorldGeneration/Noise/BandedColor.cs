using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandedColor : MonoBehaviour
{
    public Gradient BandedColorGradient;
    private static Gradient BandedColorGradient_private;
    public Gradient BandedColorGradient_a_1;
    private static Gradient BandedColorGradient_a_1_private;
    public Gradient BandedColorGradient_a_2;
    private static Gradient BandedColorGradient_a_2_private;
    public Gradient BandedColorGradient_a_3;
    private static Gradient BandedColorGradient_a_3_private;
    public Gradient BandedColorGradient_b_1;
    private static Gradient BandedColorGradient_b_1_private;
    public Gradient BandedColorGradient_b_2;
    private static Gradient BandedColorGradient_b_2_private;
    public Gradient BandedColorGradient_b_3;
    private static Gradient BandedColorGradient_b_3_private;
    public Gradient BandedColorGradient_c_1;
    private static Gradient BandedColorGradient_c_1_private;
    public Gradient BandedColorGradient_c_2;
    private static Gradient BandedColorGradient_c_2_private;
    public Gradient BandedColorGradient_c_3;
    private static Gradient BandedColorGradient_c_3_private;
    public Gradient BandedColorGradient_d_1;
    private static Gradient BandedColorGradient_d_1_private;
    public Gradient BandedColorGradient_d_2;
    private static Gradient BandedColorGradient_d_2_private;
    public Gradient BandedColorGradient_d_3;
    private static Gradient BandedColorGradient_d_3_private;
    public AnimationCurve BandedColorCurve;
    private static AnimationCurve BandedColorCurve_private;
    private static FastNoise BandedColorNoise;

    private void Awake()
    {
        BandedColorGradient_private = BandedColorGradient;
        BandedColorGradient_a_1_private = BandedColorGradient_a_1;
        BandedColorGradient_a_2_private = BandedColorGradient_a_2;
        BandedColorGradient_a_3_private = BandedColorGradient_a_3;
        BandedColorGradient_b_1_private = BandedColorGradient_b_1;
        BandedColorGradient_b_2_private = BandedColorGradient_b_2;
        BandedColorGradient_b_3_private = BandedColorGradient_b_3;
        BandedColorGradient_c_1_private = BandedColorGradient_c_1;
        BandedColorGradient_c_2_private = BandedColorGradient_c_2;
        BandedColorGradient_c_3_private = BandedColorGradient_c_3;
        BandedColorGradient_d_1_private = BandedColorGradient_d_1;
        BandedColorGradient_d_2_private = BandedColorGradient_d_2;
        BandedColorGradient_d_3_private = BandedColorGradient_d_3;
        BandedColorCurve_private = BandedColorCurve;
        BandedColorNoise = new FastNoise();
        BandedColorNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        BandedColorNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        BandedColorNoise.SetFrequency(0.01f);
        BandedColorNoise.SetFractalOctaves(4);
        BandedColorNoise.SetFractalLacunarity(2);
        BandedColorNoise.SetFractalGain(0.5f);
        BandedColorNoise.SetSeed(WorldSeed.seed);
    }

    public static Color GetColor(float height, float x, float y)
    {
        float scale = 1f;

        float multiplied = FirstRegion.GetNoise(x, y) * 10000f;
        float hueshift = multiplied - Mathf.RoundToInt(multiplied);

        float val = Mathf.PingPong((BandedColorNoise.GetNoise(x * scale, y * scale) * 0.2f) + (height * 5f), 3f);

        int banded_color_set = FirstRegion.Regions_value_next_level(x, y, 3, 100);

        Color col;
        if (banded_color_set == 0)
            col = val < 1f ? BandedColorGradient_a_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_a_2_private.Evaluate(val - 1f) : BandedColorGradient_a_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 1)
            col = val < 1f ? BandedColorGradient_b_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_b_2_private.Evaluate(val - 1f) : BandedColorGradient_b_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 2)
            col = val < 1f ? BandedColorGradient_c_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_c_2_private.Evaluate(val - 1f) : BandedColorGradient_c_3_private.Evaluate(val - 2f));
        else
            col = val < 1f ? BandedColorGradient_d_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_d_2_private.Evaluate(val - 1f) : BandedColorGradient_d_3_private.Evaluate(val - 2f));

        float h, s, v;
        Color.RGBToHSV(col, out h, out s, out v);
        float a = col.a;
        col = Color.HSVToRGB(Mathf.Repeat(h + hueshift, 1f), s, v);
        col.a = a;

        return col;
    }
}

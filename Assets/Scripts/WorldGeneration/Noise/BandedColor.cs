using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandedColor : MonoBehaviour
{
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
    public Gradient BandedColorGradient_e;
    private static Gradient BandedColorGradient_e_private;
    public Gradient BandedColorGradient_f;
    private static Gradient BandedColorGradient_f_private;
    public Gradient BandedColorGradient_g;
    private static Gradient BandedColorGradient_g_private;
    private static FastNoise BandedColorNoise;

    private void Awake()
    {
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
        BandedColorGradient_e_private = BandedColorGradient_e;
        BandedColorGradient_f_private = BandedColorGradient_f;
        BandedColorGradient_g_private = BandedColorGradient_g;
        BandedColorNoise = new FastNoise();
        BandedColorNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        BandedColorNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        BandedColorNoise.SetFrequency(0.01f);
        BandedColorNoise.SetFractalOctaves(4);
        BandedColorNoise.SetFractalLacunarity(2);
        BandedColorNoise.SetFractalGain(0.5f);
        BandedColorNoise.SetSeed(WorldSeed.seed);
    }

    public static Color GetColor(float height, float x, float y, bool first)
    {
        float scale = 1f;

        float multiplied;
        if (first)
            multiplied = FirstRegion.GetNoise(x, y) * 10000f;
        else
            multiplied = SecondRegion.GetNoise(x, y) * 10000f;
        float hueshift = multiplied - Mathf.RoundToInt(multiplied);

        float val = Mathf.PingPong((BandedColorNoise.GetNoise(x * scale, y * scale) * 0.2f) + (height * 1f), 3f);

        int banded_color_set;
        if (first)
            banded_color_set = FirstRegion.Regions_value_next_level(x, y, 6, 100);
        else
            banded_color_set = SecondRegion.Regions_value_next_level(x, y, 6, 100);

        Color col;
        if (banded_color_set == 0)
            col = val < 1f ? BandedColorGradient_a_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_a_2_private.Evaluate(val - 1f) : BandedColorGradient_a_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 1)
            col = val < 1f ? BandedColorGradient_b_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_b_2_private.Evaluate(val - 1f) : BandedColorGradient_b_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 2)
            col = val < 1f ? BandedColorGradient_c_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_c_2_private.Evaluate(val - 1f) : BandedColorGradient_c_3_private.Evaluate(val - 2f));
        else if(banded_color_set == 3)
            col = val < 1f ? BandedColorGradient_d_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_d_2_private.Evaluate(val - 1f) : BandedColorGradient_d_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 4)
            col = BandedColorGradient_e_private.Evaluate(val / 3f);
        else if (banded_color_set == 5)
            col = BandedColorGradient_f_private.Evaluate(val / 3f);
        else
            col = BandedColorGradient_g_private.Evaluate(val / 3f);

        float h, s, v;
        Color.RGBToHSV(col, out h, out s, out v);
        float a = col.a;
        col = Color.HSVToRGB(Mathf.Repeat(h + hueshift, 1f), s, v);
        col.a = a;

        return col;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepCanyonNoise : NoiseFunction
{
    public AnimationCurve CanyonCurve;
    private static AnimationCurve CanyonCurve_private;
    private static FastNoise noise;

    public AnimationCurve CanyonColorCurve;
    private static AnimationCurve CanyonColorCurve_private;

    public AnimationCurve CanyonColorCurve2;
    private static AnimationCurve CanyonColorCurve2_private;

    public override void Initialize()
    {
        CanyonCurve_private = CanyonCurve;
        CanyonColorCurve_private = CanyonColorCurve;
        CanyonColorCurve2_private = CanyonColorCurve2;
        noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise.SetFractalType(FastNoise.FractalType.Billow);
        noise.SetFrequency(0.01f);
        noise.SetFractalOctaves(6);
        noise.SetFractalLacunarity(1.9f);
        noise.SetFractalGain(0.5f);
        noise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale = 0.2f;
        float height = 1.5f;
        float canyon = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        canyon += 3f;
        canyon += 0.25f * FirstRegion.Regions_value_next_level(x, y, 10, 100f);

        return currentvalue - (CanyonCurve_private.Evaluate(smoothingvalue) * canyon);
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = FirstRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 0.5f;
        if (chosen_val < 10)
            alpha = 0.25f;
        if (chosen_val < 3)
            alpha = 0f;

        float scale = 0.2f;
        float height = 1.5f;
        height += 0.25f * FirstRegion.Regions_value_next_level(x, y, 6, 100f);
        float canyon = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        float total_canyon = canyon + 3f;
        total_canyon += 0.25f * FirstRegion.Regions_value_next_level(x, y, 10, 100f);

        float color_time = CanyonColorCurve_private.Evaluate(smoothingvalue + (CanyonColorCurve2_private.Evaluate(smoothingvalue) * (canyon / height)));

        Color toreturn = Color.Lerp(currentcolor, BandedColor.GetColor(total_canyon, x, y, true), color_time);
        toreturn.a = Mathf.Lerp(currentcolor.a, alpha, color_time);

        return toreturn;
    }
}

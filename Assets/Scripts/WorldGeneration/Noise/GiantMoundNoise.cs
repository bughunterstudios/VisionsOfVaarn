using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantMoundNoise : NoiseFunction
{
    public AnimationCurve MoundCurve;
    private static AnimationCurve MoundCurve_private;
    public AnimationCurve MoundNoiseCurve;
    private static AnimationCurve MoundNoiseCurve_private;
    private static FastNoise noise;

    public override void Initialize()
    {
        MoundCurve_private = MoundCurve;
        MoundNoiseCurve_private = MoundNoiseCurve;
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
        float scale = 0.4f;
        float height = 0.1f;
        float mound = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        mound = MoundNoiseCurve_private.Evaluate(smoothingvalue) * mound;
        mound += 2f;

        return Mathf.Lerp(currentvalue, (currentvalue * 0.25f) + mound, MoundCurve_private.Evaluate(smoothingvalue));
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = FirstRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 1f;
        if (chosen_val < 80)
            alpha = 0.75f;
        if (chosen_val < 40)
            alpha = 0.5f;
        if (chosen_val < 10)
            alpha = 0.25f;
        if (chosen_val < 3)
            alpha = 0f;

        float scale = 0.4f;
        float height = 0.1f;
        float mound = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        mound = MoundNoiseCurve_private.Evaluate(smoothingvalue) * mound;
        mound += 2f;

        Color toreturn = currentcolor;
        if (chosen_val < 40)
            toreturn = Color.Lerp(currentcolor, BandedColor.GetColor(mound, x, y, true), MoundCurve_private.Evaluate(smoothingvalue));
        toreturn.a = Mathf.Lerp(currentcolor.a, alpha, MoundCurve_private.Evaluate(smoothingvalue));

        return toreturn;
    }
}

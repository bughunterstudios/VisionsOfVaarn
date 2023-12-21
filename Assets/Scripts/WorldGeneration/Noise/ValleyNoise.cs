using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValleyNoise : NoiseFunction
{
    public AnimationCurve ValleyCurve;
    private static AnimationCurve ValleyCurve_private;
    public AnimationCurve ValleyNoiseCurve;
    private static AnimationCurve ValleyNoiseCurve_private;
    private static FastNoise noise;

    public override void Initialize()
    {
        ValleyCurve_private = ValleyCurve;
        ValleyNoiseCurve_private = ValleyNoiseCurve;
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
        float valley = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        valley = ValleyNoiseCurve_private.Evaluate(smoothingvalue) * valley;
        valley -= 1f;

        return Mathf.Lerp(currentvalue, (currentvalue * 0.25f) + valley, ValleyCurve_private.Evaluate(smoothingvalue));
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = FirstRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 1f;
        if (chosen_val < 50)
            alpha = 0.75f;

        float scale = 0.4f;
        float height = 0.1f;
        float valley = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        valley = ValleyNoiseCurve_private.Evaluate(smoothingvalue) * valley;
        valley -= 1f;

        Color toreturn = currentcolor;
        if (chosen_val < 50)
            toreturn = Color.Lerp(currentcolor, BandedColor.GetColor(valley, x, y, true), ValleyCurve_private.Evaluate(smoothingvalue));
        toreturn.a = Mathf.Lerp(currentcolor.a, alpha, ValleyCurve_private.Evaluate(smoothingvalue));

        return toreturn;
    }
}

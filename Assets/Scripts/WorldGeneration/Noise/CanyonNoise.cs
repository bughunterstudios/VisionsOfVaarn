using Unity.Mathematics;
using UnityEngine;

public class CanyonNoise : NoiseFunction
{
    public AnimationCurve CanyonCurve;
    private static AnimationCurve CanyonCurve_private;
    private static FastNoise Noise;

    public override void Initialize()
    {
        CanyonCurve_private = CanyonCurve;
        Noise = new FastNoise();
        Noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        Noise.SetFractalType(FastNoise.FractalType.Billow);
        Noise.SetFrequency(0.01f);
        Noise.SetFractalOctaves(6);
        Noise.SetFractalLacunarity(1.9f);
        Noise.SetFractalGain(0.5f);
        Noise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale = 0.2f;
        float height = 1.5f;
        float canyon = (Mathf.Min(-Noise.GetNoise(x * scale, y * scale) - 0.5f, 0f)) * height;

        return currentvalue + CanyonCurve_private.Evaluate(smoothingvalue) * canyon;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = SecondRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 0.5f;
        if (chosen_val < 20)
            alpha = 0.25f;
        if (chosen_val < 5)
            alpha = 0f;

        float scale = 0.2f;
        float height = 1.5f;
        float canyon = (Mathf.Min(-Noise.GetNoise(x * scale, y * scale) - 0.5f, 0f)) * height;

        Color toreturn = NoiseControl.ColorLerp(currentcolor, BandedColor.GetColor(canyon, x, y), smoothingvalue);
        toreturn.a = NoiseControl.FloatLerp(currentcolor.a, alpha, smoothingvalue);

        return toreturn;
    }
}

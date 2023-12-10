using UnityEngine;
using UnityEngine.UIElements;

public class MountainNoise : NoiseFunction
{
    public AnimationCurve MountainCurve;
    private static AnimationCurve MountainCurve_private;
    private static FastNoise noise;

    public override void Initialize()
    {
        MountainCurve_private = MountainCurve;
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
        float mountain = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;

        return currentvalue + MountainCurve_private.Evaluate(smoothingvalue) * mountain;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = SecondRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 0.5f;
        if (chosen_val < 20)
            alpha = 0.25f;
        if (chosen_val < 5)
            alpha = 0f;
        alpha = 0f;

        float scale = 0.2f;
        float height = 1.5f;
        float mountain = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;

        Color toreturn = NoiseControl.ColorLerp(currentcolor, BandedColor.GetColor(mountain, x, y), smoothingvalue);
        toreturn.a = NoiseControl.FloatLerp(currentcolor.a, alpha, smoothingvalue);

        return toreturn;
    }
}

using Unity.Mathematics;
using UnityEngine;

public class DryLakeNoise : NoiseFunction
{
    public AnimationCurve LakeCurve;
    private static AnimationCurve LakeCurve_private;
    private static FastNoise LakeNoise;

    public override void Initialize()
    {
        LakeCurve_private = LakeCurve;
        LakeNoise = new FastNoise();
        LakeNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        LakeNoise.SetFractalType(FastNoise.FractalType.Billow);
        LakeNoise.SetFrequency(0.01f);
        LakeNoise.SetFractalOctaves(4);
        LakeNoise.SetFractalLacunarity(1.5f);
        LakeNoise.SetFractalGain(0.5f);
        LakeNoise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale = 1f;
        float height = .2f;
        float lake = (Mathf.Min(-LakeNoise.GetNoise(x * scale, y * scale) - 0.5f, 0f)) * height;

        return currentvalue + (lake * LakeCurve_private.Evaluate(smoothingvalue));
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        int chosen_val = SecondRegion.Regions_value_next_level(x, y, 100, 100);

        Color toreturn = currentcolor;

        if (chosen_val < 20)
            toreturn.a = NoiseControl.FloatLerp(currentcolor.a, 0.75f, smoothingvalue);

        return toreturn;
    }
}

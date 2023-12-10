using UnityEngine;

public class RippleNoise : NoiseFunction
{
    public AnimationCurve RippleCurve;
    private static AnimationCurve RippleCurve_private;
    private static FastNoise noise;

    public override void Initialize()
    {
        RippleCurve_private = RippleCurve;
        noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        noise.SetFrequency(0.01f);
        noise.SetFractalOctaves(1);
        noise.SetFractalLacunarity(2f);
        noise.SetFractalGain(0.5f);
        noise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale1 = 1f;
        float scale2 = 4f;
        float height = 0.002f;
        float rip = Mathf.Sin((y * scale1) + (noise.GetNoise(x * scale2, y * scale2) * 10)) * height;

        return currentvalue + RippleCurve_private.Evaluate(smoothingvalue) * rip;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        return currentcolor;
    }
}

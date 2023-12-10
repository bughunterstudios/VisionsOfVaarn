using UnityEngine;

public class BulbousNoise : NoiseFunction
{
    public AnimationCurve ExtraDuneCurve;
    private static AnimationCurve ExtraDuneCurve_private;

    private static FastNoise SmallDunesNoise;

    public override void Initialize()
    {
        ExtraDuneCurve_private = ExtraDuneCurve;

        SmallDunesNoise = new FastNoise();
        SmallDunesNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        SmallDunesNoise.SetFractalType(FastNoise.FractalType.Billow);
        SmallDunesNoise.SetFrequency(0.01f);
        SmallDunesNoise.SetFractalOctaves(4);
        SmallDunesNoise.SetFractalLacunarity(2);
        SmallDunesNoise.SetFractalGain(0.5f);
        SmallDunesNoise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale = 1f;
        float height = 0.05f;
        float tinydunes = (SmallDunesNoise.GetNoise(x * scale, y * scale) - 0.5f) * -height;

        return currentvalue + ExtraDuneCurve_private.Evaluate(smoothingvalue) * tinydunes * -2f;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        return currentcolor;
    }
}

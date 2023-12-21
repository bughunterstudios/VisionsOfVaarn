using UnityEngine;
using UnityEngine.UIElements;

public class MountainNoise : NoiseFunction
{
    public AnimationCurve MountainCurve;
    private static AnimationCurve MountainCurve_private;
    private static FastNoise noise;

    public AnimationCurve MountainColorCurve;
    private static AnimationCurve MountainColorCurve_private;

    public AnimationCurve MountainColorCurve2;
    private static AnimationCurve MountainColorCurve2_private;

    public override void Initialize()
    {
        MountainCurve_private = MountainCurve;
        MountainColorCurve_private = MountainColorCurve;
        MountainColorCurve2_private = MountainColorCurve2;
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
        mountain += 3f;
        mountain += 0.25f * FirstRegion.Regions_value_next_level(x, y, 10, 100f);

        return currentvalue + (MountainCurve_private.Evaluate(smoothingvalue) * mountain);
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
        float height = 1f;
        height += 0.25f * FirstRegion.Regions_value_next_level(x, y, 6, 100f);
        float mountain = (Mathf.Min(noise.GetNoise(x * scale, y * scale), 0f)) * -height;
        float total_mountain = mountain + 3f;
        total_mountain += 0.25f * FirstRegion.Regions_value_next_level(x, y, 10, 100f);

        float color_time = MountainColorCurve_private.Evaluate(smoothingvalue + (MountainColorCurve2_private.Evaluate(smoothingvalue) * (mountain / height)));

        Color toreturn = Color.Lerp(currentcolor, BandedColor.GetColor(total_mountain, x, y, true), color_time);
        toreturn.a = Mathf.Lerp(currentcolor.a, alpha, color_time);

        return toreturn;
    }
}

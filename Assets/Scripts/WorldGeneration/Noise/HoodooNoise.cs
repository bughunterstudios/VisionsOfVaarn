using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HoodooNoise : NoiseFunction
{
    public AnimationCurve HoodooCurve;
    private static AnimationCurve HoodooCurve_private;
    public AnimationCurve TotalCurve;
    private static AnimationCurve TotalCurve_private;
    public AnimationCurve HoodooColorCurve;
    private static AnimationCurve HoodooColorCurve_private;
    public AnimationCurve HoodooColorEdgeCurve;
    private static AnimationCurve HoodooColorEdgeCurve_private;
    private static FastNoise cell_noise;
    private static FastNoise cell_noise_value;
    private static FastNoise noise;

    public override void Initialize()
    {
        HoodooCurve_private = HoodooCurve;
        TotalCurve_private = TotalCurve;
        HoodooColorCurve_private = HoodooColorCurve;
        HoodooColorEdgeCurve_private = HoodooColorEdgeCurve;

        cell_noise = new FastNoise();
        cell_noise.SetNoiseType(FastNoise.NoiseType.Cellular);
        cell_noise.SetFrequency(0.01f);
        cell_noise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        cell_noise.SetCellularReturnType(FastNoise.CellularReturnType.Distance);
        cell_noise.SetSeed(WorldSeed.seed);

        cell_noise_value = new FastNoise();
        cell_noise_value.SetNoiseType(FastNoise.NoiseType.Cellular);
        cell_noise_value.SetFrequency(0.01f);
        cell_noise_value.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        cell_noise_value.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);
        cell_noise_value.SetSeed(WorldSeed.seed);

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
        float scale_multiplier = 0.5f + (0.25f * SecondRegion.Regions_value_next_level(x, y, 10, 100));

        float scale1 = 1f * scale_multiplier;
        float height = 0.2f;
        float skrunkle = Mathf.Min(noise.GetNoise(x * scale1, y * scale1), 0) * -height;

        float scale2 = 3f * scale_multiplier;
        float cellval = 1f-cell_noise.GetNoise(x * scale2, y * scale2);

        float hoodooheight = 0.025f;
        hoodooheight += 0.025f * SecondRegion.Regions_value_next_level(x, y, 10, 200);
        hoodooheight *= (cell_noise_value.GetNoise(x * scale2, y * scale2) + 1f);
        float hoodoo = HoodooCurve_private.Evaluate(cellval - skrunkle) * (hoodooheight + skrunkle);

        return currentvalue + (TotalCurve_private.Evaluate(smoothingvalue) * hoodoo);
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        float scale_multiplier = 0.5f + (0.25f * SecondRegion.Regions_value_next_level(x, y, 10, 100));

        int chosen_val = SecondRegion.Regions_value_next_level(x, y, 100, 100);
        float alpha = 0.5f;
        if (chosen_val < 20)
            alpha = 0.25f;
        if (chosen_val < 5)
            alpha = 0f;

        float scale1 = 3f * scale_multiplier;
        float cellval = 1f-cell_noise.GetNoise(x * scale1, y * scale1);

        float hoodoo = HoodooColorCurve_private.Evaluate(cellval);

        float scale2 = 1f * scale_multiplier;
        float height = 2f;
        float skrunkle = Mathf.Min(noise.GetNoise(x * scale2, y * scale2), 0) * -height;

        hoodoo += skrunkle;

        hoodoo *= HoodooColorEdgeCurve_private.Evaluate(smoothingvalue);

        Color toreturn = Color.Lerp(currentcolor, BandedColor.GetColor(HoodooCurve_private.Evaluate(cellval), x, y, false), hoodoo);
        toreturn.a = Mathf.Lerp(currentcolor.a, alpha, hoodoo);

        return toreturn;
    }
}

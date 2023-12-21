using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallMesaNoise : NoiseFunction
{
    public AnimationCurve MesaCurve;
    private static AnimationCurve MesaCurve_private;
    public AnimationCurve CutoutCurve;
    private static AnimationCurve CutoutCurve_private;
    public AnimationCurve FingerCurve;
    private static AnimationCurve FingerCurve_private;
    public AnimationCurve ColorCurve;
    private static AnimationCurve ColorCurve_private;
    private static FastNoise mesa_fingerling_noise;
    private static FastNoise mesa_edge_noise;

    private static float min, max;

    public override void Initialize()
    {
        MesaCurve_private = MesaCurve;
        CutoutCurve_private = CutoutCurve;
        FingerCurve_private = FingerCurve;
        ColorCurve_private = ColorCurve;
        mesa_fingerling_noise = new FastNoise();
        mesa_fingerling_noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        mesa_fingerling_noise.SetFractalType(FastNoise.FractalType.RigidMulti);
        mesa_fingerling_noise.SetFrequency(0.01f);
        mesa_fingerling_noise.SetFractalOctaves(2);
        mesa_fingerling_noise.SetFractalLacunarity(2f);
        mesa_fingerling_noise.SetFractalGain(0.5f);
        mesa_fingerling_noise.SetSeed(WorldSeed.seed);
        mesa_edge_noise = new FastNoise();
        mesa_edge_noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        mesa_edge_noise.SetFractalType(FastNoise.FractalType.Billow);
        mesa_edge_noise.SetFrequency(0.02f);
        mesa_edge_noise.SetFractalOctaves(6);
        mesa_edge_noise.SetFractalLacunarity(1.9f);
        mesa_edge_noise.SetFractalGain(0.4f);
        mesa_edge_noise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float finger_scale = 0.1f;
        float finger = FingerCurve_private.Evaluate(mesa_fingerling_noise.GetNoise(x * finger_scale, y * finger_scale));

        float edge_scale = 0.15f;
        float edge = (mesa_edge_noise.GetNoise(x * edge_scale, y * edge_scale) + 1f) / 2f;
        edge *= CutoutCurve_private.Evaluate(smoothingvalue);
        edge *= 0.1f;

        float mesa_height = 0.5f;
        mesa_height += SecondRegion.Regions_value_next_level(x, y, 4, 1000f) * 0.2f;

        float mesatop = (currentvalue * 0.1f) + mesa_height;
        float val = Mathf.Lerp(currentvalue, mesatop, MesaCurve_private.Evaluate(smoothingvalue - edge) - finger / 2);

        return val;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        float finger_scale = 0.1f;
        float finger = FingerCurve_private.Evaluate(mesa_fingerling_noise.GetNoise(x * finger_scale, y * finger_scale));

        float edge_scale = 0.15f;
        float edge = (mesa_edge_noise.GetNoise(x * edge_scale, y * edge_scale) + 1f) / 2f;
        edge *= CutoutCurve_private.Evaluate(smoothingvalue);
        edge *= 0.1f;

        float val = (smoothingvalue - edge) - finger / 2;

        Color newcolor = Color.Lerp(currentcolor, BandedColor.GetColor(val, x, y, false), ColorCurve_private.Evaluate(val));
        newcolor.a = Mathf.Lerp(newcolor.a, 0.5f, ColorCurve_private.Evaluate(val));

        return newcolor;
    }
}

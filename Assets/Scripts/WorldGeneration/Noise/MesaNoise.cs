using UnityEngine;

public class MesaNoise : NoiseFunction
{
    public AnimationCurve MesaCurve;
    private static AnimationCurve MesaCurve_private;
    public AnimationCurve MesaShapeCurve;
    private static AnimationCurve MesaShapeCurve_private;
    private static FastNoise mesa;
    public AnimationCurve MesaHeightCurve;
    private static AnimationCurve MesaHeightCurve_private;
    private static FastNoise MesaHeightNoise;

    public override void Initialize()
    {
        MesaCurve_private = MesaCurve;
        MesaShapeCurve_private = MesaShapeCurve;
        MesaHeightCurve_private = MesaHeightCurve;
        mesa = new FastNoise();
        mesa.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        mesa.SetFractalType(FastNoise.FractalType.RigidMulti);
        mesa.SetFrequency(0.01f);
        mesa.SetFractalOctaves(4);
        mesa.SetFractalLacunarity(2f);
        mesa.SetFractalGain(0.5f);
        mesa.SetSeed(WorldSeed.seed);
        MesaHeightNoise = new FastNoise();
        MesaHeightNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        MesaHeightNoise.SetFractalType(FastNoise.FractalType.FBM);
        MesaHeightNoise.SetFrequency(0.01f);
        MesaHeightNoise.SetFractalOctaves(5);
        MesaHeightNoise.SetFractalLacunarity(0.7f);
        MesaHeightNoise.SetFractalGain(1.7f);
        MesaHeightNoise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale_height = 2f;
        float scale = 0.7f;
        float height = MesaHeightCurve_private.Evaluate(MesaHeightNoise.GetNoise(x * scale_height, y * scale_height));//.25f;
        float mesaval = MesaShapeCurve_private.Evaluate(-mesa.GetNoise(x * scale, y * scale) + 0.5f) * height;

        return Mathf.Lerp(currentvalue, (currentvalue * 0.4f) + mesaval, MesaCurve_private.Evaluate(smoothingvalue));
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        return currentcolor;
    }
}

using UnityEngine;

public class RiverNoise : NoiseFunction
{
    public AnimationCurve RiverCurve;
    private static AnimationCurve RiverCurve_private;
    public AnimationCurve RiverShapeCurve;
    private static AnimationCurve RiverShapeCurve_private;
    private static FastNoise noise;

    public override void Initialize()
    {
        RiverCurve_private = RiverCurve;
        RiverShapeCurve_private = RiverShapeCurve;
        noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise.SetFractalType(FastNoise.FractalType.RigidMulti);
        noise.SetFrequency(0.01f);
        noise.SetFractalOctaves(1);
        noise.SetFractalLacunarity(1.5f);
        noise.SetFractalGain(0.5f);
        noise.SetSeed(WorldSeed.seed);
    }

    public override float GetValue(float currentvalue, float smoothingvalue, float x, float y)
    {
        float scale = 0.5f;
        float height = .1f;
        float river = RiverShapeCurve_private.Evaluate(noise.GetNoise(x * scale, y * scale)) * -height;

        return currentvalue + (river * RiverCurve_private.Evaluate(smoothingvalue));
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

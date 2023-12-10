using UnityEngine;

public class DunesNoise : NoiseFunction
{
    private static FastNoise FlattenDunesNoise;
    private static FastNoise GiantDunesNoise;
    private static FastNoise LargeDunesNoise;
    private static FastNoise SmallDunesNoise;

    public override void Initialize()
    {
        FlattenDunesNoise = new FastNoise();
        FlattenDunesNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        FlattenDunesNoise.SetFractalType(FastNoise.FractalType.Billow);
        FlattenDunesNoise.SetFrequency(0.01f);
        FlattenDunesNoise.SetFractalOctaves(2);
        FlattenDunesNoise.SetFractalLacunarity(2);
        FlattenDunesNoise.SetFractalGain(0.5f);
        FlattenDunesNoise.SetSeed(WorldSeed.seed);

        GiantDunesNoise = new FastNoise();
        GiantDunesNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        GiantDunesNoise.SetFrequency(0.01f);
        GiantDunesNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        GiantDunesNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Add);
        GiantDunesNoise.SetSeed(WorldSeed.seed);

        LargeDunesNoise = new FastNoise();
        LargeDunesNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        LargeDunesNoise.SetFrequency(0.01f);
        LargeDunesNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        LargeDunesNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Add);
        LargeDunesNoise.SetSeed(WorldSeed.seed);

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
        float noise = GiantDunes(x, y);
        noise += LargeDunes(x, y);
        noise += TinyDunes(x, y);
        noise *= FlattenDunes(x, y);

        return noise;
    }

    public static float FlattenDunes(float x, float y)
    {
        float scale = .1f;
        return (FlattenDunesNoise.GetNoise(x * scale, y * scale) + 1f) * 0.5f;
    }

    public static float GiantDunes(float x, float y)
    {
        float scale = .1f;
        float height = .5f;
        return (GiantDunesNoise.GetNoise(x * scale, y * scale) - 0.5f) * height;
    }

    public static float LargeDunes(float x, float y)
    {
        float scale = 1f;
        float height = .15f;
        return (LargeDunesNoise.GetNoise(x * scale, y * scale) - 0.5f) * height;
    }

    public static float TinyDunes(float x, float y)
    {
        float scale = 1f;
        float height = 0.05f;
        return (SmallDunesNoise.GetNoise(x * scale, y * scale) - 0.5f) * -height;
    }

    public override Color Coloring(Color currentcolor, float smoothingvalue, float x, float y)
    {
        return currentcolor;
    }
}

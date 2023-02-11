using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseControl : MonoBehaviour
{
    private static FastNoise RegionsNoise;
    private static FastNoise GiantDunesNoise;
    private static FastNoise LargeDunesNoise;
    private static FastNoise SmallDunesNoiseNoise;

    private void Awake()
    {
        RegionsNoise = new FastNoise();
        RegionsNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        RegionsNoise.SetFractalType(FastNoise.FractalType.Billow);
        RegionsNoise.SetFrequency(0.01f);
        RegionsNoise.SetFractalOctaves(2);
        RegionsNoise.SetFractalLacunarity(2);
        RegionsNoise.SetFractalGain(0.5f);

        GiantDunesNoise = new FastNoise();
        GiantDunesNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        GiantDunesNoise.SetFrequency(0.01f);
        GiantDunesNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        GiantDunesNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Add);

        LargeDunesNoise = new FastNoise();
        LargeDunesNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        LargeDunesNoise.SetFrequency(0.01f);
        LargeDunesNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        LargeDunesNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Add);

        SmallDunesNoiseNoise = new FastNoise();
        SmallDunesNoiseNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        SmallDunesNoiseNoise.SetFractalType(FastNoise.FractalType.Billow);
        SmallDunesNoiseNoise.SetFrequency(0.01f);
        SmallDunesNoiseNoise.SetFractalOctaves(4);
        SmallDunesNoiseNoise.SetFractalLacunarity(2);
        SmallDunesNoiseNoise.SetFractalGain(0.5f);
    }

    public static float NoiseMapHeight(float x, float y)
    {
        float noise = GiantDunes(x, y);
        noise += LargeDunes(x, y);
        noise += TinyDunes(x, y);
        noise *= Regions(x, y);
        return noise;

        //return (Mathf.PerlinNoise(x * scale, y * scale) - 0.5f) * height;
    }

    public static float Regions(float x, float y)
    {
        float scale = .1f;
        return (RegionsNoise.GetNoise(x * scale, y * scale) + 1f) * 0.5f;
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
        return (SmallDunesNoiseNoise.GetNoise(x * scale, y * scale) - 0.5f) * height;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondRegion : MonoBehaviour
{
    public float scale;
    private static float private_scale;

    public RegionType[] regionTypes;
    private static RegionType[] privateRegionTypes;
    private static int Max_Weight;

    private static FastNoise RegionsNoise_value;
    private static FastNoise RegionsNoise_smooth;
    private static FastNoise RegionsNoise_Offset;

    private static float last_x;
    private static float last_y;
    private static RegionType last_region;

    private void Awake()
    {
        private_scale = scale;
        privateRegionTypes = regionTypes;

        Random.InitState(WorldSeed.seed);
        int alt_seed = Random.Range(int.MinValue, int.MaxValue);

        RegionsNoise_value = new FastNoise();
        RegionsNoise_value.SetNoiseType(FastNoise.NoiseType.Cellular);
        RegionsNoise_value.SetFrequency(0.01f);
        RegionsNoise_value.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        RegionsNoise_value.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);
        RegionsNoise_value.SetSeed(alt_seed);

        RegionsNoise_smooth = new FastNoise();
        RegionsNoise_smooth.SetNoiseType(FastNoise.NoiseType.Cellular);
        RegionsNoise_smooth.SetFrequency(0.01f);
        RegionsNoise_smooth.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        RegionsNoise_smooth.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Sub);
        RegionsNoise_smooth.SetSeed(alt_seed);

        RegionsNoise_Offset = new FastNoise();
        RegionsNoise_Offset.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        RegionsNoise_Offset.SetFractalType(FastNoise.FractalType.FBM);
        RegionsNoise_Offset.SetFrequency(0.01f);
        RegionsNoise_Offset.SetFractalOctaves(1);
        RegionsNoise_Offset.SetFractalLacunarity(2f);
        RegionsNoise_Offset.SetFractalGain(0.5f);
        RegionsNoise_Offset.SetSeed(alt_seed);
    }

    public static float Offset(float x, float y)
    {
        float offset_scale = 0.4f;
        float offset_size = 5f;
        return RegionsNoise_Offset.GetNoise(x * offset_scale, y * offset_scale) * offset_size;
    }

    public static float GetNoise(float x, float y)
    {
        float offset = Offset(x, y);
        return (RegionsNoise_value.GetNoise((x * private_scale) + offset, (y * private_scale) + offset) + 1f) / 2f;
    }

    public static RegionType GetRegion(float x, float y)
    {
        if (x == last_x && y == last_y && last_region != null)
            return last_region;

        if (Max_Weight == 0)
        {
            Max_Weight = 0;
            foreach (RegionType r in privateRegionTypes)
                Max_Weight += r.Weight;
        }

        int val = Mathf.RoundToInt(GetNoise(x, y) * Max_Weight);
        int totalweight = 0;
        RegionType toreturn = null;
        foreach (RegionType r in privateRegionTypes)
        {
            totalweight += r.Weight;
            if (val <= totalweight)
            {
                toreturn = r;
                break;
            }
        }

        return toreturn;
    }

    public static int Regions_value_next_level(float x, float y, int levels, float multiply)
    {
        float multiplied = GetNoise(x, y) * multiply;
        int rounded = Mathf.FloorToInt(multiplied);
        return Mathf.RoundToInt((multiplied - rounded) * levels);
    }

    public static float Regions_smooth(float x, float y)
    {
        float offset = Offset(x, y);
        float smooth = RegionsNoise_smooth.GetNoise((x * private_scale) + offset, (y * private_scale) + offset);
        return smooth;
    }
}

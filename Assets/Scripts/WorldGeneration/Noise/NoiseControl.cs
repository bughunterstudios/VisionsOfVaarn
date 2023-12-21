using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Region
{
    Empty,
    Flat,
    ExtraDune,
    Ripple,
    Canyon,
    Mountain,
    DryLake,
    ToxicLake,
    Oasis,
    DryRiver,
    ToxicRiver,
    Mesa,
    City,
    Cactus,
    Monolith,
    Garbage,
    Statues,
    Rocks,
    Mushrooms,
    Crystals
}

[System.Serializable]
public class RegionType
{
    public int Weight;
    public string Name;
    public NoiseFunction Function;

    public RegionType(int Weight, string Name, NoiseFunction Function)
    {
        this.Weight = Weight;
        this.Name = Name;
        this.Function = Function;
    }

    public float Evaluate(float currentvalue, float smoothing, float x, float y)
    {
        return Function.GetValue(currentvalue, smoothing, x, y);
    }
}

public class NoiseControl : MonoBehaviour
{
    /*private static RegionType[] regionTypes = {
        new RegionType(6, Region.Empty),
        new RegionType(5, Region.Flat),
        new RegionType(6, Region.ExtraDune),
        new RegionType(6, Region.Ripple),
        new RegionType(5, Region.Canyon),
        new RegionType(5, Region.Mountain),
        new RegionType(3, Region.DryLake),
        new RegionType(2, Region.ToxicLake),
        new RegionType(1, Region.Oasis),
        new RegionType(4, Region.DryRiver),
        new RegionType(2, Region.ToxicRiver),
        new RegionType(6, Region.Mesa),
    };*/
    public NoiseFunction baseNoise;
    private static NoiseFunction privateBaseNoise;

    public AnimationCurve ColorCurve;
    private static AnimationCurve ColorCurve_private;

    public float scale_map;
    private static float private_scale_map;

    /*private static RegionType[] tileRegionTypes = {
        new RegionType(4, Region.Empty),
        new RegionType(3, Region.Rocks),
        new RegionType(3, Region.Cactus),
        new RegionType(3, Region.Mushrooms),
        new RegionType(3, Region.Garbage),
        new RegionType(2, Region.Monolith),
        new RegionType(2, Region.Crystals),
        new RegionType(1, Region.City),
        new RegionType(1, Region.Statues)
    };
    private static int Tile_Max_Weight;*/

    private static float last_x;
    private static float last_y;
    private static float last_height;
    private static RegionType last_region;

    private void Awake()
    {
        privateBaseNoise = baseNoise;
        ColorCurve_private = ColorCurve;
        private_scale_map = scale_map;

        /*Tile_Max_Weight = 0;
        foreach (RegionType r in tileRegionTypes)
            Tile_Max_Weight += r.Weight;*/
    }

    public static Color ColorLerp(Color a, Color b, float t)
    {
        return Color.Lerp(a, b, ColorCurve_private.Evaluate(t));
    }

    public static float FloatLerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, ColorCurve_private.Evaluate(t));
    }

    public static Color NoiseColorMap(float x, float y)
    {
        x *= private_scale_map;
        y *= private_scale_map;
        x = Mathf.RoundToInt(x * 10f) / 10f;
        y = Mathf.RoundToInt(y * 10f) / 10f;

        Color col = SandColor.GetColor(x, y);

        RegionType region1 = FirstRegion.GetRegion(x, y);
        RegionType region2 = SecondRegion.GetRegion(x, y);

        float smoothed1 = FirstRegion.Regions_smooth(x, y);
        float smoothed2 = SecondRegion.Regions_smooth(x, y);

        col = region1.Function.Coloring(col, smoothed1, x, y);
        col = region2.Function.Coloring(col, smoothed2, x, y);

        //float coloringsmoothed = region1.Function.Coloring(smoothed1, x, y);
        //coloringsmoothed = Mathf.Max(coloringsmoothed, region2.Function.Coloring(smoothed2, x, y));

        /*if (region == Region.Canyon || region == Region.Mountain || region == Region.Mesa) // Canyon, Mountainous, Mesa
        {
            col = Color.Lerp(col, BandedColor(x, y), BandedColorCurve_private.Evaluate(Regions_smooth(x, y)));
        }*/

        return col;
    }

    public static float NoiseMapHeight(float x, float y)
    {
        x *= private_scale_map;
        y *= private_scale_map;
        x = Mathf.RoundToInt(x * 10f) / 10f;
        y = Mathf.RoundToInt(y * 10f) / 10f;

        RegionType region = FirstRegion.GetRegion(x, y);
        float noise = privateBaseNoise.GetValue(0, 0, x, y);
        float region_s = FirstRegion.Regions_smooth(x, y);
        noise = region.Function.GetValue(noise, region_s, x, y);

        RegionType region2 = SecondRegion.GetRegion(x, y);
        float region_s2 = SecondRegion.Regions_smooth(x, y);
        noise = region2.Function.GetValue(noise, region_s2, x, y);

        last_height = noise;
        last_region = region;
        last_x = x;
        last_y = y;

        noise /= private_scale_map;

        return noise;
    }

    /*public static Region Tile_Regions_value(float x, float y)
    {
        Region toreturn = Regions_value(x, y);

        if (!(toreturn == Region.Canyon || toreturn == Region.Mesa || toreturn == Region.Mountain || toreturn == Region.Oasis || toreturn == Region.ToxicLake || toreturn == Region.ToxicRiver))
        {
            int val = Regions_value_next_level(x, y, Tile_Max_Weight, 10f);
            int totalweight = 0;
            toreturn = Region.Empty;
            foreach (RegionType r in tileRegionTypes)
            {
                totalweight += r.Weight;
                if (val <= totalweight)
                {
                    toreturn = r.region;
                    break;
                }
            }
        }

        return toreturn;
    }*/
}

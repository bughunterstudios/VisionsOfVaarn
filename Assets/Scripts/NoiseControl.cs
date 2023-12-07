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
    Crystals,
    SmallerRegions
}

[System.Serializable]
public class RegionType
{
    public int Weight;
    public Region region;

    public RegionType(int weight, Region region)
    {
        this.Weight = weight;
        this.region = region;
    }
}

public class NoiseControl : MonoBehaviour
{
    private static RegionType[] regionTypes = {
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
        new RegionType(10, Region.SmallerRegions)
    };
    private static int Max_Weight;
    private static bool InSmallerRegion;

    private static RegionType[] tileRegionTypes = {
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
    private static int Tile_Max_Weight;

    private static FastNoise RegionsNoise_value;
    private static FastNoise RegionsNoise_smooth;
    private static FastNoise RegionsNoise_Offset;
    private static FastNoise FlattenDunesNoise;
    private static FastNoise GiantDunesNoise;
    private static FastNoise LargeDunesNoise;
    private static FastNoise SmallDunesNoise;

    //Regions
    public AnimationCurve FlatCurve;
    private static AnimationCurve FlatCurve_private;

    public AnimationCurve ExtraDuneCurve;
    private static AnimationCurve ExtraDuneCurve_private;

    public AnimationCurve CanyonCurve;
    private static AnimationCurve CanyonCurve_private;
    private static FastNoise CanyonNoise;

    public AnimationCurve MountainCurve;
    private static AnimationCurve MountainCurve_private;
    private static FastNoise MountainNoise;

    public AnimationCurve LakeCurve;
    private static AnimationCurve LakeCurve_private;
    private static FastNoise LakeNoise;

    public AnimationCurve RiverCurve;
    private static AnimationCurve RiverCurve_private;
    public AnimationCurve RiverShapeCurve;
    private static AnimationCurve RiverShapeCurve_private;
    private static FastNoise RiverNoise;

    public AnimationCurve MesaCurve;
    private static AnimationCurve MesaCurve_private;
    public AnimationCurve MesaShapeCurve;
    private static AnimationCurve MesaShapeCurve_private;
    private static FastNoise MesaNoise;
    public AnimationCurve MesaHeightCurve;
    private static AnimationCurve MesaHeightCurve_private;
    private static FastNoise MesaHeightNoise;

    public AnimationCurve RippleCurve;
    private static AnimationCurve RippleCurve_private;
    private static FastNoise RippleNoise;

    //Colors
    public Gradient SandColorGradient;
    private static Gradient SandColorGradient_private;
    private static FastNoise SandColorNoise;

    public Gradient BandedColorGradient;
    private static Gradient BandedColorGradient_private;
    public Gradient BandedColorGradient_a_1;
    private static Gradient BandedColorGradient_a_1_private;
    public Gradient BandedColorGradient_a_2;
    private static Gradient BandedColorGradient_a_2_private;
    public Gradient BandedColorGradient_a_3;
    private static Gradient BandedColorGradient_a_3_private;
    public Gradient BandedColorGradient_b_1;
    private static Gradient BandedColorGradient_b_1_private;
    public Gradient BandedColorGradient_b_2;
    private static Gradient BandedColorGradient_b_2_private;
    public Gradient BandedColorGradient_b_3;
    private static Gradient BandedColorGradient_b_3_private;
    public Gradient BandedColorGradient_c_1;
    private static Gradient BandedColorGradient_c_1_private;
    public Gradient BandedColorGradient_c_2;
    private static Gradient BandedColorGradient_c_2_private;
    public Gradient BandedColorGradient_c_3;
    private static Gradient BandedColorGradient_c_3_private;
    public Gradient BandedColorGradient_d_1;
    private static Gradient BandedColorGradient_d_1_private;
    public Gradient BandedColorGradient_d_2;
    private static Gradient BandedColorGradient_d_2_private;
    public Gradient BandedColorGradient_d_3;
    private static Gradient BandedColorGradient_d_3_private;
    public AnimationCurve BandedColorCurve;
    private static AnimationCurve BandedColorCurve_private;
    private static FastNoise BandedColorNoise;

    private static float last_x;
    private static float last_y;
    private static float last_height;
    private static Region last_region;

    private void Awake()
    {
        Max_Weight = 0;
        foreach (RegionType r in regionTypes)
            Max_Weight += r.Weight;

        Tile_Max_Weight = 0;
        foreach (RegionType r in tileRegionTypes)
            Tile_Max_Weight += r.Weight;

        RegionsNoise_value = new FastNoise();
        RegionsNoise_value.SetNoiseType(FastNoise.NoiseType.Cellular);
        RegionsNoise_value.SetFrequency(0.01f);
        RegionsNoise_value.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        RegionsNoise_value.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);

        RegionsNoise_smooth = new FastNoise();
        RegionsNoise_smooth.SetNoiseType(FastNoise.NoiseType.Cellular);
        RegionsNoise_smooth.SetFrequency(0.01f);
        RegionsNoise_smooth.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Euclidean);
        RegionsNoise_smooth.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Sub);

        RegionsNoise_Offset = new FastNoise();
        RegionsNoise_Offset.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        RegionsNoise_Offset.SetFractalType(FastNoise.FractalType.FBM);
        RegionsNoise_Offset.SetFrequency(0.01f);
        RegionsNoise_Offset.SetFractalOctaves(1);
        RegionsNoise_Offset.SetFractalLacunarity(2f);
        RegionsNoise_Offset.SetFractalGain(0.5f);

        FlattenDunesNoise = new FastNoise();
        FlattenDunesNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        FlattenDunesNoise.SetFractalType(FastNoise.FractalType.Billow);
        FlattenDunesNoise.SetFrequency(0.01f);
        FlattenDunesNoise.SetFractalOctaves(2);
        FlattenDunesNoise.SetFractalLacunarity(2);
        FlattenDunesNoise.SetFractalGain(0.5f);

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

        SmallDunesNoise = new FastNoise();
        SmallDunesNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        SmallDunesNoise.SetFractalType(FastNoise.FractalType.Billow);
        SmallDunesNoise.SetFrequency(0.01f);
        SmallDunesNoise.SetFractalOctaves(4);
        SmallDunesNoise.SetFractalLacunarity(2);
        SmallDunesNoise.SetFractalGain(0.5f);

        // Regions
        FlatCurve_private = FlatCurve;
        ExtraDuneCurve_private = ExtraDuneCurve;

        CanyonCurve_private = CanyonCurve;
        CanyonNoise = new FastNoise();
        CanyonNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        CanyonNoise.SetFractalType(FastNoise.FractalType.Billow);
        CanyonNoise.SetFrequency(0.01f);
        CanyonNoise.SetFractalOctaves(6);
        CanyonNoise.SetFractalLacunarity(1.9f);
        CanyonNoise.SetFractalGain(0.5f);

        MountainCurve_private = MountainCurve;
        MountainNoise = new FastNoise();
        MountainNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        MountainNoise.SetFractalType(FastNoise.FractalType.Billow);
        MountainNoise.SetFrequency(0.01f);
        MountainNoise.SetFractalOctaves(6);
        MountainNoise.SetFractalLacunarity(1.9f);
        MountainNoise.SetFractalGain(0.5f);

        LakeCurve_private = LakeCurve;
        LakeNoise = new FastNoise();
        LakeNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        LakeNoise.SetFractalType(FastNoise.FractalType.Billow);
        LakeNoise.SetFrequency(0.01f);
        LakeNoise.SetFractalOctaves(4);
        LakeNoise.SetFractalLacunarity(1.5f);
        LakeNoise.SetFractalGain(0.5f);

        RiverCurve_private = RiverCurve;
        RiverShapeCurve_private = RiverShapeCurve;
        RiverNoise = new FastNoise();
        RiverNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        RiverNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        RiverNoise.SetFrequency(0.01f);
        RiverNoise.SetFractalOctaves(1);
        RiverNoise.SetFractalLacunarity(1.5f);
        RiverNoise.SetFractalGain(0.5f);

        MesaCurve_private = MesaCurve;
        MesaShapeCurve_private = MesaShapeCurve;
        MesaHeightCurve_private = MesaHeightCurve;
        MesaNoise = new FastNoise();
        MesaNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        MesaNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        MesaNoise.SetFrequency(0.01f);
        MesaNoise.SetFractalOctaves(4);
        MesaNoise.SetFractalLacunarity(2f);
        MesaNoise.SetFractalGain(0.5f);
        MesaHeightNoise = new FastNoise();
        MesaHeightNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        MesaHeightNoise.SetFractalType(FastNoise.FractalType.FBM);
        MesaHeightNoise.SetFrequency(0.01f);
        MesaHeightNoise.SetFractalOctaves(5);
        MesaHeightNoise.SetFractalLacunarity(0.7f);
        MesaHeightNoise.SetFractalGain(1.7f);

        RippleCurve_private = RippleCurve;
        RippleNoise = new FastNoise();
        RippleNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        RippleNoise.SetFractalType(FastNoise.FractalType.FBM);
        RippleNoise.SetFrequency(0.01f);
        RippleNoise.SetFractalOctaves(1);
        RippleNoise.SetFractalLacunarity(2f);
        RippleNoise.SetFractalGain(0.5f);

        // Colors

        SandColorGradient_private = SandColorGradient;
        SandColorNoise = new FastNoise();
        SandColorNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        SandColorNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        SandColorNoise.SetFrequency(0.01f);
        SandColorNoise.SetFractalOctaves(4);
        SandColorNoise.SetFractalLacunarity(2);
        SandColorNoise.SetFractalGain(0.5f);

        BandedColorGradient_private = BandedColorGradient;
        BandedColorGradient_a_1_private = BandedColorGradient_a_1;
        BandedColorGradient_a_2_private = BandedColorGradient_a_2;
        BandedColorGradient_a_3_private = BandedColorGradient_a_3;
        BandedColorGradient_b_1_private = BandedColorGradient_b_1;
        BandedColorGradient_b_2_private = BandedColorGradient_b_2;
        BandedColorGradient_b_3_private = BandedColorGradient_b_3;
        BandedColorGradient_c_1_private = BandedColorGradient_c_1;
        BandedColorGradient_c_2_private = BandedColorGradient_c_2;
        BandedColorGradient_c_3_private = BandedColorGradient_c_3;
        BandedColorGradient_d_1_private = BandedColorGradient_d_1;
        BandedColorGradient_d_2_private = BandedColorGradient_d_2;
        BandedColorGradient_d_3_private = BandedColorGradient_d_3;
        BandedColorCurve_private = BandedColorCurve;
        BandedColorNoise = new FastNoise();
        BandedColorNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        BandedColorNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        BandedColorNoise.SetFrequency(0.01f);
        BandedColorNoise.SetFractalOctaves(4);
        BandedColorNoise.SetFractalLacunarity(2);
        BandedColorNoise.SetFractalGain(0.5f);
    }

    public static Color NoiseColorMap(float x, float y)
    {
        x = Mathf.RoundToInt(x * 10f) / 10f;
        y = Mathf.RoundToInt(y * 10f) / 10f;

        Region region = Regions_value(x, y);
        Color col = SandColor(x, y);

        if (region == Region.Canyon || region == Region.Mountain || region == Region.Mesa) // Canyon, Mountainous, Mesa
        {
            col = Color.Lerp(col, BandedColor(x, y), BandedColorCurve_private.Evaluate(Regions_smooth(x, y)));
        }

        return col;
    }

    public static float NoiseMapHeight(float x, float y)
    {
        x = Mathf.RoundToInt(x * 10f) / 10f;
        y = Mathf.RoundToInt(y * 10f) / 10f;

        Region region = Regions_value(x, y);

        float noise = GiantDunes(x, y);
        noise += LargeDunes(x, y);
        noise += TinyDunes(x, y);
        noise *= FlattenDunes(x, y);
        float region_s = Regions_smooth(x, y);
        if (region == Region.Canyon) // Canyon
        {
            noise += CanyonCurve_private.Evaluate(region_s) * Canyon(x, y);
        }
        if (region == Region.Flat) // Flat (has ripples)
        {
            noise = Mathf.Lerp(noise, Ripple(x, y) * 0.5f, FlatCurve_private.Evaluate(region_s));
        }
        if (region == Region.ExtraDune) // Extra dunes
        {
            noise += ExtraDuneCurve_private.Evaluate(region_s) * TinyDunes(x, y) * -2f;
        }
        if (region == Region.Ripple) // Ripples
        {
            noise += RippleCurve_private.Evaluate(region_s) * Ripple(x, y);
        }
        if (region == Region.Mountain) // Mountainous
        {
            noise += MountainCurve_private.Evaluate(region_s) * Mountain(x, y);
        }
        if (region == Region.DryLake || region == Region.ToxicLake || region == Region.Oasis) // Lakes
        {
            noise = Mathf.Lerp(noise, Lake(x, y), LakeCurve_private.Evaluate(region_s));
        }
        if (region == Region.DryRiver || region == Region.ToxicRiver) // River
        {
            noise = Mathf.Lerp(noise, River(x, y), RiverCurve_private.Evaluate(region_s));
        }
        if (region == Region.Mesa) // Mesa
        {
            noise = Mathf.Lerp(noise, (noise * 0.4f) + Mesa(x, y), MesaCurve_private.Evaluate(region_s));
        }

        last_height = noise;
        last_region = region;
        last_x = x;
        last_y = y;

        return noise;
    }

    public static float Offset(float x, float y)
    {
        float offset_scale = 4f;
        float offset_size = 2f;
        return RegionsNoise_Offset.GetNoise(x * offset_scale, y * offset_scale) * offset_size;
    }

    public static Region Regions_value(float x, float y)
    {
        if (x == last_x && y == last_y)
            return last_region;

        float scale = .2f;
        float offset = Offset(x, y);
        int val = Mathf.RoundToInt(((RegionsNoise_value.GetNoise((x * scale) + offset, (y * scale) + offset) + 1f) / 2f) * Max_Weight);
        int totalweight = 0;
        Region toreturn = Region.Empty;
        foreach (RegionType r in regionTypes)
        {
            totalweight += r.Weight;
            if (val <= totalweight)
            {
                toreturn = r.region;
                break;
            }
        }
        InSmallerRegion = false;
        if (toreturn == Region.SmallerRegions)
        {
            InSmallerRegion = true;
            scale = 1f;
            val = Mathf.RoundToInt(((RegionsNoise_value.GetNoise((x * scale) + offset, (y * scale) + offset) + 1f) / 2f) * (Max_Weight - regionTypes[regionTypes.Length - 1].Weight));
            totalweight = 0;
            foreach (RegionType r in regionTypes)
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
    }

    public static Region Tile_Regions_value(float x, float y)
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
    }

    public static int Regions_value_next_level(float x, float y, int levels, float multiply)
    {
        float scale = .2f;
        float offset = Offset(x, y);
        float multiplied = ((RegionsNoise_value.GetNoise((x * scale) + offset, (y * scale) + offset) + 1f) / 2f) * multiply;
        int rounded = Mathf.FloorToInt(multiplied);
        return Mathf.RoundToInt((multiplied - rounded) * levels);
    }

    public static float Regions_smooth(float x, float y)
    {
        float scale = .2f;
        float offset = Offset(x, y);
        float smooth = RegionsNoise_smooth.GetNoise((x * scale) + offset, (y * scale) + offset);
        if (InSmallerRegion)
            return RegionsNoise_smooth.GetNoise(x * 1, y * 1) * smooth;
        else
            return smooth;
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

    public static float Ripple(float x, float y)
    {
        float scale1 = 1f;
        float scale2 = 4f;
        float height = 0.002f;
        return Mathf.Sin((y * scale1) + (RippleNoise.GetNoise(x * scale2, y * scale2) * 10)) * height;
    }

    // Regions

    public static float Canyon(float x, float y)
    {
        float scale = 0.2f;
        float height = 1.5f;
        return (Mathf.Min(-CanyonNoise.GetNoise(x * scale, y * scale) - 0.5f, 0f)) * height;
    }

    public static float Mountain(float x, float y)
    {
        float scale = 0.2f;
        float height = 1.5f;
        return (Mathf.Min(MountainNoise.GetNoise(x * scale, y * scale), 0f)) * -height;
    }

    public static float Lake(float x, float y)
    {
        float scale = 1f;
        float height = .2f;
        return (Mathf.Min(-LakeNoise.GetNoise(x * scale, y * scale) - 0.5f, 0f)) * height;
    }

    public static float River(float x, float y)
    {
        float scale = 0.5f;
        float height = .1f;
        return RiverShapeCurve_private.Evaluate(RiverNoise.GetNoise(x * scale, y * scale)) * -height;
    }

    public static float Mesa(float x, float y)
    {
        float scale_height = 2f;
        float scale = 0.7f;
        float height = MesaHeightCurve_private.Evaluate(MesaHeightNoise.GetNoise(x * scale_height, y * scale_height));//.25f;
        return MesaShapeCurve_private.Evaluate(-MesaNoise.GetNoise(x * scale, y * scale) + 0.5f) * height;
    }

    // Colors
    public static Color SandColor(float x, float y)
    {
        float scale = 0.1f;
        float scale2 = .2f;
        float offset = Offset(x, y);
        float region_noise = ((RegionsNoise_value.GetNoise((x * scale2) + offset, (y * scale2) + offset) + 1f) / 2f) * Regions_smooth(x, y);
        float val = Mathf.PingPong(SandColorNoise.GetNoise(x * scale, y * scale) + 0.5f + region_noise, 1f);
        return SandColorGradient_private.Evaluate(val);
    }

    public static Color BandedColor(float x, float y)
    {
        float height = last_height;
        if (x != last_x || y != last_y)
            height = NoiseMapHeight(x, y);

        float scale = 1f;

        float scale2 = .2f;
        float offset = Offset(x, y);
        float multiplied = ((RegionsNoise_value.GetNoise((x * scale2) + offset, (y * scale2) + offset) + 1f) / 2f) * 10000f;
        float hueshift = multiplied - Mathf.RoundToInt(multiplied);

        float val = Mathf.PingPong((BandedColorNoise.GetNoise(x * scale, y * scale) * 0.2f) + (height * 5f), 3f);

        int banded_color_set = Regions_value_next_level(x, y, 3, 100);

        Color col;
        if (banded_color_set == 0)
            col = val < 1f ? BandedColorGradient_a_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_a_2_private.Evaluate(val - 1f) : BandedColorGradient_a_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 1)
            col = val < 1f ? BandedColorGradient_b_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_b_2_private.Evaluate(val - 1f) : BandedColorGradient_b_3_private.Evaluate(val - 2f));
        else if (banded_color_set == 2)
            col = val < 1f ? BandedColorGradient_c_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_c_2_private.Evaluate(val - 1f) : BandedColorGradient_c_3_private.Evaluate(val - 2f));
        else
            col = val < 1f ? BandedColorGradient_d_1_private.Evaluate(val) : (val < 2f ? BandedColorGradient_d_2_private.Evaluate(val - 1f) : BandedColorGradient_d_3_private.Evaluate(val - 2f));

        float h, s, v;
        Color.RGBToHSV(col, out h, out s, out v);
        float a = col.a;
        col = Color.HSVToRGB(Mathf.Repeat(h + hueshift, 1f), s, v);
        col.a = a;

        return col;
    }
}

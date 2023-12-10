using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandColor : MonoBehaviour
{
    public Gradient SandColorGradient;
    private static Gradient SandColorGradient_private;
    private static FastNoise SandColorNoise;

    private void Awake()
    {
        SandColorGradient_private = SandColorGradient;
        SandColorNoise = new FastNoise();
        SandColorNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        SandColorNoise.SetFractalType(FastNoise.FractalType.RigidMulti);
        SandColorNoise.SetFrequency(0.01f);
        SandColorNoise.SetFractalOctaves(4);
        SandColorNoise.SetFractalLacunarity(2);
        SandColorNoise.SetFractalGain(0.5f);
        SandColorNoise.SetSeed(WorldSeed.seed);
    }

    public static Color GetColor(float x, float y)
    {
        float scale = 0.1f;
        float scale2 = .2f;
        float offset = FirstRegion.Offset(x, y);
        float region_noise = ((FirstRegion.GetNoise((x * scale2) + offset, (y * scale2) + offset) + 1f) / 2f) * FirstRegion.Regions_smooth(x, y);
        float val = Mathf.PingPong(SandColorNoise.GetNoise(x * scale, y * scale) + 0.5f + region_noise, 1f);
        return SandColorGradient_private.Evaluate(val);
    }
}

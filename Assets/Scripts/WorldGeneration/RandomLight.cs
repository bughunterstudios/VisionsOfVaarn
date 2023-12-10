using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLight : MonoBehaviour
{
    public Gradient color;
    public float min_range;
    public float max_range;
    public float min_intensity;
    public float max_intensity;

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        Light light = GetComponent<Light>();
        light.color = color.Evaluate(Random.Range(0f, 1f));
        light.range = Random.Range(min_range, max_range);
        light.intensity = Random.Range(min_intensity, max_intensity);
    }
}

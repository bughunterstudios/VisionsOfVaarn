using LlockhamIndustries.Decals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGraffiti : MonoBehaviour
{
    public float emissionChance;
    public List<Texture2D> textures;
    public AnimationCurve size;
    public float size_min;
    public float size_max;

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);

        Projection newproj = new Metallic();
        newproj.Mat.mainTexture = textures[Random.Range(0, textures.Count)];
        ProjectionRenderer projectionrenderer = gameObject.AddComponent<ProjectionRenderer>();
        projectionrenderer.Projection = newproj;
        projectionrenderer.SetColor(0, Random.ColorHSV());
        if (Random.Range(0f, 1f) <= emissionChance)
            projectionrenderer.SetColor(1, Random.ColorHSV() * 4f);
        projectionrenderer.MaskMethod = MaskMethod.OnlyDrawOn;
        projectionrenderer.MaskLayer1 = true;
        projectionrenderer.UpdateProjection();

        transform.localScale *= (size.Evaluate(Random.Range(0f, 1f)) * (size_max - size_min)) + size_min;
    }
}

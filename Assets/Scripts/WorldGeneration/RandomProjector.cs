using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomProjector : MonoBehaviour
{
    public Gradient randomcolor;
    public List<Texture2D> textures;
    public AnimationCurve size;
    public float size_min;
    public float size_max;

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        Material mat = new Material(GetComponent<Projector>().material);

        mat.color = randomcolor.Evaluate(Random.Range(0f, 1f));
        Texture2D chosentexture = textures[Random.Range(0, textures.Count)];
        mat.SetTexture("_ShadowTex", chosentexture);
        //mat.SetTexture("_FalloffTex", chosentexture);

        GetComponent<Projector>().material = mat;

        GetComponent<Projector>().orthographicSize = (size.Evaluate(Random.Range(0f, 1f)) * (size_max - size_min)) + size_min;
    }
}

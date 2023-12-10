using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomMaterialItem
{
    public int Weight;
    public Material Mat;
}

public class RandomMaterial : MonoBehaviour
{
    public List<RandomMaterialItem> Mats;
    public bool oninit;
    public float inheritmaterialchance;
    public string inherittag;
    public int mat_count = 1;

    public bool from_noise_color;
    public bool index_from_noise;

    private Material[] chosenmat;

    private void Start()
    {
        if (oninit)
        {
            Generate();
        }
    }

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        Generate();
    }

    public void Generate()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            if (from_noise_color)
            {
                renderer.material.color = NoiseControl.NoiseColorMap(transform.position.x, transform.position.z);
            }
            else
            {
                Material[] currentmats = renderer.materials;
                chosenmat = new Material[currentmats.Length];
                for (int i = 0; i < currentmats.Length; i++)
                {
                    chosenmat[i] = ChooseMaterial(i);
                    currentmats[i] = chosenmat[i];
                }
                renderer.materials = currentmats;
            }
        }
        else
        {
            chosenmat = new Material[mat_count];
            for (int i = 0; i < mat_count; i++)
                chosenmat[i] = ChooseMaterial(i);
        }
    }

    public Material GetMaterial(int index)
    {
        return chosenmat[index];
    }

    private Material ChooseMaterial(int index)
    {
        if (inheritmaterialchance != 0)
        {
            if (inheritmaterialchance == 1 || Random.Range(0f, 1f) <= inheritmaterialchance)
            {
                RandomMaterial parentmat = transform.parent.GetComponentInParent<RandomMaterial>();
                if (parentmat != null)
                {
                    if (parentmat.inherittag == inherittag)
                        return parentmat.GetMaterial(index);
                }
            }
        }

        int totalweight = 0;
        foreach (RandomMaterialItem Mat in Mats)
        {
            totalweight += Mat.Weight;
        }
        int chosenvalue;
        if (index_from_noise)
            chosenvalue = Mathf.RoundToInt(Mathf.PingPong(FirstRegion.Regions_value_next_level(transform.position.x, transform.position.z, totalweight, 10), totalweight-1)) + 1;
        else
            chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (RandomMaterialItem Mat in Mats)
        {
            totalweight += Mat.Weight;
            if (chosenvalue <= totalweight)
            {
                if (Mat.Mat != null)
                {
                    return Mat.Mat;
                }
                break;
            }
        }
        return null;
    }
}

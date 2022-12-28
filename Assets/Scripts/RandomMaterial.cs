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
        Material[] currentmats = renderer.materials;

        for (int i = 0; i < currentmats.Length; i++)
        {
            int totalweight = 0;
            foreach (RandomMaterialItem Mat in Mats)
            {
                totalweight += Mat.Weight;
            }
            int chosenvalue = Random.Range(1, totalweight + 1);
            totalweight = 0;
            foreach (RandomMaterialItem Mat in Mats)
            {
                totalweight += Mat.Weight;
                if (chosenvalue <= totalweight)
                {
                    if (Mat.Mat != null)
                    {
                        currentmats[i] = Mat.Mat;
                        //GetComponent<MeshRenderer>().materials[i] = Mat.Mat;
                    }
                    break;
                }
            }
        }
        renderer.materials = currentmats;
    }
}

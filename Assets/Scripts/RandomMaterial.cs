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
                    GetComponent<MeshRenderer>().material = Mat.Mat;
                }
                break;
            }
        }
    }
}

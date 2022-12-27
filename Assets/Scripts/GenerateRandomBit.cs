using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomBit
{
    public int Weight;
    public GameObject Prefab;
}

public class GenerateRandomBit : MonoBehaviour
{
    public List<RandomBit> Bits;
    public int player_distance = -1;

    private Seed seed;

    private bool generated;

    private float time;

    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (!generated)
        {
            if (player_distance == -1)
            {
                time -= Time.deltaTime;
                if (time < 0)
                    ActuallyGenerate();
            }
            else
            {
                if (Vector3.Distance(cam.position, transform.position) < (player_distance * 50))
                    player_distance = -1;
            }
        }
    }

    private void ActuallyGenerate()
    {
        Random.InitState(seed.seed);
        int totalweight = 0;
        foreach (RandomBit Bit in Bits)
        {
            totalweight += Bit.Weight;
        }
        int chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (RandomBit Bit in Bits)
        {
            totalweight += Bit.Weight;
            if (chosenvalue <= totalweight)
            {
                if (Bit.Prefab != null)
                {
                    GameObject newobject = Instantiate(Bit.Prefab, transform);
                    Seed newseed = new Seed(Random.Range(int.MinValue, int.MaxValue), seed.X, seed.Y);
                    newobject.SendMessage("Generate", newseed, SendMessageOptions.DontRequireReceiver);
                }
                break;
            }
        }
        generated = true;
        this.enabled = false;
    }

    public void Generate(Seed seed)
    {
        generated = false;
        this.seed = seed;
        time = Random.Range(0f, 0.5f);

        if (seed.X == 0 && seed.Y == 0)
            ActuallyGenerate();
    }
}

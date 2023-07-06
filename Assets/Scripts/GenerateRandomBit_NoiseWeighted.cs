using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomBit_NoiseWeighted
{
    public float Base_Weight;
    public float Max_Weight;
    public GameObject Prefab;
    public AnimationCurve Influence;
    public List<Region> EffectingRegions;

    public float GetWeight(Region region, float smooth)
    {
        float toreturn = Base_Weight;
        if (EffectingRegions.Contains(region))
            toreturn += Max_Weight * Influence.Evaluate(smooth);
        return toreturn;
    }
}

public class GenerateRandomBit_NoiseWeighted : MonoBehaviour
{
    public List<RandomBit_NoiseWeighted> Bits;
    public int player_distance = -1;

    private Seed seed;

    private bool generated;
    private bool received_seed;

    private float time;

    private Transform cam;

    private Region region;
    private float smooth;
    private float chosenvalue;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        if (!generated && received_seed)
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
        if (seed == null)
        {
            Debug.LogError("Generate Random Bit Error In " + this.name);
            return;
        }

        Random.InitState(seed.seed);
        float totalweight = 0;
        foreach (RandomBit_NoiseWeighted Bit in Bits)
        {
            totalweight += Bit.GetWeight(region, smooth);
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
        received_seed = true;
        this.seed = seed;
        time = Random.Range(0f, 1f);

        region = NoiseControl.Tile_Regions_value(transform.position.x, transform.position.z);
        smooth = NoiseControl.Regions_smooth(transform.position.x, transform.position.z);

        float totalweight = 0;
        foreach (RandomBit_NoiseWeighted Bit in Bits)
        {
            totalweight += Bit.GetWeight(region, smooth);
        }
        chosenvalue = Random.Range(0f, totalweight);

        if (seed.X == 0 && seed.Y == 0)
            ActuallyGenerate();
    }
}

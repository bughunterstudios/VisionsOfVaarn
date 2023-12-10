using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRandomBit_NoWeight : MonoBehaviour
{
    public List<GameObject> Bits;
    public int player_distance = -1;

    private Seed seed;

    private bool generated;
    private bool received_seed;

    private float time;

    private Transform cam;

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
        int chosenvalue = Random.Range(0, Bits.Count);

        GameObject newobject = Instantiate(Bits[chosenvalue], transform);
        Seed newseed = new Seed(Random.Range(int.MinValue, int.MaxValue), seed.X, seed.Y);
        newobject.SendMessage("Generate", newseed, SendMessageOptions.DontRequireReceiver);

        generated = true;
        this.enabled = false;
    }

    public void Generate(Seed seed)
    {
        generated = false;
        received_seed = true;
        this.seed = seed;
        time = Random.Range(0f, 0.5f);

        if (seed.X == 0 && seed.Y == 0)
            ActuallyGenerate();
    }
}

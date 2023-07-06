using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassOnGenerate : MonoBehaviour
{
    public bool FrameStep;
    private bool generating;
    private int i;
    private Seed seed;

    private void Update()
    {
        if (generating && FrameStep)
        {
            if (i < transform.childCount)
            {
                if (transform.GetChild(i).parent == transform)
                {
                    Seed newseed = new Seed(Random.Range(int.MinValue, int.MaxValue), seed.X, seed.Y);
                    transform.GetChild(i).SendMessage("Generate", newseed, SendMessageOptions.DontRequireReceiver);
                }
                i++;
            }
            else
                generating = false;
        }
    }

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        this.seed = seed;

        if (!FrameStep)
        {
            for (i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).parent == transform)
                {
                    Seed newseed = new Seed(Random.Range(int.MinValue, int.MaxValue), seed.X, seed.Y);
                    transform.GetChild(i).SendMessage("Generate", newseed, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else
            generating = true;
    }
}

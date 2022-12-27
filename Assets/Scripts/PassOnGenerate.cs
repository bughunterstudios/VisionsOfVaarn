using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassOnGenerate : MonoBehaviour
{
    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        for (int i = 0; i < transform.childCount; i++)
        {
            Seed newseed = new Seed(Random.Range(int.MinValue, int.MaxValue), seed.X, seed.Y);
            transform.GetChild(i).SendMessage("Generate", newseed, SendMessageOptions.DontRequireReceiver);
        }
    }
}

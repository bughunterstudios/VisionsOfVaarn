using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSeed : MonoBehaviour
{
    [SerializeField]
    private int seed_value;
    public static int seed;

    private void Awake()
    {
        seed = seed_value;
    }
}

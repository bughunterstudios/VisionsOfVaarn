using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLanternScript : MonoBehaviour
{
    public Light sun;
    private float maxsun;
    private Light lantern;
    private float maxlantern;

    // Start is called before the first frame update
    void Start()
    {
        lantern = GetComponent<Light>();
        maxlantern = lantern.intensity;
        maxsun = sun.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        lantern.intensity = ((maxsun - sun.intensity) / maxsun) * maxlantern;
    }
}

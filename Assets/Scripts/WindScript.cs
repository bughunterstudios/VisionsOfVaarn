using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    public float changespeed;
    public float minvolume;

    private float maxvolume;
    private AudioSource source;

    private float target;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        maxvolume = source.volume;
        target = maxvolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (source.volume != target)
            source.volume = Mathf.Lerp(source.volume, target, changespeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Indoors")
            target = minvolume;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Indoors")
            target = maxvolume;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    public Vector3 rotationlimits;
    public Vector3 translatelimits;
    public Vector3 scalelimits;
    public bool discrete;

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        if (discrete)
        {
            transform.Rotate(Random.Range(-1000, 1000) * rotationlimits.x,
                Random.Range(-1000, 1000) * rotationlimits.y,
                Random.Range(-1000, 1000) * rotationlimits.z, Space.Self);
            transform.Translate(Random.Range(-1, 2) * translatelimits.x,
                Random.Range(-1, 2) * translatelimits.y,
                Random.Range(-1, 2) * translatelimits.z, Space.World);
            transform.localScale = new Vector3(transform.localScale.x + (Random.Range(-1, 2) * scalelimits.x),
                transform.localScale.y + (Random.Range(-1, 2) * scalelimits.y),
                transform.localScale.z + (Random.Range(-1, 2) * scalelimits.z));
        }
        else
        {
            transform.Rotate(Random.Range(-rotationlimits.x, rotationlimits.x),
                Random.Range(-rotationlimits.y, rotationlimits.y),
                Random.Range(-rotationlimits.z, rotationlimits.z), Space.Self);
            transform.Translate(Random.Range(-translatelimits.x, translatelimits.x),
                Random.Range(-translatelimits.y, translatelimits.y),
                Random.Range(-translatelimits.z, translatelimits.z), Space.World);
            transform.localScale = new Vector3(transform.localScale.x + Random.Range(-scalelimits.x, scalelimits.x),
                transform.localScale.y + Random.Range(-scalelimits.y, scalelimits.y),
                transform.localScale.z + Random.Range(-scalelimits.z, scalelimits.z));
        }
    }
}

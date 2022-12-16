using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDestroyWithChunk : MonoBehaviour
{
    public float maxdistance = 500f;
    public Transform child;

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);
        if (child == null)
            child = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(child.transform.position, Camera.main.transform.position) > maxdistance)
            Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSandwormTrail : MonoBehaviour
{
    public GameObject prefab;
    public float distance;
    public Transform position;

    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = position.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(pos, position.position) > distance)
        {
            GameObject newtrail = Instantiate(prefab);
            newtrail.transform.position = position.position;
            newtrail.transform.rotation = position.rotation;
            newtrail.transform.localScale *= (transform.parent.localScale.x);
            pos = position.position;
        }
    }
}

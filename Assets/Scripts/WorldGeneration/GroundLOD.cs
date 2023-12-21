using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLOD : MonoBehaviour
{
    public List<DynamicGround> grounds;
    public List<int> distances;
    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        bool foundtarget = false;

        Vector3 campos = cam.transform.position;
        campos.y = 0;

        for (int i = 0; i < grounds.Count; i++)
        {
            if (grounds[i].DoneGenerating() && !foundtarget && (distances[i] == -1 || Vector3.Distance(campos, transform.position) < (distances[i] * 50)))
            {
                grounds[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
                if (grounds[i].gameObject.GetComponent<MeshCollider>())
                    grounds[i].gameObject.GetComponent<MeshCollider>().enabled = true;
                foundtarget = true;
            }
            else
            {
                grounds[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
                if (grounds[i].gameObject.GetComponent<MeshCollider>())
                    grounds[i].gameObject.GetComponent<MeshCollider>().enabled = false;
            }
        }
    }
}

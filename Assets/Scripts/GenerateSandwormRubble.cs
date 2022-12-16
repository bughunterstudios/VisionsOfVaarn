using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSandwormRubble : MonoBehaviour
{
    public GameObject prefab;
    public float probability;
    public List<Transform> positions;
    public float min_scale;
    public float max_scale;
    public float min_force;
    public float max_force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0f, 1f) < probability)
        {
            GameObject newrubble = Instantiate(prefab);
            newrubble.transform.position = positions[Random.Range(0, positions.Count)].position;
            newrubble.transform.localScale *= Random.Range(min_scale, max_scale);
            newrubble.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * Random.Range(min_force, max_force));
        }
    }
}

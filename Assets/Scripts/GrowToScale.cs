using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowToScale : MonoBehaviour
{
    public float speed;
    public float lifetime;

    private Vector3 maxscale;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        maxscale = transform.localScale;
        transform.localScale = Vector3.one * 0.1f;
        time = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, maxscale, speed * Time.deltaTime);
        time -= Time.deltaTime;
        if (time < 0)
            maxscale = Vector3.one * 0.01f;
        if (transform.localScale.magnitude < 0.1f)
            Destroy(gameObject);
    }
}

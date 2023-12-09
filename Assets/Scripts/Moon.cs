using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    public float MoonCycleInMinutes;

    public Transform sun;

    private float rotations;
    private float y, z;

    // Start is called before the first frame update
    void Start()
    {
        y = transform.rotation.eulerAngles.y;
        z = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        rotations += Time.deltaTime * (360f / (MoonCycleInMinutes * 60f));
        transform.Rotate(Time.deltaTime * (360f / (MoonCycleInMinutes * 60f)), 0, 0, Space.Self);

        Shader.SetGlobalVector("_MoonAngles", new Vector3(rotations, y, z));

        float dotproduct = (Vector3.Dot(transform.forward, sun.forward) + 1f) / 2f;
        Shader.SetGlobalFloat("_Eclipse", dotproduct);
    }
}

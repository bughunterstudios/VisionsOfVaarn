using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public Gradient fog;

    public float DayLengthInMinutes;
    public Gradient light_level;

    private float rotations;
    private float y, z;
    private float max_intensity;

    // Start is called before the first frame update
    void Start()
    {
        y = transform.rotation.eulerAngles.y;
        z = transform.rotation.eulerAngles.z;
        max_intensity = GetComponent<Light>().intensity;
    }

        // Update is called once per frame
    void Update()
    {
        rotations += Time.deltaTime * (360f / (DayLengthInMinutes * 60f));
        transform.rotation = Quaternion.Euler(rotations, y, z);

        Shader.SetGlobalVector("_SunDirection", transform.forward);
        Shader.SetGlobalVector("_SunAngles", new Vector3(rotations, y, z));
        Shader.SetGlobalFloat("_SkyRotation", rotations);

        float dotproduct = (Vector3.Dot(transform.forward, Vector3.down) + 1f) / 2f;
        RenderSettings.fogColor = fog.Evaluate(dotproduct);

        GetComponent<Light>().intensity = light_level.Evaluate(dotproduct).r * max_intensity;
    }
}

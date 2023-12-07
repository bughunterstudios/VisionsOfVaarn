using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateDitherOffset : MonoBehaviour
{
    public float multiplier_x;
    public float multiplier_y;

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_DitherOffset_X", Camera.main.transform.eulerAngles.y * multiplier_x);
        Shader.SetGlobalFloat("_DitherOffset_Y", Camera.main.transform.eulerAngles.x * multiplier_y);
    }
}

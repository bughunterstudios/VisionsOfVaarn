using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mat_Animate
{
    public Material mat;
    public Vector2 speed;
}

public class TilingOffsetControl : MonoBehaviour
{
    public List<Mat_Animate> mats;

    // Update is called once per frame
    void Update()
    {
        foreach (Mat_Animate mat in mats)
            mat.mat.mainTextureOffset = mat.speed * Time.time;
    }
}

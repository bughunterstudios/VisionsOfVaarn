using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHeightToNoise : MonoBehaviour
{
    public bool AlignToNormal;
    public bool AverageOfCorners;
    public bool BottomOfCorners;
    public float CornerDistance = 1f;

    private void Start()
    {
        float newy = NoiseControl.NoiseMapHeight(transform.position.x, transform.position.z);
        transform.position = new Vector3(transform.position.x, newy * 100, transform.position.z);

        if (AlignToNormal || AverageOfCorners || BottomOfCorners)
        {
            float top_y = NoiseControl.NoiseMapHeight(transform.position.x + CornerDistance, transform.position.z + CornerDistance) * 100;
            float right_y = NoiseControl.NoiseMapHeight(transform.position.x + CornerDistance, transform.position.z - CornerDistance) * 100;
            float bottom_y = NoiseControl.NoiseMapHeight(transform.position.x - CornerDistance, transform.position.z - CornerDistance) * 100;
            float left_y = NoiseControl.NoiseMapHeight(transform.position.x - CornerDistance, transform.position.z + CornerDistance) * 100;
            Vector3[] verts = new Vector3[4];
            verts[0] = new Vector3(transform.position.x + CornerDistance, top_y, transform.position.z + CornerDistance);
            verts[1] = new Vector3(transform.position.x + CornerDistance, right_y, transform.position.z - CornerDistance);
            verts[2] = new Vector3(transform.position.x - CornerDistance, bottom_y, transform.position.z - CornerDistance);
            verts[3] = new Vector3(transform.position.x - CornerDistance, left_y, transform.position.z + CornerDistance);

            if (AlignToNormal)
            {
                var x = verts[3] - verts[0] + verts[2] - verts[1];
                var y = verts[3] - verts[2] + verts[0] - verts[1];
                var z = Vector3.Cross(x, y);

                transform.localRotation = Quaternion.LookRotation(y, z);
            }

            if (AverageOfCorners)
            {
                transform.position = new Vector3(transform.position.x, (top_y + right_y + bottom_y + left_y) / 4f, transform.position.z);
            }

            if (BottomOfCorners)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Min(top_y, right_y, bottom_y, left_y), transform.position.z);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadCam : MonoBehaviour
{
    public float acceleration;
    public float max_velocity;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity.x = Mathf.Lerp(velocity.x, Input.GetAxis("Horizontal") * max_velocity, acceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, Input.GetAxis("Vertical") * max_velocity, acceleration * Time.deltaTime);

        transform.Translate(velocity * Time.deltaTime);
    }
}

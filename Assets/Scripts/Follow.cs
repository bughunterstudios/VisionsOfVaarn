using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform follow;
    public bool position;
    public bool rotation;
    public float distance = -1;
    public float speed;
    public float turnspeed;

    // Update is called once per frame
    void Update()
    {
        float targetdist = Vector3.Distance(transform.position, follow.position);

        if (rotation)
        {
            if (distance == -1)
                transform.rotation = follow.rotation;
            else
            {
                if (targetdist > distance)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(follow.position - transform.position);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeed * (targetdist) * Time.deltaTime);
                }
            }
        }
        if (position)
        {
            if (distance == -1)
                transform.position = follow.position;
            else
            {
                if (targetdist > distance)
                    transform.position += transform.forward * speed * (targetdist / distance) * Time.deltaTime;
            }
        }
    }
}

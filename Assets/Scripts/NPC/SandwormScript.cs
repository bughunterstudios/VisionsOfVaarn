using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwormScript : MonoBehaviour
{
    public int length;
    public AnimationCurve ringsize;
    public Vector3 offset;
    public GameObject head;
    public GameObject ringprefab;
    public CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        if (head != null && ringprefab != null)
        {
            controller.enabled = false;

            Vector3 localpos = Vector3.zero;
            Transform follow = head.transform;
            for (int i = 0; i < length; i++)
            {
                float scale = ringsize.Evaluate(i / (float)length);
                localpos += offset * scale;
                GameObject newring = Instantiate(ringprefab, transform);
                newring.transform.localPosition = localpos;
                newring.transform.localPosition -= newring.transform.up * (1-scale) * 5;
                newring.transform.localScale *= scale;
                newring.GetComponent<Follow>().follow = follow;
                follow = newring.transform;

                if (i <= 1)
                    Physics.IgnoreCollision(newring.GetComponentInChildren<Collider>(), controller);
            }

            controller.enabled = true;

            Camera.main.GetComponentInParent<CameraRumble>().AddRumbleObject(head.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

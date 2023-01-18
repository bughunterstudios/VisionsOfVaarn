using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwormScript : MonoBehaviour
{
    public int min_length;
    public int max_length;
    public float min_size;
    public float max_size;
    public AnimationCurve ringsize;
    public Vector3 offset;
    public GameObject head;
    public GameObject ringprefab;
    public CharacterController controller;

    private int i;
    private int length;
    private float scale_change;
    private Vector3 localpos;
    private Transform follow;
    private bool generating;

    // Start is called before the first frame update
    void Start()
    {
        if (head != null && ringprefab != null)
        {
            controller.enabled = false;

            localpos = Vector3.zero;
            follow = head.transform;
            controller.enabled = true;
            Camera.main.GetComponentInParent<CameraRumble>().AddRumbleObject(head.transform);
            i = 0;
        }
    }

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        length = Random.Range(min_length, max_length);
        scale_change = Random.Range(min_size, max_size);
        transform.localScale *= scale_change;
        head.GetComponent<AI>().movespeed *= scale_change;
        generating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (generating)
        {
            if (i < length)
            {
                float scale = ringsize.Evaluate(i / (float)length);
                localpos += offset * scale;
                GameObject newring = Instantiate(ringprefab, transform);
                newring.transform.localPosition = localpos;
                newring.transform.localPosition -= newring.transform.up * 100;//newring.transform.up * (1 - scale) * 5;
                newring.transform.localScale *= scale;
                newring.GetComponent<Follow>().distance *= scale_change;
                newring.GetComponent<Follow>().speed *= scale_change;
                newring.GetComponent<Follow>().follow = follow;
                follow = newring.transform;

                if (i <= 1)
                    Physics.IgnoreCollision(newring.GetComponentInChildren<Collider>(), controller);
                i++;
            }
            else
            {
                this.enabled = false;
            }
        }
    }
}

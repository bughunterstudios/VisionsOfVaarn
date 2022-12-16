using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRumble : MonoBehaviour
{
    public float rumbledistance;
    public AnimationCurve rumblecurve;
    public float rumbleamount;
    public float rumblespeed;
    public float rumblefrequency;
    public float minimumrumble;

    private Vector3 initialposition;
    private List<Transform> rumbleobjects;
    private Vector3 rumble;

    // Start is called before the first frame update
    void Start()
    {
        initialposition = transform.localPosition;
        rumbleobjects = new List<Transform>();
    }

    public void AddRumbleObject(Transform t)
    {
        rumbleobjects.Add(t);
    }

    // Update is called once per frame
    void Update()
    {
        if (rumbleobjects.Count > 0)
        {
            float distance = float.PositiveInfinity;
            for (int i = 0; i < rumbleobjects.Count; i++)
            {
                if (rumbleobjects[i] == null)
                {
                    rumbleobjects.RemoveAt(i);
                    i--;
                }
                else
                {
                    float thisdistance = Vector3.Distance(transform.position, rumbleobjects[i].position);
                    if (thisdistance < distance)
                        distance = thisdistance;
                }
            }
            float normalizeddistance = distance / rumbledistance;
            float curve = rumblecurve.Evaluate(normalizeddistance);
            if (distance == float.PositiveInfinity)
                curve = 0;
            if (curve >= minimumrumble)
            {
                if (Random.Range(0f, 1f) < rumblefrequency)
                    rumble = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * curve * rumbleamount;
                transform.localPosition = Vector3.Lerp(transform.localPosition, initialposition + rumble, rumblespeed * Time.deltaTime);
            }
            else
                transform.localPosition = initialposition;
        }
    }
}

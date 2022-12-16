using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSilencer : MonoBehaviour
{
    public AnimationCurve curve;
    public float mindistance;
    public float maxdistance;

    public RandomSound music;
    private AudioSource source;
    private Transform cam;

    private List<Transform> objects;

    // Start is called before the first frame update
    void Start()
    {
        source = music.GetComponent<AudioSource>();
        cam = Camera.main.transform;
        objects = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (objects.Count > 0)
        {
            float distance = float.PositiveInfinity;
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] == null)
                {
                    objects.RemoveAt(i);
                    i--;
                }
                else
                {
                    float thisdistance = Vector3.Distance(cam.position, objects[i].position);
                    if (thisdistance < distance)
                        distance = thisdistance;
                }
            }
            float normalizeddistance = (distance - mindistance) / (maxdistance - mindistance);
            float curve_val = curve.Evaluate(normalizeddistance);
            if (distance == float.PositiveInfinity)
                curve_val = 1;
            float volume = 1;
            if (music.CurrentSound() != -1)
                volume = music.sounds[music.CurrentSound()].Volume;
            source.volume = volume * curve_val;
        }
    }

    public void AddObject(Transform t)
    {
        objects.Add(t);
    }
}

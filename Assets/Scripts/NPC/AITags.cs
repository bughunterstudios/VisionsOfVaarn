using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITags : MonoBehaviour
{
    public List<string> Tags;
    public float vision_range;

    public List<string> tagsinrange;
    private List<Transform> npcsinrange;

    private void Start()
    {
        vision_range = vision_range * vision_range;
        tagsinrange = new List<string>();
        npcsinrange = new List<Transform>();
    }

    public bool TagInRange(string tag)
    {
        return tagsinrange.Contains(tag);
    }

    public Transform TransformInRange(string tag)
    {
        if (tagsinrange.Contains(tag))
            return npcsinrange[tagsinrange.IndexOf(tag)];

        return null;
    }

    public void AddTagInRange(string tag, Transform npc)
    {
        tagsinrange.Add(tag);
        npcsinrange.Add(npc);
    }

    public void ResetTags()
    {
        tagsinrange = new List<string>();
        npcsinrange = new List<Transform>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{
    public Transform player;
    public List<string> player_tags;
    public float ai_range_from_player;
    public float ai_tag_range_from_player;

    public int resetframes;

    private List<AI> ais;
    private List<AI> nearbyais;
    private List<AITags> tags;
    private List<AITags> nearbytags;

    private float player_distance;
    private float distance_apart;
    private int i;
    private int j;

    private int tagindex;

    private static Vector3 vector;
    private static float distanceSquared;

    // Start is called before the first frame update
    void Start()
    {
        ais = new List<AI>();
        nearbyais = new List<AI>();
        tags = new List<AITags>();
        nearbytags = new List<AITags>();

        ai_range_from_player = ai_range_from_player * ai_range_from_player;
        ai_tag_range_from_player = ai_tag_range_from_player * ai_tag_range_from_player;

        ResetNearby();
    }

    private void ResetNearby()
    {
        nearbyais = new List<AI>();
        nearbytags = new List<AITags>();
        for (i = 0; i < ais.Count; i++)
        {
            if (ais[i] == null || tags[i] == null)
            {
                ais.RemoveAt(i);
                tags.RemoveAt(i);
                i--;
                continue;
            }

            player_distance = Distance(player.position, ais[i].transform.position);
            if (ais[i].always_activate || player_distance <= ai_range_from_player)
            {
                nearbyais.Add(ais[i]);
                if (ais[i].animator != null)
                    ais[i].animator.enabled = true;
            }
            else if (ais[i].animator != null)
                ais[i].animator.enabled = false;

            tags[i].ResetTags();
            if (player_distance <= ai_tag_range_from_player)
            {
                nearbytags.Add(tags[i]);

                if (player_distance <= tags[i].vision_range)
                {
                    foreach (string tag in player_tags)
                        tags[i].AddTagInRange(tag, player);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Physics for all nearby ais
        for (i = 0; i < nearbyais.Count; i++)
        {
            if (nearbyais[i] == null)
                continue;

            // Perform physics and behavior
            nearbyais[i].UpdateFrame();
        }

        // Check all other tags in range
        for (j = tagindex + 1; j < nearbytags.Count; j++)
        {
            if (nearbytags[tagindex] == null)
                break;

            if (nearbytags[j] == null)
                continue;

            // Both AIs are in range at this point
            distance_apart = Distance(nearbytags[tagindex].transform.position, nearbytags[j].transform.position);

            if (distance_apart <= nearbytags[tagindex].vision_range)
            {
                foreach (string tag in nearbytags[j].Tags)
                    nearbytags[tagindex].AddTagInRange(tag, nearbytags[j].transform);
            }

            if (distance_apart <= nearbytags[j].vision_range)
            {
                foreach (string tag in nearbytags[tagindex].Tags)
                    nearbytags[j].AddTagInRange(tag, nearbytags[tagindex].transform);
            }
        }
        tagindex++;

        if (resetframes > nearbytags.Count)
        {
            if (tagindex >= resetframes)
            {
                tagindex = 0;
                ResetNearby();
            }
        }
        else if (tagindex >= nearbytags.Count)
        {
            tagindex = 0;
            ResetNearby();
        }
    }

    public void AddAI(AI ai)
    {
        ais.Add(ai);
        tags.Add(ai.GetTag());
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        vector.x = a.x - b.x;
        //vector.y = a.y - b.y;
        vector.z = a.z - b.z;

        return vector.x * vector.x + vector.z * vector.z;

        //distanceSquared = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;

        //return (float)System.Math.Sqrt(distanceSquared);
    }

    /*void OnGUI()
    {
        GUI.Label(new Rect(0, 15, 200, 100), "AIS: " + ais.Count.ToString());
        GUI.Label(new Rect(0, 30, 200, 100), "Tags: " + tags.Count.ToString());
        GUI.Label(new Rect(0, 45, 200, 100), "Nearby AIS: " + nearbyais.Count.ToString());
        GUI.Label(new Rect(0, 60, 200, 100), "Nearby AI Tags: " + nearbytags.Count.ToString());
    }*/
}

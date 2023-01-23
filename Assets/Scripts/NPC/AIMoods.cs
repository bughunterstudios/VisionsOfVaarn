using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mood
{
    public string name;
    public int weight;
    public float min_time;
    public float max_time;
    public string animation;

    public float move;
    public float turn;

    public string tag_in_range;
}

public class AIMoods : MonoBehaviour
{
    public Animator animator;
    public bool moving_mood;
    public List<Mood> moods;
    private AITags aitags;

    private float time;
    private Mood selectedmood;
    private int totalweight;
    private int chosenvalue;

    // Start is called before the first frame update
    void Start()
    {
        time = -1;
        aitags = GetComponent<AITags>();
    }

    public Mood get_SelectedMood()
    {
        return selectedmood;
    }

    public void ResetTime()
    {
        time = 0;
    }

    // Update is called once per frame
    public void UpdateFrame()
    {
        time -= Time.deltaTime;

        if (time <= 0)
        {
            if (animator != null && selectedmood != null)
                animator.SetBool(selectedmood.animation, false);
            SelectRandomMood();
            time = Random.Range(selectedmood.min_time, selectedmood.max_time);
            gameObject.SendMessage(selectedmood.name, SendMessageOptions.DontRequireReceiver);
            if (animator != null)
            {
                if (HasParameter(selectedmood.animation))
                {
                    animator.SetBool(selectedmood.animation, true);
                    animator.SetTrigger(selectedmood.animation);
                }
            }
        }
    }

    private void SelectRandomMood()
    {
        totalweight = 0;
        foreach (Mood mood in moods)
        {
            //Does this mood rely on a tag and is that tag not in range?
            if (mood.tag_in_range != "" && !aitags.TagInRange(mood.tag_in_range))
                continue;
            totalweight += mood.weight;
        }
        chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (Mood mood in moods)
        {
            //Does this mood rely on a tag and is that tag not in range?
            if (mood.tag_in_range != "" && !aitags.TagInRange(mood.tag_in_range))
                continue;
            totalweight += mood.weight;
            if (chosenvalue <= totalweight)
            {
                selectedmood = mood;
                return;
            }
        }
    }

    private bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    public void SetMood(string mood_name)
    {
        foreach (Mood mood in moods)
        {
            if (mood.name == mood_name)
            {
                selectedmood = mood;
                return;
            }
        }
    }
}

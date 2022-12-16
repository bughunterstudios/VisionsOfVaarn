using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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
}

public class AI : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public AimConstraint headaim;

    public float movespeed;
    private float movevelocity;
    public float turnspeed;
    private float turnvelocity;
    public float move_acceleration;
    public float turn_acceleration;

    public List<Mood> moods;

    private float time;
    private float waittime = 0.4f;
    private Mood selectedmood;

    private Transform movingobject;
    private Vector3 previousposition;
    private float previousrotation;

    // Start is called before the first frame update
    void Start()
    {
        if (headaim != null)
        {
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = Camera.main.transform;
            source.weight = 1;
            headaim.AddSource(source);
            headaim.constraintActive = true;
        }
        time = -1;
    }

    private void LateUpdate()
    {
        if (controller != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2, ~LayerMask.GetMask("Ignore Raycast")))
            {
                if (hit.transform == movingobject)
                {
                    controller.Move(movingobject.position - previousposition);
                    transform.Rotate(Vector3.up * (movingobject.eulerAngles.y - previousrotation));
                }
                else
                    movingobject = hit.transform;
                previousposition = movingobject.position;
                previousrotation = movingobject.eulerAngles.y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedmood != null)
        {
            if (selectedmood.move != 0 && selectedmood.turn == 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + (transform.up * 1), transform.TransformDirection(Vector3.forward), out hit, 4, ~LayerMask.GetMask("Ignore Raycast")))
                {
                    time = 0;
                }
            }
        }

        time -= Time.deltaTime;
        waittime -= Time.deltaTime;

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

        if (selectedmood != null)
        {
            movevelocity = Mathf.Lerp(movevelocity, selectedmood.move * movespeed, move_acceleration * Time.deltaTime);
            turnvelocity = Mathf.Lerp(turnvelocity, selectedmood.turn * turnspeed, turn_acceleration * Time.deltaTime);
        }

        if (controller != null)
        {
            Vector3 moveDirection = controller.gameObject.transform.forward * movevelocity * Time.deltaTime;
            if (waittime < 0)
                moveDirection.y -= 20 * Time.deltaTime;
            controller.Move(moveDirection);

            controller.gameObject.transform.Rotate(Vector3.up * turnvelocity * Time.deltaTime);
        }
        else
        {
            transform.position += transform.forward * movevelocity * Time.deltaTime;
            transform.Rotate(Vector3.up * turnvelocity * Time.deltaTime);
        }

        if (headaim != null)
        {
            //float dotproduct = (Vector3.Dot(controller.gameObject.transform.forward, Camera.main.transform.forward) + 1f) / 2f;
            //headaim.weight = 1 - dotproduct;
            Vector3 relativePos = Camera.main.transform.position - transform.position;
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(relativePos, forward);
            headaim.weight = 1f - (angle / 180f);
        }
    }

    private void SelectRandomMood()
    {
        int totalweight = 0;
        foreach (Mood mood in moods)
        {
            totalweight += mood.weight;
        }
        int chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (Mood mood in moods)
        {
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

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

    public string tag_in_range;
}

public class AI : MonoBehaviour
{
    public CharacterController controller;
    public Rigidbody rb;
    public Animator animator;
    public AimConstraint headaim;

    public float movespeed;
    private float movevelocity;
    public float turnspeed;
    private float turnvelocity;
    public float move_acceleration;
    public float turn_acceleration;

    public List<Mood> moods;

    private AITags aitags;
    public bool always_activate;

    private float time;
    private float waittime = 0.4f;
    private Mood selectedmood;

    private Transform movingobject;
    private Vector3 previousposition;
    private float previousrotation;

    private Transform npc;
    private RaycastHit hit;
    private Vector3 targetDirection;
    private Quaternion targetRotation;
    private Quaternion rotateTowards;
    private float turn;
    private Vector3 moveDirection;
    private int totalweight;
    private int chosenvalue;
    private Vector3 forwardvelocity;
    private Vector3 relativePos;
    private Vector3 forward;
    private float angle;
    private Transform cam;

    public float forward_ray_start = 1;
    public float forward_ray_length = 3;
    public float stand_ray_start = 1;
    public float stand_ray_length = 1;
    private Vector3 stand_ray_pos;
    private Vector3 forward_ray_pos;
    private int layer_mask;

    private bool forwardintersect;
    private bool downintersect;

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
        cam = Camera.main.transform;
        aitags = GetComponent<AITags>();
        layer_mask = ~LayerMask.GetMask("Ignore Raycast");

        stand_ray_pos = transform.position + (transform.TransformDirection(Vector3.down) * stand_ray_start);
        forward_ray_pos = transform.position + (transform.up * 1) + (transform.TransformDirection(Vector3.forward) * forward_ray_start);

        GameObject.Find("TheWorld").GetComponent<AIControl>().AddAI(this);
    }

    /*private void OnDrawGizmos()
    {
        stand_ray_pos = transform.position + (transform.TransformDirection(Vector3.down) * stand_ray_start);
        forward_ray_pos = transform.position + (transform.up * 1) + (transform.TransformDirection(Vector3.forward) * forward_ray_start);
        Gizmos.color = downintersect ? Color.red : Color.white;
        Gizmos.DrawRay(stand_ray_pos, transform.TransformDirection(Vector3.down) * stand_ray_length);
        Gizmos.color = forwardintersect ? Color.red : Color.white;
        Gizmos.DrawRay(forward_ray_pos, transform.TransformDirection(Vector3.forward) * forward_ray_length);
    }//*/

    public void UpdateFrame()
    {
        //Check for following moving object
        if (controller != null && movespeed > 0)
        {
            if (Physics.Raycast(stand_ray_pos, Vector3.down, out hit, stand_ray_length, layer_mask))
            {
                if (hit.transform == movingobject)
                {
                    if (movingobject.position != previousposition)
                        Debug.Log("Moving: " + name);
                    controller.Move(movingobject.position - previousposition);
                    transform.Rotate(Vector3.up * (movingobject.eulerAngles.y - previousrotation));
                }
                else
                    movingobject = hit.transform;
                previousposition = movingobject.position;
                previousrotation = movingobject.eulerAngles.y;
                downintersect = true;
            }
            else
                downintersect = false;
        }

        if (selectedmood != null)
        {
            if (selectedmood.move != 0 && selectedmood.turn == 0 && movespeed > 0 && selectedmood.tag_in_range == "")
            {
                if (Physics.Raycast(forward_ray_pos, transform.TransformDirection(Vector3.forward), out hit, forward_ray_length, layer_mask))
                {
                    if (hit.collider.gameObject != this.gameObject && this.gameObject.name == "SandwormHead")
                        Debug.Log("hit something else");
                    time = 0;
                    forwardintersect = true;
                }
                else
                    forwardintersect = false;
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
            turn = selectedmood.turn;
            if (selectedmood.tag_in_range != "")
            {
                npc = aitags.TransformInRange(selectedmood.tag_in_range);
                if (npc != null)
                {
                    targetDirection = (npc.position - transform.position).normalized;
                    targetRotation = Quaternion.LookRotation(targetDirection * (selectedmood.turn > 0 ? 1f : -1f));
                    rotateTowards = Quaternion.RotateTowards(transform.rotation, targetRotation, Mathf.Abs(selectedmood.turn));
                    turn = Mathf.Clamp(rotateTowards.eulerAngles.y - transform.eulerAngles.y, -Mathf.Abs(selectedmood.turn), Mathf.Abs(selectedmood.turn));
                }
            }
            turnvelocity = Mathf.Lerp(turnvelocity, turn * turnspeed, turn_acceleration * Time.deltaTime);
        }

        if (controller != null)
        {
            moveDirection = controller.gameObject.transform.forward * movevelocity * Time.deltaTime;
            if (waittime < 0)
                moveDirection.y -= 20 * Time.deltaTime;
            controller.Move(moveDirection);

            controller.gameObject.transform.Rotate(Vector3.up * turnvelocity * Time.deltaTime);
        }
        else if (rb != null)
        {
            if (movevelocity != 0)
            {
                forwardvelocity = transform.forward * movevelocity * Time.deltaTime;
                rb.velocity = new Vector3(forwardvelocity.x, rb.velocity.y, forwardvelocity.z);
            }
            if (turnvelocity != 0)
                rb.angularVelocity = new Vector3(0, turnvelocity * Time.deltaTime, 0);
            rb.MoveRotation(Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0));
        }
        else
        {
            transform.position += transform.forward * movevelocity * Time.deltaTime;
            transform.Rotate(Vector3.up * turnvelocity * Time.deltaTime);
        }

        if (headaim != null)
        {
            relativePos = cam.position - transform.position;
            forward = transform.forward;
            angle = Vector3.Angle(relativePos, forward);
            headaim.weight = 1f - (angle / 180f);
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

    /*private void OnTriggerEnter(Collider other)
    {
        if (!npcsinrange.Contains(other.gameObject))
        {
            if (other.tag == "Player")
            {
                tagsinrange.Add("Player");
                npcsinrange.Add(other.gameObject);
                return;
            }

            if (other.GetComponent<AI>())
            {
                tagsinrange.Add(other.GetComponent<AI>().tag);
                npcsinrange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (npcsinrange.Contains(other.gameObject))
        {
            tagsinrange.RemoveAt(npcsinrange.IndexOf(other.gameObject));
            npcsinrange.Remove(other.gameObject);
        }
    }*/
}

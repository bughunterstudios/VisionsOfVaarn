using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AI : MonoBehaviour
{
    public CharacterController controller;
    public Rigidbody rb;
    public Animator animator;
    public AimConstraint headaim;
    public GameObject normal_surface_child;

    public float movespeed;
    private float movevelocity;
    public float turnspeed;
    private float turnvelocity;
    public float move_acceleration;
    public float turn_acceleration;

    private AITags aitags;
    public bool always_activate;

    private float waittime = 0.4f;
    private AIMoods[] moods;
    private AIMoods moving_mood;
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
    private int moving_object_mask;

    private bool forwardintersect;
    private bool downintersect;

    private Vector3 normal;

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
        moods = GetComponents<AIMoods>();
        foreach (AIMoods mood in moods)
        {
            if (mood.moving_mood)
                moving_mood = mood;
        }
        cam = Camera.main.transform;
        aitags = GetComponent<AITags>();
        layer_mask = ~LayerMask.GetMask("Ignore Raycast");
        moving_object_mask = layer_mask;//LayerMask.GetMask("MovingObject");

        stand_ray_pos = transform.position + (transform.TransformDirection(Vector3.down) * stand_ray_start);
        forward_ray_pos = transform.position + (transform.up * 1) + (transform.TransformDirection(Vector3.forward) * forward_ray_start);

        GameObject.Find("TheWorld").GetComponent<AIControl>().AddAI(this);
    }

    public AITags GetTag()
    {
        return aitags;
    }

    private void OnDrawGizmos()
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
        if (controller != null && movespeed > 0 && stand_ray_length > 0)
        {
            stand_ray_pos = transform.position + (transform.TransformDirection(Vector3.down) * stand_ray_start);
            if (Physics.Raycast(stand_ray_pos, Vector3.down, out hit, stand_ray_length, moving_object_mask))
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
                downintersect = true;

                if (normal_surface_child != null)
                {
                    normal = hit.normal;
                }
            }
            else
                downintersect = false;
        }

        if (normal_surface_child != null)
        {
            normal_surface_child.transform.localRotation = Quaternion.Slerp(normal_surface_child.transform.localRotation, Quaternion.FromToRotation(transform.up, normal), Time.deltaTime);
        }

        waittime -= Time.deltaTime;

        foreach (AIMoods mood in moods)
            mood.UpdateFrame();

        selectedmood = moving_mood.get_SelectedMood();

        if (selectedmood != null)
        {
            if (selectedmood.move != 0 && selectedmood.turn == 0 && movespeed > 0 && selectedmood.tag_in_range == "")
            {
                forward_ray_pos = transform.position + (transform.up * 1) + (transform.TransformDirection(Vector3.forward) * forward_ray_start);
                if (Physics.Raycast(forward_ray_pos, transform.TransformDirection(Vector3.forward), out hit, forward_ray_length, layer_mask))
                {
                    // // Set mood time
                    moving_mood.ResetTime();
                    forwardintersect = true;
                }
                else
                    forwardintersect = false;
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
}

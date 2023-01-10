using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumblingDroneScript : MonoBehaviour
{
    public CharacterController controller;
    public AI ai;
    public Rigidbody rb;
    public Animator animator;
    public List<GameObject> colliders;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ai.SendMessage("SetMood", "fall", SendMessageOptions.DontRequireReceiver);
            controller.enabled = false;
            rb.constraints = RigidbodyConstraints.None;
            ai.enabled = false;
            animator.SetTrigger("Fall");
            foreach (GameObject col in colliders)
                col.SetActive(true);
        }
    }
}

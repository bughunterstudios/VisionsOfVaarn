using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumblingDroneScript : MonoBehaviour
{
    public CharacterController controller;
    public AI ai;
    public Animator animator;
    public List<GameObject> colliders;
    private bool done;

    private void OnTriggerEnter(Collider other)
    {
        if (!done)
        {
            if (other.tag == "Player")
            {
                ai.SendMessage("SetMood", "fall", SendMessageOptions.DontRequireReceiver);
                Rigidbody rb = controller.gameObject.AddComponent<Rigidbody>();
                rb.mass = 10;
                controller.enabled = false;
                ai.enabled = false;
                animator.SetTrigger("Fall");
                foreach (GameObject col in colliders)
                    col.SetActive(true);
                done = true;
            }
        }
    }
}

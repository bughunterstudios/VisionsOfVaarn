using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float movespeed;
    public float sprint;
    public float friction;

    public float mousesensitivity;
    public float joysticklooksensitivity;
    public float verticalviewangle;
    public Transform camholder;
    private float rotationX;

    private CharacterController controller;

    private Transform movingobject;
    private Vector3 previousposition;
    private float previousrotation;

    private float ground_angle;
    private Vector3 ground_normal;
    private Vector3 velocity;
    private float stamina;
    private float stamina_loss_multiplier;
    private float stamina_loss_sprint_multiplier;
    private bool slipping;

    private bool locked;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        LockCursor();
    }

    private void LateUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.2f, ~LayerMask.GetMask("Ignore Raycast")))
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


            ground_normal = hit.normal;
            ground_angle = Quaternion.Angle(Quaternion.FromToRotation(transform.up, hit.normal), Quaternion.identity);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(400, 0, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        locked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        locked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (locked)
        {
            if (Input.GetAxis("Joy X") != 0)
                gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Joy X") * joysticklooksensitivity * Time.deltaTime);
            else
                gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mousesensitivity * Time.deltaTime);

            if (Input.GetAxis("Joy Y") != 0)
                rotationX += -Input.GetAxis("Joy Y") * joysticklooksensitivity * Time.deltaTime;
            else
                rotationX += -Input.GetAxis("Mouse Y") * mousesensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -verticalviewangle, verticalviewangle);
            camholder.localRotation = Quaternion.Euler(rotationX, 0, 0);

            //Debug.Log(Physics.CheckSphere(transform.position + (Vector3.down * 0.5f), 0.6f, ~LayerMask.GetMask("Ignore Raycast")));
            if (Physics.CheckSphere(transform.position + (Vector3.down * 0.5f), 0.6f, ~LayerMask.GetMask("Ignore Raycast")))
            {
                Vector3 moveDirection = gameObject.transform.forward * Input.GetAxis("Vertical");
                moveDirection += gameObject.transform.right * Input.GetAxis("Horizontal");


                //if (Input.GetKey(KeyCode.LeftControl))
                //    moveDirection += new Vector3(ground_normal.x, 0, ground_normal.z);

                //moveDirection.Normalize();
                //moveDirection *= (movespeed + (Input.GetAxis("Sprint") != 0 ? sprint : 0)) * Time.deltaTime;
                //moveDirection.y -= 20 * Time.deltaTime;

                moveDirection += new Vector3(ground_normal.x, 0, ground_normal.z) * (ground_angle / 60f) * moveDirection.magnitude;
                velocity += moveDirection * (movespeed + (Input.GetAxis("Sprint") != 0 ? sprint : 0)) * Time.deltaTime;
                velocity *= friction;
            }
            velocity += Physics.gravity * Time.deltaTime;

            controller.Move(velocity);
        }
    }
}

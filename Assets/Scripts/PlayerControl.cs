using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float movespeed;
    public float sprint;

    public Texture2D Circle;

    public float center;

    public float mousesensitivity;
    public float verticalviewangle;
    public Transform camholder;
    private float rotationX;

    private CharacterController controller;

    private Transform movingobject;
    private Vector3 previousposition;
    private float previousrotation;

    private bool locked;
    private bool wp = Application.platform == RuntimePlatform.WebGLPlayer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void LateUpdate()
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

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            locked = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.SetCursor(Circle, new Vector2(center, center), CursorMode.Auto);
            Cursor.visible = true;
            locked = false;
        }

        if (locked)
        {
            gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mousesensitivity * Time.deltaTime * (wp ? 0.1f : 1f));
            rotationX += -Input.GetAxis("Mouse Y") * mousesensitivity * Time.deltaTime * (wp ? 0.1f : 1f);
            rotationX = Mathf.Clamp(rotationX, -verticalviewangle, verticalviewangle);
            camholder.localRotation = Quaternion.Euler(rotationX, 0, 0);


            Vector3 moveDirection = gameObject.transform.forward * Input.GetAxis("Vertical");
            moveDirection += gameObject.transform.right * Input.GetAxis("Horizontal");
            //moveDirection.Normalize();
            moveDirection *= (movespeed + (Input.GetKey(KeyCode.LeftShift) ? sprint : 0)) * Time.deltaTime;
            moveDirection.y -= 20 * Time.deltaTime;
            controller.Move(moveDirection);
        }
    }
}

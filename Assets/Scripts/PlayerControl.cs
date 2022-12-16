using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float rotatespeed;
    public float movespeed;
    public float sprint;

    public Texture2D UpArrow;
    public Texture2D DownArrow;
    public Texture2D LeftArrow;
    public Texture2D RightArrow;
    public Texture2D Circle;

    public float center;

    public bool mouselook;

    public float mousesensitivity;
    public float verticalviewangle;
    public Transform camholder;
    private float rotationX;

    private Vector2 hotspot = new Vector2(16f, 16f);
    private bool held;

    private CharacterController controller;

    private Transform movingobject;
    private Vector3 previousposition;
    private float previousrotation;

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
        if (mouselook)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            bool wp = Application.platform == RuntimePlatform.WebGLPlayer;
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
        else
        {
            gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotatespeed * Time.deltaTime);
            Vector3 moveDirection = gameObject.transform.forward * Input.GetAxis("Vertical") * (movespeed + (Input.GetKey(KeyCode.LeftShift) ? sprint : 0)) * Time.deltaTime;
            moveDirection.y -= 20 * Time.deltaTime;
            controller.Move(moveDirection);

            Vector3 mousePos = Input.mousePosition;
            float x = ((Mathf.Clamp(mousePos.x, 0, Screen.width) / Screen.width) - 0.5f) * 2f;
            float y = ((Mathf.Clamp(mousePos.y, 0, Screen.height) / Screen.height) - 0.5f) * 2f;

            if (held)
            {
                gameObject.transform.Rotate(Vector3.up * x * rotatespeed * Time.deltaTime);
                moveDirection = gameObject.transform.forward * y * movespeed * Time.deltaTime;
                moveDirection.y -= 20 * Time.deltaTime;
                controller.Move(moveDirection);
            }

            if (x >= -center && x <= center && y >= -center && y <= center)
            {
                Cursor.SetCursor(Circle, hotspot, CursorMode.Auto);
            }
            else
            {
                if (y > center)
                {
                    Cursor.SetCursor(UpArrow, hotspot, CursorMode.Auto);
                }
                else if (y < -center)
                {
                    Cursor.SetCursor(DownArrow, hotspot, CursorMode.Auto);
                }
                else
                {
                    if (x > center)
                    {
                        Cursor.SetCursor(RightArrow, hotspot, CursorMode.Auto);
                    }
                    else if (x < -center)
                    {
                        Cursor.SetCursor(LeftArrow, hotspot, CursorMode.Auto);
                    }
                }

                if (Input.GetMouseButtonDown(0))
                    held = true;
            }

            if (!Input.GetMouseButton(0))
                held = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEscapeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }
}

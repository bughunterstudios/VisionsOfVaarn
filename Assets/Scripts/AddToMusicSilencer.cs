using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToMusicSilencer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.GetComponentInChildren<MusicSilencer>().AddObject(transform);
    }
}

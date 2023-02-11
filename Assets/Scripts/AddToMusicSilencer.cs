using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToMusicSilencer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Sounds").GetComponentInChildren<MusicSilencer>().AddObject(transform);
    }
}

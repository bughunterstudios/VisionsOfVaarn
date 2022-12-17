using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineBreakAnimate : MonoBehaviour
{
    [SerializeField]
    private float fade_time = 0;
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private List<Sprite> sprites = null;
    [SerializeField]
    private float speed = 1;

    private float time;
    private float animatetime;

    private bool fadeout;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        animatetime += Time.deltaTime;
        if (time <= fade_time && !fadeout)
            time += Time.deltaTime;
        else if (time >= 0 && fadeout)
            time -= Time.deltaTime;
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, time / fade_time));
        image.sprite = sprites[(int)(animatetime * 10) % sprites.Count];
    }

    public void FadeOut()
    {
        fadeout = true;
    }

    public bool DoneFading()
    {
        return time <= 0 && fadeout;
    }
}

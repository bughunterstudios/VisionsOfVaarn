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

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, time / fade_time));
        //image.sprite = sprites[(int)Mathf.Abs((GetComponent<RectTransform>().position.y * speed) + 1000) % sprites.Count];
        image.sprite = sprites[(int)(time * 10) % sprites.Count];
    }
}

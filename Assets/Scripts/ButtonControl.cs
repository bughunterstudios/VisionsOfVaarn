using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float fade_time = 0;

    private float time;
    [SerializeField]
    private Image button;
    [SerializeField]
    private Image button_layer;
    private TextMeshProDisolveIn text;

    private bool fadeout;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProDisolveIn>();
        time = 0;
        fadeout = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (time <= fade_time || fadeout)
        {
            if (fadeout)
                time -= Time.deltaTime;
            else
                time += Time.deltaTime;
            button.color = new Color(button.color.r, button.color.g, button.color.b, Mathf.Lerp(0, 1, time / fade_time));
            button_layer.color = new Color(button_layer.color.r, button_layer.color.g, button_layer.color.b, Mathf.Lerp(0, 1, time / fade_time));
        }
    }

    public void SetText(string text)
    {
        GetComponentInChildren<TextMeshPro>().text = text;
    }

    public void FadeOut()
    {
        fadeout = true;
        text.FadeOut();
    }

    public bool DoneFading()
    {
        return time <= 0 && fadeout;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.Deselect();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarPercentage : MonoBehaviour
{
    public float percentage;
    public float min_percentage;
    public float max_percentage;
    public RectTransform bar;

    private float width;

    // Start is called before the first frame update
    void Start()
    {
        width = bar.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        bar.offsetMax = new Vector2((1f - Mathf.Lerp(min_percentage, max_percentage, percentage)) * -width, bar.offsetMax.y);
    }
}

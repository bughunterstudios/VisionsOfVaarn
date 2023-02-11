using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    private int max_health;
    public HealthBarPercentage bar;
    public PostProcessVolume volume;
    public float vignette_speed;
    public float vignette_intensity;
    private float vignette_target;
    private Vignette vignette;

    private void Start()
    {
        max_health = health;
        volume.profile.TryGetSettings(out vignette);
    }

    private void Update()
    {
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, vignette_target, vignette_speed * Time.deltaTime);
        if (vignette_target != 0 && vignette.intensity.value >= vignette_target - 0.01f)
            vignette_target = 0;
    }

    public void Damage(int value, Transform attacker)
    {
        health -= value;

        bar.percentage = (float) health / (float) max_health;

        vignette_target = vignette_intensity;
        //vignette.center.value = new Vector2();

        if (health <= 0)
        {
            health = max_health;
        }
    }
}

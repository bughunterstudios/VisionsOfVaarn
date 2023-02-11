using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public Gradient fog;
    public Gradient environment;

    public ParticleSystem stars;
    private ParticleSystem.Particle[] star_particles;
    public ParticleSystem satelites;
    private ParticleSystem.Particle[] satelite_particles;

    public Gradient moon_tint;
    public SpriteRenderer moon;
    public Transform moon_holder;

    public float timespeed;

    private Light light;
    private float max_intensity;

    private bool transition;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        max_intensity = light.intensity;

        StartCoroutine(GetParticles());
    }

    IEnumerator GetParticles()
    {
        yield return new WaitForSeconds(0.1f);

        if (stars != null)
        {
            star_particles = new ParticleSystem.Particle[stars.particleCount];
            stars.GetParticles(star_particles);
        }

        if (satelites != null)
        {
            satelite_particles = new ParticleSystem.Particle[satelites.particleCount];
            satelites.GetParticles(satelite_particles);
        }

        transition = true;
    }

        // Update is called once per frame
    void Update()
    {
        float dotproduct = (Vector3.Dot(transform.forward, Vector3.down) + 1f) / 2f;
        if (dotproduct > 0.6f && transition) //Day
        {
            light.intensity = max_intensity;
            light.shadowNormalBias = 0.5f;
            RenderSettings.fogColor = fog.Evaluate(1);
            RenderSettings.ambientLight = environment.Evaluate(1);
            if (star_particles != null)
            {
                for (int i = 0; i < star_particles.Length; i++)
                    star_particles[i].startColor = new Color(1, 1, 1, 0);
                stars.SetParticles(star_particles, star_particles.Length);
            }
            if (satelite_particles != null)
            {
                for (int i = 0; i < satelite_particles.Length; i++)
                    satelite_particles[i].startColor = new Color(1, 1, 1, 0.2f);
                satelites.SetParticles(satelite_particles, satelite_particles.Length);
            }
            moon.color = moon_tint.Evaluate(1);

            transition = false;
        }
        else if (dotproduct < 0.4f && transition) //Night
        {
            light.intensity = 0.01f;
            light.shadowNormalBias = 1f;
            RenderSettings.fogColor = fog.Evaluate(0);
            RenderSettings.ambientLight = environment.Evaluate(0);
            if (star_particles != null)
            {
                for (int i = 0; i < star_particles.Length; i++)
                    star_particles[i].startColor = new Color(1, 1, 1, 1);
                stars.SetParticles(star_particles, star_particles.Length);
            }
            if (satelite_particles != null)
            {
                for (int i = 0; i < satelite_particles.Length; i++)
                    satelite_particles[i].startColor = new Color(1, 1, 1, 1);
                satelites.SetParticles(satelite_particles, satelite_particles.Length);
            }
            moon.color = moon_tint.Evaluate(0);

            transition = false;
        }
        else if (dotproduct <= 0.6f && dotproduct >= 0.4f) //Dusk/Dawn
        {
            float duskscale = (dotproduct - 0.4f) * 5f;
            light.intensity = Mathf.Lerp(0.01f, max_intensity, duskscale);
            light.shadowNormalBias = Mathf.Lerp(1f, 0.5f, duskscale);
            RenderSettings.fogColor = fog.Evaluate(duskscale);
            RenderSettings.ambientLight = environment.Evaluate(duskscale);
            if (star_particles != null)
            {
                for (int i = 0; i < star_particles.Length; i++)
                    star_particles[i].startColor = new Color(1, 1, 1, 1 - duskscale);
                stars.SetParticles(star_particles, star_particles.Length);
            }
            if (satelite_particles != null)
            {
                for (int i = 0; i < satelite_particles.Length; i++)
                    satelite_particles[i].startColor = new Color(1, 1, 1, Mathf.Lerp(1, 0.2f, duskscale));
                satelites.SetParticles(satelite_particles, satelite_particles.Length);
            }
            moon.color = moon_tint.Evaluate(duskscale);

            transition = true;
        }

        float eclipseproduct = (Vector3.Dot(transform.forward, moon_holder.forward) + 1f) / 2f;
        if (eclipseproduct >= 0.99f && dotproduct > 0.6f)
        {
            float eclipsescale = (eclipseproduct - 0.99f) * 100f;
            light.intensity = Mathf.Lerp(max_intensity, 0.01f, eclipsescale);
            light.shadowNormalBias = Mathf.Lerp(0.5f, 1f, eclipsescale);
            RenderSettings.fogColor = fog.Evaluate(1 - eclipsescale);
            RenderSettings.ambientLight = environment.Evaluate(1 - eclipsescale);
        }

        if (Input.GetKey(KeyCode.T))
            Time.timeScale = timespeed;
        else
            Time.timeScale = 1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomClip
{
    public int Weight;
    public AudioClip Clip;
    public float Volume;
}

public class RandomSound : MonoBehaviour
{
    public List<RandomClip> sounds;

    private int lastsound;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        Restart();
    }

    public void Restart()
    {
        lastsound = -1;
    }

    public void PlaySound(int index)
    {
        source.clip = sounds[index].Clip;
        source.volume = sounds[index].Volume;
        source.Play();
        lastsound = index;
    }

    public int CurrentSound()
    {
        return lastsound;
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying && source.loop == false)
        {
            NewSound();
        }
    }

    private void NewSound()
    {
        if (sounds.Count > 0)
        {
            int chosen = lastsound;
            while (chosen == lastsound)
            {
                int totalweight = 0;
                foreach (RandomClip clip in sounds)
                {
                    totalweight += clip.Weight;
                }
                int chosenvalue = Random.Range(1, totalweight + 1);
                totalweight = 0;
                chosen = 0;
                foreach (RandomClip clip in sounds)
                {
                    totalweight += clip.Weight;
                    if (chosenvalue <= totalweight)
                    {
                        break;
                    }
                    chosen++;
                }
            }
            PlaySound(chosen);
        }
    }

    public void Generate(Seed seed)
    {
        Random.InitState(seed.seed);
        source = GetComponent<AudioSource>();
        lastsound = -1;
        NewSound();
    }
}

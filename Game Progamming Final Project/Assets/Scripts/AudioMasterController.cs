using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMasterController : MonoBehaviour
{
    [HideInInspector]
    public static AudioMasterController instance;

    [SerializeField] List<Sound> sounds;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    Sound findSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name.Equals(name))
                return s;
        }

        Debug.LogError("Sound " + name + " not found!");
        return null;

    }

    public void playSound(string name, GameObject g)
    {
        Sound s = findSound(name);

        GameObject p = new GameObject();

        p.transform.SetParent(g.transform);
        p.name = "audio player for " + name;

        AudioSource source = p.AddComponent<AudioSource>();

        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch * Random.Range(1f - s.randomPitchMult, 1f + s.randomPitchMult);

        source.Play();

        Destroy(p, s.clip.length);
    }

    public void playSound(string name)
    {
        playSound(name, gameObject);
    }
}

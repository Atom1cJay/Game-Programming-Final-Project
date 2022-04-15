using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Sound", menuName = "ScriptableObjects/Sound", order = 1)]
public class Sound : ScriptableObject
{
    public string name;
    public AudioClip clip;
    public float volume, pitch, randomPitchMult;
}

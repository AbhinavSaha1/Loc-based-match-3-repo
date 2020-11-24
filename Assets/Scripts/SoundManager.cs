using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoice;

    public void PlayRandomDestroyNoise()
    {
        int clipToPlay= Random.Range(0, destroyNoice.Length);
        destroyNoice[clipToPlay].Play();
    }
}

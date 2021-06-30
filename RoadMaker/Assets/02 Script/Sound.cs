using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    AudioSource audio;
    public AudioClip[] effectSound;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void playSound(int soundNum)
    {
        audio.clip = effectSound[soundNum];
        audio.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{

    public List<AudioClip> Clips;
    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayerOnShot(int s)
    {
        source.PlayOneShot(Clips[s]);
    }
}

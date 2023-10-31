using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource clicksAudioSource;
    [SerializeField] AudioClip clickButtonClip;
    [SerializeField] AudioClip clickResourceClip;

    public void PlayButtonClick()
    {
        clicksAudioSource.PlayOneShot(clickButtonClip);
    }

    public void PlayResourceClick()
    {
        clicksAudioSource.PlayOneShot(clickResourceClip);
    }
}

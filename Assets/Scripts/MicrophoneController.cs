using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(AudioListener))]
public class MicrophoneController : MonoBehaviour
{
    AudioSource audioSource;
    // Start recording with built-in Microphone and play the recorded audio right away
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
        audioSource.loop = true;
        while(!(Microphone.GetPosition(null) > 0)) {}
        audioSource.Play();
    }
}
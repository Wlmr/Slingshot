using UnityEngine;
using System.Collections;
using System;


public class SatifactorySounds : MonoBehaviour, AudioProcessor.AudioCallbacks {

   // public AudioProcessor processor;
    public AudioClip[] toner;
    public AudioSource[] players;
    private bool awaitingBeat;
    public PlayerWithGravity playerWithGravitySC;

    // Use this for initialization
    void Start () {
    
        awaitingBeat = false;
     //   processor.addAudioCallback(this);
    }

    public void PrepareRandom() {
        //if (playerWithGravitySC.RadRadBro()) {
            for (int i = 0; i < 3; i++) {
                players[i].clip = toner[UnityEngine.Random.Range(0, 15)];
                players[i].Play();
            }
            awaitingBeat = true;
        //}
    }

    public void onOnbeatDetected() {
        if (awaitingBeat ) {
            for (int i = 0; i < 3; i++) {
               players[i].Play();
            }
            awaitingBeat = false;
        }
    }

    public void onSpectrum(float[] spectrum) {
        
    }
}

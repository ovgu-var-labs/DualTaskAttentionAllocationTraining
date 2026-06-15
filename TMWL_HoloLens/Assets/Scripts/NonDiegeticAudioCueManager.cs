using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NonDiegeticAudioCueManager : CueBaseManager
{
    [SerializeField]
    private AudioClip[] highlightSounds = new AudioClip[10];
    [SerializeField]
    private AudioSource nonDiegeticAudioSource;

    private int totalCueTime;

    private int highlightDistanceFromEnd = 0;
    private int highlightLength = 5;
    private bool soundPlaying;
 
    public override void UpdateCue(Vector3 _targetPosition, float _timeValue)
    {
        transform.position = _targetPosition;

        if (soundPlaying) 
        {
            nonDiegeticAudioSource.volume = math.remap(totalCueTime - highlightDistanceFromEnd - highlightLength, totalCueTime, 0, 0.5f, _timeValue);
        }
        else if (_timeValue > totalCueTime - highlightDistanceFromEnd - highlightLength)
        {
            nonDiegeticAudioSource.clip = highlightSounds[UnityEngine.Random.Range(0, highlightSounds.Length)];
            nonDiegeticAudioSource.Play();
            nonDiegeticAudioSource.volume = 0;

            soundPlaying = true;
        }
       
    }

    public override void SetupCue(int _totalTime)
    {
        totalCueTime = _totalTime;
    }

    public override void ResetCue()
    {
        soundPlaying = false;
        nonDiegeticAudioSource.gameObject.SetActive(false);
    }

    public override void StartCue()
    {
        soundPlaying = false;
        nonDiegeticAudioSource.gameObject.SetActive(true);
    }

    public override void EndCue()
    {
        ResetCue();
    }
}

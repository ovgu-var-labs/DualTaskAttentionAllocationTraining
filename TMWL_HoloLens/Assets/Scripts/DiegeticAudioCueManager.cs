using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DiegeticAudioCueManager : CueBaseManager
{
    [SerializeField]
    private AudioClip[] backgroundBreathing = new AudioClip[10];
    [SerializeField]
    private AudioClip[] highlightSounds = new AudioClip[10];
    [SerializeField]
    private AudioSource diegeticAudioSource;

    private int totalCueTime;

    private bool backgroundBreathingRunning = false;
    private float backgroundBreathingRunningTime;

    private int highlightDistanceFromEnd = 1;
    private int backgroundBreathingLength = 7;
    private int highlightLength = 1;
    private int minBreakTime = 1;

    private float currentBreakTime = 1;
    private float breakTimeRunningTime = 0;
    private bool breakTimeRunning;

    private bool limboPhase = false;

    public override void UpdateCue(Vector3 _targetPosition, float _timeValue)
    {
        transform.position = _targetPosition;

        if (backgroundBreathingRunning)
        {
            backgroundBreathingRunningTime += Time.deltaTime;

            if (backgroundBreathingRunningTime > backgroundBreathingLength)
            {
                backgroundBreathingRunning = false;

                float paddingTime = totalCueTime - _timeValue - backgroundBreathingLength - highlightLength;

                if (paddingTime < (backgroundBreathingLength + (2 * minBreakTime)))
                {   // if no additional breathing cycle is possible until the highlight sound
                    currentBreakTime = paddingTime;
                }
                else if (paddingTime < (2 * backgroundBreathingLength + (3 * minBreakTime)))
                {   // if the rest time is long enough to have one additional breathing cycle (common case)
                    currentBreakTime = 0.5f * (paddingTime - backgroundBreathingLength);
                }
                else
                {   // if the rest time is long enough to have two breathing cycles with min padding
                    currentBreakTime = 0.333f * (paddingTime - (2 * backgroundBreathingLength));
                }

                breakTimeRunning = true;
            }
        }
        else if (breakTimeRunning)
        {
            breakTimeRunningTime += Time.deltaTime;

            if (breakTimeRunningTime > currentBreakTime)
            {
                breakTimeRunning = false;

                if (_timeValue > (totalCueTime - highlightDistanceFromEnd - highlightLength))
                {
                    StartHighlightSounds();
                }
                else if ((totalCueTime - _timeValue) > (backgroundBreathingLength + minBreakTime))
                {
                    StartBackgroundBreathing();
                }
                else
                {
                    limboPhase = true;
                }
            }
        }
        else if (limboPhase)
        {
            if (_timeValue > (totalCueTime - highlightDistanceFromEnd - highlightLength))
            {
                StartHighlightSounds();

                limboPhase = false;
            }
        }
    }

    public override void SetupCue(int _totalTime)
    {
        totalCueTime = _totalTime;
        currentBreakTime = minBreakTime;
    }

    public override void ResetCue()
    {
        StartCoroutine(StopBreathing());
        
    }

    public override void StartCue()
    {
        StartCoroutine(StartBreathing());
    }

    private void StartBackgroundBreathing()
    {
        diegeticAudioSource.gameObject.SetActive(true);
        diegeticAudioSource.clip = backgroundBreathing[Random.Range(0, backgroundBreathing.Length)];
        diegeticAudioSource.volume = 0.7f;
        diegeticAudioSource.Play();

        backgroundBreathingRunningTime = 0f;
        backgroundBreathingRunning = true;
    }

    private void StartHighlightSounds()
    {
        diegeticAudioSource.clip = highlightSounds[Random.Range(0, highlightSounds.Length)];
        diegeticAudioSource.volume = 1;
        diegeticAudioSource.Play();
    }

    private IEnumerator StopBreathing()
    {
        while (backgroundBreathingRunning)
        {
            yield return null;
        }

        backgroundBreathingRunningTime = 0;
        currentBreakTime = 0;
        backgroundBreathingRunning = false;
        breakTimeRunning = false;
        limboPhase = false;
        diegeticAudioSource.gameObject.SetActive(false);
    }

    private IEnumerator StartBreathing()
    {
        while (backgroundBreathingRunning)
        {
            yield return null;
        }
        backgroundBreathingRunningTime = 0;
        currentBreakTime = 0;
        backgroundBreathingRunning = true;
        breakTimeRunning = false;
        limboPhase = false;

        StartBackgroundBreathing();
    }

    public override void EndCue()
    {
        backgroundBreathingRunningTime = 0;
        currentBreakTime = 0;
        backgroundBreathingRunning = false;
        breakTimeRunning = false;
        limboPhase = false;
        diegeticAudioSource.gameObject.SetActive(false);
    }
}

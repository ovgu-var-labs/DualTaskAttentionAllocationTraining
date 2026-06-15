using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CueManager : MonoBehaviour
{
    [SerializeField]
    private CueBaseManager[] cues = new CueBaseManager[5];

    private Vector3 targetPosition;

    private bool cueActive = false;
    public bool IsCueActive() {  return cueActive; }

    [SerializeField]
    private int totalCueTime = 20; //Time for cue activity (before Game Over), in seconds
    public int GetTotalCueTime() { return totalCueTime; }
    private float cueRunningTime; //time since cue activated, in seconds

    // Metrics/ Shareables
    private CueBaseManager currentCue;

    private int cueCount = 0;  //number of time a cue is triggered for the Test
    public int GetCueCount() { return cueCount; }

    public enum CueType { HaloCue, FlickerCue, DiegeticCue, NonDiegeticCue, None }
    private CueType currentCueType;

    // Start is called before the first frame update
    //void Start()
    //{
    //    foreach (CueBaseManager _cue in cues)
    //    {
    //        _cue.SetupCue(totalCueTime);
    //    }

    //    currentCue = cues[0];
    //}

    // Update is called once per frame
    void Update()
    {
        if (cueActive)
        {
            cueRunningTime = cueRunningTime + Time.deltaTime;

            currentCue.UpdateCue(targetPosition, cueRunningTime);
        }
    }

    public void SetCurrentCueType(int _cueType)
    {
        currentCue = cues[_cueType];
        currentCue.SetupCue(totalCueTime);
        currentCueType = (CueType)_cueType;
    }

    public void StartCue(Vector3 _targetPosition)
    {
        targetPosition = _targetPosition;

        cueActive = true;

        cueCount++;

        currentCue.StartCue();
    }

    public void StopCue()
    {
        cueActive = false;
        cueRunningTime = 0;

        currentCue.ResetCue();

        Debug.Log("cueCount: " + cueCount);
    }

    public void EndCueRunning()
    {
        foreach (CueBaseManager _cue in cues)
        {
            _cue.EndCue();
        }

        cueCount = 0;
        cueActive = false;
        cueRunningTime = 0;
    }

    public string GetCurrentCueType()
    {
        return currentCueType.ToString();
    }
}

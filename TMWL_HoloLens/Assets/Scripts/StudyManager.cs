using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System;
using static UnityEngine.Rendering.DebugUI.Table;

public class StudyManager : MonoBehaviour
{
    [SerializeField]
    CueManager cueManager;
    [SerializeField]
    Transform target;
    [SerializeField]
    SecondaryTaskManager secondTaskManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    LogHandler logHandler;
    [SerializeField]
    RegistrationHandler registrationHandler;

    // Trial Info
    private enum TrialType { None, Training, Measurement, Expert}
    private TrialType trialType = TrialType.Measurement;
    private int currentParticipantID;

    // Logic Handling
    private bool trialRunning;  //Trial (consisting of multiple cue and secondary task runs) in progress
    private bool cueActive; //cue event trialRunning

    // Trial timing
    //private float trialRunningTime;  //overall Test time
    //private float trialDuration = 30; // duration of trial run in seconds
    private int trialID;

    // Cue & Task timing
    private float cuePauseRunningTime; //time since last cue finished
    private float cuePauseDuration = 5; //Time between cue events
    private float secondaryTaskTimer;
    private float secondaryTaskDuration = 15;

    private float cueRunningTime; //time since cue activated, in seconds
    private int totalCueTime; //Time for cue activity (before Game Over), in seconds

    // AOI Handling
    public enum AOIType { None, Head, Body}
    private AOIType currentAOI = AOIType.None;

    void Start()
    {
        ResetTrial();
    }

    void Update()
    {
        if (trialRunning)
        {
            logHandler.AddToGazeLog(GenerateGazeLog());


            /// Switch Cue
            if (!cueActive)
            {
                cuePauseRunningTime = cuePauseRunningTime + Time.deltaTime;

                if (cuePauseRunningTime > cuePauseDuration)
                {
                    StartCue();
                }
            }
            else
            {
                cueRunningTime = cueRunningTime + Time.deltaTime;

                if (cueRunningTime > totalCueTime)
                {
                    StopCue();
                }
            }

            /// Switch secondary Task
            secondaryTaskTimer = secondaryTaskTimer + Time.deltaTime;

            if (secondaryTaskTimer > secondaryTaskDuration)
            {
                SwitchSecondaryTask();

                logHandler.WriteGazeLog(currentParticipantID, trialType.ToString(), trialID.ToString());

                secondaryTaskTimer = 0;
            }
            /// TRIAL HANDLING -> moved to desktop
            //trialRunningTime = trialRunningTime + Time.deltaTime;
            //if (trialRunningTime > trialDuration)
            //{
            //    FinishTrial(); 
            //}
        }
    }

    //*** TRIAL HANDLING
    public void StartTrial(int _participantID, int _visID, int _studyRun) 
    {
        registrationHandler.StopImageTracking();

        // participant ID from desktop application
        currentParticipantID = _participantID;

        // set trial type for this trial
        switch(_studyRun)
        {
            case 0:
            case 4:
            case 5: trialType = TrialType.Measurement; break;
            case 1:
            case 2:
            case 3: trialType = TrialType.Training; break;
            case 11: trialType = TrialType.Expert; break;
            default: trialType = TrialType.None; break;
        }
        trialID = _studyRun;

        // setup cue for this trial
        cueManager.SetCurrentCueType(_visID);
        totalCueTime = cueManager.GetTotalCueTime();
        StartCue();

        // create log header for this trial
        logHandler.SetupLogHeader();

        // Start secondary Task
        secondTaskManager.ShowNewTask();
        target.gameObject.SetActive(true);

        // Logic Handling
        trialRunning = true;

        // UI Handling
        uiManager.ActivateFinishScreen(false);
    }

    public void AbortTrial() //Beendet den Durchlauf vorzeitig
    {
        registrationHandler.StopImageTracking();
        ResetTrial();
        uiManager.SetFinishText("Trial finished early");
        uiManager.ActivateFinishScreen(true);
    }

    public void FinishTrial() //Beendet den Durchlauf
    {
        trialRunning = false;
        StopCue();

        // Add final Mainlog Info
        logHandler.AddToMainLog(GenerateMainLog());
        // Ensure that all logged data is written to file
        logHandler.WriteMainLog(currentParticipantID, trialType.ToString(), trialID.ToString());
        logHandler.WriteGazeLog(currentParticipantID, trialType.ToString(), trialID.ToString());     

        // UIHandling
        if (secondTaskManager.GetTaskCount() > 0)
        {
            uiManager.SetFinishText("Aufgaben gesehen: " + ((secondTaskManager.GetTaskSeenCount() / secondTaskManager.GetTaskCount()) * 100) + "%");
        }
        else
        {
            uiManager.SetFinishText("Keine Aufgaben gezeigt");
        }
        uiManager.ActivateFinishScreen(true);

        registrationHandler.StartImageTracking();

        // Reset for next trial
        ResetTrial();
    }

    private void ResetTrial() //setzt die standartwerte
    {
        cueActive = false;
        cuePauseRunningTime = 0;
        cueRunningTime = 0;
        secondTaskManager.ResetTask();
        cueManager.EndCueRunning();
        target.gameObject.SetActive(false);
    }

    //**** CUE HANDLING
    private void StartCue()
    {
        if (trialType == TrialType.Training)
        {
            cueManager.StartCue(target.position);
        }
            cueActive = true;
            cuePauseRunningTime = 0;
    }

    private void StopCue()
    {
        logHandler.AddToSecTaskLog(GenerateSecTaskLog(cueRunningTime));
        logHandler.WriteSecTaskLog(currentParticipantID, trialType.ToString(), trialID.ToString());

        cueActive = false;
        cueRunningTime = 0;
        cueManager.StopCue();
    }

    private void SwitchSecondaryTask()
    {
        if (secondTaskManager.secondaryTasksLeft())
        {
            secondTaskManager.ShowNewTask();
        }
        //else
        //{
        //    Debug.Log("No secondary Tasks left!");
        //}
    }



    private string GenerateMainLog()
    {
        return currentParticipantID + ";" 
            + DateTime.Now.ToString("s") + ";" 
            + trialID.ToString() + ";"
            + trialType.ToString() + ";" 
            + cueManager.GetCurrentCueType() + ";" 
            + cueManager.GetCueCount() + ";" 
            + secondTaskManager.GetTaskSeenCount() + ";"
            + secondTaskManager.GetTaskCount() + ";"
            + secondTaskManager.GetSeenChars() + ";"
            + secondTaskManager.GetNotSeenChars();
    }

    private string GenerateGazeLog()
    {
        return currentParticipantID + ";"
            + DateTime.Now.ToString("s") + ";"
            + trialID.ToString() + ";"
            + trialType.ToString() + ";"
            + cueManager.GetCurrentCueType() + ";"
            + currentAOI.ToString();
    }

    private string GenerateSecTaskLog(float taskDuration)
    {
        return currentParticipantID + ";" 
            + DateTime.Now.ToString("s") + ";" 
            + trialID.ToString() + ";"
            + trialType.ToString() + ";" 
            + cueManager.GetCurrentCueType() + ";"
            + taskDuration + ";" 
            + secondTaskManager.GetCurrentTaskSeen() + ";"
            + secondTaskManager.GetCurrentChar();
    }

    public void LookedAtTarget() //Wenn zum Kopf geschaut wurde
    {
        // Log secondary task completion
        if (!secondTaskManager.GetCurrentTaskSeen())
        {
            secondTaskManager.TaskSeen();
        }
        HitAOI(AOIType.Head);

        // Reset Attention Guidance Cue
        if (cueActive)
        {
            StopCue();
        }
    }

    public void HitAOI(AOIType _aoi)
    {
        if (currentAOI != _aoi)
        {
            currentAOI = _aoi;
        }
    }
}

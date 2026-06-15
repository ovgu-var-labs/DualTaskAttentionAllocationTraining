using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DesktopStudyManager : MonoBehaviour
{
    public Sender_UDP dataSender;

    public LogHandler logHandler;

    public SerialController serialController;

    [SerializeField]
    TMP_InputField participantIDField;
    private int participantID;

    [SerializeField]
    private int trialID = 0;
    [SerializeField]
    private int cueID = -1;

    private bool trialRunning = false;
    private float currentStopwatchTime = 0f;
    [SerializeField]
    private TMP_Text stopwatchText;

    private int trialDuration;
    [SerializeField]
    private int trainingDuration = 180;
    [SerializeField]
    private int measurementDuration = 60;

    [SerializeField]
    private TMP_InputField primaryTaskAnswers;
    [SerializeField]
    private TMP_InputField taskLog;

    // Primary Task
    [SerializeField]
    private TMP_Text currentMotorText;
    private List<int> primTaskNums = new List<int>() {1,2,3,4,5,6,7,8};
    [SerializeField]
    private TMP_Text[] taskVis = new TMP_Text[8];
    private List<int> primTaskHighlightIndices = new List<int>() { };
    private int highlightCount = 4;
    private int oldTaskNum = 1;
    [SerializeField]
    private GameObject[] primTaskHighlights = new GameObject[8];

    [SerializeField]
    Transform trialToggle;

    [SerializeField]
    Transform cueToggle;


    private void Update()
    {
        if(trialRunning)
        {
            currentStopwatchTime += Time.deltaTime;

            if (currentStopwatchTime > trialDuration)
            {
                Debug.Log("Stop Training");
                StopTrial(true);
            }
            else
            {
                int min = (int)currentStopwatchTime / 60;
                int sec = (int)currentStopwatchTime % 60;
                string s_min = min < 10 ? "0" + min : "" + min;
                string s_sec = sec < 10 ? "0" + sec : "" + sec;
                stopwatchText.text = s_min + ":" + s_sec;
            }
        }
    }

    public void SetTrialID(int _trialID)
    {
        trialID = _trialID;
    }

    public void SetCueID(int _cueID)
    {
        cueID = _cueID;
    }

    public void StartTrial()
    {
        StartStudyPackage pckg = new StartStudyPackage();
        try
        {
            participantID = int.Parse(participantIDField.text);
            
        }
        catch
        {
            Debug.Log("No participant ID set. Participant ID is set to 666");
            participantIDField.text = ""+666;
            participantID = int.Parse(participantIDField.text);
        }
        pckg.participantID = participantID;

        if(participantID % 3 == 0) // Flicker
        {
            cueToggle.GetChild(1).GetComponent<Toggle>().isOn = true;
        }
        else if(participantID % 2 == 0) // Halo
        {
            cueToggle.GetChild(2).GetComponent<Toggle>().isOn = true;
        }
        else // None
        {
            Debug.Log(cueToggle.GetChild(4).name);
            cueToggle.GetChild(4).GetComponent<Toggle>().isOn = true;
        }

        pckg.visID = cueID;
        pckg.studyRunID = trialID;

        Debug.Log("study Info sent : participant id " + participantID + " vis ID " + cueID + " trial ID " + trialID);

        dataSender.SendUdp(pckg.Serialize());

        StartArduino();
        
        trialRunning = true;
        if(trialID == 0 || trialID == 4 || trialID == 5)
        {
            trialDuration = measurementDuration;
        }
        else
        {
            trialDuration = trainingDuration;
        }
            currentStopwatchTime = 0;
    }

    public void StopTrial(bool trialsuccesfull)
    {
        trialRunning = false;
        StopStudyPackage pckg = new StopStudyPackage();
        dataSender.SendUdp(pckg.Serialize());

        stopwatchText.text = "00:00";

        EndArduino();
        logHandler.ResetTaskLog();
        ResetInputField(taskLog);

        if (trialsuccesfull)
        {
            if (trialID < 5)
            {
                trialToggle.GetChild(trialID + 1).GetComponent<Toggle>().isOn = true;
            }
            else
            {
                Debug.Log("Study done");
            }
        }
    }

    public void StartArduino()
    {
        serialController.SendSerialMessage("S");
    }

    public void StartArduinoDemo() {
        serialController.SendSerialMessage("D");
    }

    public void EndArduino()
    {
        serialController.SendSerialMessage("O");
    }

    public void AddToTaskLog(string text)
    {
        Debug.Log("Log TaskLog: " + text);
        if (!string.IsNullOrEmpty(text))
        {
            logHandler.AddToTaskLog(participantID + ";" + DateTime.Now + ";" + cueID + ";" + trialID + ";" + text[text.Length - 1]);
            logHandler.WriteTaskLog(participantID, "_" + cueID, "_" + trialID);
        }
    }

    public void ResetInputField(TMP_InputField field)
    {
        field.text = "";
    }

    public void SwitchPrimaryTask(string text)
    {
        // Get correct Task Answer
        int taskNum = Int32.Parse(text);

        // Shuffle next PrimTask Array
        System.Random r = new System.Random();
        for (int n = primTaskNums.Count - 1; n > 0; --n)
        {
            //Randomly pick an item which has not been shuffled
            int k = r.Next(n + 1);

            //Swap the selected item with the last "unstruck" letter in the collection
            if (n != oldTaskNum - 1 && k!= oldTaskNum-1)
            {
                int temp = primTaskNums[n];
                primTaskNums[n] = primTaskNums[k];
                primTaskNums[k] = temp;
            }
        }

        primTaskHighlightIndices.Clear();
        // generate random highlight indices
        primTaskHighlightIndices.Add(taskNum - 1);  // set new task Highlight
        primTaskHighlightIndices.Add(oldTaskNum - 1);  // set old task Highlight
        int index = 0;
        while (primTaskHighlightIndices.Count < highlightCount) { 
            do
            {
                index = r.Next(primTaskNums.Count);
            } while (primTaskHighlightIndices.Contains(index));
            primTaskHighlightIndices.Add(index);
        }

        // Send data to HoloLens (& Task Vis)
        SwitchPrimTaskPackage pckg = new SwitchPrimTaskPackage();
        for (int i = 0; i< primTaskNums.Count; ++i)
        {
            taskVis[i].text = primTaskNums[i].ToString();
            if (primTaskHighlights[i].activeSelf) { 
                primTaskHighlights[i].SetActive(false); 
            }
            pckg.primTaskNum[i] = primTaskNums[i];
 
        }

        for (int i = 0; i < primTaskHighlightIndices.Count; i++)
        {
            pckg.primHighlightsIndices[i] = primTaskHighlightIndices[i];
            primTaskHighlights[primTaskHighlightIndices[i]].SetActive(true);

            Debug.Log("highlight Index: " + pckg.primHighlightsIndices[i]);
        }


        dataSender.SendUdp(pckg.Serialize());

        // Get shuffled Array answer
        string taskAnswer = primTaskNums[taskNum-1].ToString();
        string oldTaskAnswer = primTaskNums[oldTaskNum-1].ToString();

        // Logging
        logHandler.AddToArduinoLog(participantID + ";" + DateTime.Now + ";" + cueID + ";" + trialID + ";" + text + ";" + taskAnswer);
        logHandler.WriteArduinoLog(participantID, cueID.ToString(), trialID.ToString());

        // UI Update
        currentMotorText.text = taskNum + " / answer / " + taskAnswer + "\n" + oldTaskNum + " / answer / " + oldTaskAnswer;

        oldTaskNum = taskNum;   // save new Task highlight

    }
}

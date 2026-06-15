using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class LogHandler : MonoBehaviour
{
    private string taskLog;
    private string arduinoLog;

    public void AddToTaskLog(string _logString)
    {
        if (taskLog == null)
        {
            taskLog = string.Empty;
        }
        taskLog = _logString + "\n";
    }

    public void AddToArduinoLog(string _logString)
    {
        if(arduinoLog == null) {
            arduinoLog = string.Empty; 
        }
        arduinoLog = _logString + "\n";
    }

    public void WriteTaskLog(int _participantID, string _type, string _run)
    {
        string folderName = _participantID.ToString();
        string fileName = _participantID + "TaskLog.txt";

        WriteLog(folderName, fileName, taskLog);
    }

    public void ResetTaskLog()
    {
        taskLog = string.Empty;
    }

    public void WriteArduinoLog(int _participantID, string _type, string _run)
    {
        string folderName = _participantID.ToString();
        string fileName = _participantID + "ArduinoLog.txt";

        WriteLog(folderName, fileName, arduinoLog);

        arduinoLog = string.Empty;
    }


    private void WriteLog(string folderName, string fileName, string _logText)
    {
        string path = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.AppendAllText(Path.Combine(path, fileName), _logText);
    }

}

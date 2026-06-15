using ARETT;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Unity.VisualScripting;
using UnityEngine;
//#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
//using Windows.Storage;
//#endif

public class LogHandler : MonoBehaviour
{
    private string gazeLog;
    private string mainLog;
    private string secTaskLog;


    public void SetupLogHeader() //start Logging, create Files and write headline
    {
        AddToGazeLog("ID;Date;TrialID;TrialType;CueType;AOI");
        AddToSecTaskLog("ID;Date;TrialID;TrialType;CueType;CueDuration;TaskSeen;TaskChar");
        AddToMainLog("ID;Date;TrialID;TrialType;CueType;TotalCueCount;TaskSeenCount;seenChars;notSeenChars");
    }

    public void AddToGazeLog(string _logString)
    {
        if (gazeLog == null)
        {
            gazeLog = string.Empty;
        }
        gazeLog += _logString + "\n";
    }

    public void AddToMainLog(string _logString)
    {
        if (mainLog == null)
        {
            mainLog = string.Empty;
        }
        mainLog += _logString + "\n";
    }

    public void AddToSecTaskLog(string _logString)
    {
        if (secTaskLog == null)
        {
            secTaskLog = string.Empty;
        }
        secTaskLog += _logString + "\n";
    }

    public void WriteGazeLog(int _participantID, string _type, string _run)
    {
        string folderName = _participantID.ToString();
        string fileName = _participantID + "GazeLog.txt";

        WriteLog(folderName, fileName, gazeLog);

        gazeLog = string.Empty;
    }

    public void WriteMainLog(int _participantID, string _type, string _run)
    {
        string folderName = _participantID.ToString();
        string fileName = _participantID + "MainLog.txt";

        WriteLog(folderName, fileName, mainLog);
        //Debug.Log("mainLog "+ mainLog);

        mainLog = string.Empty;
        
    }

    public void WriteSecTaskLog(int _participantID, string _type, string _run)
    {
        string folderName = _participantID.ToString();
        string fileName = _participantID + "SecTaskLog.txt";

        WriteLog(folderName, fileName, secTaskLog);
        //Debug.Log("secLog " +  secTaskLog);

        secTaskLog = string.Empty;
    }


    private void WriteLog(string folderName, string fileName, string _logText)
    {
        //#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        //        // Base folder is the Documents folder of the device
        //        StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
        //        // Add Folder for specified name or just get the folder if it already exists
        //        StorageFolder appStorageFolder = await storageFolder.CreateFolderAsync(Application.productName, CreationCollisionOption.OpenIfExists);
        //        // Add the current participant name (or get the existing folder)
        //        StorageFolder participantStorageFolder = await appStorageFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

        //        StorageFile logFile = await participantStorageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
        //        await FileIO.AppendTextAsync(logFile, _logText);
        //#endif

        string path = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.AppendAllText(Path.Combine(path, fileName), _logText);
    }

}

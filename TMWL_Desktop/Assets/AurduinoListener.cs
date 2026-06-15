using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AurduinoListener : MonoBehaviour
{
    [SerializeField]
    private DesktopStudyManager studyManager;

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        studyManager.SwitchPrimaryTask(msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
        {
            Debug.Log("Arduino connected");
        }
        else
        {
            Debug.Log("Arduino connection FAILED");
        }
    }
}

using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if WINDOWS_UWP
    using Windows.Storage.Streams;
#endif

public class NetworkManager : MonoBehaviour
{
    private int idPlaceholder;

    private static int startTrialPackageID = 111;
    private static int stopTrialPackageID = 666;
    private static int switchPrimTaskID = 333;

    private int participantID;
    private int visID;
    private int studyRunID;

    [SerializeField]
    StudyManager studyManager;
    [SerializeField]
    PrimaryTaskManager primaryTaskManager;

    private static string ListenPort = "9718";
    [HideInInspector]
    public bool HasNewMessage;

    private bool coroutineRuns = false;
    private bool startReceived = false;

#if WINDOWS_UWP
    
    Windows.Networking.Sockets.DatagramSocket _Socket = null;
    Windows.Networking.Sockets.DatagramSocket _SocketOut = null;
    Windows.Networking.Sockets.DatagramSocket FindBuddySocket;
#endif

    private Queue<byte[]> _receivedPackages = new Queue<byte[]>();

    void Start()
    {

#if WINDOWS_UWP
        Init_Listener();
#endif
    }

    void Update()
    {
        while (_receivedPackages.Count > 0)
        {
            var package = _receivedPackages.Dequeue();

            int id = GetPackageID(package);
            if (id == startTrialPackageID)
            {
                StartStudyPackage pckg = StartStudyPackage.Deserialize(package);
                Debug.Log("study Info: participantID: " + pckg.participantID + " vis ID " + pckg.visID + " trial ID " + pckg.studyRunID);
                studyManager.StartTrial(pckg.participantID, pckg.visID, pckg.studyRunID);
            }
            else if (id == stopTrialPackageID)
            {
                studyManager.FinishTrial();
            }
            else if (id == switchPrimTaskID)
            {
                SwitchPrimTaskPackage pckg = SwitchPrimTaskPackage.Deserialize(package);
                int[] numArray = pckg.primTaskNum;

                int[] highlightIndices = pckg.primHighlightsIndices;

                primaryTaskManager.SetNewPrimTaskNumbers(numArray);

                primaryTaskManager.SetNewPrimTaskHighlights(highlightIndices);
            }
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    studyManager.StartTrial(0, 0, 1);
        //}
    }

    public int GetCurrentParticipantID()
    {
        return idPlaceholder;
    }

    private int GetPackageID(byte[] info)
    {
        int result = -1;

        using (MemoryStream m = new MemoryStream(info))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result = reader.ReadInt32();
            }
        }
        return result;
    }


#if WINDOWS_UWP
    private void Init_Listener()
    {
        try
        {
            _Socket = new Windows.Networking.Sockets.DatagramSocket();
            _Socket.MessageReceived += ServerDatagramSocket_MessageReceived;
            _Socket.BindServiceNameAsync(ListenPort);
        }
        catch (Exception ex)
        {
            _Socket.Dispose();
            _Socket = null;

            Debug.LogError(ex.ToString());
            Debug.LogError(Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult).ToString());
        }
    }

    public void Dispose()
    {
        if (_Socket != null)
        {
            _Socket.Dispose();
            _Socket = null;
        }
    }
#endif

#if WINDOWS_UWP
    private async void ServerDatagramSocket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        try{
            string request;
            using (DataReader dataReader = args.GetDataReader())
            {
                byte[] content = new byte[dataReader.UnconsumedBufferLength];
                dataReader.ReadBytes(content);
                _receivedPackages.Enqueue(content);
            }
        }
        catch (Exception ex)
        { }
    }
#endif
}
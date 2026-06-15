using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class RegistrationHandler : MonoBehaviour
{
    [SerializeField]
    private Transform studyEnvironment;
    [SerializeField]
    private Transform registrationMarker;

    private bool updateStudyEnvironmentPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updateStudyEnvironmentPosition)
        {
            studyEnvironment.position = registrationMarker.position;
            studyEnvironment.rotation = registrationMarker.rotation;
        }
    }

    public void StartImageTracking()
    {
        VuforiaBehaviour.Instance.enabled = true;
    }

    public void StopImageTracking()
    {
        VuforiaBehaviour.Instance.enabled = false;
    }

    public void MoveStudyEnvironment(bool _move)
    {
        updateStudyEnvironmentPosition = _move;
    }
}

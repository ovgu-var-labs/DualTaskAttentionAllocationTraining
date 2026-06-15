using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CueBaseManager : MonoBehaviour
{
    public abstract void UpdateCue(Vector3 _targetPosition, float _timeValue);

    public abstract void SetupCue(int _totalTime);

    public abstract void ResetCue();

    public abstract void StartCue();

    public abstract void EndCue();
}

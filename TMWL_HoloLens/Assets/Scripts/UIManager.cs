using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text FinishScreen;
    //[SerializeField]
    //private BoundsControl studyEnvironment;

    public void ActivateFinishScreen(bool _activate)
    {
        FinishScreen.gameObject.SetActive(_activate);
    }

    public void SetFinishText(string _text)
    {
        FinishScreen.text = _text;
    }

    //public void ActivateAdjustmentStudyenvironment(bool _activate)
    //{
    //    studyEnvironment.enabled = _activate;
    //}
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrimaryTaskManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text[] primTaskNumbers = new TMP_Text[8];

    [SerializeField]
    private GameObject[] primTaskHighlights = new GameObject[8];


    public void SetNewPrimTaskNumbers(int[] num)
    {
        for (int i = 0; i < 8; i++)
        {
            primTaskNumbers[i].text = num[i].ToString();
        }
    }

    public void SetNewPrimTaskHighlights(int[] indices)
    {
        foreach (GameObject highlight in primTaskHighlights)
        {
            if(highlight.activeSelf) { highlight.SetActive(false); }
        }

        foreach (int i in indices)
        {
            primTaskHighlights[i].SetActive(true);
        }
    }

}

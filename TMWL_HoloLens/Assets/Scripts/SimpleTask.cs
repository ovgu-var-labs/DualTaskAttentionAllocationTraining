using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleTask : MonoBehaviour
{
    public GameObject VL;
    public GameObject VR;
    public GameObject HL;
    public GameObject HR;

    private bool vL;
    private bool vR;
    private bool hL;
    private bool hR;
    // Start is called before the first frame update
    void Start()
    {
        vL = false;
        vR = false;
        hL = false;
        hR = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(vL && vR && hL && hR)
        {
            SetNumbers();
            vL = false;
            vR = false;
            hL = false;
            hR = false;
        }
    }

    private void SetNumbers()
    {
        List<int> numbers = new List<int>();
        numbers.Add(1);
        numbers.Add(2);
        numbers.Add(3);
        numbers.Add(4);

        int hilf = (int)Mathf.Round(Random.Range(0, numbers.Count - 1));
        VL.GetComponent<TMP_Text>().text = numbers[hilf].ToString();
        numbers.RemoveAt(hilf);

        hilf = (int)Mathf.Round(Random.Range(0, numbers.Count - 1));
        VR.GetComponent<TMP_Text>().text = numbers[hilf].ToString();
        numbers.RemoveAt(hilf);

        hilf = (int)Mathf.Round(Random.Range(0, numbers.Count - 1));
        HL.GetComponent<TMP_Text>().text = numbers[hilf].ToString();
        numbers.RemoveAt(hilf);

        hilf = (int)Mathf.Round(Random.Range(0, numbers.Count - 1));
        HR.GetComponent<TMP_Text>().text = numbers[hilf].ToString();
        numbers.RemoveAt(hilf);
    }

    public void SetVL()
    {
        vL = true;
    }
    public void SetVR()
    {
        vR = true;
    }
    public void SetHL()
    {
        hL = true;  
    }
    public void SetHR()
    {
        hR = true;
    }
}

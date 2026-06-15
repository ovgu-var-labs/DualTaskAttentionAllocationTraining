using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class FlickerManager : CueBaseManager
{
    public Transform flickerImage;    //GameObject to flicker
    public Transform flickerCanvas;

    [SerializeField]
    private Transform orientationHelperUp;
    [SerializeField]
    private Transform orientationHelperDown;

    private Color flickerColor = Color.white;
    private float flickerTransparency;   //Transparentwert

    private float clippingOffset = 0.15f; //ab welchen prozentualen Anteil im Bild der FlickerManager an den Rand clippen soll

    private float flickerIntervall; //interval time of flicker+pause
    private float baseInterval = 0.2f;
    private float counter; //time passed since last flicker
    private float flickerDuration = 0.17f;
    private float flickerTimeOn; //how long the flicker shines
    private bool isActive; //is the light turned on?

    private float height; //reserviert für die Höhe des Canvas
    private float width;    //reserviert für die Breite des Canvas
    private float YOffset;  //um wieviel der FlickerManager in Y verschoben werden muss, um des Offset der HoloLens auszugleichen (muss getestet werden)
    private float baseOffset = 0.3f;

    private Vector3 flickerSize;

    private Vector3 flickerPos; //Position an der es Flickern soll

    private int totalCueTime;

    private float transparencyStart = 0;
    private float transparencyEnd = 1;
    private float sizeStart = 0.2f;
    private float sizeEnd = 0.5f;

    public override void SetupCue(int _totalTime)
    {
        counter = 0;
        isActive = true;

        totalCueTime = _totalTime;
    }

    public override void UpdateCue(Vector3 _targetPosition, float _timeValue)
    {
        // get timebased Values
        float timeLerpGreyscale = _timeValue / totalCueTime;
        flickerTransparency = Mathf.Lerp(transparencyStart, transparencyEnd, timeLerpGreyscale);

        if(_timeValue > 0.5f*totalCueTime)
        {
            float timeLerpSize = (_timeValue - 0.5f*totalCueTime) / (0.5f*totalCueTime);
            float x = Mathf.Lerp(sizeStart, sizeEnd, timeLerpSize);
            flickerSize = new Vector3(x, x, 1);
        }  


        height = flickerCanvas.GetComponent<RectTransform>().sizeDelta.y;
        width = flickerCanvas.GetComponent<RectTransform>().sizeDelta.x;

        flickerIntervall = baseInterval * 5;
        flickerTimeOn = Mathf.Lerp(0, flickerIntervall, flickerDuration);
        flickerImage.localScale = flickerSize;
        YOffset = Mathf.Lerp(-(flickerImage.GetComponent<RectTransform>().sizeDelta.y), flickerImage.GetComponent<RectTransform>().sizeDelta.y, baseOffset);
        counter += Time.deltaTime;

        flickerPos = Camera.main.WorldToViewportPoint(_targetPosition);
        float x1 = Mathf.LerpUnclamped(-width / 2, width / 2, flickerPos.x);
        float y1 = Mathf.LerpUnclamped(-height / 2, height / 2, flickerPos.y);
        flickerPos = new Vector3(x1, y1, flickerImage.localPosition.z);

            if (flickerPos.x < (-(width / 2) - (flickerImage.GetComponent<RectTransform>().sizeDelta.x / 2)) || flickerPos.x > (width / 2) + flickerImage.GetComponent<RectTransform>().sizeDelta.x / 2 ||
                flickerPos.y < (-(height / 2) - (flickerImage.GetComponent<RectTransform>().sizeDelta.y / 2) + YOffset) || flickerPos.y > (height / 2) + (flickerImage.GetComponent<RectTransform>().sizeDelta.x / 2) + YOffset)
            {
                Vector3 t = Camera.main.WorldToScreenPoint(orientationHelperUp.transform.position);
                Vector3 b = Camera.main.WorldToScreenPoint(orientationHelperDown.transform.position);

                Vector2 hilf = new Vector2(t.x - b.x, t.y - b.y);
                hilf = hilf.normalized;

                float f1 = Mathf.InverseLerp(-1, 1, hilf.x);
                float f2 = Mathf.InverseLerp(-1, 1, hilf.y);

                if (f1 < clippingOffset)
                {
                    f1 = -(width / 2);
                }
                else if ((1 - f1) < clippingOffset)
                {
                    f1 = width / 2;
                }
                else
                {
                    f1 = Mathf.Lerp(-(width / 2), width / 2, f1);
                }

                if (f2 < clippingOffset)
                {
                    f2 = -(height / 2);
                }
                else if ((1 - f2) < clippingOffset)
                {
                    f2 = height / 2;
                }
                else
                {
                    f2 = Mathf.Lerp(-(height / 2), height / 2, f2);
                }

                flickerImage.localPosition = new Vector3(f1, f2 + YOffset, flickerImage.position.z);
            }
            else
            {
                flickerImage.localPosition = flickerPos;
            }

            if (isActive)
            {
                if (counter >= (flickerIntervall - flickerTimeOn))
                {
                    flickerImage.gameObject.SetActive(isActive);
                    counter = 0;
                    isActive = !isActive;
                }
            }
            else
            {
                if (counter >= flickerTimeOn)
                {
                    flickerImage.gameObject.SetActive(isActive);
                    counter = 0;
                    isActive = !isActive;
                }
            }

        flickerColor.a = flickerTransparency;
        flickerImage.GetComponent<Graphic>().color = flickerColor;   //übergibt diese an das Bild
    }

    public override void ResetCue()
    {
        flickerTransparency = transparencyStart;
        flickerSize = flickerSize = new Vector3(sizeStart, sizeStart, 1); ;
        flickerCanvas.gameObject.SetActive(false);
    }

    public override void StartCue()
    {
        flickerCanvas.gameObject.SetActive(true);
    }

    public override void EndCue()
    {
        ResetCue();
    }
}

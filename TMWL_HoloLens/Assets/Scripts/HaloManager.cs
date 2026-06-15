using UnityEngine;
using UnityEngine.UI;

public class HaloManager : CueBaseManager
{
    public Transform haloCanvas;
    public Transform haloImage;

    [SerializeField]
    private Transform orientationHelperUp;
    [SerializeField]
    private Transform orientationHelperDown;

    private float clippingOffset = 0.15f; //ab welchem Abstand zum Rand der Halo an diesen ranclippen soll

    [SerializeField]
    private Vector3 haloPosition;
    [SerializeField]
    private Vector3 targetDirection;
    [SerializeField]
    private float viewTargetAngle;
    [SerializeField]
    private float targetDistance;

    [SerializeField]
    private float xScaling = 1.5f;
    [SerializeField]
    private float yScaling = 1.5f;
    [SerializeField]
    private float haloTransparency = 0;
    [SerializeField]
    private float angleScaling = 60.12f;
    [SerializeField]
    private float distanceScaling = 3f;

    private float transparencyStart = 0;
    private float transparencyEnd = 1;
    private float pixelSizeStart = 200;
    private float pixelSizeEnd = 250;
    [SerializeField]
    private float totalCueTime;

    public override void SetupCue(int _totalTime)
    {
        totalCueTime = _totalTime;
    }


    public override void UpdateCue(Vector3 _targetPosition, float _timeValue)
    {
        // Transparency
        float timeLerpGreyscale = _timeValue / totalCueTime;
        haloTransparency = Mathf.Lerp(transparencyStart, transparencyEnd, timeLerpGreyscale);

        haloImage.GetComponent<Image>().material.SetFloat("_transparency", haloTransparency); //übergibt Transparenz an den Shader

        // Halo Thickness
        float timeLerpSize = 0f;
        if (_timeValue > 0.5f*totalCueTime)
        {
            timeLerpSize = (_timeValue - 0.5f*totalCueTime) / (0.5f*totalCueTime);
        }  
        float pixelSize = Mathf.Lerp(pixelSizeStart, pixelSizeEnd, timeLerpSize);
        haloImage.GetComponent<Image>().material.SetFloat("_timeFactor", timeLerpSize);

        // Halo Position
        float canvasWidth = haloCanvas.GetComponent<RectTransform>().rect.width; //speichert die Breite des Canvas
        float canvasHeight = haloCanvas.GetComponent<RectTransform>().rect.height; //speichert die Höhe des Canvas

        float imageWidth = haloImage.GetComponent<RectTransform>().rect.width;
        float imageHeight = haloImage.GetComponent<RectTransform>().rect.height;

        targetDirection = _targetPosition - Camera.main.transform.position; //Berechnet die targetDirection
        viewTargetAngle = Vector3.Angle(Camera.main.transform.forward, targetDirection); //Winkel zwischen targetDirection und Blickrichtung
        targetDistance = targetDirection.magnitude; //Distanz zum Ziel

        haloPosition = Camera.main.WorldToViewportPoint(_targetPosition); // halo position in viewport coordinates
        float x1 = Mathf.LerpUnclamped(-0.5f * canvasWidth, 0.5f * canvasWidth, haloPosition.x);
        float y1 = Mathf.LerpUnclamped(-0.5f * canvasHeight, 0.5f * canvasHeight, haloPosition.y);
        haloPosition = new Vector3(x1, y1, haloImage.localPosition.z); // halo position (not moved to screen) in "canvas" coordinates
        
        if (haloPosition.x < (-0.5f * canvasWidth - 0.5f * imageWidth) || haloPosition.x > (0.5f * canvasWidth + 0.5f * imageWidth) ||
            haloPosition.y < (-0.5f * canvasHeight - 0.5f * imageHeight) || haloPosition.y > (0.5f * canvasHeight + 0.5f * imageHeight))
        { //halo position outside of screen
            Vector3 t = Camera.main.WorldToScreenPoint(orientationHelperUp.position);
            Vector3 b = Camera.main.WorldToScreenPoint(orientationHelperDown.position);

            Vector2 orientationHelper = new Vector2(t.x - b.x, t.y - b.y).normalized;

            float horizontalCanvasBorderPosition = Mathf.InverseLerp(-1, 1, orientationHelper.x);
            float verticalCanvasBorderPosition = Mathf.InverseLerp(-1, 1, orientationHelper.y);

            if (horizontalCanvasBorderPosition < clippingOffset) //Wenn der Halo nah am linken/rechten Rand ist, dann:
            {
                horizontalCanvasBorderPosition = -0.5f * canvasWidth  - pixelSize ;
            }
            else if ((1 - horizontalCanvasBorderPosition) < clippingOffset)
            {
                horizontalCanvasBorderPosition = 0.5f* canvasWidth  + pixelSize;
            }
            else //wenn er nicht am Rand ist:
            {
                horizontalCanvasBorderPosition = Mathf.Lerp(-0.5f*canvasWidth, 0.5f*canvasWidth, horizontalCanvasBorderPosition);
            }

            if (verticalCanvasBorderPosition < clippingOffset) //Wenn der Halo nah am oberen/unteren Rand ist, dann:
            {
                verticalCanvasBorderPosition = -0.5f*canvasHeight - pixelSize;
            }
            else if ((1 - verticalCanvasBorderPosition) < clippingOffset)
            {
                verticalCanvasBorderPosition = 0.5f*canvasHeight + pixelSize;
            }
            else //wenn er nicht am Rand ist:
            {
                verticalCanvasBorderPosition = Mathf.Lerp(-0.5f*canvasHeight, 0.5f*canvasHeight, verticalCanvasBorderPosition);
            }

            haloPosition = new Vector3(horizontalCanvasBorderPosition, verticalCanvasBorderPosition, haloImage.position.z); //setzt die neue Position des Halo
        }

        haloImage.localPosition = haloPosition;

        // Halo Scale
        haloImage.localScale = new Vector3(xScaling + viewTargetAngle / angleScaling + Mathf.Log10(targetDistance / distanceScaling), yScaling + viewTargetAngle / angleScaling + Mathf.Log10(targetDistance / distanceScaling), 1); //passt die Größe des Halo an die errechneten Gewichte an
        
    }

    public override void ResetCue()
    {
        haloTransparency = transparencyStart;

        haloCanvas.gameObject.SetActive(false);
    }

    public override void StartCue()
    {
        haloTransparency = transparencyStart;
        haloCanvas.gameObject.SetActive(true);
    }

    public override void EndCue()
    {
        ResetCue();
    }
}

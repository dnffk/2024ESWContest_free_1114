using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClassificationPlane : MonoBehaviour
{
    public ARPlane _ARPlane;
    public MeshRenderer _PlaneMeshRenderer;
    GameObject _mainCam;

    // Start is called before the first frame update
    void Start()
    {
        _mainCam = FindObjectOfType<Camera>().gameObject;
    }

    // Update is called once per frame

    void Update()
    {
        UpdatePlaneColor();
    }

    void UpdatePlaneColor()
    {
        Color planeMatColor = Color.cyan;
        //자동으로 분류해주며 분류값에 따라 Plane 색 변경
        switch (_ARPlane.classification)
        {
            case PlaneClassification.None:
                planeMatColor = Color.cyan;
                break;
            case PlaneClassification.Wall:
                planeMatColor = Color.green;
                break;
            case PlaneClassification.Floor:
                planeMatColor = Color.white;
                break;
            case PlaneClassification.Ceiling:
                planeMatColor = Color.blue;
                break;
            case PlaneClassification.Table:
                planeMatColor = Color.yellow;
                break;
            case PlaneClassification.Seat:
                planeMatColor = Color.magenta;
                break;
            case PlaneClassification.Door:
                planeMatColor = Color.red;
                break;
            case PlaneClassification.Window:
                planeMatColor = Color.clear;
                break;
        }
        _PlaneMeshRenderer.material.color = planeMatColor;
    }
}

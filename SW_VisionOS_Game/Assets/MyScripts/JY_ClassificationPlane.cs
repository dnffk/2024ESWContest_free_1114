using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class JY_ClassificationPlane : MonoBehaviour
{
    public ARPlane _ARPlane;
    public MeshRenderer _PlaneMeshRenderer;
    public Material defaultMaterial;
    public Material wallMaterial;
    public Material floorMaterial;
    public Material ceilingMaterial;
    public Material tableMaterial;
    public Material seatMaterial;
    public Material doorMaterial;
    public Material windowMaterial;

    private GameObject _mainCam;

    // Start is called before the first frame update
    void Start()
    {
        _mainCam = FindObjectOfType<Camera>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlaneMaterial();
    }

    void UpdatePlaneMaterial()
    {
        Material planeMaterial = defaultMaterial;

        switch (_ARPlane.classification)
        {
            case PlaneClassification.None:
                planeMaterial = defaultMaterial;
                break;
            case PlaneClassification.Wall:
                planeMaterial = wallMaterial;
                break;
            case PlaneClassification.Floor:
                planeMaterial = floorMaterial;
                break;
            case PlaneClassification.Ceiling:
                planeMaterial = ceilingMaterial;
                break;
            case PlaneClassification.Table:
                planeMaterial = tableMaterial;
                break;
            case PlaneClassification.Seat:
                planeMaterial = seatMaterial;
                break;
            case PlaneClassification.Door:
                planeMaterial = doorMaterial;
                break;
            case PlaneClassification.Window:
                planeMaterial = windowMaterial;
                break;
        }

        if (planeMaterial != null)
        {
            _PlaneMeshRenderer.material = planeMaterial;
        }
    }
}
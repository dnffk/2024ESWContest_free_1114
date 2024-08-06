using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CanvasControl : MonoBehaviour
{
    public Slider slider;
    public Transform cameraTransform;
    public float distanceFromCamera = 0.5f;
    public CanvasGroup canvas;
    public TextMeshProUGUI Brightness;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        canvas.alpha = slider.value;
        float a = slider.value * 100;
        Brightness.text = a.ToString("0") + "%" ;

    }
    
    public void UpdateValue()
    {
        if (ValueManager.Instance != null)
        {
            ValueManager.Instance.Set_Brightness_Value((float)Math.Round(slider.value, 2));
            Debug.Log("Brightness : " + ValueManager.Instance.Brightness_Value);
        }
        else
        {
            Debug.LogError("ValueManager instance is null.");
        }
    }
}

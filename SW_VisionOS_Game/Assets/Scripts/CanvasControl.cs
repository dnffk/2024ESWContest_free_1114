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
        //float lpwrValue = ValueManager.Instance.Check_Lpwr;
        //ValueManager.Instance.Check_Lpwr -> 여기에 0에서 1사이의 값이 저장되어있음 이걸 이용해서 먼저 실린더의 밝기를 조절해주고 그 이후에 사용자가 자기 원하는대로 조절
        //밝기가 0.5 기준 이상이면 0.3정도 밝기로 실린더를조절해서 낮추고 0.5 이하일 경우에는 0.5로

        //slider.value = lpwrValue;
        /*if (lpwrValue > 0.5f)
        {
            slider.value = 0.3f; // 밝기를 낮게 설정
        }
        else
        {
            slider.value = 0.5f; // 밝기를 0.5로 설정
        }*/

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
        float saveBrightness = slider.value;
        PlayerPrefs.SetFloat("Brightness", saveBrightness);
    }
}

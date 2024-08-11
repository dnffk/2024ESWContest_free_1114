using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameCanvasControl : MonoBehaviour
{
    public Image image;

    void Start()
    {
        if (PlayerPrefs.HasKey("CanvasAlpha"))
        {
            float alpha = PlayerPrefs.GetFloat("CanvasAlpha")/100f;
            SetImageAlpha(alpha);
        }
    }

    private void SetImageAlpha(float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha; // 알파 값 설정
            image.color = color; // 변경된 색상 적용
        }
    }
}
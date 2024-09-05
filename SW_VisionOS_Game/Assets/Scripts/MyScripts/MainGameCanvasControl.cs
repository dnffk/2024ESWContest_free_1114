using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameCanvasControl : MonoBehaviour
{
    public Image image;

    void Start()
    {
        if(PlayerPrefs.HasKey("Lightness"))
        {
            float AAAA = PlayerPrefs.GetFloat("Lightness");
            SetImageAlpha(AAAA);
            Debug.Log("밝" + AAAA);
        }
    }

    private void SetImageAlpha(float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha; // 알파 값 설정
            image.color = color; // 변경된 색상 적용
            Debug.Log("랄" + alpha);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasHP : MonoBehaviour
{
    public Image uiImage; // UI 이미지 컴포넌트

    // Start is called before the first frame update
    void Start()
    {
        uiImage = GetComponent<Image>();
        if (uiImage == null)
        {
            Debug.LogError("UIFollowCamera script needs to be attached to a GameObject with an Image component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAlpha();
    }

    private void UpdateAlpha()
    {
        float hp = GhostManager.Instance.HP;

        if (hp <= 50)
        {
            // 체력이 100일 때는 검정색, 50일 때는 빨간색으로 변하도록 계산
            float redValue = Mathf.Clamp01((50 - hp) / 50); // hp가 줄어들수록 빨간색 비율 증가
            uiImage.color = new Color(redValue, 0, 0, uiImage.color.a); // 기존 알파값 유지
        }
    }
}
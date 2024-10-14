using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightButtonManager : MonoBehaviour
{
    public GameObject vignette;
    public GameObject canvas;

    // 손전등 상태 저장 (켜짐: 1, 꺼짐: 0)
    private int lightButtonState = 0;

    // 이전 프레임의 버튼 입력 상태 저장
    private int previousButtonInput = 0;

    void Start()
    {
        // 초기 상태는 손전등이 꺼진 상태로 설정
        UpdateLightState(lightButtonState);
    }

    void Update()
    {
        // 버튼 입력 값
        int currentButtonInput = ValueManager.Instance.Check_lightButton;

        // 버튼이 0에서 1로 바뀔 때만 토글
        if (previousButtonInput == 0 && currentButtonInput == 1)
        {
            // 손전등 상태 토글 (0 -> 1 또는 1 -> 0)
            lightButtonState = lightButtonState == 1 ? 0 : 1;

            // 손전등 상태 업데이트
            UpdateLightState(lightButtonState);
        }

        // 현재 버튼 입력을 이전 상태로 업데이트
        previousButtonInput = currentButtonInput;
    }

    // 손전등 상태 업데이트 함수
    private void UpdateLightState(int state)
    {
        if (state == 1)
        {
            Debug.Log("손전등 켜짐");
            canvas.SetActive(false);
            vignette.SetActive(true);
        }
        else if (state == 0)
        {
            Debug.Log("손전등 꺼짐");
            canvas.SetActive(true);
            vignette.SetActive(false);
        }
    }
}
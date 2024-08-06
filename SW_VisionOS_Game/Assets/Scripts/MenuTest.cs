using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiSceneButtonController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonUIElementPair
    {
        public Button button;
        public GameObject uiElement;
        public GameObject iconElement;
    }

    public List<ButtonUIElementPair> buttonUIPairs;

    private void Start()
    {
        // 각 버튼 상태 로드 및 리스너 추가
        foreach (var pair in buttonUIPairs)
        {
            string key = pair.uiElement.name + "State";
            bool isButtonPressed = PlayerPrefs.GetInt(key, 0) == 1;
            pair.uiElement.SetActive(isButtonPressed);
            pair.iconElement.SetActive(!isButtonPressed);

            pair.button.onClick.AddListener(() => OnButtonPressed(pair.uiElement, pair.iconElement));
        }
    }

    private void OnButtonPressed(GameObject uiElement, GameObject iconElement)
    {
        // 버튼 상태 저장
        string key = uiElement.name + "State";
        bool isButtonPressed = PlayerPrefs.GetInt(key, 0) == 1;
        PlayerPrefs.SetInt(key, isButtonPressed ? 0 : 1);
        PlayerPrefs.Save();

        // UI 토글
        uiElement.SetActive(!isButtonPressed);
        iconElement.SetActive(isButtonPressed);
    }
}

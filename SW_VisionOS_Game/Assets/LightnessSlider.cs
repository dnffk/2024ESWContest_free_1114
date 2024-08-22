using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LightnessSlider : MonoBehaviour
{
    private Slider slider;
    //private GameObject this.gameObject;
    // Start is called before the first frame update
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때마다 오브젝트들의 위치를 플레이어 기준으로 업데이트
        slider = this.GetComponent<Slider>();
        this.slider.value = ValueManager.Instance.Check_Lpwr;
    }
    // Update is called once per frame
    void OnDisable()
    {
        // 씬 로드 이벤트 핸들러 해제 (오브젝트가 파괴될 때)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
public class XROriginManager : MonoBehaviour
{
    private XROrigin xrOrigin;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드된 후 XROrigin을 초기화합니다.
        xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            InitializeXROrigin();
        }
        else
        {
            Debug.LogWarning("XROrigin을 찾을 수 없습니다.");
        }
    }
    private void InitializeXROrigin()
    {
        // XROrigin의 상태를 초기화하는 로직을 여기에 추가합니다.
        xrOrigin.transform.position = Vector3.zero;
        xrOrigin.transform.rotation = Quaternion.identity;
        // Debug 로그 추가
        Debug.Log("XROrigin 초기화 완료: 위치와 회전 초기화");
    }
}
using UnityEngine;

public class FollowVRCamera : MonoBehaviour
{
    public Transform cameraTransform; // 카메라의 Transform을 저장할 변수
    public Vector3 offset = new Vector3(0, 0, 0); // 카메라와 오브젝트 사이의 오프셋 값

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // 메인 카메라의 Transform을 기본값으로 설정
        }
    }

    void Update()
    {
        Follow();
    }

    void Follow()
    {
        if (cameraTransform != null)
        {
            transform.position = cameraTransform.position + offset; // 카메라 위치 + 오프셋 값으로 오브젝트 위치 설정
            transform.rotation = cameraTransform.rotation; // 카메라의 회전 값으로 오브젝트 회전 설정 (필요에 따라 제거 가능)
        }
    }
}

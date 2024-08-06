using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform vrCameraTransform; // VR 카메라의 Transform을 참조할 변수
    public float followDistance = 2.0f; // 오브젝트가 VR 카메라 앞에서 유지할 거리

    void Start()
    {
        if (vrCameraTransform == null)
        {
            // 기본적으로 메인 카메라를 사용하도록 설정
            vrCameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Follow();
    }

    void Follow()
    {
        if (vrCameraTransform != null)
        {
            // 카메라의 위치와 회전으로부터 전방 벡터를 구해 일정 거리만큼 떨어진 위치를 계산
            Vector3 targetPosition = vrCameraTransform.position + vrCameraTransform.forward * followDistance;
            
            // 오브젝트를 목표 위치로 이동
            transform.position = targetPosition;
            // 오브젝트가 항상 카메라를 향하도록 회전 설정
            transform.LookAt(2 * transform.position - vrCameraTransform.position);
        }
    }
}

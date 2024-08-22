using UnityEngine;

public class CameraFollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // 따라갈 대상 (예: 카메라)
    public Transform objectToMove; // 따라가는 주체 (예: 오브젝트)
    public float distanceFromCamera = 0.5f; // 카메라로부터의 거리
    public float yOffset = 0f; // Y축 오프셋 추가 (중앙을 기준으로 움직임)
    public float rotationSpeed = 30f; // 오브젝트의 회전 속도

    private float angle = 0f; // 현재 각도

    void OnEnable()
    {
        // 스크립트가 활성화될 때 각도 초기화
        angle = 0f;
    }

    void Update()
    {
        if (cameraTransform != null && objectToMove != null) // 대상과 주체가 할당되었는지 확인
        {
            // 시간에 따라 각도를 증가시켜 원을 그림
            angle += rotationSpeed * Time.deltaTime;

            // 각도를 라디안으로 변환
            float radians = angle * Mathf.Deg2Rad;

            // 새로운 위치 계산 (카메라를 기준으로 수직 반원을 그리며 이동)
            Vector3 newPosition = cameraTransform.position;
            newPosition.y += Mathf.Sin(radians) * distanceFromCamera; // Y축에서만 변화
            newPosition.z += Mathf.Cos(radians) * distanceFromCamera; // Z축에서만 변화

            // Y축 오프셋 추가
            newPosition.y += yOffset;

            // 오브젝트의 위치 설정
            objectToMove.position = newPosition;

            // 오브젝트가 항상 카메라를 바라보도록 설정
            //objectToMove.LookAt(cameraTransform);
        }
    }

    // PinchSpawn에서 각도 값을 설정할 수 있는 메서드 추가
    public void SetAngleValue(float deltaTime)
    {
        angle += rotationSpeed * deltaTime;
    }
}

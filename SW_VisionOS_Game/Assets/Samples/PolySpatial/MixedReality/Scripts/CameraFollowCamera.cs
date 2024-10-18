using UnityEngine;

public class CameraFollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // 따라갈 대상 (예: 카메라)
    public Transform objectToMove; // 따라가는 주체 (예: 오브젝트)
    public float distanceFromCamera = 0.5f; // 카메라로부터의 거리
    public float yOffset = -0.2f; // Y축 오프셋 추가

    void Update()
    {
        if (cameraTransform != null && objectToMove != null) // 대상과 주체가 할당되었는지 확인
        {
            // 카메라와의 거리를 유지하면서 Y축 오프셋을 적용해 오브젝트의 위치를 설정
            Vector3 newPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            newPosition.y += yOffset; // Y축 오프셋 적용
            objectToMove.position = newPosition;
        }

        Vector3 rotationEulerAngles = objectToMove.rotation.eulerAngles;
        rotationEulerAngles.y = cameraTransform.rotation.eulerAngles.y; // Y축(좌우) 회전값 고정
        objectToMove.rotation = Quaternion.Euler(rotationEulerAngles.x, 0, rotationEulerAngles.z);
    }
}
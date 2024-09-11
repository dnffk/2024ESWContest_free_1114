using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public Camera playerCamera; // 플레이어의 카메라
    public float distanceFromCamera = 0.1f; // 카메라로부터의 거리
    public Vector3 offset = new Vector3(0, 0, 0); // UI 오브젝트의 위치 오프셋

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // 플레이어 카메라가 설정되지 않았다면 메인 카메라로 설정
        }
    }

    private void Update()
    {
        // 카메라 앞 일정한 거리로 UI 오브젝트의 위치를 설정
        Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * distanceFromCamera;
        targetPosition += offset;

        // UI 오브젝트의 위치를 업데이트
        transform.position = targetPosition;

        // UI 오브젝트의 회전을 카메라의 회전과 동일하게 설정
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.transform.position);
    }
}

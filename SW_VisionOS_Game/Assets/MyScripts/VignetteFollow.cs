using UnityEngine;

public class VignetteFollow : MonoBehaviour
{
    public Transform xrOrigin; // XR Origin 트랜스폼
    public float followSpeed = 0.1f; // 비네트 오브젝트가 따라오는 속도

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // 목표 위치는 XR Origin의 위치입니다.
        Vector3 targetPosition = xrOrigin.position;
        Quaternion targetRotation = xrOrigin.rotation;

        // 비네트 오브젝트의 위치를 부드럽게 목표 위치로 보간합니다.
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);

        // 비네트 오브젝트의 회전을 부드럽게 목표 회전으로 보간합니다.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, followSpeed);
    }
}

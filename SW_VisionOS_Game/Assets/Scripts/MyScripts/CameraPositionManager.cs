using Unity.Mathematics;
using UnityEngine;

public class CameraPositionManager : MonoBehaviour
{
    public Transform targetObj;
    void Update()
    {
        Vector3 targetPosition = targetObj.position + targetObj.forward * 0.334f;
            
            // 오브젝트를 목표 위치로 이동
        transform.position = targetPosition;
        transform.LookAt(targetObj.position - targetObj.forward);
    }
}

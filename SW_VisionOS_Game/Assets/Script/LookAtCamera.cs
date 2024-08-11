using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        // 메인 카메라 
        Camera mainCamera = Camera.main;

        // 카메라가 null이 아닐 때
        if (mainCamera != null)
        {
            // 오브젝트의 방향을 카메라 방향으로 설정
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0; // y축 회전을 방지 수평으로
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}

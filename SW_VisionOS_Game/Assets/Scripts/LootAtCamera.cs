using UnityEngine;

public class LookAtVRCamera : MonoBehaviour
{
    public Transform vrCameraTransform; // VR 카메라의 Transform을 참조할 변수

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
        LookAtCamera();
    }

    void LookAtCamera()
    {
        if (vrCameraTransform != null)
        {
            // 오브젝트가 항상 카메라를 향하도록 회전 설정
            transform.LookAt(2 * transform.position - vrCameraTransform.position);
        }
    }
}

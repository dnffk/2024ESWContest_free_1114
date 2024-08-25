using UnityEngine;
using UnityEngine.XR.Hands;

public class HandControlledCamera : MonoBehaviour
{
    public Transform cameraTransform; // 카메라의 트랜스폼
    public PolySpatial.Samples.HandVisualizer handVisualizer; // Hand Visualizer 스크립트의 참조
    public bool useRightHand = true; // 오른손을 사용할지 여부

    private XRHandSubsystem m_Subsystem;
    private Transform handTransform; // 실제 손의 트랜스폼
    private float initialHandHeight;
    private float maxYOffset;

    void Start()
    {
        if (handVisualizer != null)
        {
            // 손의 트랜스폼을 초기화
            m_Subsystem = handVisualizer.GetSubsystem();
            if (useRightHand)
            {
                handTransform = handVisualizer.GetRightHandTransform();
            }
            else
            {
                handTransform = handVisualizer.GetLeftHandTransform();
            }

            if (handTransform != null)
            {
                initialHandHeight = handTransform.position.y;
                maxYOffset = cameraTransform.position.y; // 초기 카메라의 Y 오프셋 저장
            }
            else
            {
                Debug.LogError("Hand Transform is not assigned or found!");
            }
        }
        else
        {
            Debug.LogError("HandVisualizer is not assigned!");
        }
    }

    void Update()
    {
        if (handTransform != null)
        {
            float handYOffset = handTransform.position.y - initialHandHeight;
            float newCameraY = maxYOffset + handYOffset;

            // 카메라의 Y 위치가 maxYOffset 값을 초과하지 않도록 설정
            if (newCameraY > maxYOffset)
            {
                newCameraY = maxYOffset;
            }

            // 카메라 위치 업데이트
            Vector3 newCameraPosition = new Vector3(cameraTransform.position.x, newCameraY, cameraTransform.position.z);
            cameraTransform.position = newCameraPosition;
        }
    }
}

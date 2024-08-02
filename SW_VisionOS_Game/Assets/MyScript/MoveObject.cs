using UnityEngine;
using Unity.PolySpatial.InputDevices; //PolySpatial 입력장치 관련 클래스
using UnityEngine.InputSystem.EnhancedTouch; //향상된 터치 시스템 클래스
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; //Touch로 간략화하여 참조
using TouchPhase = UnityEngine.InputSystem.TouchPhase; //TouchPhase로 간략화하여 참조
using UnityEngine.InputSystem.LowLevel;

public class MoveObject : MonoBehaviour
{
    private GameObject selectedObject; //선택된 객체를 저장할 변수
    private Vector3 lastPosition; //터치한 마지막 위치를 저장할 변수
    public float rotationSpeed = 0.2f;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (Touch.activeTouches.Count == 1)
        {
            TouchMovement();
        }
        else if (Touch.activeTouches.Count == 2)
        {
            TouchRotation();
        }
        else
        {
            selectedObject = null;
        }
    }

    private void TouchMovement()
    {
        foreach (var touch in Touch.activeTouches)
        { //활성화 된 모든 터치 이벤트 순회
            SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch); //현재 터치 이벤트 상태를 가져와 touchData 객체 생성

            if (touchData.targetObject != null && touchData.Kind != SpatialPointerKind.Touch)
            { //Direct Touch 제외
                if (touch.phase == TouchPhase.Began)
                { //터치가 시작되었을 때 초기 위치를 기록
                    if (touchData.targetObject.CompareTag("Cube")) // 태그가 Cube인지 확인
                    {
                        selectedObject = touchData.targetObject; //선택한 오브젝트 햘댱
                        lastPosition = touchData.interactionPosition;
                    }
                }
                else if (touch.phase == TouchPhase.Moved && selectedObject != null)
                { //터치한 객체가 이동 중일 때 마지막 위치로부터 이동거리 계산 후 업데이트
                    Vector3 deltaPosition = touchData.interactionPosition - lastPosition; //움직인 위치 - 초기 위치로 이동거리 계산
                    selectedObject.transform.position += deltaPosition; //선택했던 오브젝트의 위치에 이동거리만큼 증가
                    lastPosition = touchData.interactionPosition;
                }
            }
        }
    }

    private void TouchRotation()
    {
        Touch touch0 = Touch.activeTouches[0];
        Touch touch1 = Touch.activeTouches[1];

        SpatialPointerState touch0Data = EnhancedSpatialPointerSupport.GetPointerState(touch0);
        SpatialPointerState touch1Data = EnhancedSpatialPointerSupport.GetPointerState(touch1);

        Vector3 touch0Pos = touch0Data.interactionPosition;
        Vector3 touch1Pos = touch1Data.interactionPosition;

        Vector3 initialVector = touch0Pos - touch1Pos;

        Vector3 previousVector = initialVector;
        Vector3 currentVector = touch0Pos - touch1Pos;

        Vector3 rotationAxis = Vector3.Cross(previousVector, currentVector).normalized;
        float angle = Vector3.Angle(previousVector, currentVector);

        if (selectedObject != null)
        {
            selectedObject.transform.Rotate(rotationAxis, angle * rotationSpeed, Space.World);
        }

        previousVector = currentVector;
    }
}

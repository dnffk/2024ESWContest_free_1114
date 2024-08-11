using System.Collections;
using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using TMPro;
public class IntroTakePhoto : MonoBehaviour
{
    public Toggle targetToggle;
    public JY_CanvasFlash JYCanvasIntroFlash;
    private bool isProcessing = false;
    private GameObject QRObject; // 여기서는 QRObject 대신 UI 이미지나 텍스트를 가리키는 RectTransform을 사용
    public TMP_Text ShowNoCaptureText;
    public Image targetImage; // QRObject 대신 감지할 Image
    // 뷰포트의 가장자리를 무시할 마진 설정 (너비와 높이)
    private float verticalMargin = 0.1f;
    private float horizontalMargin = 0.05f;
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    void Update()
    {
        if (isProcessing) return; // 이미 스크린샷 처리 중이라면 무시
        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                    if (touchData.targetObject != null && touchData.Kind == SpatialPointerKind.Touch)
                    {
                        if (JY_IntroFilmControl.Flim > 0) // 필름이 남아 있을 때만 스크린샷 촬영
                        {
                            isTakePhotoQR();
                        }
                        break;
                    }
                }
            }
        }
    }
    private void isTakePhotoQR()
    {
        JYCanvasIntroFlash.TriggerFlash();
        if (IsRenderingQRObject())
        {
            JY_IntroFilmControl.Flim--; // 필름 개수 감소
            TriggerToggle();
        }
        else
        {
            StartCoroutine(ShowNoCaptureMessage());
        }
    }
    private bool IsRenderingQRObject()
    {
        if (targetImage != null)
        {
            if (IsVisibleFrom(targetImage))
            {
                Debug.LogError("Target Image is visible!");
                return true;
            }
        }
        return false;
    }
    private bool IsVisibleFrom(Image image)
    {
        Camera selectedCamera = GameObject.Find("PhotoCamera").GetComponent<Camera>();
        Debug.Log("도착;");
        if (selectedCamera == null)
        {
            Debug.LogError("PhotoCamera not found!");
            return false;
        }
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCornersCount = 0;
        foreach (Vector3 corner in corners)
        {
            Vector3 viewportPoint = selectedCamera.WorldToViewportPoint(corner);
            if (viewportPoint.x >= horizontalMargin && viewportPoint.x <= 1 - horizontalMargin &&
                viewportPoint.y >= verticalMargin && viewportPoint.y <= 1 - verticalMargin &&
                viewportPoint.z > 0) // z > 0 은 카메라 앞에 있는지를 확인
            {
                visibleCornersCount++;
            }
        }
        // 4개의 꼭지점 중 절반 이상이 카메라에 보이면 true 반환
        return visibleCornersCount >= 2;
    }
    private void TriggerToggle()
    {
        if (targetToggle != null)
        {
            targetToggle.isOn = !targetToggle.isOn;
        }
    }
    private IEnumerator ShowNoCaptureMessage()
    {
        ShowNoCaptureText.text = "Try again!"; // 메시지 설정
        ShowNoCaptureText.gameObject.SetActive(true); // 메시지 활성화
        yield return new WaitForSeconds(3f); // 3초 동안 대기
        ShowNoCaptureText.gameObject.SetActive(false); // 메시지 비활성화
    }
}
using System.Collections;
using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement; // SceneManager를 사용하기 위한 네임스페이스 추가
using System.IO;
using System;

public class TakePhoto : MonoBehaviour
{
    public Camera cameraToCapture; // 캡처할 카메라
    public RenderTexture renderTexture; // RenderTexture 템플릿 (필요 시 동적으로 생성)
    public JY_CanvasFlash jyCanvasFlash;

    private bool isProcessing = false; // 스크린샷 처리 중인지 여부를 추적
    private float cooldownTime = 5.0f; // 촬영 후 대기 시간 (초 단위)
    private float lastCaptureTime = 0f; // 마지막 촬영 시간

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        if (isProcessing) return; // 이미 스크린샷 처리 중이라면 무시
        if (Time.time - lastCaptureTime < cooldownTime) return; // 대기 시간 중이면 무시

        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                    if (touchData.targetObject != null && touchData.Kind == SpatialPointerKind.Touch)
                    {
                        if (CameraText.Flim > 0) // 필름이 남아 있을 때만 스크린샷 촬영
                        {
                            StartCoroutine(CaptureScreenshot());
                        }
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator CaptureScreenshot()
    {
        isProcessing = true; // 스크린샷 처리 시작

        jyCanvasFlash.TriggerFlash();

        CameraText.Flim--; // 필름 개수 감소

        yield return new WaitForEndOfFrame();

        // RenderTexture를 동적으로 생성
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraToCapture.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        cameraToCapture.Render();

        // RenderTexture에서 Texture2D 생성
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        // RenderTexture와 카메라 원래 상태로 되돌리기
        cameraToCapture.targetTexture = null;
        RenderTexture.active = null;

        // 스크린샷을 파일로 저장
        string folderPath = Path.Combine(Application.persistentDataPath, "Capture");
        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Destroy(renderTexture);
        Destroy(screenshot);

        lastCaptureTime = Time.time; // 마지막 촬영 시간 갱신
        isProcessing = false; // 스크린샷 처리 완료

        if(CameraText.Flim == 0)
        {
            StartCoroutine(TransitionToEndingScene());
        }
    }

    private IEnumerator TransitionToEndingScene()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Ending1");

        CameraText.Flim = 5;
    }
}
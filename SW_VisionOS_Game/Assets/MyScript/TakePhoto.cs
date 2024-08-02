using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가
using UnityEngine.UI; // RawImage를 사용하기 위한 네임스페이스 추가
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
using System;

public class TakePhoto : MonoBehaviour
{
    public TextMeshProUGUI screenshotMessageText; // 스크린샷 메시지를 표시할 TextMeshProUGUI UI 요소
    public RawImage screenshotDisplay; // 스크린샷을 표시할 RawImage UI 요소
    public Camera cameraToCapture; // 캡처할 카메라
    public RenderTexture renderTexture; // RenderTexture 템플릿 (필요 시 동적으로 생성)

    private bool isProcessing = false; // 스크린샷 처리 중인지 여부를 추적

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        screenshotMessageText.gameObject.SetActive(false); // 초기에는 메시지 비활성화
        screenshotDisplay.gameObject.SetActive(false); // 초기에는 스크린샷 표시 UI 비활성화
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
                        StartCoroutine(CaptureScreenshot());
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator CaptureScreenshot()
    {
        isProcessing = true; // 스크린샷 처리 시작

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

        try
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);

            // 저장된 파일을 불러와 RawImage에 할당
            StartCoroutine(LoadScreenshot(filePath, fileName));
        }
        catch (Exception e)
        {
            ShowMessage("Failed: " + e.Message);
        }

        // 메모리 정리
        Destroy(renderTexture);
        Destroy(screenshot);

        isProcessing = false; // 스크린샷 처리 완료
    }

    private IEnumerator LoadScreenshot(string filePath, string fileName)
    {
        // 파일을 읽어 텍스처 생성
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        yield return new WaitForEndOfFrame();

        // RawImage에 텍스처 적용 및 파일 이름 표시
        screenshotDisplay.texture = texture;
        screenshotDisplay.gameObject.SetActive(true);

        ShowMessage("Screenshot loaded: " + fileName);

        // 메시지와 스크린샷 표시를 30초 후에 숨기기
        Invoke("HideScreenshot", 30f);
    }

    private void ShowMessage(string message)
    {
        screenshotMessageText.text = message;
        screenshotMessageText.gameObject.SetActive(true);
    }

    private void HideScreenshot()
    {
        screenshotMessageText.gameObject.SetActive(false);
        screenshotDisplay.gameObject.SetActive(false);
    }
}

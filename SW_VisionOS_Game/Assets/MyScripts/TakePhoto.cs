using System.Collections;
using UnityEngine;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement; // SceneManager를 사용하기 위한 네임스페이스 추가
using System.IO;
using System;

public class TakePhoto : MonoBehaviour
{
    public Camera cameraToCapture;
    public RenderTexture renderTexture;
    public JY_CanvasFlash jyCanvasFlash;
    public CameraView TakePhotoCameraView;
    public AudioManager AudioManagerStop;
    public AudioSource ShutSound;

    private bool isProcessing = false;
    //private bool hasTakenPhoto = false;
    private int visiblePhotoCount = 0;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        Debug.Log("aa는 " + ValueManager.Instance.Check_shutButton);

        if (isProcessing)
        {
            return;
        }

        else{
            if (ValueManager.Instance.Check_shutButton == 1)
            {
                Debug.Log("필름이 남아있음 ");
                if (CameraText.Flim > 0) // 필름이 남아 있을 때만 스크린샷 촬영
                {
                    TakePhotoCameraView.CheckValidEnemiesVisibility();
                    StartCoroutine(CaptureScreenshot());
                    //hasTakenPhoto = true; // 사진 촬영 후 플래그 설정
                }
            }

            /*else if (ValueManager.Instance.Check_shutButton == 0)
            {
                //hasTakenPhoto = false; // 버튼이 다시 눌릴 수 있도록 플래그 초기화
            }*/
        }
        
    }

    private IEnumerator CaptureScreenshot()
    {
        isProcessing = true;

        jyCanvasFlash.TriggerFlash();
        ShutSound.Play();

        CameraText.Flim--;
        Debug.Log("wait");
        yield return null;
        Debug.Log("waitend");
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraToCapture.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        cameraToCapture.Render();
        Debug.Log("1렌더 ");
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();
        Debug.Log("2렌더 ");
        cameraToCapture.targetTexture = null;
        RenderTexture.active = null;
        Debug.Log("3렌 ");
        string folderPath;
        if (TakePhotoCameraView.isObjectVisibleJY)
        {
            Debug.Log("귀신이 찍혔어요");
            folderPath = Path.Combine(Application.persistentDataPath, "Capture");
            visiblePhotoCount++;
        }
        else
        {
            Debug.Log("귀신이 안찍혔어요");
            folderPath = Path.Combine(Application.persistentDataPath, "noCapture");
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        TakePhotoCameraView.isObjectVisibleJY = false;

        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Destroy(renderTexture);
        Destroy(screenshot);        

        isProcessing = false;
        Debug.Log("isprocessing false");
        Debug.Log("귀신 사진이 몇? : " + visiblePhotoCount);

        if (CameraText.Flim == 0)
        {
            if (visiblePhotoCount >= 3)
            {
                StartCoroutine(TransitionToEnding2Scene());
            }
            else
            {
                StartCoroutine(TransitionToEnding1Scene());
            }
        }
    }

    private IEnumerator TransitionToEnding1Scene()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Ending1");

        CameraText.Flim = 100;
        AudioManagerStop.StopAudio();
    }

    private IEnumerator TransitionToEnding2Scene()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Ending2");

        CameraText.Flim = 100;
        AudioManagerStop.StopAudio();
    }
}
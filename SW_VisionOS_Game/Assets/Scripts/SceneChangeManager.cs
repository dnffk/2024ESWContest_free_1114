using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneManager : MonoBehaviour
{
    private string currentSceneName;
    
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void GotoIntroScene()
    {
        StartCoroutine(FadeOut());
        string sceneName = "IntroScene";
        ValueManager.Instance.ShutterCounter(0);
        ValueManager.Instance.CompletedShutterCounter(0);
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoSettingScene()
    {
        StartCoroutine(FadeOut());
        string sceneName = "SettingLightness";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoSceneTest()
    {
        StartCoroutine(FadeOut());
        string sceneName = "TestScene";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoUITest()
    {
        StartCoroutine(FadeOut());
        string sceneName = "UI Test";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoEnding1()
    {
        StartCoroutine(FadeOut());
        string sceneName = "Ending1";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoEnding2()
    {
        StartCoroutine(FadeOut());
        string sceneName = "Ending2";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoEnding3()
    {
        StartCoroutine(FadeOut());
        string sceneName = "Ending3";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoMain()
    {
        StartCoroutine(FadeOut());
        string sceneName = "MainGame";
        StartCoroutine(WaitTime(sceneName));
    }
    public void GotoMenu()
    {
        StartCoroutine(FadeOut());
        string sceneName = "MenuScene";
        InitializeFolders();
        StartCoroutine(WaitTime(sceneName));
    }
    
    public CanvasGroup canvasGroup; // CanvasGroup 컴포넌트를 참조
    public float fadeDuration = 1.0f; // Fade Out 효과가 지속되는 시간 (초 단위)

    public GameObject loadingScreen;

    private IEnumerator WaitTime(string sceneName)
    {
        yield return new WaitForSeconds(1.0f);
        Resources.UnloadUnusedAssets();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        ValueManager.Instance.ChangeScene(sceneName, currentSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentSceneName);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
        currentSceneName = sceneName;
    }

    private void InitializeFolders()
    {
        string CaptureFolderPath = Path.Combine(Application.persistentDataPath, "Capture");
        string noCaptureFolderPath = Path.Combine(Application.persistentDataPath, "noCapture");

        // Capture 폴더 초기화
        if (Directory.Exists(CaptureFolderPath))
        {
            Directory.Delete(CaptureFolderPath, true); // 폴더와 그 안의 모든 파일 삭제
        }
        Directory.CreateDirectory(CaptureFolderPath); // 폴더 다시 생성
        
        // ghostCapture 폴더 초기화
        if (Directory.Exists(noCaptureFolderPath))
        {
            Directory.Delete(noCaptureFolderPath, true); // 폴더와 그 안의 모든 파일 삭제
        }
        Directory.CreateDirectory(noCaptureFolderPath); // 폴더 다시 생성
    }

    private IEnumerator FadeOut()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is not assigned.");
            yield break;
        }
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha = Mathf.Clamp01(1.0f - elapsedTime / fadeDuration);
            }
            yield return null;
        }

        canvasGroup.alpha = 0.0f; // 완료 시 완전 투명
    }
}

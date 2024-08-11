using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using System.IO;
using System;

public class CaptureObjectFile : MonoBehaviour
{
    public TextMeshProUGUI screenshotMessageText;
    public RawImage screenshotDisplay;
    public Camera cameraToCapture;
    public RenderTexture renderTexture;

    private bool isProcessing = false;
    private string[] validTags = { "Black", "White", "Mask", "Organ" };

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        screenshotMessageText.gameObject.SetActive(false);
        screenshotDisplay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isProcessing) return;

        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    SpatialPointerState touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                    if (touchData.targetObject != null && touchData.Kind == SpatialPointerKind.Touch)
                    {
                        if (CameraText.Flim > 0)
                        {
                            CameraText.Flim--;
                            StartCoroutine(CaptureScreenshot());
                        }
                        else
                        {
                            ShowMessage("0");
                        }
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator CaptureScreenshot()
    {
        isProcessing = true;
        string folderPath;

        float visiblePercentage = CheckVisiblePercentage();

        if (visiblePercentage >= 0.7f)
        {
            folderPath = Path.Combine(Application.persistentDataPath, "ghostCapture");
        }
        else
        {
            folderPath = Path.Combine(Application.persistentDataPath, "noCapture");
        }

        yield return new WaitForEndOfFrame();

        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraToCapture.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        cameraToCapture.Render();

        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        cameraToCapture.targetTexture = null;
        RenderTexture.active = null;

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

            StartCoroutine(LoadScreenshot(filePath, fileName));
        }
        catch (Exception e)
        {
            ShowMessage("Failed: " + e.Message);
        }

        Destroy(renderTexture);
        Destroy(screenshot);

        isProcessing = false;
    }

    private float CheckVisiblePercentage()
    {
        float maxVisiblePercentage = 0f;

        foreach (string tag in validTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                Bounds combinedBounds = new Bounds(obj.transform.position, Vector3.zero);
                foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }

                Vector3[] corners = new Vector3[8];
                corners[0] = cameraToCapture.WorldToViewportPoint(combinedBounds.min);
                corners[1] = cameraToCapture.WorldToViewportPoint(new Vector3(combinedBounds.min.x, combinedBounds.min.y, combinedBounds.max.z));
                corners[2] = cameraToCapture.WorldToViewportPoint(new Vector3(combinedBounds.min.x, combinedBounds.max.y, combinedBounds.min.z));
                corners[3] = cameraToCapture.WorldToViewportPoint(new Vector3(combinedBounds.min.x, combinedBounds.max.y, combinedBounds.max.z));
                corners[4] = cameraToCapture.WorldToViewportPoint(new Vector3(combinedBounds.max.x, combinedBounds.min.y, combinedBounds.min.z));
                corners[5] = cameraToCapture.WorldToViewportPoint(new Vector3(combinedBounds.max.x, combinedBounds.min.y, combinedBounds.max.z));
                corners[6] = cameraToCapture.WorldToViewportPoint(new Vector3(combinedBounds.max.x, combinedBounds.max.y, combinedBounds.min.z));
                corners[7] = cameraToCapture.WorldToViewportPoint(combinedBounds.max);

                int visibleCornersCount = 0;
                foreach (var corner in corners)
                {
                    if (corner.x >= 0 && corner.x <= 1 && corner.y >= 0 && corner.y <= 1 && corner.z > 0)
                    {
                        visibleCornersCount++;
                    }
                }

                float visiblePercentage = (float)visibleCornersCount / corners.Length;
                maxVisiblePercentage = Mathf.Max(maxVisiblePercentage, visiblePercentage);
            }
        }

        return maxVisiblePercentage;
    }

    private IEnumerator LoadScreenshot(string filePath, string fileName)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        yield return new WaitForEndOfFrame();

        screenshotDisplay.texture = texture;
        screenshotDisplay.gameObject.SetActive(true);

        ShowMessage("loaded: " + fileName);

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

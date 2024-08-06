using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public RenderTexture renderTexture;
    public RawImage rawImage; // UI Raw Image 요소

    void Start()
    {
        // Render Texture 생성 및 설정
        renderTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
        if (!renderTexture.Create())
        {
            Debug.LogError("RenderTexture creation failed!");
            return;
        }
        Camera.main.targetTexture = renderTexture;

        // Raw Image 텍스처 설정
        if (rawImage != null)
        {
            rawImage.texture = renderTexture;
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (renderTexture == null)
        {
            Graphics.Blit(src, dest);
        }
        else
        {
            // src를 renderTexture로 블릿(Blt)
            Graphics.Blit(src, renderTexture);
            // renderTexture를 dest로 블릿(Blt)
            Graphics.Blit(renderTexture, dest);
        }
    }

    void OnDisable()
    {
        // Render Texture 해제
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }

        // 카메라의 targetTexture 해제
        Camera.main.targetTexture = null;
    }
}

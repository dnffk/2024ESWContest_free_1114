using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JY_CanvasFlash : MonoBehaviour
{
    public RawImage flashImage;
    public float flashDuration = 0.5f; // 플래시 효과 지속 시간

    private void Start()
    {
        if (flashImage != null)
        {
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
        }
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsedTime = 0f;
        Color originalColor = flashImage.color;
        Color targetColor = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 1);

        // Fade in
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            flashImage.color = Color.Lerp(originalColor, targetColor, elapsedTime / (flashDuration / 2));
            yield return null;
        }

        // 잠시 유지
        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;

        // Fade out
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            flashImage.color = Color.Lerp(targetColor, originalColor, elapsedTime / (flashDuration / 2));
            yield return null;
        }

        flashImage.color = originalColor;
    }
}

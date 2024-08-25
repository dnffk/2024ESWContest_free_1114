using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JY_CanvasFlash : MonoBehaviour
{
    public RawImage flashImage;
    public float flashDuration = 1.5f; // 플래시 효과 지속 시간

    private Coroutine flashRoutine; // 현재 실행 중인 플래시 코루틴을 추적

    private void Start()
    {
        if (flashImage != null)
        {
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
        }
    }

    public void TriggerFlash()
    {
        // 기존의 코루틴이 실행 중이면 실행하지 않음
        if (flashRoutine != null)
        {
            return;
        }
        // 새로운 플래시 코루틴 시작
        flashRoutine = StartCoroutine(FlashRoutine());
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

        // 정확하게 최종 상태로 설정
        flashImage.color = targetColor;

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

        // 정확하게 원래 상태로 설정
        flashImage.color = originalColor;

        // 코루틴 끝났음을 표시
        flashRoutine = null;
    }
}

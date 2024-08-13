using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RenderManager : MonoBehaviour
{
    public float fadeDuration = 2.0f; // 페이드 아웃 시간
    public AudioSource Scream;
    public GameObject ghostObj;
    public ParticleSystem particle;
    public Renderer[] ghostRenders;
    private bool isFadingOut = false;

    private void Start()
    {
        ghostRenders = ghostObj.GetComponentsInChildren<Renderer>();
    }

    // 이 함수를 호출하여 페이드 아웃 효과를 시작
    public void StartFadeOut(float del)
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutCoroutine(del));
        }
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine(float del)
    {
        //모든 렌더러를 찾기  
        Debug.Log($"render count: {ghostRenders.Length}");

        float time = 0f;

        //particle.Play();

        while (time <= del)
        {
            time += Time.deltaTime;

            // 현재 진행 중인 페이드 비율을 계산

            float alpha = 1 - Mathf.Clamp01(time / del); // Fade Out

            foreach (Renderer r in ghostRenders)
            {
                // 현재 색상을 가져오기 
                Color color = r.material.color;

                // 알파 값을 감소
                color.a = alpha;
                r.material.color = color;
            }

            yield return null;
        }

        // 지정된 시간 대기
        yield return new WaitForSeconds(particle.main.duration);
        //yield return new WaitUntil(() => particle != null && !particle.isEmitting && particle.particleCount == 0);
        //yield return new WaitUntil(() => particle.isPlaying == false);

        GameObject returnGhost = ghostObj.transform.root.gameObject;
        GhostManager.Instance.ReturnObjectQueue(returnGhost);

        isFadingOut = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        // 모든 렌더러를 찾기 
        Debug.Log($"render count: {ghostRenders.Length}");

        float time = 0f;
        //particle.Play();
        foreach (Renderer r in ghostRenders)
        {
            Color color = r.material.color;
            color.a = 0f;
            r.material.color = color;
        }

        while (time <= fadeDuration)
        {
            time += Time.deltaTime;

            // 현재 진행 중인 페이드 비율을 계산하기 
            float alpha = Mathf.Clamp01(time / fadeDuration); // Fade In

            foreach (Renderer r in ghostRenders)
            {
                // 현재 색상을 가져오기 
                Color color = r.material.color;

                // 알파 값을 감소
                color.a = alpha;
                r.material.color = color;
            }

            yield return null;
        }
    }

    public void PlayParticle()
    {
        Debug.LogError("Particle 재생 전 호출");
        if (particle == null)
        {
            Debug.LogError("Particle 시스템 할당 안 됨");
            return;
        }
        particle.Play(); // 파티클 재생
        Debug.Log("파티클 재생 시작");
        Scream.Play();
        // 지정된 시간 후에 파티클을 정지하는 코루틴 호출
        StartCoroutine(StopParticleAfterDelay(3.5f));
    }

    private IEnumerator StopParticleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        particle.Stop(); // 파티클 정지
        Debug.Log("파티클 재생 종료");
    }
}
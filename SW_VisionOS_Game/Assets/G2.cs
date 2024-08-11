using UnityEngine;

public class FadingPhantomBehavior : MonoBehaviour
{
    public float minAppearTime = 1.0f; // 최소 등장 시간
    public float maxAppearTime = 3.0f; // 최대 등장 시간
    public float disappearTime = 2.0f; // 사라지는 시간
    public float minSpawnDistance = 2.0f; // 최소 생성 거리
    public float maxSpawnDistance = 5.0f; // 최대 생성 거리
    public float detectionRadius = 5.0f; // 플레이어 감지 반경

    private Transform playerTransform;
    public Renderer ghostRenderer;
    private float timer;
    private bool isVisible;
    private bool isFading;
    private float fadeStartTime;
    private Color originalColor;
    public ParticleSystem ps;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        isVisible = false;
        isFading = false;
        SetRandomTimer();
        originalColor = ghostRenderer.material.color;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (isVisible)
            {
                if (!isFading)
                {
                    FadeOut();
                }
            }
            else
            {
                Appear();
            }
        }

        if (isVisible && !isFading)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRadius)
            {
                LookAtPlayer();
                FadeOut();
            }
        }

        if (isFading)
        {
            ps.Play();
            float t = (Time.time - fadeStartTime) / disappearTime;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1, 0, t);
            ghostRenderer.material.color = newColor;
            

            if (newColor.a <= 0)
            {
                isFading = false;
                isVisible = false;
                ghostRenderer.enabled = false;
                SetRandomTimer();
                ps.Stop();
            }
        }
    }

    void SetRandomTimer()
    {
        timer = Random.Range(minAppearTime, maxAppearTime);
    }

    void Appear()
    {
        Vector3 spawnDirection = -playerTransform.forward;
        Vector3 randomOffset = Random.insideUnitSphere * maxSpawnDistance;
        randomOffset.y = 0; // 높이를 동일하게 유지
        Vector3 spawnPosition = playerTransform.position + spawnDirection * minSpawnDistance + randomOffset;
        spawnPosition = new Vector3(spawnPosition.x, playerTransform.position.y, spawnPosition.z);

        transform.position = spawnPosition;

        ghostRenderer.enabled = true;
        ghostRenderer.material.color = originalColor;
        isVisible = true;
        isFading = false;
        SetRandomTimer();
    }

    void FadeOut()
    {
        isFading = true;
        fadeStartTime = Time.time;
    }

    void LookAtPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = lookRotation;
    }
}

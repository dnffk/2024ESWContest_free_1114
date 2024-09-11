using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class NavGhost : MonoBehaviour
{
    public float moveSpeed = 0.4f; // 오브젝트 이동 속도
    public float stopDistance = 0.3f; // 플레이어와의 최소 거리
    public float minBetweenGhosts = 0.5f; // 귀신 오브젝트 간 최소 거리
    public float soundTriggerDistance = 3.0f; // 소리 재생 거리
    public float sightDuration = 3.0f; // 플레이어 시야 내에 머무는 시간
    public float waitAfterTeleport = 2.0f; // 순간 이동 후 대기 시간
    private AudioSource audioSource; // 오디오 소스 컴포넌트
    private bool isInView = false; // 화면 내에 있는지 여부
    private float timeInView = 0f; // 화면 내에 머무른 시간
    private Transform playerTransform; // 플레이어의 트랜스폼
    private List<ARPlane> planes; // PlaneList
    private float waitTime = 0f; // 순간 이동 후 대기 시간 체크
    private bool hasTeleported = false; // 순간 이동 여부
    private int teleportCount = 0; // 순간 이동 카운트
    public float teleportRadius = 20.0f; // 순간 이동 반경 (G1 기능)
    public float VecMoveSpeed = 3.0f; // 좌우 이동 속도 (G1 기능)
    public float changeDirectionTime = 2.0f; // 방향 변경 시간 (G1 기능)
    public float minAppearTime = 1.0f; // 최소 등장 시간 (G1 기능)
    public float maxAppearTime = 3.0f; // 최대 등장 시간 (G1 기능)
    public float minDisappearTime = 0.5f; // 최소 사라지는 시간 (G1 기능)
    public float maxDisappearTime = 1.5f; // 최대 사라지는 시간 (G1 기능)
    public float g1MoveSpeed = 2.0f;
    private Vector3 moveDirection;
    private float timer;
    private bool isVisible;
    private float directionChangeTimer;
    private Animator animator; // 애니메이터 컴포넌트 추가
    float heightOffset = 0.55f;
    [HideInInspector]
    public bool canMove = false; // 이동 가능 여부
    [HideInInspector]
    public bool soundPlayed = false; // 소리가 재생되었는지 여부
    private IGhostMove currentMove;
    private GhostManager.Level level; // 현재 난이도
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 추가
        playerTransform = Camera.main.transform;
        planes = GhostManager.Instance.planes;
        animator = GetComponentInChildren<Animator>(); // 애니메이터 컴포넌트 추가
        switch (this.tag)
        {
            case "Mask":
                heightOffset = 0.2f;  // GhostType1의 높이 오프셋
                break;
            case "Black":
                heightOffset = 1.1f;  // GhostType2의 높이 오프셋
                break;
            case "White":
                heightOffset = 1.1f;  // GhostType3의 높이 오프셋
                break;
            case "Organ":
                heightOffset = 0.8f;  // GhostType3의 높이 오프셋
                break;
            default:
                heightOffset = 0.55f; // 기본 오프셋 값
                break;
        }
        Debug.Log("태그" + this.tag + "높이 오프셋" + heightOffset);
    }
    public void ResetNavGhost()
    {
        canMove = false;
        soundPlayed = false;
        isInView = false;
        timeInView = 0f;
        waitTime = 0f;
        hasTeleported = false;
        teleportCount = 0;
        if (animator != null)
        {
            animator.enabled = true;
        }
        Debug.Log("NavGhost 초기화 완료");
    }
    private void Update()
    {
        if (canMove)
        {
            currentMove.Move(this);
        }
    }
    public void SetMove(IGhostMove ghostMove)
    {
        currentMove = ghostMove;
        Debug.Log("SetMove called: " + ghostMove.GetType().Name);
    }
    public void SetLevel(GhostManager.Level level)
    {
        this.level = level;
        switch (level)
        {
            case GhostManager.Level.Easy:
                moveSpeed = 0.4f;
                break;
            case GhostManager.Level.Medium:
                moveSpeed = 0.7f;
                sightDuration = 5.0f;
                g1MoveSpeed = 1.0f;
                VecMoveSpeed = 2.0f;
                break;
            case GhostManager.Level.Hard:
                moveSpeed = 1.0f;
                sightDuration = 3.0f;
                g1MoveSpeed = 2.0f;
                VecMoveSpeed = 3.0f;
                break;
        }
        Debug.Log($"moveSpeed: {moveSpeed}, sightDuration: {sightDuration}"); // 변경된 값 확인
    }
    public void StartMoving(float delay)
    {
        StartCoroutine(MoveAfterDelay(delay));
    }
    private IEnumerator MoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }
    public void MoveObjectTowardsPlayer()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 currentPosition = transform.position;
        float distanceToPlayer = Vector3.Distance(currentPosition, cameraPosition);
        // 플레이어의 몸통 높이에 맞추기 위한 값
        Vector3 targetPosition = new Vector3(cameraPosition.x, cameraPosition.y - heightOffset, cameraPosition.z);
        // 플레이어와의 거리 체크 및 소리 재생
        if (!soundPlayed && distanceToPlayer <= soundTriggerDistance)
        {
            PlaySound();
        }
        if (distanceToPlayer > stopDistance)
        {
            Vector3 direction = (targetPosition - currentPosition).normalized; // 목표 위치로 방향 설정
            transform.position += direction * moveSpeed * Time.deltaTime;
            // 귀신 간 거리 조정
            NavGhost[] allGhosts = FindObjectsOfType<NavGhost>();
            foreach (NavGhost otherGhost in allGhosts)
            {
                if (otherGhost != this)
                {
                    float distanceBetween = Vector3.Distance(currentPosition, otherGhost.transform.position);
                    if (distanceBetween < minBetweenGhosts)
                    {
                        Vector3 adjustmentDirection = (currentPosition - otherGhost.transform.position).normalized;
                        transform.position += adjustmentDirection * (minBetweenGhosts - distanceBetween);
                    }
                }
            }
        }
    }
    // G1 클래스의 등장 및 사라짐 로직
    public void UpdateVisibility()
    {
        timer -= Time.deltaTime;
        directionChangeTimer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (isVisible)
            {
                Disappear();
            }
            else
            {
                Appear();
            }
        }
    }
    // G1 클래스의 이동 로직
    public void GhostMovement()
    {
        if (isVisible)
        {
            // 좌우 이동
            if (directionChangeTimer <= 0)
            {
                SetRandomDirection();
                directionChangeTimer = changeDirectionTime;
            }
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, g1MoveSpeed * Time.deltaTime);
            transform.position += moveDirection * VecMoveSpeed * Time.deltaTime;
            // 플레이어 감지 및 좌우 접근
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20);
            ApproachPlayer();
        }
    }
    private void SetRandomTimer()
    {
        if (isVisible)
        {
            timer = Random.Range(minDisappearTime, maxDisappearTime);
        }
        else
        {
            timer = Random.Range(minAppearTime, maxAppearTime);
        }
    }
    private void Appear()
    {
        Vector3 randomPosition = playerTransform.position + (Random.insideUnitSphere * teleportRadius);
        randomPosition.y = playerTransform.position.y; // 플레이어와 같은 높이로 설정
        transform.position = randomPosition;
        if (animator != null)
        {
            animator.enabled = true; // 애니메이터 활성화
        }
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
        isVisible = true;
        SetRandomTimer();
    }
    private void Disappear()
    {
        if (animator != null)
        {
            animator.enabled = false; // 애니메이터 비활성화
        }
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        isVisible = false;
        SetRandomTimer();
    }
    private void SetRandomDirection()
    {
        int direction = Random.Range(0, 2);
        moveDirection = direction == 0 ? Vector3.left : Vector3.right;
    }
    private void ApproachPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.z))
        {
            // 좌우로 접근
            if (directionToPlayer.x > 0)
            {
                moveDirection = Vector3.right;
            }
            else
            {
                moveDirection = Vector3.left;
            }
        }
    }
    public void CheckIfInView()
    {
        Debug.Log("CheckIfInView called");
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0)
        {
            if (!isInView)
            {
                isInView = true;
                timeInView = 0f; // 시야에 처음 들어왔을 때 시간 초기화
            }
            timeInView += Time.deltaTime;
            if (timeInView >= sightDuration)
            {
                if (teleportCount < 3)
                {
                    TeleportToRandomPointOnPlane();
                    teleportCount++;
                }
                else
                {
                    TeleportBehindPlayer();
                    teleportCount = 0;
                }
                timeInView = 0f; // 순간 이동 후 시간 초기화
            }
        }
        else
        {
            isInView = false;
            timeInView = 0f; // 시야에서 벗어났을 때 시간 초기화
        }
        if (hasTeleported)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= waitAfterTeleport)
            {
                hasTeleported = false; // 대기 시간이 끝나면 다시 이동 가능
                waitTime = 0f; // 대기 시간 초기화
            }
            return;
        }
        if (!hasTeleported)
        {
            MoveObjectTowardsPlayer();
        }
    }
    private void TeleportBehindPlayer()
    {
        Vector3 behindPlayer = playerTransform.position - playerTransform.forward * 2f;
        transform.position = behindPlayer;
        hasTeleported = true;
        PlaySound();
    }
    public void TeleportToRandomPointOnPlane()
    {
        if (planes.Count == 0) return;
        int randomIndex = Random.Range(0, planes.Count);
        ARPlane randomPlane = planes[randomIndex];
        if (randomPlane == null) return;
        Vector3 randomPoint = GetRandomPointOnPlane(randomPlane);
        int maxAttempts = 10; // 최대 시도 횟수 설정
        int attempts = 0;
        while (Vector3.Distance(playerTransform.position, randomPoint) < soundTriggerDistance && attempts < maxAttempts)
        {
            randomPoint = GetRandomPointOnPlane(randomPlane);
            attempts++;
        }
        if (attempts == maxAttempts)
        {
            Debug.LogWarning("Couldn't find a random point far enough from the player.");
        }
        transform.position = randomPoint;
        hasTeleported = true;
    }
    private Vector3 GetRandomPointOnPlane(ARPlane plane)
    {
        Vector2 size = plane.size;
        float randomX = Random.Range(-size.x / 2, size.x / 2);
        float randomZ = Random.Range(-size.y / 2, size.y / 2);
        return plane.transform.TransformPoint(new Vector3(randomX, 0, randomZ));
    }
    private void PlaySound()
    {
        audioSource.Play(); // 소리 재생
        soundPlayed = true; // 소리가 재생되었음을 표시
    }
}
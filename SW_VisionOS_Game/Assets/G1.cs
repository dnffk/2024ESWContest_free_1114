using UnityEngine;

public class G1 : MonoBehaviour
{
    // 랜덤 등장 및 사라짐 관련 변수
    public float minAppearTime = 1.0f; // 최소 등장 시간
    public float maxAppearTime = 3.0f; // 최대 등장 시간
    public float minDisappearTime = 0.5f; // 최소 사라지는 시간
    public float maxDisappearTime = 1.5f; // 최대 사라지는 시간
    public float teleportRadius = 5.0f; // 순간 이동 반경

    // 이동 관련 변수
    public float moveSpeed = 2.0f; // 이동 속도
    public float VecMoveSpeed = 2.0f; // 이동 속도
    public float changeDirectionTime = 2.0f; // 방향 변경 시간

    private Transform playerTransform;
    public Renderer ghostRenderer;
    private Vector3 moveDirection;
    private float timer;
    private bool isVisible;
    private float directionChangeTimer;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        isVisible = false;
        SetRandomTimer();
        directionChangeTimer = changeDirectionTime;
    }

    void Update()
    {
        // 타이머 업데이트
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

        GhostMovement();
    }

    void GhostMovement()
    {
        if (isVisible)
        {
            // 좌우 이동
            if (directionChangeTimer <= 0)
            {
                SetRandomDirection();
                directionChangeTimer = changeDirectionTime;
            }

            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            // 귀신 이동
            transform.position += moveDirection * VecMoveSpeed * Time.deltaTime;

            // 플레이어 감지 및 좌우 접근
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20);
            ApproachPlayer();

            if (distanceToPlayer < 0.5)
            {
                Disappear();
            }
        }
    }

    void SetRandomTimer()
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

    void Appear()
    {
        Vector3 randomPosition = playerTransform.position + (Random.insideUnitSphere * teleportRadius);
        randomPosition.y = playerTransform.position.y; // 플레이어와 같은 높이로 설정
        transform.position = randomPosition;

        ghostRenderer.enabled = true;
        isVisible = true;
        SetRandomTimer();
    }

    void Disappear()
    {
        ghostRenderer.enabled = false;
        isVisible = false;
        SetRandomTimer();
    }

    void SetRandomDirection()
    {
        // 좌우 랜덤한 방향 설정
        int direction = Random.Range(0, 2);

        switch (direction)
        {
            case 0:
                moveDirection = Vector3.left; // 왼쪽
                break;
            case 1:
                moveDirection = Vector3.right; // 오른쪽
                break;
        }
    }

    void ApproachPlayer()
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
}

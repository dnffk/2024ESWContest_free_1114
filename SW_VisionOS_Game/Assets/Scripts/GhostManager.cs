using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GhostManager : MonoBehaviour
{
    private static GhostManager instanace = null;
    public static GhostManager Instance
    { get { return instanace; } }

    private Queue<GameObject> objectQueue = new Queue<GameObject>();
    private List<GameObject> curObjects = new List<GameObject>();
    [HideInInspector]
    public List<ARPlane> planes = new List<ARPlane>(); // 감지된 바닥 객체들을 저장하는 리스트

    public ARPlaneManager arPlaneManager; // 플레인 메니저의 인스턴스 저장
    public List<GameObject> ghostPrefab;
    public int maxGhostCount;
    public int maxPoolCount;
    public float delayTime;
    public float HP = 100;

    public enum Level
    {
        Easy,
        Medium,
        Hard
    }
    public Level level;

    private void Awake()
    {
        if (instanace == null)
        {
            instanace = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SetDifficultyFromValueManager();
        //arPlaneManager = GetComponent<ARPlaneManager>(); // 플레인 메니저 컴포넌트 가져오기
        arPlaneManager.planesChanged += OnPlaneChanged; // 플레인 변경 이벤트에 핸들러 추가
        //level = Level.Medium;
        //maxPoolCount = Random.Range(1, maxGhostCount);
        SetMaxPoolCount();
        InitObjectQueue();
        StartCoroutine(SpawnGhostCor());
    }

    void OnPlaneChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.added)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                planes.Add(plane);
            }
        }

        foreach (var plane in eventArgs.updated)
        {
            planes.RemoveAll(ARPlane => ARPlane == null);
        }
    }

    private void SetMaxPoolCount()
    {
        switch (level)
        {
            case Level.Easy:
                maxPoolCount = Random.Range(4, 7);
                break;
            case Level.Medium:
                maxPoolCount = Random.Range(4, 7);
                break;
            case Level.Hard:
                maxPoolCount = Random.Range(7, maxGhostCount);
                break;
        }
    }

    private void InitObjectQueue()
    {
        for (int i = 0; i < maxPoolCount; i++)
        {
            int randomPrefab = i % ghostPrefab.Count; // 순차적으로 프리팹 선택
            var obj = Instantiate(ghostPrefab[randomPrefab]);
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }
    }

    private GameObject GetObjectQueue()
    {
        if (objectQueue.Count <= 0)
        {
            return null;
        }

        GameObject obj = objectQueue.Dequeue();
        obj.SetActive(true);

        curObjects.Add(obj);

        return obj;
    }

    public void ReturnObjectQueue(GameObject obj)
    {
        if (curObjects.Contains(obj) == false)
        {
            Debug.Log("반환 대상 아님");
            return;
        }
        obj.SetActive(false);
        curObjects.Remove(obj);
        objectQueue.Enqueue(obj);
    }

    private void PlaceGhost()
    {
        int randomIndex = Random.Range(0, planes.Count);
        ARPlane randomPlane = planes[randomIndex];
        if (randomPlane == null)
        {
            Debug.LogWarning("선택된 ARPlane이 null입니다.");
            return;
        }
        Vector3 randomPoint = GetRandomPointOnPlane(randomPlane);

        GameObject ghost = GetObjectQueue();
        ghost.transform.position = randomPoint;
        ghost.transform.rotation = Quaternion.identity;

        RenderManager renderManager = ghost.GetComponent<RenderManager>();
        renderManager.StartFadeIn();

        NavGhost navGhost = ghost.GetComponent<NavGhost>();
        navGhost.SetLevel(level);
        navGhost.canMove = false;
        navGhost.soundPlayed = false;
        Debug.Log(level);
        if (navGhost != null)
        {
            switch (level)
            {
                case Level.Easy:
                    navGhost.SetMove(new EasyMove());
                    break;
                case Level.Medium:
                    navGhost.SetMove(new MediumMove());
                    break;
                case Level.Hard:
                    navGhost.SetMove(new HardMove());
                    break;
            }
            navGhost.StartMoving(2f); // 2초 후에 이동 시작
        }
    }

    protected Vector3 GetRandomPointOnPlane(ARPlane plane)
    {
        if (plane == null)
        {
            Debug.LogWarning("ARPlane이 null입니다.");
            return Vector3.zero; // 기본값 반환
        }

        Vector2 size = plane.size;
        float randomX = Random.Range(-size.x / 2, size.x / 2);
        float randomZ = Random.Range(-size.y / 2, size.y / 2);

        return plane.transform.TransformPoint(new Vector3(randomX, 0, randomZ));
    }

    private IEnumerator SpawnGhostCor()
    {
        while (true)
        {
            if (planes == null || planes.Count == 0)
            {
                yield return null; // 다음 프레임으로 대기
                continue; // 계속 반복
            }

            // 현재 필드에 최대 갯수의 고스트가 존재하면 더 이상 생성할 필요가 없으므로 계속 continue
            if (curObjects.Count >= maxPoolCount)
            {
                yield return null;
                continue;
            }

            // 현재 필드에 고스트가 최대 갯수보다 적다면 생성
            yield return new WaitForSeconds(delayTime);
            delayTime = Random.Range(15, 20);
            PlaceGhost();


        }
    }

    private void SetDifficultyFromValueManager()
    {
        int difficultyValue = ValueManager.Instance.Game_Difficulty;

        switch (difficultyValue)
        {
            case 0:
                level = Level.Easy;
                break;
            case 1:
                level = Level.Medium;
                break;
            case 2:
                level = Level.Hard;
                break;
            default:
                Debug.LogWarning("Unknown difficulty value: " + difficultyValue);
                level = Level.Medium; // 기본값 설정
                break;
        }
    }
}
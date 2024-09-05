using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectSpawner : MonoBehaviour
{
    public ARPlaneManager arPlaneManager; // ARPlaneManager 참조

    // Inspector에서 각각 할당할 프리팹들 (기존 오브젝트 최대 5개)
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;
    public GameObject prefab5;

    // 핏자국 프리팹들 (최대 8개)
    public GameObject bloodPrefab1;
    public GameObject bloodPrefab2;
    public GameObject bloodPrefab3;
    public GameObject bloodPrefab4;
    public GameObject bloodPrefab5;
    public GameObject bloodPrefab6;

    private List<ARPlane> planes = new List<ARPlane>(); // 감지된 ARPlane 저장
    private int objectSpawnCount = 0;
    private int bloodSpawnCount = 0;
    private int maxObjectSpawns = 5;
    private int maxBloodSpawns = 6;

    private void Start()
    {
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        // 새로 감지된 바닥면(ARPlane) 저장
        foreach (var plane in eventArgs.added)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp) // 바닥면만 처리
            {
                planes.Add(plane);
            }
        }

        // 업데이트된 바닥면에서 null을 제거
        planes.RemoveAll(plane => plane == null);

        // 오브젝트 생성 시도
        TrySpawnAllObjects();
    }

    private void TrySpawnAllObjects()
    {
        // 일반 오브젝트 생성
        if (objectSpawnCount < maxObjectSpawns)
        {
            SpawnPrefabObject(prefab1);
            SpawnPrefabObject(prefab2);
            SpawnPrefabObject(prefab3);
            SpawnPrefabObject(prefab4);
            SpawnPrefabObject(prefab5);
        }

        // 핏자국 오브젝트 생성
        if (bloodSpawnCount < maxBloodSpawns)
        {
            SpawnBloodObject(bloodPrefab1);
            SpawnBloodObject(bloodPrefab2);
            SpawnBloodObject(bloodPrefab3);
            SpawnBloodObject(bloodPrefab4);
            SpawnBloodObject(bloodPrefab5);
            SpawnBloodObject(bloodPrefab6);
        }

        // 모두 생성된 경우 스크립트 비활성화
        if (objectSpawnCount >= maxObjectSpawns && bloodSpawnCount >= maxBloodSpawns)
        {
            enabled = false;
        }
    }

    private void SpawnPrefabObject(GameObject prefab)
    {
        if (prefab == null || objectSpawnCount >= maxObjectSpawns)
        {
            return;
        }

        // 무작위 ARPlane 선택
        int randomIndex = Random.Range(0, planes.Count);
        ARPlane selectedPlane = planes[randomIndex];

        if (selectedPlane == null)
        {
            Debug.LogWarning("선택된 ARPlane이 null입니다.");
            return;
        }

        // 무작위 위치 선택
        Vector3 randomPosition = GetRandomPointOnPlane(selectedPlane);

        // 오브젝트 생성
        GameObject newObject = Instantiate(prefab, randomPosition, Quaternion.identity);

        // Rigidbody와 Collider 추가 (프리팹에 없을 경우)
        if (!newObject.GetComponent<Rigidbody>())
        {
            Rigidbody rb = newObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }

        if (!newObject.GetComponent<Collider>())
        {
            newObject.AddComponent<BoxCollider>(); // Collider 유형은 오브젝트에 맞게 선택
        }

        objectSpawnCount++;
    }

    private void SpawnBloodObject(GameObject prefab)
    {
        if (prefab == null || bloodSpawnCount >= maxBloodSpawns)
        {
            return;
        }

        // 무작위 ARPlane 선택
        int randomIndex = Random.Range(0, planes.Count);
        ARPlane selectedPlane = planes[randomIndex];

        if (selectedPlane == null)
        {
            Debug.LogWarning("선택된 ARPlane이 null입니다.");
            return;
        }

        // 무작위 위치 선택
        Vector3 randomPosition = GetRandomPointOnPlane(selectedPlane);

        // 오브젝트 생성
        GameObject newObject = Instantiate(prefab, randomPosition, Quaternion.Euler(90, 0, 0)); // 바닥에 맞게 회전

        // Collider 추가 (프리팹에 없을 경우)
        if (!newObject.GetComponent<Collider>())
        {
            newObject.AddComponent<BoxCollider>(); // Collider 유형은 오브젝트에 맞게 선택
        }

        bloodSpawnCount++;
    }

    private Vector3 GetRandomPointOnPlane(ARPlane plane)
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
}

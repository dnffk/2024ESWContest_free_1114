using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public bool isObjectVisibleJY = false;
    public Camera viewCamera;
    //public AudioSource Scream;
    private string[] validTags = { "Black", "White", "Mask", "Organ" }; // 유효한 태그 목록
    private float verticalMargin = 0.1f; // 뷰포트 상하 가장자리 무시할 비율
    private float horizontalMargin = 0.05f; // 뷰포트 좌우 가장자리 무시할 비율

    private void Update()
    {
        //CheckValidEnemiesVisibility();
    }

    public void CheckValidEnemiesVisibility()
    {
        Debug.Log("판별 호출 ");
        isObjectVisibleJY = false;

        // 유효한 태그를 가진 오브젝트 탐색 
        foreach (string tag in validTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                if(isObjectVisibleJY)
                {
                    return;
                }
                CheckObjectVisibility(obj);
            }
        }
    }

    private void CheckObjectVisibility(GameObject obj)
    {
        // 모든 자식 오브젝트의 Bounds를 계산
        Bounds combinedBounds = new Bounds(obj.transform.position, Vector3.zero);

        // 자식 오브젝트를 포함하여 콜라이더의 경계 박스 계산
        foreach (var collider in obj.GetComponentsInChildren<Collider>())
        {
            combinedBounds.Encapsulate(collider.bounds);
        }

        // 경계 박스를 상하로 절반 나누기
        Bounds upperHalfBounds = new Bounds(
            new Vector3(combinedBounds.center.x, combinedBounds.center.y + combinedBounds.extents.y / 2, combinedBounds.center.z),
            new Vector3(combinedBounds.size.x, combinedBounds.size.y / 2, combinedBounds.size.z)
        );

        Bounds lowerHalfBounds = new Bounds(
            new Vector3(combinedBounds.center.x, combinedBounds.center.y - combinedBounds.extents.y / 2, combinedBounds.center.z),
            new Vector3(combinedBounds.size.x, combinedBounds.size.y / 2, combinedBounds.size.z)
        );

        // 경계 박스를 좌우로 절반 나누기
        Bounds leftHalfBounds = new Bounds(
            new Vector3(combinedBounds.center.x - combinedBounds.extents.x / 2, combinedBounds.center.y, combinedBounds.center.z),
            new Vector3(combinedBounds.size.x / 2, combinedBounds.size.y, combinedBounds.size.z)
        );

        Bounds rightHalfBounds = new Bounds(
            new Vector3(combinedBounds.center.x + combinedBounds.extents.x / 2, combinedBounds.center.y, combinedBounds.center.z),
            new Vector3(combinedBounds.size.x / 2, combinedBounds.size.y, combinedBounds.size.z)
        );

        // 상단 절반 경계 박스의 모든 꼭짓점 계산
        Vector3[] upperCorners = GetViewportCorners(upperHalfBounds);
        Vector3[] lowerCorners = GetViewportCorners(lowerHalfBounds);
        Vector3[] leftCorners = GetViewportCorners(leftHalfBounds);
        Vector3[] rightCorners = GetViewportCorners(rightHalfBounds);

        // 상단 절반 화면 내에 있는 꼭짓점 수 세기
        int visibleUpperCornersCount = CountVisibleCorners(upperCorners);
        int visibleLowerCornersCount = CountVisibleCorners(lowerCorners);
        int visibleLeftCornersCount = CountVisibleCorners(leftCorners);
        int visibleRightCornersCount = CountVisibleCorners(rightCorners);

        // 가시성 판단: 상단 또는 하단 절반의 50% 이상 보이는지 여부
        float visibleUpperPercentage = (float)visibleUpperCornersCount / upperCorners.Length;
        float visibleLowerPercentage = (float)visibleLowerCornersCount / lowerCorners.Length;
        float visibleLeftPercentage = (float)visibleLeftCornersCount / leftCorners.Length;
        float visibleRightPercentage = (float)visibleRightCornersCount / rightCorners.Length;

        // 경계 박스가 화면에 보이는지 여부 판단
        if ((visibleUpperPercentage >= 0.5f || visibleLowerPercentage >= 0.5f || visibleLeftPercentage >= 0.5f || visibleRightPercentage >= 0.5f) &&
            IsWithinCentralViewport(upperCorners) && IsWithinCentralViewport(lowerCorners) && IsWithinCentralViewport(leftCorners) && IsWithinCentralViewport(rightCorners))
        {
            isObjectVisibleJY = true;
            Debug.Log($"{obj.name} is visible on the screen (50% 이상 상단, 하단, 좌측 또는 우측 절반).");
            //Destroy(obj);
            RenderManager render = obj.GetComponent<RenderManager>();
            NavGhost navGhost = obj.GetComponent<NavGhost>();

            if (navGhost != null)
            {
                Debug.Log("멈춤 ");
                navGhost.canMove = false;
            }

            if (render != null)
            {
                Debug.Log("귀신사망 ");
                render.PlayParticle();
                //Scream.Play();
                Debug.Log("소리 플레이 ");
                render.StartFadeOut(3f);
                Debug.Log("귀신 페이드아웃 ");
            }
        }
        else
        {
            isObjectVisibleJY = false;
            Debug.Log("아무것도안찍힘 ");
            //Debug.Log($"{obj.name} is not visible on the screen (50% 이하 상단, 하단, 좌측 및 우측 절반).");
        }
    }

    private Vector3[] GetViewportCorners(Bounds bounds)
    {
        Vector3[] corners = new Vector3[8];
        corners[0] = viewCamera.WorldToViewportPoint(bounds.min);
        corners[1] = viewCamera.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
        corners[2] = viewCamera.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
        corners[3] = viewCamera.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        corners[4] = viewCamera.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
        corners[5] = viewCamera.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
        corners[6] = viewCamera.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
        corners[7] = viewCamera.WorldToViewportPoint(bounds.max);
        return corners;
    }

    private int CountVisibleCorners(Vector3[] corners)
    {
        int visibleCornersCount = 0;

        foreach (var corner in corners)
        {
            if (corner.x >= horizontalMargin && corner.x <= 1 - horizontalMargin && corner.y >= verticalMargin && corner.y <= 1 - verticalMargin && corner.z > 0)
            {
                visibleCornersCount++;
            }
        }

        return visibleCornersCount;
    }

    private bool IsWithinCentralViewport(Vector3[] corners)
    {
        foreach (var corner in corners)
        {
            if (corner.x > horizontalMargin && corner.x < 1 - horizontalMargin && corner.y > verticalMargin && corner.y < 1 - verticalMargin)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        // 유효한 태그를 가진 모든 게임 오브젝트를 찾습니다.
        foreach (string tag in validTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                // 모든 자식 오브젝트의 Bounds를 계산
                Bounds combinedBounds = new Bounds(obj.transform.position, Vector3.zero);

                // 자식 오브젝트를 포함하여 콜라이더의 경계 박스 계산
                foreach (var collider in obj.GetComponentsInChildren<Collider>())
                {
                    combinedBounds.Encapsulate(collider.bounds);
                }

                // 경계 박스를 시각적으로 그리기
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(combinedBounds.center, combinedBounds.size);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCapture : MonoBehaviour
{
    private string[] validTags = { "Black", "White", "Mask", "Organ" }; // 유효한 태그 목록
    private List<GameObject> ghostObjects = new List<GameObject>();
    private List<Renderer> renderers = new List<Renderer>();
    private List<Collider> colliders = new List<Collider>();
    private List<Animator> animators = new List<Animator>();
    private bool isCoroutineRunning = false; // 코루틴 실행 중 여부

    void Start()
    {
        FindGhostObjectsWithValidTags();

        Debug.Log("Renderers found: " + renderers.Count);
        foreach (var renderer in renderers)
        {
            Debug.Log("Renderer: " + renderer.gameObject.name);
        }
        Debug.Log("Colliders found: " + colliders.Count);
        foreach (var collider in colliders)
        {
            Debug.Log("Collider: " + collider.gameObject.name);
        }
        Debug.Log("Animators found: " + animators.Count);
        foreach (var animator in animators)
        {
            Debug.Log("Animator: " + animator.gameObject.name);
        }
    }

    private void FindGhostObjectsWithValidTags()
    {
        foreach (string tag in validTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                ghostObjects.Add(obj);
                GetComponentsInChildrenRecursive(obj.transform);
            }
        }
    }

    public IEnumerator CaptureScreenshotWithGhostDeactivated()
    {
        isCoroutineRunning = true;
        Debug.Log("CaptureScreenshotWithGhostDeactivated started");

        foreach (var animator in animators)
        {
            animator.enabled = false;
            Debug.Log("Animator disabled: " + animator.gameObject.name);
        }
        yield return StartCoroutine(DeactivateTemporarily());
        Debug.Log("CaptureScreenshotWithGhostDeactivated completed");
        yield return null;
    }

    public IEnumerator DeactivateTemporarily()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
            Debug.Log("Renderer disabled: " + renderer.gameObject.name);
        }
        foreach (var collider in colliders)
        {
            collider.enabled = false;
            Debug.Log("Collider disabled: " + collider.gameObject.name);
        }
        yield return null;
    }

    public IEnumerator Reactivate()
    {
        Debug.Log("Reactivating ghost");
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
            Debug.Log("Renderer enabled: " + renderer.gameObject.name);
        }
        foreach (var collider in colliders)
        {
            collider.enabled = true;
            Debug.Log("Collider enabled: " + collider.gameObject.name);
        }
        foreach (var animator in animators)
        {
            animator.enabled = true;
            Debug.Log("Animator enabled: " + animator.gameObject.name);
        }
        isCoroutineRunning = false; // 여기서 설정
        Debug.Log("Reactivation complete");
        yield return null;
    }

    private void GetComponentsInChildrenRecursive(Transform parent)
    {
        Renderer[] childRenderers = parent.GetComponentsInChildren<Renderer>(true);
        Collider[] childColliders = parent.GetComponentsInChildren<Collider>(true);
        Animator[] childAnimators = parent.GetComponentsInChildren<Animator>(true);

        foreach (var renderer in childRenderers)
        {
            renderers.Add(renderer);
            Debug.Log("Found Renderer: " + renderer.gameObject.name);
        }
        foreach (var collider in childColliders)
        {
            colliders.Add(collider);
            Debug.Log("Found Collider: " + collider.gameObject.name);
        }
        foreach (var animator in childAnimators)
        {
            animators.Add(animator);
            Debug.Log("Found Animator: " + animator.gameObject.name);
        }

        foreach (Transform child in parent)
        {
            GetComponentsInChildrenRecursive(child); // 재귀적으로 자식 탐색
        }
    }

    public bool IsCoroutineRunning()
    {
        return isCoroutineRunning;
    }
}
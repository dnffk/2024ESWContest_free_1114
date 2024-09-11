using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GhostExit : MonoBehaviour
{
    private Trigger trigger;
    private void Awake()
    {
        // 메인 카메라에서 Trigger 스크립트를 찾습니다.
        trigger = Camera.main.GetComponent<Trigger>();
        if (trigger == null)
        {
            Debug.LogError("Trigger 스크립트를 메인 카메라에서 찾을 수 없습니다.");
        }
    }

    private void OnEnable()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();
        if (animators.Length > 0)
        {
            foreach (var animator in animators)
            {
                if (animator != null)
                {
                    animator.SetBool("Death", false);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (trigger == null) return;
        Collider collider = GetComponent<Collider>();
        if (collider != null && trigger.activeColliders.Contains(collider))
        {
            trigger.activeColliders.Remove(collider);
            Debug.Log("비활성화된 오브젝트가 activeColliders에서 제거됨");
        }
        ResetState();
    }
    private void ResetState()
    {
        NavGhost navGhost = GetComponent<NavGhost>();
        if (navGhost != null)
        {
            navGhost.canMove = false;
            navGhost.ResetNavGhost();
        }
        RenderManager renderManager = GetComponent<RenderManager>();
        if (renderManager != null)
        {
            renderManager.ResetRenderManager();
        }
    }
}
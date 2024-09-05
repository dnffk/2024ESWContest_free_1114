using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Trigger : MonoBehaviour
{
    private Dictionary<string, float> damageAmounts = new Dictionary<string, float>
    {
        { "Black", 10f },
        { "White", 10f },
        { "Mask", 15f },
        { "Organ", 20f }
    };
    private Dictionary<string, int> tagToNumberMapping = new Dictionary<string, int>
    {
        { "White", 1 },
        { "Black", 2 },
        { "Mask", 3 },
        { "Organ", 4 }
    };
    public HashSet<Collider> activeColliders = new HashSet<Collider>(); // 현재 충돌 중인 오브젝트들을 추적
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"충돌한 오브젝트 태그: {other.tag}");
        if (damageAmounts.ContainsKey(other.tag))
        {
            activeColliders.Add(other);
            if (activeColliders.Count >= 2)
            {
                Debug.Log("2개 이상의 충돌 감지됨, 여러개의 태그로 처리");
                TCPServer2.Instance.chkHitGhost(5);
                foreach (Collider collider in activeColliders)
                {
                    DamageAndReturnPool(collider, skipSend: true);
                }
            }
            else
            {
                DamageAndReturnPool(other, skipSend: false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 충돌에서 벗어날 때 오브젝트를 제거
        if (activeColliders.Contains(other))
        {
            activeColliders.Remove(other);
        }
    }
    private void Update()
    {
        if (GhostManager.Instance.HP <= 0)
        {
            Debug.Log("플레이어의 체력이 0 이하입니다. 씬을 이동합니다.");
            SceneManager.LoadScene("Ending3");
            return;
        }
        // 비활성화된 오브젝트 제거
        activeColliders.RemoveWhere(collider => collider == null || !collider.gameObject.activeInHierarchy);
    }
    private void DamageAndReturnPool(Collider collider, bool skipSend)
    {
        if (damageAmounts.ContainsKey(collider.tag))
        {
            float damageAmount = damageAmounts[collider.tag];
            GhostManager.Instance.HP -= damageAmount;
            Debug.Log($"{collider.tag} 오브젝트로부터 {damageAmount}의 피해를 입었습니다. 현재 HP: {GhostManager.Instance.HP}");
            if (!skipSend && tagToNumberMapping.ContainsKey(collider.tag))
            {
                Debug.Log("if문 내부에는 들어왔으려나 ?");
                int numberToSend = tagToNumberMapping[collider.tag];
                if (TCPServer2.Instance == null)
                {
                    Debug.Log("TCP.Instance가 null");
                }
                else
                {
                    TCPServer2.Instance.chkHitGhost(numberToSend);
                    Debug.Log("충돌한 오브젝트 진동 넘버" + numberToSend);
                }
            }
            Debug.Log("Send 메서드 이후의 코드 실행됨");
            NavGhost navGhost = collider.gameObject.GetComponent<NavGhost>();
            RenderManager render = collider.gameObject.GetComponent<RenderManager>();
            navGhost.canMove = false;
            render.StartFadeOut(2f);
            Debug.Log("navGhost 및 render 작업 완료");
        }
    }
}
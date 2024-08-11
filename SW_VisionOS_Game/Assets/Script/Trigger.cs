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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"충돌한 오브젝트 태그: {other.tag}"); // 로그 추가
        // 충돌한 오브젝트의 태그 확인
        if (damageAmounts.ContainsKey(other.tag))
        {
            // 체력을 감소시키고 오브젝트를 반환
            DamageAndReturnPool(other);
        }
    }

    private void DamageAndReturnPool(Collider collider)
    {
        if (GhostManager.Instance.HP <= 0)
        {
            Debug.Log("플레이어의 체력이 0 이하입니다. 씬을 이동합니다.");
            //SceneManager.LoadScene("GameOverScene"); // 이동할 씬 이름으로 변경
            return; // 함수 종료
        }

        // 현재 충돌 중인 오브젝트에 대해 체력 감소
        if (damageAmounts.ContainsKey(collider.tag))
        {
            float damageAmount = damageAmounts[collider.tag];
            GhostManager.Instance.HP -= damageAmount; // 플레이어 체력 감소
            Debug.Log($"{collider.tag} 오브젝트로부터 {damageAmount}의 피해를 입었습니다. 현재 HP: {GhostManager.Instance.HP}");

            NavGhost navGhost = collider.gameObject.GetComponent<NavGhost>();
            RenderManager render = collider.gameObject.GetComponent<RenderManager>();
            //render.PlayParticle();
            navGhost.canMove = false;
            render.StartFadeOut(2f);

            // 오브젝트 풀로 반환
            //GhostManager.Instance.ReturnObjectQueue(collider.gameObject);
        }
    }
}

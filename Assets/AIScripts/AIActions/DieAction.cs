using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DieAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float DieTime;
    private NavMeshAgent NV;
    private Transform Self;
    private Renderer[] mr;
    private Collider[] collider;
    private float DamageRate;

    private Coroutine co;

    public DieAction(AINew ai, string aniTriggerString, Renderer[] mr, NavMeshAgent nv, Transform self, Collider[] collider, Animator ani, float damageRate)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        this.mr = mr;
        NV = nv;
        Self = self;
        this.collider = collider;
        Ani = ani;
        DamageRate = damageRate;
    }

    public void TriggerAction()
    {
        if (co != null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
    }

    public bool CancelAction(AIState newState)
    {
        UpdateManager.Ins.StopCoroutineCustom(co);
        co = null;
        ai.SetComplete(true);
        return true;
    }

    IEnumerator Start()
    {
        NV.isStopped = true;
        NV.enabled = false;
        ai.SetComplete(false);


        Ani.SetTrigger(AniTriggerString);
        Debug.Log("死亡··············");

        foreach (Collider item in collider)
        {
            item.enabled = false;
        }

        float t = 0;
        while (mr[0].material.GetFloat("_RongJie") < 0.95f)
        {
            t += Time.deltaTime / 3;
            foreach (Renderer meshRenderer in mr)
            {
                meshRenderer.material.SetFloat("_RongJie", Mathf.Lerp(0, 1, t));
            }

            yield return null;
        }

        Self.gameObject.SetActive(false);
    }
}

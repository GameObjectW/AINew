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

    private Coroutine co;

    public DieAction(AINew ai, string aniTriggerString, float DieTime, NavMeshAgent nv, Transform self)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        this.DieTime = DieTime;
        NV = nv;
        Self = self;
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
        Self.localScale = Vector3.one;
        //Ani.SetTrigger(AniTriggerString);
        Debug.Log("死亡··············");
        yield return new WaitForSeconds(DieTime);
        
        Self.localScale = new Vector3(0.3f,1,0.3f);
        Self.gameObject.SetActive(false);
    }
}

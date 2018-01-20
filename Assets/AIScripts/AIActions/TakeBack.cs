using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TakeBack : ITriggerAIAction {
    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float OncTime;
    private NavMeshAgent NV;
    private Transform self;
    private Transform target;

    private Coroutine co;

    public TakeBack(AINew ai, string aniTriggerString, float oncTime, NavMeshAgent nv, Transform self, Transform target)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        OncTime = oncTime;
        NV = nv;
        this.self = self;
        this.target = target;
    }

    public void TriggerAction()
    {
        if (co!=null)
        {
            return;
        }
        co=UpdateManager.Ins.StartCoroutineCustom(Start());
    }
    public bool CancelAction(AIState newState)
    {
        if (!ai.GetComplete() && (newState != AIState.Die && newState != AIState.Idle))
        {
            return false;
        }
        UpdateManager.Ins.StopCoroutineCustom(co);
        co = null;
        ai.SetComplete(true);
        return true;
    }
    IEnumerator Start()
    {
        ai.SetComplete(false);
        NV.isStopped = false;
        NV.SetDestination(self.position + (self.position - target.position).normalized * 3);
        //Ani.SetTrigger(AniTriggerString);
        Debug.Log("后退中··············");
        yield return new WaitForSeconds(OncTime);
        ai.SetComplete(true);

    }
}

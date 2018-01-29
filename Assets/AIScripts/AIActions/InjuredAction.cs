using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InjuredAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float OncTime;
    private NavMeshAgent NV;

    private Coroutine co;

    public InjuredAction(AINew ai, string aniTriggerString, float oncTime, NavMeshAgent nv, Animator ani)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        OncTime = oncTime;
        NV = nv;
        Ani = ani;
    }

    public void TriggerAction()
    {
        if (co!=null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
    }

    public bool CancelAction(AIState newState)
    {
        if (!ai.GetComplete() && (newState != AIState.Injured && newState != AIState.Die && newState != AIState.Idle))
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
        NV.isStopped = true;
        ai.SetComplete(false);
        Ani.SetTrigger(AniTriggerString);
        Debug.Log("被击中··············");
        yield return new WaitForSeconds(OncTime);
        ai.SetComplete(true);

    }
}

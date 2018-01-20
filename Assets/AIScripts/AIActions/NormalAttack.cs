using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NormalAttack : ITriggerAIAction
{
    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private float OncTime;
    private NavMeshAgent NV;

    private Coroutine co;

    public NormalAttack(AINew ai, string aniTriggerString, float oncTime, NavMeshAgent nv)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        OncTime = oncTime;
        NV = nv;
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
        if (!ai.GetComplete() && (newState != AIState.Die && newState != AIState.Idle && newState != AIState.Injured))
        {
            return false;
        }
        UpdateManager.Ins.StopCoroutineCustom(co);
        ai.SetComplete(true);
        co = null;
        return true;
    }

    IEnumerator Start()
    {
        NV.avoidancePriority = 50;
        NV.isStopped = true;
        while (true)
        {
            ai.SetComplete(false);
            //Ani.SetTrigger(AniTriggerString);
            Debug.Log("攻击中··············");
            yield return new WaitForSeconds(OncTime);
            ai.SetComplete(true);
            yield return new WaitForSeconds(0.1f);
            
        }
    }
}

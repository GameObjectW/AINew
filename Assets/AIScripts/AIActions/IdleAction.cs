using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleAction : ITriggerAIAction {
    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;

    private NavMeshAgent NV;

    private Coroutine co;

    private bool isFinishInit=false;

    public IdleAction(AINew ai, string aniTriggerString, NavMeshAgent nv)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        NV = nv;
    }

    public void TriggerAction()
    {
        if (isFinishInit||co != null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
    }

    public bool CancelAction(AIState newState)
    {
        if (!ai.GetComplete())
        {
            Debug.Log("初始化未完成。。。。。。。。。");
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
        NV.enabled = true;
        NV.avoidancePriority = 50;
        //Ani.SetTrigger(AniTriggerString);
        Debug.Log("初始化中··············");

        ai.SetComplete(true);
        yield return null;
    }
}

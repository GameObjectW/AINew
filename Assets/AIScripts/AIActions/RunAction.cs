using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private Transform Target;
    private NavMeshAgent NV;

    private Coroutine co;

    public RunAction(AINew ai, string aniTriggerString, Transform Target, NavMeshAgent nv)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        this.Target = Target;
        NV = nv;
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
        return true;
    }

    IEnumerator Start()
    {
        NV.isStopped = false;
        NV.avoidancePriority = 1;
        //Ani.SetTrigger(AniTriggerString);
        while (true)
        {

            //Ani.SetTrigger(AniTriggerString);
            Debug.Log("奔跑中··············");
            NV.SetDestination(Target.position);

            yield return null;

        }
        
    }
}

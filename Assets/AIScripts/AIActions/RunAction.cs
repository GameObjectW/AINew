using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAction : ITriggerAIAction {

    private Animator Ani;
    private AINew ai;
    private string AniTriggerString;
    private Transform Target;
    private NavMeshAgent _nv;

    private Coroutine co;

    public RunAction(AINew ai, string aniTriggerString, NavMeshAgent nv, Animator ani)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        _nv = nv;
        Ani = ani;
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
        _nv.isStopped = false;
        _nv.avoidancePriority = 50;
        Ani.SetTrigger(AniTriggerString);
        while (true)
        {

            
           // Debug.Log("奔跑中··············");
            _nv.SetDestination(ai.Target.position);

            yield return null;
            Ani.ResetTrigger(AniTriggerString);
        }
        
    }
}

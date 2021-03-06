﻿using System.Collections;
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

    public TakeBack(AINew ai, string aniTriggerString, NavMeshAgent nv, Transform self)
    {
        this.ai = ai;
        AniTriggerString = aniTriggerString;
        NV = nv;
        this.self = self;

    }

    public void TriggerAction()
    {
        if (co!=null||ai.Target==null)
        {
            return;
        }
        co = UpdateManager.Ins.StartCoroutineCustom(Start());
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
        NV.SetDestination(self.position + (self.position - ai.Target.position).normalized *20);
        
        //Ani.SetTrigger(AniTriggerString);
        Debug.Log("后退中··············");
        yield return new WaitUntil(()=>ai.DistanceSelfToTarget(ai.Target)>ai.AttackDistance+1f);
        ai.SetComplete(true);

    }
}
